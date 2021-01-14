using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTurret : MonoBehaviour
{
    public static ShotTurret instance;

    public RuntimeAnimatorController[] turretAc;

    public GameObject[] bulletPrefab;
    public Transform[] pos;

    public AudioClip audioShot;

    Animator anim;
    //ParticleSystem particle;
    AudioSource audioSource;

    public int ID;

    public float bulletTime;

    public bool canShot;

    private const float shotDelay = 2f;
    private float shotTimer = 0;

    private void Awake()
    {
        instance = this;
        canShot = true;

        anim = GetComponent<Animator>();
        //particle = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

        ChangeAc();

        //particle.Stop();
        //particle.Clear();
    }

    private void Update()
    {
        if (canShot)
        {
            if (shotTimer >= shotDelay)  //쿨타임이 지났으면
            {
                ShotControl();
            }
            shotTimer += Time.deltaTime;
        }
    }

    private void ShotControl()
    {
        //particle.Clear();
        //particle.Play();

        //Invoke("ShotAction", 0.2f);
        ShotAction();

        shotTimer = 0;  //쿨타임 초기화
    }

    private void ShotAction()
    {
        audioSource.Play();
        anim.SetTrigger("doShoot");

        Invoke("Shot", bulletTime);
        //particle.Clear();
    }

    private void Shot()
    {
        if (ID == 0)
        {
            Instantiate(bulletPrefab[ID], pos[0].position, transform.rotation);  //총알 생성
        }
        else
        {
            Instantiate(bulletPrefab[ID], pos[1].position, transform.rotation);  //총알 생성
        }
    }

    private void ChangeAc()
    {
        anim.runtimeAnimatorController = turretAc[ID];
    }
}