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
    // プレイヤーのデータと武器を初期化
    public ParameterController.Player player;
    public ParameterController.Weapon player_weapon;

    // 画面上で武器を描画するための変数
    public GameObject WeaponObj; //武器のパターンのリスト(予定)
    public GameObject WeaponPrefab; // 画面上での武器

    // UI用パネルとテキストの準備
    [SerializeField] GameObject StatusPanel;
    [SerializeField] GameObject StatusText;
    private UnityEngine.UI.Text statustext;

    [SerializeField] GameObject BattlePanel;
    public GameObject BattleText;


    // Start is called before the first frame update
    void Start()
    {
        //プレイヤー初期宣言(名前、レベル、HP、攻撃力)
        player = new ParameterController.Player("yu-sya", 1, 50, 10);

        //武器初期宣言(名前、攻撃力、スキル)
        player_weapon = new ParameterController.Weapon("tmp_sword", 10, ParameterController.SKILL.MAGIC);
        WeaponPrefab = Instantiate(WeaponObj, new Vector3(0, 0, 0), Quaternion.identity);

    }

    //Update is called once per frame
    void Update()
    {
        //ステータステキストを表示
        statustext = StatusText.GetComponent<UnityEngine.UI.Text>();
        statustext.text = "Name:" + player.Getname() + "  Lv:" + player.GetLv() + "\n"
                           + "HP:" + player.GetHP() + "  ATK:" + player.GetATK() + "\n"
                           + "Weapon:" + player_weapon.GetName();
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
    }

    private IEnumerator InitializeMazeScene()
    {
        yield return null; // シーンロード後1フレーム待つ
        MazeSceneInit();
    }

    private IEnumerator InitializeBattleScene()
    {
        yield return null; // シーンロード後1フレーム待つ
        BattleSceneInit();
    }

    // Mazeシーン読み込み時にUI系オブジェクトの位置を調整
    private void MazeSceneInit()
    {
        UnityEngine.Debug.Log("迷路用の初期化処理を実行します。");
        // プレイヤーの武器をMaze UIに配置
        WeaponPrefab.transform.SetParent(StatusPanel.transform, false);
        WeaponPrefab.transform.localPosition = new Vector3(0, -30f, -1f);
        WeaponPrefab.transform.localScale = new Vector3(250f, 250f, 250f);
        WeaponPrefab.transform.Rotate(0, -180f, 0);

        BattlePanel.SetActive(false);
    }

    private void BattleSceneInit()
    {
        UnityEngine.Debug.Log("バトル用の初期化処理を実行します。");
        //プレイヤーの武器を取得して位置を調整
        WeaponPrefab.transform.position = new Vector3(2.5f, -2f, 0);
        WeaponPrefab.transform.Rotate(0, 180f, 0);

        BattlePanel.SetActive(true);

    }
}
