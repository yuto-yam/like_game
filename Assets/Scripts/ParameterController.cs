using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Playerのステータスを保持する用　DDOS想定
public class ParameterController : MonoBehaviour
{
    public class Charactor //キャラクター生成用の基底クラス
    {
        string name;
        int Lv;
        int maxHP;
        int HP;
        int ATK;

        public Charactor(string name, int lv, int maxHP, int atk)
        {
            this.name = name;
            this.Lv = lv;
            this.maxHP = maxHP;
            this.HP = maxHP;
            this.ATK = atk;
        }

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

            UnityEngine.Debug.Log("name:" + name + "Current HP:" + HP); 
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
}
