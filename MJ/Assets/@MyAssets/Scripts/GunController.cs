using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GunController : NetworkBehaviour
{
    [Header("Configuración Básica")]
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;

    [Header("Efectos Visuales")]
    public Light muzzleFlashLight;
    public float flashDuration = 0.07f;
    public float maxLightIntensity = 5f;
    public ParticleSystem muzzleFlashParticles;

    [Header("Efectos de Audio")]
    public AudioSource gunAudio;
    [Range(0, 1)] public float volume = 0.8f;

    private XRGrabInteractable grabInteractable;
    private float nextFireTime;
    private float flashTimer;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (gunAudio == null) gunAudio = GetComponent<AudioSource>();
        if (muzzleFlashLight != null) muzzleFlashLight.enabled = false;
    }

    private void Start()
    {
        grabInteractable.activated.AddListener(OnTriggerPulled);
    }

    private void Update()
    {
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0 && muzzleFlashLight != null)
            {
                muzzleFlashLight.enabled = false;
            }
        }
    }

    private void OnTriggerPulled(ActivateEventArgs arg)
    {
        if (!IsOwner) return;
        TryShoot();
    }

    private void TryShoot()
    {
        if (Time.time >= nextFireTime && bulletSpawnPoint != null)
        {
            FireBulletServerRpc(
                bulletSpawnPoint.position,
                bulletSpawnPoint.forward,
                bulletSpawnPoint.rotation
            );
            nextFireTime = Time.time + fireRate;
        }
    }

    [ServerRpc]
    private void FireBulletServerRpc(Vector3 position, Vector3 direction, Quaternion rotation)
    {
        if (!IsSpawned || bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        var netObj = bullet.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(OwnerClientId);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }

        PlayShootEffectsClientRpc();
    }

    [ClientRpc]
    private void PlayShootEffectsClientRpc()
    {
        StartCoroutine(HandleShootEffects());
    }

    private IEnumerator HandleShootEffects()
    {
        if (gunAudio != null && gunAudio.clip != null)
            gunAudio.PlayOneShot(gunAudio.clip, volume);

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            muzzleFlashLight.intensity = maxLightIntensity;
            flashTimer = flashDuration;
        }

        if (muzzleFlashParticles != null)
        {
            muzzleFlashParticles.Stop();
            muzzleFlashParticles.Play();
        }

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            if (muzzleFlashLight != null)
            {
                muzzleFlashLight.intensity = Mathf.Lerp(maxLightIntensity, 0, elapsed / flashDuration);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = false;
        }
    }
}
