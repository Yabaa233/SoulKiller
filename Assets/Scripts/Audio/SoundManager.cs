using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用説明
//1：このコンポーネントをシーン内の空のオブジェクトに取り付け、リスナーをプレイヤー/カメラに取り付けます（3Dサウンドを使用する場合は推奨）。
//2：オーディオリソースは\Assets\Resources\soundsフォルダに置いてください。
//3：SoundManager.play+***関数を直接使用し、効果音はplay_effectを、音楽はplay_musicを使用します。パラメータはsounds/"名称"です。
//5：例えば、SoundManager.play_effect("サウンド/ボタン類1-mcx20070509");
//6：その音効が二度と現れないことを確認した後、クリアすることができます。
//7：3D音効果はplay_effect3Dを使用し、音声自体の位置パラメータを追加で渡します。
//8:
public class SoundManager:MonoBehaviour
{
    // (1) 音声のルートノードのオブジェクト;
    // (2) このノードがシーン切替時に削除されないことを保証し、再度初期化する必要がなくなります。
    // (3) すべての音声を再生するソースノードは、このノードの下にあります。
    static GameObject sound_play_object;//これがルートノードです。
    static bool is_music_mute = false;//現在のグローバルバックグラウンドミュージックがミュートかどうかを保存する変数
    static bool is_effect_mute = false;//現在の音声がミュートかどうかを保存する変数
    static  float music_volume = 1;//グローバル音楽ボリュームの保存
    static  float effect_volume = 1;//グローバルサウンドエフェクトボリュームの保存

    // url --> AudioSource マッピング、音楽と効果音の区別
    static Dictionary<string, AudioSource> musics = null;//音楽表
    static Dictionary<string, AudioSource> effects = null;//効果音表

    private void Awake()
    {     
        sound_play_object = this.gameObject;
        init();//音楽と効果音の管理を初期化する
        GameObject.DontDestroyOnLoad(this.gameObject);//シーンが切り替わる時、SoundManagerは削除されません。
    }

    private void Update()
    {
        //set_volume(music_volume,effect_volume);
    }

    //初期化
    public static void init()
    {

        //sound_play_object = new GameObject("sound_play_object");//ルートノードを初期化します
        sound_play_object.AddComponent<SoundScan>();//音声検出コンポーネントをルートノードにマウントしてください。
        GameObject.DontDestroyOnLoad(sound_play_object);//シーンを切り替える際に、ルートノードは削除されません。


        //音楽テーブルと効果音テーブルの初期化
        musics = new Dictionary<string, AudioSource>();
        effects = new Dictionary<string, AudioSource>();

        // このスイッチをローカルからロードします
        if (PlayerPrefs.HasKey("music_mute"))//is_music_muteがローカルに保存されているかどうかを判断してください。
        {
            int value = PlayerPrefs.GetInt("music_mute");
            is_music_mute = (value == 1);//intをboolに変換します。value==1の場合、trueを返し、それ以外の場合はfalseを返します。
        }

        // このスイッチをローカルからロードします
        if (PlayerPrefs.HasKey("effect_mute"))//is_effect_muteがローカルに保存されているかどうかを判断してください。
        {
            int value = PlayerPrefs.GetInt("effect_mute");
            is_effect_mute = (value == 1);//intをboolに変換します。value==1の場合、trueを返し、それ以外の場合はfalseを返します。
        }
    }




    ///<summary>





    ////// 指定のバックグラウンドミュージックを再生するインターフェース





    ///</summary>
    /// <param name="url"></param>
    /// <param name="is_loop"></param>
    public static void play_music(string url,float startPercent=0, float volume=1, bool is_loop = true)
    {
        AudioSource audio_source = null;
        if (musics.ContainsKey(url))//既にバックグラウンドミュージック表にあるかどうかを判断してください。
        {
            audio_source = musics[url];//そのまま代入してください。
        }
        else//新しい空のノードを作成し、そのノードの下に新しいAudioSourceコンポーネントを作成するわけではありません。
        {
            GameObject s = new GameObject(url);//空のノードを作成する
            s.transform.parent = sound_play_object.transform;//ノードをシーンに追加する

            audio_source = s.AddComponent<AudioSource>();//空のノードにAudioSourceコンポーネントを追加する
            AudioClip clip = Resources.Load<AudioClip>(url);//コードはAudioClipリソースファイルをロードします。
            audio_source.clip = clip;//コンポーネントのclip属性をclipに設定します。
            audio_source.loop = is_loop;//コンポーネントをループ再生に設定する
            audio_source.playOnAwake = true;//再度目覚める時に音を再生する
            audio_source.spatialBlend = 0.0f;//2Dサウンドに設定する
            audio_source.volume = volume * music_volume;

            musics.Add(url, audio_source);//バックグラウンドミュージックの辞書に追加しましたので、次回からは直接値を設定できます。
        }
        audio_source.mute = is_music_mute;
        audio_source.enabled = true;
        audio_source.time = startPercent * audio_source.clip.length;
        audio_source.Play();//再生を開始します
    }

