using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用说明
//1：将此组件挂在场景中的空物体上，listener挂在玩家/摄像机身上（建议，如果使用3D音效的话）
//2：音频资源放在\Assets\Resources\sounds文件夹下
//3：直接使用SoundManager.play+***函数，音效使用play_effect，音乐使用play_music,参数为sounds/"名称"
//5：例如SoundManager.play_effect("sounds/按钮类1－mcx20070509");
//6：确定该音效不会再出现后可以clear
//7：3D音效使用play_effect3D,额外传入音频自身位置参数
//8:
public class SoundManager:MonoBehaviour
{
    // (1) 声音根节点的物体;
    // (2) 保证这个节点在场景切换的时候不会删除，这样就不用再初始化一次;
    // (3) 所有播放声音的生源节点，都是在这个节点下
    static GameObject sound_play_object;//这个就是根节点
    static bool is_music_mute = false;//存放当前全局背景音乐是否静音的变量
    static bool is_effect_mute = false;//存放当前音效是否静音的变量
    static  float music_volume = 1;//存放全局音乐音量
    static  float effect_volume = 1;//存放全局音效音量

    // url --> AudioSource 映射, 区分音乐，音效
    static Dictionary<string, AudioSource> musics = null;//音乐表
    static Dictionary<string, AudioSource> effects = null;//音效表

    private void Awake()
    {     
        sound_play_object = this.gameObject;
        init();//初始化音乐音效管理
        GameObject.DontDestroyOnLoad(this.gameObject);//场景切换的时候不会删除SoundManager
    }

    private void Update()
    {
        //set_volume(music_volume,effect_volume);
    }

    //初始化
    public static void init()
    {

        //sound_play_object = new GameObject("sound_play_object");//初始化根节点
        sound_play_object.AddComponent<SoundScan>();//把声音检测组件挂载到根节点下
        GameObject.DontDestroyOnLoad(sound_play_object);//场景切换的时候不会删除根节点


        //初始化音乐表和音效表
        musics = new Dictionary<string, AudioSource>();
        effects = new Dictionary<string, AudioSource>();

        // 从本地来加载这个开关
        if (PlayerPrefs.HasKey("music_mute"))//判断is_music_mute有没有保存在本地
        {
            int value = PlayerPrefs.GetInt("music_mute");
            is_music_mute = (value == 1);//int转换bool，如果value==1，返回true，否则就是false
        }

        // 从本地来加载这个开关
        if (PlayerPrefs.HasKey("effect_mute"))//判断is_effect_mute有没有保存在本地
        {
            int value = PlayerPrefs.GetInt("effect_mute");
            is_effect_mute = (value == 1);//int转换bool，如果value==1，返回true，否则就是false
        }
    }




    /// <summary>
    /// 播放指定背景音乐的接口
    /// </summary>
    /// <param name="url"></param>
    /// <param name="is_loop"></param>
    public static void play_music(string url,float startPercent=0, float volume=1, bool is_loop = true)
    {
        AudioSource audio_source = null;
        if (musics.ContainsKey(url))//判断是否已经在背景音乐表里面了
        {
            audio_source = musics[url];//是就直接赋值过去
        }
        else//不是就新建一个空节点，节点下再新建一个AudioSource组件
        {
            GameObject s = new GameObject(url);//创建一个空节点
            s.transform.parent = sound_play_object.transform;//加入节点到场景中

            audio_source = s.AddComponent<AudioSource>();//空节点添加组件AudioSource
            AudioClip clip = Resources.Load<AudioClip>(url);//代码加载一个AudioClip资源文件
            audio_source.clip = clip;//设置组件的clip属性为clip
            audio_source.loop = is_loop;//设置组件循环播放
            audio_source.playOnAwake = true;//再次唤醒时播放声音
            audio_source.spatialBlend = 0.0f;//设置为2D声音
            audio_source.volume = volume * music_volume;

            musics.Add(url, audio_source);//加入到背景音乐字典中，下次就可以直接赋值了
        }
        audio_source.mute = is_music_mute;
        audio_source.enabled = true;
        audio_source.time = startPercent * audio_source.clip.length;
        audio_source.Play();//开始播放
    }

    /// <summary>
    /// 停止播放指定背景音乐的接口
    /// </summary>
    /// <param name="url"></param>
    public static void stop_music(string url)
    {
        AudioSource audio_source = null;
        if (!musics.ContainsKey(url))//判断是否已经在背景音乐表里面了
        {
            return;//没有这个背景音乐就直接返回
        }
        audio_source = musics[url];//有就把audio_source直接赋值过去
        audio_source.Stop();//停止播放
    }

    /// <summary>
    ///停止播放所有背景音乐的接口
    /// </summary>
    public static void stop_all_music()
    {
        foreach (AudioSource s in musics.Values)
        {
            s.Stop();
        }
    }

