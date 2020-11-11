using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public static MiniMap instance;

    GameManager gameManager;

    public GameObject miniMap;
    public GameObject miniMapMainCam;
    public GameObject miniMapSubCam;

    public GameObject mapImg;

    public Texture mainTexture;
    public Texture subTexture;

    //cameraMove
    Transform caMove;

    //cameraSpeed
    protected float cameraSpeed = 0.2f;

    private int posJ = 0;
    private int posL = 0;
    private int posI = 0;
    private int posK = 0;

    private float x;
    private float y;

    Vector3 originMainCamPos;
    Vector3 originSubCamPos;

    void Start()
    {
        instance = this;

        gameManager = GameManager.instance;

        originMainCamPos = miniMapMainCam.transform.position;
        originSubCamPos = miniMapSubCam.transform.position;

        if (gameManager.isMainPlayer)
        {
            caMove = miniMapMainCam.transform;
        }
        else
        {
            caMove = miniMapSubCam.transform;
        }

        int index = SceneManager.GetActiveScene().buildIndex;

        switch (index)
        {
            case 4:
                posJ = -10;
                posL = 45;
                posI = 30;
                posK = -2;
                break;
            case 6:
                posJ = -40;
                posL = 20;
                posI = 80;
                posK = -6;
                break;
            case 8:
                posJ = -10;
                posL = 22;
                posI = 60;
                posK = -2;
                break;
            case 10:
                posJ = -35;
                posL = 25;
                posI = 80;
                posK = -6;
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            miniMap.SetActive(false);
        }

        //만약 'T'를 누르면 미니맵 On/Off
        if (Input.GetKeyDown(KeyCode.T) && gameManager.menuSet.activeSelf == false)
        {
            if (miniMap.activeSelf)
            {
                AudioManager.instance.PlaySFX("Minimap_Off");
                miniMap.SetActive(false);
            }
            else
            {
                AudioManager.instance.PlaySFX("Minimap_On");
                miniMap.SetActive(true);
            }
        }

        if (miniMap.activeSelf)
        {
            if (gameManager.isMainPlayer)
            {
                mapImg.GetComponent<RawImage>().texture = mainTexture;
                caMove = miniMapMainCam.transform;
            }
            else
            {
                mapImg.GetComponent<RawImage>().texture = subTexture;
                caMove = miniMapSubCam.transform;
            }

            MapCamMove();
        }

        if (gameManager.isMainPlayer)
        {
            x = Player.instance.transform.position.x;
            y = Player.instance.transform.position.y;
        }
        else
        {
            x = SubPlayer.instance.transform.position.x;
            y = SubPlayer.instance.transform.position.y;
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            ResetCamPos();
        }
    }

    private void MapCamMove()
    {
        if (caMove.position.y <= posI)
        {
            if (Input.GetKey(KeyCode.I))
            {
                caMove.Translate(0, cameraSpeed, 0);
            }
        }

        if (caMove.position.x >= posJ)
        {
            if (Input.GetKey(KeyCode.J))
            {
                caMove.Translate(-(cameraSpeed), 0, 0);
            }
        }

        if (caMove.position.x <= posL)
        {
            if (Input.GetKey(KeyCode.L))
            {
                caMove.Translate(cameraSpeed, 0, 0);
            }
        }

        if (caMove.position.y >= posK)
        {
            if (Input.GetKey(KeyCode.K))
            {
                caMove.Translate(0, (-cameraSpeed), 0);
            }
        }
    }

    public void ResetCamPos()
    {
        miniMapMainCam.transform.position = originMainCamPos;
        miniMapSubCam.transform.position = originSubCamPos;

        caMove.position = new Vector3(x, y + 5, -10);
    }
}