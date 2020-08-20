using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public class Trap : MonoBehaviour
{
    public GameObject trap;

    public GameManager gameManager;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && trap.gameObject.tag != "FenceTrap")
        {
            if (trap.gameObject.tag == "MineTrap")
            {
                trap.gameObject.GetComponent<BoxCollider2D>().enabled = false;

                gameManager.energyBar.value -= 5;

                Invoke("DelateTrap", 0.5f);
            }
            
            if (trap.gameObject.tag == "FootTrap")
            {
                trap.gameObject.GetComponent<BoxCollider2D>().enabled = false;

                gameManager.energyBar.value -= 2;

                Invoke("DelateTrap", 1.1f);
            }

            anim.SetTrigger("doTouch");
            Invoke("ReSetting", 6f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && trap.gameObject.layer == 13)
        {
            doDamage();
        }
    }

    private void doDamage()
    {
        trap.gameObject.layer = 14;

        gameManager.energyBar.value -= 1;

        Invoke("offDamage", 0.1f);
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