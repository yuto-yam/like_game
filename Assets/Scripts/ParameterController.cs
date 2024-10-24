using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Playerのステータスを保持する用　DDOS想定
public class ParameterController : MonoBehaviour
{
    public class Charactor //あとで敵生成用に引き継ぎたい
    {
        public string name;
        public int maxHP;
        public int HP;
        public int ATK;
        public int DEF;

        public Charactor(string name, int maxHP, int atk, int def)
        {
            this.name = name;
            this.maxHP = maxHP;
            this.HP = maxHP;
            this.ATK = atk;
            this.DEF = def;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        Charactor player = new Charactor("player", 50, 10, 5);
        UnityEngine.Debug.Log("name:" + player.name + " " + "HP/ATK/DEF:" + player.HP + "/" + player.ATK + "/" + player.DEF);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
