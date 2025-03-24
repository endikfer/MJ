using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MyNetworkManager : MonoBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 2;
    public UnityEvent OnFailedToJoin = new UnityEvent();

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();       
        FullGameManager.Instance.gameState = FullGameManager.GAME_STATES.Player;
        NetworkManager.Singleton.SceneManager.LoadScene(FullGameManager.Instance.gameState.ToString(), LoadSceneMode.Single);
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (SceneManager.GetActiveScene().name == FullGameManager.GAME_STATES.Main.ToString() || NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) response.Approved = false;
        else response.Approved = true;
    }

    public void StartClient()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void OnClientDisconnectCallback(ulong obj)
    {
        OnFailedToJoin.Invoke();
    }

    public void GoBack()
    {
        FullGameManager.Instance.gameState = FullGameManager.GAME_STATES.Initial;
        SceneManager.LoadScene(FullGameManager.Instance.gameState.ToString());
    }
}
