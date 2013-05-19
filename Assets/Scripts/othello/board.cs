using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        // パターンID
        public const int PATTERN_ID_LINE4_1	=   0;
        public const int PATTERN_ID_LINE4_2	=   1;
        public const int PATTERN_ID_LINE4_3	=   2;
        public const int PATTERN_ID_LINE4_4	=   3;
        public const int PATTERN_ID_LINE3_1	=   4;
        public const int PATTERN_ID_LINE3_2	=   5;
        public const int PATTERN_ID_LINE3_3	=	6;
        public const int PATTERN_ID_LINE3_4	=	7;
        public const int PATTERN_ID_LINE2_1	=	8;
        public const int PATTERN_ID_LINE2_2	=	9;
        public const int PATTERN_ID_LINE2_3	=	10;
        public const int PATTERN_ID_LINE2_4	=	11;
        public const int PATTERN_ID_DIAG8_1	=	12;
        public const int PATTERN_ID_DIAG8_2	=	13;
        public const int PATTERN_ID_DIAG7_1	=	14;
        public const int PATTERN_ID_DIAG7_2	=	15;
        public const int PATTERN_ID_DIAG7_3	=	16;
        public const int PATTERN_ID_DIAG7_4	=	17;
        public const int PATTERN_ID_DIAG6_1	=	18;
        public const int PATTERN_ID_DIAG6_2	=	19;
        public const int PATTERN_ID_DIAG6_3	=	20;
        public const int PATTERN_ID_DIAG6_4	=	21;
        public const int PATTERN_ID_DIAG5_1	=	22;
        public const int PATTERN_ID_DIAG5_2	=	23;
        public const int PATTERN_ID_DIAG5_3	=	24;
        public const int PATTERN_ID_DIAG5_4	=	25;
        public const int PATTERN_ID_DIAG4_1	=	26;
        public const int PATTERN_ID_DIAG4_2	=	27;
        public const int PATTERN_ID_DIAG4_3	=	28;
        public const int PATTERN_ID_DIAG4_4	=	29;
        public const int PATTERN_ID_EDGE8_1	=	30;
        public const int PATTERN_ID_EDGE8_2	=	31;
        public const int PATTERN_ID_EDGE8_3	=	32;
        public const int PATTERN_ID_EDGE8_4	=	33;
        public const int PATTERN_ID_EDGE8_5	=	34;
        public const int PATTERN_ID_EDGE8_6	=	35;
        public const int PATTERN_ID_EDGE8_7 =	36;
        public const int PATTERN_ID_EDGE8_8 =	37;
        public const int PATTERN_ID_CORNER8_1 =	38;
        public const int PATTERN_ID_CORNER8_2 =	39;
        public const int PATTERN_ID_CORNER8_3 =	40;
        public const int PATTERN_ID_CORNER8_4 =	41;
        public const int NUM_PATTERN_ID =		42;

        const int NUM_DISK	= (BOARD_SIZE+1)*(BOARD_SIZE+2)+1;
        const int NUM_STACK	= ((BOARD_SIZE-2)*3+3)*BOARD_SIZE*BOARD_SIZE;
        const int NUM_PATTERN_DIFF	= 6;

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
        int[] DiskNum = new int[3];
        int[] Pattern = new int[NUM_PATTERN_ID];
        int[,] PatternID = new int[NUM_DISK, NUM_PATTERN_DIFF];
        int[,] PatternDiff = new int[NUM_DISK, NUM_PATTERN_DIFF];
        Hash.Value[] HashDiffBlack = new Hash.Value[NUM_DISK];
        Hash.Value[] HashDiffWhite = new Hash.Value[NUM_DISK];
        Hash.Value HashDiffTurn;
        Random rand = new Random();

 /*
        static int Board_Initialize(Board *self);
        static int Board_Finalize(Board *self);
        static int Board_FlipLine(Board *self, int in_color, int in_pos, int in_dir);
        static int CountFlipsLine(const Board *self, int in_color, int in_pos, int in_dir);
        static void Board_InitializePatternDiff(Board *self);
        static void Board_InitializeHashDiff(Board *self);
        static void Board_AddPattern(Board *self, int in_id, const int *in_pos_list, int in_num);
        static int Board_FlipLinePattern(Board *self, int in_color, int in_pos, int in_dir);
        static void Board_FlipSquareBlack(Board *self, int in_pos);
        static void Board_FlipSquareWhite(Board *self, int in_pos);
        static void Board_PutSquareBlack(Board *self, int in_pos);
        static void Board_PutSquareWhite(Board *self, int in_pos);
        static void Board_RemoveSquareBlack(Board *self, int in_pos);
        static void Board_RemoveSquareWhite(Board *self, int in_pos);
        #define get_rand(in_max) ((int)((double)(in_max) * rand() / (RAND_MAX + 1.0)))

        Board *Board_New(void)
        {
            Board *self;

            self = malloc(sizeof(Board));
            if (self) {
                Board_InitializePatternDiff(self);
                Board_InitializeHashDiff(self);
                Board_Clear(self);
            }
            return self;
        }

        void Board_Delete(Board *self)
        {
            free(self);
        }
*/

        int get_rand(int in_max)
        {
            return rand.Next(in_max);
        }

        public Board()
        {
            InitializePatternDiff();
            InitializeHashDiff();
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
            DiskNum[BLACK] = 2;
            DiskNum[WHITE] = 2;
            DiskNum[EMPTY] = BOARD_SIZE * BOARD_SIZE - 4;

            InitializePattern();
        }

        public int GetDisk(int in_pos)
        {
            return Disk[in_pos];
        }

        public int CountDisks(int in_color)
        {
            return DiskNum[in_color];
        }

        int FlipLine(int in_color, int in_pos, int in_dir)
        {
            int result = 0;
            int op = OpponentColor(in_color);
            int pos;

            pos = in_pos + in_dir;
            if (Disk[pos] != op) {
                return 0;
            }
            pos += in_dir;
            if (Disk[pos] == op) {
                pos += in_dir;
                if (Disk[pos] == op) {
                    pos += in_dir;
                    if (Disk[pos] == op) {
                        pos += in_dir;
                        if (Disk[pos] == op) {
                            pos += in_dir;
                            if (Disk[pos] == op) {
                                pos += in_dir;
                                if (Disk[pos] != in_color) {
                                    return 0;
                                }
                                pos -= in_dir;
                                result ++;
                                Disk[pos] = in_color;
                                Push(pos);
                            } else if (Disk[pos] != in_color) {
                                return 0;
                            }
                            pos -= in_dir;
                            result ++;
                            Disk[pos] = in_color;
                            Push(pos);
                        } else if (Disk[pos] != in_color) {
                            return 0;
                        }
                        pos -= in_dir;
                        result ++;
                        Disk[pos] = in_color;
                        Push(pos);
                    } else if (Disk[pos] != in_color) {
                        return 0;
                    }
                    pos -= in_dir;
                    result ++;
                    Disk[pos] = in_color;
                    Push(pos);
                } else if (Disk[pos] != in_color) {
                    return 0;
                }
                pos -= in_dir;
                result ++;
                Disk[pos] = in_color;
                Push(pos);
            } else if (Disk[pos] != in_color) {
                return 0;
            }
            pos -= in_dir;
            result ++;
            Disk[pos] = in_color;
            Push(pos);

            return result;
        }

        public int Flip(int in_color, int in_pos)
        {
            int result = 0;

            if (Disk[in_pos] != EMPTY) {
                return 0;
            }
            switch (in_pos) {
            case C1:
            case C2:
            case D1:
            case D2:
            case E1:
            case E2:
            case F1:
            case F2:
                result += FlipLine(in_color, in_pos, DIR_LEFT);
                result += FlipLine(in_color, in_pos, DIR_RIGHT);
                result += FlipLine(in_color, in_pos, DIR_DOWN_LEFT);
                result += FlipLine(in_color, in_pos, DIR_DOWN);
                result += FlipLine(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case C8:
            case C7:
            case D8:
            case D7:
            case E8:
            case E7:
            case F8:
            case F7:
                result += FlipLine(in_color, in_pos, DIR_UP_LEFT);
                result += FlipLine(in_color, in_pos, DIR_UP);
                result += FlipLine(in_color, in_pos, DIR_UP_RIGHT);
                result += FlipLine(in_color, in_pos, DIR_LEFT);
                result += FlipLine(in_color, in_pos, DIR_RIGHT);
                break;
            case A3:
            case A4:
            case A5:
            case A6:
            case B3:
            case B4:
            case B5:
            case B6:
                result += FlipLine(in_color, in_pos, DIR_UP);
                result += FlipLine(in_color, in_pos, DIR_UP_RIGHT);
                result += FlipLine(in_color, in_pos, DIR_RIGHT);
                result += FlipLine(in_color, in_pos, DIR_DOWN);
                result += FlipLine(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case H3:
            case H4:
            case H5:
            case H6:
            case G3:
            case G4:
            case G5:
            case G6:
                result += FlipLine(in_color, in_pos, DIR_UP_LEFT);
                result += FlipLine(in_color, in_pos, DIR_UP);
                result += FlipLine(in_color, in_pos, DIR_LEFT);
                result += FlipLine(in_color, in_pos, DIR_DOWN_LEFT);
                result += FlipLine(in_color, in_pos, DIR_DOWN);
                break;
            case A1:
            case A2:
            case B1:
            case B2:
                result += FlipLine(in_color, in_pos, DIR_RIGHT);
                result += FlipLine(in_color, in_pos, DIR_DOWN);
                result += FlipLine(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case A8:
            case A7:
            case B8:
            case B7:
                result += FlipLine(in_color, in_pos, DIR_UP);
                result += FlipLine(in_color, in_pos, DIR_UP_RIGHT);
                result += FlipLine(in_color, in_pos, DIR_RIGHT);
                break;
            case H1:
            case H2:
            case G1:
            case G2:
                result += FlipLine(in_color, in_pos, DIR_LEFT);
                result += FlipLine(in_color, in_pos, DIR_DOWN_LEFT);
                result += FlipLine(in_color, in_pos, DIR_DOWN);
                break;
            case H8:
            case H7:
            case G8:
            case G7:
                result += FlipLine(in_color, in_pos, DIR_UP_LEFT);
                result += FlipLine(in_color, in_pos, DIR_UP);
                result += FlipLine(in_color, in_pos, DIR_LEFT);
                break;
            default:
                result += FlipLine(in_color, in_pos, DIR_UP_LEFT);
                result += FlipLine(in_color, in_pos, DIR_UP);
                result += FlipLine(in_color, in_pos, DIR_UP_RIGHT);
                result += FlipLine(in_color, in_pos, DIR_LEFT);
                result += FlipLine(in_color, in_pos, DIR_RIGHT);
                result += FlipLine(in_color, in_pos, DIR_DOWN_LEFT);
                result += FlipLine(in_color, in_pos, DIR_DOWN);
                result += FlipLine(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            }
            if (result > 0) {
                Disk[in_pos] = in_color;
                Push(in_pos);
                Push(OpponentColor(in_color));
                Push(result);
                DiskNum[in_color] += result + 1;
                DiskNum[OpponentColor(in_color)] -= result;
                DiskNum[EMPTY]--;
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
            DiskNum[color] += result;
            DiskNum[OpponentColor(color)] -= result + 1;
            DiskNum[EMPTY]++;

            return result;
        }

        public void InitializePattern()
        {
            int i;
            for (i = 0; i < NUM_PATTERN_ID; i++) {
                Pattern[i] = 0;
            }
            for (i = 0; i < NUM_DISK; i++) {
                if (Disk[i] == BLACK) {
                    PutSquareBlack(i);
                } else if (Disk[i] == WHITE) {
                    PutSquareWhite(i);
                }
            }
        }

        public int GetPattern(int in_id)
        {
            return Pattern[in_id];
        }

        void AddPattern(int in_id, int[] in_pos_list, int in_num)
        {
            int i, j, n;

            n = 1;
            for (i = 0; i < in_num; i++) {
                for (j = 0; PatternDiff[in_pos_list[i],j] != 0; j++) {
                }
                PatternID[in_pos_list[i],j] = in_id;
                PatternDiff[in_pos_list[i],j] = n;
                n *= 3;
            }
        }

        void InitializePatternDiff()
        {
            int i, j;
            int[][] pattern_list = new int[][] {
                new int[] { A4, B4, C4, D4, E4, F4, G4, H4, -1 },
                new int[] { A5, B5, C5, D5, E5, F5, G5, H5, -1 },
                new int[] { D1, D2, D3, D4, D5, D6, D7, D8, -1 },
                new int[] { E1, E2, E3, E4, E5, E6, E7, E8, -1 },
                new int[] { A3, B3, C3, D3, E3, F3, G3, H3, -1 },
                new int[] { A6, B6, C6, D6, E6, F6, G6, H6, -1 },
                new int[] { C1, C2, C3, C4, C5, C6, C7, C8, -1 },
                new int[] { F1, F2, F3, F4, F5, F6, F7, F8, -1 },
                new int[] { A2, B2, C2, D2, E2, F2, G2, H2, -1 },
                new int[] { A7, B7, C7, D7, E7, F7, G7, H7, -1 },
                new int[] { B1, B2, B3, B4, B5, B6, B7, B8, -1 },
                new int[] { G1, G2, G3, G4, G5, G6, G7, G8, -1 },
                new int[] { A1, B2, C3, D4, E5, F6, G7, H8, -1 },
                new int[] { A8, B7, C6, D5, E4, F3, G2, H1, -1 },
                new int[] { A2, B3, C4, D5, E6, F7, G8, -1 },
                new int[] { B1, C2, D3, E4, F5, G6, H7, -1 },
                new int[] { A7, B6, C5, D4, E3, F2, G1, -1 },
                new int[] { B8, C7, D6, E5, F4, G3, H2, -1 },
                new int[] { A3, B4, C5, D6, E7, F8, -1 },
                new int[] { C1, D2, E3, F4, G5, H6, -1 },
                new int[] { A6, B5, C4, D3, E2, F1, -1 },
                new int[] { C8, D7, E6, F5, G4, H3, -1 },
                new int[] { A4, B5, C6, D7, E8, -1 },
                new int[] { D1, E2, F3, G4, H5, -1 },
                new int[] { A5, B4, C3, D2, E1, -1 },
                new int[] { D8, E7, F6, G5, H4, -1 },
                new int[] { A5, B6, C7, D8, -1 },
                new int[] { E1, F2, G3, H4, -1 },
                new int[] { A4, B3, C2, D1, -1 },
                new int[] { E8, F7, G6, H5, -1 },
                new int[] { B2, G1, F1, E1, D1, C1, B1, A1, -1 },
                new int[] { G2, B1, C1, D1, E1, F1, G1, H1, -1 },
                new int[] { B7, G8, F8, E8, D8, C8, B8, A8, -1 },
                new int[] { G7, B8, C8, D8, E8, F8, G8, H8, -1 },
                new int[] { B2, A7, A6, A5, A4, A3, A2, A1, -1 },
                new int[] { B7, A2, A3, A4, A5, A6, A7, A8, -1 },
                new int[] { G2, H7, H6, H5, H4, H3, H2, H1, -1 },
                new int[] { G7, H2, H3, H4, H5, H6, H7, H8, -1 },
                new int[] { B3, A3, C2, B2, A2, C1, B1, A1, -1 },
                new int[] { G3, H3, F2, G2, H2, F1, G1, H1, -1 },
                new int[] { B6, A6, C7, B7, A7, C8, B8, A8, -1 },
                new int[] { G6, H6, F7, G7, H7, F8, G8, H8, -1 },
                new int[] { -1 }
            };

            for (i = 0; i < NUM_DISK; i++) {
                for (j = 0; j < NUM_PATTERN_DIFF; j++) {
                    PatternID[i,j] = 0;
                    PatternDiff[i,j] = 0;
                }
            }
            for (i = 0; pattern_list[i][0] >= 0; i++) {
                for (j = 0; pattern_list[i][j] >= 0; j++) {}
                AddPattern(i, pattern_list[i], j);
            }
        }

        void InitializeHashDiff()
        {
            int i;

            HashDiffTurn.Low = (uint)get_rand(256) | ((uint)get_rand(256) << 8) |
                ((uint)get_rand(256) << 16) | ((uint)get_rand(256) << 24); ;
            HashDiffTurn.High = (uint)get_rand(256) | ((uint)get_rand(256) << 8) |
                ((uint)get_rand(256) << 16) | ((uint)get_rand(256) << 24); ;
            for (i = 0; i < NUM_DISK; i++) {
                HashDiffBlack[i].Low = (uint)get_rand(256) | ((uint)get_rand(256) << 8) |
                    ((uint)get_rand(256) << 16) | ((ulong)get_rand(256) << 24);
                HashDiffBlack[i].High = (uint)get_rand(256) | ((uint)get_rand(256) << 8) |
                    ((uint)get_rand(256) << 16) | ((ulong)get_rand(256) << 24);
                HashDiffWhite[i].Low = (uint)get_rand(256) | ((uint)get_rand(256) << 8) |
                    ((uint)get_rand(256) << 16) | ((ulong)get_rand(256) << 24);
                HashDiffWhite[i].High = (uint)get_rand(256) | ((uint)get_rand(256) << 8) |
                    ((uint)get_rand(256) << 16) | ((uint)get_rand(256) << 24);
            }
        }

        void FlipSquareBlack(int in_pos)
        {
            Disk[in_pos] = BLACK;
            Pattern[PatternID[in_pos,0]] -= PatternDiff[in_pos,0];
            Pattern[PatternID[in_pos,1]] -= PatternDiff[in_pos,1];
            Pattern[PatternID[in_pos,2]] -= PatternDiff[in_pos,2];
            Pattern[PatternID[in_pos,3]] -= PatternDiff[in_pos,3];
            Pattern[PatternID[in_pos,4]] -= PatternDiff[in_pos,4];
            Pattern[PatternID[in_pos,5]] -= PatternDiff[in_pos,5];
        }

        void FlipSquareWhite(int in_pos)
        {
            Disk[in_pos] = WHITE;
            Pattern[PatternID[in_pos,0]] += PatternDiff[in_pos,0];
            Pattern[PatternID[in_pos,1]] += PatternDiff[in_pos,1];
            Pattern[PatternID[in_pos,2]] += PatternDiff[in_pos,2];
            Pattern[PatternID[in_pos,3]] += PatternDiff[in_pos,3];
            Pattern[PatternID[in_pos,4]] += PatternDiff[in_pos,4];
            Pattern[PatternID[in_pos,5]] += PatternDiff[in_pos,5];
        }

        void PutSquareBlack(int in_pos)
        {
            Disk[in_pos] = BLACK;
            Pattern[PatternID[in_pos,0]] += PatternDiff[in_pos,0];
            Pattern[PatternID[in_pos,1]] += PatternDiff[in_pos,1];
            Pattern[PatternID[in_pos,2]] += PatternDiff[in_pos,2];
            Pattern[PatternID[in_pos,3]] += PatternDiff[in_pos,3];
            Pattern[PatternID[in_pos,4]] += PatternDiff[in_pos,4];
            Pattern[PatternID[in_pos,5]] += PatternDiff[in_pos,5];
        }

        void PutSquareWhite(int in_pos)
        {
            Disk[in_pos] = WHITE;
            Pattern[PatternID[in_pos,0]] += PatternDiff[in_pos,0] + PatternDiff[in_pos,0];
            Pattern[PatternID[in_pos,1]] += PatternDiff[in_pos,1] + PatternDiff[in_pos,1];
            Pattern[PatternID[in_pos,2]] += PatternDiff[in_pos,2] + PatternDiff[in_pos,2];
            Pattern[PatternID[in_pos,3]] += PatternDiff[in_pos,3] + PatternDiff[in_pos,3];
            Pattern[PatternID[in_pos,4]] += PatternDiff[in_pos,4] + PatternDiff[in_pos,4];
            Pattern[PatternID[in_pos,5]] += PatternDiff[in_pos,5] + PatternDiff[in_pos,5];
        }

        void RemoveSquareBlack(int in_pos)
        {
            Disk[in_pos] = EMPTY;
            Pattern[PatternID[in_pos,0]] -= PatternDiff[in_pos,0];
            Pattern[PatternID[in_pos,1]] -= PatternDiff[in_pos,1];
            Pattern[PatternID[in_pos,2]] -= PatternDiff[in_pos,2];
            Pattern[PatternID[in_pos,3]] -= PatternDiff[in_pos,3];
            Pattern[PatternID[in_pos,4]] -= PatternDiff[in_pos,4];
            Pattern[PatternID[in_pos,5]] -= PatternDiff[in_pos,5];
        }

        void RemoveSquareWhite(int in_pos)
        {
            Disk[in_pos] = EMPTY;
            Pattern[PatternID[in_pos,0]] -= PatternDiff[in_pos,0] + PatternDiff[in_pos,0];
            Pattern[PatternID[in_pos,1]] -= PatternDiff[in_pos,1] + PatternDiff[in_pos,1];
            Pattern[PatternID[in_pos,2]] -= PatternDiff[in_pos,2] + PatternDiff[in_pos,2];
            Pattern[PatternID[in_pos,3]] -= PatternDiff[in_pos,3] + PatternDiff[in_pos,3];
            Pattern[PatternID[in_pos,4]] -= PatternDiff[in_pos,4] + PatternDiff[in_pos,4];
            Pattern[PatternID[in_pos,5]] -= PatternDiff[in_pos,5] + PatternDiff[in_pos,5];
        }

        delegate void funcFlip(int in_pos);
        int FlipLinePattern(int in_color, int in_pos, int in_dir)
        {
            int result = 0;
            int op = OpponentColor(in_color);
            int pos;
            funcFlip func_flip;

            if (in_color == BLACK) {
                func_flip = FlipSquareBlack;
            } else {
                func_flip = FlipSquareWhite;
            }

            pos = in_pos + in_dir;
            if (Disk[pos] != op) {
                return 0;
            }
            pos += in_dir;
            if (Disk[pos] == op) {
                pos += in_dir;
                if (Disk[pos] == op) {
                    pos += in_dir;
                    if (Disk[pos] == op) {
                        pos += in_dir;
                        if (Disk[pos] == op) {
                            pos += in_dir;
                            if (Disk[pos] == op) {
                                pos += in_dir;
                                if (Disk[pos] != in_color) {
                                    return 0;
                                }
                                pos -= in_dir;
                                result ++;
                                func_flip(pos);
                                Push(pos);
                            } else if (Disk[pos] != in_color) {
                                return 0;
                            }
                            pos -= in_dir;
                            result ++;
                            func_flip(pos);
                            Push(pos);
                        } else if (Disk[pos] != in_color) {
                            return 0;
                        }
                        pos -= in_dir;
                        result ++;
                        func_flip(pos);
                        Push(pos);
                    } else if (Disk[pos] != in_color) {
                        return 0;
                    }
                    pos -= in_dir;
                    result ++;
                    func_flip(pos);
                    Push(pos);
                } else if (Disk[pos] != in_color) {
                    return 0;
                }
                pos -= in_dir;
                result ++;
                func_flip(pos);
                Push(pos);
            } else if (Disk[pos] != in_color) {
                return 0;
            }
            pos -= in_dir;
            result ++;
            func_flip(pos);
            Push(pos);

            return result;
        }

        public int FlipPattern(int in_color, int in_pos)
        {
            int result = 0;

            if (Disk[in_pos] != EMPTY) {
                return 0;
            }
            switch (in_pos) {
            case C1:
            case C2:
            case D1:
            case D2:
            case E1:
            case E2:
            case F1:
            case F2:
                result += FlipLinePattern(in_color, in_pos, DIR_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_RIGHT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case C8:
            case C7:
            case D8:
            case D7:
            case E8:
            case E7:
            case F8:
            case F7:
                result += FlipLinePattern(in_color, in_pos, DIR_UP_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_UP);
                result += FlipLinePattern(in_color, in_pos, DIR_UP_RIGHT);
                result += FlipLinePattern(in_color, in_pos, DIR_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_RIGHT);
                break;
            case A3:
            case A4:
            case A5:
            case A6:
            case B3:
            case B4:
            case B5:
            case B6:
                result += FlipLinePattern(in_color, in_pos, DIR_UP);
                result += FlipLinePattern(in_color, in_pos, DIR_UP_RIGHT);
                result += FlipLinePattern(in_color, in_pos, DIR_RIGHT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case H3:
            case H4:
            case H5:
            case H6:
            case G3:
            case G4:
            case G5:
            case G6:
                result += FlipLinePattern(in_color, in_pos, DIR_UP_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_UP);
                result += FlipLinePattern(in_color, in_pos, DIR_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN);
                break;
            case A1:
            case A2:
            case B1:
            case B2:
                result += FlipLinePattern(in_color, in_pos, DIR_RIGHT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case A8:
            case A7:
            case B8:
            case B7:
                result += FlipLinePattern(in_color, in_pos, DIR_UP);
                result += FlipLinePattern(in_color, in_pos, DIR_UP_RIGHT);
                result += FlipLinePattern(in_color, in_pos, DIR_RIGHT);
                break;
            case H1:
            case H2:
            case G1:
            case G2:
                result += FlipLinePattern(in_color, in_pos, DIR_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN);
                break;
            case H8:
            case H7:
            case G8:
            case G7:
                result += FlipLinePattern(in_color, in_pos, DIR_UP_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_UP);
                result += FlipLinePattern(in_color, in_pos, DIR_LEFT);
                break;
            default:
                result += FlipLinePattern(in_color, in_pos, DIR_UP_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_UP);
                result += FlipLinePattern(in_color, in_pos, DIR_UP_RIGHT);
                result += FlipLinePattern(in_color, in_pos, DIR_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_RIGHT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN_LEFT);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN);
                result += FlipLinePattern(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            }
            if (result > 0) {
                if (in_color == BLACK) {
                    PutSquareBlack(in_pos);
                } else {
                    PutSquareWhite(in_pos);
                }
                Push(in_pos);
                Push(OpponentColor(in_color));
                Push(result);
                DiskNum[in_color] += result + 1;
                DiskNum[OpponentColor(in_color)] -= result;
                DiskNum[EMPTY]--;
            }

            return result;
        }

        public int UnflipPattern()
        {
            int result;
            int i, color;

            if (Sp <= 0) {
                return 0;
            }
            result = Pop();
            color = Pop();
            if (color == BLACK) {
                RemoveSquareWhite(Pop());
                for (i = 0; i < result; i++) {
                    FlipSquareBlack(Pop());
                }
            } else {
                RemoveSquareBlack(Pop());
                for (i = 0; i < result; i++) {
                    FlipSquareWhite(Pop());
                }
            }
            DiskNum[color] += result;
            DiskNum[OpponentColor(color)] -= result + 1;
            DiskNum[EMPTY]++;

            return result;
        }

        public void HashValue(int in_color, out Hash.Value out_value)
        {
            int i;

            out_value = new Hash.Value();
            out_value.Low = 0;
            out_value.High = 0;
            for (i = 0; i < NUM_DISK; i++) {
                switch (Disk[i]) {
                case BLACK:
                    out_value.Low ^= HashDiffBlack[i].Low;
                    out_value.High ^= HashDiffBlack[i].High;
                    break;
                case WHITE:
                    out_value.Low ^= HashDiffWhite[i].Low;
                    out_value.High ^= HashDiffWhite[i].High;
                    break;
                default:
                    break;
                }
            }
            if (in_color == WHITE) {
                out_value.Low ^= HashDiffTurn.Low;
                out_value.High ^= HashDiffTurn.High;
            }
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
            switch (in_pos) {
            case C1:
            case C2:
            case D1:
            case D2:
            case E1:
            case E2:
            case F1:
            case F2:
                result += CountFlipsLine(in_color, in_pos, DIR_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_RIGHT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case C8:
            case C7:
            case D8:
            case D7:
            case E8:
            case E7:
            case F8:
            case F7:
                result += CountFlipsLine(in_color, in_pos, DIR_UP_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_UP);
                result += CountFlipsLine(in_color, in_pos, DIR_UP_RIGHT);
                result += CountFlipsLine(in_color, in_pos, DIR_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_RIGHT);
                break;
            case A3:
            case A4:
            case A5:
            case A6:
            case B3:
            case B4:
            case B5:
            case B6:
                result += CountFlipsLine(in_color, in_pos, DIR_UP);
                result += CountFlipsLine(in_color, in_pos, DIR_UP_RIGHT);
                result += CountFlipsLine(in_color, in_pos, DIR_RIGHT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case H3:
            case H4:
            case H5:
            case H6:
            case G3:
            case G4:
            case G5:
            case G6:
                result += CountFlipsLine(in_color, in_pos, DIR_UP_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_UP);
                result += CountFlipsLine(in_color, in_pos, DIR_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN);
                break;
            case A1:
            case A2:
            case B1:
            case B2:
                result += CountFlipsLine(in_color, in_pos, DIR_RIGHT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            case A8:
            case A7:
            case B8:
            case B7:
                result += CountFlipsLine(in_color, in_pos, DIR_UP);
                result += CountFlipsLine(in_color, in_pos, DIR_UP_RIGHT);
                result += CountFlipsLine(in_color, in_pos, DIR_RIGHT);
                break;
            case H1:
            case H2:
            case G1:
            case G2:
                result += CountFlipsLine(in_color, in_pos, DIR_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN);
                break;
            case H8:
            case H7:
            case G8:
            case G7:
                result += CountFlipsLine(in_color, in_pos, DIR_UP_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_UP);
                result += CountFlipsLine(in_color, in_pos, DIR_LEFT);
                break;
            default:
                result += CountFlipsLine(in_color, in_pos, DIR_UP_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_UP);
                result += CountFlipsLine(in_color, in_pos, DIR_UP_RIGHT);
                result += CountFlipsLine(in_color, in_pos, DIR_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_RIGHT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN_LEFT);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN);
                result += CountFlipsLine(in_color, in_pos, DIR_DOWN_RIGHT);
                break;
            }

            return result;
        }

        bool CanFlip(int in_color, int in_pos)
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
            out_board.DiskNum.CopyTo(DiskNum, 0);
            out_board.Pattern.CopyTo(Pattern, 0);
            PatternID = (int[,])out_board.PatternID.Clone();
            PatternDiff = (int[,])out_board.PatternDiff.Clone();
           
            out_board.HashDiffBlack.CopyTo(HashDiffBlack, 0);
            out_board.HashDiffWhite.CopyTo(HashDiffWhite, 0);
            HashDiffTurn = out_board.HashDiffTurn;
            Sp = out_board.Sp;
        }

        public void Reverse()
        {
            int pos;
            int p;
            int n;

            for (pos = 0; pos < NUM_DISK; pos++)
            {
                if (Disk[pos] == BLACK)
                {
                    Disk[pos] = WHITE;
                }
                else if (Disk[pos] == WHITE)
                {
                    Disk[pos] = BLACK;
                }
            }
            for (p = Sp; p > 0; )
            {
                p--;
                n = Stack[p];
                p--;
                Stack[p] = OpponentColor(Stack[p]);
                p -= n + 1;
            }
            InitializePattern();
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

	    public void Push(int in_n)
	    {
		    Stack[Sp++] = in_n;
	    }

	    public int Pop()
	    {
		    return Stack[--Sp];
	    }

        public static int Pos(int in_x, int in_y)
        {
            return (in_y + 1) * (BOARD_SIZE + 1) + in_x + 1;
        }

        public static int X(int in_pos)
        {
            return in_pos % (BOARD_SIZE + 1) - 1;
        }

        public static int Y(int in_pos)
        {
            return in_pos / (BOARD_SIZE + 1) - 1;
        }

	    public static int OpponentColor(int in_color)
	    {
		    return BLACK + WHITE - in_color;
	    }
    }
}