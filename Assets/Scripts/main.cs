using UnityEngine;
using System.Collections;
	
public class main : MonoBehaviour {
	
	public enum GAME_STATUS {
		None = 0,
		LocalPlay,
		NetworkPlay,
		GameOver,
		TimeOver,
		WinByDefault
	}
	
	public enum PIECE_TYPE {
		Empty = 0,
		Black,
		White
	}
	
	public GameObject piecePrefab;
	public GameObject markerPrefab;
	public GameObject guidePrefab;
	public GUIStyle labelStyleScoreBlack;
	public GUIStyle labelStyleScoreWhite;
	public GUIStyle labelStyleScoreName;
	public GUIStyle labelStyleGameOver;
	public GUIStyle labelStyleTimer;
	
	GameObject[,] guide = new GameObject[2,8];
	string[,] guideLitteral = new string[,]{
		{"a","b","c","d","e","f","g","h"},
		{"1","2","3","4","5","6","7","8"}
	};
	
	PIECE_TYPE[,] board = new PIECE_TYPE[8,8];
	GameObject[,] pieceList = new GameObject[8,8];
	PIECE_TYPE pieceType = PIECE_TYPE.Empty;
	int white = 0;
	int black = 0;
	GAME_STATUS gamestatus = GAME_STATUS.None;
	ArrayList markerList = new ArrayList();
		
	float TimeLimit;
	float startTime;

	menu compMenu = null;
	
