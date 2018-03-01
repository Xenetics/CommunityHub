using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour
{
    public NestLevelManager.NestTypes NestType;
    private Collider2D col;

    private void Start()
    {
        col = gameObject.GetComponent<Collider2D>();
    }

    public bool IsClicked(Vector2 point)
    {
        if (col.OverlapPoint(point))
        {
            return true;
        }
        return false;
    }
}
