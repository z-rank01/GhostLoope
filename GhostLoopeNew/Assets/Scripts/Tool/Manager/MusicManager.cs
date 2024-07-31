using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : BaseSingleton<MusicManager>
{
    private AudioSource bkMusic = null; // 背景音乐组件
    private float bkValue = 1; // 背景音乐音量 [0,1]

    private float soundValue = 1; // 音效音量 [0,1]

    private GameObject soundObj = null;
    private List<AudioSource> soundList = new List<AudioSource>(); // 音效列表

    // 播放背景音乐
    public void PlayBkMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            bkMusic = obj.AddComponent<AudioSource>();

            //bkMusic = GameObject.FindGameObjectWithTag("MusicBK").GetComponent<AudioSource>();
            
        }

        ResourcesManager.GetInstance().LoadResourceAsync<AudioClip>("Music/BK/" + name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        });

    }
    // 停止播放背景音乐
    public void StopBKMusic()
    {
        if (bkMusic == null) return;
        bkMusic.Stop();
    }
    
    // 开始播放对应名字的音效
    public void PlaySound(string name)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";
        }

        ResourcesManager.GetInstance().LoadResourceAsync<AudioClip>("Music/BK/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();

            source.clip = clip;
            source.volume = soundValue;
            source.Play();

            soundList.Add(source);
        });

    }
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
    public void update()
    {
        //Debug.Log("Music Update");
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }
    public void ChangeBKValue(float value)
    {
        //bkValue = value;
        bkMusic.volume = value;
    }
}
