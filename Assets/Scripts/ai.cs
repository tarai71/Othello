using UnityEngine;
using System.Collections;
using System.Threading;
using Othello;

public class ai : MonoBehaviour {

	const int MPC_NUM = 14;
    const string MPC_FILE = "Assets/Scripts/othello/data/mpc.dat";
    const string MPC_LEARN_FILE = "Assets/Scripts/othello/data/mpc_learn.dat";
    const string EVALUATOR_FILE = "Assets/Scripts/othello/data/eval.dat";
    const string OPENING_TRANSCRIPT_FILE = "Assets/Scripts/othello/ata/open_trans.txt";
    const string OPENING_FILE = "Assets/Scripts/othello/data/open.dat";
    const int TRANSCRIPT_SIZE = 128;
	
	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	// メインコンポーネントキャッシュ用/
	main compMain = null;
	
    Thread thread = null;
	bool IsThinking = false;
	bool IsCompleteThink = false;
	float clock_start = 0f, clock_end = 0f;		

	Othello2.Board board;
	Othello2.Evaluator evaluator;
	Othello2.Opening opening;
	Othello2.Com com;

	// Use this for initialization
	void Start () {
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		compMain = GetComponent<main>();

		board = new Othello2.Board();
		evaluator = new Othello2.Evaluator();
        evaluator.Load(EVALUATOR_FILE);
		opening = new Othello2.Opening();
        opening.Load(OPENING_FILE);
		com = new Othello2.Com(ref evaluator, ref opening);	
		com.SetLevel(14, 18, 20);
        com.SetOpening(true);
        com.LoadMPCInfo(MPC_FILE);
	}
	
	// Update is called once per frame
	void Update () {
		if (compMain.IsMySide()) {
			if (compMain.IsAI()) {
				if (!IsThinking) {
					IsThinking = true;
					clock_start = Time.time;
			        thread = new Thread(new ThreadStart(defaultAI));
    			    thread.Start();
				} 
			}
		}
				if(IsCompleteThink) {
					IsCompleteThink = false;
					clock_end = Time.time;
					//Debug.Log("Think time: " + (double)(clock_end - clock_start) + " sec node: " + com.CountNodes().ToString() + " NPS: " + (double)com.CountNodes() / (clock_end - clock_start + 1) / 1000 + " knps\n");
					Debug.Log("Think time: " + (double)(clock_end - clock_start) + " sec\n");
				}
	}
	
	void defaultAI () {
		Debug.Log("AI Thinking...\n");
		
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
			move = com.NextMove(board, color, out score);
			Debug.Log(("ABCDEFGH"[Othello2.Board.X(move)]).ToString() + ("12345678"[Othello2.Board.Y(move)]).ToString() + ":" + move.ToString() + " puted\n");
			Debug.Log("Eval: " + score.ToString() + "\n");
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
			Debug.Log("Pass\n");
		}
		IsThinking = false;
		IsCompleteThink = true;
	}
	
	void OnGUI () {
	}
}

