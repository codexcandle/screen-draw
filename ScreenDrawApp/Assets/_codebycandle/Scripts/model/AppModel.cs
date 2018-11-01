using UnityEngine;

public class AppModel
{
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

    static private MaterialType[] GetStubData()
    {
        MaterialType[] data = new MaterialType[4];
        data[0] = new MaterialType("1", new Color32(217, 241, 240, 255));
        data[1] = new MaterialType("2", new Color32(160, 160, 160, 255));
        data[2] = new MaterialType("3", new Color32(255, 255, 1, 255));
        data[3] = new MaterialType("4", new Color32(195, 134, 0, 255));

        return data;
    }
}