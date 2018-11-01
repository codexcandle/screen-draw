using UnityEngine;
using UnityEngine.UI;

public class PanelButton:MonoBehaviour
{
    public delegate void OnClickDelegate(int viewIndex);
    public event OnClickDelegate OnClick;

    private Image image;
    private Button btn;

    private bool isReady;

    public int viewIndex
    {
        get;
        set;
    }

    public string labelText
    {
        set
        {
            if(!tf) tf = GetComponentInChildren<Text>();

            tf.text = value;
        }
    }

    private bool _selected;
    public bool selected
    {
        set
        {
            // sanitize!
            if (!isReady) return;

            // Image.Type imageType = value ? Image.Type.Filled: Image.Type.Sliced;

            // image.type = imageType;

            btn.interactable = !value;

            _selected = value;
        }
    }

    private Text tf;

    public void Init(int viewIndex, string labelText)
    {
        this.viewIndex = viewIndex;
        this.labelText = labelText;

        btn = GetButton();

        image = GetComponent<Image>();

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
}