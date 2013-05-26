using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello2
{
    class Hash
    {
        // ƒnƒbƒVƒ…’l
        public struct Value
        {
            public uint Low;		/* ‰º32bit */
            public uint High;		/* ã32bit */
        };

        // ‹Ç–Êî•ñ
        public struct Info
        {
            public int Lower;		/* •]‰¿’l‚Ì‰ºŒÀ */
            public int Upper;		/* •]‰¿’l‚ÌãŒÀ */
            public byte Depth;  	/* ’Tõè” */
            public byte Move;		/* Å‘Pè */
        };

        struct Data
        {
            public Value Val;
            public Info Info;
        };

        int Num;
        ulong Mask;
        Data[] data;
        int GetNum;
        int HitNum;

        public Hash(int in_size)
        {
            Initialize(in_size);
        }

        bool Initialize(int in_size)
        {
            Num = 1 << in_size;
            Mask = (ulong)((1 << in_size) - 1);
            data = new Data[Num];
            if (data == null)
            {
                return false;
            }
            Clear();
            return true;
        }

        public void Clear()
        {
            int i;

            for (i=0; i<Num; i++) {
                data[i].Val.Low = (uint)~i;
            }
            GetNum = 0;
            HitNum = 0;
        }

        public bool Set(Value in_value, Info in_info)
        {
            int i;

            i = (int)(in_value.Low & Mask);
            data[i].Val = in_value;
            data[i].Info = in_info;
            return true;
        }

        public bool Get(Value in_value, out Info out_info)
        {
            int i;

            out_info = new Info();
            i = (int)(in_value.Low & Mask);
            GetNum++;
            if (data[i].Val.Low == in_value.Low && data[i].Val.High == in_value.High) {
                out_info = data[i].Info;
                HitNum++;
                return true;
            }
            return false;
        }

        public void ClearInfo()
        {
            GetNum = 0;
            HitNum = 0;
        }

        public int CountGet()
        {
            return GetNum;
        }

        public int CountHit()
        {
            return HitNum;
        }
    }
}