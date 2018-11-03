using UnityEngine;

namespace Codebycandle.ScreenDrawApp
{
    public class CamControlPanelController:MonoBehaviour
    {
        public delegate void OnToggleModeDelegate(bool active);
        public static event OnToggleModeDelegate OnToggleMode;

        public delegate void OnNavButtonDelegate();
        public static event OnNavButtonDelegate OnNavButtonUp;
        public static event OnNavButtonDelegate OnNavButtonDown;
        public static event OnNavButtonDelegate OnNavButtonLeft;
        public static event OnNavButtonDelegate OnNavButtonRight;

        public void OnToggleModeButton(bool value)
        {
            OnToggleMode(value);
        }

        public void OnUpButtonClick()
        {
            OnNavButtonUp();
        }

        public void OnDownButtonClick()
        {
            OnNavButtonDown();
        }

        public void OnLeftButtonClick()
        {
            OnNavButtonLeft();
        }

        public void OnRightButtonClick()
        {
            OnNavButtonRight();
        }
    }
}