using UnityEngine;
using Unity.Netcode;
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
    public AudioClip shootSound;
    public Light muzzleFlashLight;
    public float flashDuration = 0.1f;

    private XRGrabInteractable grabInteractable;
    private float nextFireTime;
    private AudioSource gunAudio;
    private float flashTimer;

    private void Awake()
    {
        Debug.Log("[GunController] Inicializando arma");

        grabInteractable = GetComponent<XRGrabInteractable>();
        gunAudio = GetComponent<AudioSource>();

        if (grabInteractable == null)
        {
            Debug.LogError("[GunController] No se encontró XRGrabInteractable en el objeto");
        }
        else
        {
            grabInteractable.activated.AddListener(OnTriggerPulled);
            Debug.Log("[GunController] Listener de trigger añadido");
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

        // Apagar la luz del fogonazo después del tiempo determinado
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

        Debug.Log($"[SERVER] Instanciando bala en posición: {position}");
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
        Debug.Log($"[SERVER] Bala spawneda con NetworkObjectId: {netObj.NetworkObjectId}");

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[SERVER] La bala no tiene Rigidbody");
            return;
        }

        rb.velocity = direction * bulletSpeed;
        Debug.Log($"[SERVER] Velocidad de bala establecida: {rb.velocity}");

        Destroy(bullet, bulletLifetime);
        Debug.Log($"[SERVER] Bala programada para destrucción en {bulletLifetime}s");

        PlayGunEffectsClientRpc();
    }

    [ClientRpc]
    private void PlayGunEffectsClientRpc()
    {
        Debug.Log("[CLIENTE] Recibido PlayGunEffectsClientRpc");

        // Sonido de disparo
        if (gunAudio != null && shootSound != null)
        {
            gunAudio.PlayOneShot(shootSound);
            Debug.Log("[CLIENTE] Sonido de disparo reproducido");
        }
        else
        {
            Debug.LogWarning("[CLIENTE] No se pudo reproducir sonido de disparo (gunAudio o shootSound null)");
        }

        // Fogonazo con luz
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            flashTimer = flashDuration;
            Debug.Log("[CLIENTE] Efecto de fogonazo activado");
        }
    }

    // Método para visualizar el punto de spawn en el editor
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