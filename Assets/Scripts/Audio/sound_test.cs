using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound_test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //SoundManager.init();//音楽と効果音の管理を初期化

        //sound_manager.play_music("sounds/Login");//バックグラウンドミュージックを再生

        //this.InvokeRepeating("test_music_mute", 1, 3);

        
        SoundManager.play_effect("サウンド/ボタンクラス1-mcx20070509");//サウンドエフェクトを再生するには、resuorces/soundパスのファイル名を直接コピーしてください。
        if (SoundManager.effect_is_off())//現在がミュート状態であれば、音声ありの状態に切り替えてください。
        {
            SoundManager.switch_effect();
        }
        this.InvokeRepeating("again", 3, 3);//3秒ごとに呼び出します

        //this.InvokeRepeating("again", 2, 3);
        //this.InvokeRepeating("test_effect_mute", 1, 3);
    }

    //バックグラウンドミュージックのミュート切り替えテスト関数
    void test_music_mute()
    {
        Debug.Log("test_music_mute");
        SoundManager.switch_music();
    }

    //サウンドミュート切り替えテスト関数
    void test_effect_mute()
    {
        Debug.Log("test_effect_mute");
        SoundManager.switch_effect();
    }

    //AudioSourceコンポーネントの最適化テスト関数を隠す
    void again()
    {
        SoundManager.play_effect("サウンド/ボタンクラス1-mcx20070509");
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}