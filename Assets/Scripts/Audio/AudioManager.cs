using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] GameObject audioSourcePrefab;
    [SerializeField] int audioSourceCount = 10;

    List<AudioSource> audioSource;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        audioSource = new List<AudioSource>();

        for(int i = 0;i < audioSourceCount; i++)
        {
            GameObject go = Instantiate(audioSourcePrefab,transform);
            go.transform.localPosition = Vector3.zero;
            audioSource.Add(go.GetComponent<AudioSource>());
        }
    }
    public void Play(AudioClip audioClip)
    {
        if (audioClip == null) return;

        AudioSource audioSource = GetFreeAudioSource();

        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private AudioSource GetFreeAudioSource()
    {
        for(int i = 0; i < audioSource.Count; i++)
        {
            if (audioSource[i].isPlaying == false)
            {
                return audioSource[i];
            }
        }
        return audioSource[0];
    }
}
