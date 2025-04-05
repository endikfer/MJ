using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialSceneManager : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);
        if (FullGameManager.Instance != null)
            Destroy(FullGameManager.Instance.gameObject);
        /*Debugger[] debugger = FindObjectsOfType<Debugger>();
        if (debugger.Length > 1)
            Destroy(debugger[1].gameObject);*/
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
