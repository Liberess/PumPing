using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emp : MonoBehaviour
{
    public Turret turret;

    float time = 0;

    private void Update()
    {
        if (time < 3f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, time / 3);
        }

        time += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
            turret.canShot = false;
            Invoke("OnCanShot", 3f);
            Invoke("ReSetting", 6f);
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
        turret.canShot = true;
    }
}
