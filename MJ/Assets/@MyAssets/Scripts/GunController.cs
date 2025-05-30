using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GunController : NetworkBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;

    public Light muzzleFlashLight;
    public float flashDuration = 0.07f;
    public float maxLightIntensity = 5f;
    public ParticleSystem muzzleFlashParticles;

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
        if (Time.time < nextFireTime) return;

        Collider shooterCollider = GetComponentInParent<Collider>();
        NetworkObject shooterNetObj = shooterCollider?.GetComponent<NetworkObject>();

        if (shooterNetObj != null)
        {
            RequestFireServerRpc(
                bulletSpawnPoint.position,
                bulletSpawnPoint.forward,
                bulletSpawnPoint.rotation,
                shooterNetObj.NetworkObjectId
            );
            nextFireTime = Time.time + fireRate;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestFireServerRpc(Vector3 pos, Vector3 dir, Quaternion rot, ulong shooterColliderId, ServerRpcParams rpcParams = default)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos, rot);
        var netObj = bullet.GetComponent<NetworkObject>();
        netObj.Spawn(true);

        if (bullet.TryGetComponent(out BulletController bulletCtrl))
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(shooterColliderId, out NetworkObject shooterObj))
            {
                Collider col = shooterObj.GetComponent<Collider>();
                bulletCtrl.SetShooter(rpcParams.Receive.SenderClientId, col);
            }
        }

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = dir * bulletSpeed;
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
        gunAudio?.PlayOneShot(gunAudio.clip, volume);

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            muzzleFlashLight.intensity = maxLightIntensity;
            flashTimer = flashDuration;
        }

        muzzleFlashParticles?.Play();

        yield return new WaitForSeconds(flashDuration);

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = false;
        }
    }
}   