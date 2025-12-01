using System.Collections;
using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    private int timer = 5;
    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] ShowControls showControlsScript;

    [SerializeField] GameObject spawners;

    bool counterDisabled = false;
    private void Start()
    {
        StartCoroutine(CountDownTimer());
        spawners.SetActive(false);
    }

    private void Update()
    {
        if(timer == 0) //when timer == 0 stop timer
        {
            if(!counterDisabled)
            StartCoroutine(DisableText());
        }
        else      //update timer 
        {
            timerText.text = timer.ToString();
        }
    }
    private IEnumerator CountDownTimer()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
        }
    }

    private IEnumerator DisableText()
    {
        timerText.text = "GO!";

        showControlsScript.DisableAll();

        GameManager.Instance.StartGame();
        spawners.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        timerText.text = " ";
        counterDisabled = true;
    }
}
