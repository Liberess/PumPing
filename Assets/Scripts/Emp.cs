using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emp : MonoBehaviour
{
    public Turret turret;

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
        gameObject.SetActive(true);
    }

    public void OnCanShot()
    {
        turret.canShot = true;
    }
}
