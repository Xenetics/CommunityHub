using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using QRCodeReaderAndGenerator;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }
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
        LoginPassword_INPT.shouldHideMobileInput = true;
        RegisterPassword_INPT.shouldHideMobileInput = true;
        RegisterVerifyPassword_INPT.shouldHideMobileInput = true;
    }

    #region UI Components
    // Main Canvas object
    [Header("UI Componants")]
    public GameObject MainCanvas;
    // first play tutorial block
    [SerializeField]
    private GameObject FirstPlay_Prefab;
    // eula popup
    [SerializeField]
    private GameObject Eula_Prefab;
    // if true show eula
    [NonSerialized]
    public bool ShowEULA = false;
    [SerializeField]
    private Animator Content_Animator;
    public GameManager.GameState NextState;

    // Main panels in the UI
    [Header("Panels")]
    [SerializeField]
    private Image Splash_PNL;
    [SerializeField]
    private GameObject Start_PNL;
    [SerializeField]
    private GameObject Login_PNL;
    [SerializeField]
    private GameObject Register_PNL;
    [SerializeField]
    private GameObject Main_PNL;

    // The Sub panels in Main_PNL
    [Header("Sub-Panels")]
    [SerializeField]
    private ProfilePanel Profile_PNL;
    [SerializeField]
    private GameObject CheckIn_PNL;
    [SerializeField]
    private GameObject Map_PNL;
    [SerializeField]
    private GameObject Games_PNL;
    [SerializeField]
    private GameObject Events_PNL;

    // Settings panel UI
    [Header("Settings")]
    [SerializeField]
    private GameObject Settings_PNL;
    [SerializeField]
    private Toggle MusicToggle;
    [SerializeField]
    private Toggle SFXToggle;
    [SerializeField]
    private Text VersionText;

    // Input Fields for login and register
    [Header ("Inputs")]
    [SerializeField]
    private InputField LoginCard_INPT;
    [SerializeField]
    private InputField LoginPassword_INPT;
    [SerializeField]
    private Toggle RememberMe_TGL;
    [SerializeField]
    private InputField RegisterCard_INPT;
    [SerializeField]
    private InputField RegisterUsername_INPT;
    [SerializeField]
    private InputField RegisterEmail_INPT;
    [SerializeField]
    private InputField RegisterPassword_INPT;
    [SerializeField]
    private InputField RegisterVerifyPassword_INPT;

    // Main Screen Top Nav Buttons
    [Header("Main Nav Buttons")]
    [SerializeField]
    private Button Profile_BTN;
    [SerializeField]
    private Button Checkin_BTN;
    [SerializeField]
    private Button Map_BTN;
    [SerializeField]
    private Button Games_BTN;
    [SerializeField]
    private Button Events_BTN;

    // Notification and popup UI panels
    [Header("Notification")]
    [SerializeField]
    private GameObject Notification_PNL;
    [SerializeField]
    private Text NotificationMessage_TXT;
    [SerializeField]
    private ProfileChangeInputBox ProfileChange_INPT_Box;

    // Plash Screen Settings
    [Header("Splash")]
    [SerializeField]
    private Image SplashDisplayImage;
    [SerializeField] // Duration per image in seconds on splash screen
    private float SplashDuration = 1.0f;
    private float Timer = 0.0f;
    [SerializeField] // Images to be shown preious to the start screen
    private Sprite[] Splash_IMGs;
    // Currently displayed splash image
    private int SplashIndex = 0;

    // Map Opjects and settings
    [Header("Map")]
    public GameObject MapObject;
    /// <summary> Pin textures </summary>
    public Texture2D[] PinTextures;
    /// <summary> Pin textures when in proximity </summary>
    public Texture2D[] AltPinTextures;
    public GameObject ScoreIndicator_Prefab;
    private bool FirstMapLoad = true;
    private bool UpdateMap = false; 
    [SerializeField]
    private float TimeBeforeMapUpdate = 5f;
    private float MapUpdateTimer = 0;
    [SerializeField]
    private float MapUpdateSpeed = 0.01f;

    // Event listing settings and objects
    [Header("Events")]
    [SerializeField]
    private ScrollRect EventScrollRect;
    [SerializeField]
    private GameObject EventScrollObject;
    [SerializeField]
    private GameObject Event_Prefab;
    [SerializeField]
    private GameObject EventDetails_Popup;
    private List<GameObject> EventPanels;

    // MiniGame listing objects and settings
    [Header("MiniGames")]
    [SerializeField]
    private GameObject MinigameListing_Prefab;
    [NonSerialized]
    public List<GameObject> MinigamePanels;
    [SerializeField]
    private GameObject MinigameScroll_Object;

    // Checkin rewards objects and settings
    [Header("Rewards")]
    [SerializeField]
    private ScrollRect RewardScrollRect;
    [SerializeField]
    private GameObject RewardScrollObject;
    [SerializeField]
    private GameObject  RewardListPrefab;
    private List<GameObject> ProductList;
    [SerializeField]
    private Sprite[] ProtuctIcons;

    // QR code objects and settings
    [Header("QR Scanner")]
    [SerializeField]
    private Button Quest_BTN;
    [SerializeField]
    private Button List_BTN;
    [SerializeField]
    private Button Scan_BTN;
    [SerializeField]
    private GameObject List_Tab;
    [SerializeField]
    private GameObject Scan_Tab;
    [SerializeField]
    private RawImage CamImage;
    private WebCamTexture webcamTexture;
    private Quaternion camRotation;
    [SerializeField]
    private GameObject Confirmtion_Prefab;
    private string QRLogPath = "/qlog.dat";
    private static TimeSpan QRLockout = new TimeSpan(24, 0, 0);
    [SerializeField]
    private GameObject QuestList;
    #endregion

    // Checks requests camera permisiion
    private IEnumerator Start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
    }

    public void TransitionState(GameManager.GameState newState)
    {
        if (!Content_Animator.GetCurrentAnimatorStateInfo(0).IsName("SwapPanel"))
        {
            NextState = newState;
            Content_Animator.SetTrigger("Swap");
        }
    }

    void Update()
    {
        switch (GameManager.Cur_State)
        {
            case GameManager.GameState.Splash:
                Timer -= Time.deltaTime;
                if(Timer <= 0)
                {
                    SplashIndex++;
                    if (Splash_IMGs.Length > SplashIndex)
                    {
                        SplashDisplayImage.sprite = Splash_IMGs[SplashIndex];
                    }
                    else
                    {
                        GameManager.Instance.SetState(GameManager.GameState.StartPage);
                    }
                    Timer = SplashDuration;
                }
                break;
            case GameManager.GameState.StartPage:

                break;
            case GameManager.GameState.Login:

                break;
            case GameManager.GameState.Register:

                break;
            case GameManager.GameState.CheckIn:

                break;
            case GameManager.GameState.Map:
                SmoothMap();
                break;
            case GameManager.GameState.Games:

                break;
            case GameManager.GameState.Events:

                break;
            case GameManager.GameState.Profile:

                break;
            case GameManager.GameState.Minigame:

                break;
        }
    }

    public void EnterState()
    {
        switch (GameManager.Cur_State)
        {
            case GameManager.GameState.Splash:
                Splash_PNL.gameObject.SetActive(true);
                if (Splash_IMGs.Length > 0)
                {
                    SplashDisplayImage.sprite = Splash_IMGs[0];
                }
                else
                {
                    GameManager.Instance.SetState(GameManager.GameState.StartPage);
                }
                Timer = SplashDuration;
                break;
            case GameManager.GameState.StartPage:
                Start_PNL.SetActive(true);
                break;
            case GameManager.GameState.Login:
                if (gameObject.GetComponent<RawImage>())
                {
                    Destroy(gameObject.GetComponent<RawImage>());
                }
                TouchScreenKeyboard.hideInput = true;
                Login_PNL.SetActive(true);
                if (UserUtilities.RememberMeExists())
                {
                    RememberMe_TGL.isOn = true;
                    UserUtilities.RememberMeRetrieve();
                    LoginCard_INPT.text = UserUtilities.User.CardNumber;
                    LoginPassword_INPT.text = UserUtilities.User.Password;
                }
                else
                {
                    RememberMe_TGL.isOn = false;
                }
                break;
            case GameManager.GameState.Register:
                TouchScreenKeyboard.hideInput = true;
                Register_PNL.SetActive(true);
                break;
			case GameManager.GameState.CheckIn:
                Main_PNL.SetActive(true);
                CheckIn_PNL.SetActive(true);
                Enable_BTNs();
                Checkin_BTN.interactable = false;
                Products.InitProducts();
                CreateProductList();
                List_BTN.interactable = false;
                List_Tab.SetActive(true);
                Scan_BTN.interactable = true;
                Scan_Tab.SetActive(false);

                break;
            case GameManager.GameState.Map:
                Main_PNL.SetActive(true);
                Map_PNL.SetActive(true);
                Enable_BTNs();
                Map_BTN.interactable = false;
                OnlineMaps.instance.zoom = 18;
                if (FirstMapLoad)
                {
                    FirstMapLoad = false;
                    OnlineMaps.instance.RemoveAllMarkers();
                    PinUtilities.GetPointData();
                    PinUtilities.PinDelta();
                    PinUtilities.TokenPinDelta();
                    PinUtilities.AddPins();
                    PinUtilities.AddTokenPins();
                    OnlineMapsLocationService.instance.createMarkerInUserPosition = true;
                    OnlineMaps.instance.RedrawImmediately();
                }
                OnlineMaps.instance.position = OnlineMapsLocationService.instance.position;
                break;
            case GameManager.GameState.Games:
                Main_PNL.SetActive(true);
                Games_PNL.SetActive(true);
                Enable_BTNs();
                Games_BTN.interactable = false;
                CreateGameList();
                break;
            case GameManager.GameState.Events:
                Main_PNL.SetActive(true);
                Events_PNL.SetActive(true);
                Enable_BTNs();
                Events_BTN.interactable = false;
                Events.InitEvents();
                CreateEventList();
                break;
            case GameManager.GameState.Profile:
                TouchScreenKeyboard.hideInput = true;
                Main_PNL.SetActive(true);
                Profile_PNL.UpdateProfile();
                Profile_PNL.gameObject.SetActive(true);
                Enable_BTNs();
                Profile_BTN.interactable = false;
                break;
            case GameManager.GameState.Minigame:
                Main_PNL.SetActive(false);
                break;
        }
    }

    public void ExitState(GameManager.GameState nextState)
    {
        switch (GameManager.Cur_State)
        {
            case GameManager.GameState.Splash:
                Splash_PNL.gameObject.SetActive(false);
                break;
            case GameManager.GameState.StartPage:
                Start_PNL.SetActive(false);
                break;
            case GameManager.GameState.Login:
                Login_PNL.SetActive(false);
                break;
            case GameManager.GameState.Register:
                Register_PNL.SetActive(false);
                break;
            case GameManager.GameState.CheckIn:
                if ((int)GameManager.Cur_State < (int)GameManager.GameState.CheckIn)
                {
                    Main_PNL.SetActive(false);
                }
                CheckIn_PNL.SetActive(false);
                RewardList_BTN();
                break;
            case GameManager.GameState.Map:
                if ((int)GameManager.Cur_State < (int)GameManager.GameState.CheckIn)
                {
                    Main_PNL.SetActive(false);
                }
                Map_PNL.SetActive(false);
                break;
            case GameManager.GameState.Games:
                if ((int)GameManager.Cur_State < (int)GameManager.GameState.CheckIn)
                {
                    Main_PNL.SetActive(false);
                }
                Games_PNL.SetActive(false);
                break;
            case GameManager.GameState.Events:
                EventDetails_Popup.SetActive(false);
                if ((int)GameManager.Cur_State < (int)GameManager.GameState.CheckIn)
                {
                    Main_PNL.SetActive(false);
                }
                Events_PNL.SetActive(false);
                break;
            case GameManager.GameState.Profile:
                if ((int)GameManager.Cur_State < (int)GameManager.GameState.CheckIn)
                {
                    Main_PNL.SetActive(false);
                }
                Profile_PNL.gameObject.SetActive(false);
                break;
            case GameManager.GameState.Minigame:

                break;
        }
    }

    public void Profile_BTN_Click()
    {
        TransitionState(GameManager.GameState.Profile);
    }

    public void Checkin_BTN_Click()
    {
        TransitionState(GameManager.GameState.CheckIn);
    }

    public void Map_BTN_Click()
    {
        TransitionState(GameManager.GameState.Map);
    }

    public void Games_BTN_Click()
    {
        TransitionState(GameManager.GameState.Games);
    }

    public void Events_BTN_Click()
    {
        TransitionState(GameManager.GameState.Events);
    }

