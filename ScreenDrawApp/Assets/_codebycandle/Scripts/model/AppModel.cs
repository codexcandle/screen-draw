using UnityEngine;

namespace Codebycandle.ScreenDrawApp
{
    public class AppModel:MonoBehaviour
    {
        private static AppModel instance;

        public enum stateOption
        {
            invalid,
            preinit,
            ready,
            placing,
            drawing,
            placed
        };

        public stateOption curState
        {
            get;
            set;
        }

        static private MaterialType[] _materials;
        static public MaterialType[] materials
        {
            get
            {
                if (_materials == null)
                {
                    _materials = GetStubData();
                }

                return _materials;
            }
        }

        static public int activeMaterialIndex
        {
            get;
            set;
        }

        static public AppModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("AppModel").AddComponent<AppModel>();
                }

                return instance;
            }
        }

        public void OnApplicationQuit()
        {
            instance = null;
        }

        static private MaterialType[] GetStubData()
        {
            MaterialType[] data = new MaterialType[4];
            data[0] = new MaterialType("steel", new Color32(70, 166, 204, 255));
            data[1] = new MaterialType("copper", new Color32(255, 167, 48, 255));
            data[2] = new MaterialType("ceramic", new Color32(204, 70, 97, 255));
            data[3] = new MaterialType("plastic", new Color32(151, 204, 32, 255));

            return data;
        }
    }
}