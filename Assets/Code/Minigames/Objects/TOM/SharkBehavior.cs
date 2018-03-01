using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBehavior : MonoBehaviour
{
    public float SoundLoopTime = 1;
    private float timer = 0;

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= SoundLoopTime)
        {
            timer = 0;
            SoundManager.Instance.PlaySound("TOM_SharkNew", GameManager.Minigame.SFX);
        }
    }
}
