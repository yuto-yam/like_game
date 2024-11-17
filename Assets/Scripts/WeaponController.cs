using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    //通常攻撃用変数
    private float startAngle = 150f;  //開始角度 (度)
    private float endAngle = 210f;    //終了角度 (度)
    private float radius = 5f;        //扇形の半径
    private Vector3 center;           //中心点
    private float moveDuration = 1f;  //移動にかける時間

    void Start()
    {

    }

    void Update()
    {
        
    }

    public IEnumerator MoveWeaponCoroutine()
    {

        float startAngleRad = Mathf.Deg2Rad * startAngle;
        float endAngleRad = Mathf.Deg2Rad * endAngle;

        float elapsed = 0f; //経過時間
        while (elapsed < moveDuration)
        {
            elapsed += 0.1f; //時間を少しずつ進める(更新間隔は0.1秒)
            float t = elapsed / moveDuration; //正規化した経過時間(0～1)

            //現在の角度を線形補間で計算
            float currentAngle = Mathf.Lerp(startAngleRad, endAngleRad, t);

            //現在の位置を計算
            float x = center.x + Mathf.Cos(currentAngle) * radius;
            float y = center.y + Mathf.Sin(currentAngle) * radius;

            //オブジェクトの位置を更新
            transform.position = new Vector3(x, y, transform.position.z);

            yield return new WaitForSeconds(0.02f); // 0.1秒待機して次のフレームに移行
        }

        transform.position = new Vector3(2.5f, -2f, 0);
    }
}
