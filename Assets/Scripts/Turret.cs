using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    
    public GameManager gameManager;
    public GameObject bulletPrefab;
    public Transform pos;

    public AudioClip audioShot;

    Animator anim;
    ParticleSystem particle;
    AudioSource audioSource;

    public bool canShot = true;

    const float shotDelay = 2f;
    float shotTimer = 0;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        particle = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

        particle.Stop();
        particle.Clear();
    }
    void Update()
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

    void ShotControl()
    {
        particle.Clear();
        particle.Play();

        Invoke("ShotAction", 0.2f);

        shotTimer = 0;  //쿨타임 초기화
    }

    void ShotAction()
    {
        //audioSource.Play();
        AudioManager.instance.PlaySFX("Shot");
        anim.SetTrigger("isShoot");
        Instantiate(bulletPrefab, pos.position, transform.rotation);  //총알 생성
        particle.Clear();
    }
}
