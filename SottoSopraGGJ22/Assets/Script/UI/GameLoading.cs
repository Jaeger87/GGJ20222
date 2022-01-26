using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameLoading : MonoBehaviour
{
    public Action CountdownEnded;
    
    [SerializeField]
    private Text CountdownLabel = null;

    [SerializeField]
    private int CountdownAmount = 3;

    public void StartCountdown()
    {
        StartCoroutine(CountDownStart());
    }

    private IEnumerator CountDownStart()
    {
        int currentCountdown = CountdownAmount;
        
        CountdownLabel.text = currentCountdown.ToString();
        
        while (--currentCountdown >= 0)
        {
            yield return new WaitForSeconds(1f);
            CountdownLabel.text = currentCountdown.ToString();
        }
        
        CountdownEnded.Invoke();
    }
}
