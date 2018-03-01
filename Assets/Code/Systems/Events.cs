using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Events 
{
    /// <summary> List of events downloaded from the database </summary>
    private static List<CalendarEvent> m_Events;
    private static string EventsContainer = "events"; // REQUIRED-FIELD : Azure Blob Container for calander events

    /// <summary> Sets up the events Tab </summary>
    public static void InitEvents()
    {
        try
        {
            LoadEvents();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary> Retrieves events from the cloud, Stores the data and loads the ListView </summary>
    private static void LoadEvents()
    {
        m_Events = new List<CalendarEvent>(GameManager.Azure.GetEventsByPartitionKeyContains(EventsContainer, CalendarEvent.DateToPartitionMonth(DateTime.Today)));
        m_Events = m_Events.OrderBy(o => o.Start).ThenBy(o => o.Start).ToList();
    }

    public static List<CalendarEvent> List()
    {
        return m_Events;
    }
}