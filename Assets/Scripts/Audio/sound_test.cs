using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound_test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //SoundManager.init();//初始化音乐音效管理

        //sound_manager.play_music("sounds/Login");//播放背景音乐

        //this.InvokeRepeating("test_music_mute", 1, 3);

        
        SoundManager.play_effect("sounds/按钮类1－mcx20070509");//播放音效,直接复制resuorces/sound路径下的文件名称就好
        if (SoundManager.effect_is_off())//如果当前是静音，就切换成有声音的状态
        {
            SoundManager.switch_effect();
        }
        this.InvokeRepeating("again", 3, 3);//每隔3秒调用一次

        //this.InvokeRepeating("again", 2, 3);
        //this.InvokeRepeating("test_effect_mute", 1, 3);
    }

    //背景音乐静音切换测试函数
    void test_music_mute()
    {
        Debug.Log("test_music_mute");
        SoundManager.switch_music();
    }

    //音效静音切换测试函数
    void test_effect_mute()
    {
        Debug.Log("test_effect_mute");
        SoundManager.switch_effect();
    }

    //隐藏AudioSource组件优化测试函数
    void again()
    {
        SoundManager.play_effect("sounds/按钮类1－mcx20070509");
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}