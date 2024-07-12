using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChild : MonoBehaviour
{
    public AudioManager.AudioClipName clip;
    public AudioSource source;
    public List<AudioClip> Clips = new List<AudioClip>();
    private void OnEnable()
    {
        source.clip = GetClip();
        source.Play();
        StartCoroutine(DeactiveWhenClipPlayEnd());
    }
    private void Start()
    {
        if (Clips.Count == 1) source.clip = Clips[0];
        else source.clip = GetClip();
        source.Play();
        StartCoroutine(DeactiveWhenClipPlayEnd());
    }

    private IEnumerator DeactiveWhenClipPlayEnd()
    {
        yield return new WaitUntil(() => !source.isPlaying);
        this.gameObject.SetActive(false);
    }
    private AudioClip GetClip()
    {
        int x = Random.Range(0,Clips.Count);
        for(int i = 0; i < Clips.Count; i++)
        {
            if(x == i) return Clips[i]; 
        }
        return Clips[0];
    }
}
