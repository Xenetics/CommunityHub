using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base framework for generic mini games
[System.Serializable]
public abstract class BaseGame : MonoBehaviour
{
    [Header("General")]
    // Game name
    public string Name;
    // The static data for this game that only changes when the game activates and deactivates
    public MinigameData SavedActiveGameData;
    // Gameobject prefab with game assets in it
    [System.NonSerialized]
    public GameObject GameWorld;
    // States of the various games
    public enum State { Splash, Menu, Pregame, Playing, Paused, Gameover }
    // Current state of the game
    public static State Cur_State;
    // Last state the game was in if needed
    public static State Last_State;
    // Next state the game was in if needed
    public static State Next_State;
    // Score in the game
    public static int Score;
    // Timer for game wherever needed
    public static float Timer;
    // The time splash screen will show
    public float SplashTime = 3f;
    // Timer for splash screen
    protected float splashTimer = 0f;
    [Header("Sound")]
    // Bundle(bndl) of all SFX in game
    public List<AudioClip> SFX;
    // Bundle(bndl) of all Music in game
    public List<AudioClip> Music;

    // Used to set a state
    public virtual void SetState(State newState)
    {
        Last_State = Cur_State;
        Cur_State = newState;
    }

    // Used to execute code on state entrance
    public abstract void EnterState(State state);

    // Used to execute code on state exiting
    public abstract void ExitState(State state);

    // Execute any code before minigame closes. eg calls to main game
    public abstract void CloseGame();
}
