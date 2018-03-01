using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EventUIObject : MonoBehaviour
{
    public CalendarEvent Event;
    public Text Title;
    public Text StartTime;
    public Text EndTime;

    private void Start()
    {
        Title.text = Event.Name;
        StartTime.text = Event.GetStart().Date.ToString("MM/dd/yyyy HH:mm");
        EndTime.text = Event.GetEnd().Date.ToString("MM/dd/yyyy HH:mm");
    }

    public void Clicked()
    {
        UIManager.Instance.EventPanelShow(ref Event);
    }
}
