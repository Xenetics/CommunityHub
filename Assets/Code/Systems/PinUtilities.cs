using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class PinUtilities
{
    /// <summary> The name of the container on the blob store </summary>
    public static string containerName = "pois"; // REQUIRED-FIELD : Azure Blob Container for map points of interest
    /// <summary> Dictionary of the the longs that represent when a pin was last activated </summary>
    public static Dictionary<string, long> pinDeltas;
    /// <summary> Dictionary of the the longs that represent when a token pin was last activated </summary>
    public static Dictionary<string, long> tokenPinDeltas;
    /// <summary> Time in miliseconds </summary>
    public static long pinCooldown = 5400000;//5400000;
    /// <summary> Time in miliseconds </summary>
    public static long tokenPinCooldown = 5400000;//5400000;
    /// <summary> The percent chance out of 100 that a token pin will be a jackpot </summary>
    public static int percentJackpot = 5;
    public static Color playerCircleColor = new Color(200, 0, 255, 50);
    public static Color pinCircleColor = new Color(255, 200, 0, 50);

    // MAP
    public static PointData MyPosition;
    public static List<PointData> PointDatas;
    public static List<PointData> TokenPointDatas;

    /// <summary> Retrieves the point data from azure and prepares it for use </summary>
    public static void GetPointData()
    {
        string erBlob = "";
        try
        {
            List<string> blobs = GameManager.Azure.ListBlobsInContainer(containerName);
            blobs.Sort();
            PointDatas = new List<PointData>();
            TokenPointDatas = new List<PointData>();

            foreach (string blob in blobs)
            {
                erBlob = blob;
                PointData pd = ParsePinData(blob);
                if (pd._Type != PointData.PinType.Tokens)
                {
                    PointDatas.Add(pd);
                }
                else
                {
                    TokenPointDatas.Add(pd);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message + "/n" + erBlob);
        }
    }

    /// <summary> Adds pins to map based on point data and deltas </summary>
    public static void AddPins()
    {
        try
        {
            foreach (PointData data in PointDatas)
            {
                if (DateTime.Now > data._StartDate && DateTime.Now < data._EndDate)
                {
                    DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    long unixTime = (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
                    if (!File.Exists(Application.persistentDataPath + "/delta.dat"))
                    {
                        try
                        {
                            OnlineMaps.instance.AddMarker(new Vector2((float)data._Position.Longitude, (float)data._Position.Latitude), data._Icon, data._Label);
                            data._Pin = OnlineMaps.instance.markers[OnlineMaps.instance.markers.Length - 1];
                            data._Pin.OnClick = GameManager.Instance.Pin_Click;
                            data._Pin.align = OnlineMapsAlign.Bottom;
                            data._Pin.scale = 0.5f;
                            data._Pin.tags.Add("<lat>" + data._Position.Latitude.ToString());
                            data._Pin.tags.Add("<lon>" + data._Position.Longitude.ToString());
                            data._Pin.tags.Add("<type>" + data._Type);
                        }
                        catch(Exception e)
                        {
                            Debug.Log("ERROR: " + e.Message);
                        }
                    }
                    else
                    {
                        OnlineMaps.instance.AddMarker(new Vector2((float)data._Position.Longitude, (float)data._Position.Latitude), data._Icon, data._Label);
                        data._Pin = OnlineMaps.instance.markers[OnlineMaps.instance.markers.Length - 1];
                        data._Pin.OnClick = GameManager.Instance.Pin_Click;
                        data._Pin.align = OnlineMapsAlign.Bottom;
                        data._Pin.scale = 0.5f;
                        data._Pin.tags.Add("<lat>" + data._Position.Latitude.ToString());
                        data._Pin.tags.Add("<lon>" + data._Position.Longitude.ToString());
                        data._Pin.tags.Add("<type>" + data._Type);
                        if ((unixTime < (pinDeltas[data._Label] + pinCooldown)))
                        {
                            data.RemovePin();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    /// <summary> Adds token pins to map based on point data and token deltas </summary>
    public static void AddTokenPins()
    {
        System.Random rand = new System.Random();
        try
        {
            int i = 0;
            foreach (PointData data in TokenPointDatas)
            {
                if (DateTime.Now > data._StartDate && DateTime.Now < data._EndDate)
                {
                    DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    long unixTime = (long)(DateTime.UtcNow - epochStart).TotalSeconds;
                    if (!File.Exists(Application.persistentDataPath + "/tokendelta.dat"))
                    {
                        if (rand.Next(0, 100) < percentJackpot)
                        {
                            data.SetJackpot();
                        }
                        OnlineMaps.instance.AddMarker(new Vector2((float)data._Position.Longitude, (float)data._Position.Latitude), data._Icon, data._Label);
                        data._Pin = OnlineMaps.instance.markers[OnlineMaps.instance.markers.Length - 1];
                        data._Pin.OnClick = GameManager.Instance.Pin_Click;
                        data._Pin.align = OnlineMapsAlign.Bottom;
                        data._Pin.scale = 0.5f;
                        data._Pin.tags.Add("<lat>" + data._Position.Latitude.ToString());
                        data._Pin.tags.Add("<lon>" + data._Position.Longitude.ToString());
                        data._Pin.tags.Add("<type>" + data._Type);
                    }
                    else
                    {
                        if (rand.Next(0, 100) < percentJackpot)
                        {
                            data.SetJackpot();
                        }
                        OnlineMaps.instance.AddMarker(new Vector2((float)data._Position.Longitude, (float)data._Position.Latitude), data._Icon, data._Label);
                        data._Pin = OnlineMaps.instance.markers[OnlineMaps.instance.markers.Length - 1];
                        data._Pin.OnClick = GameManager.Instance.Pin_Click;
                        data._Pin.align = OnlineMapsAlign.Bottom;
                        data._Pin.scale = 0.5f;
                        data._Pin.tags.Add("<lat>" + data._Position.Latitude.ToString());
                        data._Pin.tags.Add("<lon>" + data._Position.Longitude.ToString());
                        data._Pin.tags.Add("<type>" + data._Type);
                        string key = data._Position.Latitude.ToString() + data._Position.Longitude.ToString();
                        if ((unixTime < (tokenPinDeltas[key] + pinCooldown)))
                        {
                            data.RemovePin();
                        }
                    }
                }
                i++;
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    /// <summary> parses the pin data into pointdata to be used </summary>
    public static PointData ParsePinData(string rawData)
    {
        try
        {
            PointData.PinType type = PointData.PinType.Generic;
            string label = "";
            double lat = 0;
            double lon = 0;
            string address = "";
            int value = 0;
            string startDate = "";
            string endDate = "";

            bool tagged = false;
            string stringTag = "";
            string tempString = "";
            for (int i = 0; i < rawData.Length; i++)
            {
                if (!tagged)
                {
                    stringTag += rawData[i];
                    if (rawData[i] == '>')
                    {
                        tagged = true;
                    }
                }
                else
                {
                    if (rawData[i] != '<')
                    {
                        tempString += rawData[i];
                    }
                    if (rawData[i] == '<' || i + 1 == rawData.Length)
                    {
                        switch (stringTag)
                        {
                            case "<TYPE>":
                                type = (PointData.PinType)Enum.Parse(typeof(PointData.PinType), tempString);
                                break;
                            case "<LABEL>":
                                label = tempString;
                                break;
                            case "<LAT>":
                                lat = double.Parse(tempString);
                                break;
                            case "<LONG>":
                                lon = double.Parse(tempString);
                                break;
                            case "<ADDRESS>":
                                address = tempString;
                                break;
                            case "<VALUE>":
                                value += int.Parse(tempString);
                                break;
                            case "<START>":
                                startDate = tempString;
                                break;
                            case "<END>":
                                endDate = tempString;
                                break;
                            default:
                                break;
                        }
                        stringTag = "<";
                        tempString = "";
                        tagged = false;
                    }
                }
            }
            if (type != PointData.PinType.Tokens)
            {
                return new PointData(type, label, new Position(lat, lon), address, value, startDate, endDate, true);
            }
            else
            {
                return new PointData(type, label, new Position(lat, lon), address, value, startDate, endDate, true);
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return null;
    }

    // Either Creates or retrievs the pin deltas
    public static void PinDelta()
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/delta.dat"))
            {
                DeserializeDeltas();
                // Remove Deltas for pins removed
                List<string> tempList = new List<string>();
                foreach (KeyValuePair<string, long> item in pinDeltas)
                {
                    bool found = false;
                    foreach (PointData pd in PointDatas)
                    {
                        if (item.Key == pd._Label)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        tempList.Add(item.Key);
                    }
                }
                foreach (string key in tempList)
                {
                    pinDeltas.Remove(key);
                }

                // Takes care of new pins addded via admin panel
                foreach (PointData point in PointDatas)
                {
                    if (!pinDeltas.ContainsKey(point._Label))
                    {
                        pinDeltas.Add(point._Label, 0);
                    }
                }
            }
            else
            {
                pinDeltas = new Dictionary<string, long>();
                foreach (PointData point in PointDatas)
                {
                    pinDeltas.Add(point._Label, 0);
                }
                SerializeDeltas();
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    // Either Creates or retrievs the token pin deltas
    public static void TokenPinDelta()
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/tokendelta.dat"))
            {
                DeserializeTokenDeltas();
                // Remove Deltas for pins removed
                List<string> tempList = new List<string>();
                foreach (KeyValuePair<string, long> item in tokenPinDeltas)
                {
                    bool found = false;
                    foreach (PointData pd in TokenPointDatas)
                    {
                        string key = pd._Position.Latitude.ToString() + pd._Position.Longitude.ToString();
                        if (item.Key == key)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        tempList.Add(item.Key);
                    }
                }
                foreach (string key in tempList)
                {
                    tokenPinDeltas.Remove(key);
                }

                // Takes care of new pins addded via admin panel
                foreach (PointData point in TokenPointDatas)
                {
                    string key = point._Position.Latitude.ToString() + point._Position.Longitude.ToString();
                    if (!tokenPinDeltas.ContainsKey(key))
                    {
                        tokenPinDeltas.Add(key, 0);
                    }
                }
                // this would be where you deserialize jackpot pins if needed
            }
            else
            {
                tokenPinDeltas = new Dictionary<string, long>();
                foreach (PointData point in TokenPointDatas)
                {
                    string key = point._Position.Latitude.ToString() + point._Position.Longitude.ToString();
                    tokenPinDeltas.Add(key, 0);
                }
                SerializeTokenDeltas();
                // this would be where you serialize jackpot if needed
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    // Deserializes the binary of deltas of the pins
    private static void DeserializeDeltas()
    {
        try
        {
            FileStream fs = File.Open(Application.persistentDataPath + "/delta.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            pinDeltas = (Dictionary<string, long>)bf.Deserialize(fs);
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    // Serializes the deltas of the pins to binay
    private static void SerializeDeltas()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Create(Application.persistentDataPath + "/delta.dat");
            bf.Serialize(fs, pinDeltas);
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    // Deserializes the binary of deltas of the Token pins
    private static void DeserializeTokenDeltas()
    {
        try
        {
            FileStream fs = File.Open(Application.persistentDataPath + "/tokendelta.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            tokenPinDeltas = (Dictionary<string, long>)bf.Deserialize(fs);
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    // Serializes the deltas of the token pins to binay
    private static void SerializeTokenDeltas()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Create(Application.persistentDataPath + "/tokendelta.dat");
            bf.Serialize(fs, tokenPinDeltas);
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    // Saves the pins via binary serialization
    public static void SavePins()
    {
        SerializeDeltas();
    }

    // Saves the token pins via binary serialization
    public static void SaveTokenPins()
    {
        SerializeTokenDeltas();
    }

    /// <summary> Takes in 2 lat/lon sets and returns the distence in meters </summary>
    public static double DistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        double er = 6378.137;
        double lat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
        double lon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
        double a = Math.Sin(lat / 2) * Math.Sin(lat / 2) + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(lon / 2) * Math.Sin(lon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double d = er * c;

        return d * 1000;
    }

    /// <summary>
    /// Finds a pointdata based on position in the given tags
    /// </summary>
    public static PointData GetPointData(List<string> tags, List<PointData> data)
    {
        double lat = 0.0;
        double lon = 0.0;
        foreach(string tag in tags)
        {
            if(tag.Contains("<lat>"))
            {
                string tmp = tag.Replace("<lat>", "");
                lat = double.Parse(tmp);
            }
            else if(tag.Contains("<lon>"))
            {
                string tmp = tag.Replace("<lon>", "");
                lon = double.Parse(tmp);
            }
        }

        for (int i = 0; i < data.Count; ++i)
        {
            Position tagLoc = new Position(lat, lon);

            if (data[i]._Position == tagLoc)
            {
                return data[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the type of pin from its tags
    /// </summary>
    public static string GetTypeFromTag(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag.Contains("<type>"))
            {
                string tmp = tag.Replace("<type>", "");
                return tmp;
            }
        }
        return "";
    }

    /// <summary>
    /// Finds a pointdata based on position in the givent data list
    /// </summary>
    public static PointData GetPointData(Position loc, List<PointData> data)
    {
        for(int i = 0; i < data.Count; ++i)
        {
            Position trunLoc = Truncate(data[i]._Position, loc);
            if(data[i]._Position == trunLoc)
            {
                return data[i];
            }
        }
        return null;
    }

    /// <summary>
    /// returns a position with doubles from b truncated to length of a's
    /// </summary>
    private static Position Truncate(Position a, Position b)
    {
        string lat = a.Latitude.ToString();
        string lon = a.Longitude.ToString();
        double latdec = Math.Pow(10.0, lat.Substring(lat.IndexOf(".")).Length);
        double londec = Math.Pow(10.0, lat.Substring(lon.IndexOf(".")).Length);

        return new Position((Math.Truncate(b.Latitude * latdec) / latdec), (Math.Truncate(b.Longitude * londec) / londec));
    }
}