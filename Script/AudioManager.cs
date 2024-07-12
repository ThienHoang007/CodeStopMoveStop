using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;
    public static AudioManager Instance => instance;
    public bool isTurnOffAudio = false;

    [Header("Audio Clip")]
    [SerializeField] private List<AudioChild> clipList = new List<AudioChild>();
    private List<AudioChild> Childs = new List<AudioChild>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (this.GetInstanceID() != instance.GetInstanceID()) Destroy(this);
        DontDestroyOnLoad(this.gameObject);
    }
    public void PlaySound(AudioClipName CLip, Transform pos)
    {
        if (isTurnOffAudio) return; 
        AudioChild audioClip = null;
        for(int i = 0; i < clipList.Count; i++)
        {
            if (clipList[i].clip == CLip) audioClip = clipList[i];
        }

        if(audioClip == null)
        {
            Debug.LogError("Don't Have AudioChild");
            return;
        }

        if (Childs.Count == 0) Childs.Add(Instantiate<AudioChild>(audioClip, pos.position, Quaternion.identity, this.transform));
        else
        {
            foreach (AudioChild child in Childs)
            {
                if (!child.gameObject.activeSelf && child.clip == audioClip.clip)
                {
                    child.gameObject.SetActive(true);
                    child.transform.position = pos.position;
                    return;
                }
            }
            Childs.Add(Instantiate<AudioChild>(audioClip, pos.position, Quaternion.identity, this.transform));
        }
    }
    public enum AudioClipName { NemVuKhi, WeoponHit, BtnClick, MainDead, EnemyDead, ZombieDead, CountDown, GameWin, LevelUp }
    
}
