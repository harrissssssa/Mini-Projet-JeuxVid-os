using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCamera : MonoBehaviour {

    GameObject player;
    float cameraHeight = 2f; 

    // Use this for initialization
    void Start () {
        player = GameObject.Find("player");

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.W)&& cameraHeight <= 3f)
        {
            cameraHeight += 0.01f;
        }

        if (Input.GetKey(KeyCode.S) && cameraHeight >=-0f)
        {
            cameraHeight -= 0.01f;
        }
        transform.position = new Vector3(player.transform.position.x, cameraHeight, -8);


    }
}
