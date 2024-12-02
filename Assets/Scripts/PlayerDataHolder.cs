using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;

// 実際にステータスを管理するクラス
public class PlayerDataHolder : MonoBehaviour
{
    // プレイヤーのデータと武器
    public ParameterDifiner.Player player;
    public ParameterDifiner.Weapon player_weapon;

    // 画面上の武器
    public GameObject TmpWeaponObj; // プレハブ生成元のオブジェクト
    public GameObject WeaponPrefab; // 画面上での武器
    public Renderer WeaponColor;    // 武器の色を指定 

    // ドロップ用
    public ParameterDifiner.Weapon new_weapon;
    public GameObject NewWeaponPrefab;
    private Renderer NewWeaponColor;

    // UI用パネルとテキストの準備
    [SerializeField] GameObject StatusPanel;
    [SerializeField] GameObject StatusText;
    private UnityEngine.UI.Text statustext; //statusの管理は全てここで

    [SerializeField] GameObject GeneralPanel;
    public GameObject GeneralText;          // 他でも触るのでpublic
    private UnityEngine.UI.Text generaltext;

    [SerializeField] GameObject BattlePanel;　// BattleManagerで触る限定
    public GameObject NumText_E; 
    public GameObject NumText_P;

    // Dropはここでやる
    [SerializeField] GameObject DropPanel;
    [SerializeField] GameObject OldDropText;
    private UnityEngine.UI.Text olddroptext;
    [SerializeField] GameObject NewDropText;
    private UnityEngine.UI.Text newdroptext;


    private void Awake()
    {
        //プレイヤー初期宣言(名前、レベル、HP、攻撃力)
        player = new ParameterDifiner.Player("yu-sya", 1, 0, 50, 10);

        //武器初期宣言(名前、攻撃力、スキル)
        player_weapon = new ParameterDifiner.Weapon("tmp_sword", 10, ParameterDifiner.SKILL.MAGIC);
        WeaponPrefab = Instantiate(TmpWeaponObj, new Vector3(0, 0, 0), Quaternion.identity);
        WeaponPrefab.transform.SetParent(StatusPanel.transform, false);
        WeaponPrefab.transform.localScale = new Vector3(250f, 250f, 250f);

        // ステータステキストを取得
        statustext = StatusText.GetComponent<UnityEngine.UI.Text>();
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }
    

    //Update is called once per frame
    void Update()
    {
        //ステータステキストを表示
        statustext.text = string.Format(
            "Name:{0,-5}  Lv:{1,-3}  Exp:{2,-3}\n" +
            "HP:{3,3}/{4,-3}  ATK:{5,-3}\n" +
            "Weapon:{6}",
            player.GetName(),
            player.GetLv(),
            player.GetExp(),
            player.GetHP(),
            player.GetMaxHP(),
            player.GetATK(),
            player_weapon.GetWeaponName()
        );

    }

    private void OnEnable()
    {
        // シーン読み込み完了時に呼び出すイベントを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // イベントの登録解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーン読み込み時に実行する処理
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnityEngine.Debug.Log("シーンが読み込まれました: " + scene.name);

        // シーンごとに異なる処理を実行
        if (scene.name == "Maze")
        {
            MazeSceneInit();
        }
        else if (scene.name == "Battle")
        {
            BattleSceneInit();
        }
        else if (scene.name == "Drop")
        {
            DropSceneInit();
        }
    }

    // Mazeシーン読み込み時にUI系オブジェクトの位置を調整
    private void MazeSceneInit()
    {
        UnityEngine.Debug.Log("迷路用の初期化処理を実行します。");
        // プレイヤーの武器をMaze UIに配置
        WeaponPrefab.transform.localPosition = new Vector3(0, -30f, -1f);

        // 使わないUIを非表示
        GeneralPanel.SetActive(false);
        BattlePanel.SetActive(false);
        DropPanel.SetActive(false);
    }

    private void BattleSceneInit()
    {
        UnityEngine.Debug.Log("バトル用の初期化処理を実行します。");
        //プレイヤーの武器を取得して位置を調整
        WeaponPrefab.transform.position = new Vector3(2.5f, -2f, 0);
        WeaponPrefab.transform.Rotate(0, 180f, 0);

        // UIの表示非表示切り替え
        GeneralPanel.SetActive(true);
        BattlePanel.SetActive(true);
        DropPanel.SetActive(false);

    }

    private void DropSceneInit()
    {
        UnityEngine.Debug.Log("ドロップ用の初期化処理を実行します。");

        // UIの表示非表示切り替え
        GeneralPanel.SetActive(true);
        BattlePanel.SetActive(false);
        DropPanel.SetActive(true);

        // 新しい武器を生成
        new_weapon = new ParameterDifiner.Weapon("new_sword", 15, ParameterDifiner.SKILL.HEAL);
        NewWeaponPrefab = Instantiate(TmpWeaponObj, new Vector3(0, 0, 0), Quaternion.identity);

        NewWeaponPrefab.transform.SetParent(StatusPanel.transform, false);
        NewWeaponPrefab.transform.localScale = new Vector3(250f, 250f, 250f);
        NewWeaponPrefab.transform.position = new Vector3(4f, 1f, 0);
        WeaponColor = NewWeaponPrefab.GetComponent<Renderer>();
        WeaponColor.material.color = Color.red;

        newdroptext = NewDropText.GetComponent<UnityEngine.UI.Text>();
        newdroptext.text = "名前：" + new_weapon.GetWeaponName() + "\n" + "ATK:" + new_weapon.GetWeaponATK() + "\n" + "SKILL:" + new_weapon.GetWeaponSKILL();

        //プレイヤーの武器を取得して位置を調整
        WeaponPrefab.transform.position = new Vector3(-4f, 1f, 0);

        olddroptext = OldDropText.GetComponent<UnityEngine.UI.Text>();
        olddroptext.text = "名前：" + player_weapon.GetWeaponName() + "\n" + "ATK:" + player_weapon.GetWeaponATK() + "\n" + "SKILL:" + player_weapon.GetWeaponSKILL();

        generaltext = GeneralText.GetComponent<UnityEngine.UI.Text>();
        generaltext.text = "新しい武器を入手しました！ 武器を変えますか？" + "         " + "Y/N";
    }
}
