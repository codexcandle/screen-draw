using UnityEngine;

public class DrawSceneManager:MonoBehaviour
{
    [SerializeField] private MaterialPanelController panel;
    [SerializeField] private MapController pipeMap;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        panel.OnClick += HandlePanelClick;
    }

    private void HandlePanelClick(int index)
    {
        Debug.Log("handlePanelClick @ DrawSceneManager: " + index);

        AppModel.activeMaterialIndex = index;
    }
}