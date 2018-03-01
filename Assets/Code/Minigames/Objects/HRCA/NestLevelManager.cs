using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestLevelManager : MonoBehaviour
{
    [NonSerialized]
    public BirdGame Game;
    public enum NestTypes { BLUE, YELLOW, RED, GREEN }

    [SerializeField]
    private GameObject nests;
    private Nest[] active_Nests;
    [SerializeField]
    private GameObject birds;
    [SerializeField]
    private List<Bird> active_Birds;
    [SerializeField]
    private GameObject spawns;
    private List<Transform> spawn_Points;
    private List<Transform> transition_Points;
    [SerializeField]
    private GameObject[] Bird_Prefabs;
    System.Random rand;

    // Pathing
    private Bird SelectedBird;
    private float MinPtoPMag = 1;

    public void Init(BirdGame game)
    {
        Game = game;
        GatData();
        active_Birds = new List<Bird>();
        rand = new System.Random((int)DateTime.Now.Ticks);
    }

    private void GatData()
    {
        active_Nests = nests.GetComponentsInChildren<Nest>();

        List<Transform> transforms = new List<Transform>(spawns.GetComponentsInChildren<Transform>());
        spawn_Points = new List<Transform>();
        transition_Points = new List<Transform>();
        foreach (Transform pt in transforms)
        {
            if (pt.tag == "BirdSpawn")
            {
                spawn_Points.Add(pt);
            }
            else if (pt.tag == "BirdSpawnTransition")
            {
                transition_Points.Add(pt);
            }
        }
    }
	
	void Update ()
    {
        if (Game.GetState() == BaseGame.State.Playing)
        {
            BirdSelector();
            PathMaker();
        }
	}

    private void BirdSelector()
    {
        if(Input.GetMouseButtonDown(0))
        {
            foreach(Bird bird in active_Birds)
            {
                if(bird.IsClicked(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    SelectedBird = bird;
                    bird.Path.Clear();
                    break;
                }
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            SelectedBird = null;
        }
    }

    private void PathMaker()
    {
        if (Input.GetMouseButton(0) && SelectedBird != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Nest newNest = NestCheck(mousePos);

            if (newNest != null)
            {
                SelectedBird.TargetNest = newNest;
            }
            else if (SelectedBird.Path.Count > 0)
            {
                if (Vector2.Distance(SelectedBird.Path[SelectedBird.Path.Count - 1], mousePos) > MinPtoPMag)
                {
                    SelectedBird.Path.Add((Vector2)mousePos);
                }
            }
            else
            {
                if (Vector2.Distance(SelectedBird.transform.position, mousePos) > MinPtoPMag)
                {
                    SelectedBird.Path.Add((Vector2)mousePos);
                }
            }
        }
    }

    private Nest NestCheck(Vector3 pos)
    {
        foreach (Nest nest in active_Nests)
        {
            if (nest.IsClicked(pos))
            {
                return nest;
            }
        }
        return null;
    }

    private int lastSpawn = -1;

    public void SpawnBird()
    {
        int num = rand.Next(0, Bird_Prefabs.Length - 1);

        GameObject newBird = Instantiate(Bird_Prefabs[num], birds.transform);
        Bird script = newBird.GetComponent<Bird>();

        script.Game = Game;
        script.Difficulty = Game.GetDiff();
        num = rand.Next(0, spawn_Points.Count - 1);
        script.TargetNest = active_Nests[num];

        do
        {
            num = rand.Next(0, spawn_Points.Count - 1);
        } while (num == lastSpawn);
        lastSpawn = num;

        newBird.transform.position = spawn_Points[num].position;
        script.Path.Add(transition_Points[num].position);

        Vector3 targetPos = script.TargetNest.transform.position;
        Vector2 v2Pos = gameObject.transform.localPosition;
        float fAngle = -(Mathf.Atan2(targetPos.x - v2Pos.x, targetPos.y - v2Pos.y) * Mathf.Rad2Deg);
        newBird.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, fAngle);

        active_Birds.Add(script);
    }

    public void KillBird(Bird bird)
    {
        active_Birds.Remove(bird);
        Destroy(bird.gameObject);
    }

    public void NestBird(Bird bird)
    {
        active_Birds.Remove(bird);
        Game.IncreaseScore(bird.Difficulty);
        Destroy(bird.gameObject);
    }

    public void Redirect(Bird bird)
    {
        int num = rand.Next(0, spawn_Points.Count - 1);
        bird.TargetNest = active_Nests[num];
    }
}
