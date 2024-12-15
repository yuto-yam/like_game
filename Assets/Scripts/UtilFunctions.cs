using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.TimeZoneInfo;

// �֗��n�֐����܂Ƃ߂ċL�q����
public class UtilFunctions : MonoBehaviour
{
    public void SetObjectColor(GameObject obj, Color newColor)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = newColor; // �����œn���ꂽ�F�ɕύX
        }
        else
        {
            UnityEngine.Debug.LogWarning("�I�u�W�F�N�g��Renderer������܂���B");
        }
    }

    // ����̃X�e�[�^�X�𗐐��Ő�������֐�
    public int[] AutoWeaponStatusGenerater(int lv)
    {
        int[] w_status = new int[4];
        PlayerDataHolder playerDataHolder = GetComponent<PlayerDataHolder>();

        w_status[0] = lv * 2 + UnityEngine.Random.Range(-lv / 2, lv / 2);                                   // �U����
        w_status[1] = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ParameterDifiner.SKILL)).Length);   // �X�L��
        w_status[2] = UnityEngine.Random.Range(0, playerDataHolder.WeaponSpriteList.Count);                 // �摜
        w_status[3] = UnityEngine.Random.Range(0, ParameterDifiner.ColorPalette.GetTotalColors());          // �F

        return w_status;
    }

    // ����̖��O���X�e�[�^�X���猈�肷��֐�
    public string GetWeaponName(int[] wstatus)
    {
        string skillName = wstatus[1] switch
        {
            (int)ParameterDifiner.SKILL.MAGIC => "瞂�",
            (int)ParameterDifiner.SKILL.HEAL => "���Ȃ�",
            _ => "����",
        };

        // ��U���ߑł���
        string weaponName = wstatus[2] switch
        {
            0 => "��",
            1 => "��",
            2 => "��",
            3 => "��",
            _ => "��",
        };

        string colorName = ParameterDifiner.ColorPalette.ColorToName(wstatus[3]);

        return $"{skillName}{colorName}{weaponName}";
    }

}
