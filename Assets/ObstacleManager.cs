using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager instance;

    Animator anim;
    BoxCollider2D boxCol;

    void Start()
    {
        instance = this;

        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    public void OnDestroy()
    {
        boxCol.isTrigger = true;
        anim.SetBool("isDestroy", true);
    }

    public void OnCreate()
    {
        boxCol.isTrigger = false;
        anim.SetBool("isDestroy", false);
    }
}
