using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public static Key instance;

    public bool haveKey;

    public void Start()
    {
        haveKey = false;
        gameObject.SetActive(true);
    }

    void Awake() => instance = this;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            haveKey = true;
            gameObject.SetActive(false);
        }
    }
}
