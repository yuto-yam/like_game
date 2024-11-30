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
    public ParameterController.Player player;
    public ParameterController.Weapon player_weapon;

    // 画面上の武器
    public GameObject WeaponObj; //武器の候補リスト(予定)
    public GameObject WeaponPrefab; // 画面上での武器

    // ドロップ用
    public ParameterController.Weapon new_weapon;
    public GameObject NewWeaponPrefab;
    private Renderer NewWeaponColor;

    [SerializeField] GameObject DropPanel;
    [SerializeField] GameObject OldDropText;
    private UnityEngine.UI.Text olddroptext;
    [SerializeField] GameObject NewDropText;
    private UnityEngine.UI.Text newdroptext;

    // UI用パネルとテキストの準備
    [SerializeField] GameObject StatusPanel;
    [SerializeField] GameObject StatusText;
    private UnityEngine.UI.Text statustext;

    [SerializeField] GameObject BattlePanel;
    public GameObject BattleText;
    private UnityEngine.UI.Text battletext;

    public GameObject NumText_E;
    private UnityEngine.UI.Text numtext_E;

    public GameObject NumText_P;
    private UnityEngine.UI.Text numtext_P;


    // Start is called before the first frame update
    void Start()
    {
        //プレイヤー初期宣言(名前、レベル、HP、攻撃力)
        player = new ParameterController.Player("yu-sya", 1, 50, 10);

        //武器初期宣言(名前、攻撃力、スキル)
        player_weapon = new ParameterController.Weapon("tmp_sword", 10, ParameterController.SKILL.MAGIC);
        WeaponPrefab = Instantiate(WeaponObj, new Vector3(0, 0, 0), Quaternion.identity);
        WeaponPrefab.transform.SetParent(StatusPanel.transform, false);
        WeaponPrefab.transform.localScale = new Vector3(250f, 250f, 250f);
        UnityEngine.Debug.Log("Start Done");

    }

    //Update is called once per frame
    void Update()
    {
        //ステータステキストを表示
        statustext = StatusText.GetComponent<UnityEngine.UI.Text>();
        statustext.text = "Name:" + player.Getname() + "  Lv:" + player.GetLv() + "\n"
                           + "HP:" + player.GetHP() + "/" + player.GetMaxHP() + "  ATK:" + player.GetATK() + "\n"
                           + "Weapon:" + player_weapon.GetWeaponName();

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
            StartCoroutine(InitializeMazeScene());
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

    private IEnumerator InitializeMazeScene()
    {
        yield return null; // シーンロード後1フレーム待つ
        MazeSceneInit();
    }

    private IEnumerator InitializeBattleScene()
    {
        yield return null; // シーンロード後1フレーム待つ
        //yield return new WaitForSeconds(3f); // シーンロード後3秒待つ
        BattleSceneInit();
    }

    private IEnumerator InitializeDropScene()
    {
        yield return null; // シーンロード後1フレーム待つ
        DropSceneInit();
    }

    // Mazeシーン読み込み時にUI系オブジェクトの位置を調整
    private void MazeSceneInit()
    {
        UnityEngine.Debug.Log("迷路用の初期化処理を実行します。");
        // プレイヤーの武器をMaze UIに配置
        WeaponPrefab.transform.localPosition = new Vector3(0, -30f, -1f);
        //WeaponPrefab.transform.Rotate(0, -180f, 0);

        BattlePanel.SetActive(false);
        DropPanel.SetActive(false);
    }

    private void BattleSceneInit()
    {
        UnityEngine.Debug.Log("バトル用の初期化処理を実行します。");
        //プレイヤーの武器を取得して位置を調整
        WeaponPrefab.transform.position = new Vector3(2.5f, -2f, 0);
        WeaponPrefab.transform.Rotate(0, 180f, 0);

        BattlePanel.SetActive(true);
        DropPanel.SetActive(false);

    }

    private void DropSceneInit()
    {
        UnityEngine.Debug.Log("ドロップ用の初期化処理を実行します。");
        DropPanel.SetActive(true);

        // 新しい武器を生成
        new_weapon = new ParameterController.Weapon("new_sword", 15, ParameterController.SKILL.HEAL);
        NewWeaponPrefab = Instantiate(WeaponObj, new Vector3(0, 0, 0), Quaternion.identity);

        NewWeaponPrefab.transform.SetParent(StatusPanel.transform, false);
        NewWeaponPrefab.transform.localScale = new Vector3(250f, 250f, 250f);
        NewWeaponPrefab.transform.position = new Vector3(4f, 1f, 0);
        NewWeaponColor = NewWeaponPrefab.GetComponent<Renderer>();
        NewWeaponColor.material.color = Color.red;

        newdroptext = NewDropText.GetComponent<UnityEngine.UI.Text>();
        newdroptext.text = "名前：" + new_weapon.GetWeaponName() + "\n" + "ATK:" + new_weapon.GetWeaponATK() + "\n" + "SKILL:" + new_weapon.GetWeaponSKILL();

        //プレイヤーの武器を取得して位置を調整
        WeaponPrefab.transform.position = new Vector3(-4f, 1f, 0);

        olddroptext = OldDropText.GetComponent<UnityEngine.UI.Text>();
        olddroptext.text = "名前：" + player_weapon.GetWeaponName() + "\n" + "ATK:" + player_weapon.GetWeaponATK() + "\n" + "SKILL:" + player_weapon.GetWeaponSKILL();

        battletext = BattleText.GetComponent<UnityEngine.UI.Text>();
        battletext.text = "新しい武器を入手しました！ 武器を変えますか？" + "         " + "Y/N";
    }
}
