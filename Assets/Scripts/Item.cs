﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ShotTurret[] turret = null;

    float time = 0;

    private void Update()
    {
        if (time < 3f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, time / 3);
        }

        time += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.tag == "MainPotion")
        {
            if (collision.gameObject.tag == "MainPlayer")
            {
                GameManager.instance.mainEnergyBar.value += 15;

                AudioManager.instance.PlaySFX("HpItem");

                gameObject.SetActive(false);
                Invoke("ReSetting", 6f);
            }
        }

        if (gameObject.tag == "SubPotion")
        {
            if (collision.gameObject.tag == "SubPlayer")
            {
                GameManager.instance.subEnergyBar.value += 10;

                AudioManager.instance.PlaySFX("HpItem");

                gameObject.SetActive(false);
                Invoke("ReSetting", 6f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            /* if (gameObject.tag == "SpeedItem")
            {
                AudioManager.instance.PlaySFX("SpeedItem");
            } */

            if(gameObject.tag == "EmpItem")
            {
                AudioManager.instance.PlaySFX("EmpItem");

                for(int i = 0; i < turret.Length; i++)
                {
                    turret[i].canShot = false;
                }
                
                Invoke("OnCanShot", 3f);

                gameObject.SetActive(false);
                Invoke("ReSetting", 6f);
            }
        }
    }

    private void ReSetting()
    {
        time = 0;
        gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void OnCanShot()
    {
        for (int i = 0; i < turret.Length; i++)
        {
            turret[i].canShot = true;
        }
    }
}