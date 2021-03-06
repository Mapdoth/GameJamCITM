﻿using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player_move : MonoBehaviour
{

    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    static public bool death = false;
    public bool death2 = false;
    private bool instance = true;

    public int count_box = 3;
    public Text text_box;

    public Image fadeBlack;
    //button things
    public float speed;
    public GameObject clown_box;
    private Vector3 dir;
    private Animator animation;
    private SpriteRenderer flip;
    public float timerDeath = 1.0f;
    private float time = 0.0f;

    void Start()
    {
        death = false;
        animation = GetComponentInChildren<Animator>();
        flip = GetComponentInChildren<SpriteRenderer>();
        fadeBlack.color = new Color(fadeBlack.color.r, fadeBlack.color.g, fadeBlack.color.b, 0);
    }

    void FixedUpdate()
    {
        GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);
    }

    // Update is called once per frame
    void Update()
    {
        if (!death && !animation.GetCurrentAnimatorStateInfo(0).IsName("clown_attack"))
        {
            // Find a PlayerIndex, for a single player game
            // Will find the first controller that is connected ans use it
            if (!playerIndexSet || !prevState.IsConnected)
            {
                for (int i = 0; i < 4; ++i)
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                        playerIndex = testPlayerIndex;
                        playerIndexSet = true;
                    }
                }
            }

            prevState = state;
            state = GamePad.GetState(playerIndex);
            //button things

            dir = Vector3.zero;

            float x_motion = state.ThumbSticks.Left.X;
            float y_motion = state.ThumbSticks.Left.Y;

            if (state.Triggers.Right != 0 && instance == true)
            {
                if (count_box > 0)
                {
                    Instantiate(clown_box, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
                    count_box--;
                    text_box.text = count_box.ToString();
                    instance = false;
                }
            }

            else if (state.Triggers.Right == 0)
            {
                instance = true;
            }

            if (y_motion != 0)
            {
                animation.SetBool("Walk", true);

            }

            if (x_motion < 0)
            {
                flip.flipX = true;
                animation.SetBool("Walk", true);
            }

            if (x_motion > 0)
            {
                flip.flipX = false;
                animation.SetBool("Walk", true);
            }

            dir.x = x_motion;
            dir.z = y_motion;

            if (dir != Vector3.zero)
            {
                transform.Translate(dir.normalized * speed);
            }
            else
            {
                animation.SetBool("Walk", false);
            }
            time = Time.time;
        }
        else if (animation.GetBool("Attacking"))
            Debug.Log(":::::");
        else if (death)
        {
            death = death2;
            animation.SetBool("Nurse_hit", true);
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            Nurse_move.movements = NurseMovements.Patrol;

            if (Time.time >= (time + timerDeath))
            {
                var tempColor = fadeBlack.color;
                tempColor.a = (tempColor.a + 0.01f);
                fadeBlack.color = tempColor;
                if (tempColor.a >= 1.0f)
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }
}
