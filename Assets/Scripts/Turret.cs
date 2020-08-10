using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject bulletPrefab;
    public Transform pos;

    Animator anim;

    public bool canShot = true;

    const float shotDelay = 2f;
    float shotTimer = 0;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        ShotControl();
    }

    void ShotControl()
    {
        if (canShot)
        {
            if(shotTimer > shotDelay)  //쿨타임이 지났으면
            {
                //anim.SetBool("isShot", true);
                anim.SetTrigger("isShoot");
                Instantiate(bulletPrefab, pos.position, transform.rotation);  //총알 생성
                shotTimer = 0;  //쿨타임 초기화
            }
            shotTimer += Time.deltaTime;
        }
    }
}
