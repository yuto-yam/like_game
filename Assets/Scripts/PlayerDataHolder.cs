using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

//���ۂɃX�e�[�^�X���Ǘ�����N���X�@DDOS�z��
public class PlayerDataHolder : MonoBehaviour
{
    public ParameterController.Player player;
    public ParameterController.Weapon player_weapon;

    [SerializeField] GameObject WeaponObj; //�������X�g������
    public GameObject WeaponPrefab; //��ʏ�ł̕���

    // Start is called before the first frame update
    void Start()
    {
        player = new ParameterController.Player("player", 1, 50, 10);
        
        player_weapon = new ParameterController.Weapon("tmp_sword", 10, ParameterController.SKILL.MAGIC);
        WeaponPrefab = Instantiate(WeaponObj, new Vector3(0, 0, 0), Quaternion.identity);
        UnityEngine.Debug.Log(player_weapon.GetName());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
