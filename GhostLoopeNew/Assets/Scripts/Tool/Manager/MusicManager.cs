using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : BaseSingleton<MusicManager>
{
    private AudioSource backgroundMusic = null; // �����������

    //private AudioSource environmnentMusic = null; // ��������

    //private AudioSource fireMusic = null; // �������



    private float backgroundValue = 0.5f; // ������������ [0,1]

    private float environmentValue = 0.5f; // ��Ч���� [0,1]


    private float fireValue = 0.5f; // ������������ [0,1]


    private GameObject soundObj = new GameObject("Sound");
    private List<AudioSource> soundList = new List<AudioSource>(); // ��Ч�б�

    // ���ű�������
    public void PlayBackgroundMusic(string name)
    {
        if (backgroundMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            backgroundMusic = obj.AddComponent<AudioSource>();

            //bkMusic = GameObject.FindGameObjectWithTag("MusicBK").GetComponent<AudioSource>();
            
        }

        ResourcesManager.GetInstance().LoadResourceAsync("Music/Background/" + name, (clip) =>
        {
            backgroundMusic.clip = clip as AudioClip;
            backgroundMusic.loop = true;
            backgroundMusic.volume = backgroundValue;
            backgroundMusic.Play();
        });

    }
    // ֹͣ���ű�������
    public void StopBackgroundMusic()
    {
        if (backgroundMusic == null) return;
        backgroundMusic.Stop();
    }



    

    public void PlayEnvironmentSound(string name)
    {
        ResourcesManager.GetInstance().LoadResourceAsync("Music/Environment/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();

            source.clip = clip as AudioClip;
            source.volume = environmentValue;
            source.Play();

            soundList.Add(source);
        });
    }





    // ��ʼ�����������Ч
    public void PlayFireSound(string name)
    {

        ResourcesManager.GetInstance().LoadResourceAsync("Music/Fire/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();

            source.clip = clip as AudioClip;
            source.volume = fireValue;
            source.Play();

            soundList.Add(source);
        });
    }













    //public void StopSound(AudioSource source)
    //{
    //    if (soundList.Contains(source))
    //    {
    //        soundList.Remove(source);
    //        source.Stop();
    //        GameObject.Destroy(source);
    //    }
    //}
    //public void update()
    //{
    //    //Debug.Log("Music Update");
    //    for (int i = soundList.Count - 1; i >= 0; i--)
    //    {
    //        if (!soundList[i].isPlaying)
    //        {
    //            GameObject.Destroy(soundList[i]);
    //            soundList.RemoveAt(i);
    //        }
    //    }
    //}
    public void ChangeBackgroundValue(float value)
    {
        if(backgroundMusic == null) return;
        backgroundValue = value;
        backgroundMusic.volume = value;
    }
    public void ChangeEnvironmentValue(float value)
    {
        
        environmentValue = value;
        //environmnentMusic.volume = value;
    }
    public void ChangeFireValue(float value)
    {
        fireValue = value;
        //fireMusic.volume = value;
    }

}
