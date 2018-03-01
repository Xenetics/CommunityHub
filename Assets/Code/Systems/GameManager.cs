using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }

    // State
    public enum GameState { Splash, StartPage, Login, Register, CheckIn, Map, Games, Events, Profile, Minigame }
    [NonSerialized]
    public static GameState Cur_State = GameState.Splash;

    // Cloud Support
    public static AzureHelper Azure;

    // General
    public enum Organizations { MPL, TOM, Conservation, Heritage }

    // Analytics
    public GoogleAnalyticsV4 analytics;

    [Header("Mini Games")]
    [SerializeField]
    public GameObject[] MiniGame_Objects;
    /// <summary> Current mini game </summary>
    public static BaseGame Minigame;
    /// <summary> Current mini games data </summary>
    public static MinigameData MGD;
    /// <summary> How long minigames stay active for </summary>
    public static TimeSpan MinigameActiveTime = new TimeSpan(24, 0, 0);
    /// <summary> Organizations that have minigames </summary>
    public List<Organizations> MiniGameEnabledOrgs;
    /// <summary> Organizations that can unlock minigames via pin </summary>
    public List<Organizations> MinigamePinUnlock;
    [Header("Trivia")]
    /// <summary> Organizations that have Trivia enabled for pins </summary>
    public List<Organizations> TriviaEnabledOrgs;
    /// <summary> Trivia Game script access </summary>
    public Quizgame TriviaGame;
    /// <summary> TriviaGame object in scene </summary>
    public GameObject TriviaGameObject;

    [Header("Map")]
    /// <summary> Radius of a circle around the player pin </summary>
    public double playerPinRadius = 10f;
    /// <summary> The max radus of the circle around a pin in which you can be detected </summary>
    public double pinRadius = 30f;
    /// <summary> Represents the time when the epoch started counting </summary>
    public DateTime epochStart;
    /// <summary> The time in milliseconds between the player pin updates </summary>
    private long m_playerTick = 100;
    /// <summary> The epoch time in milliseconds at the time of the last player pin tick </summary>
    private long m_playerLastTick = 0;
    /// <summary> The time in milliseconds between the pin updates </summary>
    private long m_pinTick = 100;
    /// <summary> The epoch time in milliseconds at the time of the last pin tick </summary>
    private long m_pinLastTick = 0;
    /// <summary> The epoch time in milliseconds at the time of the last token pin tick </summary>
    private long m_tokenPinLastTick = 0;
    /// <summary> The last index point that was used during the last pin update loop </summary>
    private int m_lastPlace = 0;
    /// <summary> The last index point that was used during the last token pin update loop </summary>
    private int m_TokenLastPlace = 0;
    /// <summary> The amount of pins that the update loop will updater per loop </summary>
    private int pinsPerUpdate = 5;
    /// <summary> The token jackpot multiplier on the jackpot tokens </summary>
    private int jackpotMultiplier = 30;


    void Start ()
    {
        epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Azure = new AzureHelper("mplpins");
        analytics.StartSession();
        EnterState(Cur_State);
    }

    private void OnApplicationQuit()
    {
        analytics.StopSession();
    }

    void Update ()
    {
        switch (Cur_State)
        {
            case GameState.Splash:

                break;
            case GameState.StartPage:

                break;
            case GameState.Login:

                break;
            case GameState.Register:

                break;
            case GameState.CheckIn:
                if (!NetCheck()) return;
                break;
            case GameState.Map:
                if (!NetCheck()) return;
                long now = (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
                PlayerUpdate(now);
                PinUpdate(now);
                TokenPinUpdate(now);
                break;
            case GameState.Games:
                if (!NetCheck()) return;
                break;
            case GameState.Events:
                if (!NetCheck()) return;
                break;
            case GameState.Profile:

                break;
            case GameState.Minigame:
                
                break;
        }
    }

    public void SetState(GameState newState)
    {
        ExitState(Cur_State, newState);
        Cur_State = newState;
        EnterState(Cur_State);
    }

    public void EnterState(GameState state)
    {
        UIManager.Instance.EnterState();
        switch (state)
        {
            case GameState.Splash:

                break;
            case GameState.StartPage:

                break;
            case GameState.Login:

                break;
            case GameState.Register:

                break;
            case GameState.CheckIn:

                break;
            case GameState.Map:
                TriviaGame.Init();
                break;
            case GameState.Games:

                break;
            case GameState.Events:
                
                break;
            case GameState.Profile:

                break;
            case GameState.Minigame:

                break;
        }
    }

    public void ExitState(GameState state, GameState nextState)
    {
        switch (state)
        {
            case GameState.Splash:
                SoundManager.Instance.PlayMusic("MainTheme");
                break;
            case GameState.StartPage:

                break;
            case GameState.Login:

                break;
            case GameState.Register:

                break;
            case GameState.CheckIn:

                break;
            case GameState.Map:

                break;
            case GameState.Games:

                break;
            case GameState.Events:

                break;
            case GameState.Profile:

                break;
            case GameState.Minigame:
                SoundManager.Instance.PlayMusic("MainTheme");
                break;
        }
        UIManager.Instance.ExitState(nextState);
    }

    /// <summary> Checks internet connectivity and displays message if it is faulty </summary>
    public bool NetCheck()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UIManager.Instance.DisplayNotification(true, "Cannot connect. Please ensure wifi or mobile data connection.");
            return false;
        }
        return true;
    }

