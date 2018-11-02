using UnityEngine;

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
            if(_materials == null)
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
        data[0] = new MaterialType("fiber", new Color32(70, 166, 204, 105));
        data[1] = new MaterialType("copper", new Color32(255, 167, 48, 105));
        data[2] = new MaterialType("brick", new Color32(204, 70, 97, 105));
        data[3] = new MaterialType("plastic", new Color32(151, 204, 32, 105));

        return data;
    }
}