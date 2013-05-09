﻿namespace Othello
{
	//using UnityEngine;
	using System.Collections;
	
	public class Piece
	{
		// 升の状態を列挙/
		public enum TYPE {
			Empty = 0,			// 空/
			Black,				// 黒配置/
			White				// 白配置/
		}
	}

	public class Board
	{
		// シングルトン
		private static Board _instance = null;
		public static Board Instance()
		{
			if(_instance == null)
			{
				_instance = new Board();
			}
			return _instance;
		}
		private Board(){}
		
		
		public class Position {
			public int x;
			public int y;
			
			public Position(int ix, int iy)
			{
				x = ix;
				y = iy;
			}
		}
		
		// 盤面サイズ定義/
		public const int SIZEX = 8;
		public const int SIZEY = 8;
		
		// 盤面定義/
		Piece.TYPE[,] board = new Piece.TYPE[SIZEX,SIZEY];
		
		// 白駒数/
		int white = 0;
		// 黒駒数/
		int black = 0;
		
		// 盤面初期化/
		public void Initialize()
		{
			// 駒数初期化
			white = 0;
			black = 0;
			
			// 盤面クリア/
			for (int i=0; i<SIZEX; i++) {
				for (int j=0; j<SIZEY; j++) {
					board[i,j] = Piece.TYPE.Empty;
				}
			}

			// 駒の初期配置/
			board[3,4] = Piece.TYPE.White;
			board[3,3] = Piece.TYPE.Black;
			board[4,4] = Piece.TYPE.Black;
			board[4,3] = Piece.TYPE.White;
			calcPiecies();
		}
		
		// 駒を盤に置く/
		public void putPiece(Piece.TYPE pieceType, int x, int y)
		{
			if (x < 0 || y < 0 || x >= SIZEX || y >= SIZEY)
			{
				return;
			}
			if (board[x,y] != Piece.TYPE.Empty)
			{
				return;
			}
			//if (gamestatus != GAME_STATUS.LocalPlay && gamestatus != GAME_STATUS.NetworkPlay) {
			//	return;
			//}
			
			string place;
			Board.Instance().posToCode(x, y, out place);
			//Debug.Log("put:" + place");
			
			board[x,y] = pieceType;
			bool changeFlag = updateBoard(pieceType, x, y, true);
		
			if (changeFlag) {
				calcPiecies();
/*
				var rotation = Quaternion.identity;
				var position = new Vector3(key.x -4.0f + 0.5f, 0.25f, key.y -4.0f + 0.5f);
				//if (pieceType == 1) {
				//	rotation = Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
				//} else {
				//	rotation = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
				//}
				pieceList[x, y] = (GameObject)Instantiate(piecePrefab, position, rotation);
				for (int i=0; i<board.GetLength(0); i++) {
					for (int j=0; j<board.GetLength(1); j++) {
						if (board[i,j] == Piece.TYPE.Black) {
							pieceList[i, j].renderer.material.color = new Color(0,0,0,255);
						} else if (board[i,j] == Piece.TYPE.White) {
							pieceList[i, j].renderer.material.color = new Color(255,255,255,255);
						}
					}
				}
				Piece.TYPE pieceSide = (pieceType == Piece.TYPE.Black)? Piece.TYPE.White : Piece.TYPE.Black;
				// 置くところが無いかチェック/
				foreach(Object obj in markerList) {
					Object.Destroy(obj);
				}
				ArrayList enablePutList = new ArrayList();
				if (!checkEnablePut(pieceSide, ref enablePutList) && !initialFlag) {
					pieceSide = (pieceSide == Piece.TYPE.Black)? Piece.TYPE.White : Piece.TYPE.Black;
					// 攻守交代2回してどこも置けなかったらそのゲームは終了/
					if (!checkEnablePut(pieceSide, ref enablePutList) && !initialFlag) {
						StartCoroutine("GameOver", new GameEnd("game over", GAME_STATUS.GameOver, 2.5f));
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
*/
			} else {
				//Debug.Log("cannot put here");
				board[x,y] = Piece.TYPE.Empty;
			}
	
			// for debug
			/*
			string _s = "";
			for (int i=0; i<SIZEX; i++) {
				_s = _s + '\n';
				for (int j=0; j<SIZEY; j++) {
					if(board[i,j] == Piece.TYPE.Black) {
						_s = _s + 'X';
					} else if(board[i,j] == Piece.TYPE.White) {
						_s = _s + 'O';
					} else {
						_s = _s + '&';
					}
				}
			}
			Debug.Log(_s);
			*/
		}
		
		// 盤面の更新、updateFlag が false なら/
		// その場所に置けるかどうかのチェックだけ/
		bool updateBoard(Piece.TYPE pieceType, int x, int y, bool updateFlag)
		{
			int ix = 0; int iy = 0;
			
			ArrayList[] revList = new ArrayList[8];
			var changeFlag = false;
			// horizon
			ix = x + 1; iy = y;
			revList[0] = new ArrayList();
			while (true) {
				if (ix >= board.GetLength(0)) {
					revList[0].Clear();
					break;
				}
				if (board[ix,iy] != Piece.TYPE.Empty && board[ix,iy] != pieceType) {
					revList[0].Add(new Position(ix, iy));
				} else if (revList[0].Count > 0 && board[ix,iy] != Piece.TYPE.Empty) {
					changeFlag = true;
					break;
				} else {
					revList[0].Clear();
					break;
				}
				ix += 1;
			}
			
			ix = x - 1; iy = y;
			revList[1] = new ArrayList();
			while (true) {
				if (ix < 0) {
					revList[1].Clear();
					break;
				}
				if (board[ix,iy] != Piece.TYPE.Empty && board[ix,iy] != pieceType) {
					revList[1].Add(new Position(ix, iy));
				} else if (revList[1].Count > 0 && board[ix,iy] != Piece.TYPE.Empty) {
					changeFlag = true;
					break;
				} else {
					revList[1].Clear();
					break;
				}
				ix -= 1;
			}
		
			// vertical
			ix = x; iy = y + 1;
			revList[2] = new ArrayList();
			while (true) {
				if (iy >= board.GetLength(1)) {
					revList[2].Clear();
					break;
				}
				if (board[ix,iy] != Piece.TYPE.Empty && board[ix,iy] != pieceType) {
					revList[2].Add(new Position(ix, iy));
				} else if (revList[2].Count > 0 && board[ix,iy] != Piece.TYPE.Empty) {
					changeFlag = true;
					break;
				} else {
					revList[2].Clear();
					break;
				}
				iy += 1;
			}
		
			ix = x; iy = y - 1; 
			revList[3] = new ArrayList();
			while (true) {
				if (iy < 0) {
					revList[3].Clear();
					break;
				}
				if (board[ix,iy] != Piece.TYPE.Empty && board[ix,iy] != pieceType) {
					revList[3].Add(new Position(ix, iy));
				} else if (revList[3].Count > 0 && board[ix,iy] != Piece.TYPE.Empty) {
					changeFlag = true;
					break;
				} else {
					revList[3].Clear();
					break;
				}
				iy -= 1;
			}
		
			// cross
			ix = x + 1; iy = y + 1;
			revList[4] = new ArrayList();
			while (true) {
				if (ix >= board.GetLength(0) || iy >= board.GetLength(1)) {
					revList[4].Clear();
					break;
				}
				if (board[ix,iy] != Piece.TYPE.Empty && board[ix,iy] != pieceType) {
					revList[4].Add(new Position(ix, iy));
				} else if (revList[4].Count > 0 && board[ix,iy] != Piece.TYPE.Empty) {
					changeFlag = true;
					break;
				} else {
					revList[4].Clear();
					break;
				}
				iy += 1; ix += 1;
			}
		
			revList[5] = new ArrayList();
			ix = x + 1; iy = y - 1;
			while (true) {
				if (ix >= board.GetLength(0) || iy < 0 ) {
					revList[5].Clear();
					break;
				}
				if (board[ix,iy] != Piece.TYPE.Empty && board[ix,iy] != pieceType) {
					revList[5].Add(new Position(ix, iy));
				} else if (revList[5].Count > 0 && board[ix,iy] != Piece.TYPE.Empty) {
					changeFlag = true;
					break;
				} else {
					revList[5].Clear();
					break;
				}
				ix += 1; iy -= 1;
			}
			
		
			revList[6] = new ArrayList();
			ix = x - 1; iy = y + 1;
			while (true) {
				if (ix < 0 || iy >= board.GetLength(1)) {
					revList[6].Clear();
					break;
				}
				if (board[ix,iy] != Piece.TYPE.Empty && board[ix,iy] != pieceType) {
					revList[6].Add(new Position(ix, iy));
				} else if (revList[6].Count > 0 && board[ix,iy] != Piece.TYPE.Empty) {
					changeFlag = true;
					break;
				} else {
					revList[6].Clear();
					break;
				}
				ix -= 1; iy += 1;
			}
		
			revList[7] = new ArrayList();
			ix = x - 1; iy = y - 1;
			while (true) {
				if (ix < 0 || iy < 0) {
					revList[7].Clear();
					break;
				}
				if (board[ix,iy] != Piece.TYPE.Empty && board[ix,iy] != pieceType) {
					revList[7].Add(new Position(ix, iy));
				} else if (revList[7].Count > 0 && board[ix,iy] != Piece.TYPE.Empty) {
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
						foreach (Position v in val) {
							board[v.x, v.y] = (board[v.x, v.y] == Piece.TYPE.Black) ? Piece.TYPE.White : Piece.TYPE.Black;
						} 
					}
				}
				return true;
			} else {
				return false;
			}
		}
		
		// 駒数カウント/
		void calcPiecies() {
			int _white = 0;
			int _black = 0;
			for (int x=0; x<SIZEX; x++) {
				for (int y=0; y<SIZEY; y++) {
					if (board[x,y] == Piece.TYPE.Black) {
						_black += 1;
					} else if (board[x,y] == Piece.TYPE.White) {
						_white += 1;
					}
				}
			}
			white = _white;
			black = _black;
		}
		public Piece.TYPE GetPiece(int x, int y)
		{
			return board[x,y];
		}
		public int GetBlackPiecies()
		{
			return black;
		}
		public int GetWhitePiecies()
		{
			return white;
		}
		
		// 駒を置ける場所を検索/
		public bool CheckPutable(Piece.TYPE pieceType, ref ArrayList list)
		{
			for (int x=0; x<SIZEX; x++) {
				for (int y=0; y<SIZEY; y++) {
					if (board[x,y] == Piece.TYPE.Empty && updateBoard(pieceType, x, y, false)) {
						list.Add(new Board.Position(x,y));
					}
				}
			}
			return (list.Count > 0);
		}

		public bool posToCode(int x, int y, out string code) {
			if (x < 0 || y < 0 || x >= SIZEX || y >= SIZEY)
			{
				code = "";
				return false;
			}
			
			code = ((char)('a'+x)).ToString() + ((char)('0'+(8-y))).ToString();
			return true;
		}
	
		public  bool codeToPos(string code, out int x, out int y) {
			if (code[0] < 'a' || code[1] < '0' || code[0] > 'h' || code[1] > '8')
			{
				x = 0;
				y = 0;
				return false;
			}

			x =(int)code[0]-'a';
			y = 8-((int)code[1]-'0');
			return true;
		}
	}
}
