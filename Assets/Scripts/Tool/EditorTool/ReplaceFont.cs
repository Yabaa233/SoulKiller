// /*****************************************************************
//  *Author: ZhaoZhen
//  *UnityVersion: 2020.3.1f1
//  *Function: 
//  *******************************************************************/
// using UnityEngine;
// using System.Collections;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine.UI;
// using System.Collections.Generic;

// namespace zhaozhen
// {
//     /// <summary>
//     /// ���������е�����
//     /// </summary>
//     public class ReplaceFont : EditorWindow
//     {
//         [MenuItem("Tools/������������")]
//         public static void Open()
//         {
//             EditorWindow.GetWindow(typeof(ReplaceFont));
//         }

//         Font toChange;
//         static Font toChangeFont;
//         FontStyle toFontStyle;
//         static FontStyle toChangeFontStyle;

//         void OnGUI()
//         {
//             toChange = (Font)EditorGUILayout.ObjectField(toChange, typeof(Font), true, GUILayout.MinWidth(100f));
//             toChangeFont = toChange;
//             toFontStyle = (FontStyle)EditorGUILayout.EnumPopup(toFontStyle, GUILayout.MinWidth(100f));
//             toChangeFontStyle = toFontStyle;
//             if (GUILayout.Button("����Hierarchy���������text����"))
//             {
//                 ChangeFont_Scene();
//             }
//             if (GUILayout.Button("����Ԥ����������text����"))
//             {
//                 ChangeFont_Prefab();
//             }
//         }

//         /// <summary>
//         /// ����������Hierarchy���������text���巽��
//         /// </summary>
//         public static void ChangeFont_Scene()
//         {
//             //Ѱ��Hierarchy��������е�Text
//             var tArray = Resources.FindObjectsOfTypeAll(typeof(Text));
//             for (int i = 0; i < tArray.Length; i++)
//             {
//                 Text t = tArray[i] as Text;
//                 //�������Ҫ�����û��������룬unity�ǲ��������༭���иĶ��ģ���Ȼ�������ֱ���л������ı��ǲ�������� ��
//                 //��������������  ��������ĺ� �Լ�����ֶ��޸��³����������״̬ �ڱ���ͺ��� 
//                 Undo.RecordObject(t, t.gameObject.name);
//                 t.font = toChangeFont;
//                 t.fontStyle = toChangeFontStyle;
//                 //�൱������ˢ���� ��Ȼunity��ʾ���滹��֪���Լ��Ķ�����������  �����������ʾ֮ǰ�Ķ���
//                 EditorUtility.SetDirty(t);
//             }
//             Debug.Log("Succed��Hierarchy�����text��������ɹ���");
//         }

//         /// <summary>
//         /// ����Ԥ�Ƽ���text����
//         /// </summary>
//         public static void ChangeFont_Prefab()
//         {
//             List<Text[]> textList = new List<Text[]>();//��ʾ��ȡ����Ԥ�Ƽ��е�text����б�
//                                                        //��ȡAsset�ļ���������Prefab��GUID
//             string[] ids = AssetDatabase.FindAssets("t:Prefab");
//             string tmpPath;//��ʾ��ȡ����Ԥ�Ƽ���λ��·��
//             GameObject tmpObj;//��ʾ��ȡ����Ԥ�Ƽ�����
//             Text[] tmpArr;
//             for (int i = 0; i < ids.Length; i++)
//             {
//                 tmpObj = null;
//                 tmpArr = null;
//                 //����GUID��ȡ·��
//                 tmpPath = AssetDatabase.GUIDToAssetPath(ids[i]);
//                 if (!string.IsNullOrEmpty(tmpPath))//·����Ϊ��
//                 {
//                     //����·����ȡPrefab(GameObject)
//                     tmpObj = AssetDatabase.LoadAssetAtPath(tmpPath, typeof(GameObject)) as GameObject;
//                     //�ж��Ƿ���text���
//                     if (tmpObj.GetComponentsInChildren<Text>().Length != 0)
//                     {
//                         //����Ԥ����
//                         var path = tmpPath;
//                         var instance = PrefabUtility.LoadPrefabContents(path);
//                         if (instance != null)
//                         {
//                             //��ȡPrefab����������������.......������Text���
//                             tmpArr = instance.GetComponentsInChildren<Text>();
//                             for (int j = 0; j < tmpArr.Length; j++)
//                             {
//                                 tmpArr[j].font = toChangeFont;
//                                 tmpArr[j].fontStyle = toChangeFontStyle;
//                             }
//                         }
//                         // ����Ԥ����
//                         PrefabUtility.SaveAsPrefabAsset(instance, path);
//                         PrefabUtility.UnloadPrefabContents(instance);
//                     }
//                 }
//             }
//             Debug.Log("Succed��Ԥ�Ƽ���������ɹ���");
//         }
//     }
// }

