using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.TimeZoneInfo;

public class UtilFunctions : MonoBehaviour
{
    public void SetObjectColor(GameObject obj, Color newColor)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = newColor; // 引数で渡された色に変更
        }
        else
        {
            UnityEngine.Debug.LogWarning("オブジェクトにRendererがありません。");
        }
    }

    public IEnumerator ChangeCameraSize(Camera targetCamera, float initialSize, float targetSize, float transitionTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            // 時間経過に合わせてカメラサイズを変更
            targetCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最終的に目標サイズを設定
        targetCamera.orthographicSize = targetSize;
    }

}
