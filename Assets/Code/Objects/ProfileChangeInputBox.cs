using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileChangeInputBox : MonoBehaviour
{
    private static ProfileChangeInputBox instance;
    public static ProfileChangeInputBox Instance { get { return instance; } }
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

    public Text Current_LBL;
    public Text Current_PlaceHolder;
    public InputField Current_INPT;
    public Text New_LBL;
    public Text New_PlaceHolder;
    public InputField New_INPT;
    public Text Confirm_LBL;
    public Text Confirm_PlaceHolder;
    public InputField Confirm_INPT;
    public Button OK_BTN;
}