    ///<summary>


    ////// 指定したバックグラウンドミュージックの再生を停止するインターフェース


    ///</summary>
    /// <param name="url"></param>
    public static void stop_music(string url)
    {
        AudioSource audio_source = null;
        if (!musics.ContainsKey(url))//既にバックグラウンドミュージック表にあるかどうかを判断してください。
        {
            return;//このバックグラウンドミュージックがなければ、直接戻ってください。
        }
        audio_source = musics[url];//それがあれば、audio_sourceを直接割り当ててください。
        audio_source.Stop();//再生を停止
    }

    /// <summary>
    ///すべてのバックグラウンド音楽の再生を停止するインターフェース
    /// </summary>
    public static void stop_all_music()
    {
        foreach (AudioSource s in musics.Values)
        {
            s.Stop();
        }
    }

    ///<summary>


    ////// 指定されたバックグラウンドミュージックとそのノードを削除します


    ///</summary>
    /// <param name="url"></param>
    public static void clear_music(string url)
    {
        AudioSource audio_source = null;
        if (!musics.ContainsKey(url))//既にバックグラウンドミュージック表にあるかどうかを判断してください。
        {
            return;//このバックグラウンドミュージックがなければ、直接戻ってください。
        }
        audio_source = musics[url];//それがあれば、audio_sourceを直接割り当ててください。
        musics[url] = null;//audio_sourceコンポーネントをクリアに指定します
        GameObject.Destroy(audio_source.gameObject);//指定のaudio_sourceコンポーネントがマウントされているノードを削除します
    }

    /// <summary>
    ///バックグラウンドミュージックのミュートスイッチを切り替える
    /// </summary>
    public static void switch_music()
    {
        // ミュートとサウンドの状態を切り替える
        is_music_mute = !is_music_mute;

        //現在のミュート状態をローカルに書き込む
        int value = (is_music_mute) ? 1 : 0;//boolをintに変換
        PlayerPrefs.SetInt("music_mute", value);

        // すべてのバックグラウンドミュージックのAudioSource要素を走査します。
        foreach (AudioSource s in musics.Values)
        {
            s.mute = is_music_mute;//現在の状態に設定する
        }
    }
    ///<summary>

    ////// 音楽再生をオンまたはオフに設定する

    ///</summary>
    /// <param name="to"></param>
    public static void switch_music_to(bool to)
    {
        // ミュートとサウンドの状態を切り替える
        is_music_mute = to;

        //現在のミュート状態をローカルに書き込む
        int value = (is_music_mute) ? 1 : 0;//boolをintに変換
        PlayerPrefs.SetInt("music_mute", value);

        // すべてのバックグラウンドミュージックのAudioSource要素を走査します。
        foreach (AudioSource s in musics.Values)
        {
            s.mute = is_music_mute;//現在の状態に設定する
        }
    }

    /// <summary>
    /// インターフェース：私のインターフェースのミュートボタンが表示されるとき、それは「オフ」を表示するべきか、それとも「開始」状態を表示するべきか。
    /// </summary>
    /// <returns></returns>
    public static bool music_is_off()
    {
        return is_music_mute;
    }





    //次に始まるのは、サウンドエフェクトのインターフェースです。
    //指定の音効を再生するインターフェース
    ///<summary>

    ////// 指定された音響効果を再生するインターフェース

