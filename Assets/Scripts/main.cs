using UnityEngine;
using System.Collections;
	
using Othello;

public class main : MonoBehaviour {
	
	// Prefab定義/
	public GameObject piecePrefab;
	public GameObject markerPrefab;
	public GameObject guidePrefab;
	
	// GUIStyle定義/
	public GUIStyle labelStyleScoreBlack;
	public GUIStyle labelStyleScoreWhite;
	public GUIStyle labelStyleScoreName;
	public GUIStyle labelStyleGameOver;
	public GUIStyle labelStyleTimer;
	public GUIStyle labelStyleLaberl;
	
	// ゲームの状態を列挙/
	public enum GAME_STATUS {
		None = 0,
		LocalPlay,			// ローカル対戦中/
		NetworkPlay,		// 通信対戦中/
		GameOver,			// ゲームオーバー表示中/
		TimeOver,			// タイムオーバー表示中/
		WinByDefault		// 不戦勝表示中/
	}
	
	// ゲーム終了処理クラス/
	class GameEnd {
		public string debug;
		public GAME_STATUS status;
		public float wait;
		
		public GameEnd(string debug, GAME_STATUS status, float wait)
		{
			this.debug = debug;
			this.status = status;
			this.wait = wait;
		}
	}
	
	// 盤面に配置した駒のオブジェクトリスト/
	GameObject[,] pieceList = new GameObject[8,8];
	// 盤面のガイド表示のオブジェクトリスト/
	GameObject[,] guide = new GameObject[2,8];
		
	// 盤面のガイド表示用文字データ/
	string[,] guideLitteral = new string[2,8]{
		{"a","b","c","d","e","f","g","h"},
		{"1","2","3","4","5","6","7","8"}
	};
	// 盤面に配置した着手可能場所マーカーのオブジェクトリスト/
	ArrayList markerList = new ArrayList();
	// 現在の手順/
	Piece.TYPE pieceSide = Piece.TYPE.Black;
	// ゲームの状態/
	GAME_STATUS gamestatus = GAME_STATUS.None;
	// 制限時間/
	float TimeLimit;
	// 残り時間計算用ワーク/
	float startTime;
	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	connect compConnect = null;
		
	// Use this for initialization
	void Start () {
		// メニューコンポーネントをキャッシュ/
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		compConnect = GameObject.Find("Menu").GetComponent<connect>();
		
		// 駒オブジェクト生成/
		for (int i=0; i<8; i++)
		{
			for (int j=0; j<8; j++)
			{
				var rotation = Quaternion.identity;
				var position = new Vector3(j -4.0f + 0.5f, 0.25f, i -4.0f + 0.5f);
				pieceList[j, i] = (GameObject)Instantiate(piecePrefab, position, rotation);
			}
		}
		
		// 盤面の横ガイド表示用オブジェクト生成/
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
		// 盤面の縦ガイド表示用オブジェクト生成/
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
		
		// ゲーム状態初期化/
		gamestatus = (compMenu.getLockType() == menu.LOCK_TYPE.FREE)? GAME_STATUS.LocalPlay : GAME_STATUS.NetworkPlay;
		
		// 盤面初期化/
		Board.Instance().Initialize();

		// メニューから制限時間取得/
		TimeLimit = compMenu.getLimitTime();

		// ターンを初期化/
		InitializeTurn();
	}
	
	// Update is called once per frame
	void Update () {
		
		// 駒オブジェクト表示制御/
		for (int i=0; i<8; i++) {
			for (int j=0; j<8; j++) {
				if (Board.Instance().GetPiece(i,j) == Piece.TYPE.Black) {
					pieceList[i, j].renderer.enabled = true;
					pieceList[i, j].renderer.material.color = new Color(0,0,0,255);
				} else if (Board.Instance().GetPiece(i,j) == Piece.TYPE.White) {
					pieceList[i, j].renderer.enabled = true;
					pieceList[i, j].renderer.material.color = new Color(255,255,255,255);
				} else {
					pieceList[i, j].renderer.enabled = false;
				}
			}
		}
		
		
		
		byte alfa;
		alfa = (Time.time % 1.0f < 0.5f)? (byte)((Time.time % 1.0f) * 255 + 127) : (byte)((1.0f - Time.time % 1.0f) * 255 + 127);
		
		foreach(GameObject obj in markerList) {
			if(obj) {
				obj.renderer.enabled = IsMySide();
				obj.renderer.material.color = new Color32(209,221, 48,alfa);
			}
		}
		
		if(pieceSide == Piece.TYPE.Black) {
			labelStyleScoreBlack.normal.textColor = new Color32(209,221, 48,alfa);
			labelStyleScoreWhite.normal.textColor = new Color32(193,193,193,255);
		} else if(pieceSide == Piece.TYPE.White) {
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
				StartCoroutine("GameOver", new GameEnd("time over", GAME_STATUS.TimeOver, 2.5f));
			}
		}
		
