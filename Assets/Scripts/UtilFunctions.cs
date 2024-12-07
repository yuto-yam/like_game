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
            renderer.material.color = newColor; // �����œn���ꂽ�F�ɕύX
        }
        else
        {
            UnityEngine.Debug.LogWarning("�I�u�W�F�N�g��Renderer������܂���B");
        }
    }

    public IEnumerator ChangeCameraSize(Camera targetCamera, float initialSize, float targetSize, float transitionTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            // ���Ԍo�߂ɍ��킹�ăJ�����T�C�Y��ύX
            targetCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ŏI�I�ɖڕW�T�C�Y��ݒ�
        targetCamera.orthographicSize = targetSize;
    }

}
