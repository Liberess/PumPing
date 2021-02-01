﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;
using UnityEngine.SceneManagement;

public class SubPlayer : MonoBehaviour
{
    #region 선언
    public static SubPlayer instance;

    GameManager gameManager;

    //플레이어 이동
    private float moveSpeed;
    private float maxSpeed = 10;
    private float jumpPower = 10;
    private static float maxPumping = 2;
    private static float maxJump = 2;
    private int jumpCount = 0;
    public float pumpingCount = 0;

    //사망 효과음 On/Off
    private bool audioPlay;

    //플레이어 움직임 On/Off
    private bool isMove;
    public bool canJump;
    public bool isSliding;
    private bool isEnding;

    private bool isGround;
    private bool isEnemy;
    private bool isFence;
    private bool isMine;

    public float pumpingTimer = 1;       //펌핑 시간
    private float pumpingDelay = 0f;      //펌핑 쿨타임 1초

    private Animator animator; //Animator Layer 제어
    public Transform pos;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    BoxCollider2D boxCollider;  //Sliding

    Vector3 target = new Vector3(13.4f, 77f, 0f);

    Vector2 previousPos;
    Vector2 nextPos;
    #endregion

    private void Awake()
    {
        #region 기본 초기화
        instance = this;

        audioPlay = true;
        isMove = true;
        canJump = true;
        isSliding = true;
        isEnding = false;
        isFence = false;

        moveSpeed = 10;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        #endregion
    }

    private void Start()
    {
        gameManager = GameManager.instance;

        StartCoroutine(AutoPos());
    }

    private void Update()
    {
        isMine = gameManager.isAlive && isMove && (gameManager.isMainPlayer == false);

        isGround = Physics2D.OverlapCircle(pos.position, 0.5f, LayerMask.GetMask("Platform"));
        isEnemy = Physics2D.OverlapCircle(pos.position, 0.5f, LayerMask.GetMask("Enemy"));
        isFence = Physics2D.OverlapCircle(pos.position, 0.5f, LayerMask.GetMask("FenceTrap"));

        Ending();

        if (!isGround)
        {
            anim.SetBool("isJump", true);
        }
        else
        {
            anim.SetBool("isJump", false);
            jumpCount = 0;
        }

        if (isEnemy)
        {
            anim.SetBool("isJump", false);
        }

        if (isFence)
        {
            jumpCount = 0;
            canJump = false;

            anim.SetBool("isJump", false);
            anim.SetBool("isJumpDown", true);
        }

        if (audioPlay == true)
        {
            if (gameManager.subEnergyBar.value <= 0.5f)
            {
                onDie();
                audioPlay = false;
            }
            else
            {
                gameManager.subEnergyBar.value = Mathf.MoveTowards(gameManager.subEnergyBar.value, 30f, Time.deltaTime * 1f);
            }
        }

        if (isMine)
        {
            Jump();

            gameManager.pumpUI.transform.Find("BackImg").transform.Find("CoolTxt")
                .gameObject.GetComponent<Text>().text = ((int)pumpingTimer).ToString();

            if (pumpingTimer <= pumpingDelay)
            {
                gameManager.pumpUI.transform.Find("BackImg").gameObject.SetActive(false);

                Pumping();
            }
            else
            {
                pumpingTimer -= Time.deltaTime;
            }
        }

        nextPos = transform.position;

        if (previousPos.y >= nextPos.y)
        {
            anim.SetBool("isJumpDown", true);
        }

        if (gameManager.isAlive == false)
        {
            anim.SetTrigger("doDead");
        }
    }

    private void FixedUpdate()
    {
        if (isMine)
        {
            Move();
        }

        CheckGapPos();
        LandingPlatform();
    }

    IEnumerator AutoPos()
    {
        previousPos = transform.position;

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(AutoPos());
    } //0.2초마다 위치 저장