    /// <summary>
    ///删除指定背景音乐和它的节点
    /// </summary>
    /// <param name="url"></param>
    public static void clear_music(string url)
    {
        AudioSource audio_source = null;
        if (!musics.ContainsKey(url))//判断是否已经在背景音乐表里面了
        {
            return;//没有这个背景音乐就直接返回
        }
        audio_source = musics[url];//有就把audio_source直接赋值过去
        musics[url] = null;//指定audio_source组件清空
        GameObject.Destroy(audio_source.gameObject);//删除掉挂载指定audio_source组件的节点
    }

    /// <summary>
    ///切换背景音乐静音开关
    /// </summary>
    public static void switch_music()
    {
        // 切换静音和有声音的状态
        is_music_mute = !is_music_mute;

        //把当前是否静音写入本地
        int value = (is_music_mute) ? 1 : 0;//bool转换int
        PlayerPrefs.SetInt("music_mute", value);

        // 遍历所有背景音乐的AudioSource元素
        foreach (AudioSource s in musics.Values)
        {
            s.mute = is_music_mute;//设置为当前的状态
        }
    }
    /// <summary>
    /// 设置音乐播放开启或关闭
    /// </summary>
    /// <param name="to"></param>
    public static void switch_music_to(bool to)
    {
        // 切换静音和有声音的状态
        is_music_mute = to;

        //把当前是否静音写入本地
        int value = (is_music_mute) ? 1 : 0;//bool转换int
        PlayerPrefs.SetInt("music_mute", value);

        // 遍历所有背景音乐的AudioSource元素
        foreach (AudioSource s in musics.Values)
        {
            s.mute = is_music_mute;//设置为当前的状态
        }
    }

    /// <summary>
    /// 接口：当我的界面的静音按钮要显示的时候，到底是显示关闭，还是开始状态;
    /// </summary>
    /// <returns></returns>
    public static bool music_is_off()
    {
        return is_music_mute;
    }





    //接下来开始是音效的接口
    //播放指定音效的接口
    /// <summary>
    /// 播放指定音效的接口
    /// </summary>
    /// <param name="url"></param>
    /// <param name="is_loop"></param>
    public static void play_effect(string url, float startPercent=0, float volume = 1,bool is_loop = false)
    {
        AudioSource audio_source = null;
        if (effects.ContainsKey(url))//判断是否已经在音效表里面了
        {
            audio_source = effects[url];//是就直接赋值过去
            if (audio_source.isPlaying)
            {
                GameObject s = new GameObject(url);//创建一个空节点
                s.transform.parent = sound_play_object.transform;//加入节点到场景中
                Destroy(s, 3);
                audio_source = s.AddComponent<AudioSource>();//空节点添加组件AudioSource
                AudioClip clip = Resources.Load<AudioClip>(url);//代码加载一个AudioClip资源文件
                audio_source.clip = clip;//设置组件的clip属性为clip
                audio_source.loop = is_loop;//设置组件循环播放
                audio_source.playOnAwake = true  ;//再次唤醒时播放声音
                audio_source.spatialBlend = 0.0f;//设置为2D声音
                audio_source.volume = volume * effect_volume;

            }


        }
        else//不是就新建一个空节点，节点下再新建一个AudioSource组件
        {
            GameObject s = new GameObject(url);//创建一个空节点
            s.transform.parent = sound_play_object.transform;//加入节点到场景中

            audio_source = s.AddComponent<AudioSource>();//空节点添加组件AudioSource
            AudioClip clip = Resources.Load<AudioClip>(url);//代码加载一个AudioClip资源文件
            audio_source.clip = clip;//设置组件的clip属性为clip
            audio_source.loop = is_loop;//设置组件循环播放
            audio_source.playOnAwake = true  ;//再次唤醒时播放声音
            audio_source.spatialBlend = 0.0f;//设置为2D声音
            audio_source.volume = volume * effect_volume;

            effects.Add(url, audio_source);//加入到音效字典中，下次就可以直接赋值了
           

        }
        audio_source.mute = is_effect_mute;
        audio_source.enabled = true;
        //audio_source.SetScheduledStartTime(percent * audio_source.clip.length);
        //audio_source.SetScheduledEndTime( audio_source.clip.length);
        //audio_source.PlayScheduled(percent * audio_source.clip.length);//开始播放
        audio_source.time = startPercent * audio_source.clip.length;
        audio_source.Play();

    }


    //停止播放指定音效的接口
    /// <summary>
    /// 停止播放指定音效的接口
    /// </summary>
    /// <param name="url"></param>
    public static void stop_effect(string url)
    {
        AudioSource audio_source = null;
        if (!effects.ContainsKey(url))//判断是否已经在音效表里面了
        {
            return;//没有这个背景音乐就直接返回
        }
        audio_source = effects[url];//有就把audio_source直接赋值过去
        audio_source.Stop();//停止播放
    }

    //停止播放所有音效的接口
    /// <summary>
    /// 停止播放所有音效的接口
    /// </summary>
    public static void stop_all_effect()
    {
        foreach (AudioSource s in effects.Values)
        {
            s.Stop();
        }
    }

