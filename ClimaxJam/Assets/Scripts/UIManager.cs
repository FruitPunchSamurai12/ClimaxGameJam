using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    int linksCollected = 0;
    [SerializeField] Image chainImage;
    [SerializeField] Sprite[] chainSprites;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI victoryTimerText;
    [SerializeField] CanvasGroup victoryPanel;
    [SerializeField] AnimationCurve showCurve;
    float speedrunTimer = 0;
    float timer = 0;
    [SerializeField] float showTime = 1;

    private void Start()
    {
        GameManager.Instance.onChainLinkPick += HandleLinkCollected;
        GameManager.Instance.onVictory += HandleVictory;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onChainLinkPick -= HandleLinkCollected;
        GameManager.Instance.onVictory -= HandleVictory;
    }

    private void Update()
    {
        speedrunTimer += Time.deltaTime;
        float minutes = Mathf.FloorToInt(speedrunTimer / 60);
        float seconds = Mathf.FloorToInt(speedrunTimer % 60);
        string ifLessThan10Seconds = seconds < 10 ? "0" : "";
        timerText.SetText($"{minutes}:{ifLessThan10Seconds}{seconds}");
    }

    void HandleLinkCollected()
    {
        linksCollected++;
        if(linksCollected-1<chainSprites.Length)
        {
            chainImage.sprite = chainSprites[linksCollected-1];
        }
    }

    void HandleVictory()
    {
        float minutes = Mathf.FloorToInt(speedrunTimer / 60);
        float seconds = Mathf.FloorToInt(speedrunTimer % 60);
        string ifLessThan10Seconds = seconds < 10 ? "0" : "";
        victoryTimerText.SetText($"TIME\n{minutes}:{ifLessThan10Seconds}{seconds}");
        StartCoroutine(ShowVictoryScreen());
    }

    IEnumerator ShowVictoryScreen()
    {
        while (timer < showTime)
        {
            timer += Time.unscaledDeltaTime;
            float percentage = timer / showTime;
            victoryPanel.alpha = showCurve.Evaluate(percentage);
            yield return null;
        }
        victoryPanel.alpha = 1f;
        victoryPanel.blocksRaycasts = true;
        victoryPanel.interactable = true;
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
