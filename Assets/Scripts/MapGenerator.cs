using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] TextAsset mapText;
    [SerializeField] GameObject[] prefabs; //地面、壁、プレイヤー、階段の順で
    public enum MAP_TYPE
    {
        GROUND, //0
        WALL,   //1
        PLAYER, //2
        STAIR   //3
    }
    
    MazeCreator maze; //MazeCreator型の変数を定義
    int[,] mazeDatas; //迷路データ用にint型の二次元配列の変数を定義

    public MAP_TYPE GetNextMapType(Vector2Int _pos) //MAP_TYPEを返す関数
    {
        return mapTable[_pos.x, _pos.y];
    }

    MAP_TYPE[,] mapTable;
    float mapSize; //マップのサイズ用変数
    //中心座標用の変数
    Vector2 centerPos;

    public int MazeSize_h = 13;
    public int MazeSize_w = 13;

    // Start is called before the first frame update
    void Start()
    {
        _loadMapData();
        _createMap();
    }

    
    void _loadMapData()
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
                GameObject _ground  = Instantiate(prefabs[(int)MAP_TYPE.GROUND], transform);
                GameObject _map = Instantiate(prefabs[(int)mapTable[x,y]], transform);
                _ground.transform.position = ScreenPos(pos);
                _map.transform.position = ScreenPos(pos);
                
                /*
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
                //プレイヤーの初期位置を取得
                if(mapTable[x, y] == MAP_TYPE.PLAYER)
                {
                    _map.GetComponent<PlayerWalker>().currentPos = pos;
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
