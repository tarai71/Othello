using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello2
{
    class Hash
    {
        // �n�b�V���l
        public struct Value
        {
            public uint Low;		/* ��32bit */
            public uint High;		/* ��32bit */
        };

        // �ǖʏ��
        public struct Info
        {
            public int Lower;		/* �]���l�̉��� */
            public int Upper;		/* �]���l�̏�� */
            public byte Depth;  	/* �T���萔 */
            public byte Move;		/* �őP�� */
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