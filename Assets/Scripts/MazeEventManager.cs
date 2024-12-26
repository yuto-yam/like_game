using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 迷路のイベント制御スクリプト(プレハブにアタッチして使う)
public class MazeEventManager : MonoBehaviour
{
    ParameterDifiner parameterdifiner;
    PlayerDataHolder playerdataholder;

    // Start is called before the first frame update
    void Start()
    {
        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();
        playerdataholder = GameManager.Instance.GetComponent<PlayerDataHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerdataholder.player.HealDamage(playerdataholder.player.Lv);
            playerdataholder.player.MPChange(-playerdataholder.player.Lv);
            Destroy(this.gameObject);
        }
    }
}
