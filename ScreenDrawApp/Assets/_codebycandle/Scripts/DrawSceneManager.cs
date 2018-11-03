using UnityEngine;
using System.Collections;

namespace Codebycandle.ScreenDrawApp
{
    public class DrawSceneManager:MonoBehaviour
    {
        [SerializeField] private MapController map;
        [SerializeField] private CameraRigController camRig;
        [SerializeField] private UIController ui;
        // TODO - move below to ui-controller
        [SerializeField] private MaterialPanelController materialPanel;

        private AppModel model;

        private void SetViewState(AppModel.stateOption newState)
        {
            if (newState != model.curState)
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
                        SetPromptText("Plot on map.");

                        camRig.keyActive = true;

                        break;
                    case AppModel.stateOption.drawing:
                        SetPromptText("Draw material path.");

                        break;
                    case AppModel.stateOption.placed:
                        SetPromptText("-path confirmed!");

                        break;
                    case AppModel.stateOption.readyNext:
                        SetPromptText("Add next material path.");

                        break;
                }

                model.curState = newState;
            }
        }

        void Start()
        {
            Init();
        }

        void OnApplicationQuit()
        {
            Destroy();
        }

        private void Init()
        {
            model = AppModel.Instance;

            UIController.OnStageRevealed += HandleUIStageRevealed;

            MaterialPanelController.OnClick += HandleMaterialPanelClick;

            MapController.OnMaterialPathAddStart += HandleMapPathAddStart;
            MapController.OnMaterialPathAddComplete += HandleMapPathAddComplete;

            StartCoroutine(StartScene());
        }

        private IEnumerator StartScene()
        {
            SetViewState(AppModel.stateOption.preinit);

            yield return new WaitForSeconds(1);

            SetViewState(AppModel.stateOption.ready);

            yield return new WaitForSeconds(2);
        }

        private IEnumerator ShowMaterialPathAdded()
        {
            SetViewState(AppModel.stateOption.placed);

            yield return new WaitForSeconds(2);

            SetViewState(AppModel.stateOption.readyNext);
        }

        private void HandleUIStageRevealed()
        {
            SetViewState(AppModel.stateOption.placing);
        }

        private void HandleMaterialPanelClick(int index)
        {
            AppModel.activeMaterialIndex = index;

            if (model.curState == AppModel.stateOption.ready)
            {
                SetPromptText("");

                ui.RevealStage();
            }
        }

        private void HandleMapPathAddStart()
        {
            SetViewState(AppModel.stateOption.drawing);
        }

        private void HandleMapPathAddComplete()
        {
            StartCoroutine(ShowMaterialPathAdded());
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
            ui.SetPromptText(txt);
        }

        private void Destroy()
        {
            UIController.OnStageRevealed -= HandleUIStageRevealed;

            MaterialPanelController.OnClick -= HandleMaterialPanelClick;

            MapController.OnMaterialPathAddStart -= HandleMapPathAddStart;
            MapController.OnMaterialPathAddComplete -= HandleMapPathAddComplete;
        }
    }
}