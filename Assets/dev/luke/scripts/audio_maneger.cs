using UnityEngine;
using System;

// Dit maakt een Singleton van de AudioManager, zodat deze eenvoudig aanroepbaar is.
public class AudioManager : MonoBehaviour
{
    // De static referentie naar de enige instantie van deze manager.
    public static AudioManager Instance;

    [Header("Audio Instellingen")]
    // De AudioSource component die de geluiden daadwerkelijk zal afspelen.
    public AudioSource audioSource;
    
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

        // Zorg ervoor dat er een AudioSource component is om de geluiden af te spelen.
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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
        // Omdat 'Sound' een struct is, controleren we of de naam leeg is bij een niet-gevonden element.
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

        // Stel de instellingen van de centrale AudioSource in.
        audioSource.clip = s.clip;
        audioSource.volume = s.volume;
        audioSource.pitch = s.pitch;
        audioSource.loop = s.loop;

        // Speel het geluid af.
        audioSource.Play();
    }
}

// Een serialiseerbare struct om je audio clips en hun instellingen te organiseren in de Unity Inspector.
[System.Serializable]
public struct Sound
{
    public string name;         // De unieke naam om het geluid mee aan te roepen (b.v. "JumpSound").
    public AudioClip clip;      // Het daadwerkelijke geluidsbestand.
    [Range(0f, 1f)]
    public float volume;        // Het volume (van 0 tot 1).
    [Range(0.1f, 3f)]
    public float pitch;         // De toonhoogte/afspeelsnelheid.
    public bool loop;           // Moet het geluid herhalen?
}