using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GunShooter : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform shotPoint;
    public InputActionProperty triggerAction;

    public float bulletSpeed = 10f;
    public float fireRate = 0.5f;

    private float lastShotTime;
    private XRGrabInteractable grabInteractable;
    private bool isHeldByLocalPlayer = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public override void OnNetworkSpawn()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Verificamos si el interactor pertenece al cliente local
        var interactorNetworkObject = args.interactorObject.transform.GetComponent<NetworkObject>();

        if (interactorNetworkObject != null && interactorNetworkObject.IsLocalPlayer)
        {
            isHeldByLocalPlayer = true;
            Debug.Log("El jugador local ha agarrado el arma");
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Sólo consideramos soltado si el arma ya NO está siendo agarrada por nadie
        if (grabInteractable.isSelected)
        {
            Debug.Log("Intento de liberar arma, pero sigue siendo agarrada, no cambio estado");
            return;
        }

        isHeldByLocalPlayer = false;
        Debug.Log("El jugador local soltó el arma");
    }

    void Update()
    {
        if (!IsOwner) return;

        // Por seguridad, chequeamos si el arma sigue seleccionada; si no, consideramos que no está sostenida
        if (!grabInteractable.isSelected)
        {
            isHeldByLocalPlayer = false;
            return;
        }

        if (!isHeldByLocalPlayer) return;

        bool triggerPressed = triggerAction.action.ReadValue<float>() > 0.5f;
        bool mouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if ((triggerPressed || mouseClicked) && Time.time >= lastShotTime + fireRate)
        {
            lastShotTime = Time.time;
            Debug.Log("DISPARO local");
            ShootServerRpc();
        }
    }

    [ServerRpc]
    void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log("Entró al ShootServerRpc");
        GameObject bullet = Instantiate(bulletPrefab, shotPoint.position, shotPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = shotPoint.forward * bulletSpeed;
        bullet.GetComponent<NetworkObject>().Spawn();
    }
}
