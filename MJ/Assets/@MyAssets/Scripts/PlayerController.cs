using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    float moveSpeed = 3f;

    private Animator animator;

    [SerializeField] private InputActionReference moveAction;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (!IsOwner) return;
        Debug.Log("Player Start IsServer: " + IsServer);
        Debug.Log("Player Start IsClient: " + IsClient);
        Debug.Log("Player Start isHost: " + IsHost);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        Debug.Log("Player Spawn IsServer: " + IsServer);
        Debug.Log("Player Spawn IsClient: " + IsClient);
        Debug.Log("Player Spawn isHost: " + IsHost);

        base.OnNetworkSpawn();
    }

    void Update()
    {
        if (!this.IsOwner) return;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
