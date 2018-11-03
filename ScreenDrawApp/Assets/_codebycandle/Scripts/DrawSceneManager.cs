using UnityEngine;
using System.Collections;

namespace Codebycandle.ScreenDrawApp
{
    public class DrawSceneManager:MonoBehaviour
    {
        [SerializeField] private MapController map;
        [SerializeField] private CameraRigController camRig;
        // TODO - add ui-manager to decouple below refs
        [SerializeField] private MaterialPanelController materialPanel;
        [SerializeField] private CamControlPanelController camPanel;
        [SerializeField] private TMPro.TextMeshProUGUI promptText;
        [SerializeField] private CanvasGroup stageBlocker;

        private AppModel model;
        private bool stageActive;

        private void SetViewState(AppModel.stateOption newState)
        {
            if (newState != AppModel.Instance.curState)
            {
                switch (newState)
                {
                    case AppModel.stateOption.preinit:
                        SetPromptText("");

                        break;
                    case AppModel.stateOption.ready:
                        SetPromptText("Select material.");

                        break;
                    case AppModel.stateOption.placing:
                        SetPromptText("");

                        StartCoroutine(FadeEffect.FadeCanvas(stageBlocker, 1f, 0f, 1f, HandleBlockerFadeOutComplete));

                        break;
                    case AppModel.stateOption.drawing:
                        SetPromptText("Draw path.");

                        break;
                    case AppModel.stateOption.placed:
                        SetPromptText("Add next path.");

                        break;
                }

                model.curState = newState;
            }
        }

        private void HandleBlockerFadeOutComplete()
        {
            stageActive = true;

            camRig.keyActive = true;

            stageBlocker.blocksRaycasts = false;

            SetPromptText("Plot on map.");
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            model = AppModel.Instance;

            MaterialPanelController.OnClick += HandleMaterialPanelClick;

            MapController.OnItemAddStart += HandleMapItemAddStart;
            MapController.OnItemAddComplete += HandleMapItemAddComplete;

            CamControlPanelController.OnToggleMode += HandleCamControlModeToggle;
            CamControlPanelController.OnNavButtonUp += HandleCamControlNavUp;
            CamControlPanelController.OnNavButtonDown += HandleCamControlNavDown;
            CamControlPanelController.OnNavButtonLeft += HandleCamControlNavLeft;
            CamControlPanelController.OnNavButtonRight += HandleCamControlNavRight;

            StartCoroutine(StartScene());
        }

        private IEnumerator StartScene()
        {
            SetViewState(AppModel.stateOption.preinit);

            yield return new WaitForSeconds(1);

            SetViewState(AppModel.stateOption.ready);

            yield return new WaitForSeconds(2);
        }

        private void HandleMaterialPanelClick(int index)
        {
            AppModel.activeMaterialIndex = index;

            if (!stageActive)
            {
                SetViewState(AppModel.stateOption.placing);
            }
        }

        private void HandleMapItemAddStart()
        {
            SetViewState(AppModel.stateOption.drawing);
        }

        private void HandleMapItemAddComplete()
        {
            SetViewState(AppModel.stateOption.placed);
        }

        private void HandleCamControlModeToggle(bool value)
        {
            camRig.SetCamProjectionMode(value);
        }

        private void HandleCamControlNavUp()
        {
            camRig.RotateVertical(true);
        }

        private void HandleCamControlNavDown()
        {
            camRig.RotateVertical(false);
        }

        private void HandleCamControlNavLeft()
        {
            camRig.RotateHorizontal(true);
        }

        private void HandleCamControlNavRight()
        {
            camRig.RotateHorizontal(false);
        }

        private void SetPromptText(string txt)
        {
            promptText.text = txt;
        }
    }
}