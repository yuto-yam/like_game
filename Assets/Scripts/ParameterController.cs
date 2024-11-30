using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

// 種々の変数を宣言しておくスクリプト
public class ParameterController : MonoBehaviour
{
    // 各種変数の準備
    public int BattleCount = 0; //バトルの数を数える
    public int MazeCount = 0; //迷路の階層を数える
    public Vector2Int cpos = new Vector2Int(1, 1); //位置を保存　迷路に戻るときに使用
    public string MoveFromScene = "None"; //移動時に、居たシーンを覚えておく
    public static bool IsBossBatlle = false; // ボス戦以降を判定するフラグ

    public class Charactor // キャラクター生成用の基底クラス
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

        public string Getname() { return Name; }

        public int GetLv() { return Lv; }

        public int GetMaxHP() { return maxHP; }

        public int GetHP() { return HP; }

        public int GetATK() { return ATK; }

        public void SetATK(int new_attack)
        {
            ATK = new_attack;
        }

        public void TakeDamage(int damage)
        {
            damage -= Lv;
            if (damage < 0) damage = 0; // 0以下にならないように
            HP -= damage;

            UnityEngine.Debug.Log("name:" + Name + "Current HP:" + HP); 
        }

        public virtual void YouAreDEAD() // 死亡時の関数 上書き前提
        {
            
        }
    }

    public class Player : Charactor // プレイヤーを生成し、管理するクラス
    {
        public Player(string name, int lv, int maxHP, int atk) : base(name, lv, maxHP, atk) { }

        public override void YouAreDEAD() // プレイヤーが死亡時
        {
            UnityEngine.Debug.Log("player is DEAD.");
        }
    }

    public class Enemy : Charactor // 敵を生成し、管理するクラス
    {
        public Enemy(string name, int lv, int maxHP, int atk) : base(name, lv, maxHP, atk) { }

        public override void YouAreDEAD() //敵が死亡時
        {
            UnityEngine.Debug.Log("Enemy is down.");
            if (IsBossBatlle)
            {
                IsBossBatlle = false;
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
    public class Weapon //武器のクラス、データのみ
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
}
