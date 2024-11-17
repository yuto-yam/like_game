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
    public static PlayerDataHolder Instance { get; private set; }

    public ParameterController.Player player;
    public ParameterController.Weapon player_weapon;

    [SerializeField] GameObject WeaponObj; //武器のパターンのリスト(予定)
    public GameObject WeaponPrefab; //画面上での武器

    [SerializeField] GameObject StatusPanel;
    [SerializeField] GameObject StatusText;
    private Text statustext; //ステータス描画用のテキスト

    private void Awake()
    {
        //シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //シーン間で破棄されないようにする
        }
        else
        {
            Destroy(gameObject); //複数のインスタンスを防止
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //プレイヤー初期宣言(名前、レベル、HP、攻撃力)
        player = new ParameterController.Player("yu-sya", 1, 50, 10);

        //武器初期宣言(名前、攻撃力、スキル)
        player_weapon = new ParameterController.Weapon("tmp_sword", 10, ParameterController.SKILL.MAGIC);
        WeaponPrefab = Instantiate(WeaponObj, new Vector3(0, 0, 0), Quaternion.identity);

        //武器の設定
        WeaponPrefab.transform.SetParent(StatusPanel.transform, false);
        WeaponPrefab.transform.localPosition = new Vector3(0, -30f, -1f);
        WeaponPrefab.transform.localScale = new Vector3(250f, 250f, 250f);

        //ステータステキストの初期化
        statustext = StatusText.GetComponent<Text>();
    }

    //Update is called once per frame
    void Update()
    {
        statustext.text = "Name:" + player.Getname() + "  Lv:" + player.GetLv() + "\n"
                            + "HP:" + player.GetHP() + "  ATK:" + player.GetATK() + "\n\n"
                            + "Weapon:" + player_weapon.GetName();
    }
}
