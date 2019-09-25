using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShowLocalCanvas : NetworkBehaviour {

	void Start () {
        //Only show the canvas of the local player.
        if (isLocalPlayer) GetComponentInChildren<Canvas>().enabled = true;
        else GetComponentInChildren<Canvas>().enabled = false;
    }
	

}
