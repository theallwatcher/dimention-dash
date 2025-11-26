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

    //Singleton
    public static audioManagerSam Instance;

    //All sounds and their associated type - Set these in the inspector
    public Sound[] AllSounds;

    //Runtime collections
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
        //Set up sounds
        foreach (var s in AllSounds){

            _soundDictionary[s.Type] = s;
        }
    }

    //Call this method to play a sound
    public void Play(SoundType type){

        //Make sure there's a sound assigned to your specified type
        if (!_soundDictionary.TryGetValue(type, out Sound s)){

            Debug.LogWarning($"Sound type {type} not found!");
            return;
        }

        //Creates a new sound object
        var soundObj = new GameObject($"Sound_{type}");
        var audioSrc = soundObj.AddComponent<AudioSource>();

        //Assigns your sound properties
        audioSrc.clip = s.Clip;
        audioSrc.volume = s.Volume;

        //Play the sound
        audioSrc.Play();

        //Destroy the object
        Destroy(soundObj, s.Clip.length);
    }
}