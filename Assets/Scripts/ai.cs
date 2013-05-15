using UnityEngine;
using System.Collections;
using Othello;

public class ai : MonoBehaviour {
	
	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	// メインコンポーネントキャッシュ用/
	main compMain = null;
	
	bool IsThinking = false;
	
	// Use this for initialization
	void Start () {
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
		yield return new WaitForSeconds(0.5f+Random.Range(0f, 0f));

		string place = "";
		ArrayList PutableList = new ArrayList();
		if (Board.Instance().CheckPutable(compMain.GetPieceSide(), ref PutableList)) {
			Board.Position v = (Board.Position)PutableList[Random.Range(0,PutableList.Count-1)];
			if(Board.Instance().posToCode(v.x, v.y, out place))
			{
				compMenu.oms.WritePutList(place);
				if (compMenu.getLockType() != menu.LOCK_TYPE.FREE) {
					compMenu.oms.PutPiece(compMenu.GetYourID(), place);
				}
				IsThinking = false;
			}
		}
	}

	void OnGUI () {
	}
}

