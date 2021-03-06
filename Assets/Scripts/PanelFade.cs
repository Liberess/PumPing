﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelFade : MonoBehaviour
{
    public static PanelFade instance;
    public GameObject UI;
    public Image Panel;
    float time = 0f;
    float F_time = 1f;

    public bool doFade;

    public void Start()
    {
        instance = this;

        doFade = false;
        UI.SetActive(false);
        StartCoroutine(FadeFlow());
    }

    public void Update()
    {
        if (doFade)
        {
            doFade = false;
            UI.SetActive(false);
            StartCoroutine(FadeFlow());
        }
    }

    IEnumerator FadeFlow()
    {
        Panel.gameObject.SetActive(true);
        Color alpha = Panel.color;

        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            Panel.color = alpha;
            yield return null;
        }

        time = 0f;

        yield return new WaitForSeconds(1f);

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);
            Panel.color = alpha;
            yield return null;
        }

        UI.SetActive(true);
        Panel.gameObject.SetActive(false);
        yield return null;
    }
}