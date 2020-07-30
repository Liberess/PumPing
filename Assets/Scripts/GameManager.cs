using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Player playerMv;

    public Slider energyBar;

    public GameObject pumping;
    public GameObject menuSet;
    public GameObject player;
    public GameObject miniMap;

    public int stageIndex;
    public string gaSc;
    public float pumpingGauge;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(1920, 1080, true);
    }

    public void Start()
    {
        //GameLoad();
    }

    public void Update()
    {
        //Sub Menu
        //esc를 눌렀을 때 만약 stage가 0, 즉 메인화면이라면 메뉴화면을 켤 수 없다.
        //또한 현재 미니맵이 꺼져있고 재시작 버튼이 비활성화 되어 있어야지 메뉴를 불러 올 수 있다.
        if (stageIndex != 0 && Input.GetButtonDown("Cancel") && miniMap.activeSelf == false && playerMv.UIReStart.activeSelf == false)
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
    }

    public void NextStage()
    {
        stageIndex++;

        //Calculate Point
        /* totalPoint += stagePoint;
        stagePoint = 0; */

        SceneManager.LoadScene("Stage_2");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
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
        PlayerPrefs.SetString("GaSc", SceneManager.GetActiveScene().name);
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
        int gameScene = PlayerPrefs.GetInt("GameScene");
        gaSc = PlayerPrefs.GetString("GaSc");

        SceneManager.LoadScene(gameScene);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
