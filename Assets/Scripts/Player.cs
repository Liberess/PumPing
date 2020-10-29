using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.WebCam;

public class Player : MonoBehaviour
{
    //타 스크립트
    public GameManager gameManager;

    //플레이어 이동
    private float moveSpeed = 10;
    private float maxSpeed = 10;
    private float jumpPower = 10;
    private static float maxJump = 2;
    private int jumpCount = 0;

    //사망 효과음 On/Off
    private bool audioPlay;

    //플레이어 움직임 On/Off
    private bool isMove;
    private bool isSliding;
    private bool isEnding;

    private bool isGround;
    private bool isEnemy;

    private float slidingTimer = 0;
    private float slidingDelay = 5;

    private Animator animator;
    public Transform pos;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    BoxCollider2D boxCollider;  //Sliding

    Vector3 target = new Vector3(13.4f, 77f, 0f);

    private void Awake()
    {
        audioPlay = true;
        isMove = true;
        isSliding = true;
        isEnding = false;

        moveSpeed = 10;

        gameManager.mainEnergyBar.value = 15f;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGround = Physics2D.OverlapCircle(pos.position, 0.5f, LayerMask.GetMask("Platform"));
        isEnemy = Physics2D.OverlapCircle(pos.position, 0.5f, LayerMask.GetMask("Enemy"));

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
                Invoke("Ending", 3f);
            }
        }

        if (!isGround)
        {
            anim.SetBool("isJump", true);
        }
        else
        {
            anim.SetBool("isJump", false);
        }

        if (isEnemy)
        {
            anim.SetBool("isJump", false);
        }

        if (audioPlay)
        {
            if (gameManager.mainEnergyBar.value <= 0.5f)
            {
                onDie();
                audioPlay = false;
            }
            else
            {
                gameManager.mainEnergyBar.value = Mathf.MoveTowards(gameManager.mainEnergyBar.value, 14f, Time.deltaTime * 1f);
            }
        }

        //Energy가 0.5f 이상이고 살아있으며 isMove가 true여야 움직일 수 있다.
        if (gameManager.isAlive && isMove && gameManager.isMainPlayer)
        {
            Jump();

            if (isSliding && (slidingTimer >= slidingDelay))
            {
                Sliding();
            }
            slidingTimer += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (gameManager.isAlive && isMove && gameManager.isMainPlayer)
        {
            Move();
        }

        if (!gameManager.isAlive)
        {
            anim.SetTrigger("doDead");
        }

        //Landing Platform
        if (rigid.velocity.y <= 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Platform"));

            RaycastHit2D rayHit2 = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Enemy"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 1f)
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
            }
        }
    }

    private void Ending()
    {
        GameManager.instance.canvas.enabled = false;
        SceneManager.LoadScene("Ending", LoadSceneMode.Additive);
        AudioManager.instance.PlayBGM("Ending");
        AudioManager.instance.bgmPlayer.volume = 1;
    }

    private void Move()  //FixedUpdate
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
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

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            VelocityZero();
        }

        if (gameManager.isAlive)
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
        if (Input.GetButtonDown("Jump") && jumpCount < maxJump && gameManager.mainEnergyBar.value >= 1f)
        {
            animator.SetLayerWeight(1, 0);

            rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);

            anim.SetBool("isJump", true);

            jumpCount++;
            gameManager.mainEnergyBar.value--;

            AudioManager.instance.PlaySFX("Jump");
        }
    }

    private void Sliding()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
        {
            animator.SetLayerWeight(1, 1);

            isSliding = false;

            boxCollider.enabled = true;
            capsuleCollider.enabled = false;

            gameManager.mainEnergyBar.value -= 1;
            
            anim.SetTrigger("doSliding");
            anim.SetBool("isJump", false);
        }

        Invoke("SlidingOff", 1f);
    }

    private void SlidingOff()
    {
        slidingTimer = 0;

        isSliding = true;
        boxCollider.enabled = false;
        capsuleCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "GameController")
        {
            onDie();
        }

        if (collision.gameObject.tag == "Bullet")
        {
            AudioManager.instance.PlaySFX("Damaged");
            onDamaged(collision.transform.position, 5);
            gameManager.mainEnergyBar.value -= 5;
        }

        if (collision.gameObject.tag == "Land")
        {
            anim.SetBool("isJump", false);
            jumpCount = 0;
        }

        if (collision.gameObject.tag == "Platform")
        {
            anim.SetBool("isJump", false);
        }

        if (collision.gameObject.tag == "MineTrap")
        {
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

            anim.SetBool("isRun", false);
            anim.SetBool("isJump", false);

            Invoke("MoveOn", 1.8f);
        }

        if (collision.gameObject.tag == "SpeedItem")
        {
            moveSpeed += 10;
            Invoke("MoveOn", 3f);
        }
    }

    public void MoveOn()
    {
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
            //rigid.AddForce(new Vector2(dirc, 1f) * 5f, ForceMode2D.Impulse);
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

    public void onDie()
    {
        animator.SetLayerWeight(3, 1);
        gameManager.isAlive = false;

        gameManager.mainEnergyBar.value = 0f;
        gameManager.subEnergyBar.value = 0f;

        gameManager.reStartUI.SetActive(true);

        gameManager.menuSet.SetActive(false);

        gameManager.miniMap.SetActive(false);

        AudioManager.instance.PlaySFX("GameOver");

        anim.Play("Dead");

        anim.SetTrigger("doDead");

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

        gameObject.tag = "Untagged";
        gameObject.layer = 0;

        Invoke("VelocityZero", 3f);
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
            Invoke("NextStage", 0.6f);
            Invoke("MoveSpeedReturn", 2f);
        }

        if (collision.gameObject.tag == "Ending")
        {
            isMove = false;

            moveSpeed = 0;

            VelocityZero();

            isEnding = true;
        }
    }

    private void NextStage()
    {
        gameManager.NextStage();
    }
}