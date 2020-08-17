using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFade : MonoBehaviour
{
    public float FadeTime = 2f; // Fade효과 재생시간

    Image fadeImg;
    Text text;

    float start;
    float end;
    float time = 0f;
    bool isPlaying = false;

    void Awake()
    {
        fadeImg = GetComponent<Image>();
        text = GetComponent<Text>();
        InStartFadeAnim();
    }

    public void OutStartFadeAnim()
    {
        if (isPlaying == true) //중복재생방지
        {
            return;
        }
        start = 1f;
        end = 0f;

        StartCoroutine("fadeoutplay");    //코루틴 실행
    }

    public void InStartFadeAnim()
    {
        if (isPlaying == true) //중복재생방지
        {
            return;
        }
        StartCoroutine("fadeIntanim");
    }

    IEnumerator fadeoutplay()
    {
        time = 0f;
        isPlaying = true;
        Color fadeColor_Text = text.color;
        Color fadecolor = fadeImg.color;
        fadecolor.a = Mathf.Lerp(start, end, time);
        fadeColor_Text.a = Mathf.Lerp(start, end, time);

        while (fadecolor.a > 0f)
        {
            time += Time.deltaTime / FadeTime;
            fadecolor.a = Mathf.Lerp(start, end, time);
            fadeColor_Text.a = Mathf.Lerp(start, end, time);
            fadeImg.color = fadecolor;
            text.color = fadeColor_Text;

            yield return null;
        }
        isPlaying = false;
    }
}