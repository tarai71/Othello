using UnityEngine;
using System.Collections;
using Othello;

public class controller : MonoBehaviour {

	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	// メインコンポーネントキャッシュ用/
	main compMain = null;

	// Use this for initialization
	void Start () {
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		compMain = GetComponent<main>();
	}
	
	// Update is called once per frame
	public void UpdateHuman () {
		if (compMain.IsMySide()) {
			if (!compMain.IsAI()) {
				if (Input.GetButtonDown("Fire1")) {
					Vector3 screenPoint = Input.mousePosition;		
					screenPoint.z = Camera.main.transform.position.y;
		 			Vector3 v = Camera.main.ScreenToWorldPoint(screenPoint);
					int key_x = Mathf.FloorToInt(v.x) + 4;
					int key_y = Mathf.FloorToInt(v.z) + 4;
					if (key_x < 0 || key_y < 0 || key_x > 7 || key_y > 7) {
						return;
					}
					string place;
					if(Board.Instance().posToCode(key_x, key_y, out place))
					{
						if(Board.Instance().IsPutEnable(compMain.GetPieceSide(), key_x, key_y))
						{
							//compMenu.oms.WritePutList(place);
				// 駒を置く/
				if(Board.Instance().putPiece(compMain.GetPieceSide(), key_x, key_y))
				{
					// 駒オブジェクトを置く/
					compMain.pieceList[key_x, key_y].Enabled(true);
					compMain.pieceList[key_x, key_y].SetHight(5f);
					if(compMain.GetPieceSide() == Piece.TYPE.Black)
						compMain.pieceList[key_x, key_y].ToBlack(false);
					else
						compMain.pieceList[key_x, key_y].ToWhite(false);
				}
				
				// ターンを初期化/
				compMain.InitializeTurn();
							if (compMenu.getLockType() != menu.LOCK_TYPE.FREE) {
								compMenu.oms.PutPiece(compMenu.GetYourID(), place);
							}
						}
					}
				}
			}
		}
	}
}
