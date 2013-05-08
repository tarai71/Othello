using UnityEngine;
using System.Collections;
	
public class ai : MonoBehaviour {
	
	// コネクトコンポーネントキャッシュ用/
	connect compConnect = null;
	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	// メインコンポーネントキャッシュ用/
	main compMain = null;
	
	bool IsThinking = false;
	
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
				if (!IsThinking) {
					IsThinking = true;
					StartCoroutine("defaultAI");
				}
			}
		}
	}
	
	IEnumerator defaultAI () {
		yield return new WaitForSeconds(1.0f+Random.Range(0f, 5f);

		string place = "";
		ArrayList enablePutList = new ArrayList();
		if (compMain.checkEnablePut(ref enablePutList)) {
			Vector2 v = (Vector2)enablePutList[Random.Range(0,enablePutList.Count-1)];
			place = compMenu.posToCode(v);
			string put = "{\"type\":\"put\",\"id\":\"" + compMenu.getYourID().ToString() + "\",\"place\":\"" + place + "\"}";
			compConnect.putPiece(put);
			if (compMenu.getLockType() != menu.LOCK_TYPE.FREE) {
				compConnect.Send(put);
			}
			IsThinking = false;
		}
	}

	void OnGUI () {
	}
}

