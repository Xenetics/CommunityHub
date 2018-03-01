using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MemoryMatch : BaseGame
{
    [Header("UI")]
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
    // Meter that represents how long to next level
    [SerializeField]
    private Image MatchGoalMeter;
    // Text on meter that represent next level
    [SerializeField]
    private Text MatchGoalText;

    [Header("Gameplay")]
    // is the game pasused of not
    [NonSerialized]
    public bool Paused = false;
    // Tile cells in the game
    public MatchCell[][] Cells;
    // Side scale of the tile board 2 - 7
    public int TileCountScale = 3;
    private int MinTileCountScale = 3;
    private int MaxTileCountScale = 7;
    // Prefab of game tile
    public GameObject TilePrefab;
    // This object contains the game tiles
    public GameObject PlayArea;
    // Images that can match
    public List<Sprite> Matchables;
    // Is the board setting up
    [NonSerialized]
    public bool BoardSetup = false;
    // Sequential tiles being added and moved
    private bool Adding = false;
    // Speed a tile moves to position
    public float TileSpeed = 3f;
    // Time between tiles being added
    public float addTileDelay = 0.1f;
    // pregame count down timer
    private float pregameTimer = 0f;
    // length of pregame cound down
    public float PregameTime = 3f;
    // is pregame counting down
    private bool PregameCountdown = false;
    // level transitioning
    private bool LevelTransitioning = false;
    // Currently selected match tiles
    public List<MatchTile> matchingTiles;
    // count of current matches
    private int matchCount = 0;
    // amount of tiles to remove for next level
    private int matchCountGoal = 15;
    // Points per Tile
    public int PointsPerTile = 20;
    // The prefab for the score indicator in match game
    public GameObject ScoreIndicator;
    // Time till game over
    public float GameTime = 90f;
    // Time added per tile per match made
    public float PerTileTime = 0.5f;
    // Time added per tile when level up
    public float LvLPerTileTime = 1f;
    // The ratio os tokens you get for playing the game
    private float tokenRatio = 0.2f;
    // Duration the game is played for
    private float gameDuration = 0;

    void Start()
    {
        Cur_State = State.Splash;
        EnterState(State.Splash);
        Timer = GameTime;
    }

    /// <summary> returns the current state of the game </summary>
    public State GetState()
    {
        return Cur_State;
    }

    /// <summary> Handles update loop dependant on state of the game </summary>
    void Update()
    {
        switch (Cur_State)
        {
            case State.Splash:
                splashTimer += Time.deltaTime;
                if(splashTimer >= SplashTime)
                {
                    SetState(State.Menu);
                }
                break;
            case State.Menu:

                break;
            case State.Pregame:
                gameDuration += Time.deltaTime;
                matchCount = 0;
                if (!TransitionCheck() && !PregameCountdown)
                {
                    OpenTiles();
                    BoardSetup = false;
                    PregameCountdown = true;
                }
                else if(PregameCountdown)
                {
                    pregameTimer += Time.deltaTime;
                    if(pregameTimer >= PregameTime)
                    {
                        pregameTimer = 0;
                        matchCountGoal = (TileCountScale * TileCountScale * TileCountScale);
                        HandleMeter();
                        SetState(State.Playing);
                        PregameCountdown = false;
                    }
                }
                break;
            case State.Playing:
                gameDuration += Time.deltaTime;
                if (TileCountScale < MaxTileCountScale)
                {
                    if (matchCount >= matchCountGoal)
                    {
                        KillAllTiles();
                        TileCountScale++;
                        Timer += LvLPerTileTime * (TileCountScale * TileCountScale);
                        SetState(State.Pregame);
                    }
                }
                EmptyCellCheck();
                HandleTimer();
                break;
            case State.Paused:

                break;
            case State.Gameover:

                break;
        }
    }

    /// <summary> Handles the entrance of a state dependant of state given </summary>
    public override void EnterState(State state)
    {
        switch (state)
        {
            case State.Splash:
                UI.Splash_PNL.SetActive(true);
                splashTimer = 0;
                SoundManager.Instance.PlayMusic("MPL_MainLoop", Music);
                break;
            case State.Menu:
                UI.Menu_PNL.SetActive(true);
                SetHighScore(GameManager.MGD.Highscore.ToString());
                break;
            case State.Pregame:
                UI.Playing_PNL.SetActive(true);
                BoardSetup = true;
                CreatCells();
                AddTiles("random");
                matchCountGoal = (TileCountScale * TileCountScale * TileCountScale);
                break;
            case State.Playing:
                UI.Playing_PNL.SetActive(true);
                break;
            case State.Paused:
                UI.Paused_PNL.SetActive(true);
                break;
            case State.Gameover:
                GameManager.Instance.analytics.LogTiming(new TimingHitBuilder().SetTimingCategory(Name).SetTimingInterval((long)gameDuration).SetTimingName("Playtime"));
                GameManager.Instance.analytics.LogEvent(new EventHitBuilder().SetEventCategory(Name).SetEventAction("Points").SetEventValue(Score));
                GameManager.Instance.analytics.LogEvent(new EventHitBuilder().SetEventCategory(Name).SetEventAction("Tokens").SetEventValue((long)(Score * tokenRatio)));
                UI.Gameover_PNL.SetActive(true);
                KillAllTiles();
                UI.GameOverScore_Text.text = Score.ToString();
                UI.GameOverToken_Text.text = (Score * tokenRatio).ToString();
                UpdateHighScore(Score);
                UserUtilities.AllocatePoints((int)(Score * tokenRatio));
                SoundManager.Instance.PlaySound("CoinCollect");
                break;
        }
    }

    /// <summary> Handles the exit of a state dependant of state given </summary>
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

                break;
            case State.Playing:
                if (Next_State != State.Paused)
                {
                    UI.Playing_PNL.SetActive(false);
                }
                break;
            case State.Paused:
                UI.Paused_PNL.SetActive(false);
                if(Next_State == State.Menu)
                {
                    KillAllTiles();
                    ResetGame();
                }
                break;
            case State.Gameover:
                UI.Gameover_PNL.SetActive(false);
                ResetGame();
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
        gameDuration = 0;
        TileCountScale = MinTileCountScale;
        Score = 0;
        UI.Score_Text.text = Score.ToString();
        Timer = GameTime;
        UI.Timer_Text.text = Timer.ToString("00");
        TimerRadial.fillAmount = Timer / GameTime;
        TimerRadial.color = Color.Lerp(NoTimeCol, FullTimeCol, TimerRadial.fillAmount);
        matchCountGoal = (TileCountScale * TileCountScale * TileCountScale);
        HandleMeter();
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

    /// <summary> Sets up the cells for the board </summary>
    private void CreatCells()
    {
        // Cellarrays 
        Cells = new MatchCell[TileCountScale][];
        for(int i = 0; i < Cells.Length; ++i)
        {
            Cells[i] = new MatchCell[TileCountScale];
        }

        // calculate offset of tiles
        float tileOffset = TilePrefab.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
        // Tile position in top left
        Vector3 topleft = new Vector3(0 - (tileOffset * (TileCountScale - 1)), 0 + (tileOffset * (TileCountScale - 1)), 0);
        // used for pos each tile
        Vector3 pos = Vector3.zero;

        for (int col = 0; col < TileCountScale; ++col)
        {
            for(int row = 0; row < TileCountScale; ++row)
            {
                // Calculate position
                pos.x = topleft.x + (col * (tileOffset * 2));
                pos.y = topleft.y - (row * (tileOffset * 2));

                Cells[col][row] = new MatchCell();
                Cells[col][row].Cell = new Vector2(col, row);
                Cells[col][row].Position = pos;
            }
        }
        matchingTiles = new List<MatchTile>();
    }

    /// <summary> Calculates and places tiles </summary>
    private void AddTiles(string spew)
    {
        int count = 0;
        for (int col = 0; col < TileCountScale; ++col)
        {
            for (int row = 0; row < TileCountScale; ++row, ++count)
            {
                // Create object
                GameObject newTile = Instantiate(TilePrefab, PlayArea.transform);
                newTile.GetComponent<RectTransform>().localPosition = new Vector3(0, 1000, 0);
                // get refference to tile class
                MatchTile tile = newTile.GetComponent<MatchTile>();
                // Random image
                System.Random rand = new System.Random((int)DateTime.Now.Ticks);
                tile.MatchImage.sprite = Matchables[rand.Next(0, Matchables.Count)];
                tile.Transitioning = true;
                tile.game = this;
                tile.currentCell = new Vector2(col, row);
                if (spew == "random")
                {
                    tile.SetupDelay = UnityEngine.Random.Range(0f, 1f);
                }
                else if(spew == "ordered")
                {
                    tile.SetupDelay = 0.5f + (count * addTileDelay);
                }
                // set position
                tile.FinalPos = Cells[col][row].Position;
                Cells[col][row].Tile = tile;
            }
        }
    }

    /// <summary> Calculates and places a single tile </summary>
    private void AddTile(MatchCell cell)
    {
        // Create object
        GameObject newTile = Instantiate(TilePrefab, PlayArea.transform);
        newTile.GetComponent<RectTransform>().localPosition = new Vector3(0, 1000, 0);
        // get refference to tile class
        MatchTile tile = newTile.GetComponent<MatchTile>();

        // Random image
        System.Random rand = new System.Random((int)DateTime.Now.Ticks);
        tile.MatchImage.sprite = Matchables[rand.Next(0, Matchables.Count)];

        tile.Transitioning = true;
        tile.game = this;
        tile.currentCell = cell.Cell;
        tile.SetupDelay = 0f;
        // set position
        tile.FinalPos = cell.Position;
        cell.Tile = tile;
    }

    /// <summary> Checks all tiles to see if they are transitioning </summary>
    private bool TransitionCheck()
    {
        for (int col = 0; col < TileCountScale; ++col)
        {
            for (int row = 0; row < TileCountScale; ++row)
            {
                if (Cells[col][row].Tile.Transitioning)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary> Opens all tiles </summary>
    private void OpenTiles()
    {
        for (int col = 0; col < TileCountScale; ++col)
        {
            for (int row = 0; row < TileCountScale; ++row)
            {
                Cells[col][row].Tile.OpenTile();
            }
        }
    }

    /// <summary> Checks for empty cells and either creates new tiles or make others fall </summary>
    private void EmptyCellCheck()
    {
        // loop row then column to go left to right top to bottom
        for (int row = 0; row < TileCountScale; ++row)
        {
            for (int col = 0; col < TileCountScale; ++col)
            {
                if (Cells[col][row].Tile == null)
                {
                    if(row == 0)
                    {
                        AddTile(Cells[col][row]);
                    }
                    else
                    {
                        if(Cells[col][row - 1].Tile != null)
                        {
                            Cells[col][row].Tile = Cells[col][row - 1].Tile;
                            Cells[col][row].Tile.currentCell = new Vector2(col, row);
                            Cells[col][row - 1].Tile = null;
                            Cells[col][row].Tile.FinalPos = Cells[col][row].Position;
                            Cells[col][row].Tile.Transitioning = true;
                            Cells[col][row].Tile.SetupDelay = 0;
                        }
                    }
                    Adding = true;
                    return;
                }
            }
        }
        if (Adding && !TransitionCheck())
        {
            Adding = false;
            OpenTiles();
        }
    }

    /// <summary> Kills all the tiles in the current board </summary>
    private void KillAllTiles()
    {
        for (int col = 0; col < TileCountScale; ++col)
        {
            for (int row = 0; row < TileCountScale; ++row)
            {
                KillTile(new Vector2(col, row));
            }
        }
    }

    /// <summary> Destroys the tile in the cell </summary>
    public void KillTile(Vector2 cell)
    {
        if (Cells[(int)cell.x][(int)cell.y].Tile != null)
        {
            Vector3 newPos = Cells[(int)cell.x][(int)cell.y].Tile.FinalPos;
            newPos.x = 0;
            newPos.y = newPos.y - 1500;
            Cells[(int)cell.x][(int)cell.y].Tile.FinalPos = newPos;
            Cells[(int)cell.x][(int)cell.y].Tile.Exiting = true;
            Cells[(int)cell.x][(int)cell.y].Tile.gameObject.transform.SetParent(gameObject.transform);
            Cells[(int)cell.x][(int)cell.y].Tile = null;
        }
    }

    /// <summary> Checks a tile for match and adds to the  </summary>
    public void SelectTile(MatchTile tile)
    {
        if (matchingTiles.Count == 0)
        {
            tile.Selected = true;
            matchingTiles.Add(tile);
            SoundManager.Instance.PlaySound("MatchGameOpen", SFX);
            return;
        }
        else
        {
            for (int i = 0; i < matchingTiles.Count; ++i)
            {
                if (matchingTiles[i].MatchImage.sprite == tile.MatchImage.sprite)
                {
                    tile.Selected = true;
                    matchingTiles.Add(tile);
                    SoundManager.Instance.PlaySound("MatchGameOpen", SFX);
                    if (!AnyMatches())
                    {
                        EndCombo();
                        SoundManager.Instance.PlaySound("ComboMatch", SFX);
                    }
                    return;
                }
                else
                {
                    SoundManager.Instance.PlaySound("FailMatch", SFX);
                    EndCombo();
                }
            }
        }
    }

    /// <summary> Returns true if there are any matches not yet matched </summary>
    private bool AnyMatches()
    {
        for (int col = 0; col < TileCountScale; ++col)
        {
            for (int row = 0; row < TileCountScale; ++row)
            {
                if(!Cells[col][row].Tile.Selected && Cells[col][row].Tile.MatchImage.sprite == matchingTiles[0].MatchImage.sprite)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary> Ends the tile selection combo and destros the selected tiles and allocates points </summary>
    private void EndCombo()
    {
        if (matchingTiles.Count > 1)
        {
            for (int i = 0; i < matchingTiles.Count; ++i)
            {
                AllocatePoints(PointsPerTile * (matchingTiles.Count - 1), matchingTiles[i]);
                KillTile(matchingTiles[i].currentCell);
                matchCount++;
                Timer += PerTileTime;
            }
            HandleMeter();
            matchingTiles = new List<MatchTile>();
        }
        else
        {
            for (int i = 0; i < matchingTiles.Count; ++i)
            {
                matchingTiles[i].Selected = false;
                matchingTiles = new List<MatchTile>();
            }
        }
    }

    /// <summary> Allocates points for the game </summary>
    public void AllocatePoints(int value, MatchTile tile)
    {
        Score += value;
        UI.Score_Text.text = Score.ToString();
        GameObject go = Instantiate(ScoreIndicator, gameObject.transform);
        go.transform.position = tile.gameObject.transform.position;
        go.GetComponent<ScoreIndicator>().ScrollDest = UI.Score_Text.transform;
        go.GetComponent<ScoreIndicator>().Init(value, true);
    }

    /// <summary> Handles the timer and the radial </summary>
    private void HandleTimer()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            SetState(State.Gameover);
        }
        UI.Timer_Text.text = Timer.ToString("00");
        TimerRadial.fillAmount = Timer / GameTime;
        TimerRadial.color = Color.Lerp(NoTimeCol, FullTimeCol, TimerRadial.fillAmount);
    }

    /// <summary> Handles Updating the match goal meter </summary>
    private void HandleMeter()
    {
        if (TileCountScale != MaxTileCountScale)
        {
            MatchGoalText.text = (matchCountGoal - matchCount) + " Matches till Level";
            MatchGoalMeter.fillAmount = ((float)matchCount / (float)matchCountGoal);
            MatchGoalMeter.color = Color.Lerp(NoTimeCol, FullTimeCol, MatchGoalMeter.fillAmount);
        }
        else
        {
            MatchGoalText.text = "Max Difficulty";
            MatchGoalMeter.fillAmount = 1;
            MatchGoalMeter.color = FullTimeCol;
        }
    }

    /// <summary> Sets the high score text </summary>
    public void SetHighScore(string value)
    {
        UI.HighScore_Text.text = value;
    }

    /// <summary> Updates and saves data if score is larger </summary>
    private void UpdateHighScore(int score)
    {
        if(GameManager.MGD.Highscore < score)
        {
            GameManager.MGD.Highscore = score;
            GameManager.MGD.SaveGameData();
        }
    }
}
