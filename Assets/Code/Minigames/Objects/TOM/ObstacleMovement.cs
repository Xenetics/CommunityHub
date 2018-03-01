using System;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [NonSerialized]
    public WaterRun Game;

    [NonSerialized]
    public Vector3 Origin;

    private SpriteRenderer render;

    public bool bob;
    public float bobSpeed = 1f;
    public float bobStrength = 1f;
    private bool m_bob = true;
    private float m_bobPos = 0;

    public bool sway;
    public float swaySpeed = 1f;
    public float swayStrength = 1f;
    private bool m_sway = true;
    private float m_swayPos = 0;

    public bool teeter;
    public float teeterSpeed = 1f;
    public float maxTeeter = 30;
    private bool m_teeter = true;

    public bool rightLeft;
    public float rightLeftSpeed = 1f;
    public float maxRightLeftDist = 5f;
    private bool m_rightLeft = true;
    private float m_rightLeftPos = 0;

    public bool upDown;
    public float upDownSpeed = 1f;
    public float maxUpDownDist = 5f;
    private bool m_upDown = true;
    private float m_upDownPos = 0;

    public bool flipX;
    public bool flipY;

    private void Awake()
    {
        Origin = gameObject.transform.localPosition;
        render = gameObject.GetComponent<SpriteRenderer>();
        System.Random rand = new System.Random((int)DateTime.Now.Ticks);

        Vector3 nextPos = Origin;
        float bob_result = 0;
        float sway_result = 0;
        float rightLeft_result = 0;
        float upDown_result = 0;

        if (bob)
        {
            bob_result = Mathf.MoveTowards(m_bobPos, rand.Next((int)-bobStrength, (int)bobStrength), Time.deltaTime * bobSpeed);
            m_bobPos = bob_result;
            m_bob = (rand.Next(0, 1) == 0) ? (false) : (true);
        }

        if (sway)
        {
            sway_result = Mathf.MoveTowards(m_swayPos, rand.Next((int)-swayStrength, (int)swayStrength), Time.deltaTime * swaySpeed);
            m_swayPos = sway_result;
            m_sway = (rand.Next(0, 1) == 0) ? (false) : (true);
        }

        if (teeter)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, rand.Next(0, (int)maxTeeter)), teeterSpeed);
            m_teeter = (rand.Next(0, 1) == 0) ? (false) : (true);
        }

        if (rightLeft)
        {
            rightLeft_result = Mathf.MoveTowards(m_rightLeftPos, rand.Next((int)-maxRightLeftDist, (int)maxRightLeftDist), Time.deltaTime * rightLeftSpeed);
            m_rightLeftPos = rightLeft_result;
            m_rightLeft = (rand.Next(0, 1) == 0) ? (false) : (true);
        }

        if (upDown)
        {
            upDown_result = Mathf.MoveTowards(m_upDownPos, rand.Next((int)-maxUpDownDist, (int)maxUpDownDist), Time.deltaTime * upDownSpeed);
            m_upDownPos = upDown_result;
            m_upDown = (rand.Next(0, 1) == 0) ? (false) : (true);
        }

        nextPos.x = Origin.x + m_swayPos + m_rightLeftPos;
        nextPos.y = Origin.y + m_bobPos + m_upDownPos;
        gameObject.transform.localPosition = nextPos;
    }

    void Update ()
    {
        if (Game.GetState() == BaseGame.State.Playing)
        {
            Vector3 pos = gameObject.transform.localPosition;
            Vector3 nextPos = Origin;
            float bob_result = 0;
            float sway_result = 0;
            float rightLeft_result = 0;
            float upDown_result = 0;

            if (bob)
            {
                if (m_bobPos >= bobStrength)
                {
                    m_bob = false;
                }
                else if (m_bobPos <= -bobStrength)
                {
                    m_bob = true;
                }

                if (m_bob)
                {
                    bob_result = Mathf.MoveTowards(m_bobPos, bobStrength, Time.deltaTime * bobSpeed);
                }
                else
                {
                    bob_result = Mathf.MoveTowards(m_bobPos, -bobStrength, Time.deltaTime * bobSpeed);
                }
                m_bobPos = bob_result;
            }

            if (sway)
            {
                if (m_swayPos >= swayStrength)
                {
                    m_sway = false;
                }
                else if (m_swayPos <= -swayStrength)
                {
                    m_sway = true;
                }

                if (m_sway)
                {
                    sway_result = Mathf.MoveTowards(m_swayPos, swayStrength, Time.deltaTime * swaySpeed);
                }
                else
                {
                    sway_result = Mathf.MoveTowards(m_swayPos, -swayStrength, Time.deltaTime * swaySpeed);
                }
                m_swayPos = sway_result;
            }

            if (teeter)
            {
                Vector3 oldRot = transform.rotation.eulerAngles;

                if (transform.rotation.eulerAngles.z >= maxTeeter && transform.rotation.eulerAngles.z < 180)
                {
                    m_teeter = false;
                }
                else if (transform.rotation.eulerAngles.z <= (360 - maxTeeter) && transform.rotation.eulerAngles.z > 180)
                {
                    m_teeter = true;
                }

                if (m_teeter)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, maxTeeter), teeterSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 360 - maxTeeter), teeterSpeed);
                }
            }

            if (rightLeft)
            {
                if (m_rightLeftPos >= maxRightLeftDist)
                {
                    m_rightLeft = false;

                }
                else if (m_rightLeftPos <= -maxRightLeftDist)
                {
                    m_rightLeft = true;
                }

                if (flipX)
                {
                    render.flipX = m_rightLeft;
                }

                if (m_rightLeft)
                {
                    rightLeft_result = Mathf.MoveTowards(m_rightLeftPos, maxRightLeftDist, Time.deltaTime * rightLeftSpeed);
                }
                else
                {
                    rightLeft_result = Mathf.MoveTowards(m_rightLeftPos, -maxRightLeftDist, Time.deltaTime * rightLeftSpeed);
                }
                m_rightLeftPos = rightLeft_result;
            }

            if (upDown)
            {
                if (m_upDownPos >= maxUpDownDist)
                {
                    m_upDown = false;
                }
                else if (m_upDownPos <= -maxUpDownDist)
                {
                    m_upDown = true;
                }

                if (flipY)
                {
                    render.flipY = m_upDown;
                }

                if (m_upDown)
                {
                    upDown_result = Mathf.MoveTowards(m_upDownPos, maxUpDownDist, Time.deltaTime * upDownSpeed);
                }
                else
                {
                    upDown_result = Mathf.MoveTowards(m_upDownPos, -maxUpDownDist, Time.deltaTime * upDownSpeed);
                }
                m_upDownPos = upDown_result;
            }

            nextPos.x = Origin.x + m_swayPos + m_rightLeftPos;
            nextPos.y = Origin.y + m_bobPos + m_upDownPos;
            gameObject.transform.localPosition = nextPos;
        }
    }
}
