using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseScreenManager : MonoBehaviour
{
    [SerializeField] private SettingsObject settings;
    [SerializeField] private GameObject pauseScreen;
    private void Update()
    {
        if (settings.IsPaused)
        {
            pauseScreen.SetActive(true);
        }
        else
        {
            pauseScreen.SetActive(false);
        }
    }

    public void DisableAndEnablePause()
    {
        GameManager.Instance.Pause();
    }

    public void ReturnToStart()
    {
        settings.IsPaused = false;
        SceneManager.LoadScene("start");
    }
}
