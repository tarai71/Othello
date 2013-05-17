using UnityEngine;
using System.Collections;
using System.Threading;
using Othello;

public class ai : MonoBehaviour {
	
	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	// メインコンポーネントキャッシュ用/
	main compMain = null;
	
    Thread thread = null;
	bool IsThinking = false;
	bool IsCompleteThink = false;
	float clock_start = 0f, clock_end = 0f;		
	
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
		
		Othello2.Board board = new Othello2.Board();	
		Com com = new Com();	
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
					board.Disk[board.Pos(j,i)] = 1;
					break;
				case Piece.TYPE.White:
					board.Disk[board.Pos(j,i)] = 2;
					break;
				}
			}
		}

		com.SetLevel(8, 12, 12);
	
		if (board.CanPlay(color)) {
			move = com.NextMove(board, color, out score);
			Debug.Log(("ABCDEFGH"[board.X(move)]).ToString() + ("12345678"[board.Y(move)]).ToString() + ":" + move.ToString() + " puted\n");
			Debug.Log("Eval: " + score.ToString() + "\n");
			board.Flip(color, move);
			
			int x = board.X(move);
			int y = 7 - board.Y(move);
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

