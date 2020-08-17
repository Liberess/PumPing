using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    float time = 0f;
    float F_time = 1f;

    SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeFlow());
    }

    IEnumerator FadeFlow()
    {
        gameObject.SetActive(true);
        Color alpha = spriteRenderer.color;

        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            spriteRenderer.color = alpha;
            yield return null;
        }

        time = 0f;

        yield return new WaitForSeconds(1f);

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);
            spriteRenderer.color = alpha;
            yield return null;
        }

        gameObject.SetActive(true);
        yield return null;
    }
}
