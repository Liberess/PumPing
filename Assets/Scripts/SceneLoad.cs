using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public Slider progressBar;
    public Text loadText;

    public static int gameScene;
    public static int loadType;

    public GameManager gameManager;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadSceneHandle(int _gameScene, int _loadType)
    {
        gameScene = _gameScene;
        loadType = _loadType;

        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(gameScene); //원래는 Stage_0 -> gameScene -> game.Manager.gameScene

        operation.allowSceneActivation = false;

        while (!operation.isDone)  //작업이 true 상태가 될때까지
        {
            yield return null;

            if (progressBar.value < 0.9f)
            {
                progressBar.value = Mathf.MoveTowards(progressBar.value, 0.9f, Time.deltaTime);
            }
            else if (operation.progress >= 0.9f)
            {
                progressBar.value = Mathf.MoveTowards(progressBar.value, 1f, Time.deltaTime);
            }

            if (progressBar.value >= 1f)
            {
                loadText.text = "Press SpaceBar !!";
            }

            if (Input.GetKeyDown(KeyCode.Space) && (progressBar.value >= 1f) && (operation.progress >= 0.9f))
            {
                operation.allowSceneActivation = true;
            }
        }

        if (loadType == 0)
        {
            SceneManager.LoadScene("Main");  //Main화면으로
        }
        else if (loadType == 1)
        {
            SceneManager.LoadScene("Stage_0");  //새게임
        }
        else if (loadType == 2)
        {
            gameManager.GameLoad();  //옛게임
        }
        else
        {
            gameManager.NextStage();
        }
    }
}
