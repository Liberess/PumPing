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

    public Slider energyBar;

    public GameObject player;
    public GameObject pumping;
    public GameObject menuSet;
    public GameObject miniMap;
    public GameObject reStartUI;

    private int stageIndex;
    public int gameScene;
    public float pumpingGauge;

    void Awake()
    {
        instance = this;

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
        //if (stageIndex != 0 && Input.GetButtonDown("Cancel") && miniMap.activeSelf == false && playerMv.UIReStart.activeSelf == false)
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

        stageIndex = SceneManager.GetActiveScene().buildIndex;
        gameScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerMv.onDie();
        }
    }

    public void GameSave()
    {
        //PlayerPrefs : 간단한 데이터 저장 기능을 지원하는 클래스
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
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

        player.transform.position = new Vector3(x, y, 0);
        
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
        player.transform.position = new Vector3(0, 0, -1);
        playerMv.VelocityZero();
    }

    public void NextStage()
    {
        if (gameScene < 9 && gameScene > 3)
        {
            SceneLoad.LoadSceneHandle(gameScene, 3);
            gameScene += 1;
        }
    }
}
