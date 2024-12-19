using System;
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

    ParameterDifiner.Enemy enemy; // 敵のクラス
    [SerializeField] int[] e_status; // 敵のステータスをランダムで作る
    [SerializeField] GameObject EnemyObject; // 敵のオブジェクト
    private Sprite enemysprite; // 敵の画像
    private SpriteRenderer spriterenderer;
    private GameObject EnemyWeaponPrefab; // 敵の武器
    private Sprite EnemyWeaponSprite; // 武器の画像

    private int turn, damage, heal; //各種数字


    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーデータの取得
        playerdataholder = GameManager.Instance.GetComponent<PlayerDataHolder>();
        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();

        // 敵を生成
        // enemy = new ParameterDifiner.Enemy("enemy", 2, 20, 5, parameterdifiner);
        e_status = AutoEnemyGenerater(playerdataholder.player.Lv, parameterdifiner.IsBossBattle);
        enemy = new ParameterDifiner.Enemy(enemies[e_status[0]].Name, e_status[1], e_status[2], e_status[3], parameterdifiner);
        UnityEngine.Debug.Log("player's HP: " + playerdataholder.player.HP + ", enemy's HP: " + enemy.HP);

        enemysprite = EnemySpriteList[e_status[0]];
        spriterenderer = EnemyObject.GetComponent<SpriteRenderer>();
        spriterenderer.sprite = enemysprite;
        EnemyObject.transform.position = new Vector3 (-5, 0, 0);

        // 武器を取得
        PlayerWeponObj = playerdataholder.WeaponPrefab;
        weaponcontroller_p = PlayerWeponObj.GetComponent<WeaponController>();

        EnemyWeaponPrefab = Instantiate(playerdataholder.TmpWeaponObj, new Vector3(-2.5f, -2f, 0), Quaternion.identity);
        if (e_status[4] >= 0)
        {
            EnemyWeaponSprite = playerdataholder.WeaponSpriteList[e_status[4]];
            spriterenderer = EnemyWeaponPrefab.GetComponent<SpriteRenderer>();
            spriterenderer.sprite = EnemyWeaponSprite;
        }
        else
        {
            EnemyWeaponPrefab.SetActive(false);
        }
        weaponcontroller_e = EnemyWeaponPrefab.GetComponent<WeaponController>();

        // テキスト表示の準備
        generaltext = playerdataholder.GeneralText.GetComponent<UnityEngine.UI.Text>();
        generaltext.text = "Test success!";
        numtext_e = playerdataholder.NumText_E.GetComponent<UnityEngine.UI.Text>();
        numtext_e.text = "";
        numtext_p = playerdataholder.NumText_P.GetComponent<UnityEngine.UI.Text>();
        numtext_p.text = "";
        

        turn = 1; // ターン数初期化
        StartCoroutine(BattleTurnCoroutine()); // バトル開始
    }

    // ターン管理用コルーチン
    private IEnumerator BattleTurnCoroutine()
    {
        while (playerdataholder.player.HP > 0 && enemy.HP > 0)
        {
            // プレイヤーターンの開始
            yield return StartCoroutine(PlayerTurn());

            // 敵が倒れた場合は終了
            if (enemy.HP <= 0) break;

            // 敵ターンの処理
            yield return StartCoroutine(EnemyTurn());

            // プレイヤーが倒れた場合は終了
            if (playerdataholder.player.HP <= 0) break;

            // ターンを進める
            turn++;
        }

        // 結果の処理
        if (playerdataholder.player.HP <= 0)
        {
            generaltext.text = "負けてしまった！";
            playerdataholder.player.YouAreDEAD();
        }
        else if (enemy.HP <= 0)
        {
            generaltext.text = enemy.Name + "を倒した!";
            yield return new WaitForSeconds(0.5f);

            // 倒した敵のレベル分経験値を得る
            playerdataholder.player.AddExp(enemy.Lv);

            // 現在レベルの2倍の経験値を得たらレベルアップ
            if (playerdataholder.player.Exp >= playerdataholder.player.Lv * 2)
            {
                playerdataholder.player.LevelUP();
                // テキスト表示
                generaltext.text = $"{playerdataholder.player.Name}のレベルが{playerdataholder.player.Lv}に上がった！";
                numtext_p.text = "LvUP!";
                numtext_p.color = Color.white;
                yield return new WaitForSeconds(1f);
            }

            enemy.YouAreDEAD();
        }
    }

    // プレイヤーターン処理
    private IEnumerator PlayerTurn()
    {
        generaltext.text = $"ターン {turn}: {playerdataholder.player.Name} はどうする？    A:攻撃/S:スキル";
        yield return new WaitForSeconds(0.2f); // 表示時間調整

        bool IsWaiting = false; // 戦闘用フラグ
        while (!IsWaiting)
        {
            if (Input.GetKeyDown(KeyCode.A)) // 通常攻撃
            {
                // 攻撃処理
                damage = enemy.TakeDamage(playerdataholder.player.ATK); // ダメージを算出
                generaltext.text = $"{playerdataholder.player.Name} の攻撃！";
                // ダメージを表示
                numtext_e.text = (-damage).ToString();
                numtext_e.color = Color.red;
                // 武器のアニメーション
                StartCoroutine(weaponcontroller_p.MoveWeaponCoroutine(new Vector3(2.5f, -2f, 0), true));
                yield return new WaitForSeconds(0.5f);

                // リセット系処理
                numtext_e.text = "";
                IsWaiting = true;
            }

            else if (Input.GetKeyDown(KeyCode.S)) // スキル攻撃
            {
                // スキル処理
                // MAGIC
                if (playerdataholder.player_weapon.WeaponSkill == ParameterDifiner.SKILL.MAGIC)
                {
                    damage = enemy.TakeDamage(playerdataholder.player.ATK + enemy.Lv); // 魔法はレベル防御を貫通する
                    generaltext.text = $"{playerdataholder.player.Name} のスキル攻撃！";
                    // ダメージを表示
                    numtext_e.text = (-damage).ToString();
                    numtext_e.color = Color.red;
                }
                // HEAL
                else if(playerdataholder.player_weapon.WeaponSkill == ParameterDifiner.SKILL.HEAL)
                {
                    heal = playerdataholder.player.HealDamage(playerdataholder.player_weapon.WeaponATK); // 回復力=武器攻撃力
                    generaltext.text = $"{playerdataholder.player.Name} は回復した！";
                    // 回復量表示
                    numtext_p.text = $" + {(heal).ToString()}";
                    numtext_p.color = Color.green;
                }

                yield return new WaitForSeconds(0.5f);
                // リセット系処理
                numtext_e.text = "";
                numtext_p.text = "";
                IsWaiting = true;
            }

            yield return null; // 入力待機
        }
    }

    // 敵ターン処理
    private IEnumerator EnemyTurn()
    {
        // ダメージの算出
        damage = playerdataholder.player.TakeDamage(enemy.ATK);
        generaltext.text = "敵の攻撃！";
        // ダメージ表示
        numtext_p.text = (-damage).ToString();
        numtext_p.color = Color.red;
        // 攻撃アニメーション
        StartCoroutine(weaponcontroller_e.MoveWeaponCoroutine(new Vector3(-2.5f, -2f, 0), false));
        // リセット系処理
        yield return new WaitForSeconds(0.5f);
        numtext_p.text = "";
    }

    // 敵の自動的生成
    // ベースとなるステータスの設定
    public struct EnemyStatus
    {
        public string Name;
        public int BaseHp;
        public int BaseAtk;
        public int WeaponIndex; // 武器のインデックス番号
    }

    [SerializeField] List<Sprite> EnemySpriteList; // 敵の画像リスト

    EnemyStatus[] enemies = new EnemyStatus[]
    {
        // 宣言順は、画像リストと同じにすること(EnemySpriteListの0はゴブリンということ)
        new EnemyStatus { Name = "ゴブリン", BaseHp = 10, BaseAtk = 1, WeaponIndex = 1 }, // 武器は斧
        new EnemyStatus { Name = "オーガ", BaseHp = 20, BaseAtk = 5, WeaponIndex = 0 },   // 武器は剣
        new EnemyStatus { Name = "ウィッチ", BaseHp = 15, BaseAtk = 3, WeaponIndex = -1 },// 武器は無し                
    };

    private int[] AutoEnemyGenerater(int pLv, bool Boss)
    {
        int[] enemy_status = new int[5]; // インデックス番号、レベル、HP、攻撃力、武器インデックス

        // どの敵かをランダムで決定
        enemy_status[0] = UnityEngine.Random.Range(0, enemies.Length);
        // 敵のレベルをプレイヤーレベルに応じて決定
        enemy_status[1] = UnityEngine.Random.Range(Mathf.Max(1, pLv - 2), pLv + 2);
        if (Boss)
        {
            enemy_status[1] += 2; // Bossなら、2レベル上げる
        }
        // レベルに応じてステータス調整
        enemy_status[2] = enemies[enemy_status[0]].BaseHp  + enemy_status[1] * 2;
        enemy_status[3] = enemies[enemy_status[0]].BaseAtk + enemy_status[1];
        // 武器インデックスを収納
        enemy_status[4] = enemies[enemy_status[0]].WeaponIndex;

        return enemy_status;
    }
}
