using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHolder : MonoBehaviour
{
    public ParameterController.Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = new ParameterController.Player("player", 1, 50, 10);
        //UnityEngine.Debug.Log("name:" + player.name + " " + "Lv:" + player.Lv + " " + "HP/ATK:" + player.HP + "/" + player.ATK);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
