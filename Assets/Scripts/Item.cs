using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject item;
    public GameManager gameManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            item.SetActive(false);

            Invoke("ReSetting", 6f);
        }
    }

    private void ReSetting()
    {
        item.SetActive(true);
    }
}