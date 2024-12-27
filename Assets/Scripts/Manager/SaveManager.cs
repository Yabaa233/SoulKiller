using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    /// <summary>
    /// 存档
    /// </summary>
    /// <param name="saveFileName">存档文件的名字</param>
    /// <param name="data">存档的数据</param>
    public static void SaveByJson(string saveFileName,object data)
    {
        var json = JsonUtility.ToJson(data);
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);
    }
    /// <summary>
    /// 读档
    /// </summary>
    /// <typeparam name="T">泛型，指我们需要哪种类型的数据</typeparam>
    /// <param name="saveFileName">存档所在的文件名称</param>
    /// <returns></returns>
    public static T LoadFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<T>(json);
        return data;
    }
}
