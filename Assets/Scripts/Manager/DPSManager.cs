using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DPSManager : MonoBehaviour
{
    private static DPSManager instance;
    public static DPSManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DPSManager();              
            }               
            return instance;
        }
    }
    [Header("DPS:")]
    public float showDPS = 0;
    public float showBOSSDPS = 0;

    [Header("����ʱ��")]
    [Range(0.1f,1.5f)]
    public float sampleTime = 1;
    [Header("BOSS����ʱ��")]
    [Range(2f, 5f)]
    public float sampleBOSSTime = 2.5f;
    private float totalDamage=0;
    private float totalBOSSDamage = 0;

    private float time=0;
    private float bossTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void PlusDamage(float damage)
    {
        totalDamage += damage;
    }
    public void PlusBOSSDamage(float damage)
    {
        totalBOSSDamage += damage;
    }
    private void ClearDamage()
    {
        totalDamage = 0;
    }
    // Update is called once per frame

    public IEnumerator UpdateDPS()
    {
        while (true)
        {
        time += Time.deltaTime;
        if (time >= sampleTime)
        {
            //Debug.LogFormat("DPS:", totalDamage);
            showDPS = totalDamage / sampleTime;

            time = 0.0f;
            totalDamage = 0;

        }
        bossTime += Time.deltaTime;
        if (bossTime >= sampleBOSSTime)
        {
            //Debug.LogFormat("DPS:", totalDamage);
            showBOSSDPS = totalBOSSDamage / sampleBOSSTime;
            bossTime = 0.0f;

            totalBOSSDamage = 0;
        }
        yield return null;
        }
    }
    void Update()
    {
        time += Time.deltaTime;
        if (time >= sampleTime)
        {
            //Debug.LogFormat("DPS:", totalDamage);
            showDPS = totalDamage / sampleTime;

            time = 0.0f;
            totalDamage = 0;

        }
        bossTime += Time.deltaTime;
        if (bossTime >= sampleBOSSTime)
        {
            //Debug.LogFormat("DPS:", totalDamage);
            showBOSSDPS = totalBOSSDamage / sampleBOSSTime;
            bossTime = 0.0f;

            totalBOSSDamage = 0;
        }

    }
}

// public class DPSWindow : EditorWindow
// {
//     private static DPSWindow window;
//     private static readonly Vector2 MIN_SIZE = new Vector2(200, 100);
//     private float dps;
//     private float bossDps;


//     [MenuItem("Tools/DPSCaculate")]
//     private static void PopUp()
//     {
//         window = GetWindow<DPSWindow>("");
//         window.minSize = MIN_SIZE;
//         window.maxSize = MIN_SIZE;
//         window.Show();
//     }

//     private void OnGUI()
//     {

//         EditorGUILayout.FloatField("��ҵ�DPS", dps);


//         EditorGUILayout.FloatField("BOSS��DPS",bossDps);
        
        
//     }
//     private void Update()
//     {
        
//         dps = DPSManager.Instance.showDPS;
//         bossDps = DPSManager.Instance.showBOSSDPS;
//         Repaint();
//     }
// }