	// Use this for initialization
	void Start () {
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		
		gamestatus = (compMenu.getLockType() == menu.LOCK_TYPE.FREE)? GAME_STATUS.LocalPlay : GAME_STATUS.NetworkPlay;

		for (int i=0; i<board.GetLength(0); i++) {
			for (int j=0; j<board.GetLength(1); j++) {
				board[i,j] = PIECE_TYPE.Empty;
			}
		}
		pieceType = PIECE_TYPE.White; putPiece(new Vector2(3,4));
		pieceType = PIECE_TYPE.Black; putPiece(new Vector2(3,3));
		pieceType = PIECE_TYPE.Black; putPiece(new Vector2(4,4));
		pieceType = PIECE_TYPE.White; putPiece(new Vector2(4,3));
	
		TimeLimit = compMenu.getLimitTime();
		
		float x=-3.5f,y=+4.5f;
		for (int i=0; i<8; i++) {
			guide[0,i] = (GameObject)Instantiate(guidePrefab, new Vector3(0,0,0), Quaternion.identity);
			guide[0,i].guiText.text = guideLitteral[0,i];
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x,0f,y));
			guide[0,i].transform.position = new Vector3(screenPos.x/Screen.width,screenPos.y/Screen.height,0);
			guide[0,i].guiText.fontSize = 20;
			guide[0,i].guiText.anchor = TextAnchor.MiddleCenter;
			guide[0,i].guiText.alignment = TextAlignment.Center;
			x+=1f;
		}
		x=-4.5f;y=+3.5f;
		for (int i=0; i<8; i++) {
			guide[1,i] = (GameObject)Instantiate(guidePrefab, new Vector3(0,0,0), Quaternion.identity);
			guide[1,i].guiText.text = guideLitteral[1,i];
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x,0f,y));
			guide[1,i].transform.position = new Vector3(screenPos.x/Screen.width,screenPos.y/Screen.height,0);
			guide[1,i].guiText.fontSize = 20;
			guide[1,i].guiText.anchor = TextAnchor.MiddleCenter;
			guide[1,i].guiText.alignment = TextAlignment.Center;
			y-=1f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		byte alfa;
		alfa = (Time.time % 1.0f < 0.5f)? (byte)((Time.time % 1.0f) * 255 + 127) : (byte)((1.0f - Time.time % 1.0f) * 255 + 127);
		
		foreach(GameObject obj in markerList) {
			if(obj) {
				obj.renderer.enabled = IsMySide();
				obj.renderer.material.color = new Color32(209,221, 48,alfa);
			}
		}
		
		if(pieceType == PIECE_TYPE.Black) {
			labelStyleScoreBlack.normal.textColor = new Color32(209,221, 48,alfa);
			labelStyleScoreWhite.normal.textColor = new Color32(193,193,193,255);
		} else if(pieceType == PIECE_TYPE.White) {
			labelStyleScoreBlack.normal.textColor = new Color32(193,193,193,255);
			labelStyleScoreWhite.normal.textColor = new Color32(209,221, 48,alfa);
		}
		
		float x=-3.5f,y=+4.5f;
		for (int i=0; i<8; i++) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x,0f,y));
			guide[0,i].transform.position = new Vector3(screenPos.x/Screen.width,screenPos.y/Screen.height,0);
			x+=1f;
		}
		x=-4.5f;y=+3.5f;
		for (int i=0; i<8; i++) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x,0f,y));
			guide[1,i].transform.position = new Vector3(screenPos.x/Screen.width,screenPos.y/Screen.height,0);
			y-=1f;
		}
		
		if (TimeLimit > 0f) {
			if (Time.time - startTime >= TimeLimit) {
				StartCoroutine("TimeOver");
			}
		}
		
		// 対戦相手がいなくなったら不戦勝/
		if (gamestatus == GAME_STATUS.NetworkPlay && compMenu.getLockType() == menu.LOCK_TYPE.FREE) {
			StartCoroutine("WinByDefault");
		}
	}


	// 駒を盤に置く/
	void putPiece(Vector2 key)
	{
		if (key.x < 0 || key.y < 0 || key.x > 7 || key.y > 7) {
			return;
		}
		if (board[(int)key.x,(int)key.y] != PIECE_TYPE.Empty) {
			return;
		}
		if (gamestatus != GAME_STATUS.LocalPlay && gamestatus != GAME_STATUS.NetworkPlay) {
			return;
		}
			
		Debug.Log("put " + posToCode(key));
		
		board[(int)key.x,(int)key.y] = pieceType;
		bool changeFlag = updateBoard(key, true);
	
		// initial position
		var initialFlag = false;
		if (key == new Vector2(3,3) || key == new Vector2(3,4) || key == new Vector2(4,3) || key == new Vector2(4,4)) {
			initialFlag = true;
		}
		if (changeFlag || initialFlag) {
			calcStatus();
			var rotation = transform.rotation;
			var position = new Vector3(key.x -4.0f + 0.5f, 0.25f, key.y -4.0f + 0.5f);
			//if (pieceType == 1) {
			//	rotation = Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
			//} else {
			//	rotation = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
			//}
			pieceList[(int)key.x, (int)key.y] = (GameObject)Instantiate(piecePrefab, position, rotation);
			for (int i=0; i<board.GetLength(0); i++) {
				for (int j=0; j<board.GetLength(1); j++) {
					if (board[i,j] == PIECE_TYPE.Black) {
						pieceList[i, j].renderer.material.color = new Color(0,0,0,255);
					} else if (board[i,j] == PIECE_TYPE.White) {
						pieceList[i, j].renderer.material.color = new Color(255,255,255,255);
					}
				}
			}
			pieceType = (pieceType == PIECE_TYPE.Black)? PIECE_TYPE.White : PIECE_TYPE.Black;
			// 置くところが無いかチェック/
			foreach(Object obj in markerList) {
				Object.Destroy(obj);
			}
			ArrayList enablePutList = new ArrayList();
			if (!checkEnablePut(ref enablePutList) && !initialFlag) {
				pieceType = (pieceType == PIECE_TYPE.Black)? PIECE_TYPE.White : PIECE_TYPE.Black;
				// 攻守交代2回してどこも置けなかったらそのゲームは終了/
				if (!checkEnablePut(ref enablePutList) && !initialFlag) {
					StartCoroutine("GameOver");
				}
			}
			startTime = Time.time;
			
			if (compMenu.getGuideEnable()) {
				foreach(Vector2 v in enablePutList) {
					position = new Vector3(v.x -4.0f + 0.5f, 0.201f, v.y -4.0f + 0.5f);
					markerList.Add(Instantiate(markerPrefab, position, Quaternion.identity));
				}
			}
			enablePutList.Clear();
		} else {
			Debug.Log("cannot put here");
			board[(int)key.x,(int)key.y] = PIECE_TYPE.Empty;
		}

		// for debug
		string _s = "";
		for (int i=0; i<board.GetLength(0); i++) {
			_s = _s + '\n';
			for (int j=0; j<board.GetLength(1); j++) {
				if(board[i,j] == PIECE_TYPE.Black) {
					_s = _s + 'X';
				} else if(board[i,j] == PIECE_TYPE.White) {
					_s = _s + 'O';
				} else {
					_s = _s + '&';
				}
			}
		}
		Debug.Log(_s);
	}
	
	IEnumerator GameOver() {
		Debug.Log("game over");
		gamestatus = GAME_STATUS.GameOver;
		yield return new WaitForSeconds(2.0f);
		//while (!Input.GetButtonDown("Fire1") || Input.touches.Length > 0) yield return;

		compMenu.enabled = true;
	}
	
	IEnumerator TimeOver() {
		Debug.Log("time over");
		gamestatus = GAME_STATUS.TimeOver;
		yield return new WaitForSeconds(2.0f);
		//while (!Input.GetButtonDown("Fire1") || Input.touches.Length > 0) yield return;

		compMenu.enabled = true;
	}
	
	IEnumerator WinByDefault() {
		Debug.Log("Win by default");
		gamestatus = GAME_STATUS.WinByDefault
;
		yield return new WaitForSeconds(2.0f);
		//while (!Input.GetButtonDown("Fire1") || Input.touches.Length > 0) yield return;
		
		compMenu.enabled = true;
		Application.LoadLevel("Empty");
	}
	
	// 置ける場所があるかどうか検索/
	bool checkEnablePut(ref ArrayList list) {
		for (int x=0; x<board.GetLength(0); x++) {
			for (int y=0; y<board.GetLength(1); y++) {
				if (board[x,y] == PIECE_TYPE.Empty && updateBoard(new Vector2(x,y),false)) {
					list.Add(new Vector2(x,y));
				}
			}
		}
		return (list.Count > 0);
	}

	// 盤面の更新、updateFlag が false なら/
	// その場所に置けるかどうかのチェックだけ/
	bool updateBoard(Vector2 key, bool updateFlag) {
		int ix = 0; int iy = 0;
		
		ArrayList[] revList = new ArrayList[8];
		var changeFlag = false;
		// horizon
		ix = (int)key.x + 1; iy = (int)key.y;
		revList[0] = new ArrayList();
		while (true) {
			if (ix >= board.GetLength(0)) {
				revList[0].Clear();
				break;
			}
			if (board[ix,iy] != PIECE_TYPE.Empty && board[ix,iy] != pieceType) {
				revList[0].Add(new Vector2(ix, iy));
			} else if (revList[0].Count > 0 && board[ix,iy] != PIECE_TYPE.Empty) {
				changeFlag = true;
				break;
			} else {
				revList[0].Clear();
				break;
			}
			ix += 1;
		}
		
		ix = (int)key.x - 1; iy = (int)key.y;
		revList[1] = new ArrayList();
		while (true) {
			if (ix < 0) {
				revList[1].Clear();
				break;
			}
			if (board[ix,iy] != PIECE_TYPE.Empty && board[ix,iy] != pieceType) {
				revList[1].Add(new Vector2(ix,iy));
			} else if (revList[1].Count > 0 && board[ix,iy] != PIECE_TYPE.Empty) {
				changeFlag = true;
				break;
			} else {
				revList[1].Clear();
				break;
			}
			ix -= 1;
		}
	
		// vertical
		ix = (int)key.x; iy = (int)key.y + 1;
		revList[2] = new ArrayList();
		while (true) {
			if (iy >= board.GetLength(1)) {
				revList[2].Clear();
				break;
			}
			if (board[ix,iy] != PIECE_TYPE.Empty && board[ix,iy] != pieceType) {
				revList[2].Add(new Vector2(ix, iy));
			} else if (revList[2].Count > 0 && board[ix,iy] != PIECE_TYPE.Empty) {
				changeFlag = true;
				break;
			} else {
				revList[2].Clear();
				break;
			}
			iy += 1;
		}
	
		ix = (int)key.x; iy = (int)key.y - 1; 
		revList[3] = new ArrayList();
		while (true) {
			if (iy < 0) {
				revList[3].Clear();
				break;
			}
			if (board[ix,iy] != PIECE_TYPE.Empty && board[ix,iy] != pieceType) {
				revList[3].Add(new Vector2(ix, iy));
			} else if (revList[3].Count > 0 && board[ix,iy] != PIECE_TYPE.Empty) {
				changeFlag = true;
				break;
			} else {
				revList[3].Clear();
				break;
			}
			iy -= 1;
		}
	
		// cross
		ix = (int)key.x + 1; iy = (int)key.y + 1;
		revList[4] = new ArrayList();
		while (true) {
			if (ix >= board.GetLength(0) || iy >= board.GetLength(1)) {
				revList[4].Clear();
				break;
			}
			if (board[ix,iy] != PIECE_TYPE.Empty && board[ix,iy] != pieceType) {
				revList[4].Add(new Vector2(ix,iy));
			} else if (revList[4].Count > 0 && board[ix,iy] != PIECE_TYPE.Empty) {
				changeFlag = true;
				break;
			} else {
				revList[4].Clear();
				break;
			}
			iy += 1; ix += 1;
		}
	
		revList[5] = new ArrayList();
		ix = (int)key.x + 1; iy = (int)key.y - 1;
		while (true) {
			if (ix >= board.GetLength(0) || iy < 0 ) {
				revList[5].Clear();
				break;
			}
			if (board[ix,iy] != PIECE_TYPE.Empty && board[ix,iy] != pieceType) {
				revList[5].Add(new Vector2(ix,iy));
			} else if (revList[5].Count > 0 && board[ix,iy] != PIECE_TYPE.Empty) {
				changeFlag = true;
				break;
			} else {
				revList[5].Clear();
				break;
			}
			ix += 1; iy -= 1;
		}
		
	
		revList[6] = new ArrayList();
		ix = (int)key.x - 1; iy = (int)key.y + 1;
		while (true) {
			if (ix < 0 || iy >= board.GetLength(1)) {
				revList[6].Clear();
				break;
			}
			if (board[ix,iy] != PIECE_TYPE.Empty && board[ix,iy] != pieceType) {
				revList[6].Add(new Vector2(ix,iy));
			} else if (revList[6].Count > 0 && board[ix,iy] != PIECE_TYPE.Empty) {
				changeFlag = true;
				break;
			} else {
				revList[6].Clear();
				break;
			}
			ix -= 1; iy += 1;
		}
	
		revList[7] = new ArrayList();
		ix = (int)key.x - 1; iy = (int)key.y - 1;
		while (true) {
			if (ix < 0 || iy < 0) {
				revList[7].Clear();
				break;
			}
			if (board[ix,iy] != PIECE_TYPE.Empty && board[ix,iy] != pieceType) {
				revList[7].Add(new Vector2(ix,iy));
			} else if (revList[7].Count > 0 && board[ix,iy] != PIECE_TYPE.Empty) {
				changeFlag = true;
				break;
			} else {
				revList[7].Clear();
				break;
			}
			ix -= 1; iy -= 1;
		}
	
		if (changeFlag) {
			if (updateFlag) {
				foreach (ArrayList val in revList) {
					foreach (Vector2 v in val) {
						pieceList[(int)v.x, (int)v.y].transform.rotation *= Quaternion.AngleAxis(180, new Vector3(1,0,0));
						board[(int)v.x, (int)v.y] = (board[(int)v.x, (int)v.y] == PIECE_TYPE.Black) ? PIECE_TYPE.White : PIECE_TYPE.Black;
					} 
				}
			}
			return true;
		} else {
			return false;
		}
	}

	void calcStatus() {
		int _white = 0;
		int _black = 0;
		for (int x=0; x<board.GetLength(0); x++) {
			for (int y=0; y<board.GetLength(1); y++) {
				if (board[x,y] == PIECE_TYPE.Black) {
					_black += 1;
				} else if (board[x,y] == PIECE_TYPE.White) {
					_white += 1;
				}
			}
		}
		white = _white;
		black = _black;
	}
	
	public string posToCode(Vector2 pos) {
		string code = ((char)('a'+pos.x)).ToString() + ((char)('0'+(8-pos.y))).ToString();
		return code;
	}

	public Vector2 codeToPos(string code) {
		Vector2 pos = new Vector2((int)code[0]-'a', 8-((int)code[1]-'0'));
		return pos;
	}
	
	void OnGUI() {
		GUI.Box(new Rect(10,10,100,80), StringTable.BLACK);
		GUI.Label(new Rect(10,10,100,80), black.ToString("d2"), labelStyleScoreBlack);
		GUI.Box(new Rect(10,100,100,80), StringTable.WHITE);
		GUI.Label(new Rect(10,100,100,80), white.ToString("d2"), labelStyleScoreWhite);
		switch (compMenu.getLockType()) {
		case menu.LOCK_TYPE.LOCK:
			GUI.Label(new Rect(60,70,100,80), compMenu.getMyName(), labelStyleScoreName);
			GUI.Label(new Rect(60,160,100,80), compMenu.getYourName(), labelStyleScoreName);
			break;
		case menu.LOCK_TYPE.LOCKED:
			GUI.Label(new Rect(60,70,100,80), compMenu.getYourName(), labelStyleScoreName);
			GUI.Label(new Rect(60,160,100,80), compMenu.getMyName(), labelStyleScoreName);
			break;
		}
		
		if (TimeLimit > 0f) {
			float restTime = TimeLimit - (Time.time - startTime);
			if (restTime < 0f) restTime = 0f;
			GUI.Box(new Rect(10,220,150,80), StringTable.TIMER);
			GUI.Label(new Rect(10,220,150,80), restTime.ToString("f02"), labelStyleTimer);
			if (restTime < 3f) {
				labelStyleTimer.normal.textColor = new Color32(230,0,0,255);
			} else {
				labelStyleTimer.normal.textColor = new Color32(57,196,225,255);
			}
		}
		
		Rect rect_gameover = new Rect(Screen.width / 2 - 300, Screen.height / 2 - 50, 600, 100);
		switch (gamestatus) {
		case GAME_STATUS.GameOver:
			string result = "";
			if (white > black) {
				result = StringTable.WIN_WHITE;
			} else if (white < black) {
				result = StringTable.WIN_BLACK;
			} else {
				result = StringTable.DRAW;
			}
			GUI.Box(rect_gameover, "");
			GUI.Box(rect_gameover, "");
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		case GAME_STATUS.TimeOver:
			result = "";
			if (pieceType == PIECE_TYPE.Black) {
				result = StringTable.WIN_WHITE;
			} else if (pieceType == PIECE_TYPE.White){
				result = StringTable.WIN_BLACK;
			} else {
				result = StringTable.DRAW;
			}
			GUI.Box(rect_gameover, "");
			GUI.Box(rect_gameover, "");
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		case GAME_STATUS.WinByDefault:
			result = StringTable.ESCAPE;
			GUI.Box(rect_gameover, "");
			GUI.Box(rect_gameover, "");
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		}

	}

	public PIECE_TYPE getPieceType ()
	{
		return pieceType;
	}

	public bool IsMySide ()
	{
		menu.LOCK_TYPE t = compMenu.getLockType();
		if (t == menu.LOCK_TYPE.LOCK) {
			return (getPieceType() == PIECE_TYPE.Black);
		} else if (t == menu.LOCK_TYPE.LOCKED) {
			return (getPieceType() == PIECE_TYPE.White);
		}

		return false;
	}
}
