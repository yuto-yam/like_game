using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalker : MonoBehaviour
{
    Animator animator; //アニメーターの取得

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); //起動時にオブジェクトを取得
    }

    // Update is called once per frame
    void Update()
    {
        //左を向く
       if (Input.GetKeyDown(KeyCode.LeftArrow)){
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", true);
        transform.localScale = new Vector3(1,1,1);
       }
       //右を向く
       if (Input.GetKeyDown(KeyCode.RightArrow)){
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", true);
        transform.localScale = new Vector3(-1,1,1);
       }
       //前を向く
       if (Input.GetKeyDown(KeyCode.DownArrow)){
        animator.SetBool("isWalkingFront", true);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", false);
       }
       //背面を向くアニメーション
       if (Input.GetKeyDown(KeyCode.UpArrow)){
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", true);
        animator.SetBool("isWalkingLeft", false);
       }
    }
}
