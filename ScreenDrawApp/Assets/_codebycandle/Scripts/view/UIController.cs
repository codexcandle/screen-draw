using UnityEngine;
using UnityEngine.UI;

namespace Codebycandle.ScreenDrawApp
{
    public class UIController:MonoBehaviour
    {
        public delegate void OnStageRevealedDelegate();
        public static event OnStageRevealedDelegate OnStageRevealed;

        [SerializeField] private CanvasGroup stageBlocker;
        [SerializeField] private GameObject mapPointDistanceLabelGO;
        [SerializeField] private GameObject camInstructionsLabelGO;
        [SerializeField] private Text mapPointDistanceText;
        [SerializeField] private Text mapMaterialCountText;
        [SerializeField] private TMPro.TextMeshProUGUI promptText;

        public void Reset()
        {
            EnableMapPointDistanceLabelGO(false);
            EnableCamInstructionsLabelGO(false);
            SetPromptText("");
            SetMaterialCount(-1);
        }

        public void RevealStage()
        {
            StartCoroutine(FadeEffect.FadeCanvas(stageBlocker, 1f, 0f, 1f, HandleBlockerFadeOutComplete));
        }

        public void SetPromptText(string txt)
        {
            promptText.text = txt;
        }

        public void SetMapPointDistance(float distance)
        {
            mapPointDistanceText.text = distance.ToString("f2");

            EnableMapPointDistanceLabelGO(true);
        }

        public void SetMaterialCount(int count)
        {
            mapMaterialCountText.text = (count < 1) ? "" : "path count: " + count.ToString();
        }

        private void HandleBlockerFadeOutComplete()
        {
            stageBlocker.blocksRaycasts = false;

            EnableCamInstructionsLabelGO(true);

            if (OnStageRevealed != null) OnStageRevealed();
        }

        private void EnableMapPointDistanceLabelGO(bool value)
        {
            mapPointDistanceLabelGO.SetActive(value);
        }

        private void EnableCamInstructionsLabelGO(bool value)
        {
            camInstructionsLabelGO.SetActive(value);
        }
    }
}