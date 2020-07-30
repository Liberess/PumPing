using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public Slider progressBar;
    public Text loadText;
    public static string loadScene;
    public static int loadType;

    public GameManager game;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadSceneHandle(string _name, int _loadType)
    {
        loadScene = _name;
        loadType = _loadType;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync("Stage_0");

        operation.allowSceneActivation = false;

        while (!operation.isDone)
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

            if (loadType == 0)
            {
                //Main화면으로
                SceneManager.LoadScene("Main");
            }
            else if (loadType == 1)
            {
                //새게임
                SceneManager.LoadScene("Stage_0");
            }
            else if (loadType == 2)
            {
                //옛게임
                game.GameLoad();
            }
        }
    }
}
