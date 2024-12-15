using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.TimeZoneInfo;

// 便利系関数をまとめて記述する
public class UtilFunctions : MonoBehaviour
{
    public void SetObjectColor(GameObject obj, Color newColor)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = newColor; // 引数で渡された色に変更
        }
        else
        {
            UnityEngine.Debug.LogWarning("オブジェクトにRendererがありません。");
        }
    }

    // 武器のステータスを乱数で生成する関数
    public int[] AutoWeaponStatusGenerater(int lv)
    {
        int[] w_status = new int[4];
        PlayerDataHolder playerDataHolder = GetComponent<PlayerDataHolder>();

        w_status[0] = lv * 2 + UnityEngine.Random.Range(-lv / 2, lv / 2);                                   // 攻撃力
        w_status[1] = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ParameterDifiner.SKILL)).Length);   // スキル
        w_status[2] = UnityEngine.Random.Range(0, playerDataHolder.WeaponSpriteList.Count);                 // 画像
        w_status[3] = UnityEngine.Random.Range(0, ParameterDifiner.ColorPalette.GetTotalColors());          // 色

        return w_status;
    }

    // 武器の名前をステータスから決定する関数
    public string GetWeaponName(int[] wstatus)
    {
        string skillName = wstatus[1] switch
        {
            (int)ParameterDifiner.SKILL.MAGIC => "迸る",
            (int)ParameterDifiner.SKILL.HEAL => "聖なる",
            _ => "無の",
        };

        // 一旦決め打ちで
        string weaponName = wstatus[2] switch
        {
            0 => "剣",
            1 => "斧",
            2 => "槍",
            3 => "槌",
            _ => "手",
        };

        string colorName = ParameterDifiner.ColorPalette.ColorToName(wstatus[3]);

        return $"{skillName}{colorName}{weaponName}";
    }

}
