using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// オーディオマネージャー、すべてのオーディオを保存し、自由に再生や一時停止が可能です。
/// </summary>
public class AudioManager : MonoBehaviour
{
    ///<summary>

    ////// 単一のオーディオ情報の保存

    ///</summary>
    public class Sound
    {
        [Tooltip("オーディオクリップ")]
        public AudioClip clip;
        [Header("オーディオグループ")]
        public AudioMixerGroup outputGroup;
        [Tooltip("オーディオボリューム")]
        [Range(0 ,1)]
        public float volume;
        [Header("オーディオは再生されていますか？")]
        public bool playOnAwake;
        public bool loop;
    }
}
