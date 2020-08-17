using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum BTNType
{
    New,
    Load,
    Save,
    Option,
    SFX,
    Bgm,
    Back,
    Main,
    Exit,
    Restart
}

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;

    public Transform buttonScale;

    public GameManager gameManager;

    public CanvasGroup mainGroup;
    public CanvasGroup optionGroup;

    Vector3 defaultScale;

    bool isSfx;
    bool isBgm;

    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    public void OnBtnClick()
    {
        AudioManager.instance.PlaySFX("Click");
        switch (currentType)
        {
            case BTNType.New:
                SceneLoad.LoadSceneHandle(3, 1);
                break;
            case BTNType.Load:
                //SceneLoad.LoadSceneHandle(gameManager.gameScene, 2);
                gameManager.GameLoad();
                break;
            case BTNType.Save:
                gameManager.GameSave();
                break;
            case BTNType.Option:
                CanvasGroupOn(optionGroup);
                CanvasGroupOff(mainGroup);
                break;
            case BTNType.SFX:
                if (isSfx)
                {
                    isSfx = false;
                }
                else
                {
                    isSfx = true;
                }
                break;
            case BTNType.Bgm:
                if (isBgm)
                {
                    isBgm = false;
                }
                else
                {
                    isBgm = true;
                }
                break;
            case BTNType.Back:
                CanvasGroupOn(mainGroup);
                CanvasGroupOff(optionGroup);
                break;
            case BTNType.Main:
                SceneLoad.LoadSceneHandle(1, 0);
                break;
            case BTNType.Exit:
                Application.Quit();
                break;
            case BTNType.Restart:
                //SceneManager를 이용하여 씬을 불러오는데, 현재 씬의 인덱스 번호를 불러와서 현재의 씬을 불러온다.
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }

    public void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}