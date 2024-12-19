using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

// 武器のアニメーションを実現するためのスクリプト
public class WeaponController : MonoBehaviour
{
    // スキル用オブジェクト
    [SerializeField] GameObject MagicObject;
    [SerializeField] GameObject HealObject;

    // 通常攻撃用変数
    private float startAngle = 150f;  // 開始角度 (度)
    private float endAngle = 210f;    // 終了角度 (度)
    private float radius = 5f;        // 扇形の半径
    private Vector3 center;           // 中心点
    private float moveDuration = 1f;  // 移動にかける時間

    // 剣のアニメーション(上から下へ斬り下ろす)
    public IEnumerator SwordAnimationCoroutine(Vector3 goal, bool left)
    {
        if (!left)
        {
            startAngle = 30f;
            endAngle = -30f;
        }
        // 角度をラジアンに変換
        float startAngleRad = Mathf.Deg2Rad * startAngle;
        float endAngleRad = Mathf.Deg2Rad * endAngle;

        float elapsed = 0f; // 経過時間
        while (elapsed < moveDuration)
        {
            elapsed += 0.1f; // 時間を少しずつ進める(更新間隔は0.1秒)
            float t = elapsed / moveDuration; // 正規化した経過時間(0～1)

            // 現在の角度を線形補間で計算
            float currentAngle = Mathf.Lerp(startAngleRad, endAngleRad, t);

            // 現在の位置を計算
            float x = center.x + Mathf.Cos(currentAngle) * radius;
            float y = center.y + Mathf.Sin(currentAngle) * radius;

            // オブジェクトの位置を更新
            transform.position = new Vector3(x, y, transform.position.z);

            yield return new WaitForSeconds(0.02f); // 0.02秒待機して次のフレームに移行
        }

        // 処理が終わったら元の位置に戻る
        transform.position = goal;
    }

    // 斧のアニメーション(下から上へ斬り上げる)
    public IEnumerator AxAnimationCoroutine(Vector3 goal, bool left)
    {
        startAngle = 210f;
        endAngle = 150f;
        if (!left)
        {
            startAngle = -30f;
            endAngle = 30f;
        }
        // 角度をラジアンに変換
        float startAngleRad = Mathf.Deg2Rad * startAngle;
        float endAngleRad = Mathf.Deg2Rad * endAngle;

        float elapsed = 0f; // 経過時間
        while (elapsed < moveDuration)
        {
            elapsed += 0.1f; // 時間を少しずつ進める(更新間隔は0.1秒)
            float t = elapsed / moveDuration; // 正規化した経過時間(0～1)

            // 現在の角度を線形補間で計算
            float currentAngle = Mathf.Lerp(startAngleRad, endAngleRad, t);

            // 現在の位置を計算
            float x = center.x + Mathf.Cos(currentAngle) * radius;
            float y = center.y + Mathf.Sin(currentAngle) * radius;

            // オブジェクトの位置を更新
            transform.position = new Vector3(x, y, transform.position.z);

            yield return new WaitForSeconds(0.02f); // 0.02秒待機して次のフレームに移行
        }

        // 処理が終わったら元の位置に戻る
        transform.position = goal;
    }

    // 槍のアニメーション(真っ直ぐ進んでいく)
    public IEnumerator SpearAnimationCoroutine(Vector3 goal, bool left)
    {
        Vector3 startpos = new Vector3(5, 0, 0);
        Vector3 endpos = new Vector3(-5, 0, 0);

        Vector3 LocalAngle = transform.localEulerAngles;
        Vector3 ChangeAngle = LocalAngle;
        // 相手に刃を向ける
        ChangeAngle.z += 45.0f;

        if (!left)
        {
            startpos = new Vector3(-5, 0, 0);
            endpos = new Vector3(5, 0, 0);

            // 敵の場合は回転方向が逆転(余分に回して戻す)
            ChangeAngle.z -= 90.0f;
        }

        transform.localEulerAngles = ChangeAngle;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += 0.1f; // 時間を少しずつ進める(更新間隔は0.1秒)
            float t = elapsed / moveDuration; // 正規化した経過時間(0～1)

            float x = (endpos.x - startpos.x) * t;
            transform.position = new Vector3(x, 0, transform.position.z);

            yield return new WaitForSeconds(0.02f);
        }

        transform.localEulerAngles = LocalAngle;
        transform.position = goal;
    }

    // 槌のアニメーション(振り上げて、真っ直ぐ振り下ろす)
    public IEnumerator MiceAnimationCoroutine(Vector3 goal, bool left)
    {
        float halfDuration = moveDuration / 2;
        float elapsed = 0f;
        Vector3 startpos, endpos;

        Vector3 LocalAngle = transform.localEulerAngles;
        Vector3 ChangeAngle = LocalAngle;

        // 振り上げる
        startpos = transform.position;
        endpos.x = -(startpos.x * 2f);
        endpos.y = -startpos.y;
        endpos.z = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += 0.1f;
            float t = elapsed / halfDuration;

            transform.position = Vector3.Lerp(startpos, endpos, t);
            yield return new WaitForSeconds(0.02f);
        }

        // 武器を水平に
        ChangeAngle.z += 45f;
        if (!left) { ChangeAngle.z -= 90f; }

        transform.localEulerAngles = ChangeAngle;

        // 振り下ろす
        startpos = transform.position;
        endpos = startpos;
        endpos.y = -startpos.y;

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += 0.1f;
            float t = elapsed / halfDuration;

            transform.position = Vector3.Lerp(startpos, endpos, t);
            yield return new WaitForSeconds(0.02f);
        }

        // 回転を元に戻す
        transform.localEulerAngles = LocalAngle;

        // ゴールに戻る
        transform.position = goal;
    }

    // MAGIC攻撃用アニメーション(引数が色の点に注意)
    public IEnumerator MagicAnimationCoroutine(Color Weaponcolor, bool left)
    {
        Vector3 startpos = new Vector3(5f, 0, 0);
        if (!left) {  startpos.x = -startpos.x; }
        Vector3 endpos = startpos;
        endpos.x = -startpos.x;

        GameObject magicinstance = Instantiate(MagicObject, startpos, Quaternion.identity);
        magicinstance.transform.localScale = new Vector3(10f, 10f, 10f);
        Renderer renderer = magicinstance.GetComponent<Renderer>();
        renderer.material.color = Weaponcolor; // 引数で渡された色に変更

        // 指向性のある向きに回転
        Vector3 LocalAngle = magicinstance.transform.localEulerAngles;
        LocalAngle.z -= 90f;
        if (!left) { LocalAngle.z += 180f; }
        magicinstance.transform.localEulerAngles = LocalAngle;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += 0.1f;
            float t = elapsed / moveDuration;

            magicinstance.transform.position = Vector3.Lerp(startpos, endpos, t);
            yield return new WaitForSeconds(0.02f);
        }

        Destroy(magicinstance);
    }

    // 回復用アニメーション(一定時間演出を出すだけ)
    public IEnumerator HealAnimationCoroutine(bool left)
    {
        Vector3 pos = new Vector3(5f, -0.5f, 0);
        if (!left) { pos.x = -pos.x; }

        GameObject healinstance = Instantiate(HealObject, pos, Quaternion.identity);
        healinstance.transform.localScale = new Vector3(3f, 3f, 3f);


        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.02f);
        }

        Destroy(healinstance);
    }
}
