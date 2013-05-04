using UnityEngine;
using System.Collections;

public class controller : MonoBehaviour {

	connect compConnect = null;
	menu compMenu = null;
	main compMain = null;

	// Use this for initialization
	void Start () {
		compConnect = GameObject.Find("Menu").GetComponent<connect>();
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		compMain = GetComponent<main>();
	}
	
	// Update is called once per frame
	void Update () {
		if (compMain.IsMySide()) {
			if (Input.GetButtonDown("Fire1")) {
				Vector3 screenPoint = Input.mousePosition;		
				screenPoint.z = 10;
	 			Vector3 v = Camera.main.ScreenToWorldPoint(screenPoint);
				float key_x = Mathf.Floor(v.x) + 4.0f;
				float key_y = Mathf.Floor(v.z) + 4.0f;
				if (key_x < 0 || key_y < 0 || key_x > 7 || key_y > 7) {
					return;
				}
				string put = "{\"type\":\"put\",\"id\":\"" + compMenu.getYourID().ToString() + "\",\"place\":\"" + GameObject.FindWithTag("GameController").GetComponent<main>().posToCode(new Vector2(key_x, key_y)) + "\"}";
				compConnect.putPiece(put);
				compConnect.Send(put);
			}
		}
	}
}
