using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    public AudioMixer masterMixer;

    public AudioClip audioMain;
    public AudioClip audioLoading;
    public AudioClip audioStage_0;
    public AudioClip audioStage_1;
    public AudioClip audioStage_2;
    public AudioClip audioStage_3;
    public AudioClip audioStage_4;

    //사운드
    public AudioClip audioJump;
    public AudioClip audioDamaged;
    public AudioClip audioDie;
    public AudioClip audioMineTrap;
    public AudioClip audioHpItem;
    public AudioClip audioSpeedItem;
    public AudioClip audioEmpItem;

    AudioSource audioSource;

    public float bgmSound;
    public float sfxSound;

    int first;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioMain;


        if(first < 1)
        {
            bgmSlider.value = 1;
            sfxSlider.value = 1;
        }

        first++;

        if (bgmSlider.value == 1)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }

        if (sfxSlider.value == 1)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Update()
    {
        AudioControl();
    }

    public void AudioControl()
    {
        bgmSound = bgmSlider.value;
        sfxSound = sfxSlider.value;

        if(bgmSound == 0)
        {
            BgmOff();
        }
        else
        {
            BgmOn();
         }

        if (sfxSound == 0)
        {
            SfxOff();
        }
        else
        {
            SfxOn();
        }
    }

    public void BgmOn()
    {
        PlayerPrefs.SetInt("BGM", 1);
        masterMixer.SetFloat("BGM", bgmSound + 2f);
    }

    public void BgmOff()
    {
        PlayerPrefs.SetInt("BGM", 0);
        masterMixer.SetFloat("BGM", -80f);
    }

    public void SfxOn()
    {
        PlayerPrefs.SetInt("SFX", 1);
        masterMixer.SetFloat("SFX", sfxSound + 2f);
    }

    public void SfxOff()
    {
        PlayerPrefs.SetInt("SFX", 0);
        masterMixer.SetFloat("SFX", -80f);
    }

    public void PlaySound(string action)  //Player Sounds
    {
        switch (action)
        {
            case "Jump":
                audioSource.clip = audioJump;
                break;
            case "Damaged":
                audioSource.clip = audioDamaged;
                break;
            case "Die":
                audioSource.clip = audioDie;
                break;
            case "MineTrap":
                audioSource.clip = audioMineTrap;
                break;
            case "HpItem":
                audioSource.clip = audioHpItem;
                break;
            case "SpeedItem":
                audioSource.clip = audioSpeedItem;
                break;
            case "EmpItem":
                audioSource.clip = audioEmpItem;
                break;
        }

        audioSource.Play();

        if (action == "Died")
        {
            audioSource.PlayOneShot(audioDie);
        }
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }

    /* public void SoundSave()
    {
        PlayerPrefs.SetFloat("BGMCheck", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXCheck", sfxSlider.value);
        PlayerPrefs.Save();

        Debug.Log("사운드 매니저 Save BGM " + bgmSlider.value);
        Debug.Log("사운드 매니저 Save SFX " + sfxSlider.value);
    } */

    /* public void SoundLoad()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGMCheck");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXCheck");

        Debug.Log("사운드 매니저 Load BGM " + bgmSlider.value);
        Debug.Log("사운드 매니저 Load SFX " + sfxSlider.value);
    } */
}