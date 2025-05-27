using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class GunController : NetworkBehaviour
{
    [Header("Configuración Básica")]
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;
    public float bulletLifetime = 3f;

    [Header("Efectos Sin Partículas")]
    public AudioSource gunAudio;
    public Light muzzleFlashLight;
    public float flashDuration = 0.1f;
    public float maxLightIntensity = 5f;

    private XRGrabInteractable grabInteractable;
    private float nextFireTime;
    private float flashTimer;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        Debug.Log("[GunController] Inicializando arma");

        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("[GunController] No se encontró XRGrabInteractable en el objeto");
        }
        else
        {
            grabInteractable.activated.AddListener(OnTriggerPulled);
            Debug.Log("[GunController] Listener de trigger añadido");
        }

        if (gunAudio == null)
        {
            Debug.LogError("[GunController] No se asignó el AudioSource en el inspector");
        }

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = false;
        }
        else
        {
            Debug.LogWarning("[GunController] No hay luz de fogonazo asignada");
        }
    }

    private void Update()
    {
        if (IsOwner && Input.GetMouseButtonDown(0))
        {
            Debug.Log("[GunController] Click izquierdo detectado (Owner)");
            Shoot();
        }

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
        Debug.Log("[GunController] Trigger del controlador VR activado");
        if (!IsOwner)
        {
            Debug.LogWarning("[GunController] Intento de disparo pero no somos owners");
            return;
        }
        Shoot();
    }

    private void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            Debug.Log("[GunController] Disparo válido - Llamando al ServerRpc");

            if (bulletSpawnPoint == null)
            {
                Debug.LogError("[GunController] ¡bulletSpawnPoint no está asignado!");
                return;
            }

            FireBulletServerRpc(bulletSpawnPoint.position, bulletSpawnPoint.forward);
            nextFireTime = Time.time + fireRate;
        }
        else
        {
            Debug.Log($"[GunController] En enfriamiento. Tiempo restante: {nextFireTime - Time.time}s");
        }
    }

    [ServerRpc]
    private void FireBulletServerRpc(Vector3 position, Vector3 direction)
    {
        Debug.Log("[SERVER] Recibido FireBulletServerRpc");

        if (bulletPrefab == null)
        {
            Debug.LogError("[SERVER] ¡bulletPrefab no está asignado!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.LookRotation(direction));

        if (bullet == null)
        {
            Debug.LogError("[SERVER] Fallo al instanciar la bala");
            return;
        }

        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("[SERVER] La bala no tiene NetworkObject");
            Destroy(bullet);
            return;
        }

        netObj.Spawn();

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[SERVER] La bala no tiene Rigidbody");
            return;
        }

        rb.velocity = direction * bulletSpeed;

        Destroy(bullet, bulletLifetime);

        PlayGunEffectsClientRpc();
    }

    [ClientRpc]
    private void PlayGunEffectsClientRpc()
    {
        if (gunAudio != null)
        {
            gunAudio.Play(); // Usa el AudioClip asignado en el AudioSource
        }

        StartMuzzleFlash();

        PlayMuzzleFlashClientRpc();
    }

    private void StartMuzzleFlash()
    {
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            muzzleFlashLight.intensity = maxLightIntensity;
            flashTimer = flashDuration;

            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            flashCoroutine = StartCoroutine(DecayMuzzleFlash());
        }
    }

    private IEnumerator DecayMuzzleFlash()
    {
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
    }

    [ClientRpc]
    private void PlayMuzzleFlashClientRpc()
    {
        if (!IsOwner)
        {
            StartMuzzleFlash();
        }
    }

    private void OnDrawGizmos()
    {
        if (bulletSpawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bulletSpawnPoint.position, 0.05f);
            Gizmos.DrawLine(bulletSpawnPoint.position, bulletSpawnPoint.position + bulletSpawnPoint.forward * 0.5f);
        }
    }
}