#region Settings
    public void MuteSFX_BTN_Click()
    {
        SoundManager.Instance.Mute_SFX(SFXToggle.isOn);
    }

    public void MuteMusic_BTN_Click()
    {
        SoundManager.Instance.Mute_Music(MusicToggle.isOn);
    }

    public void Exit_BTN_Click()
    {
        Application.Quit();
    }

    public void Settings_BTN_Click()
    {
        if (!Settings_PNL.GetComponent<Animator>().GetBool("Open"))
        {
            Settings_PNL.GetComponent<Animator>().SetBool("Open", true);
            VersionText.text = "Version " + Application.version;
        }
        else
        {
            Settings_PNL.GetComponent<Animator>().SetBool("Open", false);
        }
    }
    #endregion

    private void Enable_BTNs()
    {
        Profile_BTN.interactable = true;
        Checkin_BTN.interactable = true;
        Map_BTN.interactable = true;
        Games_BTN.interactable = true;
        Events_BTN.interactable = true;
    }

#region Login
    public void ConfirmLogin_BTN_Click()
    {
        string feedback = UserUtilities.CheckRemoteData(LoginCard_INPT.text, LoginPassword_INPT.text);
        switch(feedback)
        {
            case "BadConnection":
                DisplayNotification(true, "Cannot connect. Please ensure wifi or mobile data connection.");
                break;
            case "Incorrect":
                DisplayNotification(true, "Library card and/or password is incorrect");
                break;
            case "Correct":
                if(RememberMe_TGL.isOn)
                {
                    UserUtilities.RememberMeSave();
                }
                else
                {
                    UserUtilities.RememberMeDelete();
                }
                GameManager.Instance.SetState(GameManager.GameState.Map);
                if (UserUtilities.User.Active == "New")
                {
                    GameObject tut = Instantiate(FirstPlay_Prefab, gameObject.transform);
                }
                if(ShowEULA)
                {
                    GameObject eula = Instantiate(Eula_Prefab, gameObject.transform);
                }
                break;
            case "NonExistant":
                DisplayNotification(true, "Library card not registered");
                break;
            case "Empty":
                DisplayNotification(true, "Please enter both a Library card number and a password");
                break;
            case "Error":
                DisplayNotification(true, "Unexpected error. Please try again.");
                break;
        }
    }

    public void ConfirmRegister_BTN_Click()
    {
        string feedback = UserUtilities.CreateremoteData(RegisterUsername_INPT.text, RegisterCard_INPT.text, RegisterPassword_INPT.text, RegisterVerifyPassword_INPT.text, RegisterEmail_INPT.text);
        switch (feedback)
        {
            case "BadConnection":
                DisplayNotification(true, "Cannot connect. Please ensure wifi or mobile data connection.");
                break;
            case "EmptyField":
                DisplayNotification(true, "Please enter all fields");
                break;
            case "InvalidCard":
                DisplayNotification(true, "Invalid Library Card");
                break;
            case "UsernameTooShort":
                DisplayNotification(true, "Username should be 3 or more characters long");
                break;
            case "UsernameIllegal":
                DisplayNotification(true, "Username inappropriate");
                break;
            case "InvalidEmail":
                DisplayNotification(true, "Not a valid Email format");
                break;
            case "UserExists":
                DisplayNotification(true, "Library card is already registered");
                break;
            case "MismatchedPass":
                DisplayNotification(true, "Passwords do not match");
                break;
            case "WeakPass":
                DisplayNotification(true, "Password needs to be " + UserUtilities.minPassLength + " - " + UserUtilities.MaxInputLength + " characters long and contain 1 uppercase, 1 symbol, & 1 number");
                break;
            case "Created":
                LoginCard_INPT.text = RegisterCard_INPT.text;
                LoginPassword_INPT.text = RegisterPassword_INPT.text;
                ClearRegisterInput();
                GameManager.Instance.SetState(GameManager.GameState.Login);
                break;
            case "Error":
                DisplayNotification(true, "Unexpected error. Please try again.");
                break;
        }
    }

    public void BackToStart_BTN_Click()
    {
        GameManager.Instance.SetState(GameManager.GameState.StartPage);
    }

    public void Login_BTN_Click()
    {
        GameManager.Instance.SetState(GameManager.GameState.Login);
    }

    public void Register_BTN_Click()
    {
        GameManager.Instance.SetState(GameManager.GameState.Register);
    }

    public void RememberMeToggled()
    {
        if(!RememberMe_TGL.isOn)
        {
            UserUtilities.RememberMeDelete();
        }
        else
        {

        }
    }

    private void ClearLoginInput()
    {
        LoginCard_INPT.text = "";
        LoginPassword_INPT.text = "";
    }

    private void ClearRegisterInput()
    {
        RegisterCard_INPT.text = "";
        RegisterUsername_INPT.text = "";
        RegisterEmail_INPT.text = "";
        RegisterPassword_INPT.text = "";
        RegisterVerifyPassword_INPT.text = "";
    }
    #endregion

