using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikonovAV.HM.BlowfishCrypt
{
    public class BlowFeistel
    {
        public void BlowfishEncrypt(BlowfishContext context, ref UInt32 left, ref UInt32 right)
        {
            for (int i = 0; i < 16; i++)
            {
                left ^= context.PKeys[i];
                right ^= FeistelFunc(left, context);
                Swap(ref left, ref right);
            }
            Swap(ref left, ref right);
            left ^= context.PKeys[17];
            right ^= context.PKeys[16];
        }

        public void BlowfishDecrypt(BlowfishContext context, ref UInt32 left, ref UInt32 right)
        {
            for (int i = 17; i > 1; i--)
            {
                left ^= context.PKeys[i];
                right ^= FeistelFunc(left, context);
                Swap(ref left, ref right);
            }
            Swap(ref left, ref right);
            left ^= context.PKeys[0];
            right ^= context.PKeys[1];
        }

        private void Swap(ref UInt32 a, ref UInt32 b)
        {
            UInt32 temp = a;
            a = b;
            b = temp;
        }

        private UInt32 FeistelFunc(uint halfBlock, BlowfishContext context)
        {
            byte x4 = (byte)(halfBlock & 0xFF);
            halfBlock >>= 8;
            byte x3 = (byte)(halfBlock & 0xFF);
            halfBlock >>= 8;
            byte x2 = (byte)(halfBlock & 0xFF);
            halfBlock >>= 8;
            byte x1 = (byte)(halfBlock & 0xFF);

            uint result = ((context.SKeys[0][x1] + context.SKeys[1][x2]) ^ context.SKeys[2][x3]) + context.SKeys[3][x4];

            return result;
        }

        public void CryptPkeys(BlowfishContext context) 
        {
            //encrypt PKeys
            uint tempKey1 = 0u;
            uint tempKey2 = 0u;
            for (int i = 0; i < 18; i++)
            {
                BlowfishEncrypt(context, ref tempKey1, ref tempKey2);
                context.PKeys[i] = tempKey1;
                context.PKeys[++i] = tempKey2;
            }
            //encrypt SKeys
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    BlowfishEncrypt(context, ref tempKey1, ref tempKey2);
                    context.SKeys[i][j] = tempKey1;
                    context.SKeys[i][++j] = tempKey2;
                }
            }
        }

        public static void LongDivide(ulong data, ref uint left, ref uint right) 
        {
            right = (uint)(data & 0x00000000FFFFFFFF);
            left = (uint)((data >> 32) & 0x00000000FFFFFFFF);
        }

        public static ulong UnionTwoUInt(uint left, uint right)
        {
            ulong result = left;
            return (result << 32) | right;
        }
    }
}
