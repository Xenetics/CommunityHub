using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpScreen : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Panels;
    private int Current_PNL = 0;
    [SerializeField]
    private Button Back_BTN;
    [SerializeField]
    private Button Forward_BTN;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Current_PNL = 0;
        foreach (GameObject go in Panels)
        {
            go.SetActive(false);
        }
        Panels[0].SetActive(true);
        Back_BTN.gameObject.SetActive(false);
        Forward_BTN.gameObject.SetActive(true);
    }

    public void LastPanel()
    {
        Forward_BTN.gameObject.SetActive(true);
        if (Current_PNL > 0)
        {
            Panels[Current_PNL].SetActive(false);
            Current_PNL--;
            Panels[Current_PNL].SetActive(true);
            if (Current_PNL == 0)
            {
                Back_BTN.gameObject.SetActive(false);
            }
        }
    }

    public void NextPanel()
    {
        Back_BTN.gameObject.SetActive(true);
        if (Current_PNL < Panels.Count - 1)
        {
            Panels[Current_PNL].SetActive(false);
            Current_PNL++;
            Panels[Current_PNL].SetActive(true);
            if (Current_PNL == Panels.Count - 1)
            {
                Forward_BTN.gameObject.SetActive(false);
            }
        }
    }
}

