using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


// SEを再生するスクリプト(Prefabから音を鳴らしたりする関係上、専用のオブジェクトではなくGameManagerにアタッチ)
public class SoundEffectPlayer : MonoBehaviour
{
    public AudioSource audioSource;   // SEを再生するAudioSource
    public AudioClip[] weaponSEClips; // 武器SEのリスト
    public AudioClip[] ExweaponSEClips; // その他の武器系SEリスト
    // 0→剣のミス、1→大剣、2→魔法

    public AudioClip DangerSE;
    public AudioClip DecideSE;
    public AudioClip EncountSE;
    public AudioClip EquipChangeSE;
    public AudioClip HealSE;
    public AudioClip LvUpSE;
    public AudioClip MoveCancelSE;
    public AudioClip StairDownSE;

    
    // 汎用的なSE再生メソッド
    public void PlaySE(AudioClip clip)
    {
        if (clip == null)
        {
            UnityEngine.Debug.LogWarning("AudioClip is null and cannot be played.");
            return;
        }

        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource is not assigned.");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void WeaponSEPlay(int weapon_number)
    {
        if (weapon_number < 0 || weapon_number >= weaponSEClips.Length)
        {
            UnityEngine.Debug.LogWarning("Invalid weapon_number: " + weapon_number);
            return;
        }

        PlaySE(weaponSEClips[weapon_number]);
    }

    public void ExtraWeaponSEplay(int extra_weapon_number)
    {
        if (extra_weapon_number < 0 || extra_weapon_number >= ExweaponSEClips.Length)
        {
            UnityEngine.Debug.LogWarning("Invalid extra_weapon_number: " + extra_weapon_number);
            return;
        }

        PlaySE(ExweaponSEClips[extra_weapon_number]);
    }

    public void DangerSEPlay()
    {
        PlaySE(DangerSE);
    }

    public void DecideSEPlay()
    {
        PlaySE(DecideSE);
    }

    public void EncountSEPlay()
    {
        PlaySE(EncountSE);
    }

    public void EquipChangeSEPlay()
    {
        PlaySE(EquipChangeSE);
    }

    public void HealSEPlay()
    {
        PlaySE(HealSE);
    }

    public void LvUpSEPlay()
    {
        PlaySE(LvUpSE);
    }

    public void MoveCancelSEPlay()
    {
        PlaySE(MoveCancelSE);
    }

    public void StairDownSEPlay()
    {
        PlaySE(StairDownSE);
    }
}
