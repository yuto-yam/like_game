﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Specialized;


// 敵の生成及びバトルを制御するためのスクリプト
public class BattleManager : MonoBehaviour
{   
    // プレイヤーのデータを参照する準備
    PlayerDataHolder playerdataholder;
    ParameterDifiner parameterdifiner;
    // 武器を参照する準備
    GameObject PlayerWeponObj;
    WeaponController weaponcontroller_p, weaponcontroller_e;

    // バトル用テキスト
    private UnityEngine.UI.Text generaltext;
    private UnityEngine.UI.Text numtext_e;
    private UnityEngine.UI.Text numtext_p;

    private Coroutine cuurentCoroutine; // 表示テキスト管理用コルーチン

    ParameterDifiner.Enemy enemy; // 敵
    [SerializeField] List<Sprite> EnemySpriteList; // 敵の画像リスト
    [SerializeField] GameObject EnemyObject;
    private Sprite enemysprite;
    private SpriteRenderer spriterenderer;
    private GameObject EnemyWeaponPrefab; // 敵の武器 

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

        int randomIndex = UnityEngine.Random.Range(0, EnemySpriteList.Count);
        enemysprite = EnemySpriteList[randomIndex];
        spriterenderer = EnemyObject.GetComponent<SpriteRenderer>();
        spriterenderer.sprite = enemysprite;
        EnemyObject.transform.position = new Vector3 (-5, 0, 0);

        // 武器を取得
        PlayerWeponObj = playerdataholder.WeaponPrefab;
        weaponcontroller_p = PlayerWeponObj.GetComponent<WeaponController>();

        EnemyWeaponPrefab = Instantiate(playerdataholder.TmpWeaponObj, new Vector3(-2.5f, -2f, 0), Quaternion.identity);
        weaponcontroller_e = EnemyWeaponPrefab.GetComponent<WeaponController>();

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
                StartCoroutine(weaponcontroller_p.MoveWeaponCoroutine(new Vector3(2.5f, -2f, 0), true));
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
        StartCoroutine(weaponcontroller_e.MoveWeaponCoroutine(new Vector3(-2.5f, -2f, 0), false));
        yield return new WaitForSeconds(0.5f);
        numtext_p.text = "";
    }
}
