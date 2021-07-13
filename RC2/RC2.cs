using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC2
{
    class RC2
    {
        ushort[] K;
        ushort[] R;
        int j;

        public string GenerateKey()
        {
            Random rnd = new Random();
            string key = "";
            for (int i = 0; i < 8; i++)
                key += rnd.Next(10);
            return key;
        }

        public string Encript(string input, string key)
        {
            if (input.Length % 2 == 1)
                input += " ";

            string output;

            ushort[] words = ConvertToUshort(Encoding.Unicode.GetBytes(input));

            K = ExpandKey(key);

            for (int k = 0; k < words.Length; k++)
            {
                for (int i = 0; i < 4; i++)
                    R[i] = words[k + i];

                j = 0;

                for (int i = 0; i < 5; i++)
                    MixingRound();

                MashingRound();

                for (int i = 0; i < 6; i++)
                    MixingRound();

                MashingRound();

                for (int i = 0; i < 5; i++)
                    MixingRound();

                for (int i = 0; i < 4; i++)
                    words[k + i] = R[i];
            }
            output = Encoding.Unicode.GetString(ConvertToByte(words));

            return output;
        }

        public string Decript(string input, string key)
        {
            string output;

            ushort[] words = ConvertToUshort(Encoding.Unicode.GetBytes(input));

            K = ExpandKey(key);

            for (int k = 0; k < words.Length; k++)
            {
                for (int i = 0; i < 4; i++)
                    R[i] = words[k + i];

                j = 63;

                for (int i = 0; i < 5; i++)
                    RMixingRound();

                RMashingRound();

                for (int i = 0; i < 6; i++)
                    RMixingRound();

                RMashingRound();

                for (int i = 0; i < 5; i++)
                    RMixingRound();

                for (int i = 0; i < 4; i++)
                    words[k + i] = R[i];
            }
            output = Encoding.Unicode.GetString(ConvertToByte(words));

            return output;
        }

        private ushort[] ExpandKey(string key)
        {
            ushort[] K = new ushort[64];
            byte[] L = new byte[128];
            byte[] inputKey = Encoding.Unicode.GetBytes(key);
            int T = inputKey.Length;
            int T1 = T;
            int T8 = (T1 + 7) / 8;
            int TM = 255 % (int)Math.Pow(2, 8 + T1 - 8 * T8);

            byte[] pitable = new byte[256]
            { 217, 120, 249, 196, 25, 221, 181, 237, 40, 233, 253, 121, 74, 160, 216, 157,
              198, 126, 55, 131, 43, 118, 83, 142, 98, 76, 100, 136, 68, 139, 251, 162,
              23, 154, 89, 245, 135, 179, 79, 19, 97, 69, 109, 141, 9, 129, 125, 50,
              189, 143, 64, 235, 134, 183, 123, 11, 240, 149, 33, 34, 92, 107, 78, 130,
              84, 214, 101, 147, 206, 96, 178, 28, 115, 86, 192, 20, 167, 140, 241, 220,
              18, 117, 202, 31, 59, 190, 228, 209, 66, 61, 212, 48, 163, 60, 182, 38,
              111, 191, 14, 218, 70, 105, 7, 87, 39, 242, 29, 155, 188, 148, 67, 3,
              248, 17, 199, 246, 144, 239, 62, 231, 6, 195, 213, 47, 200, 102, 30, 215,
              8, 232, 234, 222, 128, 82, 238, 247, 132, 170, 114, 172, 53, 77, 106, 42,
              150, 26, 210, 113, 90, 21, 73, 116, 75, 159, 208, 94, 4, 24, 164, 236,
              194, 224, 65, 110, 15, 81, 203, 204, 36, 145, 175, 80, 161, 244, 112, 57,
              153, 124, 58, 133, 35, 184, 180, 122, 252, 2, 54, 91, 37, 85, 151, 49,
              45, 93, 250, 152, 227, 138, 146, 174, 5, 223, 41, 16, 103, 108, 186, 201,
              211, 0, 230, 207, 225, 158, 168, 44, 99, 22, 1, 63, 88, 226, 137, 169,
              13, 56, 52, 27, 171, 51, 255, 176, 187, 72, 12, 95, 185, 177, 205, 46,
              197, 243, 219, 71, 229, 165, 156, 119, 10, 166, 32, 104, 254, 127, 193, 173};

            for (int i = 0; i < T; i++)
                L[i] = inputKey[i];

            for (int i = T; i < 128; i++)
                L[i] = pitable[L[i - 1] + L[i - T]];

            L[128 - T8] = pitable[L[128 - T8] & TM];

            for (int i = 128 - T8; i >= 0; i--)
                L[i] = pitable[L[i + 1] ^ L[i + T8]];

            K = ConvertToUshort(L);

            return K;
        }

        private ushort[] ConvertToUshort(byte[] input)
        {
            ushort[] output = new ushort[input.Length / 2];

            for (int i = 0; i < output.Length; i++)
                output[i] = BitConverter.ToUInt16(new byte[2] { input[i * 2], input[i * 2 + 1] }, 0);

            return output;
        }

        private byte[] ConvertToByte(ushort[] input)
        {
            byte[] output = new byte[input.Length * 2];

            for (int i = 0; i < input.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(input[i]);
                output[i * 2] = temp[0];
                output[i * 2 + 1] = temp[1];
            }

            return output;
        }

        private void MixingRound()
        {
            MixUpR(0);
            MixUpR(1);
            MixUpR(2);
            MixUpR(3);
        }

        private void RMixingRound()
        {
            RMixUpR(3);
            RMixUpR(2);
            RMixUpR(1);
            RMixUpR(0);
        }

        private void MashingRound()
        {
            MashR(0);
            MashR(1);
            MashR(2);
            MashR(3);
        }

        private void RMashingRound()
        {
            RMashR(3);
            RMashR(2);
            RMashR(1);
            RMashR(0);
        }

        private void MixUpR(int i)
        {
            int[] s = new int[] { 1, 2, 3, 5 };
            R[i] = (ushort)(R[i] + K[j] + (R[I(i - 1)] & R[I(i - 2)]) + ((~R[I(i + 1)]) & R[I(i - 3)]));
            j++;
            R[i] = RoL(R[i], s[i]);
        }

        private void RMixUpR(int i)
        {
            int[] s = new int[] { 1, 2, 3, 5 };
            R[i] = RoR(R[i], s[i]);
            R[i] = (ushort)(R[i] - K[j] - (R[I(i - 1)] & R[I(i - 2)]) - ((~R[I(i - 1)]) & R[I(i - 3)]));
            j--;
        }

        private void MashR(int i)
        {
            R[i] = (ushort)(R[i] + K[R[I(i - 1)] & 63]);
        }

        private void RMashR(int i)
        {
            R[i] = (ushort)(R[i] - K[R[I(i - 1)] & 63]);
        }

        private int I(int i)
        {
            return (i + 4 * Math.Abs(i)) % 4;
        }

        private ushort RoL(ushort R, int s)
        {
            ushort[] r = new ushort[] { R };
            byte[] b = ConvertToByte(r);
            BitArray bitArray = new BitArray(b);
            BitArray output = bitArray;
            for (int i = 0; i < 16; i++)
            {
                if (i < s)
                    output[i] = bitArray[i + 16 - s];
                else
                    output[i] = bitArray[i - s];
            }
            byte[] bytes = new byte[2];
            output.CopyTo(bytes, 0);
            ushort[] res = ConvertToUshort(bytes);
            return res[0];
        }

        private ushort RoR(ushort R, int s)
        {
            ushort[] r = new ushort[] { R };
            byte[] b = ConvertToByte(r);
            BitArray bitArray = new BitArray(b);
            BitArray output = bitArray;
            for (int i = 0; i < 16; i++)
            {
                if (i > 16 - s)
                    output[i] = bitArray[i - 16 - s];
                else
                    output[i] = bitArray[i + s];
            }
            byte[] bytes = new byte[2];
            output.CopyTo(bytes, 0);
            ushort[] res = ConvertToUshort(bytes);
            return res[0];
        }
    }
}
