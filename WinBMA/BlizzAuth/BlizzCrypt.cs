using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WinBMA.BlizzAuth
{
    class BlizzCrypt
    {
        private int[] funky_consts_1 = {
            unchecked((int)0x955e4bd9), unchecked((int)0x89f3917d), 0x2f15544a, 0x7e0504eb, unchecked((int)0x9d7bb66b), 0x6f8a2fe4, 0x70e453c7, 0x79200e5e, 0x3ad2e43a, 0x02d06c4a, 
            unchecked((int)0xdbd8d328), unchecked((int)0xf1a426b8), 0x3658e88b, unchecked((int)0xfd949b2a), unchecked((int)0xf4eaf300), 0x54673a14, 0x19a250fa, 0x4cc1278d, 0x12855b5b, 0x25818d16, 
            0x2c6e6ee2, unchecked((int)0xab4a350d), 0x401d78f6, unchecked((int)0xddb99711), unchecked((int)0xe72626b4), unchecked((int)0x8bd8b5b0), unchecked((int)0xb7f3acf9), unchecked((int)0xea3c9e00), 0x05fee59e, 0x19136cdb, 
            0x7c83f2ab, unchecked((int)0x8b0a2a99)
		};

        private int[] funky_consts_2 = {
            unchecked((int)0x9403ecf6), 0x24ab7e62, unchecked((int)0xf06bb765), unchecked((int)0xa9a7d8e3), 0x56e338ec, unchecked((int)0xa5e418fa), 0x568d53fa, 0x61d2fa63, 0x6fd56ce5, 0x7f9bdd64, 
            0x7b4fdf6b, 0x0345627f, 0x06ff163c, 0x69e0d8ee, unchecked((int)0xf28532b3), 0x755bec12, 0x26fd4162, unchecked((int)0xa79f1268), unchecked((int)0xcdefce44), 0x4bb0e8f3, 
            unchecked((int)0xe3daca30), unchecked((int)0x93e4852b), unchecked((int)0xb7e14266), unchecked((int)0xd5c3e3af), 0x7a33cdde, 0x30b02806, 0x1ff386d5, unchecked((int)0xda74d118), unchecked((int)0xdc065349), 0x743e9227, 
            unchecked((int)0x95457c7b), 0x57205fb6
		};

        private byte[] xor_key_random37;
        private byte[] mash;

        private int[] otp;
        private int[] a;
        private int[] b;

        public BlizzCrypt(Region region)
        {
            xor_key_random37 = BlizzCrypt.GenerateRandomBytes(37);

            mash = new byte[55];
            Array.Copy(xor_key_random37, 0, mash, 0, 37);
            byte[] tmp = Encoding.ASCII.GetBytes(region.RegionString);
            Array.Copy(tmp, 0, mash, 37, Math.Min(tmp.Length, 2));
            tmp = Encoding.ASCII.GetBytes("Motorola RAZR v3");
            Array.Copy(tmp, 0, mash, 39, Math.Min(tmp.Length, 16));

            byte[] abyte1;
            (abyte1 = new byte[mash.Length + 1])[0] = 1;
            Array.Copy(mash, 0, abyte1, 1, mash.Length);

            byte[] abyte2 = abyte1;
            otp = new int[32];
            int i = otp.Length - 1;
            for (int k = abyte2.Length - 1; k >= 0; k -= 4)
            {
                int x = 0;
                for (int j1 = 3; j1 >= 0; j1--)
                    if (k - j1 >= 0)
                        x = (x <<= 8) | abyte2[k - j1] & 0xff;

                otp[i] = (int)x;
                i--;
            }

            int round_counter = 0;

            while (round_counter < 12)
            {
                if (round_counter == 0)
                {
                    a = new int[33];
                    DoEncryptWork(a, otp, funky_consts_2, funky_consts_1, 0x31d17657L);
                }
                else if (round_counter == 1)
                {
                    b = new int[32];
                    Array.Copy(otp, 0, b, 0, 32);
                }
                else if (round_counter < 10)
                {
                    DoEncryptWork(a, otp, otp, funky_consts_1, 0x31d17657L);
                }
                else if (round_counter == 10)
                {
                    DoEncryptWork(a, otp, b, funky_consts_1, 0x31d17657L);
                }
                else if (round_counter == 11)
                {
                    SetElementsToZero(b);
                    b[b.Length - 1] = 1;
                    DoEncryptWork(a, otp, b, funky_consts_1, 0x31d17657L);
                }
                round_counter++;
            }
        }

        private void SetElementsToZero(int[] ai)
        {
            for (int i = 0; i < ai.Length; i++)
                ai[i] = 0;
        }

        private void DoEncryptWork(int[] a1, int[] a2, int[] a3, int[] a4, long lfoo)
        {

            int i = a4.Length;
            int k = i - 1;
            long l2 = (long)a3[i - 1] & 0xffffffffL;
            SetElementsToZero(a1);
            for (int i1 = i; i1 > 0; i1--)
            {
                long l3 = (long)a2[i1 - 1] & 0xffffffffL;
                long l4 = (((long)a1[i] & 0xffffffffL) + (l3 * l2 & 0xffffffffL) & 0xffffffffL) * lfoo & 0xffffffffL;
                long l5 = l3 * l2;
                long l7 = l4 * ((long)a4[i - 1] & 0xffffffffL);
                long l9 = ((long)a1[i] & 0xffffffffL) + (l5 & 0xffffffffL) + (l7 & 0xffffffffL);
                long l11 = (long)(((ulong)l5) >> 32) + (long)(((ulong)l7) >> 32) + (long)(((ulong)l9) >> 32);
                for (int j1 = k; j1 > 0; j1--)
                {
                    long l6 = l3 * ((long)a3[j1 - 1] & 0xffffffffL);
                    long l8 = l4 * ((long)a4[j1 - 1] & 0xffffffffL);
                    long l10 = ((long)a1[j1] & 0xffffffffL) + (l6 & 0xffffffffL) + (l8 & 0xffffffffL) + (l11 & 0xffffffffL);

                    long s1 = (long)(((ulong)l11) >> 32);
                    long s2 = (long)(((ulong)l6) >> 32);
                    long s3 = (long)(((ulong)l8) >> 32);
                    long s4 = (long)(((ulong)l10) >> 32);

                    l11 = s1 + (long)s2 + s3 + s4;

                    a1[j1 + 1] = (int)l10;
                }

                l11 += (long)a1[0] & 0xffffffffL;
                a1[1] = (int)l11;
                a1[0] = (int)(((ulong)l11) >> 32);
            }

            if (CheckArray(a1, a4))
                FixArray(a1, a4);
            Array.Copy(a1, 1, a2, 0, i);
        }

        public byte[] EncryptDecrypt(byte[] message)
        {
            byte[] encryptedMessage = message;
            for (int i = xor_key_random37.Length - 1; i >= 0; i--)
                encryptedMessage[i] ^= xor_key_random37[i];

            return encryptedMessage;
        }

        private bool CheckArray(int[] aint1, int[] aint2)
        {
            if (aint1[0] != 0)
                return true;
            for (int i = 0; i < aint2.Length; i++)
            {
                long l1 = (long)aint1[i + 1] & 0xffffffffL;
                long l2 = (long)aint2[i] & 0xffffffffL;
                if (l1 > l2)
                    return true;
                if (l2 > l1)
                    return false;
            }

            return true;
        }

        private void FixArray(int[] aint1, int[] aint2)
        {
            int i = 0;
            for (int k = aint1.Length - 1; k >= 1; k--)
            {
                long l1 = (long)aint1[k] & 0xffffffffL;
                long l2 = (long)aint2[k - 1] & 0xffffffffL;
                l1 = (l1 - l2) + (long)i;
                aint1[k] = (int)l1;
                i = (int)(l1 >> 63);
            }

        }

        public byte[] OTP
        {
            get
            {
                return Helper.ConvertIntArrayToBytes(otp);
            }
        }

        public int[] OTPInt
        {
            get
            {
                return otp;
            }
        }

        public static byte[] GenerateRandomBytes(int size)
        {
            byte[] result = new byte[size];
            Random random = new Random();
            random.NextBytes(result);

            return result;
        }
    }
}
