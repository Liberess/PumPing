using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    Animator anim;
    BoxCollider2D box;

    void Awake()
    {
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            Invoke("OffCollision", 1.3f);
            Invoke("BrokenAnim", 1f);
        }
    }

    private void OffCollision()
    {
        box.enabled = false;
    }

    private void BrokenAnim()
    {
        anim.SetBool("isBroken", true);
        Invoke("OnDestroy", 1f);
    }

    private void OnDestroy()
    {
        gameObject.SetActive(false);
        anim.SetBool("isBroken", false);
        Invoke("Reset", 4f);
    }

    private void Reset()
    {
        box.enabled = true;
        gameObject.SetActive(true);
        anim.SetTrigger("doReset");
    }
}