#region Profile
    public void ChangeUserName_Confirm_BTN_CLick()
    {
        if (ProfileChange_INPT_Box.New_INPT.text.Length > 3)
        {
            if (UserUtilities.UsernameClean(ProfileChange_INPT_Box.New_INPT.text))
            {
                UserUtilities.ChangeUsername(ProfileChange_INPT_Box.New_INPT.text);
                ProfileChange_INPT_Box_Click();
                Profile_PNL.UpdateProfile();
            }
            else
            {
                DisplayNotification(true, "Username inappropriate");
            }
        }
        else
        {
            DisplayNotification(true, "Username should be 3 or more characters long");
        }
    }

    public void ChangeEmail_Confirm_BTN_Click()
    {
        if (UserUtilities.IsAnEMail(ProfileChange_INPT_Box.New_INPT.text))
        {
            UserUtilities.ChangeEmail(ProfileChange_INPT_Box.New_INPT.text);
            ProfileChange_INPT_Box_Click();
            Profile_PNL.UpdateProfile();
        }
        else
        {
            DisplayNotification(true, "Not a valid Email format");
        }
    }

    public void ChangePassword_Confirm_BTN_Click()
    {
        if (UserUtilities.PasswordMatch(ProfileChange_INPT_Box.New_INPT.text, ProfileChange_INPT_Box.Confirm_INPT.text))
        {
            if (UserUtilities.PasswordViability(ProfileChange_INPT_Box.New_INPT.text))
            {
                UserUtilities.ChangePassword(ProfileChange_INPT_Box.New_INPT.text);
                ProfileChange_INPT_Box_Click();
                Profile_PNL.UpdateProfile();
            }
            else
            {
                DisplayNotification(true, "Password needs to be " + UserUtilities.minPassLength + " - " + UserUtilities.MaxInputLength + " characters long and contain 1 uppercase, 1 symbol, & 1 number");
            }
        }
        else
        {
            DisplayNotification(true, "Passwords do not match");
        }
    }

    public void ProfileChangeInputDisplay(string buttonName)
    {

        switch (buttonName)
        {
            case "ChangeUsername_BTN":
                ProfileChange_INPT_Box.Current_INPT.contentType = InputField.ContentType.Alphanumeric;
                ProfileChange_INPT_Box.Current_LBL.text = "Current Username";
                ProfileChange_INPT_Box.Current_PlaceHolder.text = "username...";
                ProfileChange_INPT_Box.New_INPT.contentType = InputField.ContentType.Alphanumeric;
                ProfileChange_INPT_Box.New_LBL.text = "New Username";
                ProfileChange_INPT_Box.New_PlaceHolder.text = "username...";
                //ProfileChange_INPT_Box.Confirm_INPT.contentType = InputField.ContentType.Alphanumeric;
                ProfileChange_INPT_Box.Confirm_INPT.gameObject.SetActive(false);
                ProfileChange_INPT_Box.Confirm_LBL.gameObject.SetActive(false);
                //ProfileChange_INPT_Box.Confirm_LBL.text = "Confirm Username";
                //ProfileChange_INPT_Box.Confirm_PlaceHolder.text = "username...";
                ProfileChange_INPT_Box.OK_BTN.onClick.RemoveAllListeners();
                ProfileChange_INPT_Box.OK_BTN.onClick.AddListener(ChangeUserName_Confirm_BTN_CLick);
                break;
            case "ChangeEmail_BTN":
                ProfileChange_INPT_Box.Current_INPT.contentType = InputField.ContentType.EmailAddress;
                ProfileChange_INPT_Box.Current_LBL.text = "Current Email";
                ProfileChange_INPT_Box.Current_PlaceHolder.text = "email...";
                ProfileChange_INPT_Box.New_INPT.contentType = InputField.ContentType.EmailAddress;
                ProfileChange_INPT_Box.New_LBL.text = "New Email";
                ProfileChange_INPT_Box.New_PlaceHolder.text = "email...";
                //ProfileChange_INPT_Box.Confirm_INPT.contentType = InputField.ContentType.EmailAddress;
                ProfileChange_INPT_Box.Confirm_INPT.gameObject.SetActive(false);
                ProfileChange_INPT_Box.Confirm_LBL.gameObject.SetActive(false);
                //ProfileChange_INPT_Box.Confirm_LBL.text = "Confirm Email";
                //ProfileChange_INPT_Box.Confirm_PlaceHolder.text = "email...";
                ProfileChange_INPT_Box.OK_BTN.onClick.RemoveAllListeners();
                ProfileChange_INPT_Box.OK_BTN.onClick.AddListener(ChangeEmail_Confirm_BTN_Click);
                break;
            case "ChangePassword_BTN":
                ProfileChange_INPT_Box.Current_INPT.contentType = InputField.ContentType.Password;
                ProfileChange_INPT_Box.Current_LBL.text = "Current Password";
                ProfileChange_INPT_Box.Current_PlaceHolder.text = "password...";
                ProfileChange_INPT_Box.New_INPT.contentType = InputField.ContentType.Password;
                ProfileChange_INPT_Box.New_LBL.text = "New Password";
                ProfileChange_INPT_Box.New_PlaceHolder.text = "password...";
                ProfileChange_INPT_Box.Confirm_INPT.contentType = InputField.ContentType.Password;
                ProfileChange_INPT_Box.Confirm_INPT.gameObject.SetActive(true);
                ProfileChange_INPT_Box.Confirm_LBL.gameObject.SetActive(true);
                ProfileChange_INPT_Box.Confirm_LBL.text = "Confirm Password";
                ProfileChange_INPT_Box.Confirm_PlaceHolder.text = "password...";
                ProfileChange_INPT_Box.OK_BTN.onClick.RemoveAllListeners();
                ProfileChange_INPT_Box.OK_BTN.onClick.AddListener(ChangePassword_Confirm_BTN_Click);
                break;
        }
        ProfileChange_INPT_Box.gameObject.SetActive(true);
    }

    public void ProfileChange_INPT_Box_Click()
    {
        ProfileChange_INPT_Box.gameObject.SetActive(false);
    }
    #endregion

