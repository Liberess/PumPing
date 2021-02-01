using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public class Trap : MonoBehaviour
{
    GameManager gameManager;

    public GameObject trap;

    Animator anim;

    private void Awake()
    {
        gameManager = GameManager.instance;

        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(trap.gameObject.tag != "FenceTrap")
        {
            if (trap.gameObject.tag == "MineTrap")
            {
                trap.gameObject.GetComponent<BoxCollider2D>().enabled = false;

                if (collision.gameObject.tag == "MainPlayer")
                {
                    GameManager.instance.mainEnergyBar.value -= 5;
                }
                else if(collision.gameObject.tag == "SubPlayer")
                {
                    GameManager.instance.subEnergyBar.value -= 5;
                }

                Invoke("DelateTrap", 0.5f);
            }
            
            if (trap.gameObject.tag == "FootTrap")
            {
                trap.gameObject.GetComponent<BoxCollider2D>().enabled = false;

                if (collision.gameObject.tag == "MainPlayer")
                {
                    GameManager.instance.mainEnergyBar.value -= 2;
                }
                else if (collision.gameObject.tag == "SubPlayer")
                {
                    GameManager.instance.subEnergyBar.value -= 2;
                }

                Invoke("DelateTrap", 1.1f);
            }

            anim.SetTrigger("doTouch");
            Invoke("ReSetting", 6f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(trap.gameObject.layer == 13 && GameManager.instance.isAlive == true)
        {
            if (collision.gameObject.tag == "MainPlayer")
            {
                doDamage();
                GameManager.instance.mainEnergyBar.value -= 1;
            }
            else if (collision.gameObject.tag == "SubPlayer")
            {
                doDamage();
                GameManager.instance.subEnergyBar.value -= 1;
            }
        }
    }

    private void doDamage()
    {
        trap.gameObject.layer = 14;

        Invoke("offDamage", 0.2f);
    }

    private void offDamage()
    {
        trap.gameObject.layer = 13;
    }

    private void DelateTrap()
    {
        trap.SetActive(false);
    }

    private void ReSetting()
    {
        trap.SetActive(true);

        trap.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }
}