using UnityEngine;
using UnityEngine.UI; // <-- BELANGRIJK: Vergeet deze using niet!

/// <summary>
/// Dit script beheert de volume sliders en roept de methoden in de AudioManager aan.
/// Plaats dit script op een game object in je Hoofdmenu/Instellingenscène.
/// </summary>
public class VolumeSettings : MonoBehaviour
{
    // Koppel deze in de Unity Inspector aan de UI Slider component.
    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    // Constante sleutels voor PlayerPrefs om de instellingen op te slaan
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    void Start()
    {
        // Zorg ervoor dat de AudioManager al geladen is
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager is niet gevonden. Zorg ervoor dat de AudioManager in je eerste scène geladen wordt en 'DontDestroyOnLoad' heeft.");
            return;
        }

        // 1. Laad de opgeslagen waarden (PlayerPrefs)
        // Gebruik de standaardwaarde 1.0 (volledig volume) als er nog niets is opgeslagen.
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1.0f);

        // 2. Pas de UI Sliders aan en stel direct het volume in
        
        if (musicSlider != null)
        {
            // Update de slider naar de opgeslagen waarde
            musicSlider.value = musicVolume;
            // Stel het volume in de AudioManager in (belangrijk voor muziek die al speelt)
            AudioManager.Instance.SetMusicVolume(musicVolume);
            // Koppel de methode aan de On Value Changed event, dit is de beste praktijk
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        else
        {
            // Zorg ervoor dat het volume ingesteld wordt, zelfs zonder slider
            AudioManager.Instance.SetMusicVolume(musicVolume);
        }

        if (sfxSlider != null)
        {
            // Update de slider naar de opgeslagen waarde
            sfxSlider.value = sfxVolume;
            // Stel het volume in de AudioManager in
            AudioManager.Instance.SetSFXVolume(sfxVolume);
            // Koppel de methode aan de On Value Changed event
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        else
        {
            // Zorg ervoor dat het volume ingesteld wordt, zelfs zonder slider
            AudioManager.Instance.SetSFXVolume(sfxVolume);
        }
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de Muziek Slider beweegt.
    /// </summary>
    /// <param name="volume">De nieuwe volumewaarde van de slider (0.0 tot 1.0).</param>
    public void SetMusicVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            // 1. Pas het volume aan in de AudioManager
            AudioManager.Instance.SetMusicVolume(volume);
            // 2. Sla de instelling op voor de volgende keer
            PlayerPrefs.SetFloat(MusicVolumeKey, volume);
            PlayerPrefs.Save(); // Sla direct op na elke verandering
        }
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de SFX Slider beweegt.
    /// </summary>
    /// <param name="volume">De nieuwe volumewaarde van de slider (0.0 tot 1.0).</param>
    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            // 1. Pas het volume aan in de AudioManager
            AudioManager.Instance.SetSFXVolume(volume);
            // 2. Sla de instelling op voor de volgende keer
            PlayerPrefs.SetFloat(SFXVolumeKey, volume);
            PlayerPrefs.Save(); // Sla direct op na elke verandering
        }
    }

    // OnApplicationQuit() is optioneel geworden door PlayerPrefs.Save() in de methoden
}