using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Othello2
{
 
    class Com
    {
        public const int MPC_DEPTH_MIN = 3;
        struct MPCInfo
        {
            public int Depth;
            public int Offset;
            public int Deviation;
        };

        const int MAX_VALUE = (Evaluator.DISK_VALUE * 200);
        const int HASH_SIZE	= 18;

        class MoveList
        {
            public MoveList Prev;
            public MoveList Next;
            public int Pos;
        };

        struct MoveInfo
        {
            public MoveList Move;
            public int Value;
        };

        Board board;
        Evaluator evaluator;
        Opening opening;
        Hash hash;
        bool UseOpening;
        int MidDepth;
        int WLDDepth;
        int ExactDepth;
        int Node;
        MoveList[] Moves = new MoveList[Board.BOARD_SIZE * Board.BOARD_SIZE];
        int MpcInfoNum;
        MPCInfo[] MpcInfo;
        Random rand = new Random();
/*
        static int Com_Initialize(Com *self, Evaluator *evaluator, Opening *opening);
        static int Com_OpeningSearch(Com *self, int in_color, int in_opponent, int *out_move);
        static int Com_MidSearch(Com *self, int in_depth, int in_alpha, int in_beta, int in_color, int in_opponent, int in_pass, int *out_move);
        static int Com_EndSearch(Com *self, int in_depth, int in_alpha, int in_beta, int in_color, int in_opponent, int in_pass, int *out_move);
        static void Com_MakeList(Com *self);
        static int Com_Sort(Com *self, int in_color, MoveInfo *out_info);
        static int Com_ReadMPCInfo(Com *self, FILE *fp);
        static void RemoveList(MoveList *self);
        static void RecoverList(MoveList *self);
        #define get_rand(in_max) ((int)((double)(in_max) * rand() / (RAND_MAX + 1.0)))
*/
        int get_rand(int in_max)
        {
            return rand.Next(in_max);
        }

        public Com(ref Evaluator _evaluator, ref Opening _opening)
        {
            Initialize(ref _evaluator, ref _opening);
        }

        bool Initialize(ref Evaluator _evaluator, ref Opening _opening)
        {
            board = new Board();
            if (board == null) {
                return false;
            }
            evaluator = _evaluator;
            if (evaluator == null) {
                return false;
            }
            opening = _opening;
            if (opening == null)
            {
                return false;
            }
            hash = new Hash(HASH_SIZE);
            if (hash == null) {
                return false;
            }
            for (int i = 0; i < Board.BOARD_SIZE * Board.BOARD_SIZE; i++)
            {
                Moves[i] = new MoveList();
            }

            hash.Clear();
            UseOpening = false;
            MidDepth = 1;
            WLDDepth = 1;
            ExactDepth = 1;
            Node = 0;
            MpcInfoNum = 0;
            MpcInfo = null;
            return true;
        }

        public void SetOpening(bool in_use)
        {
            UseOpening = in_use;
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
            MakeList();
            board.InitializePattern();
            hash.ClearInfo();
            value = OpeningSearch(in_color, Board.OpponentColor(in_color), out result);
            if (result != Board.NOMOVE) {
            } else if (left <= ExactDepth) {
                value = EndSearch(left, -Board.BOARD_SIZE * Board.BOARD_SIZE, Board.BOARD_SIZE * Board.BOARD_SIZE, in_color, Board.OpponentColor(in_color), false, out result);
                value *= Evaluator.DISK_VALUE;
            } else if (left <= WLDDepth) {
                value = EndSearch(left, -Board.BOARD_SIZE * Board.BOARD_SIZE, 1, in_color, Board.OpponentColor(in_color), false, out result);
                value *= Evaluator.DISK_VALUE;
            } else {
                if ((in_color == Board.WHITE && MidDepth % 2 == 0) ||
                    (in_color == Board.BLACK && MidDepth % 2 == 1))
                {
                    board.Reverse();
                    color = Board.OpponentColor(in_color);
                } else {
                    color = in_color;
                }
                value = MidSearch(MidDepth, -MAX_VALUE, MAX_VALUE, color, Board.OpponentColor(color), false, out result);
            }
            out_value = value;

			return result;
        }

        int OpeningSearch(int in_color, int in_opponent, out int out_move)
        {
            MoveList p;
            Opening.PositionInfo info;
            int value, max = -MAX_VALUE;
            int count = 0;

            out_move = Board.NOMOVE;
            if (!UseOpening || opening == null) {
                return max;
            }
            for (p = Moves[0].Next; p != null; p = p.Next) {
                if (board.Flip(in_color, p.Pos)>0) {
                    if (opening.Info(board, in_opponent, out info)) {
                        value = -info.Value;
                        if (value > max) {
                            out_move = p.Pos;
                            max = value;
                            count = 1;
                        } else if (value == max) {
                            count++;
                            if (get_rand(count) < 1) {
                                out_move = p.Pos;
                            }
                        }
                    }
                    board.Unflip();
                }
            }
            return max;
        }

        int MidSearch(int in_depth, int in_alpha, int in_beta, int in_color, int in_opponent, bool in_pass, out int out_move)
        {
			MoveList p;
            int value, max = in_alpha;
            bool can_move = false;
            int move;
            MoveInfo[] info = new MoveInfo[Board.BOARD_SIZE * Board.BOARD_SIZE / 2];
            int i, info_num;
            MPCInfo mpc_info;
            Hash.Value hash_value;
            Hash.Info hash_info;

            hash_value.High = 0;
            hash_value.Low = 0;
            hash_info.Depth = 0;
            hash_info.Lower = 0;
            hash_info.Upper = 0;
            hash_info.Move = 0;

//            for (i = 0; i < Board.BOARD_SIZE * Board.BOARD_SIZE / 2; i++)
//            {
//                info[i] = new MoveInfo();
//            }

            out_move = Board.NOMOVE;
            if (in_depth == 0)
            {
                Node++;
info = null;
                return evaluator.GetValue(board);
            }
            if (in_depth > 2) {
                board.HashValue(in_color, out hash_value);
                if (hash.Get(hash_value, out hash_info)) {
                    if (hash_info.Depth >= in_depth) {
                        if (hash_info.Upper <= in_alpha) {
                            out_move = hash_info.Move;
info = null;
                            return in_alpha;
                        } else if (hash_info.Lower >= in_beta) {
                            out_move = hash_info.Move;
info = null;
                            return in_beta;
                        } else if (hash_info.Lower >= hash_info.Upper) {
                            out_move = hash_info.Move;
info = null;
                            return hash_info.Lower;
                        }
                        if (hash_info.Upper < in_beta) {
                            in_beta = hash_info.Upper;
                        }
                        if (hash_info.Lower > in_alpha) {
                            in_alpha = hash_info.Lower;
                        }
                    } else {
                        hash_info.Depth = (byte)in_depth;
                        hash_info.Lower = -MAX_VALUE;
                        hash_info.Upper = MAX_VALUE;
                    }
                } else {
                    hash_info.Depth = (byte)in_depth;
                    hash_info.Lower = -MAX_VALUE;
                    hash_info.Upper = MAX_VALUE;
                }
            }
            if (in_depth >= MPC_DEPTH_MIN && in_depth < MPC_DEPTH_MIN + MpcInfoNum)
            {
                mpc_info = MpcInfo[in_depth - MPC_DEPTH_MIN];
                value = in_alpha - mpc_info.Deviation + mpc_info.Offset;
                if (MidSearch(mpc_info.Depth, value - 1, value, in_color, in_opponent, in_pass, out out_move) < value)
                {
info = null;
                    return in_alpha;
                }
                value = in_beta + mpc_info.Deviation + mpc_info.Offset;
                if (MidSearch(mpc_info.Depth, value, value + 1, in_color, in_opponent, in_pass, out out_move) > value)
                {
info = null;
                   return in_beta;
                }
            }
            out_move = Board.NOMOVE;
            if (in_depth > 2) {
                info_num = Sort(in_color, ref info);
                if (info_num > 0) {
                    out_move = info[0].Move.Pos;
                    can_move = true;
                }
                for (i = 0; i < info_num; i++) {
                    board.FlipPattern(in_color, info[i].Move.Pos);
                    RemoveList(ref info[i].Move);
                    value = -MidSearch(in_depth - 1, -in_beta, -max, in_opponent, in_color, false, out move);
                    board.UnflipPattern();
                    RecoverList(ref info[i].Move);
                    if (value > max) {
                        max = value;
                        out_move = info[i].Move.Pos;
                        if (max >= in_beta) {
                            hash_info.Lower = max;
                            hash_info.Move = (byte)out_move;
                            hash.Set(hash_value, hash_info);
info = null;
                           return in_beta;
                        }
                    }
                }
                if (out_move != Board.PASS) {
                    hash_info.Upper = max;
                    hash_info.Move = (byte)out_move;
                    hash.Set(hash_value, hash_info);
                }
            } else {
                for (p = Moves[0].Next; p != null; p = p.Next) {
                    if (board.FlipPattern(in_color, p.Pos)>0) {
                        RemoveList(ref p);
                        if (!can_move) {
                            out_move = p.Pos;
                            can_move = true;
                        }
                        value = -MidSearch(in_depth - 1, -in_beta, -max, in_opponent, in_color, false, out move);
                        board.UnflipPattern();
                        RecoverList(ref p);
                        if (value > max) {
                            max = value;
                            out_move = p.Pos;
                            if (max >= in_beta) {
info = null;
                                return in_beta;
                            }
                        }
                    }
                }
            }
            if (!can_move) {
                if (in_pass) {
                    out_move = Board.NOMOVE;
                    Node++;
                    max = Evaluator.DISK_VALUE * (board.CountDisks(in_color) - board.CountDisks(in_opponent));
                } else {
                    out_move = Board.PASS;
                    max = -MidSearch(in_depth - 1, -in_beta, -max, in_opponent, in_color, true, out move);
                }
            }
info = null;
            return max;
        }

        int EndSearch(int in_depth, int in_alpha, int in_beta, int in_color, int in_opponent, bool in_pass, out int out_move)
        {
            MoveList p;
            int value, max = in_alpha;
            bool can_move = false;
            int move;
            MoveInfo[] info = new MoveInfo[Board.BOARD_SIZE * Board.BOARD_SIZE / 2];
            int i, info_num;
            Hash.Value hash_value;
            Hash.Info hash_info;

//            for (i = 0; i < Board.BOARD_SIZE * Board.BOARD_SIZE / 2; i++)
//            {
//                info[i] = new MoveInfo();
//            }

            if (in_depth == 1)
            {
                Node++;
                p = Moves[0].Next;
                value = board.CountFlips(in_color, p.Pos);
                max = board.CountDisks(in_color) - board.CountDisks(in_opponent);
                if (value > 0) {
                    out_move = p.Pos;
info = null;
                    return max + value + value + 1;
                }
                value = board.CountFlips(in_opponent, Moves[0].Next.Pos);
                if (value > 0) {
                    out_move = Board.PASS;
info = null;
                   return max - value - value - 1;
                }
                out_move = Board.NOMOVE;
info = null;
                return max;
            }
            out_move = Board.NOMOVE;
            if (in_depth > 8) {
                board.HashValue(in_color, out hash_value);
                if (hash.Get(hash_value, out hash_info)) {
                    if (hash_info.Depth >= in_depth) {
                        if (hash_info.Upper <= in_alpha) {
                            out_move = hash_info.Move;
info = null;
                           return in_alpha;
                        } else if (hash_info.Lower >= in_beta) {
                            out_move = hash_info.Move;
info = null;
                            return in_beta;
                        } else if (hash_info.Lower >= hash_info.Upper) {
                            out_move = hash_info.Move;
info = null;
                            return hash_info.Lower;
                        }
                        if (hash_info.Upper < in_beta) {
                            in_beta = hash_info.Upper;
                        }
                        if (hash_info.Lower > in_alpha) {
                            in_alpha = hash_info.Lower;
                        }
                    } else {
                        hash_info.Depth = (byte)in_depth;
                        hash_info.Lower = -MAX_VALUE;
                        hash_info.Upper = MAX_VALUE;
                    }
                } else {
                    hash_info.Depth = (byte)in_depth;
                    hash_info.Lower = -MAX_VALUE;
                    hash_info.Upper = MAX_VALUE;
                }
                info_num = Sort(in_color, ref info);
                if (info_num > 0) {
                    out_move = info[0].Move.Pos;
                    can_move = true;
                }
                for (i = 0; i < info_num; i++) {
                    board.FlipPattern(in_color, info[i].Move.Pos);
                    RemoveList(ref info[i].Move);
                    value = -EndSearch(in_depth - 1, -in_beta, -max, in_opponent, in_color, false, out move);
                    board.UnflipPattern();
                    RecoverList(ref info[i].Move);
                    if (value > max) {
                        max = value;
                        out_move = info[i].Move.Pos;
                        if (max >= in_beta) {
                            hash_info.Lower = max;
                            hash_info.Move = (byte)out_move;
                            hash.Set(hash_value, hash_info);
info = null;
                           return in_beta;
                        }
                    }
                }
                if (out_move != Board.PASS && out_move != Board.NOMOVE) {
                    hash_info.Upper = max;
                    hash_info.Move = (byte)out_move;
                    hash.Set(hash_value, hash_info);
                }
            } else {
                for (p = Moves[0].Next; p != null; p = p.Next) {
                    if (board.Flip(in_color, p.Pos)>0) {
                        RemoveList(ref p);
                        if (!can_move) {
                            out_move = p.Pos;
                            can_move = true;
                        }
                        value = -EndSearch(in_depth - 1, -in_beta, -max, in_opponent, in_color, false, out move);
                        board.Unflip();
                        RecoverList(ref p);
                        if (value > max) {
                            max = value;
                            out_move = p.Pos;
                            if (max >= in_beta) {
info = null;
                               return in_beta;
                            }
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
                    max = -EndSearch(in_depth, -in_beta, -max, in_opponent, in_color, true, out move);
                }
            }
info = null;
            return max;
        }

        public int CountNodes()
        {
            return Node;
        }

        int CountHashGet()
        {
            return hash.CountGet();
        }

        int CountHashHit()
        {
            return hash.CountHit();
        }

        void MakeList()
        {
            int[] pos_list = new int[] {
                Board.A1, Board.A8, Board.H8, Board.H1,
                Board.D3, Board.D6, Board.E3, Board.E6, Board.C4, Board.C5, Board.F4, Board.F5,
                Board.C3, Board.C6, Board.F3, Board.F6,
                Board.D2, Board.D7, Board.E2, Board.E7, Board.B4, Board.B5, Board.G4, Board.G5,
                Board.C2, Board.C7, Board.F2, Board.F7, Board.B3, Board.B6, Board.G3, Board.G6,
                Board.D1, Board.D8, Board.E1, Board.E8, Board.A4, Board.A5, Board.H4, Board.H5,
                Board.C1, Board.C8, Board.F1, Board.F8, Board.A3, Board.A6, Board.H3, Board.H6,
                Board.B2, Board.B7, Board.G2, Board.G7,
                Board.B1, Board.B8, Board.G1, Board.G8, Board.A2, Board.A7, Board.H2, Board.H7,
                Board.D4, Board.D5, Board.E4, Board.E5,
                Board.NOMOVE
            };
            int i,p;
            MoveList[] prev;

            p = 0;
            prev = Moves;
            prev[p].Pos = Board.NOMOVE;
            prev[p].Prev = null;
            prev[p].Next = null;
            for (i = 0; pos_list[i] != Board.NOMOVE; i++) {
                if (board.GetDisk(pos_list[i]) == Board.EMPTY) {
                    prev[p+1].Pos = pos_list[i];
                    prev[p+1].Prev = prev[p];
                    prev[p+1].Next = null;
                    prev[p].Next = prev[p+1];
                    p++;
                }
            }
        }

        static void RemoveList(ref MoveList self)
        {
            if (self.Prev != null)
            {
                self.Prev.Next = self.Next;
            }
            if (self.Next != null)
            {
                self.Next.Prev = self.Prev;
            }
        }

        static void RecoverList(ref MoveList self)
        {
            if (self.Prev != null)
            {
                self.Prev.Next = self;
            }
            if (self.Next != null)
            {
                self.Next.Prev = self;
            }
        }

        int Sort(int in_color, ref MoveInfo[] out_info)
        {
            int info_num = 0;
            MoveList p;
            MoveInfo info_tmp;
            int best_info;
            int i, j;

            for (p = Moves[0].Next; p != null; p = p.Next) {
                if (board.FlipPattern(in_color, p.Pos)>0) {
                    out_info[info_num].Move = p;
                    out_info[info_num].Value =evaluator.GetValue(board);
                    info_num++;
                    board.UnflipPattern();
                }
            }
            if (in_color == Board.WHITE) {
                for (i = 0; i < info_num; i++) {
                    out_info[i].Value = -out_info[i].Value;
                }
            }
            for (i = 0; i < info_num; i++) {
                best_info = i;
                for (j = i + 1; j < info_num; j++) {
                    if (out_info[j].Value > out_info[best_info].Value)
                    {
                        best_info = j;
                    }
                }
                info_tmp = out_info[best_info];
                out_info[best_info] = out_info[i];
                out_info[i] = info_tmp;
            }
            return info_num;
        }

        public bool LoadMPCInfo(Stream stream)
        {
		    using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    MpcInfoNum = reader.ReadInt32();
                    if (MpcInfoNum == 0)
                    {
                        MpcInfo = null;
                        return true;
                    }

                    Array.Resize(ref MpcInfo, MpcInfoNum);
                    for (int i = 0; i < MpcInfoNum; i++)
                    {
                        MpcInfo[i].Depth = reader.ReadInt32();
                        MpcInfo[i].Offset = reader.ReadInt32();
                        MpcInfo[i].Deviation = reader.ReadInt32();
                  }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
     }
}