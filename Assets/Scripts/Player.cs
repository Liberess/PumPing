using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;

public class Player : MonoBehaviour
{
    //타 스크립트
    public GameManager gameManager;
    public PumpingGauge pumpingGauge;

    //사운드
    public AudioClip audioJump;
    public AudioClip audioDamaged;
    public AudioClip audioDie;
    public AudioClip audioMineTrap;
    public AudioClip audioHpItem;
    public AudioClip audioSpeedItem;

    //게임 중 사망 시 다시 시작 버튼
    public GameObject UIReStart;

    //플레이어 이동
    private float moveSpeed = 10;
    private float jumpPower = 10;
    private float maxPumping = 200;
    float maxJump = 2;
    int jumpCount = 0;
    int pumpingCount = 0;

    //사망 효과음 On/Off
    private bool audioPlay;

    //플레이어 움직임 On/Off
    private bool isMove;

    private Animator animator;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    //실시간 플레이어 위치
    Vector3 previousPosition = new Vector3();

    private void Awake()
    {
        audioPlay = true;
        isMove = true;

        gameManager.energyBar.value = 10f;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (audioPlay == true)
        {
            if (gameManager.energyBar.value <= 0.5f)
            {
                onDie();
                audioPlay = false;
            }
            else
            {
                gameManager.energyBar.value = Mathf.MoveTowards(gameManager.energyBar.value, 10f, Time.deltaTime * 1f);
            }
        }

        //Energy가 0.5f 이상이고 살아있으며 isMove가 true여야 움직일 수 있다.
        if (gameManager.energyBar.value >= 0.5f && IsAlive && isMove == true)
        {
            Move();
            Jump();
            Pumping();
            Sliding();
        }
    }

    private void FixedUpdate()
    {
        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJumpUp", false);
                    anim.SetBool("isJumpDown", false);
                    jumpCount = 0;
                }
            }
        }
    }

    public bool IsAlive  //플레이어가 살아있는지, 죽었는지
    {
        get
        {
            return gameManager.energyBar.value > 0;
        }
    }

    private void Move()
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Run Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3f && isMove == true)
        {
            anim.SetBool("isRun", false);
        }
        else
        {
            anim.SetBool("isRun", true);
        }
    }

    private void Jump()
    {
        if (this.transform.position.y < this.previousPosition.y)
        {
            anim.SetBool("isJumpUp", false);
        }

        this.previousPosition = this.transform.position;

        if (Input.GetButtonDown("Jump") && jumpCount < 2 && gameManager.energyBar.value >= 1f)
        {
            if (jumpCount < maxJump)
            {
                animator.SetLayerWeight(1, 0);

                rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);

                anim.SetBool("isJumpUp", true);
                anim.SetBool("isJumpDown", true);

                gameManager.energyBar.value--;
                jumpCount++;

                PlaySound("Jump");
            }
        }
    }

    private void Pumping()
    {
        //Pumping Charging Down
        if (Input.GetKey(KeyCode.UpArrow) && gameManager.pumpingGauge < 1f && gameManager.energyBar.value >= 1f)
        {
            gameManager.pumping.SetActive(true);

            if (pumpingCount <= maxPumping)
            {
                pumpingCount += 3;
                gameManager.pumpingGauge = Mathf.MoveTowards(gameManager.pumpingGauge, 1f, Time.deltaTime);
            }
        }

        //Pumping Charging Up
        if (Input.GetKeyUp(KeyCode.UpArrow) && gameManager.energyBar.value >= 1f)
        {
            if (pumpingCount >= 10 && gameManager.pumpingGauge >= 0.2)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, jumpPower * pumpingCount / 100);
                anim.SetBool("isJumpUp", true);
                anim.SetBool("isJumpDown", true);
                gameManager.energyBar.value -= 3;
            }
            pumpingCount = 0;
            gameManager.pumpingGauge = 0;

            gameManager.pumping.SetActive(false);
            gameManager.pumping.GetComponent<Image>().sprite = pumpingGauge.emptySprite;
        }
    }

    private void Sliding()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
        {
            animator.SetLayerWeight(1, 1);
            anim.SetTrigger("doSliding");
        }

        Invoke("MoveSpeedReturn", 1.2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            anim.SetBool("isJumpUp", false);
            anim.SetBool("isJumpDown", false);
        }

        if (collision.gameObject.tag == "MineTrap")
        {
            PlaySound("MineTrap");
            onDamaged(collision.transform.position, 5);
        }

        if (collision.gameObject.tag == "FootTrap")
        {
            this.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y);

            isMove = false;

            anim.SetBool("isJumpUp", false);
            anim.SetBool("isJumpDown", false);

            Invoke("MoveOn", 1.5f);
        }

        if (collision.gameObject.tag == "HpItem")
        {
            gameManager.energyBar.value += 4;
            PlaySound("HpItem");
        }

        if (collision.gameObject.tag == "SpeedItem")
        {
            moveSpeed = 15;
            PlaySound("SpeedItem");
            Invoke("MoveSpeedReturn", 5f);
        }
    }

    private void MoveOn()
    {
        isMove = true;
    }

    private void MoveSpeedReturn()
    {
        moveSpeed = 10;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "FenceTrap")
        {
            onDamaged(collision.transform.position, 1);
        }
    }

    private void onDamaged(Vector2 targetPos, int what)
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
            rigid.AddForce(new Vector2(dirc, 3f) * 4f, ForceMode2D.Impulse);
        }

        //Animation
        anim.SetTrigger("doDamaged");

        Invoke("offDamaged", 0.5f);
    }

    void offDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void onDie()
    {
        gameManager.energyBar.value = 0f;

        UIReStart.SetActive(true);

        gameManager.menuSet.SetActive(false);

        gameManager.miniMap.SetActive(false);

        PlaySound("Die");

        anim.SetTrigger("doDied");

        gameObject.layer = 11;

        Invoke("VelocityZero", 1);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "door")
        {
            isMove = false;

            this.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y);

            anim.SetTrigger("doDoor");

            Invoke("MoveOn", 1f);
            Invoke("NextStage", 0.8f);
        }
    }

    private void NextStage()
    {
        gameManager.NextStage();
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "Jump":
                audioSource.clip = audioJump;
                break;
            case "Damaged":
                audioSource.clip = audioDamaged;
                break;
            case "Die":
                audioSource.clip = audioDie;
                break;
            case "MineTrap":
                audioSource.clip = audioMineTrap;
                break;
            case "HpItem":
                audioSource.clip = audioHpItem;
                break;
            case "SpeedItem":
                audioSource.clip = audioSpeedItem;
                break;
        }

        audioSource.Play();

        if (action == "Died")
        {
            audioSource.PlayOneShot(audioDie);
        }
    }
}