    //删除指定音效和它的节点
    /// <summary>
    /// 删除指定音效和它的节点
    /// </summary>
    /// <param name="url"></param>
    public static void clear_effect(string url)
    {
        AudioSource audio_source = null;
        if (!effects.ContainsKey(url))//判断是否已经在音效表里面了
        {
            return;//没有这个音效就直接返回
        }
        audio_source = effects[url];//有就把audio_source直接赋值过去
        effects[url] = null;//指定audio_source组件清空
        GameObject.Destroy(audio_source.gameObject);//删除掉挂载指定audio_source组件的节点
    }

    //切换音效静音开关
    /// <summary>
    /// 切换音效静音开关
    /// </summary>
    public static void switch_effect()
    {
        // 切换静音和有声音的状态
        is_effect_mute = !is_effect_mute;

        //把当前是否静音写入本地
        int value = (is_effect_mute) ? 1 : 0;//bool转换int
        PlayerPrefs.SetInt("effect_mute", value);

        // 遍历所有音效的AudioSource元素
        foreach (AudioSource s in effects.Values)
        {
            s.mute = is_effect_mute;//设置为当前的状态
        }
    }
    /// <summary>
    /// 设置音效播放开启或关闭
    /// </summary>
    /// <param name="to"></param>
    public static void switch_effect(bool to)
    {
        // 切换静音和有声音的状态
        is_effect_mute = to;

        //把当前是否静音写入本地
        int value = (is_effect_mute) ? 1 : 0;//bool转换int
        PlayerPrefs.SetInt("effect_mute", value);

        // 遍历所有音效的AudioSource元素
        foreach (AudioSource s in effects.Values)
        {
            s.mute = is_effect_mute;//设置为当前的状态
        }
    }
    //当我的界面的静音按钮要显示的时候，到底是显示关闭，还是开始状态;
    /// <summary>
    /// 当我的界面的静音按钮要显示的时候，到底是显示关闭，还是开始状态;
    /// </summary>
    /// <returns></returns>
    public static bool effect_is_off()
    {
        return is_effect_mute;
    }

    //播放3D的音效
    /// <summary>
    /// 播放3D的音效
    /// </summary>
    /// <param name="url"></param>
    /// <param name="pos"></param>
    /// <param name="is_loop"></param>
    public static void play_effect3D(string url, Vector3 pos,float startPercent=0, float volume=1, bool is_loop = false)
    {
        AudioSource audio_source = null;
        if (effects.ContainsKey(url))
        {
            audio_source = effects[url];
        }
        else
        {
            GameObject s = new GameObject(url);
            s.transform.parent = sound_play_object.transform;
            s.transform.position = pos;//3D音效的位置

            audio_source = s.AddComponent<AudioSource>();
            AudioClip clip = Resources.Load<AudioClip>(url);
            audio_source.clip = clip;
            audio_source.loop = is_loop;
            audio_source.playOnAwake = true;
            audio_source.spatialBlend = 1.0f; // 3D音效
            audio_source.volume = volume * effect_volume;

            effects.Add(url, audio_source);
        }
        audio_source.mute = is_effect_mute;
        audio_source.enabled = true;
        audio_source.time = startPercent * audio_source.clip.length;

        audio_source.Play();
    }




    //优化策略接口
    public static void disable_over_audio()
    {
        //遍历背景音乐表
        foreach (AudioSource s in musics.Values)
        {
            if (!s.isPlaying)//判断是否在播放
            {
                s.enabled = false;//不在播放就直接隐藏
            }
        }

        //遍历音效表
        foreach (AudioSource s in effects.Values)
        {
            if (!s.isPlaying)//判断是否在播放
            {
                s.enabled = false;//不在播放就直接隐藏
            }
        }
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="m_value"></param>
    /// <param name="e_value"></param>
    public static void set_volume(float m_value,float e_value)
    {
        //遍历背景音乐表
        foreach (AudioSource s in musics.Values)
        {
            s.volume = m_value/100;
            PlayerPrefs.SetFloat("music_volume", m_value);
        }

        //遍历音效表
        foreach (AudioSource s in effects.Values)
        {
            s.volume = e_value/100;
            PlayerPrefs.SetFloat("effect_volume", e_value);
        }
    }

    public static void clear_all()
    {
        //遍历背景音乐表
        foreach (AudioSource s in musics.Values)
        {
            if(s.enabled == false)
            {

            }
           
           
        }

        //遍历音效表
        foreach (AudioSource s in effects.Values)
        {
            if (s.enabled == false)
            {

            }
        }
    }
}

/// <summary>
/// 播放完毕后关闭组件，优化性能
/// </summary>
public class SoundScan : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //固定一个节奏去扫描，每隔0.5s扫描一次
        this.InvokeRepeating("scan", 0, 0.5f);
        //this.InvokeRepeating("autoClear", 0, 15f);

    }


    //定时器函数
    void scan()
    {
        SoundManager.disable_over_audio();//调用隐藏AudioSource组件接口
    }

    void autoClear()
    {
        SoundManager.clear_all();
    }
}