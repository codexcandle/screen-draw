using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Codebycandle.ScreenDrawApp
{
    [RequireComponent(typeof(ObjectPooler))]
    public class MapController:MonoBehaviour
    {
        public delegate void OnItemEventDelegate();
        public static OnItemEventDelegate OnItemAddStart;
        public static OnItemEventDelegate OnItemAddComplete;

        [SerializeField] private GameObject pipeSegmentPrefab;
        [SerializeField] private Transform itemRoot;
        [SerializeField] private Text mapItemCountText;
        [SerializeField] private int maxItemCount = 10;

        private Camera cam;
        private Vector3 lastPos;
        private int itemCount;

        private bool startAdded;
        private bool isPlacing;

        private List<GameObject> lines;

        void Start()
        {
            Init();
        }

        void FixedUpdate()
        {
            bool clicked = false;
            if (Input.GetMouseButtonDown(0))
            {
                // sanitize! (ignore if clicking on ui-element)
                if (EventSystem.current.IsPointerOverGameObject()) return;

                clicked = true;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.tag == "Ground")
                {
                    if (clicked)
                    {
                        if (itemCount < maxItemCount)
                        {
                            if (isPlacing)
                            {
                                EndPlacement(hitInfo.point);
                            }
                            else
                            {
                                BeginPlacement(hitInfo.point);
                            }
                        }
                    }
                    else if (startAdded)
                    {
                        GameObject go = lines[lines.Count - 1] as GameObject;
                        EndLine(go.GetComponent<LineRenderer>(), hitInfo.point);
                    }
                }
            }
        }

        private void Init()
        {
            cam = Camera.main;

            RefreshMapText(true);

            // init pool
            ObjectPooler.current.Init();
        }

        private void BeginPlacement(Vector3 startPos)
        {
            isPlacing = true;

            AddMapPoint(startPos);

            var go = GetNewLineObject();
            if (!go) return;

            if (lines == null) lines = new List<GameObject>();
            lines.Add(go);

            BeginLine(go.GetComponent<LineRenderer>(), startPos);

            startAdded = true;

            // dispatch event
            if (OnItemAddStart != null) OnItemAddStart();
        }

        private void BeginLine(LineRenderer line, Vector3 startPos)
        {
            if (!line.gameObject.activeInHierarchy)
            {
                line.gameObject.SetActive(true);
            }

            line.SetPosition(0, startPos);
        }

        private GameObject GetNewLineObject()
        {
            var go = ObjectPooler.current.GetPooledObject();
            if (!go) return null;

            LineRenderer line = go.GetComponent<LineRenderer>();
            if (!line) return null;

            InitLineRenderer(go.GetComponent<LineRenderer>());

            return go;
        }

        private void InitLineRenderer(LineRenderer line)
        {
            line.gameObject.SetActive(false);
            line.material = new Material(Shader.Find("Particles/Additive"/*"Sprites/Default"*/));
            line.widthMultiplier = 0.2f;
            line.positionCount = 2;

            float alpha = 1.0f;
            Color c1 = GetActiveMaterialColor();
            Color c2 = GetActiveMaterialColor();
            GradientColorKey[] colorKeys = { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f)};
            GradientAlphaKey[] alphaKeys = { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f)};
            Gradient g = new Gradient();
            g.SetKeys(colorKeys, alphaKeys);
            line.colorGradient = g;
            line.loop = false;
        }

        private void EndPlacement(Vector3 destPos)
        {
            isPlacing = false;

            AddMapPoint(destPos);

            GameObject go = lines[lines.Count - 1] as GameObject;
            EndLine(go.GetComponent<LineRenderer>(), destPos);

            // dispatch event
            if (OnItemAddComplete != null) OnItemAddComplete();
        }

        private void EndLine(LineRenderer line, Vector3 endPos)
        {
            line.SetPosition(1, endPos);
        }

        private void AddMapPoint(Vector3 pos)
        {
            GameObject go = Instantiate(pipeSegmentPrefab, pos, transform.rotation) as GameObject;

            go.GetComponent<Renderer>().material.color = GetActiveMaterialColor();

            go.transform.SetParent(itemRoot);

            itemCount++;

            RefreshMapText();

            if (startAdded)
            {
                isPlacing = false;
                startAdded = false;
            }
        }

        private void RefreshMapText(bool reset = false)
        {
            string newTxt = reset ? "" : ("item count: " + itemCount + " / " + maxItemCount);
            mapItemCountText.text = newTxt;
        }

        private Color GetActiveMaterialColor()
        {
            return AppModel.materials[AppModel.activeMaterialIndex].color;
        }
    }
}