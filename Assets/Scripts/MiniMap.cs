using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniMap : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject miniMap;
    public GameObject miniMapCamera;

    //cameraMove
    Transform caMove;

    //cameraSpeed
    protected float cameraSpeed = 0.5f;

    private int posJ = 0;
    private int posL = 0;
    private int posI = 0;
    private int posK = 0;

    void Start()
    {
        caMove = miniMapCamera.transform;

        int index = SceneManager.GetActiveScene().buildIndex;

        switch (index)
        {
            case 4:
                posJ = -10;
                posL = 45;
                posI = 30;
                posK = -2;
                break;
            case 5:
                posJ = -40;
                posL = 20;
                posI = 80;
                posK = -6;
                break;
            case 6:
                posJ = -10;
                posL = 22;
                posI = 60;
                posK = -2;
                break;
            case 7:
                posJ = -35;
                posL = 25;
                posI = 80;
                posK = -6;
                break;
        }
    }

    void Update()
    {
        //만약 'T'를 누르면 미니맵 On/Off
        if (Input.GetKeyDown(KeyCode.T) && gameManager.menuSet.activeSelf == false && gameManager.reStartUI.activeSelf == false)
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

            if(caMove.position.x >= posJ)
            {
                if (Input.GetKey(KeyCode.J))
                {
                    caMove.Translate(-(cameraSpeed), 0, 0);
                }
            }

            if(caMove.position.x <= posL)
            {
                if (Input.GetKey(KeyCode.L))
                {
                    caMove.Translate(cameraSpeed, 0, 0);
                }
            }

            if(caMove.position.y <= posI)
            {
                if (Input.GetKey(KeyCode.I))
                {
                    caMove.Translate(0, cameraSpeed, 0);
                }
            }

            if(caMove.position.y >= posK)
            {
                if (Input.GetKey(KeyCode.K))
                {
                    caMove.Translate(0, (-cameraSpeed), 0);
                }
            }
        }

        float x = gameManager.player.transform.position.x;
        float y = gameManager.player.transform.position.y;

        if (Input.GetKeyUp(KeyCode.T))
        {
            caMove.position = new Vector3(x, y + 5, -10);
        }
    }
}