#region Utility
    public void DisplayNotification(bool on,string message = "")
    {
        NotificationMessage_TXT.text = message;
        Notification_PNL.SetActive(on);
    }

    public void Notification_PNL_Click()
    {
        DisplayNotification(false);
    }
    #endregion

#region Check In
    private void CreateProductList()
    {
        if (ProductList != null)
        {
            foreach (GameObject go in ProductList)
            {
                Destroy(go);
            }
        }

        ProductList = new List<GameObject>();

        foreach (Product prod in Products.List())
        {
            GameObject newProd = Instantiate(RewardListPrefab, RewardScrollObject.transform);
            newProd.GetComponent<ProductListObject>().Icon.sprite = ProtuctIcons[(int)prod.Organization];
            newProd.GetComponent<ProductListObject>().Name.text = prod.ProductName;
            newProd.GetComponent<ProductListObject>().Cost.text = prod.TokenValue.ToString();
            ProductList.Add(newProd);
        }
        ResetScroll(RewardScrollRect);
    }


#endregion

#region QR
    private void OnEnable()
    {
        QRCodeManager.onError += HandleOnError;
        QRCodeManager.onQrCodeFound += HandleOnQRCodeFound;
    }

    private void OnDisable()
    {
        QRCodeManager.onError -= HandleOnError;
        QRCodeManager.onQrCodeFound -= HandleOnQRCodeFound;
    }

    private void HandleOnError(string e)
    {
        QRCodeManager.Instance.StopScanning();
#if UNITY_EDITOR
        Debug.LogError(e);
#endif
    }
    // QR found by cam handler
    private void HandleOnQRCodeFound(ZXing.BarcodeFormat barCodeType, string barCodeValue)
    {
        QRCodeManager.Instance.StopScanning();
        Product prod = Product.Parse(barCodeValue);
        if (prod == null)
        {
            QRCode qr = QRCode.QRParse(barCodeValue);
            if (ValidQR(qr.RawData))
            {
                UserUtilities.AllocatePoints(qr.TokenValue);
                IndicateScore(qr.TokenValue, true);
                SoundManager.Instance.PlaySound("CoinCollect");

                GameManager.Instance.analytics.LogEvent(new EventHitBuilder().SetEventCategory("Token Distribution").SetEventAction(qr.QRType.ToString()).SetEventValue(qr.TokenValue));

                if (qr.QRType != QRCode.QRTypes.Tokens)
                {
                    GameManager.Organizations Org = (GameManager.Organizations)Enum.Parse(typeof(GameManager.Organizations), qr.QRType.ToString());
                    if (GameManager.Instance.MiniGameEnabledOrgs.Contains(Org))
                    {
                        GameManager.Instance.Unlock(Org);
                    }
                }
            }
            else
            {
                DisplayNotification(true, "You have already redeemed this reward today.");
            }
        }
        else
        {
            GameObject Popup = Instantiate(Confirmtion_Prefab, gameObject.transform);
            ConfirmationPopup conf = Popup.GetComponent<ConfirmationPopup>();
            conf.prod = prod;
            conf.Message.text = "Are you certain you would like to spend " + prod.TokenValue + " Tokens to recieve " + prod.ProductName + ".";
        }

        List_BTN.interactable = false;
        List_Tab.SetActive(true);
        Scan_BTN.interactable = true;
        Scan_Tab.SetActive(false);
    }

    // Begins QR scan
    public void Scan_BTN_Click()
    {
        try
        {
            List_BTN.interactable = true;
            List_Tab.SetActive(false);
            Scan_BTN.interactable = false;
            Scan_Tab.SetActive(true);
            Quest_BTN.interactable = true;
            QuestList.SetActive(false);

            QRCodeManager.CameraSettings camSettings = new QRCodeManager.CameraSettings();
#if UNITY_EDITOR
            string camName = (WebCamTexture.devices.Length > 0) ? WebCamTexture.devices[0].name : null;
#else
            string camName = GetFrontCamName();
#endif
            if (camName != null)
            {
                camSettings.deviceName = camName;
                camSettings.maintainAspectRatio = true;
                camSettings.scanType = ScanType.ONCE;

                QRCodeManager.Instance.ScanQRCode(camSettings, CamImage, 1f);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    // Switches to list and turns off QR scan
    public void RewardList_BTN()
    {
        QRCodeManager.Instance.StopScanning();

        List_BTN.interactable = false;
        List_Tab.SetActive(true);
        Scan_BTN.interactable = true;
        Scan_Tab.SetActive(false);
        Quest_BTN.interactable = true;
        QuestList.SetActive(false);
    }

    // Switches to the Quest list tab turns off others
    public void QuestList_BTN()
    {
        List_BTN.interactable = true;
        List_Tab.SetActive(false);
        Scan_BTN.interactable = true;
        Scan_Tab.SetActive(false);
        Quest_BTN.interactable = false;
        QuestList.SetActive(true);
    }

    // Get front facing camera
    private string GetFrontCamName()
    {
        foreach (WebCamDevice device in WebCamTexture.devices)
        {
            if (!device.isFrontFacing)
            {
                return device.name;
            }
        }
        return null;
    }

    // Write and timestamps the used QR to log
    private void WriteToQLog(string log)
    {
        using (StreamWriter qlogFile = new StreamWriter(Application.persistentDataPath + QRLogPath, true))
        {
            qlogFile.WriteLine(DateTime.Now + log);
        }
    }

    // check the qr log for a qr and when it was last logged
    private bool ValidQR(string qr)
    {
        if (File.Exists(Application.persistentDataPath + QRLogPath))
        {
            string line = string.Empty;
            List<string> lines = new List<string>();
            StreamReader sr = new StreamReader(Application.persistentDataPath + QRLogPath);
            while((line = sr.ReadLine()) != null)
            {
                lines.Add(line);
            }
            sr.Close();

            if (lines.Count > 0)
            {
                int logLine = 0;
                foreach(string ln in lines)
                {
                    if(ln.Contains(qr))
                    {
                        line = ln;
                        break;
                    }
                    logLine++;
                }

                if (line != null)
                {
                    line = line.Replace(qr, "");

                    DateTime time = DateTime.Parse(line);

                    if (DateTime.Now < (time + QRLockout))
                    {
                        return false;
                    }
                    lines.RemoveAt(logLine);
                    File.Delete(Application.persistentDataPath + QRLogPath);
                    FileStream fs1 = File.Create(Application.persistentDataPath + QRLogPath);
                    fs1.Close();
                    File.WriteAllLines(Application.persistentDataPath + QRLogPath, lines.ToArray());
                }
                WriteToQLog(qr);
            }
            else
            {
                WriteToQLog(qr);
            }
            return true;
        }
        FileStream fs2 = File.Create(Application.persistentDataPath + QRLogPath);
        fs2.Close();
        WriteToQLog(qr);
        return true;
    }
#endregion

#region Events
    private void CreateEventList()
    {
        if (EventPanels != null)
        {
            foreach (GameObject go in EventPanels)
            {
                Destroy(go);
            }
        }

        EventPanels = new List<GameObject>();

        foreach (CalendarEvent evt in Events.List())
        {
            GameObject newEvent = Instantiate(Event_Prefab, EventScrollObject.transform);
            newEvent.GetComponent<EventUIObject>().Event = evt;
            EventPanels.Add(newEvent);
        }
        ResetScroll(EventScrollRect);
    }

    public void EventPanelShow(ref CalendarEvent evnt)
    {
        EventDetails_Popup.GetComponent<DetailsUIObject>().Title.text = evnt.Name;
        EventDetails_Popup.GetComponent<DetailsUIObject>().Start.text = evnt.GetStart().Date.ToString("MM/dd/yyyy HH:mm");
        EventDetails_Popup.GetComponent<DetailsUIObject>().End.text = evnt.GetEnd().Date.ToString("MM/dd/yyyy HH:mm");
        EventDetails_Popup.GetComponent<DetailsUIObject>().Details.text = evnt.Details;

        EventDetails_Popup.SetActive(true);
    }

    private void ResetScroll(ScrollRect scrollObject)
    {
        scrollObject.normalizedPosition = new Vector2(0, 1);
    }
#endregion

#region Games
    private void CreateGameList()
    {
        if(MinigamePanels != null)
        foreach(GameObject go in MinigamePanels)
        {
            Destroy(go);
        }

        MinigamePanels = new List<GameObject>();

        int count = 0;
        foreach (GameObject go in GameManager.Instance.MiniGame_Objects)
        {
            GameObject newGame = Instantiate(MinigameListing_Prefab, MinigameScroll_Object.transform);
            newGame.GetComponent<MinigameBTNObject>().GameName = go.GetComponent<BaseGame>().Name;
            if(!MinigameData.FileExists(newGame.GetComponent<MinigameBTNObject>().GameName))
            {
                MinigameData MGD = new MinigameData();
                MGD.GameName = newGame.GetComponent<MinigameBTNObject>().GameName;
                MGD.TimeActivated = new DateTime(1999, 12, 31).ToString();
                MGD.Highscore = 0;
                MGD.Org = (GameManager.Organizations)count;
                MGD.SaveGameData();
            }
            MinigamePanels.Add(newGame);
            count++;
        }
    }
#endregion

#region Map
    // Spawns a floating score indicator
    public void IndicateScore(int score, bool gain)
    {
        GameObject go = Instantiate(ScoreIndicator_Prefab, gameObject.transform);
        go.GetComponent<ScoreIndicator>().Init(score, gain);
    }

    // Smoothly moves the map to position
    public void SmoothMap()
    {
        if (Input.GetMouseButton(0) && UpdateMap)
        {
            UpdateMap = false;
        }

        if (UpdateMap)
        {
            if (OnlineMaps.instance.position != OnlineMapsLocationService.instance.position)
            {
                OnlineMaps.instance.position = Vector2.MoveTowards(OnlineMaps.instance.position, OnlineMapsLocationService.instance.position, Time.deltaTime * 0.01f);                
            }
        }
        else
        {
            MapUpdateTimer += Time.deltaTime;
            if (MapUpdateTimer >= TimeBeforeMapUpdate)
            {
                MapUpdateTimer = 0;
                UpdateMap = true;
            }
        }
    }
    #endregion
}
