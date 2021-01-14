using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTurret : MonoBehaviour
{
    public static MoveTurret instance;

    public RuntimeAnimatorController[] turretAc;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    private int nextMove;

    public int ID;
    public bool isMove;

    private void Awake()
    {
        instance = this;

        isMove = true;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        ChangeAc();

        Invoke("Think", 3f);
    }

    private void FixedUpdate()
    {
        if (isMove)  //Move
        {
            rigid.velocity = new Vector2(nextMove * 2f, rigid.velocity.y);

            //Platform Check
            Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y - 0.5f);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider == null)
            {
                Turn();
            }
        }
    }

    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);

        if(nextMove == 0)
        {
            nextMove += 1;
        }

        //Flip Sprite
        if(nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }

        //Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= (-1);
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2f);
    }

    private void ChangeAc()
    {
        anim.runtimeAnimatorController = turretAc[ID];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MainPlayer"))
        {
            GameManager.instance.mainEnergyBar.value -= 7;
            Player.instance.onDamaged(collision.transform.position, 6);
        }

        if (collision.gameObject.CompareTag("SubPlayer"))
        {
            GameManager.instance.subEnergyBar.value -= 7;
            SubPlayer.instance.onDamaged(collision.transform.position, 6);
        }
    }
}