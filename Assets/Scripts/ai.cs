using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Othello;

public class ai : MonoBehaviour {
	
	public delegate bool loadcallback(Stream s);
	public struct LOAD {
		string filename;
		string assetname;
		loadcallback callback;
		
		public LOAD(string fn, string an, loadcallback cb)
		{
			filename = fn;
			assetname = an;
			callback = cb;
		}
		
		public string Filename
		{
			set{this.filename = value;}
			get{return this.filename;}
		}

		public string Assetname
		{
			set{this.assetname = value;}
			get{return this.assetname;}
		}

		public loadcallback Callback
		{
			set{this.callback = value;}
			get{return this.callback;}
		}
	}
	
//	int MPC_NUM = 14;
    string MPC_FILE ;
//    string MPC_LEARN_FILE;
    string EVALUATOR_FILE;
//    string OPENING_TRANSCRIPT_FILE;
    string OPENING_FILE;
//    int TRANSCRIPT_SIZE;
	
	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	// メインコンポーネントキャッシュ用/
	main compMain = null;
	
	bool IsThinking = false;
	bool IsComplete = false;
	int tx,ty;
	Stopwatch sw = new Stopwatch(); 

	Othello2.Board board;
	Othello2.Evaluator evaluator;
	Othello2.Opening opening;
	Othello2.Com com;

	public GUIStyle debugStyle;

	// Use this for initialization
	void   Start() {
//		MPC_NUM = 14;
	    MPC_FILE = "mpc";
//		MPC_LEARN_FILE = "mpc_learn";
	    EVALUATOR_FILE = "eval";
//	    OPENING_TRANSCRIPT_FILE = "open_trans.txt";
	    OPENING_FILE = "open";
//	    TRANSCRIPT_SIZE = 128;
	
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		compMain = GetComponent<main>();
		
		board = new Othello2.Board();
		evaluator = new Othello2.Evaluator();
		TextAsset assetEval = Resources.Load(EVALUATOR_FILE) as TextAsset;
		evaluator.Load(new MemoryStream(assetEval.bytes));
		opening = new Othello2.Opening();
		TextAsset assetOpen = Resources.Load(OPENING_FILE) as TextAsset;
		opening.Load(new MemoryStream(assetOpen.bytes));
		com = new Othello2.Com(ref evaluator, ref opening);	
#if UNITY_ANDROID
		com.SetLevel(10, 18, 20);
#elif UNITY_IPHONE
		com.SetLevel(10, 18, 20);
#else
		com.SetLevel(14, 18, 20);
		//com.SetLevel(16, 20, 22);
		//com.SetLevel(20, 24, 26);
#endif
        com.SetOpening(true);
		TextAsset assetMpc = Resources.Load(MPC_FILE) as TextAsset;
		com.LoadMPCInfo(new MemoryStream(assetMpc.bytes));
//		Object.DestroyImmediate(assetEval, true);
//		Object.DestroyImmediate(assetOpen, true);
//		Object.DestroyImmediate(assetMpc, true);
//		Resources.UnloadUnusedAssets();
	}
	
	// Update is called once per frame
	public void UpdateAI () {
		if (compMain.IsMySide()) {
			if (compMain.IsAI()) {
				if (!IsThinking && !IsComplete) {
					IsThinking = true;
				    Thread thread = new Thread(new ThreadStart(defaultAI));
    			    thread.Start();
				} 
			}
		}
		if(IsComplete)
		{
			IsComplete = false;
			string place = "";
			if(Board.Instance().posToCode(tx, ty, out place))
			{
				//compMenu.oms.WritePutList(place);
				// 駒を置く/
				if(Board.Instance().putPiece(compMain.GetPieceSide(), tx, ty))
				{
					// 駒オブジェクトを置く/
					compMain.pieceList[tx, ty].Enabled(true);
					compMain.pieceList[tx, ty].SetHight(5f);
					if(compMain.GetPieceSide() == Piece.TYPE.Black)
						compMain.pieceList[tx, ty].ToBlack(false);
					else
						compMain.pieceList[tx, ty].ToWhite(false);
				}
				
				// ターンを初期化/
				compMain.InitializeTurn();
				if (compMenu.getLockType() != menu.LOCK_TYPE.FREE) {
					compMenu.oms.PutPiece(compMenu.GetYourID(), place);
				}
			}
		}
	}
	
	void CompleteAI(int x, int y)
	{
		IsThinking = false;
		IsComplete = true;
		tx = x;
		ty = y;
	}
	
	void defaultAI () {
		UnityEngine.Debug.Log("コンピュータ思考中...\n");
		
		int color = (compMain.GetPieceSide() == Othello.Piece.TYPE.Black)? 
			Othello2.Board.BLACK : Othello2.Board.WHITE;
		int move;
		int score;
		
		board.Clear();
		for(int i=0; i<8; i++) {
			for( int j=0; j<8; j++) {
				switch(Board.Instance().GetPiece(j, 7-i))
				{
				case Piece.TYPE.Black:
					board.Disk[Othello2.Board.Pos(j,i)] = 1;
					break;
				case Piece.TYPE.White:
					board.Disk[Othello2.Board.Pos(j,i)] = 2;
					break;
				}
			}
		}

	
		int x = 0;
		int y = 0;
		if (board.CanPlay(color)) {
			sw.Reset();
			sw.Start();
			move = com.NextMove(board, color, out score);
            sw.Stop(); 
			UnityEngine.Debug.Log(("ABCDEFGH"[Othello2.Board.X(move)]).ToString() + ("12345678"[Othello2.Board.Y(move)]).ToString() + "に置きます\n");
			UnityEngine.Debug.Log("評価値: " + score.ToString() + "\n");
			UnityEngine.Debug.Log("思考時間: " + (double)(sw.ElapsedMilliseconds+1) / 1000 + "秒 ノード数:" + com.CountNodes() + " NPS:" + (double)com.CountNodes() * 1000 / (sw.ElapsedMilliseconds+1) + " knps");
			board.Flip(color, move);
			
			x = Othello2.Board.X(move);
			y = 7 - Othello2.Board.Y(move);
		} else {
			UnityEngine.Debug.Log("パスします\n");
		}

		// 思考時間が1秒未満の時は1~2秒になるまで考えてるふりをする
		if((sw.ElapsedMilliseconds+1) / 1000f < 1f)
		{
			System.Threading.Thread.Sleep(1000 + (int)(sw.ElapsedMilliseconds+1));
		}

		CompleteAI(x,y);
	}
	
//	public GUIStyle debugStayle;
	void OnGUI () {
		//GUI.Label(new Rect(50,200,600,400), EVALUATOR_FILE, debugStyle);
	}
}

