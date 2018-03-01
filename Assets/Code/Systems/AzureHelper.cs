using UnityEngine;
using System.Linq;
using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Xml;

public class AzureHelper
{
    // Constructor
    public AzureHelper(string storageAccount)
    {
        BlobHelper = new AzureStorageConsole.BlobHelper(AzureStorageConsole.RESTHelper.GetAccount(storageAccount));
        TableHelper = new AzureStorageConsole.TableHelper(AzureStorageConsole.RESTHelper.GetAccount(storageAccount));
    }

    private AzureStorageConsole.BlobHelper BlobHelper;
    private AzureStorageConsole.TableHelper TableHelper;

    #region Blob
    // Creates a Blob container with specific name
    public void CreateBlobContainer(string name)
    {
        BlobHelper.CreateContainer(name);
    }

    // Gets a list of blobs in the container
    public List<string> ListBlobsInContainer(string name)
    {
        return BlobHelper.ListBlobs(name);
    }

    // Pushes a blob to a specified container with a specified content name
    public void PushBlob(string containerName, string cardNum, string rawData)
    {
        BlobHelper.PutBlob(containerName, cardNum, rawData);
    }

    // Pulls a blob from a specified container with a specified content name
    public string PullBlob(string containerName, string blobName)
    {
        string rawData;
        HttpWebResponse response = BlobHelper.GetBlob(containerName, blobName);
        Stream inputStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(inputStream);
        rawData = reader.ReadToEnd();
        reader.Close();
        return rawData;
    }

    // Replaces a blob with a specified name in a specified container with another blob with specified name
    public void ReplaceBlob(string containerName, string contentString, string newContentString)
    {
        BlobHelper.DeleteBlob(containerName, contentString);

        BlobHelper.PutBlob(containerName, newContentString, newContentString);
    }

    // Deletes a blob with a specified name in a container of a specified name
    public void DeleteBlob(string containerName, string contentString)
    {
        BlobHelper.DeleteBlob(containerName, contentString);
    }
    #endregion

    #region Table
    // Creates a table with specified name
    public void CreateTable(string name)
    {
        TableHelper.CreateTable(name);
    }

    // Inserts a new row object with specified partition and row keys
    public void InsertRow(string tableName, string partitionKey, string rowKey, object obj)
    {
        TableHelper.InsertRow(tableName, partitionKey, rowKey, obj);
    }

    // Gets a single partition of given table using the given partition key
    public string[] GetRow(string tableName, string partitionKey, string rowKey)
    {
        return TableHelper.GetPartition(tableName, partitionKey, rowKey);
    }

    //// Gets a table by name
    //public string[] GetTable(string tableName)
    //{
    //    return TableHelper.GetTable(tableName);
    //}

    //// Gets a table entity in a specified table by using a partition key
    //public CalendarEvent[] GetByPartitionKey(string tableName, string partitionKey)
    //{
    //    string[] table = TableHelper.GetTable(tableName);
    //    CalendarEvent evnt = new CalendarEvent();

    //    //foreach(string rawData in table)
    //    //{
    //    //    CalendarEvent evnt;
    //    //    DateTime start;
    //    //    DateTime end;
    //    //    string name;
    //    //    string detains;
    //    //    string org;

    //    //    bool tagged = false;
    //    //    string stringTag = "";
    //    //    string tempString = "";
    //    //    for (int i = 0; i < rawData.Length; i++)
    //    //    {
    //    //        if (!tagged)
    //    //        {
    //    //            stringTag += rawData[i];
    //    //            if (rawData[i] == '>')
    //    //            {
    //    //                tagged = true;
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            if (rawData[i] != '<')
    //    //            {
    //    //                tempString += rawData[i];
    //    //            }
    //    //            if (rawData[i] == '<' || i + 1 == rawData.Length)
    //    //            {
    //    //                switch (stringTag)
    //    //                {
    //    //                    case "<CARD>":
    //    //                        card = tempString;
    //    //                        break;
    //    //                    case "<USER>":
    //    //                        name = tempString;
    //    //                        break;
    //    //                    case "<PASS>":
    //    //                        pass = tempString;
    //    //                        break;
    //    //                    case "<EMAIL>":
    //    //                        email = tempString;
    //    //                        break;
    //    //                    case "<CURRENT>":
    //    //                        current = int.Parse(tempString);
    //    //                        break;
    //    //                    case "<TOTAL>":
    //    //                        total = int.Parse(tempString);
    //    //                        break;
    //    //                    case "<LASTMOD>":
    //    //                        lastmod = long.Parse(tempString);
    //    //                        break;
    //    //                    case "<STATUS>":
    //    //                        status = tempString;
    //    //                        break;
    //    //                    default:
    //    //                        break;
    //    //                }
    //    //                stringTag = "<";
    //    //                tempString = "";
    //    //                tagged = false;
    //    //            }
    //    //        }
    //    //    }
    //    //}

    //    List<CalendarEvent> returnlist = new List<CalendarEvent>();
    //    return returnlist.ToArray();
    //}

    // Gets a table entity in a specified table by using a partition key containing a string
    public CalendarEvent[] GetEventsByPartitionKeyContains(string tableName, string partitionKeyStart)
    {
        XmlNodeList nodes = TableHelper.GetTable(tableName);
        List<CalendarEvent> returnlist = new List<CalendarEvent>();

        foreach (XmlNode node in nodes)
        {
            
            CalendarEvent evnt = new CalendarEvent();
            evnt.PartitionKey = node.ChildNodes[0].InnerText;
            evnt.RowKey = node.ChildNodes[1].InnerText;
            evnt.Start = node.ChildNodes[3].InnerText;
            evnt.End = node.ChildNodes[4].InnerText;
            evnt.Name = node.ChildNodes[5].InnerText;
            evnt.Details = node.ChildNodes[6].InnerText;
            evnt.Org = node.ChildNodes[7].InnerText;
            if (int.Parse(evnt.PartitionKey) >= int.Parse(partitionKeyStart))
            {
                returnlist.Add(evnt);
            }
        }
        return returnlist.ToArray();
    }

    // Gets a table entity in a specified table by using a partition key containing a string
    public TriviaQuestion[] GetQuestionsByPartitionKeyContains(string tableName, string partitionKey)
    {
        XmlNodeList nodes = TableHelper.GetTable(tableName);
        List<TriviaQuestion> returnlist = new List<TriviaQuestion>();

        foreach (XmlNode node in nodes)
        {
            TriviaQuestion que = new TriviaQuestion();
            que.PartitionKey = node.ChildNodes[0].InnerText;
            que.RowKey = node.ChildNodes[1].InnerText;
            que.Org = node.ChildNodes[3].InnerText;
            que.Location = node.ChildNodes[4].InnerText;
            que.Question = node.ChildNodes[5].InnerText;
            que.AnswerA = node.ChildNodes[6].InnerText;
            que.AnswerB = node.ChildNodes[7].InnerText;
            que.AnswerC = node.ChildNodes[8].InnerText;
            que.AnswerD = node.ChildNodes[9].InnerText;
            que.CorrectAnswer = node.ChildNodes[10].InnerText;
            que.Value = int.Parse(node.ChildNodes[11].InnerText);

            returnlist.Add(que);
        }
        return returnlist.ToArray();
    }
    #endregion
}

