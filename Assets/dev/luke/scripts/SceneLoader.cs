using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Nodig voor Coroutines (IEnumerator)

public class SceneLoader : MonoBehaviour
{
    // De tijd die we wachten nadat het geluid is gestart, voordat we de scene laden.
    // Pas deze waarde aan op basis van de lengte van je "button" geluid.
    [Tooltip("Tijd in seconden om te wachten nadat het geluid is afgespeeld.")]
    public float loadDelay = 0.3f; // 0.3 seconden is vaak genoeg voor een klikgeluid
    public GameObject manubutton;
    public GameObject menuui;
    public GameObject backbutton;
    public GameObject closmenu;

    /// <summary>
    /// De nieuwe publieke methode die je aan de Button (On Click) koppelt.
    /// Deze start de Coroutine om het laden met vertraging uit te voeren.
    /// </summary>
    /// <param name="buildIndex">De index van de te laden scene.</param>
    public void PlaySoundAndLoadScene(int buildIndex)
    {
        // 1. Speel het geluid af
        // Zorg ervoor dat de AudioManager klasse beschikbaar is.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("button");
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance is null. Scene zal zonder geluid laden.");
        }

        // 2. Start de Coroutine die de scene met vertraging laadt
        StartCoroutine(LoadSceneAfterDelay(buildIndex));
    }

    public void openmenu()
    {
        manubutton.SetActive(false);
        menuui.SetActive(true);
        backbutton.SetActive(false);
        closmenu.SetActive(true);
    }
    public void closemenu()
    {
        manubutton.SetActive(true);
        menuui.SetActive(false);
        backbutton.SetActive(true);
        closmenu.SetActive(false);
    }

    /// <summary>
    /// Coroutine om de scene te laden na een korte vertraging.
    /// </summary>
    private IEnumerator LoadSceneAfterDelay(int buildIndex)
    {
        // Wacht de ingestelde tijd (loadDelay)
        yield return new WaitForSeconds(loadDelay);

        // 3. Laad de scene pas nu
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Ongeldige Scene Index: {buildIndex}. Controleer je Build Settings.");
            yield break; // Stop de Coroutine
        }

        SceneManager.LoadScene(buildIndex);
    }
}