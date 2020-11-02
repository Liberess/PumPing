using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(GameManager.instance.isClear_Main && GameManager.instance.isClear_Sub)
        {
            anim.SetTrigger("doDoor");
            Invoke("OnDestroy", 1f);
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
