using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    /// <summary>
    /// アーカイブ
    /// </summary>
    /// <param name="saveFileName">セーブファイルの名前</param>
    /// <param name="data">保存されたデータ</param>
    public static void SaveByJson(string saveFileName,object data)
    {
        var json = JsonUtility.ToJson(data);
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);
    }
    /// <summary>
    /// セーブデータを読む
    /// </summary>
    /// <typeparam name="T">ジェネリック、つまり、どのタイプのデータが必要かを指します</typeparam>
    /// <param name="saveFileName">セーブデータのファイル名</param>
    /// <returns></returns>
    public static T LoadFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<T>(json);
        return data;
    }
}
