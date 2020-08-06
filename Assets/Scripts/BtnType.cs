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
    Sound,
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

    public Text SoundText;
    public Text BgmText;

    Vector3 defaultScale;

    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    bool isSound;
    bool isBgm;

    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.New:
                SceneLoad.LoadSceneHandle(2, 1);
                break;
            case BTNType.Load:
                //SceneLoad.LoadSceneHandle(gameManager.gameScene, 2);
                gameManager.GameLoad();
                break;
            case BTNType.Option:
                CanvasGroupOn(optionGroup);
                CanvasGroupOff(mainGroup);
                break;
            case BTNType.Sound:
                if (isSound)
                {
                    isSound = false;
                    SoundText.text = "Sound Off";
                }
                else
                {
                    isSound = true;
                    SoundText.text = "Sound On";
                }
                break;
            case BTNType.Bgm:
                if (isBgm)
                {
                    isBgm = false;
                    BgmText.text = "Bgm Off";
                }
                else
                {
                    isBgm = true;
                    BgmText.text = "Bgm On";
                }
                break;
            case BTNType.Back:
                CanvasGroupOn(mainGroup);
                CanvasGroupOff(optionGroup);
                break;
            case BTNType.Main:
                SceneLoad.LoadSceneHandle(0, 0);
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