using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Othello;

public class main : MonoBehaviour {
	
	// Prefab定義/
	public GameObject piecePrefab;
	
	// GUISkin/
	public GUISkin mainSkin;
	
	// GUIStyle定義/
	public GUIStyle labelStyleScoreBlack;
	public GUIStyle labelStyleLabelBlack;
	public GUIStyle labelStyleScoreNameBlack;
	public GUIStyle labelStyleScoreWhite;
	public GUIStyle labelStyleLabelWhite;
	public GUIStyle labelStyleScoreNameWhite;
	public GUIStyle labelStyleGameOver;
	public GUIStyle labelStyleLabelTimer;
	public GUIStyle labelStyleTimer;
	public GUIStyle labelStyleMoveBlack;
	public GUIStyle labelStyleMoveWhite;
	
	// ゲームの状態を列挙/
	public enum GAME_STATUS {
		None = 0,
		LocalPlay,			// ローカル対戦中/
		NetworkPlay,		// 通信対戦中/
		GameOver,			// ゲームオーバー表示中/
		TimeOver,			// タイムオーバー表示中/
		WinByDefault		// 不戦勝表示中/
	}
	
	// 駒のオブジェクトリスト/
	public PieceObject[,] pieceList = new PieceObject[8,8];
	// 棋譜の表示/
	Vector2 scrollPosition = Vector2.zero;
		
	// 現在の手順/
	Piece.TYPE pieceSide = Piece.TYPE.Black;
	// ゲームの状態/
	GAME_STATUS gamestatus = GAME_STATUS.None;
	// 制限時間/
	float TimeLimit;
	// 残り時間計算用ワーク/
	float startTime;
	float restTime;
	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	board compBoard = null;
	ai  compAI = null;
	controller compController = null;
		
	// Use this for initialization
	void Start () {
		// メニューコンポーネントをキャッシュ/
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		compBoard = GetComponent<board>();
		compAI = GetComponent<ai>();
		compController = GetComponent<controller>();
		
		// 駒オブジェクト生成/
		for (int i=0; i<8; i++)
		{
			for (int j=0; j<8; j++)
			{
				Vector3 position = new Vector3(j -4.0f + 0.5f, 0.3f, i -4.0f + 0.5f);
				GameObject blackPiece = (GameObject)Instantiate(piecePrefab);
				GameObject whitePiece = (GameObject)Instantiate(piecePrefab);
				pieceList[j, i] = new PieceObject(ref blackPiece, ref whitePiece, position);
			}
		}
		
		// ゲーム状態初期化/
		gamestatus = (compMenu.getLockType() == menu.LOCK_TYPE.FREE)? GAME_STATUS.LocalPlay : GAME_STATUS.NetworkPlay;
		
		// 盤面初期化/
		Board.Instance().Initialize();
		
		// 駒オブジェクト初期表示/
		for (int i=0; i<8; i++)
		{
			for (int j=0; j<8; j++)
			{
				if (Board.Instance().GetPiece(i,j) == Piece.TYPE.Black) {
					pieceList[i, j].Enabled(true);
					pieceList[i, j].ToBlack(false);
				}
				else
				if (Board.Instance().GetPiece(i,j) == Piece.TYPE.White) {
					pieceList[i, j].Enabled(true);
					pieceList[i, j].ToWhite(false);
				}
			}
		}
			
		// メニューから制限時間取得/
		TimeLimit = compMenu.GetLimitTime();

		// ターンを初期化/
		pieceSide = Piece.TYPE.White;
		InitializeTurn();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(gamestatus == GAME_STATUS.LocalPlay || gamestatus == GAME_STATUS.NetworkPlay)
		{
			if (IsMySide()) {
				if (IsAI()) {
					compAI.UpdateAI();
				} else {
					compController.UpdateHuman();
				}
			}
		}
		
		// 駒オブジェクト表示制御/
		bool inDrop = false;
		for (int i=0; i<8; i++)
		{
			for (int j=0; j<8; j++)
			{
				if(pieceList[i, j].GetEnabled())
				{
					if(pieceList[i, j].GetDrop())
					{
						inDrop = true;
						pieceList[i, j].ToDrop();
					}
				}
			}
		}
		if(!inDrop)
		{
			for (int i=0; i<8; i++)
			{
				for (int j=0; j<8; j++)
				{
					if(pieceList[i, j].GetEnabled())
					{
						if (Board.Instance().GetPiece(i,j) == Piece.TYPE.Black) {
							pieceList[i, j].ToBlack();
						}
						else
						if (Board.Instance().GetPiece(i,j) == Piece.TYPE.White) {
							pieceList[i, j].ToWhite();
						}
					}
				}
			}
		}
		
		byte alfa;
		alfa = (Time.time % 1.0f < 0.5f)? (byte)((Time.time % 1.0f) * 255 + 127) : (byte)((1.0f - Time.time % 1.0f) * 255 + 127);
		
		if(pieceSide == Piece.TYPE.Black) {
			labelStyleScoreBlack.normal.textColor = new Color32(209,221, 48,alfa);
			labelStyleScoreWhite.normal.textColor = new Color32(192,192,192,255);
		} else if(pieceSide == Piece.TYPE.White) {
			labelStyleScoreBlack.normal.textColor = new Color32( 64, 64, 64,255);
			labelStyleScoreWhite.normal.textColor = new Color32(209,221, 48,alfa);
		}
		
		// 制限時間切れでゲームは終了/
		if (TimeLimit > 0f) {
			if (restTime <= 0f) {
				SetGameEnd(GAME_STATUS.TimeOver);
			}
		}
		// 対戦相手がいなくなったらメニューに戻る/
		if (gamestatus == GAME_STATUS.NetworkPlay && compMenu.getLockType() == menu.LOCK_TYPE.FREE) {
			//SetGameEnd(GAME_STATUS.WinByDefault);
			ReturnMenu();
		}
		// 対戦相手がゲームを終了したらメニューに戻る/
		if (compMenu.GetEndGame()) {
			ReturnMenu();
		}

		
		// 駒更新/
		string location;
		if (compMenu.oms.ReadPutList(out location))
		{
			Board.Position pos = new Board.Position(0,0);
			if(Board.Instance().codeToPos(location, out pos.x, out pos.y))
			{
				// 駒を置く/
				if(Board.Instance().putPiece(pieceSide, pos.x, pos.y))
				{
					// 駒オブジェクトを置く/
					pieceList[pos.x, pos.y].Enabled(true);
					pieceList[pos.x, pos.y].SetHight(5f);
					if(pieceSide == Piece.TYPE.Black)
						pieceList[pos.x, pos.y].ToBlack(false);
					else
						pieceList[pos.x, pos.y].ToWhite(false);
				}
				
				// ターンを初期化/
				InitializeTurn();
			}
		}
	}
	
