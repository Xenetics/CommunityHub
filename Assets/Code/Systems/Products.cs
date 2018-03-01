using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Products
{
    /// <summary> List of events downloaded from the database </summary>
    private static List<Product> m_Products;
    public static string ProductContainer = "products";

    /// <summary> Sets up the events Tab </summary>
    public static void InitProducts()
    {
        try
        {
            LoadProducts();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary> Retrieves events from the cloud, Stores the data and loads the ListView </summary>
    private static void LoadProducts()
    {
        List<string> productBlobs = GameManager.Azure.ListBlobsInContainer(ProductContainer);
        m_Products = new List<Product>();
        foreach (string blob in productBlobs)
        {
            m_Products.Add(Product.Parse(blob));
        }
        m_Products = m_Products.OrderBy(o => o.Organization).ThenBy(o => o.TokenValue).ToList();
    }

    public static List<Product> List()
    {
        return m_Products;
    }
}
