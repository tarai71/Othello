using UnityEngine;
using System.Collections;
	
public class ai : MonoBehaviour {
	
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
			if (compMain.IsAI()) {
				string place;
				if (ai_(out place)) {
					string put = "{\"type\":\"put\",\"id\":\"" + compMenu.getYourID().ToString() + "\",\"place\":\"" + place + "\"}";
					compConnect.putPiece(put);
					if (compMenu.getLockType() != menu.LOCK_TYPE.FREE) {
						compConnect.Send(put);
					}
				}
			}
		}
	}

	bool ai_ (out string place) {
		place = "";

		ArrayList enablePutList = new ArrayList();
		if (compMain.checkEnablePut(ref enablePutList)) {
			Vector2 v = (Vector2)enablePutList[Random.Range(0,enablePutList.Count-1)];
			place = GameObject.FindWithTag("GameController").GetComponent<main>().posToCode(v);
			return true;
		}
		return false;
	}

	void OnGUI () {
	}
}
