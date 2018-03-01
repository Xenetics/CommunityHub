using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPlay : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Panels;
    private int Current_PNL = 0;

    public void NextPanel()
    {
        if (Current_PNL < Panels.Count - 1)
        {
            Panels[Current_PNL].SetActive(false);
            Current_PNL++;
            Panels[Current_PNL].SetActive(true);
            if(Current_PNL == Panels.Count - 1)
            {
                Panels[Current_PNL].GetComponentInChildren<Button>().gameObject.GetComponent<Image>().color = Color.red;
                Panels[Current_PNL].GetComponentInChildren<Button>().gameObject.GetComponentInChildren<Text>().text = "Done";
            }
        }
        else
        {
            UserUtilities.ActivateAccount();
            Destroy(this.gameObject);
        }
    }
}
