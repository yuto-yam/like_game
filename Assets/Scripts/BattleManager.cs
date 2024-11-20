using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using UnityEngine;
//using UnityEngine.SceneManagement;



//敵の生成及びバトルを制御するためのスクリプト
public class BattleManager : MonoBehaviour
{
    //プレイヤーのデータを参照する準備
    PlayerDataHolder playerdataholder;
    ParameterController parametercontroller;

    GameObject PlayerWeponObj;
    WeaponController weaponController;

    GameObject BattleText;
    private UnityEngine.UI.Text battletext; //バトル用のテキスト

    ParameterController.Enemy enemy; //エネミー生成のための宣言　ここで宣言しないとUpdateで使えない
    int turn; //ターン数表示用変数
    bool IsWaitingInput = true; //入力待ちのためのbool

    // Start is called before the first frame update
    void Start()
    {
        playerdataholder = GameManager.Instance.GetComponent<PlayerDataHolder>();
        parametercontroller = GameManager.Instance.GetComponent<ParameterController>();

        enemy = new ParameterController.Enemy("enemy", 2, 20, 5);

        UnityEngine.Debug.Log("player's HP: " + playerdataholder.player.GetHP() + ", enemy's HP: " + enemy.GetHP());

        //プレイヤーの武器を取得して位置を調整
        PlayerWeponObj = playerdataholder.WeaponPrefab;
        //PlayerWeponObj.transform.position = new Vector3(2.5f, -2f, 0);
        //PlayerWeponObj.transform.Rotate(0, 180f, 0);
        //PlayerWeponObj = GameManager.WeaponPrefab;

        weaponController = PlayerWeponObj.GetComponent<WeaponController>();

        BattleText = playerdataholder.BattleText;
        battletext = BattleText.GetComponent<UnityEngine.UI.Text>();
        battletext.text = "Test success!";


        turn = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // HPが両方とも正で、入力待ちでない場合
        if (playerdataholder.player.GetHP() > 0 && enemy.GetHP() > 0 && IsWaitingInput)
        {
            UnityEngine.Debug.Log("Turn: " + turn);

            // Aキーが押された場合
            if (Input.GetKeyDown(KeyCode.A)) // 通常攻撃
            {
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv()); // 与ダメージ=攻撃力+Lv
                playerdataholder.player.TakeDamage(enemy.GetATK() + enemy.GetLv()); //被ダメージ=攻撃力+相手Lv-自分のレベル;
                IsWaitingInput = false; // 入力待ちを解除

                StartCoroutine(weaponController.MoveWeaponCoroutine());
            }
            // Sキーが押された場合
            else if (Input.GetKeyDown(KeyCode.S)) // スキル
            {
                enemy.TakeDamage(playerdataholder.player.GetATK() + playerdataholder.player.GetLv() + enemy.GetLv()); // スキル攻撃の場合、さらに敵のレベルを無視
                playerdataholder.player.TakeDamage(enemy.GetATK() + enemy.GetLv()); //被ダメージ=攻撃力+相手Lv-自分のレベル;
                IsWaitingInput = false; // 入力待ちを解除
            }

            // 敵のHPが0以下になった場合
            if (enemy.GetHP() <= 0)
            {
                enemy.YouAreDEAD();
            }
            if (playerdataholder.player.GetHP() <= 0)
            {
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
