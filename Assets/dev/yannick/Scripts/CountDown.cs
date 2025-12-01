using System.Collections;
using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    private int timer = 5;
    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] ShowControls showControlsScript;

    bool counterDisabled = false;
    private void Start()
    {
        StartCoroutine(CountDownTimer());
    }

    private void Update()
    {
        if(timer == 0) //when timer == 0 stop timer
        {
            if (!counterDisabled)
            {
                timerText.text = " ";

                showControlsScript.DisableAll();
                
                GameManager.Instance.StartGame();
                counterDisabled = true;
            }
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
}
