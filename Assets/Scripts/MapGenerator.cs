using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] TextAsset mapText;
    public List<TextAsset> MapTextList = new List<TextAsset>();
    [SerializeField] GameObject[] prefabs;
    public enum MAP_TYPE { GROUND, WALL, PLAYER, STAIR }

    public MAP_TYPE[,] mapTable;
    private float mapSize;
    private Vector2 centerPos;

    ParameterDifiner parameterdifiner;
    UtilFunctions utilFunctions;
    private Vector2Int Cpos;

    public MAP_TYPE GetNextMapType(Vector2Int _pos)
    {
        return mapTable[_pos.y, _pos.x];
    }

    void Start()
    {
        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();
        utilFunctions = GameManager.Instance.GetComponent<UtilFunctions>();

        if (MapTextList == null || MapTextList.Count == 0)
        {
            UnityEngine.Debug.LogError("MapTextList is not initialized.");
            return;
        }

        if (parameterdifiner.IsFromBattle)
        {
            mapText = MapTextList[parameterdifiner.MapNumber];
        }
        else
        {
            int mapnumber = UnityEngine.Random.Range(0, MapTextList.Count);
            mapText = MapTextList[mapnumber];
            parameterdifiner.MapNumber = mapnumber;
        }

        _loadMapData(mapText);
        _createMap();
    }

    void _loadMapData(TextAsset textAsset)
    {
        string[] mapLines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        int row = mapLines.Length;
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

    void _createMap()
    {
        if (prefabs == null || prefabs.Length == 0 || prefabs[1] == null)
        {
            UnityEngine.Debug.LogError("Prefabs array is not set up correctly.");
            return;
        }

        mapSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;

        centerPos = new Vector2(
            (mapTable.GetLength(1) % 2 == 0 ? mapTable.GetLength(1) / 2 - 0.5f : mapTable.GetLength(1) / 2) * mapSize,
            (mapTable.GetLength(0) % 2 == 0 ? mapTable.GetLength(0) / 2 - 0.5f : mapTable.GetLength(0) / 2) * mapSize
        );

        for (int y = 0; y < mapTable.GetLength(0); y++)
        {
            for (int x = 0; x < mapTable.GetLength(1); x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                Instantiate(prefabs[(int)MAP_TYPE.GROUND], ScreenPos(pos), Quaternion.identity, transform);
                GameObject _map = Instantiate(prefabs[(int)mapTable[y, x]], ScreenPos(pos), Quaternion.identity, transform);

                if (parameterdifiner.MazeCount % 3 == 2 && mapTable[y, x] == MAP_TYPE.STAIR)
                {
                    utilFunctions.SetObjectColor(_map, Color.red);
                }
            }
        }

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
