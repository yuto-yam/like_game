using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Battleシーンでプレイヤーを描画するためのスクリプト
public class BattlePlayerController : MonoBehaviour
{
    Animator animator; // アニメーターの取得
    public enum DIRECTION
    {
        TOP,
        RIGHT,
        DOWN,
        LEFT
    }
    public DIRECTION directon; // 方向

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); // 起動時にオブジェクトを取得
        directon = DIRECTION.LEFT; // (念のため)左を向いておく

        //以下、左を向いて歩かせておく
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
