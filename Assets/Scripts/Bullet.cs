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
            transform.Translate(transform.right * (-1) * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(transform.right * moveSpeed * Time.deltaTime);
        }

        Destroy(this.gameObject, 9f);
    }

    /* private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainPlayer") || collision.CompareTag("SubPlayer") || collision.CompareTag("Platform") || collision.CompareTag("Land"))
        {
            Debug.Log("닿음");

            Destroy(this.gameObject);
        }
    } */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MainPlayer") || collision.gameObject.CompareTag("SubPlayer") || collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Land"))
        {
            Destroy(this.gameObject);
        }
    }
}