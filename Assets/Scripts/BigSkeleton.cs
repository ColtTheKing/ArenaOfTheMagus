using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSkeleton : MonoBehaviour
{
    public List<AudioClip> enterVoiceLines, advanceVoiceLines, loseVoiceLines;

    private AudioSource voiceSource;
    private int curEnterLine, curLoseLine;

    void Awake()
    {
        voiceSource = GetComponent<AudioSource>();
    }

    public void PlayEnter()
    {
        if (curEnterLine >= enterVoiceLines.Count)
        {
            voiceSource.clip = enterVoiceLines[Random.Range(0, enterVoiceLines.Count)];
            voiceSource.Play();
        }
        else
        {
            voiceSource.clip = enterVoiceLines[curEnterLine++];
            voiceSource.Play();
        }
    }

    public void PlayAdvance(int voiceLine)
    {
        if(voiceLine < advanceVoiceLines.Count)
        {
            voiceSource.clip = advanceVoiceLines[voiceLine];
            voiceSource.Play();
        }
    }

    public void PlayLose()
    {
        if (curLoseLine >= loseVoiceLines.Count)
        {
            voiceSource.clip = loseVoiceLines[Random.Range(0, loseVoiceLines.Count)];
            voiceSource.Play();
        }
        else
        {
            voiceSource.clip = loseVoiceLines[curLoseLine++];
            voiceSource.Play();
        }
    }
}
