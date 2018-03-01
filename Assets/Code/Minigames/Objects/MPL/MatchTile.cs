using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MatchTile : MonoBehaviour
{
    // Memory match game refference
    public MemoryMatch game;
    // The column and row in cells of this tile
    public Vector2 currentCell;
    // Tile animation
    public Animator anim;
    // used for movement and such
    public RectTransform rect;
    private float Timer = 0f;
    public float OpenTime = 5f;
    public bool Open = false;
    public bool Selected = false;
    public Image MatchImage;
    [NonSerialized]
    public bool Transitioning = false;
    [NonSerialized]
    public bool Exiting = false;
    [NonSerialized]
    public Vector3 FinalPos;
    [NonSerialized]
    public float SetupDelay = 0f;
    private float delayTimer = 0f;


	// Use this for initialization
	void Awake()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!Transitioning && !Exiting)
        {
            if (anim.GetBool("Open") && !Selected)
            {
                Timer += Time.deltaTime;
                if (Timer >= ((game.GetState() == BaseGame.State.Playing) ? (OpenTime) : (game.PregameTime)))
                {
                    anim.SetBool("Open", false);
                    Timer = 0;
                }
            }
        }
        else if (Exiting && !Transitioning)
        {
            rect.localPosition = Vector3.Lerp(rect.localPosition, FinalPos, (game.TileSpeed * 0.1f) * Time.deltaTime);
            if (Vector3.Distance(rect.localPosition, FinalPos) < 5)
            {
                Destroy(this);
            }
        }
        else if (Transitioning && !Exiting)
        {
            if (delayTimer >= SetupDelay)
            {
                rect.localPosition = Vector3.Lerp(rect.localPosition, FinalPos, game.TileSpeed * Time.deltaTime);
                if (Vector3.Distance(rect.localPosition, FinalPos) < 5)
                {
                    rect.localPosition = FinalPos;
                    Transitioning = false;
                    if (!game.BoardSetup)
                    {
                        anim.SetBool("Open", true);
                    }
                }
            }
            else
            {
                delayTimer += Time.deltaTime;
            }
        }
	}

    // on tile being clicked
    public void TileClick()
    {
        if (game.GetState() == BaseGame.State.Playing)
        {
            if (!Transitioning && !game.BoardSetup && !anim.GetBool("Open"))
            {
                game.SelectTile(this);
                anim.SetBool("Open", true);
            }
        }
    }

    // on release of tile being clicked
    public void TileClickReleased()
    {
        if (game.GetState() == BaseGame.State.Playing)
        {
            if (!Transitioning && !game.BoardSetup && !anim.GetBool("Open"))
            {

            }
        }
    }

    // Triggers the open tile animation
    public void OpenTile()
    {
        anim.SetBool("Open", true);
    }

    /// <summary> Destroys this tile </summary>
    public void DestroyTile()
    {
        game.KillTile(currentCell);
    }
}
