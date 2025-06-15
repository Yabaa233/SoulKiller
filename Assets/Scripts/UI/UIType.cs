using System.Collections;
using UnityEngine;

/// <summary>
/// UIの情報を保存します。これには名前のパスとレベルが含まれます。
/// </summary>
public class UIType
{
    public string _name { get; private set; }
    public string _path { get; private set; }
    //階層
    public PanelManager.Layer _layer;

    public UIType(string path, PanelManager.Layer layer = PanelManager.Layer.Panel)
    {
        _path = path;
        _name = path.Substring(path.LastIndexOf('/') + 1);
        _layer = layer;
    }

}