using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    int[,] move ={
        {0, -1}, // TOP
        {1, 0},  // RIGHT
        {0, 1},  // DOWN
        {-1, 0}  // LEFT
    };
    // 各種スクリプト取得
    MapGenerator mapgenerator;
    ParameterDifiner parameterdifiner;
    PlayerDataHolder playerdataholder;
    UtilFunctions utilFunctions;
    SoundEffectPlayer soundEffectPlayer;
    public Camera TargetCamera;

    [SerializeField] float Encounter = 0; // 敵との遭遇危険度 1以上でエンカウント

    private bool isChangingCamera = false; // カメラ変更中フラグ
    private Vector3 originalCameraPos; // カメラの元の位置

    // Start is called before the first frame update
    void Start()
    {
        // 起動時にオブジェクトを取得
        animator = GetComponent<Animator>();
        mapgenerator = transform.parent.GetComponent<MapGenerator>();
        TargetCamera = Camera.main;
        TargetCamera.orthographicSize = (mapgenerator.mapTable.GetLength(0) + mapgenerator.mapTable.GetLength(1)) / 2 - 3;

        // カメラの元の位置を保存
        originalCameraPos = TargetCamera.transform.position;

        // 始めは下向き
        directon = DIRECTION.DOWN;

        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();
        playerdataholder = GameManager.Instance.GetComponent<PlayerDataHolder>();
        utilFunctions = GameManager.Instance.GetComponent<UtilFunctions>();
        soundEffectPlayer = GameManager.Instance.GetComponent<SoundEffectPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChangingCamera) return; // カメラ変更中は処理を中断

        if (Input.GetKeyDown(KeyCode.LeftArrow)) // 左を向く
        {
            animator.SetBool("isWalkingFront", false);
            animator.SetBool("isWalkingBack", false);
            animator.SetBool("isWalkingLeft", true);
            transform.localScale = new Vector3(1, 1, 1);

            // 左方向に歩く
            directon = DIRECTION.LEFT;
            _move();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) // 右を向く
        {
            animator.SetBool("isWalkingFront", false);
            animator.SetBool("isWalkingBack", false);
            animator.SetBool("isWalkingLeft", true);
            transform.localScale = new Vector3(-1, 1, 1);

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
            parameterdifiner.MazeCount = (parameterdifiner.MazeCount + 1);

            // 迷路3つごとにボス戦へ
            if (parameterdifiner.MazeCount % 3 == 0)
            {
                // エンカウントSEの再生
                soundEffectPlayer.EncountSEPlay();
                parameterdifiner.IsBossBattle = true;
                StartCoroutine(HandleEncounter(1.0f, 0.3f));
            }
            else
            {
                // 階段を降りるSE
                soundEffectPlayer.StairDownSEPlay();
                SceneManager.LoadScene("Maze");
            }
        }
        else if (mapgenerator.GetNextMapType(currentPos) == MapGenerator.MAP_TYPE.EVENT) // イベントマスでHPとMP回復
        {
            // 回復SE
            soundEffectPlayer.HealSEPlay();
        }
        else if (Encounter >= 1f) //ゴールとイベントマス以外で、敵との遭遇危険度が1を超えたら戦闘へ
        {
            // エンカウントSEの再生
            soundEffectPlayer.EncountSEPlay();
            parameterdifiner.CPOS = currentPos;
            StartCoroutine(HandleEncounter(0.5f, 0.1f));
        }
    }

    private IEnumerator HandleEncounter(float durationTime, float shakeamount)
    {
        isChangingCamera = true; // カメラ変更中フラグを立てる

        // カメラを揺らす
        StartCoroutine(ShakeCamera(durationTime, shakeamount));

        // カメラのサイズ変更を待つ
        yield return StartCoroutine(ChangeCameraSize(TargetCamera, 5.0f, 7.0f, durationTime));

        // 揺れを終わらせる
        StopCoroutine(ShakeCamera(durationTime, shakeamount));
        TargetCamera.transform.position = originalCameraPos; // 元の位置に戻す

        // シーン遷移
        SceneManager.LoadScene("Battle");

        isChangingCamera = false; // カメラ変更が完了したのでフラグを解除
    }

    private IEnumerator ShakeCamera(float duration, float amount)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // ランダムにカメラ位置を揺らす
            TargetCamera.transform.position = originalCameraPos + (Vector3)(UnityEngine.Random.insideUnitCircle * amount);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 揺れが終わったら元の位置に戻す
        TargetCamera.transform.position = originalCameraPos;
    }

    void _move() // 移動用関数
    {
        nextPos = currentPos + new Vector2Int(move[(int)directon, 0], move[(int)directon, 1]);
        if (mapgenerator.GetNextMapType(nextPos) != MapGenerator.MAP_TYPE.WALL) //MAP_TYPEが壁でなければ移動
        {
            transform.localPosition = mapgenerator.ScreenPos(nextPos);
            currentPos = nextPos;

            // 敵との遭遇率を計上
            float tmp_enc = ((float)currentPos.x + (float)currentPos.y) /
                             ((float)mapgenerator.mapTable.GetLength(0) * (float)mapgenerator.mapTable.GetLength(1)); // 現在座標の和をマス目の数で正規化、端の方が敵が出やすい
            Encounter += tmp_enc * UnityEngine.Random.Range(parameterdifiner.Encount_Rate, 1.0f); // 敵に出会いすぎないように補正

            UnityEngine.Debug.Log(Encounter);
            UnityEngine.Debug.Log(currentPos);
        }
        else
        {
            soundEffectPlayer.MoveCancelSEPlay();
        }
    }

    // カメラのサイズをスムーズに変更するコルーチン
    private IEnumerator ChangeCameraSize(Camera camera, float startSize, float targetSize, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            camera.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最終的に目標サイズを設定
        camera.orthographicSize = targetSize;
    }
}