    ///</summary>
    /// <param name="url"></param>
    /// <param name="is_loop"></param>
    public static void play_effect(string url, float startPercent=0, float volume = 1,bool is_loop = false)
    {
        AudioSource audio_source = null;
        if (effects.ContainsKey(url))//既に音響表の中にあるかどうかを判断してください。
        {
            audio_source = effects[url];//そのまま代入してください。
            if (audio_source.isPlaying)
            {
                GameObject s = new GameObject(url);//空のノードを作成する
                s.transform.parent = sound_play_object.transform;//ノードをシーンに追加する
                Destroy(s, 3);
                audio_source = s.AddComponent<AudioSource>();//空のノードにAudioSourceコンポーネントを追加する
                AudioClip clip = Resources.Load<AudioClip>(url);//コードはAudioClipリソースファイルをロードします。
                audio_source.clip = clip;//コンポーネントのclip属性をclipに設定します。
                audio_source.loop = is_loop;//コンポーネントをループ再生に設定する
                audio_source.playOnAwake = true  ;//再度目覚める時に音を再生する
                audio_source.spatialBlend = 0.0f;//2Dサウンドに設定する
                audio_source.volume = volume * effect_volume;

            }


        }
        else//新しい空のノードを作成し、そのノードの下に新しいAudioSourceコンポーネントを作成するわけではありません。
        {
            GameObject s = new GameObject(url);//空のノードを作成する
            s.transform.parent = sound_play_object.transform;//ノードをシーンに追加する

            audio_source = s.AddComponent<AudioSource>();//空のノードにAudioSourceコンポーネントを追加する
            AudioClip clip = Resources.Load<AudioClip>(url);//コードはAudioClipリソースファイルをロードします。
            audio_source.clip = clip;//コンポーネントのclip属性をclipに設定します。
            audio_source.loop = is_loop;//コンポーネントをループ再生に設定する
            audio_source.playOnAwake = true  ;//再度目覚める時に音を再生する
            audio_source.spatialBlend = 0.0f;//2Dサウンドに設定する
            audio_source.volume = volume * effect_volume;

            effects.Add(url, audio_source);//サウンドエフェクト辞書に追加し、次回からは直接値を設定できます。
           

        }
        audio_source.mute = is_effect_mute;
        audio_source.enabled = true;
        //audio_source.SetScheduledStartTime(percent * audio_source.clip.length);
        //audio_source.SetScheduledEndTime( audio_source.clip.length);
        //audio_source.PlayScheduled(percent * audio_source.clip.length);//再生を開始する
        audio_source.time = startPercent * audio_source.clip.length;
        audio_source.Play();

    }


    //指定された音響効果の再生を停止するインターフェース
    ///<summary>

    ////// 指定された音響効果の再生を停止するインターフェース

    ///</summary>
    /// <param name="url"></param>
    public static void stop_effect(string url)
    {
        AudioSource audio_source = null;
        if (!effects.ContainsKey(url))//既に音響表の中にあるかどうかを判断してください。
        {
            return;//このバックグラウンドミュージックがなければ、直接戻ってください。
        }
        audio_source = effects[url];//それがあれば、audio_sourceを直接割り当ててください。
        audio_source.Stop();//再生を停止
    }

    //すべての音声を停止するインターフェース
    /// <summary>
    /// すべての音声を停止するインターフェース
    /// </summary>
    public static void stop_all_effect()
    {
        foreach (AudioSource s in effects.Values)
        {
            s.Stop();
        }
    }

    //指定された音響効果とそのノードを削除します
    ///<summary>

    ////// 指定された音響効果とそのノードを削除します

    ///</summary>
    /// <param name="url"></param>
    public static void clear_effect(string url)
    {
        AudioSource audio_source = null;
        if (!effects.ContainsKey(url))//既に音響表の中にあるかどうかを判断してください。
        {
            return;//このサウンドエフェクトがなければ、直接戻ってください。
        }
        audio_source = effects[url];//それがあれば、audio_sourceを直接割り当ててください。
        effects[url] = null;//audio_sourceコンポーネントをクリアに指定します
        GameObject.Destroy(audio_source.gameObject);//指定のaudio_sourceコンポーネントがマウントされているノードを削除します
    }

