using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

}
