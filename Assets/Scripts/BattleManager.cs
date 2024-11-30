using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



// 敵の生成及びバトルを制御するためのスクリプト
public class BattleManager : MonoBehaviour
{
    // プレイヤーのデータを参照する準備
    PlayerDataHolder playerdataholder;
    ParameterController parametercontroller;
    // 武器を参照する準備
    GameObject PlayerWeponObj;
    WeaponController weaponController;
    // バトル用テキスト
    private GameObject BattleText;
    private UnityEngine.UI.Text battletext;
    // ダメージ、回復、レベルアップを表示するテキストを用意
    [SerializeField] GameObject NumText_Enemy;
    private UnityEngine.UI.Text numtext_enemy;

    [SerializeField] GameObject NumText_Player;
    private UnityEngine.UI.Text numtext_player;

    private Coroutine cuurentCoroutine; // 表示テキスト管理用コルーチン

    ParameterController.Enemy enemy; // エネミー生成のための宣言　ここで宣言しないとUpdateで使えない
    int turn; // ターン数
    bool IsWaitingInput = true; // 入力待ちのためのbool

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーデータの取得
        playerdataholder = GameManager.Instance.GetComponent<PlayerDataHolder>();
        parametercontroller = GameManager.Instance.GetComponent<ParameterController>();
        // 敵を生成
        enemy = new ParameterController.Enemy("enemy", 2, 20, 5);
        UnityEngine.Debug.Log("player's HP: " + playerdataholder.player.GetHP() + ", enemy's HP: " + enemy.GetHP());
        // プレイヤーの武器を取得
        PlayerWeponObj = playerdataholder.WeaponPrefab;
        // 武器を動かすスクリプトを取得
        weaponController = PlayerWeponObj.GetComponent<WeaponController>();
        // テキスト表示の準備
        BattleText = playerdataholder.BattleText;
        battletext = BattleText.GetComponent<UnityEngine.UI.Text>();
        battletext.text = "Test success!";

        NumText_Enemy = playerdataholder.NumText_E;
        numtext_enemy = playerdataholder.NumText_E.GetComponent<UnityEngine.UI.Text>();
        numtext_enemy.text = "0!";
        NumText_Player = playerdataholder.NumText_P;
        numtext_player = playerdataholder.NumText_P.GetComponent<UnityEngine.UI.Text>();
        numtext_player.text = "0!";
        

