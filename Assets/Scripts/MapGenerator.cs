using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// �e�L�X�g�f�[�^������H�����X�N���v�g
public class MapGenerator : MonoBehaviour
{
    [SerializeField] TextAsset mapText; // ���H�̌��ƂȂ�e�L�X�g�A�Z�b�g
    public List<TextAsset> MapTextList = new List<TextAsset>(); // ���H�e�L�X�g�̈ꗗ
    [SerializeField] GameObject[] prefabs; // ���H�̃p�[�c�p�v���n�u
    public enum MAP_TYPE
    { GROUND, // 0
      WALL,   // 1
      STAIR, // 2
      PLAYER   // 3
    }

    public MAP_TYPE[,] mapTable; // �e�L�X�g�ƃv���n�u���q����e�[�u��
    private float mapSize;
    private Vector2 centerPos;

    // �e��X�N���v�g�擾
    ParameterDifiner parameterdifiner;
    UtilFunctions utilFunctions;
    
    private Vector2Int Cpos; // ���݈ʒu

    public MAP_TYPE GetNextMapType(Vector2Int _pos)
    {
        return mapTable[_pos.y, _pos.x];
    }

    // Start is called before the first frame update
    void Start()
    {
        // �e��X�N���v�g�擾
        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();
        utilFunctions = GameManager.Instance.GetComponent<UtilFunctions>();

        if (MapTextList == null || MapTextList.Count == 0)
        {
            UnityEngine.Debug.LogError("MapTextList is not initialized.");
            return;
        }

        // �o�g�������H�̏ꍇ�O�̖��H�ƈʒu�ɖ߂�
        if (parameterdifiner.IsFromBattle)
        {
            mapText = MapTextList[parameterdifiner.MapNumber];
        }
        // �ꗗ���烉���_���ɖ��H��1�I��
        else
        {
            int mapnumber = UnityEngine.Random.Range(0, MapTextList.Count);
            mapText = MapTextList[mapnumber];
            parameterdifiner.MapNumber = mapnumber;
        }
        // ���H����
        _loadMapData(mapText);
        _createMap();
    }

    // txt�t�@�C����ǂݍ��߂�悤�ɕϊ�����
    void _loadMapData(TextAsset textAsset)
    {
        // �񂲂Ƃ�txt��ǂݍ���
        string[] mapLines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        // ���H�̑傫���擾
        int row = mapLines.Length;
        // �R���}��؂�Ɖ���
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

    // �ϊ�����txt����Ή�����v���n�u��ݒu���Ė��H����
    void _createMap()
    {
        if (prefabs == null || prefabs.Length == 0 || prefabs[1] == null)
        {
            UnityEngine.Debug.LogError("Prefabs array is not set up correctly.");
            return;
        }

        mapSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;

        // �v���n�u�̃T�C�Y�𒲐�
        centerPos = new Vector2(
            (mapTable.GetLength(1) % 2 == 0 ? mapTable.GetLength(1) / 2 - 0.5f : mapTable.GetLength(1) / 2) * mapSize,
            (mapTable.GetLength(0) % 2 == 0 ? mapTable.GetLength(0) / 2 - 0.5f : mapTable.GetLength(0) / 2) * mapSize
        );

        // ���ԂɃv���n�u�ʒu
        for (int y = 0; y < mapTable.GetLength(0); y++)
        {
            for (int x = 0; x < mapTable.GetLength(1); x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                Instantiate(prefabs[(int)MAP_TYPE.GROUND], ScreenPos(pos), Quaternion.identity, transform);
                GameObject _map = Instantiate(prefabs[(int)mapTable[y, x]], ScreenPos(pos), Quaternion.identity, transform);

                // �{�X��O�̊K�i�͐Ԃ�
                if (parameterdifiner.MazeCount % 3 == 2 && mapTable[y, x] == MAP_TYPE.STAIR)
                {
                    utilFunctions.SetObjectColor(_map, Color.red);
                }
            }
        }
        // �o�g�������H�Ȃ猳�̈ʒu�ցA����ȊO�ł���΍���X�^�[�g
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
