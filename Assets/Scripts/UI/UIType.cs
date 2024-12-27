using System.Collections;
using UnityEngine;

/// <summary>
/// 储存单个UI的信息，包括名称路径和层级
/// </summary>
public class UIType
{
    public string _name { get; private set; }
    public string _path { get; private set; }
    //层级
    public PanelManager.Layer _layer;

    public UIType(string path, PanelManager.Layer layer = PanelManager.Layer.Panel)
    {
        _path = path;
        _name = path.Substring(path.LastIndexOf('/') + 1);
        _layer = layer;
    }

}