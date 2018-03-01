using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QRCode
{
    /// <summary> Enum of organizations a QR organization </summary>
    public enum QRTypes { MPL, TOM, Heritage, Conservation, Tokens }
    /// <summary> This QRCodes organization </summary>
    public QRTypes QRType;
    /// <summary> Azure container name </summary>
    private const string QRCodeContainer = "qrcodes"; // REQUIRED-FIELD : Azure Blob Container for QR codes
    /// <summary> How many tokens this QR awards </summary>
    public int TokenValue;
    /// <summary> The original raw data from the QR </summary>
    public string RawData;

    // Constructor
    public QRCode(QRTypes type, int tokens)
    {
        QRType = type;
        TokenValue = tokens;
    }

    // Converts raw string data into a QRCode
    public static QRCode QRParse(string rawData)
    {
        QRTypes type = QRTypes.Tokens;
        int tokens = 0;


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
                            type = (QRTypes)Enum.Parse(typeof(QRTypes), tempString);
                            break;
                        case "<TOKENS>":
                            tokens = int.Parse(tempString);
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
        QRCode returnCode = new QRCode(type, tokens);
        returnCode.RawData = rawData;
        return returnCode;
    }

    // Confirms the QR code with the server
    public static bool ConfirmQRCode(string qrData)
    {
        try
        {
            string rawData = GameManager.Azure.PullBlob(QRCodeContainer, qrData);
            if(rawData != "")
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return false;
    }
}
