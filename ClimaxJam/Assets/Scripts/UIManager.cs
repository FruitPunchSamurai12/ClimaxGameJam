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

    float timer = 0;

    private void Start()
    {
        GameManager.Instance.onChainLinkPick += HandleLinkCollected;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onChainLinkPick -= HandleLinkCollected;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);
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
}
