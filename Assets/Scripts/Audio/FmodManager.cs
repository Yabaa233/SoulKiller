using UnityEngine;
using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;


public class FmodManager : singleton<FmodManager>
{
    //protected static FmodManager _instance;

    //public static FmodManager Instance
    //{
    //    get
    //    {
    //        return _instance;
    //    }
    //}


    public BGMPathDefinition[] BGMPathDefinitions=new BGMPathDefinition[5];
    protected Dictionary<BGMType, string> BGMPathDict;
    public BGMType currentAmbientAudio;

    static float music_volume = 0;//���ȫ����������?
    static float effect_volume = 0;//���ȫ����Ч����?
    
    //public BossControl bossControl;
    public int bossState=-1;
    public int normalState=-1;

    //static bool is_music_mute = false;//��ŵ�ǰȫ�ֱ��������Ƿ����ı���?
    //static bool is_effect_mute = false;//��ŵ�ǰ��Ч�Ƿ����ı���?
    //һ������ʵ�����ɿ��ơ�
    //ѭ�����ŵĿ��Դ���һ��,Ҫ�ֶ�release, OneShot Ϊtrue�������Զ�release.
    //http://www.fmod.org/docs/content/generated/studio_api_EventInstance.html
    //http://www.fmod.org/docs/content/generated/FMOD_Studio_EventDescription_IsOneshot.html
    public EventInstance BGMEvent;
    protected EventReference readyEffectEvent;
    //������ȡ�����ڹ���������������?�ò���
    //http://www.fmod.org/docs/content/generated/studio_api_EventDescription.html
    //private EventDescription musicDescription;

    protected Dictionary<EventReference, EventInstance> loopingSoundEvents = new Dictionary<EventReference, EventInstance>();
    protected Dictionary<string, EventInstance> SpecialSoundEvents = new Dictionary<string, EventInstance>();

    protected List<EventInstance> pausedSoundEvents;
    //protected const int minPathLength = 7;
    protected virtual void Awake()
    {
        base.Awake();
        GameObject.DontDestroyOnLoad(this.gameObject);

       // _instance = this;
        //FMODUnity.RuntimeManager.LoadBank("bank:/Master");
        //FMODUnity.RuntimeManager.LoadBank("bank:/Player");
        //FMODUnity.RuntimeManager.LoadBank("bank:/Music");
        //FMODUnity.RuntimeManager.LoadBank("bank:/Monster");
        //FMODUnity.RuntimeManager.LoadBank("bank:/Level");
        //FMODUnity.RuntimeManager.LoadBank("bank:/BOSS");

        FMODUnity.RuntimeManager.WaitForAllSampleLoading();

    }
    private void Start()
    {

        //BGMPathDefinitions[0].set(BGMType.bossLevel, "event:/BOSS/�վֶ�̬����");

        var Path = "event:/BGM/finalBGM";
        var type = BGMType.bossLevel;
        BGMPathDefinitions[0].path = Path;
        BGMPathDefinitions[0].ambientAudioType = type;
        var Path1 = "event:/BGM/normalBGM";
        var type1 = BGMType.normalLevel;
        BGMPathDefinitions[1].ambientAudioType = type1;
        BGMPathDefinitions[1].path = Path1;
        var Path2 = "event:/BGM/level2";
        var type2 = BGMType.normalLevel2;
        BGMPathDefinitions[2].ambientAudioType = type2;
        BGMPathDefinitions[2].path = Path2;

        var Path3 = "event:/BGM/level3";
        var type3 = BGMType.normalLevel3;
        BGMPathDefinitions[3].ambientAudioType = type3;
        BGMPathDefinitions[3].path = Path3;

        var Path4 = "event:/BGM/startBGM";
        var type4 = BGMType.start;
        BGMPathDefinitions[4].ambientAudioType = type4;
        BGMPathDefinitions[4].path = Path4;
        BGMPathDict = new Dictionary<BGMType, string>();

        foreach (var ambientAudioDefinition in BGMPathDefinitions)
        {
            if (BGMPathDict.ContainsKey(ambientAudioDefinition.ambientAudioType))
            {
                continue;
            }
            BGMPathDict.Add(ambientAudioDefinition.ambientAudioType, ambientAudioDefinition.path);
        }

        //PlayBGM(BGMPathDefinitions[0].ambientAudioType);
        //GameManager.Instance.BossDie += BossState_Die;
        //bossControl = GameManager.Instance.currentBoss;

        FmodManager.Instance.PlayBGM(FmodManager.Instance.BGMPathDefinitions[4].ambientAudioType);
    }
    public void BossState_Die()
    {
        print("��Ƶ������ܵ�BOSS����");
        BGMEvent.setParameterByName("bossState", 4);
        currentAmbientAudio = BGMType.defaultType;
        //GameManager.Instance.BossDie -= BossState_Die;

    }
    //public void PlayNormalLevel()
    //{
    //    PlayBGM(BGMPathDefinitions[1].ambientAudioType);
    //}
    public void PlayNormalLevelEnd()
    {
        BGMEvent.setParameterByName("battleState", 1);
        print("����С�ؽ�����Ƶ");
        normalState = 1;
    }
    
