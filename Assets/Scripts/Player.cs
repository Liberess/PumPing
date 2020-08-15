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

    //게임 중 사망 시 다시 시작 버튼
    public GameObject UIReStart;

    //플레이어 이동
    public float moveSpeed = 10;
    private float maxSpeed = 10;
    private float jumpPower = 10;
    private float maxPumping = 200;
    private float maxJump = 2;
    private int jumpCount = 0;
    private int pumpingCount = 0;

    //사망 효과음 On/Off
    private bool audioPlay;

    //플레이어 움직임 On/Off
    private bool isMove;
    private bool isSliding;

    private bool isGround;
    private bool isEnemy;

    private Animator animator;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    BoxCollider2D boxCollider;  //Sliding
    AudioManager audioManager;

    //실시간 플레이어 위치
    Vector3 previousPosition = new Vector3();

    private void Awake()
    {
        audioPlay = true;
        isMove = true;

        moveSpeed = 10;

        gameManager.energyBar.value = 14f;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioManager = AudioManager.instance;
    }

    void Update()
    {
        isGround = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Platform"));
        isEnemy = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Enemy"));

        if (!isGround)
        {
            anim.SetBool("isJump", true);
        }
        else
        {
            jumpCount = 0;
            anim.SetBool("isJump", false);
        }

        if (isEnemy)
        {
            jumpCount = 0;
            anim.SetBool("isJump", false);
        }

        if (audioPlay == true)
        {
            if (gameManager.energyBar.value <= 0.5f)
            {
                onDie();
                audioPlay = false;
            }
            else
            {
                gameManager.energyBar.value = Mathf.MoveTowards(gameManager.energyBar.value, 14f, Time.deltaTime * 1f);
            }
        }

        //Energy가 0.5f 이상이고 살아있으며 isMove가 true여야 움직일 수 있다.
        if (IsAlive && isMove == true)
        {
            Jump();
            Pumping();
            Sliding();
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            //rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            VelocityZero();
        }

        //Sprite Flip
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
    }

    void FixedUpdate()
    {
        if (this.transform.position.y < this.previousPosition.y)
        {
            //anim.SetBool("isJumpUp", false);
            anim.SetBool("isJump", false);
        }

        this.previousPosition = this.transform.position;

        //Energy가 0.5f 이상이고 살아있으며 isMove가 true여야 움직일 수 있다.
        if (IsAlive && isMove == true)
        {
            Move();
        }

        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            /* Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Platform"));

            RaycastHit2D rayHit2 = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Enemy"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    //anim.SetBool("isJumpUp", false);
                    anim.SetBool("isJump", false);
                    jumpCount = 0;
                }
            }

            if (rayHit2.collider != null)
            {
                if (rayHit2.distance < 0.5f)
                {
                    //anim.SetBool("isJumpUp", false);
                    anim.SetBool("isJump", false);
                    jumpCount = 0;
                }
            } */
        }
    }

    public bool IsAlive  //플레이어가 살아있는지, 죽었는지
    {
        get
        {
            return gameManager.energyBar.value >= 0.5f;
        }
    }

    private void Move()  //FixedUpdate
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);

        //MaxSpeed
        if(rigid.velocity.x > maxSpeed)  //Right
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
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < 2 && gameManager.energyBar.value >= 1f)
        {
            if (jumpCount < maxJump)
            {
                animator.SetLayerWeight(1, 0);

                rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);

                //anim.SetBool("isJumpUp", true);
                //anim.SetBool("isJumpDown", true);
                anim.SetBool("isJump", true);

                gameManager.energyBar.value--;
                jumpCount++;

                AudioManager.instance.PlaySFX("Jump");
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
                rigid.velocity = new Vector2(rigid.velocity.x, jumpPower * pumpingCount / 120);
                //anim.SetBool("isJumpUp", true);
                //anim.SetBool("isJumpDown", true);
                anim.SetBool("isJump", true);
                AudioManager.instance.PlaySFX("Pumping");
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
        if (isSliding == true)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
            {
                //VelocityZero();
                isSliding = false;
                //isMove = false;

                boxCollider.enabled = true;
                capsuleCollider.enabled = false;

                //float h = Input.GetAxisRaw("Horizontal");
                //rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
                //rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);

                gameManager.energyBar.value -= 1;
                animator.SetLayerWeight(1, 1);
                anim.SetTrigger("doSliding");
            }
  
            Invoke("MoveOn", 3f);
            Invoke("SlidingOff", 3f);
        }
    }

    private void SlidingOff()
    {
        boxCollider.enabled = false;
        capsuleCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            onDamaged(collision.transform.position, 5);
            gameManager.energyBar.value -= 5;
        }

        if (collision.gameObject.tag == "Platform")
        {
            //anim.SetBool("isJumpUp", false);
            //anim.SetBool("isJumpDown", false);
            anim.SetBool("isJump", false);
        }

        if (collision.gameObject.tag == "MineTrap")
        {
            //sfxManager.PlaySound("MineTrap");
            AudioManager.instance.PlaySFX("MineTrap");
            onDamaged(collision.transform.position, 5);
        }

        if (collision.gameObject.tag == "FootTrap")
        {
            isMove = false;

            VelocityZero();

            moveSpeed = 0;

            this.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y);

            AudioManager.instance.PlaySFX("FootTrap");

            //anim.Play("Idle");
            anim.SetBool("isRun", false);
            //anim.SetBool("isJumpUp", false);
            //anim.SetBool("isJumpDown", false);
            anim.SetBool("isJump", false);

            Invoke("MoveOn", 1.8f);
        }

        if (collision.gameObject.tag == "HpItem")
        {
            gameManager.energyBar.value += 4;
            //sfxManager.PlaySound("HpItem");
            AudioManager.instance.PlaySFX("HpItem");
        }

        if (collision.gameObject.tag == "SpeedItem")
        {
            //VelocityZero();
            moveSpeed += 10;
            Debug.Log(moveSpeed);
            //sfxManager.PlaySound("SpeedItem");
            AudioManager.instance.PlaySFX("SpeedItem");
            Invoke("MoveOn", 3f);
        }

        if (collision.gameObject.tag == "EmpItem")
        {
            //sfxManager.PlaySound("EmpItem");
            AudioManager.instance.PlaySFX("EmpItem");
        }
    }

    public void MoveOn()
    {
        isSliding = true;
        isMove = true;
        moveSpeed = 10;
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
            rigid.AddForce(new Vector2(dirc, 3f) * 4f, ForceMode2D.Impulse);
        }
        else if(what == 6)
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

    public void onDie()
    {
        gameManager.energyBar.value = 0f;

        UIReStart.SetActive(true);

        gameManager.menuSet.SetActive(false);

        gameManager.miniMap.SetActive(false);

        AudioManager.instance.PlaySFX("GameOver");

        anim.Play("Died");

        anim.SetTrigger("doDied");

        gameObject.layer = 11;

        Invoke("VelocityZero", 1f);
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

            moveSpeed = 0;

            VelocityZero();

            this.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y);

            anim.SetTrigger("doDoor");

            Invoke("MoveOn", 1f);
            Invoke("NextStage", 0.8f);
            Invoke("MoveSpeedReturn", 2f);
        }
    }

    private void NextStage()
    {
        gameManager.NextStage();
    }
}