        turn = 1; // ターン数初期化
        // バトル開始
        StartCoroutine(BattleTurnCoroutine());
    }

    /*
    // Update is called once per frame
    void Update()
    {
        // HPが両方とも正で、入力待ちでない場合
        if (playerdataholder.player.GetHP() > 0 && enemy.GetHP() > 0 && IsWaitingInput)
        {
            UnityEngine.Debug.Log("Turn: " + turn);
            battletext.text = "ターン" + turn + " " + playerdataholder.player.Getname() + " はどうする？" + " A:攻撃/S:スキル";
            // 戦闘用処理を走らせる

            // Aキーが押された場合
            if (Input.GetKeyDown(KeyCode.A)) // 通常攻撃
            {
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv()); // 与ダメージ=攻撃力+Lv
                playerdataholder.player.TakeDamage(enemy.GetATK() + enemy.GetLv()); // 被ダメージ=攻撃力+相手Lv-自分のレベル;
                // 武器を振る
                StartCoroutine(weaponController.MoveWeaponCoroutine());
                // テキストとダメージ表示
                DisplayText(battletext, playerdataholder.player.Getname() + "の攻撃！", 120f);

                IsWaitingInput = false; // 入力待ちを解除
            }
            // Sキーが押された場合
            else if (Input.GetKeyDown(KeyCode.S)) // スキル
            {
                // ここに回復スキルを実装
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv() + enemy.GetLv()); // スキル攻撃の場合、さらに敵のレベルを無視
                playerdataholder.player.TakeDamage(enemy.GetATK() + enemy.GetLv()); // 被ダメージ=攻撃力+相手Lv-自分のレベル;
                IsWaitingInput = false; // 入力待ちを解除
            }

            // 敵のHPが0以下になった場合
            if (enemy.GetHP() <= 0)
            {
                // 戻ったときに前の場所に戻れるように
                parametercontroller.MoveFromScene = SceneManager.GetActiveScene().name;
                // 勝利回数を数えておく
                parametercontroller.BattleCount += 1;
                // 敵が倒れたときの処理
                enemy.YouAreDEAD();
            }
            if (playerdataholder.player.GetHP() <= 0)
            {
                // GameOverの処理へ
                playerdataholder.player.YouAreDEAD();
            }
        }
        else if (!IsWaitingInput) // 入力待ちが解除されている場合
        {
            turn++; // ターンを進める
            IsWaitingInput = true; // 次のターンのために入力待ちに戻す
        }
    }
    */

    // ターン管理用コルーチン
    private IEnumerator BattleTurnCoroutine()
    {
        while (playerdataholder.player.GetHP() > 0 && enemy.GetHP() > 0)
        {
            // プレイヤーターンの開始
            yield return StartCoroutine(PlayerTurn());

            // 敵が倒れた場合は終了
            if (enemy.GetHP() <= 0) break;

            // 敵ターンの処理（例として簡略化）
            yield return StartCoroutine(EnemyTurn());

            // プレイヤーが倒れた場合は終了
            if (playerdataholder.player.GetHP() <= 0) break;

            // ターンを進める
            turn++;
        }

        // 結果の処理
        if (playerdataholder.player.GetHP() <= 0)
        {
            battletext.text = "Game Over!";
            playerdataholder.player.YouAreDEAD();
        }
        else if (enemy.GetHP() <= 0)
        {
            battletext.text = "Victory!";
            enemy.YouAreDEAD();
        }
    }

    // プレイヤーターン処理
    private IEnumerator PlayerTurn()
    {
        battletext.text = $"ターン {turn}: {playerdataholder.player.Getname()} はどうする？ A:攻撃 / S:スキル";
        yield return new WaitForSeconds(1f); // 表示時間調整

        bool actionTaken = false;
        while (!actionTaken)
        {
            if (Input.GetKeyDown(KeyCode.A)) // 通常攻撃
            {
                // 攻撃処理
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv());
                playerdataholder.player.TakeDamage(enemy.GetATK() + enemy.GetLv());
                battletext.text = $"{playerdataholder.player.Getname()} の攻撃！";
                StartCoroutine(weaponController.MoveWeaponCoroutine());
                yield return new WaitForSeconds(1f);
                actionTaken = true;
            }
            else if (Input.GetKeyDown(KeyCode.S)) // スキル攻撃
            {
                // スキル処理
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv() + enemy.GetLv());
                battletext.text = $"{playerdataholder.player.Getname()} のスキル攻撃！";
                yield return new WaitForSeconds(1f);
                actionTaken = true;
            }

            yield return null; // 入力待機
        }
    }

    // 敵ターン処理
    private IEnumerator EnemyTurn()
    {
        battletext.text = "敵の攻撃！";
        yield return new WaitForSeconds(1f);

        playerdataholder.player.TakeDamage(enemy.GetATK());
        yield return new WaitForSeconds(1f);
    }


    public void DisplayText (UnityEngine.UI.Text uitext, string messeage, float duration = 1f)
    {
        // 表示しているものを停止
        if (cuurentCoroutine != null)
        {
            StopCoroutine(cuurentCoroutine); 
        }
        // 新しく表示
        cuurentCoroutine = StartCoroutine(DisplayTextCoroutine(uitext, messeage, duration));
    }

    private IEnumerator DisplayTextCoroutine(UnityEngine.UI.Text uitext, string messeage, float duration)
    {
        uitext.text = messeage;
        uitext.enabled = true;

        yield return new WaitForSeconds(duration);

        uitext.enabled = false;
    }
}
