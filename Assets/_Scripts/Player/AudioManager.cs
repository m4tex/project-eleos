using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public AudioSource playerAudio;
    public List<AudioClip> playerDamageSoundClips;
    public float pitchVariation = .2f;
    
    private static AudioManager _ins;

    public static void Damage()
    {
        if (_ins.playerAudio.isPlaying) return;
        
        var pitch = 1 - Random.Range(0, _ins.pitchVariation);

        _ins.playerAudio.pitch = pitch; 
        
        var clip = _ins.playerDamageSoundClips[Random.Range(0, _ins.playerDamageSoundClips.Count)];
        _ins.playerAudio.PlayOneShot(clip);
    }

    private void Awake()
    {
        _ins = this;
    }
}