    private void LandingPlatform()
    {
        //Landing Platform
        if (rigid.velocity.y <= 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Platform"));
            RaycastHit2D rayHit2 = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Enemy"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJump", false);
                    anim.SetBool("isJumpDown", true);
                    jumpCount = 0;
                }
            }

            if (rayHit2.collider != null)
            {
                if (rayHit2.distance < 0.5f)
                {
                    anim.SetBool("isJump", false);
                    jumpCount = 0;
                }
            }
        }
    } //Raycast로 Platform인지 Enemy인지 체크

    private void CheckGapPos()
    {
        if (gameManager.isMainPlayer == true)
        {
            float gapX = Mathf.Abs(nextPos.x) - Mathf.Abs(previousPos.x);

            if ((gapX <= 0.5) && isGround)
            {
                VelocityZero();
            }
        }
    } //경사면에서 천천히

    #region 엔딩
    private void Ending()
    {
        if (isEnding)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 0.1f);

            if (transform.position.x >= target.x - 1f)
            {
                rigid.gravityScale = 0f;
                rigid.AddForce(Vector2.up * 50f, ForceMode2D.Impulse);

                AudioManager.instance.sfxSlider.value = 0;
                AudioManager.instance.bgmPlayer.volume = 0;

                isEnding = false;
                Invoke("EndingScene", 3f);
            }
        }
    }

    private void EndingScene()
    {
        GameManager.instance.canvas.enabled = false;
        SceneManager.LoadScene("Ending", LoadSceneMode.Additive);
        AudioManager.instance.PlayBGM("Ending");
        AudioManager.instance.bgmPlayer.volume = 1;
    }
    #endregion

    private void Move()  //FixedUpdate
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);

        //MaxSpeed
        if (rigid.velocity.x > maxSpeed)  //Right
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1))  //Left
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        //Run Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3f)
        {
            anim.SetBool("isRun", false);
        }
        else
        {
            anim.SetBool("isRun", true);
        }

        if (isMine)
        {
            //Sprite Flip
            if (Input.GetButton("Horizontal"))
            {
                spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            }
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            if (Input.GetButtonDown("Jump") && jumpCount < maxJump && gameManager.subEnergyBar.value >= 1f)
            {
                animator.SetLayerWeight(1, 0);

                rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);

                anim.SetBool("isJump", true);
                anim.SetBool("isJumpDown", false);

                jumpCount++;
                gameManager.subEnergyBar.value--;

                AudioManager.instance.PlaySFX("Jump");
            }
        }
    }

    private void Pumping()
    {
        //Pumping Charging Down
        if (Input.GetKey(KeyCode.UpArrow) && gameManager.subEnergyBar.value >= 1f)
        {
            gameManager.pumping.SetActive(true);

            if (pumpingCount <= maxPumping)
            {
                pumpingCount += Time.deltaTime;

                gameManager.pumpingGauge = Mathf.MoveTowards(gameManager.pumpingGauge, 1f, Time.deltaTime);
            }
        }

        //Pumping Charging Up
        if (Input.GetKeyUp(KeyCode.UpArrow) && gameManager.subEnergyBar.value >= 1f)
        {
            if (pumpingCount >= 0.5f && gameManager.pumpingGauge >= 0.2f)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, (jumpPower * pumpingCount) / 1.2f);
                anim.SetBool("isJump", true);
                AudioManager.instance.PlaySFX("Pumping");
                gameManager.subEnergyBar.value -= 3;
            }

            pumpingCount = 0;
            gameManager.pumpingGauge = 0;
            gameManager.pumping.GetComponent<Image>().sprite = PumpingManager.instance.emptySprite;
            gameManager.pumping.SetActive(false);

            pumpingTimer = 3;

            gameManager.pumpUI.transform.Find("BackImg").gameObject.SetActive(true);
        }
    }

    #region 물리 충돌 (Trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /* if (collision.gameObject.tag == "door")
        {
            gameManager.isClear_Sub = true;

            if(gameManager.isClear_Main && gameManager.isClear_Sub)
            {
                isMove = false;

                moveSpeed = 0;

                VelocityZero();

                this.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y);

                //anim.SetTrigger("doDoor");

                Invoke("MoveOn", 1f);
                //Invoke("NextStage", 0.6f);
                Invoke("MoveSpeedReturn", 2f);
            }
        } */

        if (collision.gameObject.tag == "Ending")
        {
            isMove = false;

            moveSpeed = 0;

            VelocityZero();

            isEnding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "door")
        {
            //gameManager.isClear_Sub = false;
        }
    }
    #endregion

    #region 물리 충돌 (Collision)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            AudioManager.instance.PlaySFX("Damaged");
            onDamaged(collision.transform.position, 5);
            gameManager.subEnergyBar.value -= 5;
        }

        if (collision.gameObject.tag == "Land")
        {
            anim.SetBool("isJumpDown", true);
            anim.SetBool("isJump", false);
            jumpCount = 0;
        }

        if (collision.gameObject.tag == "Platform")
        {
            anim.SetBool("isJumpDown", true);
        }

        if (collision.gameObject.tag == "MineTrap")
        {
            AudioManager.instance.PlaySFX("MineTrap");
            onDamaged(collision.transform.position, 5);
        }

        if (collision.gameObject.tag == "FootTrap")
        {
            isMove = false;
            moveSpeed = 0;

            VelocityZero();

            transform.position = new Vector3(collision.transform.position.x, transform.position.y);

            AudioManager.instance.PlaySFX("FootTrap");

            anim.SetBool("isRun", false);
            anim.SetBool("isJump", false);

            Invoke("MoveOn", 1.8f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "FenceTrap")
        {
            Invoke("FenceSound", 0.2f);
            onDamaged(collision.transform.position, 1);
        }

        if (collision.gameObject.tag == "MoveTrap")
        {
            onDamaged(collision.transform.position, 1);
        }
    }
    #endregion

    public void MoveOn()
    {
        isMove = true;
        moveSpeed = 10;
    }

    private void FenceSound()
    {
        AudioManager.instance.PlaySFX("FenceTrap");
    }

    public void onDamaged(Vector2 targetPos, int what)
    {
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;

        if (what == 1)
        {
            //Change Layer (Immortal Active)
            gameObject.layer = 11;

            //View Alpha
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);

            //Reaction Force
            rigid.AddForce(new Vector2(dirc, 0.5f) * 0.5f, ForceMode2D.Impulse);
        }
        else if (what == 5)
        {
            //Reaction Force
            rigid.AddForce(new Vector2(dirc, 1f) * 5f, ForceMode2D.Impulse);
        }
        else if (what == 6)
        {
            //Change Layer (Immortal Active)
            gameObject.layer = 11;

            //View Alpha
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);

            //Reaction Force
            rigid.AddForce(new Vector2(dirc * 10f, 1f) * 2f, ForceMode2D.Impulse);
        }

        //Animation
        anim.SetTrigger("doDamaged");

        Invoke("offDamaged", 0.5f);
    }

    public void offDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void onAlive()
    {
        audioPlay = true;
        isMove = true;
        canJump = true;
        isSliding = true;
        isEnding = false;

        moveSpeed = 10;

        gameManager.subEnergyBar.value = 30f;

        gameObject.tag = "SubPlayer";
        gameObject.layer = 15;

        anim.ResetTrigger("doDead");
        anim.Play("Idle");

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void onDie()
    {
        gameManager.subEnergyBar.value = 0f;

        pumpingCount = 0;
        gameManager.pumpingGauge = 0;

        gameManager.pumping.SetActive(false);

        AudioManager.instance.PlaySFX("GameOver");

        anim.SetTrigger("doDead");

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

        gameObject.tag = "Untagged";
        gameObject.layer = 0;

        Invoke("VelocityZero", 3f);

        gameManager.isAlive = false;
        gameManager.canvas.enabled = false;
        gameManager.reStartBtn.SetActive(true);
    }

    public void VelocityZero()
    {
        anim.SetBool("isRun", false);
        anim.SetBool("isJump", false);
        rigid.velocity = Vector2.zero;
    }
}