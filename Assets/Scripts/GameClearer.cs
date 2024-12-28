using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ゲームクリアか継続かを選ぶスクリプト
public class GameClearer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            UnityEngine.Debug.Log($"タイトル画面に行きたい");
            SceneManager.LoadScene("Title");
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene("Maze");
        }
    }
}
