using UnityEngine;

public class ScubaPlayerController : MonoBehaviour
{
    [System.NonSerialized]
    public WaterRun Game;

    private Vector2 Origin;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    public Rigidbody2D RB;

    [SerializeField]
    private float Swimforce = 5f;

    private void Awake()
    {
        Origin = gameObject.transform.position;
    }

    public void Setup()
    {
        gameObject.transform.position = Origin;
        RB.simulated = false;
    }

    public void PlaySound()
    {
        //SoundManager.Instance.PlaySound("TOM_ScubaNew", GameManager.Minigame.SFX);
    }
 
	void Update ()
    {
        if (Game.GetState() == BaseGame.State.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("Swim");
                RB.AddForce(Vector2.up * Swimforce, ForceMode2D.Impulse);
            }
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("KillBox"))
        {
            Game.SetState(BaseGame.State.Gameover);
            RB.simulated = false;
        }
    }
}