    private void Update()
    {
        if (currentAmbientAudio == BGMType.bossLevel)
        {
            if (GameManager.Instance.currentBoss != null)
            {

            bossState = GameManager.Instance.currentBoss.stage;
                if (GameManager.Instance.currentBoss.isDead)
                {
                    bossState = 5;
                    return;

                }
            }

            switch (bossState) { 
                case 1:

                break;
                case 2:
                    //TODO:ȥ���׶�
                    BGMEvent.setParameterByName("bossState",2);
                    break;

                case 3:
                    //TODO:ȥ���׶�
                    BGMEvent.setParameterByName("bossState", 2);

                    break;

                case 4:
                    //TODO:ȥ���׶�
                    BGMEvent.setParameterByName("bossState", 3);

                    break;

                case 5:
                    //TODO:ȥ��
                    BGMEvent.setParameterByName("bossState", 4);

                    break;

                default:
                break;       
            }

            //
        }
    }
    //protected virtual void OnDestroy()
    //{
    //    //StopAndReleaseAll();

    //    if (BGMEvent != null)
    //    {
    //        BGMEvent.stop(STOP_MODE.IMMEDIATE);
    //        BGMEvent.release();
    //    }
    //    _instance = null;
    //}


    public virtual void StopAndReleaseAll()
    {
        foreach (var sound in loopingSoundEvents.Values)
        {
            //if (sound == null)
                //continue;

            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }

        loopingSoundEvents.Clear();
    }


    public virtual void pauseAll()
    {
        pausedSoundEvents = new List<EventInstance>();

        foreach (var loopingInstance in loopingSoundEvents.Values)
        {
            //if (loopingInstance == null)
                //continue;

            PLAYBACK_STATE state;
            loopingInstance.getPlaybackState(out state);
            if (state == PLAYBACK_STATE.PLAYING
                || state == PLAYBACK_STATE.STARTING)
            {
                loopingInstance.setPaused(true);
                pausedSoundEvents.Add(loopingInstance);
            }
        }

        PauseBGM(true);
    }

    public virtual void resumeAll()
    {
        //PauseBGM(!GameController.Instance.MusicEnable);
        PauseBGM(false);


        //if (!GameController.Instance.SFXEnable || pausedSoundEvents == null)
            if (pausedSoundEvents == null)

                if (pausedSoundEvents == null)

                return;

        foreach (var pausedSound in pausedSoundEvents)
        {
            pausedSound.setPaused(false);
        }
    }

    #region BGM

    public virtual void PlayBGM(BGMType type)
    {
        currentAmbientAudio = type;

        //if (GameController.Instance.MusicEnable)
        if (type == BGMType.bossLevel)
        {
            //print("��Ƶ������Ի�ȡ��ǰBOSS");
           // bossControl = GameManager.Instance.currentBoss;

        }
        if (type == BGMType.normalLevel)
        {
            //print("��Ƶ�������С�ر���");

            normalState = 0;
        }
        {
            CheckBGMEventInstance();
        }
    }

