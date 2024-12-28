using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// BGMを再生するスクリプト
public class BGMPlayer : MonoBehaviour
{
    public AudioSource audioSource; // BGMを再生するAudioSource
    public AudioClip[] bgmClips; // 各シーンごとのBGM
    public string[] SceneNames = // シーン名のリスト、この順にBGMをセット
    {
        "Opening",  // 0
        "Maze",     // 1
        "Battle",   // 2
        "Drop",     // 3
        "GameOver", // 4
        "GameClear" // 5
    };

    private float fadeDuration = 1.0f; // フェードイン/フェードアウトの時間

    public void PlayBGMForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        // デフォルトでループはoff
        audioSource.loop = false;
        
        // シーン名に対応するBGMを検索
        for (int i = 0; i < SceneNames.Length; i++)
        {
            if (SceneNames[i] == currentSceneName)
            {
                // 違うBGMを再生する場合のみ切り替え
                if (audioSource.clip != bgmClips[i])
                {
                    StartCoroutine(FadeToNewClip(bgmClips[i]));
                    // MazeとBattleではループon
                    if (currentSceneName == "Maze" ||  currentSceneName == "Battle")
                    {
                        audioSource.loop = true;
                    }
                }
                return;
            }
        }

        // 対応するBGMが見つからない場合は停止
        StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeToNewClip(AudioClip newClip)
    {
        // フェードアウト
        yield return StartCoroutine(FadeOut());

        // 新しいクリップをセットして再生
        audioSource.clip = newClip;
        audioSource.Play();

        // フェードイン
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0;
    }

    private IEnumerator FadeIn()
    {
        float startVolume = 0;
        audioSource.volume = 0;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 1, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1;
    }

    private IEnumerator FadeOutAndStop()
    {
        yield return StartCoroutine(FadeOut());
        audioSource.Stop();
    }
}
