using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> Product that tokens can be used to redeem </summary>
public class Product
{
    /// <summary> Enum of organizations a product organization </summary>
    public enum ProductOrg { MPL, TOM, Heritage, Conservation }
    /// <summary> This Products organization </summary>
    public ProductOrg Organization;
    /// <summary> Azure container name </summary>
    private const string ProductsContainer = "products"; // REQUIRED-FIELD : Azure Blob Container for products
    /// <summary> The Name of the product </summary>
    public string ProductName;
    /// <summary> Full value of the product </summary>
    public string ProductValue;
    /// <summary> The % discount on this product </summary>
    public string Discount;
    /// <summary> The amount of tokens that this QR code is worth </summary>
    public long TokenValue;
    /// <summary> The raw string data </summary>
    public string RawData;

    // Constructor
    public Product() { }
    public Product(string name, string value, string discount, ProductOrg type, long tokenCost)
    {
        ProductName = name.Replace("/", " ");

        ProductValue = (value.Contains(".")) ? (value) : (value + ".00");
        Discount = discount;
        Organization = type;
        TokenValue = tokenCost;
        RawData = Encode(ProductName, ProductValue, Discount, Organization, TokenValue.ToString());
    }

    // Parses the raw data and returns a QR code
    public static Product Parse(string rawData)
    {
        try
        {
            string name = "";
            string value = "";
            string discount = "";
            ProductOrg type = ProductOrg.MPL;
            long tokens = 0;

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
                            case "<NAME>":
                                name = tempString;
                                break;
                            case "<VALUE>":
                                value = tempString;
                                break;
                            case "<DISCOUNT>":
                                discount = tempString;
                                break;
                            case "<TYPE>":
                                type = (ProductOrg)Enum.Parse(typeof(ProductOrg), tempString);
                                break;
                            case "<TOKENS>":
                                tokens = long.Parse(tempString);
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
            Product returnCode = new Product(name, value, discount, type, tokens);
            returnCode.RawData = rawData;
            if (returnCode.ProductName != "")
            {
                return returnCode;
            }
            else
            {
                Debug.Log("Not Product");
                return null;
            }
        }
        catch(Exception e)
        {
            Debug.Log("Not Product");
            return null;
        }
    }

    // Encodes the data to create a QR code
    public static string Encode(string name, string value, string discount, ProductOrg type, string points)
    {
        string encoded = "";
        encoded += "<NAME>";
        encoded += name;
        encoded += "<VALUE>";
        encoded += value;
        encoded += "<DISCOUNT>";
        encoded += discount;
        encoded += "<TYPE>";
        encoded += type.ToString();
        encoded += "<TOKENS>";
        encoded += points;

        return encoded;
    }

    // Confirms the product with the server
    public static bool Confirm(string data)
    {
        try
        {
            string rawData = GameManager.Azure.PullBlob(ProductsContainer, data);
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