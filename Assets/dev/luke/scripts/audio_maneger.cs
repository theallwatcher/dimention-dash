using UnityEngine;
using System;

// Definieert het type geluid.
public enum SoundType {
    Music,
    SFX // Sound Effect
}

// Dit maakt een Singleton van de AudioManager, zodat deze eenvoudig aanroepbaar is.
public class AudioManager : MonoBehaviour
{
    // De static referentie naar de enige instantie van deze manager.
    public static AudioManager Instance;

    [Header("Audio Instellingen")]
    // Eén AudioSource voor achtergrondmuziek.
    public AudioSource musicSource;
    // Eén AudioSource voor geluidseffecten.
    public AudioSource sfxSource;
    
    [Space(10)]
    [Header("Geluidslijst")]
    // Een array van de Sound-structuur om je audio clips te organiseren.
    public Sound[] sounds;

    // Wordt aangeroepen wanneer het object wordt geladen.
    void Awake()
    {
        // --- Singleton Implementatie ---
        if (Instance == null)
        {
            Instance = this;
            // Zorgt ervoor dat de AudioManager blijft bestaan bij het laden van nieuwe scènes.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Vernietig dubbele managers als we er al één hebben.
            Destroy(gameObject);
            return;
        }

        // Zorg ervoor dat de AudioSource componenten bestaan en stel ze in.
        // Controleer of de musicSource bestaat. Zo niet, voeg toe.
        if (musicSource == null)
        {
            // Voeg een AudioSource toe, geef deze een duidelijke naam, en wijs toe.
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.gameObject.name = "MusicAudioSource";
        }
        
        // Controleer of de sfxSource bestaat. Zo niet, voeg toe.
        if (sfxSource == null)
        {
            // Voeg een AudioSource toe, geef deze een duidelijke naam, en wijs toe.
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.gameObject.name = "SFXAudioSource";
        }
    }

    /// <summary>
    /// Speelt een geluid af aan de hand van de naam (string).
    /// Roep aan met: AudioManager.Instance.Play("MijnGeluidsNaam");
    /// </summary>
    /// <param name="name">De naam (string) van het geluid dat moet worden afgespeeld.</param>
    public void Play(string name)
    {
        // Zoek het Sound-object in de array met de opgegeven naam.
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        // Controleer of er een geldig geluid is gevonden.
        if (s.name == null || s.name == "")
        {
            Debug.LogWarning($"Geluid: '{name}' is niet gevonden in de AudioManager!");
            return;
        }
        
        // Zorg ervoor dat er een clip is ingesteld
        if (s.clip == null)
        {
            Debug.LogWarning($"Geluid: '{name}' heeft geen AudioClip ingesteld!");
            return;
        }

        // Kies de juiste AudioSource (Music of SFX)
        AudioSource sourceToUse = (s.type == SoundType.Music) ? musicSource : sfxSource;
        
        // Stel de instellingen van de gekozen AudioSource in.
        sourceToUse.clip = s.clip;
        
        // BELANGRIJK: Zorg ervoor dat de Play() methode de volume-instelling van de slider respecteert,
        // maar de pitch en loop van de Sound struct behoudt.
        // We stellen alleen de pitch en loop in van de Sound struct.
        // sourceToUse.volume = s.volume; // Dit wordt nu extern via de slider geregeld
        sourceToUse.pitch = s.pitch;
        sourceToUse.loop = s.loop;

        // Als het een SFX is, gebruik dan PlayOneShot om meerdere tegelijk af te spelen.
        if (s.type == SoundType.SFX)
        {
            // Gebruik PlayOneShot voor SFX. We gebruiken het volume van de Sound struct (s.volume)
            // als een multiplier op het volume dat is ingesteld door de slider (sfxSource.volume).
            sfxSource.PlayOneShot(s.clip, s.volume * sfxSource.volume);
        }
        else // Als het Music is, speel dan gewoon (en overschrijf de vorige muziek).
        {
            // Voor muziek behoudt de musicSource.volume de waarde van de slider.
            // Als je wilt dat het individuele volume (s.volume) van de muziekclip ook wordt gebruikt,
            // dan moet je musicSource.volume = s.volume * volumeVandeSlider doen.
            // Omdat de slider het volume van musicSource al beheert, is sourceToUse.Play() voldoende.
            sourceToUse.Play();
        }
    }
    
    // Optionele methode om de muziek te stoppen.
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // ************************************************
    // NIEUWE VOLUME FUNCTIES VOOR DE SLIDERS
    // ************************************************

    /// <summary>
    /// Stelt het globale volume van de achtergrondmuziek in.
    /// Deze methode wordt aangeroepen door de UI Slider.
    /// </summary>
    /// <param name="volume">De nieuwe volumewaarde (0.0 tot 1.0).</param>
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            // Update direct het volume van de AudioSource
            musicSource.volume = volume;
        }
    }

    /// <summary>
    /// Stelt het globale volume van de geluidseffecten (SFX) in.
    /// Deze methode wordt aangeroepen door de UI Slider.
    /// </summary>
    /// <param name="volume">De nieuwe volumewaarde (0.0 tot 1.0).</param>
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            // Update direct het volume van de AudioSource.
            // Dit werkt als de globale multiplier voor PlayOneShot.
            sfxSource.volume = volume;
        }
    }
}

// Een serialiseerbare struct om je audio clips en hun instellingen te organiseren in de Unity Inspector.
[System.Serializable]
public struct Sound
{
    public string name;         // De unieke naam om het geluid mee aan te roepen (b.v. "JumpSound").
    public SoundType type;       // NEW: Is dit Music of SFX?
    public AudioClip clip;      // Het daadwerkelijke geluidsbestand.
    [Range(0f, 1f)]
    public float volume;        // Het individuele volume (wordt gebruikt als multiplier).
    [Range(0.1f, 3f)]
    public float pitch;         // De toonhoogte/afspeelsnelheid.
    public bool loop;           // Moet het geluid herhalen?
}