	public void InitializeTurn()
	{
		// ターンを入れ替える/
		pieceSide = (pieceSide == Piece.TYPE.Black)? Piece.TYPE.White : Piece.TYPE.Black;

		// 計測開始時間をセット/
		startTime = Time.time;
	
		// マーカー更新/
		compBoard.MarkerUpdate();
		compBoard.MarkerEnabled(IsMySide());
	}
	
	void ReturnMenu()
	{
		if(compMenu.getLockType() == menu.LOCK_TYPE.LOCK)
		{
			compMenu.oms.EndGame(compMenu.GetMyID(), compMenu.GetYourID());
		}
		compMenu.enabled = true;
		Application.LoadLevel("Empty");
	}

	void OnGUI() {
		GUI.skin = mainSkin;
		
		int black = Board.Instance().GetBlackPiecies();
		int white = Board.Instance().GetWhitePiecies();
		
		GUI.Label(new Rect(10,10,80,80), StringTable.BLACK, labelStyleLabelBlack);
		GUI.Label(new Rect(10,10,80,80), black.ToString("d2"), labelStyleScoreBlack);
		GUI.Label(new Rect(Screen.width-100,10,80,80), StringTable.WHITE, labelStyleLabelWhite);
		GUI.Label(new Rect(Screen.width-100,10,80,80), white.ToString("d2"), labelStyleScoreWhite);
		switch (compMenu.getLockType()) {
		case menu.LOCK_TYPE.LOCK:
			GUI.Label(new Rect(10+40,20,80,80), compMenu.GetMyName(), labelStyleScoreNameBlack);
			GUI.Label(new Rect(Screen.width-100-40,20,80,80), compMenu.GetYourName(), labelStyleScoreNameWhite);
			break;
		case menu.LOCK_TYPE.LOCKED:
			GUI.Label(new Rect(10+40,20,80,80), compMenu.GetYourName(), labelStyleScoreNameBlack);
			GUI.Label(new Rect(Screen.width-100-40,20,80,80), compMenu.GetMyName(), labelStyleScoreNameWhite);
			break;
		}

		if (TimeLimit > 0f) {
			if(gamestatus == GAME_STATUS.LocalPlay || gamestatus == GAME_STATUS.NetworkPlay)
			{
				restTime = TimeLimit - (Time.time - startTime);
			}
			if (restTime < 0f) restTime = 0f;
			GUI.Label(new Rect((Screen.width-150)/2,40,150,70), StringTable.TIMER, labelStyleLabelTimer);
			GUI.Label(new Rect((Screen.width-150)/2,40,150,70), restTime.ToString("f02"), labelStyleTimer);
			if (restTime < 3f) {
				labelStyleTimer.normal.textColor = new Color32(250,64,64,255);
			} else {
				labelStyleTimer.normal.textColor = new Color32(250,250,250,255);
			}
		}

		Rect rect_gameover = new Rect((Screen.width-600)/2, Screen.height-120, 600, 100);
		switch (gamestatus) {
		case GAME_STATUS.GameOver:
			string result = "";
			if (white > black) {
				result = StringTable.WIN_WHITE;
				labelStyleGameOver.normal.textColor = new Color32(192,192,192,255);
			} else if (white < black) {
				result = StringTable.WIN_BLACK;
				labelStyleGameOver.normal.textColor = new Color32(64,64,64,255);
			} else {
				result = StringTable.DRAW;
				labelStyleGameOver.normal.textColor = new Color32(250,250,250,255);
			}
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		case GAME_STATUS.TimeOver:
			result = "";
			if (pieceSide == Piece.TYPE.Black) {
				result = StringTable.WIN_WHITE;
				labelStyleGameOver.normal.textColor = new Color32(192,192,192,255);
			} else if (pieceSide == Piece.TYPE.White){
				result = StringTable.WIN_BLACK;
				labelStyleGameOver.normal.textColor = new Color32(64,64,64,255);
			} else {
				result = StringTable.DRAW;
				labelStyleGameOver.normal.textColor = new Color32(250,250,250,255);
			}
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		case GAME_STATUS.WinByDefault:
			result = StringTable.ESCAPE;
			labelStyleGameOver.normal.textColor = new Color32(250,250,250,255);
			GUI.Label(rect_gameover, result, labelStyleGameOver);
			break;
		}
		
		if(compMenu.getLockType() != menu.LOCK_TYPE.LOCKED)
		{
			GUILayout.BeginArea(new Rect(10, Screen.height-60, Screen.width-20, 60));
			if(GUILayout.Button(StringTable.RETURNMENU)) {
				ReturnMenu();
			}
			GUILayout.EndArea();
		}
		
		// 棋譜の表示
		GUILayout.BeginArea(new Rect(10, 100, 100, Screen.height));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width (50), GUILayout.Height (Screen.height - 180));
		List<Board.MoveForm> moveList = Board.Instance().GetMoveList();
		for(int i=moveList.Count-1; i>=0; i--)
		{
			GUILayout.Label((i+1).ToString("D2") + ":" + moveList[i].location, (moveList[i].type == Piece.TYPE.Black)? labelStyleMoveBlack : labelStyleMoveWhite);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	public void SetGameEnd(GAME_STATUS status)
	{
		switch(status)
		{
		case GAME_STATUS.GameOver:
			Debug.Log("game over");
			gamestatus = GAME_STATUS.GameOver;
			break;
		case GAME_STATUS.TimeOver:
			Debug.Log("time over");
			gamestatus = GAME_STATUS.TimeOver;
			break;
		case GAME_STATUS.WinByDefault:
			Debug.Log("win by default");
			gamestatus = GAME_STATUS.WinByDefault;
			break;
		}
	}
	
	public Piece.TYPE GetPieceSide ()
	{
		return pieceSide;
	}
	public Piece.TYPE ChangePieceSide()
	{
		pieceSide = (pieceSide == Piece.TYPE.Black)? Piece.TYPE.White : Piece.TYPE.Black;
		return pieceSide;
	}

	public bool IsMySide ()
	{
		menu.LOCK_TYPE t = compMenu.getLockType();
		if (t == menu.LOCK_TYPE.LOCK) {
			return (GetPieceSide() == Piece.TYPE.Black);
		} else if (t == menu.LOCK_TYPE.LOCKED) {
			return (GetPieceSide() == Piece.TYPE.White);
		}

		return true;
	}
	
	public bool IsAI ()
	{
		int side = -1;
		switch(GetPieceSide()) {
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
		
		return (compMenu.GetKind(side) == 1);
	}
}
