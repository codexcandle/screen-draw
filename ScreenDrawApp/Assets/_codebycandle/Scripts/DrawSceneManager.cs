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
                        ui.Reset();

                        break;
                    case AppModel.stateOption.ready:
                        SetPromptText("Please select material.");

                        break;
                    case AppModel.stateOption.placing:
                        SetPromptText("Click map to begin drawing material path.");

                        camRig.keyActive = true;

                        break;
                    case AppModel.stateOption.drawingStarted:
                        SetPromptText("(-minimum of 2 points required.)");

                        break;
                    case AppModel.stateOption.drawing:
                        SetPromptText("Extend path. (or hit ESC to save)");

                        break;
                    case AppModel.stateOption.placed:
                        SetPromptText("-Path saved!");

                        // TODO - implement ACTUAL SERIALIZATION for saving data!
                        // - just saves to map for now...

                        ui.SetMaterialCount(map.pathCount);

                        break;
                    case AppModel.stateOption.readyNext:
                        SetPromptText("Draw next path, or select new material.");

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
            MapController.OnPathSegmentDistanceUpdate += HandleMapSegmentDistanceUpdate;
            MapController.OnMaterialPathMinPointsReached += HandleMapPathMinPointsReached;

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
            SetViewState(AppModel.stateOption.drawingStarted);
        }

        private void HandleMapPathAddComplete()
        {
            StartCoroutine(ShowMaterialPathAdded());
        }

        private void HandleMapSegmentDistanceUpdate(float distance)
        {
            ui.SetMapPointDistance(distance);
        }
   
        private void HandleMapPathMinPointsReached()
        {
            SetViewState(AppModel.stateOption.drawing);
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
            MapController.OnPathSegmentDistanceUpdate -= HandleMapSegmentDistanceUpdate;
            MapController.OnMaterialPathMinPointsReached -= HandleMapPathMinPointsReached;
        }
    }
}