using UnityEngine;
using UnityEngine.UI;

namespace Codebycandle.ScreenDrawApp
{
    public class MaterialPanelController:MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonRoot;

        public delegate void OnClickDelegate(int index);
        public static event OnClickDelegate OnClick;

        private int buttonCount;
        private int activeIndex;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            InitButtons();
        }

        private void InitButtons()
        {
            var types = AppModel.materials;

            for (int i = 0; i < types.Length; i++)
            {
                MaterialType type = types[i];

                var go = Instantiate(buttonPrefab, transform.position, transform.rotation) as GameObject;

                var panelBtn = go.GetComponent<PanelButton>();
                panelBtn.Init(i, type.materialName);
                panelBtn.OnClick += HandleButtonClick;

                go.transform.SetParent(buttonRoot);

                go.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
                go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                go.GetComponent<Image>().color = type.color;
            }

            this.buttonCount = types.Length;
        }

        private void HandleButtonClick(int index)
        {
            if (OnClick != null)
            {
                OnClick(index);
            }

            SetActiveButton(index);
        }

        private void SetActiveButton(int index)
        {
            // sanitize!
            if (index < 0 || index >= buttonCount) return;

            // deselect old
            if (activeIndex > -1)
            {
                var go = buttonRoot.GetChild(activeIndex).gameObject;
                go.GetComponent<PanelButton>().selected = false;
            }

            // mark new
            var go2 = buttonRoot.GetChild(index).gameObject;
            go2.GetComponent<PanelButton>().selected = true;

            // update model
            this.activeIndex = index;
        }
    }
}