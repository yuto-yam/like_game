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
    ParameterDifiner parameterdifiner;
    // 武器を参照する準備
    GameObject PlayerWeponObj;
    WeaponController weaponController;

    // バトル用テキスト
    private UnityEngine.UI.Text generaltext;
    private UnityEngine.UI.Text numtext_e;
    private UnityEngine.UI.Text numtext_p;

    private Coroutine cuurentCoroutine; // 表示テキスト管理用コルーチン

    ParameterDifiner.Enemy enemy; // エネミー生成のための宣言　ここで宣言しないとUpdateで使えない

    private int turn; // ターン数
    private int damage; // damageを保持する


    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーデータの取得
        playerdataholder = GameManager.Instance.GetComponent<PlayerDataHolder>();
        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();

        // 敵を生成
        enemy = new ParameterDifiner.Enemy("enemy", 2, 20, 5, parameterdifiner);
        UnityEngine.Debug.Log("player's HP: " + playerdataholder.player.GetHP() + ", enemy's HP: " + enemy.GetHP());
        // プレイヤーの武器を取得
        PlayerWeponObj = playerdataholder.WeaponPrefab;
        // 武器を動かすスクリプトを取得
        weaponController = PlayerWeponObj.GetComponent<WeaponController>();

        // テキスト表示の準備
        generaltext = playerdataholder.GeneralText.GetComponent<UnityEngine.UI.Text>();
        generaltext.text = "Test success!";
        numtext_e = playerdataholder.NumText_E.GetComponent<UnityEngine.UI.Text>();
        numtext_e.text = "0!";
        numtext_p = playerdataholder.NumText_P.GetComponent<UnityEngine.UI.Text>();
        numtext_p.text = "0!";
        

        turn = 1; // ターン数初期化
        StartCoroutine(BattleTurnCoroutine()); // バトル開始
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

            // 敵ターンの処理
            yield return StartCoroutine(EnemyTurn());

            // プレイヤーが倒れた場合は終了
            if (playerdataholder.player.GetHP() <= 0) break;

            // ターンを進める
            turn++;
        }

        // 結果の処理
        if (playerdataholder.player.GetHP() <= 0)
        {
            generaltext.text = "負けてしまった！";
            playerdataholder.player.YouAreDEAD();
        }
        else if (enemy.GetHP() <= 0)
        {
            generaltext.text = enemy.GetName() + "を倒した!";
            enemy.YouAreDEAD();
        }
    }

    // プレイヤーターン処理
    private IEnumerator PlayerTurn()
    {
        generaltext.text = $"ターン {turn}: {playerdataholder.player.GetName()} はどうする？    A:攻撃/S:スキル";
        yield return new WaitForSeconds(0.2f); // 表示時間調整

        bool IsWaiting = false; // 戦闘用フラグ
        while (!IsWaiting)
        {
            if (Input.GetKeyDown(KeyCode.A)) // 通常攻撃
            {
                // 攻撃処理
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv());
                damage = playerdataholder.player.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv());
                generaltext.text = $"{playerdataholder.player.GetName()} の攻撃！";
                numtext_e.text = (-damage).ToString();
                numtext_e.color = Color.red;
                StartCoroutine(weaponController.MoveWeaponCoroutine());
                yield return new WaitForSeconds(0.5f);
                numtext_e.text = "";
                IsWaiting = true;
            }
            else if (Input.GetKeyDown(KeyCode.S)) // スキル攻撃
            {
                // スキル処理
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv() + enemy.GetLv());
                generaltext.text = $"{playerdataholder.player.GetName()} のスキル攻撃！";
                yield return new WaitForSeconds(0.5f);
                IsWaiting = true;
            }

            yield return null; // 入力待機
        }
    }

    // 敵ターン処理
    private IEnumerator EnemyTurn()
    {
        generaltext.text = "敵の攻撃！";
        yield return new WaitForSeconds(1f);

        damage = playerdataholder.player.TakeDamage(enemy.GetATK());
        numtext_p.text = (-damage).ToString();
        numtext_p.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        numtext_p.text = "";
    }

    /*
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
    */
}
