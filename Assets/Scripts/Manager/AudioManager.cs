using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频管理器，存储所有音频并且可以随意播放和暂停
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// 单个音频信息存储
    /// </summary>
    public class Sound
    {
        [Tooltip("音频剪辑")]
        public AudioClip clip;
        [Header("音频分组")]
        public AudioMixerGroup outputGroup;
        [Tooltip("音频音量")]
        [Range(0 ,1)]
        public float volume;
        [Header("音频是否开具播放")]
        public bool playOnAwake;
        public bool loop;
    }
}
