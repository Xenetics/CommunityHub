using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PufferBehavior : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    public float IdleTime = 3;
    private float timer = 0;
	
	void Update ()
    {
        timer += Time.deltaTime;
        if(timer >= IdleTime)
        {
            timer = 0;
            if (anim.GetBool("puff"))
            {
                
                anim.SetBool("puff", false);
                SoundManager.Instance.PlaySound("TOM_Puffer_Deflate", GameManager.Minigame.SFX);
            }
            else
            {
                anim.SetBool("puff", true);
                SoundManager.Instance.PlaySound("TOM_Puffer_Inflate", GameManager.Minigame.SFX);
            }
        }
	}


}
