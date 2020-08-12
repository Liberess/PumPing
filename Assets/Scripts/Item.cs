using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    float time = 0;

    private void Update()
    {
        if(time < 3f)
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
            Invoke("ReSetting", 6f);
        }
    }

    private void ReSetting()
    {
        time = 0;
        gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }
}