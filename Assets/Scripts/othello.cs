namespace Othello
{
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
		
		// 駒を指定位場所に置けるか確認/
		public bool IsPutEnable(Piece.TYPE pieceType, int x, int y)
		{
			if (x < 0 || y < 0 || x >= SIZEX || y >= SIZEY)
			{
				return false;
			}
			if (board[x,y] != Piece.TYPE.Empty)
			{
				return false;
			}
			
			string place;
			Board.Instance().posToCode(x, y, out place);
			
			board[x,y] = pieceType;
			bool change =updateBoard(pieceType, x, y, false);
			board[x,y] = Piece.TYPE.Empty;
			return change;
		}
		
		// 駒を盤に置く/
		public bool putPiece(Piece.TYPE pieceType, int x, int y)
		{
			if (x < 0 || y < 0 || x >= SIZEX || y >= SIZEY)
			{
				return false;
			}
			if (board[x,y] != Piece.TYPE.Empty)
			{
				return false;
			}
			
			string place;
			Board.Instance().posToCode(x, y, out place);
			
			board[x,y] = pieceType;
			if(!updateBoard(pieceType, x, y, true))
			{
				board[x,y] = Piece.TYPE.Empty;
				return false;
			}
			
			calcPiecies();
			return true;
		
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

		// ポジションをガイドコードに変換/
		public bool posToCode(int x, int y, out string code) {
			if (x < 0 || y < 0 || x >= SIZEX || y >= SIZEY)
			{
				code = "";
				return false;
			}
			
			code = ((char)('a'+x)).ToString() + ((char)('0'+(8-y))).ToString();
			return true;
		}
	
		// ガイドコードをポジションに変換/
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

namespace Othello2
{

class Board
{

/* 盤面の大きさ */
public const int BOARD_SIZE = 8;

/* マスの状態 */
public const int WALL  = -1;
public const int EMPTY = 0;
public const int BLACK = 1;
public const int WHITE = 2;

/* マスの位置または手の種類 */
public const int PASS   = -1;
public const int NOMOVE = -2;

public const int A1 = 10;
public const int B1 = 11;
public const int C1 = 12;
public const int D1 = 13;
public const int E1 = 14;
public const int F1 = 15;
public const int G1 = 16;
public const int H1 = 17;

public const int A2 = 19;
public const int B2 = 20;
public const int C2 = 21;
public const int D2 = 22;
public const int E2 = 23;
public const int F2 = 24;
public const int G2 = 25;
public const int H2 = 26;

public const int A3 = 28;
public const int B3 = 29;
public const int C3 = 30;
public const int D3 = 31;
public const int E3 = 32;
public const int F3 = 33;
public const int G3 = 34;
public const int H3 = 35;

public const int A4 = 37;
public const int B4 = 38;
public const int C4 = 39;
public const int D4 = 40;
public const int E4 = 41;
public const int F4 = 42;
public const int G4 = 43;
public const int H4 = 44;

public const int A5 = 46;
public const int B5 = 47;
public const int C5 = 48;
public const int D5 = 49;
public const int E5 = 50;
public const int F5 = 51;
public const int G5 = 52;
public const int H5 = 53;

public const int A6 = 55;
public const int B6 = 56;
public const int C6 = 57;
public const int D6 = 58;
public const int E6 = 59;
public const int F6 = 60;
public const int G6 = 61;
public const int H6 = 62;

public const int A7 = 64;
public const int B7 = 65;
public const int C7 = 66;
public const int D7 = 67;
public const int E7 = 68;
public const int F7 = 69;
public const int G7 = 70;
public const int H7 = 71;

public const int A8 = 73;
public const int B8 = 74;
public const int C8 = 75;
public const int D8 = 76;
public const int E8 = 77;
public const int F8 = 78;
public const int G8 = 79;
public const int H8 = 80;

const int NUM_DISK	= (BOARD_SIZE+1)*(BOARD_SIZE+2)+1;
const int NUM_STACK	= ((BOARD_SIZE-2)*3+3)*BOARD_SIZE*BOARD_SIZE;

const int DIR_UP_LEFT		= -BOARD_SIZE-2;
const int DIR_UP			= -BOARD_SIZE-1;
const int DIR_UP_RIGHT		= -BOARD_SIZE;
const int DIR_LEFT			= -1;
const int DIR_RIGHT			= 1;
const int DIR_DOWN_LEFT		= BOARD_SIZE;
const int DIR_DOWN			= BOARD_SIZE+1;
const int DIR_DOWN_RIGHT	= BOARD_SIZE+2;
	
	public int[] Disk = new int[NUM_DISK];
	int[] Stack = new int[NUM_STACK];
	int Sp;

	public Board()
	{
		Clear();
	}
	
	public void Clear()
	{
		int i, j;
	
		for (i = 0; i < NUM_DISK; i++) {
			Disk[i] = WALL;
		}
		for (i = 0; i < BOARD_SIZE; i++) {
			for (j = 0; j < BOARD_SIZE; j++) {
				Disk[Pos(i, j)] = EMPTY;
			}
		}
		Disk[E4] = BLACK;
		Disk[D5] = BLACK;
		Disk[D4] = WHITE;
		Disk[E5] = WHITE;
	
		Sp = 0;
	}

	public int GetDisk(int in_pos)
	{
		return Disk[in_pos];
	}
	
	public int CountDisks(int in_color)
	{
		int result = 0;
		int pos;
	
		for (pos = 0; pos < NUM_DISK; pos++) {
			if (Disk[pos] == in_color) {
				result++;
			}
		}
	
		return result;
	}

	int FlipLine(int in_color, int in_pos, int in_dir)
	{
		int result = 0;
		int op = OpponentColor(in_color);
		int pos;
	
		for (pos = in_pos + in_dir; Disk[pos] == op; pos += in_dir) {}
		if (Disk[pos] == in_color) {
			for (pos -= in_dir; Disk[pos] == op; pos -= in_dir) {
				result++;
				Disk[pos] = in_color;
				Push(pos);
			}
		}
	
		return result;
	}

	public int Flip(int in_color, int in_pos)
	{
		int result = 0;
	
		if (Disk[in_pos] != EMPTY) {
			return 0;
		}
		result += FlipLine(in_color, in_pos, DIR_UP_LEFT);
		result += FlipLine(in_color, in_pos, DIR_UP);
		result += FlipLine(in_color, in_pos, DIR_UP_RIGHT);
		result += FlipLine(in_color, in_pos, DIR_LEFT);
		result += FlipLine(in_color, in_pos, DIR_RIGHT);
		result += FlipLine(in_color, in_pos, DIR_DOWN_LEFT);
		result += FlipLine(in_color, in_pos, DIR_DOWN);
		result += FlipLine(in_color, in_pos, DIR_DOWN_RIGHT);
		if (result > 0) {
			Disk[in_pos] = in_color;
			Push(in_pos);
			Push(OpponentColor(in_color));
			Push(result);
		}
	
		return result;
	}

	public int Unflip()
	{
		int result;
		int i, color;
	
		if (Sp <= 0) {
			return 0;
		}
		result = Pop();
		color = Pop();
		Disk[Pop()] = EMPTY;
		for (i = 0; i < result; i++) {
			Disk[Pop()] = color;
		}
	
		return result;
	}

	int CountFlipsLine(int in_color, int in_pos, int in_dir)
	{
		int result = 0;
		int op = OpponentColor(in_color);
		int pos;
	
		for (pos = in_pos + in_dir; Disk[pos] == op; pos += in_dir) {
			result++;
		}
		if (Disk[pos] != in_color) {
			return 0;
		}
	
		return result;
	}

	public int CountFlips(int in_color, int in_pos)
	{
		int result = 0;
	
		if (Disk[in_pos] != EMPTY) {
			return 0;
		}
		result += CountFlipsLine(in_color, in_pos, DIR_UP_LEFT);
		result += CountFlipsLine(in_color, in_pos, DIR_UP);
		result += CountFlipsLine(in_color, in_pos, DIR_UP_RIGHT);
		result += CountFlipsLine(in_color, in_pos, DIR_LEFT);
		result += CountFlipsLine(in_color, in_pos, DIR_RIGHT);
		result += CountFlipsLine(in_color, in_pos, DIR_DOWN_LEFT);
		result += CountFlipsLine(in_color, in_pos, DIR_DOWN);
		result += CountFlipsLine(in_color, in_pos, DIR_DOWN_RIGHT);
	
		return result;
	}

	public bool CanFlip(int in_color, int in_pos)
	{
		if (Disk[in_pos] != EMPTY) {
			return false;
		}
		if (CountFlipsLine(in_color, in_pos, DIR_UP_LEFT)>0) {
			return true;
		}
		if (CountFlipsLine(in_color, in_pos, DIR_UP)>0) {
			return true;
		}
		if (CountFlipsLine(in_color, in_pos, DIR_UP_RIGHT)>0) {
			return true;
		}
		if (CountFlipsLine(in_color, in_pos, DIR_LEFT)>0) {
			return true;
		}
		if (CountFlipsLine(in_color, in_pos, DIR_RIGHT)>0) {
			return true;
		}
		if (CountFlipsLine(in_color, in_pos, DIR_DOWN_LEFT)>0) {
			return true;
		}
		if (CountFlipsLine(in_color, in_pos, DIR_DOWN)>0) {
			return true;
		}
		if (CountFlipsLine(in_color, in_pos, DIR_DOWN_RIGHT)>0) {
			return true;
		}
	
		return false;
	}

	public void Copy(Board out_board)
	{
		out_board.Disk.CopyTo(Disk, 0);
		out_board.Stack.CopyTo(Stack, 0);
		Sp = out_board.Sp;
	}
	
	public void Reverse()
	{
		int pos;
		int p;
		int n;
	
		for (pos = 0; pos < NUM_DISK; pos++) {
			if (Disk[pos] == BLACK) {
				Disk[pos] = WHITE;
			} else if (Disk[pos] == WHITE) {
				Disk[pos] = BLACK;
			}
		}
		for (p = Sp; p > 0;) {
			p--;
			n = Stack[p];
			p--;
			Stack[p] = OpponentColor(Stack[p]);
			p -= n + 1;
		}
	}

	public bool CanPlay(int in_color)
	{
		int x, y;
	
		for (x = 0; x < BOARD_SIZE; x++) {
			for (y = 0; y < BOARD_SIZE; y++) {
				if (CanFlip(in_color, Pos(x, y))) {
					return true;
				}
			}
		}
		return false;
	}
	
	public int Pos(int in_x, int in_y)
	{
		return (in_y + 1) * (BOARD_SIZE + 1) + in_x + 1;
	}
	
	public int X(int in_pos)
	{
		return in_pos % (BOARD_SIZE + 1) - 1;
	}
	
	public int Y(int in_pos)
	{
		return in_pos / (BOARD_SIZE + 1) - 1;
	}
	
	public int OpponentColor(int in_color)
	{
		return BLACK + WHITE - in_color;
	}

	public void Push(int in_n)
	{
		Stack[Sp++] = in_n;
	}

	public int Pop()
	{
		return Stack[--Sp];
	}

};

}
	
	
