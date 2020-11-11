using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    [SerializeField] Text[] texts = null;

    float time = 0f;
    float F_time = 0.5f;

    Color alpha;

    public void Start()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        if (index >= 10)
        {
            StartCoroutine(EndingFadeFlow());
        }
        else
        {
            StartCoroutine(FadeFlow());
        }
    }

    IEnumerator FadeFlow()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            alpha = texts[i].color;
        }

        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);

            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = alpha;
            }
            yield return null;
        }

        time = 0f;

        yield return new WaitForSeconds(1f);

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);

            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = alpha;
            }
            yield return null;
        }

        /* int index = SceneManager.GetActiveScene().buildIndex;

        switch (index)
        {
            case 3:
                SceneManager.LoadScene("Tutorial");
                break;
            case 5:
                SceneManager.LoadScene("Stage_1");
                break;
            case 7:
                SceneManager.LoadScene("Stage_2");
                break;
            case 9:
                SceneManager.LoadScene("Stage_3");
                break;
        } */

        yield return null;
    }

    IEnumerator EndingFadeFlow()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            alpha = texts[i].color;

            time = 0f;

            while (alpha.a < 1f)
            {
                time += Time.deltaTime / F_time;
                alpha.a = Mathf.Lerp(0, 1, time);

                texts[i].color = alpha;

                yield return null;
            }

            time = 0f;

            if(i <= 1)
            {
                yield return new WaitForSeconds(3f);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
            }

            yield return new WaitForSeconds(2f);
        }
        SceneManager.LoadScene("Title");
    }
}