    //サウンドエフェクトのミュートスイッチを切り替える
    /// <summary>
    /// サウンドエフェクトのミュートスイッチを切り替える
    /// </summary>
    public static void switch_effect()
    {
        // ミュートとサウンドの状態を切り替える
        is_effect_mute = !is_effect_mute;

        //現在のミュート状態をローカルに書き込む
        int value = (is_effect_mute) ? 1 : 0;//boolをintに変換
        PlayerPrefs.SetInt("effect_mute", value);

        // すべての音響効果のAudioSource要素を走査する
        foreach (AudioSource s in effects.Values)
        {
            s.mute = is_effect_mute;//現在の状態に設定する
        }
    }
    /// <summary>
    /// サウンドエフェクトの再生をオンまたはオフに設定します
    /// </summary>
    /// <param name="to"></param>
    public static void switch_effect(bool to)
    {
        // ミュートとサウンドの状態を切り替える
        is_effect_mute = to;

        //現在のミュート状態をローカルに書き込む
        int value = (is_effect_mute) ? 1 : 0;//boolをintに変換
        PlayerPrefs.SetInt("effect_mute", value);

        // すべての音響効果のAudioSource要素を走査する
        foreach (AudioSource s in effects.Values)
        {
            s.mute = is_effect_mute;//現在の状態に設定する
        }
    }
    //私のインターフェースのミュートボタンが表示されるとき、結局はオフ表示にするべきか、それとも開始状態にするべきか。
    ///<summary>

    ////// 私のインターフェースにミュートボタンが表示されるとき、結局はオフ表示にすべきなのか、それとも開始状態にすべきなのか。

    ///</summary>
    /// <returns></returns>
    public static bool effect_is_off()
    {
        return is_effect_mute;
    }

    //3D音響効果を再生する
    /// <summary>
    /// 3D音響効果を再生する
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
            s.transform.position = pos;//3D音響の位置

            audio_source = s.AddComponent<AudioSource>();
            AudioClip clip = Resources.Load<AudioClip>(url);
            audio_source.clip = clip;
            audio_source.loop = is_loop;
            audio_source.playOnAwake = true;
            audio_source.spatialBlend = 1.0f; // 3Dサウンドエフェクト
            audio_source.volume = volume * effect_volume;

            effects.Add(url, audio_source);
        }
        audio_source.mute = is_effect_mute;
        audio_source.enabled = true;
        audio_source.time = startPercent * audio_source.clip.length;

        audio_source.Play();
    }




    //最適化戦略インターフェース
    public static void disable_over_audio()
    {
        //バックグラウンドミュージック表を走査する
        foreach (AudioSource s in musics.Values)
        {
            if (!s.isPlaying)//再生中かどうかを判断する
            {
                s.enabled = false;//再生していない場合は直接非表示にします
            }
        }

        //サウンドエフェクト表を走査する
        foreach (AudioSource s in effects.Values)
        {
            if (!s.isPlaying)//再生中かどうかを判断する
            {
                s.enabled = false;//再生していない場合は直接非表示にします
            }
        }
    }

    /// <summary>
    /// ボリュームを設定する
    /// </summary>
    /// <param name="m_value"></param>
    /// <param name="e_value"></param>
    public static void set_volume(float m_value,float e_value)
    {
        //バックグラウンドミュージック表を走査する
        foreach (AudioSource s in musics.Values)
        {
            s.volume = m_value/100;
            PlayerPrefs.SetFloat("music_volume", m_value);
        }

        //サウンドエフェクト表を走査する
        foreach (AudioSource s in effects.Values)
        {
            s.volume = e_value/100;
            PlayerPrefs.SetFloat("effect_volume", e_value);
        }
    }

    public static void clear_all()
    {
        //バックグラウンドミュージック表を走査する
        foreach (AudioSource s in musics.Values)
        {
            if(s.enabled == false)
            {

            }
           
           
        }

        //サウンドエフェクト表を走査する
        foreach (AudioSource s in effects.Values)
        {
            if (s.enabled == false)
            {

            }
        }
    }
}

///<summary>


////// 再生が完了した後、コンポーネントを閉じてパフォーマンスを最適化します。


///</summary>
public class SoundScan : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //一定のリズムでスキャンし、0.5秒ごとに一度スキャンします。
        this.InvokeRepeating("scan", 0, 0.5f);
        //this.InvokeRepeating("autoClear", 0, 15f);

    }


    //タイマー関数
    void scan()
    {
        SoundManager.disable_over_audio();//隠されたAudioSourceコンポーネントインターフェースを呼び出す
    }

    void autoClear()
    {
        SoundManager.clear_all();
    }
}