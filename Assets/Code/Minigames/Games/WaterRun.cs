using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterRun : BaseGame
{
    [Header("UI")]
    // Background image for menu
    [SerializeField]
    private Image BackGroundImage;
    // Radial image for the timer
    [SerializeField]
    private Image TimerRadial;
    // Color representing full or positive
    public Color FullTimeCol = Color.green;
    // Color representing empty or negative
    public Color NoTimeCol = Color.red;
    // UI script with access to UI elements of game
    [SerializeField]
    private GameUI UI;

    [Header("Pregame")]
    // pregame count down timer
    private float pregameTimer = 0f;
    // length of pregame cound down
    public float PregameTime = 3f;
    // Pregame countdown
    public int PregameCount = 0;
    // is pregame counting down
    private bool PregameCountdown = false;
    // Count down in pregame
    [SerializeField]
    private Text Countdown_Text;
    // Sequential colors of countdown
    [SerializeField]
    private Color[] Countdown_Colors;

    [Header("GamePlay")]
    // Duration the game is played for
    private float gameDuration = 0;
    // Current difficulty level of the came
    [SerializeField]
    private int difficulty = 1;
    // The difficulty step
    [SerializeField]
    private float difficultyCoeficiant = 0.2f;
    // Seconds till the difficulty increases
    [SerializeField]
    private float timeTillDifficultyInscreas = 20f;

    [Header("Game Objects")]
    // Prefab of Physical game world which contains all game elements needed for gameplay
    [SerializeField]
    private GameObject GameWorld_Prefab;
    private ScubaPlayerController Player;

    // The ratio os tokens you get for playing the game
    private float tokenRatio = 100f;

    void Start()
    {
        Cur_State = State.Splash;
        EnterState(State.Splash);
    }

    private void Init()
    {
        if (GameWorld != null)
        {
            Destroy(GameWorld);
        }
        GameWorld = Instantiate(GameWorld_Prefab) as GameObject;
        Player = GameWorld.GetComponent<LevelManager>().Player;
        GameWorld.GetComponent<LevelManager>().Init(this);
        Timer = 0;
        pregameTimer = PregameTime;
        difficulty = 1;
        Score = 0;
        UI.Score_Text.text = "0";
        PregameCount = 0;
        Countdown_Text.text = PregameTime.ToString();
        Countdown_Text.color = Countdown_Colors[PregameCount];
        HandleTimer();
    }

    /// <summary> returns the current state of the game </summary>
    public State GetState()
    {
        return Cur_State;
    }

    void Update()
    {
        switch (Cur_State)
        {
            case State.Splash:
                splashTimer += Time.deltaTime;
                if (splashTimer >= SplashTime)
                {
                    SetState(State.Menu);
                }
                break;
            case State.Menu:

                break;
            case State.Pregame:
                PregameTimer();
                break;
            case State.Playing:
                gameDuration += Time.deltaTime;

                HandleTimer();
                break;
            case State.Paused:

                break;
            case State.Gameover:

                break;
        }
    }

    public override void EnterState(State state)
    {
        switch (state)
        {
            case State.Splash:
                UI.Splash_PNL.SetActive(true);
                splashTimer = 0;
                SoundManager.Instance.PlayMusic("TOM_MainLoop", Music);
                break;
            case State.Menu:
                BackGroundImage.enabled = true;
                UI.Menu_PNL.SetActive(true);
                SetHighScore(GameManager.MGD.Highscore.ToString());
                Init();
                break;
            case State.Pregame:
                BackGroundImage.enabled = false;
                UI.Playing_PNL.SetActive(true);
                Countdown_Text.gameObject.SetActive(true);
                break;
            case State.Playing:
                BackGroundImage.enabled = false;
                UI.Playing_PNL.SetActive(true);
                Countdown_Text.gameObject.SetActive(false);
                Player.RB.simulated = true;
                break;
            case State.Paused:
                UI.Paused_PNL.SetActive(true);
                Time.timeScale = 0;
                break;
            case State.Gameover:
                GameManager.Instance.analytics.LogTiming(new TimingHitBuilder().SetTimingCategory(Name).SetTimingInterval((long)gameDuration).SetTimingName("Playtime"));
                GameManager.Instance.analytics.LogEvent(new EventHitBuilder().SetEventCategory(Name).SetEventAction("Points").SetEventValue(Score));
                GameManager.Instance.analytics.LogEvent(new EventHitBuilder().SetEventCategory(Name).SetEventAction("Tokens").SetEventValue((long)(Score * tokenRatio)));
                UI.Gameover_PNL.SetActive(true);
                BackGroundImage.enabled = true;
                UI.GameOverScore_Text.text = Score.ToString();
                UI.GameOverToken_Text.text = (Score * tokenRatio).ToString();
                UpdateHighScore(Score);
                UserUtilities.AllocatePoints((int)(Score * tokenRatio));
                SoundManager.Instance.PlaySound("CoinCollect");
                break;
        }
    }

    public override void ExitState(State state)
    {
        switch (state)
        {
            case State.Splash:
                UI.Splash_PNL.SetActive(false);
                break;
            case State.Menu:
                UI.Menu_PNL.SetActive(false);
                break;
            case State.Pregame:
                UI.Playing_PNL.SetActive(false);
                break;
            case State.Playing:
                UI.Playing_PNL.SetActive(false);
                break;
            case State.Paused:
                UI.Paused_PNL.SetActive(false);
                Time.timeScale = 1;
                break;
            case State.Gameover:
                UI.Gameover_PNL.SetActive(false);
                break;
        }
    }

    /// <summary> Sets the game to a state given also calling exit state then enterstate </summary>
    public override void SetState(State newState)
    {
        Next_State = newState;
        ExitState(Cur_State);
        base.SetState(Next_State);
        EnterState(Next_State);
    }

    /// <summary> Closes the game </summary>
    public override void CloseGame()
    {
        ResetGame();
        gameObject.SetActive(false);
    }

    /// <summary> Resets the game </summary>
    public void ResetGame()
    {
        Score = 0;
        UI.Score_Text.text = Score.ToString();
        //Timer = GameTime;
        UI.Timer_Text.text = Timer.ToString("00");
    }

    /// <summary> Take the game to play mode </summary>
    public void Play_BTN()
    {
        SoundManager.Instance.PlaySound("HRCA_button");
        SetState(State.Pregame);
    }

    /// <summary> Pauses and unpauses the game </summary>
    public void Pause_BTN()
    {
        SoundManager.Instance.PlaySound("HRCA_button");
        if (GetState() == State.Playing || GetState() == State.Paused)
        {
            if (GetState() == State.Paused)
            {
                SetState(State.Playing);
            }
            else
            {
                SetState(State.Paused);
            }
        }
    }

    /// <summary> Displays help panel </summary>
    public void Help_BTN()
    {
        SoundManager.Instance.PlaySound("HRCA_button");
        if (UI.Help_PNL.activeSelf == false)
        {
            UI.Help_PNL.SetActive(true);
            gameObject.GetComponentInChildren<HelpScreen>().Init();
        }
        else
        {
            UI.Help_PNL.SetActive(false);
        }
    }

    /// <summary> Closes game </summary>
    public void Exit_BTN()
    {
        SoundManager.Instance.PlaySound("HRCA_button");
        GameManager.Instance.SetState(GameManager.GameState.Games);
        Destroy(GameWorld);
        Destroy(gameObject);
    }

    /// <summary> Sets game to menu </summary>
    public void Menu_BTN()
    {
        SoundManager.Instance.PlaySound("HRCA_button");
        if (GetState() == State.Paused)
        {
            UI.Playing_PNL.SetActive(false);
        }
        SetState(State.Menu);
    }


    // Pregame Timer with countdown 
    private void PregameTimer()
    {
        if (!PregameCountdown)
        {
            PregameCountdown = true;
        }
        else if (PregameCountdown)
        {
            pregameTimer -= Time.deltaTime;
            PregameCount = (int)(pregameTimer);
            Countdown_Text.text = (PregameCount > 0)?((PregameCount).ToString()):("Go");
            Countdown_Text.color = Countdown_Colors[PregameCount];

            if (pregameTimer <= 0)
            {
                pregameTimer = PregameTime;
                SetState(State.Playing);
                PregameCountdown = false;
            }
        }
    }

    /// <summary> Handles the timer and the radial </summary>
    private void HandleTimer()
    {
        Timer += Time.deltaTime;

        if (Timer >= timeTillDifficultyInscreas)
        {
            Timer = 0;
            difficulty++;
        }
        UI.Timer_Text.text = difficulty.ToString();
        TimerRadial.fillAmount = Timer / timeTillDifficultyInscreas;
        TimerRadial.color = Color.Lerp(FullTimeCol, NoTimeCol, TimerRadial.fillAmount);
    }

    // returns the difficulty with coefficient
    public float Difficulty()
    {
        return difficulty * difficultyCoeficiant;
    }

    // Increases score by one
    public void IncreaseScore()
    {
        Score++;
        UI.Score_Text.text = Score.ToString();
    }

    /// <summary> Sets the high score text </summary>
    public void SetHighScore(string value)
    {
        UI.HighScore_Text.text = value;
    }

    /// <summary> Updates and saves data if score is larger </summary>
    private void UpdateHighScore(int score)
    {
        if (GameManager.MGD.Highscore < score)
        {
            GameManager.MGD.Highscore = score;
            GameManager.MGD.SaveGameData();
        }
    }
}
