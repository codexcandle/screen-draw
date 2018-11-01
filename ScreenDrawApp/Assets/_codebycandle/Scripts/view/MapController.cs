using UnityEngine;
using UnityEngine.UI;

public class MapController:MonoBehaviour
{
    [SerializeField] private GameObject pipeSegmentPrefab;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform itemRoot;
    [SerializeField] private Text mapText;
    [SerializeField] private int maxItemCount = 10;

    private Camera cam;
    private Vector3 lastPos;
    private int itemCount;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.tag == "Ground")
                {
                    if (itemCount < maxItemCount)
                    {
                        UpdateLine(itemCount, hitInfo.point);

                        AddMapPoint(hitInfo.point);
                    }
                }
            }
        }
    }

    private void Init()
    {
        cam = Camera.main;

        InitLine();

        RefreshMapText();
    }

    private void InitLine()
    {
        line.gameObject.SetActive(false);
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.widthMultiplier = 0.2f;
        line.positionCount = 20;
        line.loop = false;

        float alpha = 1.0f;
        Color c1 = Color.yellow;
        Color c2 = Color.red;
        GradientColorKey[] colorKeys = { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) };
        GradientAlphaKey[] alphaKeys = { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) };
        Gradient g = new Gradient();
        g.SetKeys(colorKeys, alphaKeys);
        line.colorGradient = g;
    }

    private void AddMapPoint(Vector3 pos)
    {
        GameObject go = Instantiate(pipeSegmentPrefab, pos, transform.rotation) as GameObject;

        go.GetComponent<Renderer>().material.color = AppModel.materials[AppModel.activeMaterialIndex].color;

        go.transform.SetParent(itemRoot);

        itemCount++;

        RefreshMapText();
    }

    private void UpdateLine(int pointIndex, Vector3 dest)
    {
        if(!line.gameObject.activeInHierarchy)
        {
            line.gameObject.SetActive(true);
        }
        
        line.SetPosition(itemCount, dest);

        line.loop = false;
    }

    private void RefreshMapText()
    {
        mapText.text = "item count: " + itemCount + " / " + maxItemCount;
    }
}
