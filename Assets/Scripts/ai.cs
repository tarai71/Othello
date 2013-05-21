using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Othello;

public class ai : MonoBehaviour {

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
	int waitTime = 0;
	Stopwatch sw = new Stopwatch(); 
	
	Othello2.Board board;
	Othello2.Evaluator evaluator;
	Othello2.Opening opening;
	Othello2.Com com;

	// Use this for initialization
	void  Start () {
//		MPC_NUM = 14;
		string path;
#if UNITY_ANDROID
		path = "file://" + Application.dataPath + "!/assets";
#elif UNITY_IPHONE
		path = Application.dataPath + "/Raw";
#else
		path = Application.dataPath + "/StreamingAssets";
#endif
	    MPC_FILE = path + "/data/mpc.dat";
//		MPC_LEARN_FILE = path + "/data/mpc_learn.dat";
	    EVALUATOR_FILE = path + "/data/eval.dat";
//	    OPENING_TRANSCRIPT_FILE = path + "/data/open_trans.txt";
	    OPENING_FILE = path + "/data/open.dat";
//	    TRANSCRIPT_SIZE = 128;
		
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		compMain = GetComponent<main>();

		board = new Othello2.Board();
		evaluator = new Othello2.Evaluator();
        evaluator.Load(EVALUATOR_FILE);
		opening = new Othello2.Opening();
        opening.Load(OPENING_FILE);
		com = new Othello2.Com(ref evaluator, ref opening);	
//		com.SetLevel(14, 18, 20);
		com.SetLevel(18, 24, 26);
        com.SetOpening(true);
        com.LoadMPCInfo(MPC_FILE);
	}
	
	// Update is called once per frame
	void Update () {
		if (compMain.IsMySide()) {
			if (compMain.IsAI()) {
				if (!IsThinking) {
					IsThinking = true;
					waitTime = Random.Range(2000, 4000);
				    Thread thread = new Thread(new ThreadStart(defaultAI));
    			    thread.Start();
				} 
			}
		}
	}
	
	void CompleteAI()
	{
		IsThinking = false;
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

	
		if (board.CanPlay(color)) {
			sw.Reset();
			sw.Start();
			move = com.NextMove(board, color, out score);
            sw.Stop(); 
			UnityEngine.Debug.Log(("ABCDEFGH"[Othello2.Board.X(move)]).ToString() + ("12345678"[Othello2.Board.Y(move)]).ToString() + ":" + move.ToString() + "に置きます\n");
			UnityEngine.Debug.Log("評価値: " + score.ToString() + "\n");
			UnityEngine.Debug.Log("思考時間: " + sw.ElapsedMilliseconds / 1000f + "秒 ノード数:" + com.CountNodes() + " NPS:" + com.CountNodes() * 1000f / sw.ElapsedMilliseconds + " knps");
			board.Flip(color, move);
			
			int x = Othello2.Board.X(move);
			int y = 7 - Othello2.Board.Y(move);
			string place = "";
			if(Board.Instance().posToCode(x, y, out place))
			{
				compMenu.oms.WritePutList(place);
				if (compMenu.getLockType() != menu.LOCK_TYPE.FREE) {
					compMenu.oms.PutPiece(compMenu.GetYourID(), place);
				}
			}
		} else {
			UnityEngine.Debug.Log("パスします\n");
		}

		// 思考時間が1秒未満の時は1~2秒になるまで考えてるふりをする
		if(sw.ElapsedMilliseconds / 2000 < 2f)
		{
			System.Threading.Thread.Sleep(waitTime - (int)sw.ElapsedMilliseconds);
		}

		CompleteAI();
	}
	
//	public GUIStyle debugStayle;
	void OnGUI () {
//		GUI.Label(new Rect(50,200,600,400), "MPC_FILE:" + MPC_FILE, debugStayle);
	}
}

