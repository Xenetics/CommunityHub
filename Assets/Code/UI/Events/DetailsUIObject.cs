using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailsUIObject : MonoBehaviour
{
    public Text Title;
    public Text Start;
    public Text End;
    public Text Details;

    public void Clicked()
    {
        gameObject.SetActive(false);
    }
}
