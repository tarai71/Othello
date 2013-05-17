using Othello2;
	
class Com
{
	Board board;
	int MidDepth;
	int WLDDepth;
	int ExactDepth;
	int Node;

	
	public Com()
	{
		Initialize();
	}

	bool Initialize()
	{
		board = new Board();
		if (board == null) {
			return false;
		}
		MidDepth = 1;
		WLDDepth = 1;
		ExactDepth = 1;
		Node = 0;
		return true;
	}
	
	public void SetLevel(int in_mid, int in_exact, int in_wld)
	{
		MidDepth = in_mid;
		WLDDepth = in_wld;
		ExactDepth = in_exact;
	}

	public int NextMove(Board in_board, int in_color, out int out_value)
	{
		int result;
		int left;
		int value;
		int color;

		board.Copy(in_board);
		Node = 0;
		left = board.CountDisks(Board.EMPTY);
		if (left <= ExactDepth) {
			value = EndSearch(left, in_color, board.OpponentColor(in_color), false, out result);
		} else if (left <= WLDDepth) {
			value = EndSearch(left, in_color, board.OpponentColor(in_color), false, out result);
		} else {
			if ((in_color == Board.WHITE && MidDepth % 2 == 0) ||
				(in_color == Board.BLACK && MidDepth % 2 == 1)) {
				board.Reverse();
				color = board.OpponentColor(in_color);
			} else {
				color = in_color;
			}
			value = MidSearch(MidDepth, color, board.OpponentColor(color), false, out result);
		}
		out_value = value;
	
		return result;
	}

	int MidSearch(int in_depth, int in_color, int in_opponent, bool in_pass, out int out_move)
	{
		int x, y;
		int value, max = -Board.BOARD_SIZE * Board.BOARD_SIZE;
		bool can_move = false;
		int move;
	
		out_move = Board.NOMOVE;
		if (in_depth == 0) {
			Node++;
			return board.CountDisks(in_color) - board.CountDisks(in_opponent);
		}
		for (x = 0; x < Board.BOARD_SIZE; x++) {
			for (y = 0; y < Board.BOARD_SIZE; y++) {
				if (board.Flip(in_color, board.Pos(x, y))>0) {
					if (!can_move) {
						out_move = board.Pos(x, y);
						can_move = true;
					}
					value = -MidSearch(in_depth - 1, in_opponent, in_color, false, out move);
					board.Unflip();
					if (value > max) {
						max = value;
						out_move = board.Pos(x, y);
					}
				}
			}
		}
		if (!can_move) {
			if (in_pass) {
				out_move = Board.NOMOVE;
				Node++;
				max = board.CountDisks(in_color) - board.CountDisks(in_opponent);
			} else {
				out_move = Board.PASS;
				max = -MidSearch(in_depth - 1, in_opponent, in_color, true, out move);
			}
		}
		return max;
	}

	int EndSearch(int in_depth, int in_color, int in_opponent, bool in_pass, out int out_move)
	{
		int x, y;
		int value, max = -Board.BOARD_SIZE * Board.BOARD_SIZE;
		bool can_move = false;
		int move;
	
		out_move = Board.NOMOVE;
		if (in_depth == 0) {
			Node++;
			return board.CountDisks(in_color) - board.CountDisks(in_opponent);
		}
		for (x = 0; x < Board.BOARD_SIZE; x++) {
			for (y = 0; y < Board.BOARD_SIZE; y++) {
				if (board.Flip(in_color, board.Pos(x, y))>0) {
					if (!can_move) {
						out_move = board.Pos(x, y);
						can_move = true;
					}
					value = -EndSearch(in_depth - 1, in_opponent, in_color, false, out move);
					board.Unflip();
					if (value > max) {
						max = value;
						out_move = board.Pos(x, y);
					}
				}
			}
		}
		if (!can_move) {
			if (in_pass) {
				out_move = Board.NOMOVE;
				Node++;
				max = board.CountDisks(in_color) - board.CountDisks(in_opponent);
			} else {
				out_move = Board.PASS;
				max = -EndSearch(in_depth, in_opponent, in_color, true, out move);
			}
		}
		return max;
	}

	public int CountNodes()
	{
		return Node;
	}
};

