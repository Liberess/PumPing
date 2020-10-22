using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const float moveSpeed = 10f;

    void Update()
    {
        if (transform.rotation.y == 0)
        {
            transform.Translate(transform.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(transform.right * (-1) * moveSpeed * Time.deltaTime);
        }
        Destroy(gameObject, 9f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer" || collision.gameObject.tag == "Platform")
        {
            Destroy(gameObject);
        }
    }
}