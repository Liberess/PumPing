using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressBtn : MonoBehaviour
{
    public static PressBtn instance;

    public Sprite Pressed_Off;

    [SerializeField]
    private Sprite[] Pressed_On;

    public bool isPressed;

    private int i = 0;
    private int j = 0;
    private int max_i = 3;

    private SpriteRenderer sprite;
    private PolygonCollider2D pol;

    void Awake()
    {
        instance = this;

        isPressed = false;

        sprite = GetComponent<SpriteRenderer>();
        pol = GetComponent<PolygonCollider2D>();
    }

    IEnumerator PressOn()
    {
        sprite.sprite = Pressed_On[i];

        if (i < max_i)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
            yield return StartCoroutine("PressOn");
        }
        else
        {
            pol.enabled = false;
            yield break;
        }
    }

    IEnumerator PressOff()
    {
        sprite.sprite = Pressed_On[j];

        if (j > 0)
        {
            j--;
            yield return new WaitForSeconds(0.01f);
            yield return StartCoroutine("PressOff");
        }
        else
        {
            pol.enabled = true;
            sprite.sprite = Pressed_Off;
            yield break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            isPressed = true;
            pol.enabled = false;
            StartCoroutine("PressOn");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            i = 0;
            j = max_i;
            isPressed = false;

            StartCoroutine("PressOff");
        }
    }
}