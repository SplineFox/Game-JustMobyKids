using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TowerSave : ISaveObject
{
    public List<TowerElementData> ElementsData = new();
}

[Serializable]
public class TowerElementData
{
    public string ConfigurationId;
    public Vector2 AnchoredPosition;
}