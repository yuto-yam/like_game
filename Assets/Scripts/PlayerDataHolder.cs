using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

//実際にステータスを管理するクラス
public class PlayerDataHolder : MonoBehaviour
{
    //シングルトンインスタンス
    //public static PlayerDataHolder Instance { get; private set; }

    public ParameterController.Player player;
    public ParameterController.Weapon player_weapon;

    public GameObject WeaponObj; //武器のパターンのリスト(予定)

    // Start is called before the first frame update
    void Start()
    {
        //プレイヤー初期宣言(名前、レベル、HP、攻撃力)
        player = new ParameterController.Player("yu-sya", 1, 50, 10);

        //武器初期宣言(名前、攻撃力、スキル)
        player_weapon = new ParameterController.Weapon("tmp_sword", 10, ParameterController.SKILL.MAGIC);

    }

    //Update is called once per frame
    void Update()
    {
        
    }
}