    public virtual void PauseBGM(bool pause=true)
    {
        //if (BGMEvent == null)
        //    CheckBGMEventInstance();

        //if (BGMEvent != null)
            BGMEvent.setPaused(pause);
    }
    public void stopBGM()
    {
        
                BGMEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                BGMEvent.release();
    }

    protected void CheckBGMEventInstance()
    {

        if (BGMPathDict.ContainsKey(currentAmbientAudio))
        {
            //ֹ֮ͣǰ��,��ÿ���ͨ�������仯��������?
            //if (BGMEvent != null)
            {
                BGMEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                BGMEvent.release();
            }
            BGMEvent = RuntimeManager.CreateInstance(BGMPathDict[currentAmbientAudio]);
            BGMEvent.start();
        }
    }

    #endregion

    #region SFX

    /// <summary>
    /// ����һ�Σ����ܸı��κβ���
    /// </summary>
    //public virtual void PlaySoundOnce(string sound)
    //{
    //    //if (!GameController.Instance.SFXEnable || string.IsNullOrEmpty(sound))
    //    if (string.IsNullOrEmpty(sound))

    //        return;

    //    if (sound.Length < minPathLength)
    //    {
    //        Debug.Log("Wrong Sound Path:" + sound);
    //        return;
    //    }

    //    RuntimeManager.PlayOneShot(sound);
    //}
    public  void PlaySoundOnce(EventReference sound,float dur=0f)
    {
        //if (!GameController.Instance.SFXEnable || string.IsNullOrEmpty(sound))
        if (sound.IsNull)

            return;
        readyEffectEvent = sound;
        //if (sound.Length < minPathLength)
        //{
        //    Debug.Log("Wrong Sound Path:" + sound);
        //    return;
        //}
        Invoke("truePlaySoundOnce", dur);
        
    }
    private void truePlaySoundOnce()
    {
        RuntimeManager.PlayOneShot(readyEffectEvent);

    }
    /// <summary>
    /// Ŀ��λ�ò���һ��(λ�ò��䣩
    /// </summary>
    public void PlaySoundOnce(EventReference sound, Vector3 position)
    {
        //if (GameController.Instance.SFXEnable)
            RuntimeManager.PlayOneShot(sound, position);
    }

    /// <summary>
    /// ����һ�Σ�λ����attach��(����attach�ƶ�)
    /// </summary>
    public void PlaySoundOnce(EventReference sound, GameObject attach)
    {
        //if (GameController.Instance.SFXEnable)
            RuntimeManager.PlayOneShotAttached(sound, attach);
    }

    /// <summary>
    /// ѭ������,ҪFmod������OneShotΪfalse
    /// </summary>
    public virtual EventInstance PlayLoopSounds(EventReference sound)
    {
        //if (!GameController.Instance.SFXEnable || string.IsNullOrEmpty(sound))
        //if (string.IsNullOrEmpty(sound))

            //return null;

        //if (sound.Length < minPathLength)
        //{
        //    Debug.Log("Wrong Sound Path:" + sound);
        //    return null;
        //}

        if (HavaEventInstance(sound))
        {
            loopingSoundEvents[sound].start();
            return loopingSoundEvents[sound];
        }

        var newInstance = RuntimeManager.CreateInstance(sound);

        newInstance.start();
        loopingSoundEvents[sound] = newInstance;

        return newInstance;
    }
    public virtual EventInstance PlaySpecialSound(string sound)
    {

        //if (HavaEventInstance(sound))
        //{
         //   SpecialSoundEvents[sound].start();
           // return SpecialSoundEvents[sound];
       // }

        var newInstance = RuntimeManager.CreateInstance(sound);

        newInstance.start();
        SpecialSoundEvents[sound] = newInstance;
        

        return newInstance;
    }

