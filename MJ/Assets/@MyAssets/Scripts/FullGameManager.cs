using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FullGameManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs;

    public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
    {

        public ulong clientId;
        public int playerType;
        public bool isReady;

        public PlayerData(ulong id, int type, bool ready)
        {
            clientId = id;
            playerType = type;
            isReady = ready;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref playerType);
            serializer.SerializeValue(ref isReady);
        }
        public bool Equals(PlayerData other)
        {
            return clientId == other.clientId && playerType == other.playerType && isReady == other.isReady;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(clientId, playerType, isReady);
        }
    }

    public enum GAME_STATES
    {
        Initial = 0,
        Lobby = 1,
        Player = 2,
        Main = 3,
        End = 4
    }

    public GAME_STATES gameState;

    public Transform spawnContrabandista;
    public Transform spawnPolicia;

    public NetworkList<PlayerData> playerDataList;

    public static FullGameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        gameState = GAME_STATES.Lobby;

        playerDataList = new NetworkList<PlayerData>();
    }

    public void SelectPlayer(int player)
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        SelectPlayerRpc(localClientId, player);
    }

    [Rpc(SendTo.Server)]
    public void SelectPlayerRpc(ulong clientId, int playerType)
    {
        for (int i = 0; i < playerDataList.Count; i++)
        {
            if (playerDataList[i].clientId == clientId)
            {
                playerDataList[i] = new PlayerData(clientId, playerType, false);
                return;
            }
        }
        playerDataList.Add(new PlayerData(clientId, playerType, false));

    }

    public void GoToGame()
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        GoToGameRpc(localClientId);
    }

    [Rpc(SendTo.Server)]
    public void GoToGameRpc(ulong clientId)
    {
        for (int i = 0; i < playerDataList.Count; i++)
        {
            if (playerDataList[i].clientId == clientId)
            {
                PlayerData newPlayerData = new PlayerData(playerDataList[i].clientId, playerDataList[i].playerType, true);
                playerDataList[i] = newPlayerData;
                break;
            }
        }

        if (NetworkManager.ConnectedClientsList.Count > 1 && playerDataList.Count > 1)
        {
            bool allReady = true;

            for (int i = 0; i < playerDataList.Count; i++)
            {
                if (!playerDataList[i].isReady)
                {
                    allReady = false;
                    break;
                }
            }

            if (allReady)
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete += GameSceneLoaded;

                NetworkManager.Singleton.SceneManager.LoadScene(GAME_STATES.Main.ToString(), LoadSceneMode.Single);
            }
        }
    }

    private void GameSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (clientId != NetworkManager.ServerClientId) return;

        gameState = GAME_STATES.Main;

        foreach (PlayerData playerData in playerDataList)
        {
            // Determinar el spawn correcto
            Transform spawnPoint = spawnContrabandista; // por defecto

            if (playerData.playerType == 1)
                spawnPoint = spawnPolicia;

            // Validación para evitar errores si el spawn aún es null
            if (spawnPoint == null)
            {
                Debug.LogWarning($"SpawnPoint para el jugador {playerData.playerType} no está asignado.");
                spawnPoint = new GameObject("FallbackSpawn").transform; // Crear uno en (0,0,0)
            }

            GameObject playerGo = Instantiate(
                playerPrefabs[playerData.playerType],
                spawnPoint.position,
                spawnPoint.rotation
            );

            playerGo.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerData.clientId, true);
        }
    }

    public int GetPlayerType(ulong clientId)
    {
        foreach (var player in playerDataList)
        {
            if (player.clientId == clientId)
                return player.playerType;
        }

        return 0; // Default
    }

}
