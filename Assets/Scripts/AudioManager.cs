using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer masterMixer;

    public Slider bgmSlider;
    public Slider sfxSlider;

    [SerializeField] Sound[] sfx = null;
    [SerializeField] Sound[] bgm = null;

    public AudioSource bgmPlayer = null;
    public AudioSource[] sfxPlayer = null;

    public float bgmSound;
    public float sfxSound;

    void Start()
    {
        instance = this;

        if (!PlayerPrefs.HasKey("BGMCheck"))
        {
            PlayerPrefs.SetFloat("BGMCheck", 1);
            PlayerPrefs.SetFloat("SFXCheck", 1);

            bgmSlider.value = 1;
            sfxSlider.value = 1;
        }
        else
        {
            AudioLoad();
        }

        int index = SceneManager.GetActiveScene().buildIndex;

        switch (index)
        {
            case 1:
                PlayBGM("Main");
                break;
            case 4:
                PlayBGM("Tutorial");
                break;
            case 5:
                PlayBGM("Stage_1");
                break;
            case 6:
                PlayBGM("Stage_2");
                break;
            case 7:
                PlayBGM("Stage_3");
                break;
            case 8:
                PlayBGM("Ending");
                break;
        }
    }

    private void Update()
    {
        AudioControl();
        AudioSave();
    }

    public void AudioControl()
    {
        bgmSound = bgmSlider.value;
        sfxSound = sfxSlider.value;

        if (bgmSound == 0)
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
        PlayerPrefs.SetInt("BGMCheck", 1);
        masterMixer.SetFloat("BGM", bgmSound + 2f);
    }

    public void BgmOff()
    {
        PlayerPrefs.SetInt("BGMCheck", 0);
        masterMixer.SetFloat("BGM", -80f);
    }

    public void SfxOn()
    {
        PlayerPrefs.SetInt("SFXCheck", 1);
        masterMixer.SetFloat("SFX", sfxSound + 2f);
    }

    public void SfxOff()
    {
        PlayerPrefs.SetInt("SFXCheck", 0);
        masterMixer.SetFloat("SFX", -80f);
    }

    public void PlayBGM(string p_bgmName)
    {
        for(int i = 0; i < bgm.Length; i++)
        {
            if(p_bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(string p_sfxName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (p_sfxName == sfx[i].name)
            {
                for(int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (!sfxPlayer[x].isPlaying)
                    {
                        sfxPlayer[x].clip = sfx[i].clip;
                        sfxPlayer[x].Play();
                        return;
                    }
                }
                return;
            }
        }
    }

    public void AudioSave()
    {
        PlayerPrefs.SetFloat("BGMCheck", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXCheck", sfxSlider.value);
        PlayerPrefs.Save();
    }

    public void AudioLoad()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGMCheck");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXCheck");
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}
