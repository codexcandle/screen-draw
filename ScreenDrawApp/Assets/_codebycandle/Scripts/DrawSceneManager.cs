using UnityEngine;
using System.Collections;

namespace Codebycandle.ScreenDrawApp
{
    public class DrawSceneManager:MonoBehaviour
    {
        [SerializeField] private MaterialPanelController panel;
        [SerializeField] private MapController map;
        [SerializeField] private CameraRigController camRig;
        [SerializeField] private TMPro.TextMeshProUGUI promptText;
        [SerializeField] private GameObject stageBlocker;

        private bool stageActive;

        private AppModel model;

        private void SetViewState(AppModel.stateOption newState)
        {
            if (newState != AppModel.Instance.curState)
            {
                int index = (int)newState;

                switch (newState)
                {
                    case AppModel.stateOption.preinit:
                        SetPromptText("");

                        break;
                    case AppModel.stateOption.ready:
                        SetPromptText("Select material.");

                        break;
                    case AppModel.stateOption.placing:
                        camRig.Reset();

                        if (stageBlocker.activeInHierarchy) stageBlocker.SetActive(false);

                        SetPromptText("Place on map.");

                        stageActive = true;

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

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            model = AppModel.Instance;

            MaterialPanelController.OnClick += HandlePanelClick;

            MapController.OnItemAddStart += HandleMapItemAddStart;
            MapController.OnItemAddComplete += HandleMapItemAddComplete;

            StartCoroutine(StartScene());
        }

        private IEnumerator StartScene()
        {
            // pre-init
            SetViewState(AppModel.stateOption.preinit);

            yield return new WaitForSeconds(1);

            // ready
            SetViewState(AppModel.stateOption.ready);

            yield return new WaitForSeconds(2);
        }

        private void HandlePanelClick(int index)
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

        private void SetPromptText(string txt)
        {
            promptText.text = txt;
        }
    }
}