using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [NonSerialized]
    public BirdGame Game;
    public NestLevelManager.NestTypes NestType;
    private PolygonCollider2D col;
    [SerializeField]
    private LineRenderer lRend;
    [SerializeField]
    private float Speed = 5;
    [NonSerialized]
    public Nest TargetNest;
    public List<Vector3> Path;
    [NonSerialized]
    public int Difficulty = 1;

    void Start ()
    {
        col = gameObject.GetComponent<PolygonCollider2D>();
        SoundManager.Instance.PlaySound("HRCA_IncomingBird", Game.SFX);
        col.enabled = false;
    }
	
	void Update ()
    {
        if (Game.GetState() == BaseGame.State.Playing)
        {
            if (Path.Count > 0)
            {
                RotateMove(Path[0]);
                if (gameObject.transform.localPosition == Path[0])
                {
                    Path.RemoveAt(0);
                    if(col.enabled == false)
                    {
                        col.enabled = true;
                    }
                }
            }
            else
            {
                if (TargetNest != null)
                {
                    RotateMove(TargetNest.transform.localPosition);
                }
            }
        }

        List<Vector3> verts = new List<Vector3>();
        verts.Add(transform.position);
        verts.AddRange(Path);
        verts.Add(TargetNest.transform.position);
        lRend.positionCount = verts.Count;
        lRend.SetPositions(verts.ToArray());
	}

    private void RotateMove(Vector2 targetPos)
    {
        Vector2 v2Pos = gameObject.transform.localPosition;
        gameObject.transform.localPosition = Vector2.MoveTowards(v2Pos, targetPos, Time.deltaTime * Speed);
        float fAngle = -(Mathf.Atan2(targetPos.x - v2Pos.x, targetPos.y - v2Pos.y) * Mathf.Rad2Deg);
        Quaternion targetRot = Quaternion.Euler(0.0f, 0.0f, fAngle);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * Speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("KillBox"))
        {
            SoundManager.Instance.PlaySound("HRCA_Impact", Game.SFX);
            Game.levelManager.KillBird(this);
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Nest"))
        {
            Nest nest = collision.gameObject.GetComponent<Nest>();
            if (nest.NestType == NestType)
            {
                SoundManager.Instance.PlaySound("HRCA_bird_correct", Game.SFX);
                Game.levelManager.NestBird(this);
            }
            else
            { 
                NestLevelManager.NestTypes nestType = nest.NestType;
                do
                {
                    Game.levelManager.Redirect(this);
                } while (NestType == nestType);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bird"))
        {
            if (collision.gameObject.GetComponent<Bird>().NestType != NestType)
            {
                Game.SetState(BaseGame.State.Gameover);
            }
        }
    }

    public bool IsClicked(Vector2 point)
    {
        if(col.OverlapPoint(point))
        {
            return true;
        }
        return false;
    }
}
