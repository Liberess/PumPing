using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public Text stateTxt;

    private bool canSave;

    private float time = 0f;
    private float F_time = 0.5f;

    Color alpha;

    Animator anim;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canSave = true;

        anim = GetComponent<Animator>();
    }

    IEnumerator FadeFlow()
    {
        alpha = stateTxt.color;

        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);

            stateTxt.color = alpha;

            yield return null;
        }

        time = 0f;

        yield return new WaitForSeconds(1f);

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);

            stateTxt.color = alpha;

            yield return null;
        }

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(canSave == true)
        {
            if (this.gameObject.tag == "MainSave" && collision.gameObject.tag == "MainPlayer")
            {
                canSave = false;

                anim.SetTrigger("doSave");

                stateTxt.text = "Save Complete";

                GameManager.instance.mainPos = gameObject.transform.position;

                StartCoroutine(FadeFlow());
            }

            if (this.gameObject.tag == "SubSave" && collision.gameObject.tag == "SubPlayer")
            {
                canSave = false;

                anim.SetTrigger("doSave");

                stateTxt.text = "Save Complete";

                GameManager.instance.subPos = gameObject.transform.position;

                StartCoroutine(FadeFlow());
            }
        }
    }
}