using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Base framework for ui in mini games
public class GameUI : MonoBehaviour
{
    public Sprite Active_Img;
    public Sprite Inactive_Img;
    public GameObject Splash_PNL;
    public GameObject Menu_PNL;
    public GameObject Help_PNL;
    public GameObject Playing_PNL;
    public GameObject Paused_PNL;
    public GameObject Gameover_PNL;
    public Text HighScore_Text;
    public Text Score_Text;
    public Text Timer_Text;
    public Text GameOverScore_Text;
    public Text GameOverToken_Text;

    /// <summary> Turns off all UI panels </summary>
    public void PanelsOff()
    {
        Splash_PNL.SetActive(false);
        Menu_PNL.SetActive(false);
        Help_PNL.SetActive(false);
        Playing_PNL.SetActive(false);
        Paused_PNL.SetActive(false);
        Gameover_PNL.SetActive(false);
    }
}
