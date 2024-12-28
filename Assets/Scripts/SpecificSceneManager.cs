using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// GameClear, GameOverでシーン遷移を行うスクリプト
public class SpecificSceneManager : MonoBehaviour
{
    string currentSceneName; // 今のシーン名

    // Start is called before the first frame update
    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        UnityEngine.Debug.Log($"現在のシーン名：{currentSceneName}");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSceneName == "GameClear")
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SceneManager.LoadScene("Maze");
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                GameManager.ResetGameManager();
                SceneManager.LoadScene("Title");
            }
        }
        if (currentSceneName == "GameOver")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.ResetGameManager();
                SceneManager.LoadScene("Title");
            }
        }
    }
}
