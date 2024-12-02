using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

// 種々の変数を宣言しておくスクリプト
public class ParameterDifiner : MonoBehaviour
{
    // 各種変数の準備
    public int BattleCount = 0; //バトルの数
    public int MazeCount = 0; //迷路の階層数
    public float Encount_Rate = 0.5f; // 敵の出現頻度 ~1まで
    public bool IsBossBattle = false; // ボス戦を判定するフラグ
    public bool IsFromBattle = false; // バトル→迷路かどうかを制御
    [SerializeField] Vector2Int cpos = new Vector2Int(1, 1); //迷路でのいた位置を記憶

    public Vector2Int CPOS // cposはprivateにしたので、値の入出力用の関数を作成
    {
        get { return cpos; }
        set //迷路外にはいかないようにしておく(最大値は仮の値)
        {
            cpos = new Vector2Int(
                Mathf.Clamp(value.x, 1, 21), // 1～21に補正
                Mathf.Clamp(value.y, 1, 21)  // 1～21に補正
            );
        }
    }

    public List<Sprite> WeaponSpriteList; //武器の候補リスト 敵も味方も

    // キャラクター生成用の基底クラス
    public class Charactor
    {
        protected string Name;
        protected int Lv;
        protected int maxHP;
        protected int HP;
        protected int ATK;

        public Charactor(string name, int lv, int maxHP, int atk)
        {
            this.Name = name;
            this.Lv = lv;
            this.maxHP = maxHP;
            this.HP = maxHP;
            this.ATK = atk;
        }

        public string GetName() { return Name; }
        public int GetLv() { return Lv; }
        public int GetMaxHP() { return maxHP; }
        public int GetHP() { return HP; }
        public int GetATK() { return ATK; }

        public void SetLv(int new_lv){ Lv = new_lv; }
        public void SetMaxHP(int new_maxHP){ maxHP = new_maxHP; }
        public void SetATK(int new_attack) { ATK = new_attack; }

        public int TakeDamage(int atk) // 引数は攻撃力
        {
            int damage = atk - Lv; // レベル=防御力とする
            if (damage < 0) damage = 0; // 0以下にならないように
            HP -= damage;

            UnityEngine.Debug.Log("name:" + Name + "Current HP:" + HP); 
            return damage;
        }

        public int HealDamage(int heal) // 引数は回復力
        {
            HP += heal;
            if (HP > maxHP) HP = maxHP; // 最大HPは超えない

            UnityEngine.Debug.Log("name:" + Name + "Current HP:" + HP);
            return heal; // damageと揃えるため返り値にしておく
        }

        public virtual void YouAreDEAD() // 死亡時の関数 上書き前提
        {
            
        }
    }

    // プレイヤーを生成し、管理するクラス
    public class Player : Charactor 
    {
        protected int Exp;

        public Player(string name, int lv, int exp, int maxHP, int atk) : base(name, lv, maxHP, atk)
        {
            this.Exp = exp;
        }

        public int GetExp() { return Exp; }
        public void SetExp(int e) { Exp += e; }

        public void LevelUP()
        {
            maxHP += Lv;
            HP += Lv;
            ATK += Lv;
            Lv += 1;
            Exp = 0;
        }

        public override void YouAreDEAD() // プレイヤーが死亡時
        {
            UnityEngine.Debug.Log("player is DEAD.");
        }
    }

    // 敵を生成し、管理するクラス
    public class Enemy : Charactor
    {
        private ParameterDifiner parameterDifiner;

        public Enemy(string name, int lv, int maxHP, int atk, ParameterDifiner parameterDifiner) : base(name, lv, maxHP, atk)
        {
            this.parameterDifiner = parameterDifiner;
        }

        public override void YouAreDEAD() //敵が死亡時
        {
            UnityEngine.Debug.Log("Enemy is down.");
            parameterDifiner.IsFromBattle = true;

            if (parameterDifiner.IsBossBattle)
            {
                parameterDifiner.IsBossBattle = false;
                SceneManager.LoadScene("Drop");
            }
            else
            {
                SceneManager.LoadScene("Maze");
            }
        }
    }

    // 武器につけるスキル
    public enum SKILL
    {
        MAGIC, //レベル防御貫通攻撃
        HEAL   //回復
    }

    //武器のクラス、データのみ
    public class Weapon
    {
        protected string Name;
        protected int WeaponATK;
        protected SKILL WeaponSKILL;

        public Weapon(string name, int watk, SKILL skill)
        {
            this.Name = name;
            this.WeaponATK = watk;
            this.WeaponSKILL = skill;
        }

        public string GetWeaponName() { return Name; }

        public int GetWeaponATK() { return WeaponATK; }

        public SKILL GetWeaponSKILL() { return WeaponSKILL;}
    }

    // 以下、util系の関数

    public void SetObjectColor(GameObject obj, Color newColor)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = newColor; // 引数で渡された色に変更
        }
        else
        {
            UnityEngine.Debug.LogWarning("Renderer not found on the object.");
        }
    }
}
