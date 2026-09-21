using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sky : MonoBehaviour {

    GameObject player;

    

	// Use this for initialization
	void Start () {
        player = GameObject.Find("player");
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = new Vector3(player.transform.position.x, 8, 22);
     


    }
}
