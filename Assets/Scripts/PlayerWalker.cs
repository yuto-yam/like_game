using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWalker : MonoBehaviour
{
    Animator animator; //アニメーターの取得
    public enum DIRECTION
    {
        TOP,
        RIGHT,
        DOWN,
        LEFT
    }
    public DIRECTION directon; //方向
    public Vector2Int currentPos, nextPos; //現在位置と次の位置を示す変数
    int[,] move={
        {0, -1}, //TOP
        {1, 0},  //RIGHT
        {0, 1},  //DOWN
        {-1, 0}  //LEFT
    };
    MapGenerator mapGenerator;
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); //起動時にオブジェクトを取得
        mapGenerator = transform.parent.GetComponent<MapGenerator>(); //MapManagerからMapGeneratorを取得
        directon = DIRECTION.DOWN;
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.LeftArrow)) //左を向く
       {
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", true);
        transform.localScale = new Vector3(1,1,1);

        //左方向に歩く
        directon = DIRECTION.LEFT;
        _move();
       }
       if (Input.GetKeyDown(KeyCode.RightArrow)) //右を向く
       {
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", true);
        transform.localScale = new Vector3(-1,1,1);

        //右方向に歩く
        directon = DIRECTION.RIGHT;
        _move();
       }
       if (Input.GetKeyDown(KeyCode.DownArrow)) //前を向く
       {
        animator.SetBool("isWalkingFront", true);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingLeft", false);

        //下方向に歩く
        directon = DIRECTION.DOWN;
        _move();
       }
       if (Input.GetKeyDown(KeyCode.UpArrow)) //背面を向く
       {
        animator.SetBool("isWalkingFront", false);
        animator.SetBool("isWalkingBack", true);
        animator.SetBool("isWalkingLeft", false);

        //上方向に歩く
        directon = DIRECTION.TOP;
        _move();
       }

       if (currentPos.x == mapGenerator.MazeSize_h - 2 && currentPos.y == mapGenerator.MazeSize_w -2){
        //Debug.Log("Goal!");
        SceneManager.LoadScene("Maze");
       }
    }

    void _move() //移動用関数
    {
        nextPos = currentPos + new Vector2Int(move[(int)directon, 0], move[(int)directon, 1]);
        if(mapGenerator.GetNextMapType(nextPos) != MapGenerator.MAP_TYPE.WALL) //MAP_TYPEが壁でなければ移動
        {
            transform.localPosition = mapGenerator.ScreenPos(nextPos);
            currentPos = nextPos;
            Debug.Log(currentPos);
        }
    }
}
