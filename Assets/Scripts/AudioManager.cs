using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] AudioSource[] sfxPlayer = null;

    public float bgmSound;
    public float sfxSound;

    void Start()
    {
        instance = this;
        AudioLoad();
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
        //masterMixer.SetFloat("BGM", bgmSound + 2f);
    }

    public void BgmOff()
    {
        PlayerPrefs.SetInt("BGMCheck", 0);
        //masterMixer.SetFloat("BGM", -80f);
    }

    public void SfxOn()
    {
        PlayerPrefs.SetInt("SFXCheck", 1);
        //masterMixer.SetFloat("SFX", sfxSound + 2f);
    }

    public void SfxOff()
    {
        PlayerPrefs.SetInt("SFXCheck", 0);
        //masterMixer.SetFloat("SFX", -80f);
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
}
