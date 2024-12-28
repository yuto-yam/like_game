using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// テキストデータから迷路を作るスクリプト
public class MapGenerator : MonoBehaviour
{
    [SerializeField] TextAsset mapText; // 迷路の元となるテキストアセット
    public List<TextAsset> MapTextList = new List<TextAsset>(); // 迷路テキストの一覧
    [SerializeField] GameObject[] prefabs; // 迷路のパーツ用プレハブ
    public enum MAP_TYPE
    { 
        GROUND,  // 0
        WALL,    // 1
        STAIR,   // 2
        EVENT,   // 3
        PLAYER   // 4
    }

    public MAP_TYPE[,] mapTable; // テキストとプレハブを繋げるテーブル
    private float mapSize;
    private Vector2 centerPos;

    // 各種スクリプト取得
    ParameterDifiner parameterdifiner;
    UtilFunctions utilFunctions;
    
    private Vector2Int Cpos; // 現在位置

    public MAP_TYPE GetNextMapType(Vector2Int _pos)
    {
        return mapTable[_pos.y, _pos.x];
    }

    // Start is called before the first frame update
    void Start()
    {
        // 各種スクリプト取得
        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();
        utilFunctions = GameManager.Instance.GetComponent<UtilFunctions>();

        if (MapTextList == null || MapTextList.Count == 0)
        {
            UnityEngine.Debug.LogError("MapTextList is not initialized.");
            return;
        }

        // バトル→迷路の場合前の迷路と位置に戻る
        if (parameterdifiner.IsFromBattle)
        {
            mapText = MapTextList[parameterdifiner.MapNumber];
        }
        // 一覧からランダムに迷路を1つ選ぶ
        else
        {
            int mapnumber = UnityEngine.Random.Range(0, MapTextList.Count);
            mapText = MapTextList[mapnumber];
            parameterdifiner.MapNumber = mapnumber;
        }
        // 迷路生成
        _loadMapData(mapText);
        _createMap();
    }

    // txtファイルを読み込めるように変換する
    void _loadMapData(TextAsset textAsset)
    {
        // 列ごとにtxtを読み込み
        string[] mapLines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        // 迷路の大きさ取得
        int row = mapLines.Length;
        // コンマ区切りと仮定
        int col = mapLines[0].Split(',').Length;
        mapTable = new MAP_TYPE[row, col];

        for (int y = 0; y < row; y++)
        {
            string[] mapValues = mapLines[y].Split(',');
            for (int x = 0; x < col; x++)
            {
                mapTable[y, x] = (MAP_TYPE)int.Parse(mapValues[x]);
            }
        }
    }

    // 変換したtxtから対応するプレハブを設置して迷路生成
    void _createMap()
    {
        if (prefabs == null || prefabs.Length == 0 || prefabs[1] == null)
        {
            UnityEngine.Debug.LogError("Prefabs array is not set up correctly.");
            return;
        }

        mapSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;

        // プレハブのサイズを調整
        centerPos = new Vector2(
            (mapTable.GetLength(1) % 2 == 0 ? mapTable.GetLength(1) / 2 - 0.5f : mapTable.GetLength(1) / 2) * mapSize,
            (mapTable.GetLength(0) % 2 == 0 ? mapTable.GetLength(0) / 2 - 0.5f : mapTable.GetLength(0) / 2) * mapSize
        );

        // 順番にプレハブ位置
        for (int y = 0; y < mapTable.GetLength(0); y++)
        {
            for (int x = 0; x < mapTable.GetLength(1); x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                Instantiate(prefabs[(int)MAP_TYPE.GROUND], ScreenPos(pos), Quaternion.identity, transform);
                GameObject _map = Instantiate(prefabs[(int)mapTable[y, x]], ScreenPos(pos), Quaternion.identity, transform);

                // ボス戦前の階段は赤く
                if (parameterdifiner.MazeCount % 3 == 2 && mapTable[y, x] == MAP_TYPE.STAIR)
                {
                    utilFunctions.SetObjectColor(_map, Color.red);
                }
            }
        }
        // バトル→迷路なら元の位置へ、それ以外であれば左上スタート
        Cpos = parameterdifiner.IsFromBattle ? parameterdifiner.CPOS : new Vector2Int(1, 1);
        parameterdifiner.IsFromBattle = false;

        GameObject player = Instantiate(prefabs[(int)MAP_TYPE.PLAYER], ScreenPos(Cpos), Quaternion.identity, transform);
        player.GetComponent<PlayerWalker>().currentPos = Cpos;

    }

    public Vector2 ScreenPos(Vector2Int _pos)
    {
        return new Vector2(
            (_pos.x - mapTable.GetLength(1) / 2) * mapSize,
            -(_pos.y - mapTable.GetLength(0) / 2) * mapSize
        );
    }
}
