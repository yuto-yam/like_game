using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] TextAsset mapText; //mapのデータを格納するためのテキスト(リスト)
    [SerializeField] GameObject[] prefabs; //地面、壁、プレイヤー、階段の順で
    public enum MAP_TYPE //上のprefabの順でMAP_TYPEを定義
    {
        GROUND, //0
        WALL,   //1
        PLAYER, //2
        STAIR   //3
    }
    
    MazeCreator maze; //MazeCreator型の変数を定義
    public int MazeSize_w;
    public int MazeSize_h;
    int[,] mazeDatas; //迷路データ用にint型の二次元配列の変数を定義

    MAP_TYPE[,] mapTable;
    float mapSize; //マップのサイズ用変数
    Vector2 centerPos; //中心座標用の変数

    [SerializeField] GameObject player;

    ParameterController parametercontroller;
    private Vector2Int Cpos;

    public MAP_TYPE GetNextMapType(Vector2Int _pos) //MAP_TYPEを返す関数
    {
        return mapTable[_pos.x, _pos.y];
    }

    // Start is called before the first frame update
    void Start()
    {
        parametercontroller = GameManager.Instance.GetComponent<ParameterController>();
        _loadMapData();
        _createMap();
    }

    
    void _loadMapData() //テキストを行列に変換
    {
        string[] mapLines = mapText.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        int row = mapLines.Length; //行の数
        int col = mapLines[0].Split(new char[] { ',' }).Length; //列の数
        mapTable = new MAP_TYPE[col,row]; //初期化

        for (int y = 0; y < row; y++) //行の数だけループ
        {
            string[] mapValues = mapLines[y].Split(new char[] { ',' }); //1行をカンマ区切りで分割
            for (int x = 0; x < col; x++) //列の数だけループ
            {
                mapTable[x, y] = (MAP_TYPE)int.Parse(mapValues[x]); //mapValuesのx番目をMAP_TYPEにキャストしてmapTable[x,y]番目に代入
            }
        }
    }
    /* //自動生成用
    void _loadMapData()
    {
        //MazeCreatorをインスタンス化（例として13×13のマップ）
        maze = new MazeCreator(MazeSize_h, MazeSize_w);
        //迷路データ用二次元配列を生成
        mazeDatas = new int[9,9];
        //迷路データ作成＆取得
        mazeDatas = maze.CreateMaze();
        //マップの縦の長さ取得
        int row = mazeDatas.GetLength(1);
        //マップの横の長さ取得
        int col = mazeDatas.GetLength(0);
        //マップテーブル初期化（前回と同じ）
        mapTable = new MAP_TYPE[col, row];
        
        //マップテーブル作成
        for(int y = 0;y < row; y++)
        {
            for(int x = 0;x < col; x++)
            {
                //迷路データをMAP_TYPEにキャストしてマップテーブルに格納
                mapTable[x, y] = (MAP_TYPE)mazeDatas[x, y];
            }
        }
    }
    */

    void _createMap()
    {
        mapSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;

        //中心座標を取得
        //縦横の数を半分にしてmapSizeを掛けることで中心を求める
        
        //列が偶数の場合
        if(mapTable.GetLength(0) % 2 == 0)
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize - (mapSize / 2);
        }
        else
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize;
        }
        //行が偶数の場合
        if(mapTable.GetLength(1) % 2 == 0)
        {
            centerPos.y = mapTable.GetLength(1) / 2 * mapSize - ( mapSize / 2);
        }
        else
        {
            centerPos.y = mapTable.GetLength(1) / 2 * mapSize;
        }

        for (int y = 0;y < mapTable.GetLength(1); y++)
        {
            for(int x = 0;x < mapTable.GetLength(0); x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                //地面を先に敷く
                GameObject _ground  = Instantiate(prefabs[(int)MAP_TYPE.GROUND], transform);
                _ground.transform.position = ScreenPos(pos);
                //各位置に対応するPrefabを配置
                GameObject _map = Instantiate(prefabs[(int)mapTable[x,y]], transform);
                _map.transform.position = ScreenPos(pos);

                /* //自動生成用
                if (x == 1 && y == 1)
                {
                    //プレイヤーを生成
                    _map = Instantiate(prefabs[2], transform);
                    //ポジション修正
                    _map.transform.position = ScreenPos(pos);
                    //プレイヤーの現在ポジションを設定
                    _map.GetComponent<PlayerWalker>().currentPos = pos;
                }
                */

                //プレイヤーの位置を取得して配置
                //ループの最後に配置
                if (x == mapTable.GetLength(0) -1 && y == mapTable.GetLength(1) -1)
                {
                    if (parametercontroller.MoveFromScene == "Battle")
                    {
                        Cpos = parametercontroller.cpos;
                    }
                    else
                    {
                        Cpos = new Vector2Int(1, 1);
                    }
                    _map = Instantiate(prefabs[2], transform);
                    _map.transform.position = ScreenPos(Cpos);
                    _map.GetComponent<PlayerWalker>().currentPos = Cpos;
                }
            }
        }
    }

    public Vector2 ScreenPos(Vector2Int _pos)
    {
        //centerPos.xとcenterPos.yをそれぞれひいて位置を取得する
        return new Vector2(
            _pos.x * mapSize - centerPos.x,
            //Yをマイナスにする
            -(_pos.y * mapSize - centerPos.y));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