		// 対戦相手がいなくなったら不戦勝/
		if (gamestatus == GAME_STATUS.NetworkPlay && compMenu.getLockType() == menu.LOCK_TYPE.FREE) {
			StartCoroutine("GameOver", new GameEnd("win by default", GAME_STATUS.WinByDefault, 2.5f));
		}

	}

	void LateUpdate () {
		Board.Position pos;
		if (compConnect.ReadPutList(out pos)) {
			
			// 駒を置く/
			Board.Instance().putPiece(pieceSide, (int)pos.x, (int)pos.y);
			
			// ターンを入れ替える/
			pieceSide = (pieceSide == Piece.TYPE.Black)? Piece.TYPE.White : Piece.TYPE.Black;

			// ターンを初期化/
			InitializeTurn();
		}
	}
	
	void InitializeTurn()
	{
		// 計測開始時間をセット/
		startTime = Time.time;
		
		// 着手可能場所マーカーのオブジェクトを削除/
		foreach(Object obj in markerList) {
			Object.Destroy(obj);
		}
		// 着手可能場所チェック/
		ArrayList PutableList = new ArrayList();
		if (!Board.Instance().CheckPutable(pieceSide, ref PutableList)) {
			// 攻守交代2回してどこも置けなかったらそのゲームは終了/
			pieceSide = (pieceSide == Piece.TYPE.Black)? Piece.TYPE.White : Piece.TYPE.Black;
			if (!Board.Instance().CheckPutable(pieceSide, ref PutableList)) {
				StartCoroutine("GameOver", new GameEnd("game over", GAME_STATUS.GameOver, 2.5f));
			}
		}
		// 着手可能場所マーカーのオブジェクトを生成/
		if (compMenu.getGuideEnable()) {
			foreach(Board.Position v in PutableList) {
				Vector3 position = new Vector3(v.x -4.0f + 0.5f, 0.201f, v.y -4.0f + 0.5f);
				markerList.Add(Instantiate(markerPrefab, position, Quaternion.identity));
			}
		}
		PutableList.Clear();
	}
	
	IEnumerator GameOver(GameEnd obj) {
		Debug.Log(obj.debug);
		gamestatus = obj.status;
		yield return new WaitForSeconds(obj.wait);
		//while (!Input.GetButtonDown("Fire1") || Input.touches.Length > 0) yield return;

		compMenu.enabled = true;
		Application.LoadLevel("Empty");
	}

	void OnGUI() {
		int black = Board.Instance().GetBlackPiecies();
		int white = Board.Instance().GetWhitePiecies();
		
		GUI.Label(new Rect(10,20,100,80), StringTable.BLACK, labelStyleLaberl);
		GUI.Label(new Rect(10,10,100,80), black.ToString("d2"), labelStyleScoreBlack);
		GUI.Label(new Rect(10,110,100,80), StringTable.WHITE, labelStyleLaberl);
		GUI.Label(new Rect(10,100,100,80), white.ToString("d2"), labelStyleScoreWhite);
		switch (compMenu.getLockType()) {
		case menu.LOCK_TYPE.LOCK:
			GUI.Label(new Rect(80,80,100,80), compMenu.getMyName(), labelStyleScoreName);
			GUI.Label(new Rect(80,170,100,80), compMenu.getYourName(), labelStyleScoreName);
			break;
		case menu.LOCK_TYPE.LOCKED:
			GUI.Label(new Rect(80,80,100,80), compMenu.getYourName(), labelStyleScoreName);
			GUI.Label(new Rect(80,170,100,80), compMenu.getMyName(), labelStyleScoreName);
			break;
		}
		
		if (TimeLimit > 0f) {
			float restTime = TimeLimit - (Time.time - startTime);
			if (restTime < 0f) restTime = 0f;
			GUI.Label(new Rect(10,235,150,80), StringTable.TIMER, labelStyleLaberl);
			GUI.Label(new Rect(10,220,150,80), restTime.ToString("f02"), labelStyleTimer);
			if (restTime < 3f) {
				labelStyleTimer.normal.textColor = new Color32(64,64,64,255);
			} else {
				labelStyleTimer.normal.textColor = new Color32(193,193,193,255);
			}
		}
		
		Rect rect_gameover = new Rect(10, 320, 600, 100);
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
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		case GAME_STATUS.TimeOver:
			result = "";
			if (pieceSide == Piece.TYPE.Black) {
				result = StringTable.WIN_WHITE;
			} else if (pieceSide == Piece.TYPE.White){
				result = StringTable.WIN_BLACK;
			} else {
				result = StringTable.DRAW;
			}
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		case GAME_STATUS.WinByDefault:
			result = StringTable.ESCAPE;
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		}

	}
	
	public Piece.TYPE getPieceSide ()
	{
		return pieceSide;
	}

	public bool IsMySide ()
	{
		menu.LOCK_TYPE t = compMenu.getLockType();
		if (t == menu.LOCK_TYPE.LOCK) {
			return (getPieceSide() == Piece.TYPE.Black);
		} else if (t == menu.LOCK_TYPE.LOCKED) {
			return (getPieceSide() == Piece.TYPE.White);
		}

		return true;
	}
	
	public bool IsAI ()
	{
		int side = -1;
		switch(getPieceSide()) {
		case Piece.TYPE.Black:
			side = 0;
			break;
		case Piece.TYPE.White:
			side = 1;
			break;
		}
		if (side == -1) {
			return false;
		}
		
		return (compMenu.getKind(side) == 1);
	}
}
