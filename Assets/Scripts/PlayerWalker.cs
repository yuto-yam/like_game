using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

// 迷路の中を歩くためのスクリプト
public class PlayerWalker : MonoBehaviour
{
    // アニメーターの取得
    Animator animator;
    public enum DIRECTION
    {
        TOP,
        RIGHT,
        DOWN,
        LEFT
    }
    // 方向
    public DIRECTION directon;
    // 現在位置と次の位置を示す変数
    public Vector2Int currentPos, nextPos; 
    int[,] move={
        {0, -1}, // TOP
        {1, 0},  // RIGHT
        {0, 1},  // DOWN
        {-1, 0}  // LEFT
    };
    // 各種スクリプト取得
    MapGenerator mapgenerator;
    ParameterDifiner parameterdifiner;
    // public GameObject mazemanager;
    // MazeSaver mazesaver; //JSON保存用スクリプト

    [SerializeField] float Encounter = 0; // 敵との遭遇危険度 1以上でエンカウント


    // Start is called before the first frame update
    void Start()
    {
        // 起動時にオブジェクトを取得
        animator = GetComponent<Animator>();
        // MazeManagerからスクリプトを取得
        // mazemanager = GameObject.FindWithTag("MazeManager");
        mapgenerator = transform.parent.GetComponent<MapGenerator>();
        // mazesaver = transform.parent.GetComponent<MazeSaver>();

        // 始めは下向き
        directon = DIRECTION.DOWN;

        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();
        
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.LeftArrow)) // 左を向く
       {
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", true);
        transform.localScale = new Vector3(1,1,1);

        // 左方向に歩く
        directon = DIRECTION.LEFT;
        _move();
       }
       if (Input.GetKeyDown(KeyCode.RightArrow)) // 右を向く
       {
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", true);
        transform.localScale = new Vector3(-1,1,1);

        // 右方向に歩く
        directon = DIRECTION.RIGHT;
        _move();
       }
       if (Input.GetKeyDown(KeyCode.DownArrow)) // 前を向く
       {
        animator.SetBool("isWalkingFront", true);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", false);

        // 下方向に歩く
        directon = DIRECTION.DOWN;
        _move();
       }
       if (Input.GetKeyDown(KeyCode.UpArrow)) // 背面を向く
       {
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", true);
        animator.SetBool("isWalkingLeft", false);

        // 上方向に歩く
        directon = DIRECTION.TOP;
        _move();
       }

       // ゴールしたら、次の迷路へ
       if (currentPos.x == mapgenerator.mapTable.GetLength(0) - 2 && currentPos.y == mapgenerator.mapTable.GetLength(1) - 2)
       {    
            parameterdifiner.MazeCount += 1;
            
            // 迷路3つごとにボス戦へ
            if (parameterdifiner.MazeCount %3 == 0)
            {
                parameterdifiner.IsBossBattle = true;
                SceneManager.LoadScene("Battle");
            }
            else
            {
                SceneManager.LoadScene("Maze");
            }
       }
       else if (Encounter >= 1f) //ゴール以外で、敵との遭遇危険度が1を超えたら戦闘へ
       {
            parameterdifiner.CPOS = currentPos;
            // mazesaver.SaveScene();
            SceneManager.LoadScene("Battle");
       }
       
       

    }

    void _move() // 移動用関数
    {
        nextPos = currentPos + new Vector2Int(move[(int)directon, 0], move[(int)directon, 1]);
        if(mapgenerator.GetNextMapType(nextPos) != MapGenerator.MAP_TYPE.WALL) //MAP_TYPEが壁でなければ移動
        {
            transform.localPosition = mapgenerator.ScreenPos(nextPos);
            currentPos = nextPos;

            // 敵との遭遇率を計上
            float tmp_enc = ((float)currentPos.x + (float)currentPos.y) /
                             ((float)mapgenerator.mapTable.GetLength(0) * (float)mapgenerator.mapTable.GetLength(1)); // 現在座標の和をマス目の数で正規化、端の方が敵が出やすい
            Encounter += tmp_enc * UnityEngine.Random.Range(parameterdifiner.Encount_Rate, 1.0f); // 敵に出会いすぎないように補正、9*9で1回あるかどうかくらい

            UnityEngine.Debug.Log(Encounter);
            UnityEngine.Debug.Log(currentPos);
        }
    }
}
