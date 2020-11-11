using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        GameObject.Find("Door").transform.Find("Canvas").gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Key.instance.haveKey)
        {
            GameObject.Find("Door").transform.Find("Canvas").transform.Find("txt").
gameObject.GetComponent<Text>().text = "Space를 누르면 문이 열립니다.";
        }
        else
        {
            GameObject.Find("Door").transform.Find("Canvas").transform.Find("txt").
gameObject.GetComponent<Text>().text = "Key가 필요합니다.";
        }
    }

    private void OnDestroy()
    {
        Player.instance.canJump = true;
        SubPlayer.instance.canJump = true;
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            Player.instance.canJump = false;
            SubPlayer.instance.canJump = false;

            GameObject.Find("Door").transform.Find("Canvas").gameObject.SetActive(true);

            if (Key.instance.haveKey && Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger("doDoor");
                Invoke("OnDestroy", 2f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            Player.instance.canJump = true;
            SubPlayer.instance.canJump = true;

            GameObject.Find("Door").transform.Find("Canvas").gameObject.SetActive(false);
        }
    }
}