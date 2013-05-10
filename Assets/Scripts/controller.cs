using UnityEngine;
using System.Collections;
using Othello;

public class controller : MonoBehaviour {

	// コネクトコンポーネントキャッシュ用/
	connect compConnect = null;
	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	// メインコンポーネントキャッシュ用/
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
			if (!compMain.IsAI()) {
				if (Input.GetButtonDown("Fire1")) {
					Vector3 screenPoint = Input.mousePosition;		
					screenPoint.z = 10;
		 			Vector3 v = Camera.main.ScreenToWorldPoint(screenPoint);
					int key_x = Mathf.FloorToInt(v.x) + 4;
					int key_y = Mathf.FloorToInt(v.z) + 4;
					if (key_x < 0 || key_y < 0 || key_x > 7 || key_y > 7) {
						return;
					}
					string place;
					if(Board.Instance().posToCode(key_x, key_y, out place))
					{
						string put = "{\"type\":\"put\",\"id\":\"" + compMenu.GetYourID().ToString() + "\",\"place\":\"" + place + "\"}";
						compConnect.putPiece(put);
						if (compMenu.getLockType() != menu.LOCK_TYPE.FREE) {
							compConnect.Send(put);
						}
					}
				}
			}
		}
	}
}
