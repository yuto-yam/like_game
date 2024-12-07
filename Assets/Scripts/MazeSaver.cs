using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SceneData
{
    public List<ObjectData> objects;
}

[System.Serializable]
public class ObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string spriteName;
    public int sortingOrder; // レイヤー順序を保存
    public string animatorControllerName;
    public List<string> scriptNames;
    public List<ObjectData> children;
}

public class MazeSaver : MonoBehaviour
{
    private string savePath = "Assets/Resources/SceneData/sceneData.json";

    public void SaveScene()
    {
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        SceneData sceneData = new SceneData { objects = new List<ObjectData>() };

        foreach (Transform obj in transform)
        {
            sceneData.objects.Add(GetObjectData(obj));
        }

        string json = JsonUtility.ToJson(sceneData, true);
        File.WriteAllText(savePath, json);

        UnityEngine.Debug.Log("Scene saved to " + savePath);
    }

    private ObjectData GetObjectData(Transform obj)
    {
        ObjectData objectData = new ObjectData
        {
            name = obj.name,
            position = obj.position,
            rotation = obj.rotation,
            scale = obj.localScale,
            spriteName = GetSpriteName(obj),
            sortingOrder = GetSortingOrder(obj), // レイヤー順序を取得
            animatorControllerName = GetAnimatorControllerName(obj),
            scriptNames = GetAttachedScriptNames(obj),
            children = new List<ObjectData>()
        };

        foreach (Transform child in obj)
        {
            objectData.children.Add(GetObjectData(child));
        }

        return objectData;
    }

    private string GetSpriteName(Transform obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        return spriteRenderer != null && spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "";
    }

    private int GetSortingOrder(Transform obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        return spriteRenderer != null ? spriteRenderer.sortingOrder : 0;
    }

    private string GetAnimatorControllerName(Transform obj)
    {
        Animator animator = obj.GetComponent<Animator>();
        return animator != null && animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "";
    }

    private List<string> GetAttachedScriptNames(Transform obj)
    {
        List<string> scriptNames = new List<string>();
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            scriptNames.Add(script.GetType().Name);
        }
        return scriptNames;
    }

    public void LoadScene()
    {
        if (!File.Exists(savePath))
        {
            UnityEngine.Debug.LogError("Scene data file not found: " + savePath);
            return;
        }

        string json = File.ReadAllText(savePath);
        SceneData sceneData = JsonUtility.FromJson<SceneData>(json);

        foreach (var objectData in sceneData.objects)
        {
            LoadObjectData(objectData, null);
        }
    }

    private void LoadObjectData(ObjectData data, Transform parent)
    {
        GameObject newObject = new GameObject(data.name);
        newObject.transform.position = data.position;
        newObject.transform.rotation = data.rotation;
        newObject.transform.localScale = data.scale;

        if (!string.IsNullOrEmpty(data.spriteName))
        {
            SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + data.spriteName);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.sortingOrder = data.sortingOrder; // レイヤー順序を復元
            }
        }

        if (!string.IsNullOrEmpty(data.animatorControllerName))
        {
            Animator animator = newObject.AddComponent<Animator>();
            RuntimeAnimatorController animatorController = Resources.Load<RuntimeAnimatorController>("Animators/" + data.animatorControllerName);
            if (animatorController != null)
            {
                animator.runtimeAnimatorController = animatorController;
            }
        }

        foreach (var scriptName in data.scriptNames)
        {
            System.Type scriptType = System.Type.GetType(scriptName);
            if (scriptType != null)
            {
                newObject.AddComponent(scriptType);
            }
        }

        if (parent != null)
        {
            newObject.transform.SetParent(parent);
        }

        foreach (var childData in data.children)
        {
            LoadObjectData(childData, newObject.transform);
        }
    }
}
