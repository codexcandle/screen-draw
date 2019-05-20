using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Codebycandle.ScreenDrawApp
{
    [RequireComponent(typeof(ObjectPooler))]
    public class MapController:MonoBehaviour
    {
        public delegate void OnMaterialEventDelegate();
        public static OnMaterialEventDelegate OnMaterialPathAddStart;
        public static OnMaterialEventDelegate OnMaterialPathMinPointsReached;
        public static OnMaterialEventDelegate OnMaterialPathAddComplete;

        public delegate void OnPathSegmentDistanceUpdateDelegate(float distance);
        public static event OnPathSegmentDistanceUpdateDelegate OnPathSegmentDistanceUpdate;

        private int _pathCount;
        public int pathCount
        {
            get
            {
                return _pathCount;
            }
            private set
            {
                _pathCount = value;
            }
        }

        [SerializeField] private GameObject pipeSegmentPrefab;
        [SerializeField] private Transform pointRoot;
        [SerializeField] private Transform lineRoot;
        [SerializeField] private Text mapItemCountText;

        private Camera cam;
        private Vector3 lastPos;
        private int pathPointMin = 2;
        private int curPathPointCount;
        private float pathDistanceSaved;

        private bool isPlacing;
        private bool awaitingDrawMovement;
        private bool pathReadyForSubmission;

        // TODO - deprecate these with "finished 3D models"
        private ObjectPooler lineObjecPool;
        private List<GameObject> pointGOList;
        private List<LineRenderer> activeLines;

        void Start()
        {
            cam = Camera.main;

            // init pool
            lineObjecPool = GetComponent<ObjectPooler>();
            lineObjecPool.Init(lineRoot);
        }

        void FixedUpdate()
        {
            bool clicked = false;
            if (Input.GetMouseButtonDown(0))
            {
                // ignore ui elements
                if (EventSystem.current.IsPointerOverGameObject()) return;

                clicked = true;
            }

            // sanitize
            // TODO - confirm if needed?
            if (!clicked && !isPlacing) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.tag == "Ground")
                {
                    if (clicked)
                    {
                        if (isPlacing)
                        {
                            AddMidPoint(hitInfo.point);
                        }
                        else
                        {
                            AddStartPoint(hitInfo.point);
                        }
                    }
                    else if (isPlacing)
                    {
                        if (awaitingDrawMovement)
                        {
                            if ((Input.GetAxis("Mouse X") != 0) || 
                                (Input.GetAxis("Mouse Y") != 0))
                            {
                                awaitingDrawMovement = false;

                                // if => not first point
                                if(curPathPointCount > 1)
                                {
                                    /*
                                    update line-pos count ONLY after
                                    user begins drawing!
                                    */
                                    GetActiveLine().positionCount++;
                                }
                            }
                        }
                        else
                        {
                            DrawTempLine(hitInfo.point);

                            // listen for "trim" command
                            // + HACK to ensure valid shape count
                            if (curPathPointCount < 2) return;
                            if (Input.GetKeyUp(KeyCode.Escape))
                            {
                                SaveMaterialPath();
                            }
                        }
                    }
                }
            }
        }

        #region METHODS-PRIVATE-POINTS
        private void AddStartPoint(Vector3 pos)
        {
            AddMapPoint(pos);

            isPlacing = true;
            awaitingDrawMovement = true;

            // dispatch event
            if (OnMaterialPathAddStart != null) OnMaterialPathAddStart();

            ////////////////////////////////////////////////
            LineRenderer rend = GetActiveLine();
            rend.SetPosition(0, pos);
            rend.positionCount++;
            ////////////////////////////////////////////////
            
            // ensure value reset
            pathDistanceSaved = 0F;
        }

        private void AddMidPoint(Vector3 pos)
        {
            AddMapPoint(pos);

            awaitingDrawMovement = true;

            // update "saved distance" 
            pathDistanceSaved += Vector3.Distance(pointGOList[pointGOList.Count - 2].transform.position, pos);

            ////////////////////////////////////////////////
            LineRenderer rend = GetActiveLine();
            int pointCount = pointGOList.Count;
            rend.SetPosition(curPathPointCount - 1, pos);
            // rend.positionCount++;
            ////////////////////////////////////////////////

            // dispatch event that "valid" minimum path-point count reached
            if(curPathPointCount >= pathPointMin)
            {
                OnMaterialPathMinPointsReached();
            }
        }

        private void AddMapPoint(Vector3 pos)
        {
            GameObject go = Instantiate(pipeSegmentPrefab, pos, transform.rotation) as GameObject;
            go.GetComponent<Renderer>().material.color = GetActiveMaterialColor();
            go.transform.SetParent(pointRoot);

            if (pointGOList == null) pointGOList = new List<GameObject>();
            pointGOList.Add(go);

            // update count
            curPathPointCount++;
        }
        #endregion

        #region METHODS-PRIVATE-LINES
        private void DrawTempLine(Vector3 destPos)
        {
            LineRenderer line = GetActiveLine();
            if (!line.gameObject.activeInHierarchy)
            {
                line.gameObject.SetActive(true);
            }

            line.SetPosition(line.positionCount - 1, destPos);

            // update path distance info
            float curSegmentDistance = Vector3.Distance(pointGOList[pointGOList.Count - 1].transform.position, destPos);
            
            // fire event
            OnPathSegmentDistanceUpdate(curSegmentDistance + pathDistanceSaved);
        }

        private LineRenderer GetActiveLine()
        {
            if(activeLines == null)
            {
                activeLines = new List<LineRenderer>();
            }

            // make NEW line object if needed
            int activeCount = activeLines.Count;
            if(_pathCount == activeCount)
            {
                activeLines.Add(GetNewLineObject());
            }

            return activeLines[_pathCount];
        }

        private LineRenderer GetNewLineObject()
        {
            var go = lineObjecPool.GetPooledObject();
            if (!go) return null;

            LineRenderer line = go.GetComponent<LineRenderer>();
            if (!line) return null;

            InitLineRenderer(line);

            return line;
        }

        private void InitLineRenderer(LineRenderer line)
        {
            line.gameObject.SetActive(false);
            line.widthMultiplier = 0.2f;
            line.positionCount = 1;

            float alpha = 1.0f;
            Color c1 = GetActiveMaterialColor();
            Color c2 = GetActiveMaterialColor();
            GradientColorKey[] colorKeys = { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) };
            GradientAlphaKey[] alphaKeys = { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) };
            Gradient g = new Gradient();
            g.SetKeys(colorKeys, alphaKeys);
            line.colorGradient = g;
            line.loop = false;
        }

        private void SaveMaterialPath()
        {
            TrimLine();

            // TODO - hide for now, will recycle.
            // pointRoot.gameObject.SetActive(false);

            _pathCount++;

            curPathPointCount = 0;

            // dispatch event
            if (OnMaterialPathAddComplete != null) OnMaterialPathAddComplete();
        }

        private void TrimLine()
        {
            isPlacing = false;

            GetActiveLine().positionCount--;

            pathReadyForSubmission = true;
        }
        #endregion

        private Color GetActiveMaterialColor()
        {
            return AppModel.materials[AppModel.activeMaterialIndex].color;
        }
    }
}