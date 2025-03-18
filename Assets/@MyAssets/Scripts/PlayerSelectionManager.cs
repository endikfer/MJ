using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    public void SelectPlayer(int player)
    {
        FullGameManager.Instance.SelectPlayer(player);
    }

    public void GoToGame()
    {
        FullGameManager.Instance.GoToGame();
    }
}
