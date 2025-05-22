using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunShooter : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float fireRate = 0.5f;

    private float nextFireTime = 0f;
    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"[OnNetworkSpawn] GunShooter activo. IsOwner: {IsOwner}, IsServer: {IsServer}, ClientId: {OwnerClientId}");
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (IsOwner)
        {
            isHeld = true;
            Debug.Log("Pistola agarrada por el owner.");
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (IsOwner)
        {
            isHeld = false;
            Debug.Log("Pistola soltada por el owner.");
        }
    }

    private void Update()
    {
        if (!IsOwner || !isHeld) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Debug.Log("Click detectado, disparando...");
            Debug.Log($"[CLIENT] IsOwner: {IsOwner}, IsHost: {IsHost}, IsServer: {IsServer}");
            Debug.Log($"[CLIENT] OwnerClientId: {OwnerClientId}, LocalClientId: {NetworkManager.Singleton.LocalClientId}");
            Debug.Log($"[CLIENT] IsSpawned: {GetComponent<NetworkObject>().IsSpawned}");

            if (firePoint == null)
            {
                Debug.LogError("firePoint no está asignado en el Inspector.");
                return;
            }

            ShootServerRpc(firePoint.position, firePoint.rotation);
        }
#endif
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"ServerRpc llamado por: {rpcParams.Receive.SenderClientId}");

        if (rpcParams.Receive.SenderClientId != OwnerClientId)
        {
            Debug.LogWarning("Cliente no autorizado intentó disparar.");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<NetworkObject>().Spawn();

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.VelocityChange);
        }
    }
}
