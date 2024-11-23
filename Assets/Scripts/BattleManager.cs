using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using UnityEngine;
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
    GameObject BattleText;
    private UnityEngine.UI.Text battletext; 

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

        turn = 1; // ターン数初期化
    }

    // Update is called once per frame
    void Update()
    {
        // HPが両方とも正で、入力待ちでない場合
        if (playerdataholder.player.GetHP() > 0 && enemy.GetHP() > 0 && IsWaitingInput)
        {
            UnityEngine.Debug.Log("Turn: " + turn);
            // 戦闘用処理を走らせる

            // Aキーが押された場合
            if (Input.GetKeyDown(KeyCode.A)) // 通常攻撃
            {
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv()); // 与ダメージ=攻撃力+Lv
                playerdataholder.player.TakeDamage(enemy.GetATK() + enemy.GetLv()); // 被ダメージ=攻撃力+相手Lv-自分のレベル;
                IsWaitingInput = false; // 入力待ちを解除
                // 武器を振る
                StartCoroutine(weaponController.MoveWeaponCoroutine());
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
}
