using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIndicator : MonoBehaviour
{
    // Text object for the score
    public Text text;
    // Scrolling speed
    public float speed = 10;
    // how long the indicator is on screen
    public float screenTime = 3;
    // timer for the scroll
    private float m_timer;
    // Does this object scroll towards a destination. if false just scrolls up or down
    public bool ScrollToDest = false;
    // The destination to scroll
    public Transform ScrollDest;
    // If this is a gain or loss indicator
    private bool Gain;

	public void Init(int score, bool gain)
    {
        Gain = gain;
        if (gain)
        {
            text.text = "+" + score.ToString();
            text.color = Color.green;
        }
        else
        {
            text.text = "-" + score.ToString();
            text.color = Color.red;
        }
	}
	
	void Update()
    {
        if (!ScrollToDest)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + ((Gain)?(transform.up):(-transform.up)), Time.deltaTime * speed);
            Color col = text.color;
            text.color = Color.Lerp(text.color, new Color(col.r, col.g, col.b, 0), Time.deltaTime);
            m_timer += Time.deltaTime;
            if (m_timer >= screenTime)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, ScrollDest.position, Time.deltaTime * speed);
            Color col = text.color;
            text.color = Color.Lerp(text.color, new Color(col.r, col.g, col.b, 0), Time.deltaTime);
            m_timer += Time.deltaTime;
            if (m_timer >= screenTime)
            {
                Destroy(gameObject);
            }
        }
	}
}
