using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class MyNetworkManager : MonoBehaviour
{

    //    public void StartServer()
    //    {
    //        NetworkManager.Singleton.StartServer();
    //    }

    private const int MAX_PLAYER_AMOUNT = 2;
    public UnityEvent OnFailedToJoin = new UnityEvent();

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
        //FullGameManager.Instance.gameStates = FullGameManager.GAME_STATES.PlayerSelectionScene;
        //NetworkManager.Singleton.SceneManager.LoadScene(FullGameManager.Instance.gameStates.ToString(), LoadSceneMode.Single);
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        //if (SceneManager.GetActiveScene().name == FullGameManager.GAME_STATES.MainScene.ToString() || NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) response.Approved = false;
        //else response.Approved = true;
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
        //FullGameManager.Instance.gameStates = FullGameManager.GAME_STATES.InitialScene;
        //SceneManager.LoadScene(FullGameManager.Instance.gameStates.ToString());
    }
}
