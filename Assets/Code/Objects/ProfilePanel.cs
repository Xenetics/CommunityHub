using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    private static ProfilePanel instance;
    public static ProfilePanel Instance { get { return instance; } }
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

    public Text Tokens;
    public Text ID;
    public Text Username;
    public Text Email;

    public void UpdateProfile()
    {
        Tokens.text = UserUtilities.User.CurrentPoints.ToString();
        ID.text = UserUtilities.User.CardNumber;
        Username.text = UserUtilities.User.Username;
        Email.text = UserUtilities.User.EMail;
    }
}
