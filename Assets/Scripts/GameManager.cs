using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using static System.Net.Mime.MediaTypeNames;

// GameManagerをDontDestroyOnLoadに登録するスクリプト
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移でも破棄されないようにする
        }
        else
        {
            Destroy(gameObject); // 複数のインスタンスを防止
        }
    }
}
