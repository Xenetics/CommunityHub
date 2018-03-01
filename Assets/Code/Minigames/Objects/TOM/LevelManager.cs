using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [NonSerialized]
    public WaterRun Game;
    public ScubaPlayerController Player;

    [SerializeField]
    private GameObject Wall_Scroll;
    [SerializeField]
    private GameObject Water_Scroll;
    [SerializeField]
    private float[] Wall_Positions;
    [SerializeField]
    private float[] Water_Positions;
    [SerializeField]
    private List<GameObject> Wall_PNLS;
    [SerializeField]
    private List<GameObject> Water_PNLS;
    [SerializeField]
    private float Wall_Speed = 1;
    [SerializeField]
    private float Water_Speed = 1;
    [SerializeField]
    private List<GameObject> Obstacle_Prefabs;  
    public List<GameObject> Obstacles;
    [SerializeField]
    private int[] Obstacle_Tracks;

    public void Init(WaterRun game)
    {
        Game = game;
        Player.Game = game;
        foreach(GameObject ob in  Obstacles)
        {
            ob.GetComponent<ObstacleMovement>().Game = game;
        }
    }
	
	void Update ()
    {
        if (Game.GetState() == BaseGame.State.Playing)
        {
            ScrollWalls();
            ScrollWater();
        }
	}

    private void ScrollWalls()
    {
        Wall_Scroll.transform.position = Vector3.MoveTowards(Wall_Scroll.transform.position, new Vector3(Wall_Positions[0], Wall_Scroll.transform.position.y, Wall_Scroll.transform.position.z), Time.deltaTime * (Wall_Speed + Game.Difficulty()));

        if (Wall_Scroll.transform.position.x <= Wall_Positions[1])
        {
            GameObject panel = Wall_PNLS[0];
            Wall_PNLS.Add(panel);
            Wall_PNLS.RemoveAt(0);
            for(int i = 0; i < Wall_PNLS.Count; ++i)
            {
                Wall_PNLS[i].transform.localPosition = new Vector3(Wall_Positions[i], Wall_PNLS[i].transform.localPosition.y, Wall_PNLS[i].transform.localPosition.z);
            }
            Wall_Scroll.transform.position = new Vector3(Wall_Positions[2], Wall_Scroll.transform.position.y, Wall_Scroll.transform.position.z);
        }
    }

    private void ScrollWater()
    {
        Water_Scroll.transform.position = Vector3.MoveTowards(Water_Scroll.transform.position, new Vector3(Water_Positions[0], Water_Scroll.transform.position.y, Water_Scroll.transform.position.z), Time.deltaTime * (Water_Speed + Game.Difficulty()));
        
        if(Water_Scroll.transform.position.x <= Water_Positions[1])
        {
            if (Obstacles.Count > 0)
            {
                Destroy(Obstacles[0]);
                Obstacles.RemoveAt(0);
                Game.IncreaseScore();
            }
            GameObject panel = Water_PNLS[0];
            Water_PNLS.Add(panel);
            Water_PNLS.RemoveAt(0);
            for (int i = 0; i < Water_PNLS.Count; ++i)
            {
                Water_PNLS[i].transform.localPosition = new Vector3(Water_Positions[i], Water_PNLS[i].transform.localPosition.y, Water_PNLS[i].transform.localPosition.z);
            }
            Water_Scroll.transform.position = new Vector3(Water_Positions[2], Water_Scroll.transform.position.y, Water_Scroll.transform.position.z);
            AddObstacle();
        }
    }

    private void AddObstacle()
    {
        for(int i = 0; i < Obstacles.Count; i++)
        {
            ObstacleMovement obs = Obstacles[i].GetComponent<ObstacleMovement>();
            obs.Origin = new Vector3(Water_PNLS[i].transform.position.x, obs.Origin.y, 0);
        }        

        if(Obstacle_Prefabs.Count > 0 )
        {
            System.Random rand = new System.Random((int)DateTime.Now.Ticks);
            int ind = rand.Next(0, Obstacle_Prefabs.Count);
            GameObject newObstacle = Instantiate(Obstacle_Prefabs[ind], new Vector3(Water_PNLS[Water_PNLS.Count - 1].transform.localPosition.x, Obstacle_Tracks[ind], 0), Quaternion.identity, Water_Scroll.transform);
            newObstacle.GetComponent<ObstacleMovement>().Game = Game;
            Obstacles.Add(newObstacle);
        }
    }
}
