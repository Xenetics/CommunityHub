using System;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class RestHelper
{
    /// <summary> Object that holds the auth recived from server upon request </summary>
    [Serializable]
    public class AuthObject
    {
        /// <summary> The token used to access the server for further calls </summary>
        public string access_token;
        /// <summary> The type of token recived from server </summary>
        public string token_type;
        /// <summary> How long until this authorization expires </summary>
        public string expires_in;
    }

    /// <summary> The sierra server URL </summary>
    private static string m_url = ""; // REQUIRED-FIELD : Same url as admin
    /// <summary> The Auth secret used to access data on sierra </summary>
    private static string m_authSecret = ""; // REQUIRED-FIELD : Same url as admin
    /// <summary> This is the last valid authobject in use </summary>
    public static AuthObject currentAuth;
    /// <summary> Last time of auth previous to the current auth </summary>
    public static DateTime lastAuth { private set; get; }

    /// <summary> Authenticate with the sierra server setting the current auth code </summary>
    public static void Authenticate()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        string url = m_url;
        url += "/token";
        byte[] buffer = Encoding.UTF8.GetBytes("grant_type=client_credentials");

        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = buffer.Length;
        request.Headers["Authorization"] = "Basic " + m_authSecret;

        Stream stream = request.GetRequestStream();
        stream.Write(buffer, 0, buffer.Length);
        stream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
        {
            string result = sr.ReadToEnd();
            try
            {
                currentAuth = JsonUtility.FromJson<AuthObject>(result);
                lastAuth = DateTime.Now;
            }
            catch (Exception e)
            {
                Debug.Log("Error >>>>>>>>> " + e.Message);
            }
        }
    }

    /// <summary> Gets a user from the sierra databasefor comparison to confirm existence </summary>
    public static bool GetUser(string libCard)
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        string url = m_url;
        url += "/patrons";
        url += "/find?barcode=";
        url += libCard;

        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
        request.Method = "GET";
        request.ContentType = "application/json;charset=UTF-8";
        request.Accept = "application/json";
        request.Headers["Authorization"] = currentAuth.token_type + " " + currentAuth.access_token;

        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error >>>>>>>>> " + e.Message);
            return false;
        }
            
        return false;
    }

    public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
}