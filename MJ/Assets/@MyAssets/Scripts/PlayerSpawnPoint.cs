using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public enum SpawnType { Contrabandista, Policia }
    public SpawnType spawnType;

    private void OnEnable()
    {
        if (FullGameManager.Instance == null) return;

        switch (spawnType)
        {
            case SpawnType.Contrabandista:
                FullGameManager.Instance.spawnContrabandista = transform;
                break;
            case SpawnType.Policia:
                FullGameManager.Instance.spawnPolicia = transform;
                break;
        }
    }
}
