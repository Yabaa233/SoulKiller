using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound_test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //SoundManager.init();//��ʼ��������Ч����

        //sound_manager.play_music("sounds/Login");//���ű�������

        //this.InvokeRepeating("test_music_mute", 1, 3);

        
        SoundManager.play_effect("sounds/��ť��1��mcx20070509");//������Ч,ֱ�Ӹ���resuorces/sound·���µ��ļ����ƾͺ�
        if (SoundManager.effect_is_off())//�����ǰ�Ǿ��������л�����������״̬
        {
            SoundManager.switch_effect();
        }
        this.InvokeRepeating("again", 3, 3);//ÿ��3�����һ��

        //this.InvokeRepeating("again", 2, 3);
        //this.InvokeRepeating("test_effect_mute", 1, 3);
    }

    //�������־����л����Ժ���
    void test_music_mute()
    {
        Debug.Log("test_music_mute");
        SoundManager.switch_music();
    }

    //��Ч�����л����Ժ���
    void test_effect_mute()
    {
        Debug.Log("test_effect_mute");
        SoundManager.switch_effect();
    }

    //����AudioSource����Ż����Ժ���
    void again()
    {
        SoundManager.play_effect("sounds/��ť��1��mcx20070509");
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}