using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PumpingManager : MonoBehaviour
{
    public static PumpingManager instance;

    GameManager gameManager;

    public Image pumpingImage;
    public Sprite emptySprite;
    public Sprite halfSprite;
    public Sprite fullSprite1;
    public Sprite fullSprite2;

    private float timer;
    private float waitingTime;

    private int count = 0;

    void Start()
    {
        instance = this;

        gameManager = GameManager.instance;

        timer = 0.0f;
        waitingTime = 0.2f;
        pumpingImage = GetComponent<Image>();
    }

    void Update()
    {
        if (gameManager.pumpingGauge < 0.1f)
        {
            gameObject.GetComponent<Image>().sprite = emptySprite;
        }
        else if (gameManager.pumpingGauge >= 0.1f && gameManager.pumpingGauge < 0.5f)
        {
            gameObject.GetComponent<Image>().sprite = halfSprite;
        }
        else if (gameManager.pumpingGauge >= 0.7f)
        {
            timer += Time.deltaTime;

            count++;

            if (timer > waitingTime)
            {
                FullSprite();

                timer = 0;
                count = 0;
            }
        }
    }

    private void FullSprite()
    {
        if (count % 2 != 0)
        {
            gameObject.GetComponent<Image>().sprite = fullSprite1;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = fullSprite2;
        }
    }
}
