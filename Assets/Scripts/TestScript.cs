using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject parent; 
    MazeSaver mazeSaver;

    // Start is called before the first frame update
    void Start()
    {
        mazeSaver = parent.GetComponent<MazeSaver>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Translate(transform.right * 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            mazeSaver.SaveScene();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            mazeSaver.LoadScene();
        }
    }
}