#region Map
    /// <summary> Handles when a pin is clicked </summary>
    public void Pin_Click(OnlineMapsMarkerBase marker)
    {
        try
        {
            Vector2 pos = OnlineMapsLocationService.instance.position;
            Vector2 markerPos = marker.position;

            double pinDist = PinUtilities.DistanceInMeters(pos.y, pos.x, markerPos.y, markerPos.x);
            if (pinDist < pinRadius)
            {
                PointData pd = PinUtilities.GetPointData(marker.tags, PinUtilities.PointDatas);

                if (PinUtilities.GetTypeFromTag(marker.tags) != "Tokens")
                {
                    UserUtilities.AllocatePoints(pd._Value);
                    UIManager.Instance.IndicateScore(pd._Value, true);

                    SoundManager.Instance.PlaySound("CoinCollect");

                    long now = (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
                    PinUtilities.pinDeltas[marker.label] = now;

                    pd.RemovePin();

                    PinUtilities.SavePins();
                    UserUtilities.Save();

                    Organizations pinOrg = (Organizations)Enum.Parse(typeof(Organizations), pd._Type.ToString());

                    if (MinigamePinUnlock.Contains(pinOrg) && MiniGameEnabledOrgs.Contains(pinOrg))
                    {
                        Unlock(pinOrg);
                    }

                    if (TriviaEnabledOrgs.Contains(pinOrg))
                    {
                        TriviaGame.CheckForTrivia(marker.label);
                    }

                    return;
                }
                else
                {
                    pd = PinUtilities.GetPointData(marker.tags, PinUtilities.TokenPointDatas);
                    TokenPinActivate(pd, pd._Position.Latitude.ToString() + pd._Position.Longitude.ToString());
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    // Adds and activates a pin to the map for a location point
    public void AddPin(PointData point)
    {
        int index = PinUtilities.PointDatas.IndexOf(point);
        if (point._Type != PointData.PinType.Tokens)
        {
            System.Random rand = new System.Random();
            if (rand.Next(0, 100) < PinUtilities.percentJackpot)
            {
                PinUtilities.PointDatas[index]._Jackpot = true;
            }
            else
            {
                PinUtilities.PointDatas[index]._Jackpot = false;
            }
        }
        else
        {
            PinUtilities.PointDatas[index]._Jackpot = false;
        }
        point._Pin.enabled = true;
        OnlineMaps.instance.Redraw();
    }

    // Adds and activates a token pin to the map for a location point
    public void AddTokenPin(PointData point)
    {
        int index = PinUtilities.TokenPointDatas.IndexOf(point);
        if (point._Type != PointData.PinType.Tokens)
        {
            System.Random rand = new System.Random();
            if (rand.Next(0, 100) < PinUtilities.percentJackpot)
            {
                PinUtilities.TokenPointDatas[index]._Jackpot = true;
            }
            else
            {
                PinUtilities.TokenPointDatas[index]._Jackpot = false;
            }
        }
        else
        {
            PinUtilities.TokenPointDatas[index]._Jackpot = false;
        }
        point._Pin.enabled = true;
        OnlineMaps.instance.Redraw();
        
    }

    private void PlayerUpdate(long thisTick)
    {
        if(thisTick > m_playerLastTick + m_playerTick)
        {
            m_playerLastTick = (long)thisTick;
        }
    }

    // Handles update for normal pins
    private void PinUpdate(long thisTick)
    {
        if (thisTick > m_pinLastTick + m_pinTick)
        {
            int thisCount = 0;
            Vector2 pos = OnlineMapsLocationService.instance.position;
            for (int i = 0 + m_lastPlace; i < PinUtilities.PointDatas.Count && thisCount < pinsPerUpdate; ++i, ++m_lastPlace, ++thisCount)
            {
                int index = i;
                if (PinUtilities.PointDatas[index]._Pin.enabled == true
                && !PinUtilities.PointDatas[index].DeltasActiveNow((long)thisTick, PinUtilities.pinDeltas[PinUtilities.PointDatas[index]._Label])
                && !PinUtilities.PointDatas[index].ActiveNow())
                {
                    PinUtilities.PointDatas[index].RemovePin();
                    continue;
                }
                else if (PinUtilities.PointDatas[index]._Pin.enabled == false
                && PinUtilities.PointDatas[index].DeltasActiveNow((long)thisTick, PinUtilities.pinDeltas[PinUtilities.PointDatas[index]._Label])
                && PinUtilities.PointDatas[index].ActiveNow())
                {
                    AddPin(PinUtilities.PointDatas[index]);
                    continue;
                }

                double pinDist = PinUtilities.DistanceInMeters(pos.y, pos.x, PinUtilities.PointDatas[index]._Position.Latitude, PinUtilities.PointDatas[index]._Position.Longitude);
                if (PinUtilities.PointDatas[index]._Pin.texture.name != PinUtilities.PointDatas[index]._Icon.name
                && (PinUtilities.PointDatas[index]._Pin.enabled == false
                || !PinUtilities.PointDatas[index].DeltasActiveNow((long)thisTick, PinUtilities.pinDeltas[PinUtilities.PointDatas[index]._Label])
                || !PinUtilities.PointDatas[index].ActiveNow()
                || pinDist > pinRadius))
                {
                    PinUtilities.PointDatas[index]._Pin.texture = PinUtilities.PointDatas[index]._Icon;
                    PinUtilities.PointDatas[index]._Pin.Init();
                    OnlineMaps.instance.Redraw();
                    continue;
                }
                else if (PinUtilities.PointDatas[index]._Pin.texture.name != PinUtilities.PointDatas[index]._ActiveIcon.name
                && PinUtilities.PointDatas[index]._Pin.enabled == true
                && PinUtilities.PointDatas[index].DeltasActiveNow((long)thisTick, PinUtilities.pinDeltas[PinUtilities.PointDatas[index]._Label])
                && PinUtilities.PointDatas[index].ActiveNow()
                && pinDist < pinRadius)
                {
                    PinUtilities.PointDatas[index]._Pin.texture = PinUtilities.PointDatas[index]._ActiveIcon;
                    PinUtilities.PointDatas[index]._Pin.Init();
                    OnlineMaps.instance.Redraw();
                    continue;
                }

                if (m_lastPlace >= PinUtilities.PointDatas.Count - 1)
                {
                    m_lastPlace = 0;
                }
            }
            m_pinLastTick = (long)thisTick;
        }
    }

    // Handles update for token pins
    private void TokenPinUpdate(long thisTick)
    {
        if (thisTick > m_TokenLastPlace + m_pinTick)
        {
            int thisCount = 0;
            Vector2 pos = OnlineMapsLocationService.instance.position;
            for (int i = 0 + m_TokenLastPlace; i < PinUtilities.TokenPointDatas.Count && thisCount < pinsPerUpdate; ++i, ++m_TokenLastPlace, ++thisCount)
            {
                int index = i;
                double pinDist = PinUtilities.DistanceInMeters(pos.y, pos.x, PinUtilities.TokenPointDatas[index]._Position.Latitude, PinUtilities.TokenPointDatas[index]._Position.Longitude);
                string key = PinUtilities.TokenPointDatas[index]._Position.Latitude.ToString() + PinUtilities.TokenPointDatas[index]._Position.Longitude.ToString();

                if (PinUtilities.TokenPointDatas[index]._Pin.enabled == false
                && PinUtilities.TokenPointDatas[index].TokenDeltasActiveNow((long)thisTick, PinUtilities.tokenPinDeltas[key])
                && PinUtilities.TokenPointDatas[index].ActiveNow())
                {
                    AddTokenPin(PinUtilities.TokenPointDatas[index]);
                    continue;
                }
                else if (pinDist < pinRadius
                && PinUtilities.TokenPointDatas[index]._Pin.enabled == true
                && PinUtilities.TokenPointDatas[index].TokenDeltasActiveNow((long)thisTick, PinUtilities.tokenPinDeltas[key])
                && PinUtilities.TokenPointDatas[index].ActiveNow())
                {
                    if(PinUtilities.TokenPointDatas[index]._Pin.texture.name != PinUtilities.TokenPointDatas[index]._ActiveIcon.name)
                    {
                        PinUtilities.TokenPointDatas[index]._Pin.texture = PinUtilities.TokenPointDatas[index]._ActiveIcon;
                        PinUtilities.TokenPointDatas[index]._Pin.Init();
                        OnlineMaps.instance.Redraw();
                    }
                    continue;
                }
                else if(pinDist > pinRadius
                && PinUtilities.TokenPointDatas[index]._Pin.enabled == true
                && PinUtilities.TokenPointDatas[index].TokenDeltasActiveNow((long)thisTick, PinUtilities.tokenPinDeltas[key])
                && PinUtilities.TokenPointDatas[index].ActiveNow())
                {
                    if(PinUtilities.TokenPointDatas[index]._Pin.texture.name != PinUtilities.TokenPointDatas[index]._Icon.name)
                    {
                        PinUtilities.TokenPointDatas[index]._Pin.texture = PinUtilities.TokenPointDatas[index]._Icon;
                        PinUtilities.TokenPointDatas[index]._Pin.Init();
                        OnlineMaps.instance.Redraw();
                    }
                    continue;
                }
            }
            if (m_TokenLastPlace >= PinUtilities.TokenPointDatas.Count - 1)
            {
                m_TokenLastPlace = 0;
            }
        }
        m_tokenPinLastTick = (long)thisTick;
    }

    // Event executes when token pins are clicked
    private void TokenPinActivate(PointData pinData, string key)
    {
        if (pinData._Jackpot)
        {
            UserUtilities.AllocatePoints(pinData._Value * jackpotMultiplier);
            UIManager.Instance.IndicateScore(pinData._Value * jackpotMultiplier, true);
        }
        else
        {
            UserUtilities.AllocatePoints(pinData._Value);
            UIManager.Instance.IndicateScore(pinData._Value, true);
        }

        SoundManager.Instance.PlaySound("CoinCollect");

        long now = (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;

        PinUtilities.tokenPinDeltas[key] = now;

        pinData.RemovePin();

        PinUtilities.SaveTokenPins();
        UserUtilities.Save();
    }
#endregion

#region MiniGames
    /// <summary> Unlocks a specific organizations minigame </summary>
    public void Unlock(Organizations org)
    {
        string gname = MiniGame_Objects[(int)org].name.Replace("_UI", "");
        MinigameData.Activate(gname);
    }
#endregion
}
