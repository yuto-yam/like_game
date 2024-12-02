using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 新しい武器に更新するかどうかを制御するスクリプト
public class DropRenewer : MonoBehaviour
{
    ParameterDifiner parameterdifiner;
    PlayerDataHolder playerdataholder;

    [SerializeField] GameObject CurrentWeaponPrefab;
    [SerializeField] GameObject NewWeaponPrefab;

    // Start is called before the first frame update
    void Start()
    {
        parameterdifiner = GameManager.Instance.GetComponent<ParameterDifiner>();
        playerdataholder = GameManager.Instance.GetComponent<PlayerDataHolder>();

        CurrentWeaponPrefab = playerdataholder.WeaponPrefab;
        NewWeaponPrefab = playerdataholder.NewWeaponPrefab;

        parameterdifiner.IsFromBattle = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            playerdataholder.player_weapon = playerdataholder.new_weapon;
            playerdataholder.WeaponPrefab = NewWeaponPrefab;
            Destroy(CurrentWeaponPrefab);
            SceneManager.LoadScene("Maze");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Destroy(NewWeaponPrefab);
            SceneManager.LoadScene("Maze");
        }
    }
}
