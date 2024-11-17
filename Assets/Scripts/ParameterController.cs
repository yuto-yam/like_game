using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Playerのステータスを保持する用　DDOS想定
public class ParameterController : MonoBehaviour
{
    public class Charactor //キャラクター生成用の基底クラス
    {
        string Name;
        int Lv;
        int maxHP;
        int HP;
        int ATK;

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

        public int GetHP() { return HP; }

        public int GetATK() { return ATK; }

        public void SetATK(int new_attack)
        {
            ATK = new_attack;
        }

        public void TakeDamage(int damage)
        {
            damage -= Lv;
            if (damage < 0) damage = 0; //0以下にならないように
            HP -= damage;

            UnityEngine.Debug.Log("name:" + Name + "Current HP:" + HP); 
        }

        public virtual void YouAreDEAD() //死亡時の関数 上書き前提
        {
            
        }
    }

    public class Player : Charactor //プレイヤーを管理するためのクラス
    {
        public Player(string name, int lv, int maxHP, int atk) : base(name, lv, maxHP, atk) { }

        public override void YouAreDEAD() //プレイヤーが死亡時
        {
            UnityEngine.Debug.Log("player is DEAD.");
        }
    }

    public class Enemy : Charactor //敵の管理のためのクラス
    {
        public Enemy(string name, int lv, int maxHP, int atk) : base(name, lv, maxHP, atk) { }

        public override void YouAreDEAD() //敵が死亡時
        {
            UnityEngine.Debug.Log("Enemy is down.");
        }
    }

    public int BattleCount = 0; //バトルの数を数える
    public int SceneCount = 0; //迷路の階層を数える
    public Vector2Int cpos; //位置を保存　迷路に戻るときに使用

    public enum SKILL
    {
        MAGIC, //レベル防御貫通攻撃
        HEAL   //回復
    }
    public class Weapon //武器のクラス
    {
        public string Name;
        public int WeaponATK;
        public SKILL WeaponSKILL;
        //public GameObject WeaponImage;

        public Weapon(string name, int watk, SKILL skill)
        {
            this.Name = name;
            this.WeaponATK = watk;
            this.WeaponSKILL = skill;
            //this.WeaponImage = obj;
        }

        public string GetName() { return Name; }

        public int GetWeaponATK() { return WeaponATK; }

        public SKILL GetWeaponSKILL() { return WeaponSKILL;}

        //public GameObject GetWeaponImage() { return WeaponImage;}
    }
}
