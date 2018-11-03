using UnityEngine;
using UnityEngine.UI;

namespace Codebycandle.ScreenDrawApp
{
    public class PanelButton:MonoBehaviour
    {
        public delegate void OnClickDelegate(int viewIndex);
        public event OnClickDelegate OnClick;

        private Image image;
        private Button btn;

        private bool isReady;
        private Color bgColor;
        private Color textColor;

        public int viewIndex
        {
            get;
            set;
        }

        private Text tf;
        public string labelText
        {
            set
            {
                tf.text = value;
            }
        }

        private bool _selected;
        public bool selected
        {
            get
            {
                return _selected;
            }
            set
            {
                // sanitize!
                if (!isReady) return;

                btn.interactable = !value;

                _selected = value;
            }
        }

        public bool dimmed
        {
            set
            {
                SetImageColor(value ? Color.black : bgColor);
                tf.color = value ? Color.white : textColor;
            }
        }

        public void Init(int viewIndex, string labelText, Color bgColor, Color textColor)
        {
            btn = GetButton();

            image = GetComponent<Image>();
            image.color = bgColor;

            tf = GetComponentInChildren<Text>();
            tf.color = textColor;

            this.viewIndex = viewIndex;
            this.labelText = labelText;
            this.bgColor = bgColor;
            this.textColor = textColor;

            isReady = true;
        }

        private Button GetButton()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(() => HandleClick());

            return btn;
        }

        private void HandleClick()
        {
            OnClick(viewIndex);
        }

        private void SetImageColor(Color c)
        {
            image.color = c;
        }
    }
}