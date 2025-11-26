using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class audioManagerSam : MonoBehaviour
{
    public enum SoundType{

        Jump,
        Powerup,
        buton
    }

    [System.Serializable]
    public class Sound{

        public SoundType Type;
        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume = 1f;

        [HideInInspector]
        public AudioSource Source;
    }

    public static audioManagerSam Instance;

    public Sound[] AllSounds;

    private Dictionary<SoundType, Sound> _soundDictionary = new Dictionary<SoundType, Sound>();
    private AudioSource _musicSource;

    private void Awake(){

        if (Instance == null){

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{

            Destroy(gameObject);
        }

        foreach (var s in AllSounds){

            _soundDictionary[s.Type] = s;
        }
    }

    public void Play(SoundType type){

        if (!_soundDictionary.TryGetValue(type, out Sound s)){

            Debug.LogWarning($"Sound type {type} not found!");
            return;
        }

        var soundObj = new GameObject($"Sound_{type}");
        var audioSrc = soundObj.AddComponent<AudioSource>();
        audioSrc.clip = s.Clip;
        audioSrc.volume = s.Volume;
        audioSrc.Play();
        Destroy(soundObj, s.Clip.length);
    }
}