﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        this.gameObject.SetActive(true);
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

        yield return new WaitForSeconds(2f);

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);
            spriteRenderer.color = alpha;
            yield return null;
        }

        this.gameObject.SetActive(true);
        SceneManager.LoadScene("Main");
        yield return null;
    }
}
