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
            renderer.material.color = newColor; // �����œn���ꂽ�F�ɕύX
        }
        else
        {
            UnityEngine.Debug.LogWarning("�I�u�W�F�N�g��Renderer������܂���B");
        }
    }

}
