using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameBTNObject : MonoBehaviour
{
    public string GameName;

    public void Start()
    {
        MinigameData data = MinigameData.LoadGameData(GameName);
        if(data.IsActive())
        {
            gameObject.GetComponent<Image>().sprite = GameManager.Instance.MiniGame_Objects.First(o => o.name.Contains(GameName)).GetComponent<GameUI>().Active_Img;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = GameManager.Instance.MiniGame_Objects.First(o => o.name.Contains(GameName)).GetComponent<GameUI>().Inactive_Img;
        }
    }

    public void Clicked()
    {
        MinigameData data = MinigameData.LoadGameData(GameName);
        if (data.IsActive())
        {
            foreach (GameObject go in GameManager.Instance.MiniGame_Objects)
            {
                if (go.name == GameName + "_UI")
                {
                    GameObject GameUI = Instantiate(go, UIManager.Instance.MainCanvas.transform);
                    GameManager.Minigame = GameUI.GetComponent<BaseGame>();
                    GameManager.MGD = MinigameData.RetrieveData(GameName);
                }
            }
            GameManager.Instance.SetState(GameManager.GameState.Minigame);
        }
    }
}
