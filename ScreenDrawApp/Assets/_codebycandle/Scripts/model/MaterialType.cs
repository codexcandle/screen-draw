using System;
using UnityEngine;

public class MaterialType:MonoBehaviour
{
    public Color32 color
    {
        get;
        private set;
    }

    public string materialName
    {
        get;
        private set;
    }

    public MaterialType(string materialName, Color32 color)
    {
        this.materialName = materialName;
        this.color = color;
    }
}