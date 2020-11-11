using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    MoveTurret moveTurret;
    ShotTurret shotTurret;
    public Sprite LeverOn;
    public Sprite LeverOff;

    public bool isPressed;

    SpriteRenderer sprite;

    void Start()
    {
        isPressed = false;

        moveTurret = MoveTurret.instance;
        shotTurret = ShotTurret.instance;
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPressed)
        {
            sprite.sprite = LeverOn;
            moveTurret.isMove = false;
            shotTurret.canShot = false;
        }
        else
        {
            sprite.sprite = LeverOff;
            moveTurret.isMove = true;
            shotTurret.canShot = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            Player.instance.canJump = false;
            SubPlayer.instance.canJump = false;

            if (Input.GetKey(KeyCode.Space))
            {
                isPressed = true;
            }
            else
            {
                isPressed = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            Player.instance.canJump = true;
            SubPlayer.instance.canJump = true;
        }
    }
}
