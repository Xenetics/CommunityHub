using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct MinigameData
{
    // Game of the game that corresponds with the Mini game name set on minigame script
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string GameName;
    // Time when the game was last activated
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string TimeActivated;
    // The highest score achieved in the minigame
    public int Highscore;
    // This game belongs to this org
    public GameManager.Organizations Org;

    // Activates game for 24 hours
    public static void Activate(string gameName)
    {
        MinigameData mgd = RetrieveData(gameName);
        mgd.TimeActivated = DateTime.Now.ToString();
        mgd.SaveGameData();
    }

    // Returns if the game is still in its active timespan
    public bool IsActive()
    {
        if(LastActivated() + GameManager.MinigameActiveTime < DateTime.Now)
        {
            return false;
        }
        return true;
    }

    /// <summary> Saves the game data to file </summary>
    public void SaveGameData()
    {
        SaveData(this);
    }

    public static MinigameData LoadGameData(string gameName)
    {
        return RetrieveData(gameName);
    }

    /// <summary> Serializes the data for the remember me </summary>
    public static void SaveData(MinigameData MGData)
    {
        try
        {
            byte[] data = DataToBytes(MGData);
            FileStream fs = new FileStream(Application.persistentDataPath + "/" + MGData.GameName + ".dat", FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    /// <summary> Simply returns true if there is remember me data and false if not </summary>
    public static bool FileExists(string gameName)
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/" + gameName + ".dat"))
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return false;
    }

    /// <summary> Retrieves and deserializes the remember me data for use if it exists </summary>
    public static MinigameData RetrieveData(string gameName)
    {
        try
        {
            if (FileExists(gameName))
            {
                FileStream fs = new FileStream(Application.persistentDataPath + "/" + gameName + ".dat", FileMode.Open);
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();
                MinigameData MGD = BytesToData(data);
                return MGD;
            }
            else
            {
                MinigameData MGD = new MinigameData();
                MGD.GameName = gameName;
                MGD.TimeActivated = DateTime.Now.ToString();
                MGD.Highscore = 0;
                SaveData(MGD);
                return MGD;
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return new MinigameData();
    }

    /// <summary> Converts a MinigameData to Bytes for storage </summary>
    private static byte[] DataToBytes(MinigameData MGData)
    {
        try
        {
            int size = Marshal.SizeOf(MGData);
            byte[] buffer = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(MGData, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);

            return buffer;
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return new byte[0];
    }

    /// <summary> Converts Bytes to MinigameData for use </summary>
    private static MinigameData BytesToData(byte[] bytes)
    {
        try
        {
            MinigameData ud = new MinigameData();
            int size = Marshal.SizeOf(ud);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            ud = (MinigameData)Marshal.PtrToStructure(ptr, ud.GetType());
            Marshal.FreeHGlobal(ptr);
            return ud;
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return new MinigameData();
    }

    // Converts sting activated time to date time
    private DateTime LastActivated()
    {
        return DateTime.Parse(TimeActivated);
    }
}
