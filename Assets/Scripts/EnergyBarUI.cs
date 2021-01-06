using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public static EnergyBarUI instance;

    public Sprite MainOn;
    public Sprite MainOff;

    public Sprite SubOn;
    public Sprite SubOff;

    public Sprite MainFillOn;
    public Sprite MainFillOff;

    public Sprite SubFillOn;
    public Sprite SubFillOff;

    public Image MainFill;
    public Image SubFill;

    public Image MainMiddle;
    public Image SubMiddle;

    public GameObject MainHighlight;
    public GameObject SubHighlight;

    Animator anim;

    private void Awake()
    {
        instance = this;

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.instance.isMainPlayer)
        {
            MainFill.sprite = MainFillOn;
            SubFill.sprite = SubFillOff;

            MainMiddle.sprite = MainOn;
            SubMiddle.sprite = SubOff;

            MainHighlight.SetActive(true);
            SubHighlight.SetActive(false);
        }
        else
        {
            MainFill.sprite = MainFillOff;
            SubFill.sprite = SubFillOn;

            MainMiddle.sprite = MainOff;
            SubMiddle.sprite = SubOn;

            MainHighlight.SetActive(false);
            SubHighlight.SetActive(true);
        }
    }

    public void Change()
    {
        anim.SetTrigger("doChange");
    }
}
