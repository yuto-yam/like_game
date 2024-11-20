using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using static System.Net.Mime.MediaTypeNames;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private PlayerDataHolder playerDataHolder;
    public GameObject WeaponPrefab; //画面上での武器
    [SerializeField] GameObject StatusPanel;
    [SerializeField] GameObject StatusText;
    private Text statustext; //ステータス描画用のテキスト


    private void Awake()
    {
        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移でも破棄されないようにする
        }
        else
        {
            Destroy(gameObject); // 複数のインスタンスを防止
        }
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

    private void MazeSceneInit()
    {
        UnityEngine.Debug.Log("迷路用の初期化処理を実行します。");
        playerDataHolder = GetComponent<PlayerDataHolder>();
        if (playerDataHolder == null)
        {
            UnityEngine.Debug.LogError("PlayerDataHolder がアタッチされていません！");
            return; // 処理を中断
        }

        WeaponPrefab = Instantiate(playerDataHolder.WeaponObj, new Vector3(0, 0, 0), Quaternion.identity);
        WeaponPrefab.transform.SetParent(StatusPanel.transform, false);
        WeaponPrefab.transform.localPosition = new Vector3(0, -30f, -1f);
        WeaponPrefab.transform.localScale = new Vector3(250f, 250f, 250f);

        //ステータステキストの初期化
        statustext = StatusText.GetComponent<Text>();
        statustext.text = "Name:" + playerDataHolder.player.Getname() + "  Lv:" + playerDataHolder.player.GetLv() + "\n"
                           + "HP:" + playerDataHolder.player.GetHP() + "  ATK:" + playerDataHolder.player.GetATK() + "\n"
                           + "Weapon:" + playerDataHolder.player_weapon.GetName();
    }

    private void BattleSceneInit()
    {
        UnityEngine.Debug.Log("バトル用の初期化処理を実行します。");
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
