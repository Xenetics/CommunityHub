using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EULAPopup : MonoBehaviour
{
    public Scrollbar ScrollBar;
    public Button Accept_BTN;

    public void ScrollValue()
    {
        if (ScrollBar.value == 0 && Accept_BTN.interactable == false)
        {
            Accept_BTN.interactable = true;
        }
    }

    public void DeclineBTN()
    {
        Application.Quit();
    }

    public void AcceptBTN()
    {
        UIManager.Instance.ShowEULA = false;
        UserUtilities.Save();
        Destroy(gameObject);
    }
}
