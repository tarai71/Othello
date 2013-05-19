using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Othello2
{
    class Opening
    {
 
        // 局面情報のブロックサイズ
        // PositionInfoのメモリ領域はNUM_INFO_BLOCK * sizeof(PositionInfo)
        // の整数倍確保するようにする

        const int NUM_INFO_BLOCK = 0x010000;

        // 盤面の状態をあらわす構造体
        // bl、bhに手番の石の状態を、wl、whに手番でない石の状態を格納する
        // 石が存在する場合には各ビットを1にする
        public struct PositionKey
        {
            public uint bl;
            public uint bh;
            public uint wl;
            public uint wh;

            public void Zero()
            {
                bl = 0;
                bh = 0;
                wl = 0;
                wh = 0;
            }
        };

        // 局面情報
        public class PositionInfo
        {
	        public int Value;		// 評価値
        };

        // 局面データ
        public struct PositionData
        {
            public PositionKey key;
            public PositionInfo info;
        };

        int Num;		        // 局面データの数
        int Max;		        // 保持可能な局面データの数
        PositionData[] Data;    // 局面データ
/*
        static int Opening_Initialize(Opening *self);
        static void Opening_Finalize(Opening *self);
        static int Opening_Read(Opening *self, FILE *fp);
        static int Opening_Write(const Opening *self, FILE *fp);
        static PositionInfo * Opening_Find(const Opening *self, const Board *in_board, int in_color);
        static int PositionKey_Comp(const PositionKey *in_key1, const PositionKey *in_key2);
        static void Board_Key(const Board *in_board, int in_color, PositionKey *out_key);
        static int Board_RotatePos(int in_x, int in_y, int in_type);
*/
        public Opening()
        {
            Initialize();
        }

        bool Initialize()
        {
            Num = 0;
            Max = NUM_INFO_BLOCK;
            Data = new PositionData[Max];
            if (Data == null) {
                return false;
            }
            return true;
        }

        public bool Load(string in_file_name)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(@in_file_name)))
            {
                try
                {
                    Num = reader.ReadInt32();
                    if (Num % NUM_INFO_BLOCK == 0)
                    {
                        Max = (Num / NUM_INFO_BLOCK + 1) * NUM_INFO_BLOCK;
                    }
                    else
                    {
                        Max = (Num / NUM_INFO_BLOCK + 2) * NUM_INFO_BLOCK;
                    }
                    Array.Resize(ref Data, Max);
                    for (int i = 0; i < Num; i++)
                    {
                        Data[i].key.bl = (uint)reader.ReadInt32();
                        Data[i].key.bh = (uint)reader.ReadInt32();
                        Data[i].key.wl = (uint)reader.ReadInt32();
                        Data[i].key.wh = (uint)reader.ReadInt32();
                        Data[i].info = new PositionInfo();
                        Data[i].info.Value = reader.ReadInt32();
                    }
                }
                catch
                {
                    return false;
                }
            }
           
            return true;
        }
/*
        static int Opening_Write(const Opening *self, FILE *fp)
        {
            if (fwrite(&self->Num, sizeof(int), 1, fp) < 1) {
                return 0;
            }
            if (fwrite(self->Data, sizeof(PositionData), self->Num, fp) < (size_t)self->Num) {
                return 0;
            }
            return 1;
        }

*/
        static int PositionKey_Comp(PositionKey in_key1, PositionKey in_key2)
        {
            if (in_key1.wh > in_key2.wh) {
                return 1;
            } else if (in_key1.wh < in_key2.wh) {
                return -1;
            } else if (in_key1.wl > in_key2.wl) {
                return 1;
            } else if (in_key1.wl < in_key2.wl) {
                return -1;
            } else if (in_key1.bh > in_key2.bh) {
                return 1;
            } else if (in_key1.bh < in_key2.bh) {
                return -1;
            } else if (in_key1.bl > in_key2.bl) {
                return 1;
            } else if (in_key1.bl < in_key2.bl) {
                return -1;
            }
            return 0;
        }

        static int Board_RotatePos(int in_x, int in_y, int in_type)
        {
            switch (in_type) {
            case 0:
                return Board.Pos(in_x, in_y);
            case 1:
                return Board.Pos(Board.BOARD_SIZE - in_x - 1, in_y);
            case 2:
                return Board.Pos(in_x, Board.BOARD_SIZE - in_y - 1);
            case 3:
                return Board.Pos(Board.BOARD_SIZE - in_x - 1, Board.BOARD_SIZE - in_y - 1);
            case 4:         
                return Board.Pos(in_y, in_x);
            case 5:
                return Board.Pos(Board.BOARD_SIZE - in_y - 1, in_x);
            case 6:
                return Board.Pos(in_y, Board.BOARD_SIZE - in_x - 1);
            case 7:
                return Board.Pos(Board.BOARD_SIZE - in_y - 1, Board.BOARD_SIZE - in_x - 1);
            default:
                break;
            }
            return 0;
        }

        static void Board_Key(Board in_board, int in_color, out PositionKey out_key)
        {
            PositionKey key = new PositionKey();
            uint flag;
            int i, x, y, c;
            int op_color;

            out_key = new PositionKey();
            op_color = Board.OpponentColor(in_color);
            for (i = 0; i < 8; i++) {
                key.Zero();
                flag = 1;
                for (y = 0; y < Board.BOARD_SIZE / 2; y++)
                {
                    for (x = 0; x < Board.BOARD_SIZE; x++)
                    {
                        c = in_board.GetDisk(Board_RotatePos(x, y, i));
                        if (c == in_color) {
                            key.bl |= flag;
                        } else if (c == op_color) {
                            key.wl |= flag;
                        }
                        c = in_board.GetDisk(Board_RotatePos(x, y + Board.BOARD_SIZE / 2, i));
                        if (c == in_color) {
                            key.bh |= flag;
                        } else if (c == op_color) {
                            key.wh |= flag;
                        }
                        flag <<= 1;
                    }
                }
                if (i == 0 || PositionKey_Comp(key, out_key) < 0) {
                    out_key = key;
                }
            }
        }

        PositionInfo Find(Board in_board, int in_color)
        {
            int i;
            PositionKey key;

            Board_Key(in_board, in_color, out key);
            for (i = 0; i < Num; i++) {
                if (PositionKey_Comp(key, Data[i].key) == 0) {
                    return Data[i].info;
                }
            }
            return null;
        }

        public bool Info(Board in_board, int in_color, out PositionInfo out_info)
        {
             out_info = Find(in_board, in_color);
             if (out_info == null)
             {
                return false;
             }
             return true;
        }

        int SetInfo(Board in_board, int in_color, PositionInfo in_info)
        {
            PositionInfo info;

            info = Find(in_board, in_color);
            if (info != null) {
                info = in_info;
            } else {
                if (Num >= Max) {
                    Max += NUM_INFO_BLOCK;
                    Array.Resize(ref Data, Max);
                 }
                Board_Key(in_board, in_color, out Data[Num].key);
                Data[Num].info = in_info;
                Num++;
            }
            return 1;
        }
    }
}