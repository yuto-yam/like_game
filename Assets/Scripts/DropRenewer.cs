using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 新しい武器に更新するかどうかを制御するスクリプト
public class DropRenewer : MonoBehaviour
{
    ParameterDifiner parameterdifiner;
    PlayerDataHolder playerdataholder;
    SoundEffectPlayer soundEffectPlayer;

    [SerializeField] GameObject CurrentWeaponPrefab;
    [SerializeField] GameObject NewWeaponPrefab;

    // Start is called before the first frame update
    void Start()
    {
        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();
        playerdataholder = GameManager.Instance.GetComponent<PlayerDataHolder>();
        soundEffectPlayer = GameManager.Instance.GetComponent<SoundEffectPlayer>();

        CurrentWeaponPrefab = playerdataholder.WeaponPrefab;
        NewWeaponPrefab = playerdataholder.NewWeaponPrefab;

        parameterdifiner.IsFromBattle = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            soundEffectPlayer.DecideSEPlay();
            playerdataholder.player_weapon = playerdataholder.new_weapon;
            playerdataholder.WeaponPrefab = NewWeaponPrefab;
            Destroy(CurrentWeaponPrefab);
            // 9階層(ボス3回)で一旦クリアへ
            if (parameterdifiner.MazeCount == 9)
            {
                SceneManager.LoadScene("GameClear");
            }
            else { SceneManager.LoadScene("Maze"); }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Destroy(NewWeaponPrefab);
            // 9階層(ボス3回)で一旦クリアへ
            if (parameterdifiner.MazeCount == 9)
            {
                SceneManager.LoadScene("GameClear");
            }
            else { SceneManager.LoadScene("Maze"); }
        }
    }
}
