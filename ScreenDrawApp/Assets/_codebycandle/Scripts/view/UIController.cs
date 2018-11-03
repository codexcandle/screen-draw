using UnityEngine;
using UnityEngine.UI;

public class UIController:MonoBehaviour
{
    public delegate void OnStageRevealedDelegate();
    public static event OnStageRevealedDelegate OnStageRevealed;

    [SerializeField] private CanvasGroup stageBlocker;
    [SerializeField] private Text mapPointDistanceText;
    [SerializeField] private TMPro.TextMeshProUGUI promptText;

    public void Reset()
    {
        SetMapPointDistanceText();
    }

    public void RevealStage()
    {
        StartCoroutine(FadeEffect.FadeCanvas(stageBlocker, 1f, 0f, 1f, HandleBlockerFadeOutComplete));
    }

    public void SetPromptText(string txt = "")
    {
        promptText.text = txt;
    }

    public void SetMapPointDistanceText(string txt = "")
    {
        mapPointDistanceText.text = txt;
    }

    private void HandleBlockerFadeOutComplete()
    {
        stageBlocker.blocksRaycasts = false;

        if(OnStageRevealed != null) OnStageRevealed();
    }
}