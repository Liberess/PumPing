using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player playerMv;
    public AudioManager audioManager;
    public PumpingGauge pumpingManager;
    public Canvas canvas;

    public Slider mainEnergyBar;
    public Slider subEnergyBar;

    public GameObject mainPlayer;
    public GameObject subPlayer;
    public GameObject pumping;
    public GameObject menuSet;
    public GameObject miniMap;
    public GameObject reStartUI;

    public GameObject mainCamera;
    public GameObject subCamera;

    public int gameScene;
    public float pumpingGauge;

    public bool isMainPlayer;
    public bool isAlive;

    private float time;
    private float delaytime = 2f;

    void Awake()
    {
        instance = this;

        isAlive = true;
        isMainPlayer = true;

        if (!PlayerPrefs.HasKey("BGMCheck"))
        {
            PlayerPrefs.SetFloat("BGMCheck", 1);
            PlayerPrefs.SetFloat("SFXCheck", 1);
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(1920, 1080, true);
    }

    public void Update()
    {
        //Sub Menu
        //esc를 눌렀을 때 만약 stage가 0, 즉 메인화면이라면 메뉴화면을 켤 수 없다.
        //또한 현재 미니맵이 꺼져있고 재시작 버튼이 비활성화 되어 있어야지 메뉴를 불러 올 수 있다.
        if (gameScene > 2 && Input.GetButtonDown("Cancel") && miniMap.activeSelf == false && reStartUI.activeSelf == false)
        {
            if (menuSet.activeSelf)
            {
                menuSet.SetActive(false);
            }
            else
            {
                menuSet.SetActive(true);
            }
        }

        if(time >= delaytime)
        {
            if (gameScene > 4 && Input.GetKeyDown(KeyCode.R))
            {
                ChangePlayer();
            }
        }

        time += Time.deltaTime;

        gameScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void ChangePlayer()
    {
        MiniMap.instance.ResetCamPos();

        if (isMainPlayer)
        {
            mainCamera.SetActive(false);
            subCamera.SetActive(true);

            isMainPlayer = false;
        }
        else
        {
            mainCamera.SetActive(true);
            subCamera.SetActive(false);

            isMainPlayer = true;
        }

        time = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainPlayer" || collision.gameObject.tag == "SubPlayer")
        {
            playerMv.onDie();
        }
    }

    public void GameSave()
    {
        //PlayerPrefs : 간단한 데이터 저장 기능을 지원하는 클래스
        PlayerPrefs.SetFloat("PlayerX", mainPlayer.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", mainPlayer.transform.position.y);
        PlayerPrefs.SetFloat("SubPlayerX", subPlayer.transform.position.x);
        PlayerPrefs.SetFloat("SubPlayerY", subPlayer.transform.position.y);
        PlayerPrefs.SetInt("GameScene", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetFloat("BGMCheck", audioManager.bgmSlider.value);
        PlayerPrefs.SetFloat("SFXCheck", audioManager.sfxSlider.value);
        PlayerPrefs.Save();

        menuSet.SetActive(false);
    }

    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("PlayerX"))
        {
            return;
        }

        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");

        mainPlayer.transform.position = new Vector3(x, y, 0);
        
        gameScene = PlayerPrefs.GetInt("GameScene");
        audioManager.bgmSlider.value = PlayerPrefs.GetFloat("BGMCheck");
        audioManager.sfxSlider.value = PlayerPrefs.GetFloat("SFXCheck");

        SceneManager.LoadScene(gameScene);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void PlayerReposition()
    {
        mainPlayer.transform.position = new Vector3(0, 0, -1);
        playerMv.VelocityZero();
    }

    public void NextStage()
    {
        if (gameScene < 11 && gameScene > 3)
        {
            SceneLoad.LoadSceneHandle(gameScene, 3);
            gameScene += 1;
        }
    }
}