    public void PauseSpecialSound(string sound)
    {
        //if (HavaEventInstance(sound))
        //{

            var result = SpecialSoundEvents[sound].setPaused(true);

            //return result == FMOD.RESULT.OK;
        //}
    }
    public void StopSpecialSound(string sound)
    {
        var result = SpecialSoundEvents[sound].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SpecialSoundEvents[sound].release();
        SpecialSoundEvents.Remove(sound);
        //return result == FMOD.RESULT.OK;
    }
    /// <summary>
    /// ��ָͣ��ѭ����Ч
    /// </summary>
    public bool PauseSound(EventReference sound, bool pause = true)
    {
        if (HavaEventInstance(sound))
        {
            //var result = loopingSoundEvents[sound].setPaused(pause && GameController.Instance.SFXEnable);
            var result = loopingSoundEvents[sound].setPaused(pause);

            return result == FMOD.RESULT.OK;
        }

        return false;
    }
    /// <summary>
    /// ָֹͣ��ѭ����Ч�����ͷ�
    /// </summary>
    public bool StopSound(EventReference sound)
    {
        if (HavaEventInstance(sound))
        {
            var result = loopingSoundEvents[sound].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            loopingSoundEvents[sound].release();
            loopingSoundEvents.Remove(sound);
            return result == FMOD.RESULT.OK;
        }

        return false;
    }

    protected bool HavaEventInstance(EventReference sound)
    {
        //return !string.IsNullOrEmpty(sound) && loopingSoundEvents.ContainsKey(sound) && !loopingSoundEvents[sound].IsNull();
        return loopingSoundEvents.ContainsKey(sound);

    }
    #endregion

    //void OnApplicationPause(bool didPause) {
    //	if (didPause) {
    //		pauseAll();
    //	}
    //	else {
    //		resumeAll();
    //	}
    //}

    //void OnApplicationFocus(bool focus)
    //{
    //    if (focus)
    //    {
    //        resumeAll();
    //    }
    //    else
    //    {
    //        pauseAll();
    //    }
    //}
    /// <summary>
    /// range form 0 to 1
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {

        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        //Debug.Log(bus);
        //if (bus != null)
        {
            //bus.setFaderLevel(volume);
            bus.setVolume(volume* allVolumeMul);
            music_volume = volume;
        }
    }

    public void MuteMusicTrigger(bool trigger)
    {
        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Music");

        bus.setMute(trigger);

    }
    /// <summary>
    /// range form 0 to 1
    /// </summary>
    /// <param name="volume"></param>
    public void SetEffectVolume(float volume)
    {

        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Effect");
        //Debug.Log(bus);
        //if (bus != null)
        {
            //bus.setFaderLevel(volume);
            bus.setVolume(volume* allVolumeMul);         
            effect_volume = volume;
        }
    }
    public void MuteEffectTrigger(bool trigger )
    {
        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Effect");

        bus.setMute(trigger);

    }

    public void SetAllVolume(float volume)
    {



        allVolumeMul = volume;
    }
    public float allVolumeMul = 1;
    public void MuteAll(bool trigger)
    {
        MuteEffectTrigger(trigger);
        MuteMusicTrigger(trigger);
    }
}


[Serializable]
public class BGMPathDefinition
{

    public BGMType ambientAudioType;
    //{
    //    set
    //    {
    //        ambientAudioType = value;
    //    }
    //    get
    //    {
    //        return ambientAudioType;
    //    }
    //}

    //public EventReference reference
    //{
    //    get
    //    {
    //        if (reference.IsNull)
    //        {
    //            reference.Path = path;
    //        }
    //        return reference;
    //    }
    //}


    public string path;
    //{
    //    set
    //    {
    //        path = value;
    //    }
    //    get
    //    {
    //        return path;
    //    }
    //}
    
    public void set(BGMType type,string thisPath)
    {
        ambientAudioType = type;
        path = thisPath;
    }

}

public enum BGMType
{
    normalLevel = 0,
    bossLevel = 20,
    specialLevel = 40,
    defaultType=60,
    normalLevel2 = 80,
    normalLevel3 = 100,
    start=120,

}