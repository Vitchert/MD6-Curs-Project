using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MD6Project
{
    class MD6
    {
        public const int OK = 0;
        public const int NO_FILE = 1;
        public const int WRONG_FILE_SIZE = 2;

        public UInt64[] Message;
        private UInt64 Messagelength;
        private UInt64[] Key;
        uint keylen;
        uint d;
        public uint L;
        uint r;


        public const int bw = 64;//words
        public const int cw = 16;//words        
        private UInt64[] Q = { 0x7311c2812425cfa0
                        ,0x6432286434aac8e7
                        ,0xb60450e9ef68b7c1
                        ,0xe8fb23908d9f06f1
                        ,0xdd2e76cba691e5bf
                        ,0x0cd0d63b2c30bc41
                        ,0x1f8ccf6823058f8a
                        ,0x54e5ed5b88e3775d
                        ,0x4ad12aae0a6d6031
                        ,0x3e7f16bb88222e0d
                        ,0x8af8671d3fb50c2c
                        ,0x995ad1178bd25c31
                        ,0xc878c1dd04c4b633
                        ,0x3b72066c7a1552ac
                        ,0x0d6f3522631effcb }; // 960 bits of √6 as a sequence of 15 64-bit words


        public MD6(uint dVal = 256, uint LVal = 64, uint rVal = 0)
        {
            d = dVal;
            L = LVal;
            r = rVal;
        }

        public int readMessageFile(string filePath)
        {
            FileInfo fileInf = new FileInfo(filePath);
            if (fileInf.Exists)
            {
                Messagelength = (UInt64)fileInf.Length;
                Message = new UInt64[Messagelength / 8 + (UInt64)(Messagelength % 8 > 0 ? 1 : 0)];
                Messagelength *= 8; //from bytes to bits
                using (FileStream fs = fileInf.OpenRead())
                {
                    UInt64 readCount = (UInt64)Message.Length;
                    byte[] buf = new byte[8];

                    for (UInt64 k = 0; k < readCount; ++k)
                    {
                        fs.Read(buf, 0, buf.Length);
                        if (BitConverter.IsLittleEndian)
                        {
                            Message[k] |= (UInt64)buf[0] << 56;
                            Message[k] |= (UInt64)buf[1] << 48;
                            Message[k] |= (UInt64)buf[2] << 40;
                            Message[k] |= (UInt64)buf[3] << 32;
                            Message[k] |= (UInt64)buf[4] << 24;
                            Message[k] |= (UInt64)buf[5] << 16;
                            Message[k] |= (UInt64)buf[6] << 8;
                            Message[k] |= (UInt64)buf[7];
                        }
                        else
                        {
                            Message[k] = BitConverter.ToUInt64(buf, 0);
                        }
                        Array.Clear(buf, 0, buf.Length);
                    }
                }
            }
            else
                return (NO_FILE);

            return (OK);
        }

        public void readMessageString(string str)
        {
            Messagelength = (UInt64)str.Length;
            Message = new UInt64[Messagelength / 8 + (UInt64)(Messagelength % 8 > 0 ? 1 : 0)];

            UInt64 readCount = (UInt64)Message.Length;
            byte[] buf = new byte[8];

            byte[] stringArray = Encoding.ASCII.GetBytes(str);

            for (UInt64 k = 0; k < readCount; ++k)
            {
                int copySize = k < (readCount - 1) ? 8 : (int)Messagelength%8;
                Array.Copy(stringArray,(int)k*8,buf,0, copySize);
                if (BitConverter.IsLittleEndian)
                {
                    Message[k] |= (UInt64)buf[0] << 56;
                    Message[k] |= (UInt64)buf[1] << 48;
                    Message[k] |= (UInt64)buf[2] << 40;
                    Message[k] |= (UInt64)buf[3] << 32;
                    Message[k] |= (UInt64)buf[4] << 24;
                    Message[k] |= (UInt64)buf[5] << 16;
                    Message[k] |= (UInt64)buf[6] << 8;
                    Message[k] |= (UInt64)buf[7];
                }
                else
                {
                    Message[k] = BitConverter.ToUInt64(buf, 0);
                }
                Array.Clear(buf, 0, buf.Length);
            }
            Messagelength *= 8;//from bytes to bits
        }

        public int readKeyFile(string filePath)
        {
            FileInfo fileInf = new FileInfo(filePath);
            if (fileInf.Exists)
            {
                UInt64 Testlength = (UInt64)fileInf.Length;
                if(Testlength > 512)
                {
                    return(WRONG_FILE_SIZE);
                }
                keylen = (uint)Testlength;
                Key = new UInt64[8];
                using (FileStream fs = fileInf.OpenRead())
                {
                    UInt64 readCount = (UInt64)(keylen / 8 + ((keylen % 8 > 0) ? 1 : 0));
                    byte[] buf = new byte[8];

                    for (UInt64 k = 0; k < readCount; ++k)
                    {
                        fs.Read(buf, 0, buf.Length);
                        if (BitConverter.IsLittleEndian)
                        {
                            Key[k] |= (UInt64)buf[0] << 56;
                            Key[k] |= (UInt64)buf[1] << 48;
                            Key[k] |= (UInt64)buf[2] << 40;
                            Key[k] |= (UInt64)buf[3] << 32;
                            Key[k] |= (UInt64)buf[4] << 24;
                            Key[k] |= (UInt64)buf[5] << 16;
                            Key[k] |= (UInt64)buf[6] << 8;
                            Key[k] |= (UInt64)buf[7];
                        }
                        else
                        {
                            Message[k] = BitConverter.ToUInt64(buf, 0);
                        }
                        Array.Clear(buf, 0, buf.Length);
                    }
                }
                if (r == 0)
                {
                    r = 40 + d / 4;
                    if ((keylen != 0) && (r < 80))
                    {
                        r = 80;
                    }
                }
            }
            else
                return (NO_FILE);

            return (OK);
        }

        public int readKeyString(string str)
        {
            keylen = (uint)str.Length;
            if (keylen > 512)
            {
                return (WRONG_FILE_SIZE);
            }
            Key = new UInt64[8];

            UInt64 readCount = (UInt64)(keylen/8 + ( (keylen%8 > 0) ? 1:0));
            byte[] buf = new byte[8];

            byte[] stringArray = Encoding.ASCII.GetBytes(str);

            for (UInt64 k = 0; k < readCount; ++k)
            {
                int copySize = k < (readCount - 1) ? 8 : (int)keylen % 8;
                Array.Copy(stringArray, (int)k * 8, buf, 0, copySize);
                if (BitConverter.IsLittleEndian)
                {
                    Key[k] |= (UInt64)buf[0] << 56;
                    Key[k] |= (UInt64)buf[1] << 48;
                    Key[k] |= (UInt64)buf[2] << 40;
                    Key[k] |= (UInt64)buf[3] << 32;
                    Key[k] |= (UInt64)buf[4] << 24;
                    Key[k] |= (UInt64)buf[5] << 16;
                    Key[k] |= (UInt64)buf[6] << 8;
                    Key[k] |= (UInt64)buf[7];
                }
                else
                {
                    Message[k] = BitConverter.ToUInt64(buf, 0);
                }
                Array.Clear(buf, 0, buf.Length);
            }

            if (r == 0)
            {
                r = 40 + d / 4;
                if ((keylen != 0) && (r < 80))
                {
                    r = 80;
                }
            }
            return (OK);
        }

        public string Hash()
        {
            UInt64[] res = ModeOfOperation();
            byte[] retVal = new byte[(d + 7) / 8];
            int i = 0;
            while (i < retVal.Length)
            {
                UInt64 val = res[res.Length - 1 - i / 8];
                int k = 0;
                while ((i < retVal.Length) && (k < 8))
                {
                    byte mask = 0xFF;
                    retVal[retVal.Length - 1 - i++] = (byte)(val & mask);
                    val = val >> 8;
                    ++k;
                }
            }
            byte bitoffset = (byte)(d % 8);
            if (bitoffset > 0)
            {
                for (int k = 0; k < retVal.Length - 1; ++k)
                {
                    retVal[k] = (byte)((retVal[k] << (8 - bitoffset)) | (retVal[k + 1] >> bitoffset));
                }
                retVal[retVal.Length - 1] = (byte)(retVal[retVal.Length - 1] << (8 - bitoffset));
            }

            StringBuilder sb = new StringBuilder(retVal.Length * 2);
            foreach (byte b in retVal)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            string hashval = sb.ToString();
            if ((d % 8 <5) && (d%8 > 0)) {
                hashval = hashval.Remove(hashval.Length - 1);
            }

            return (hashval);
        }

        private UInt64[] ModeOfOperation()
        {

            uint l = 0;
            while (true)
            {
                ++l;
                if (l == L + 1)
                {
                    return SEQ();
                }
                else
                {
                    Message = PAR(l);
                    if (Message.Length == cw)
                    {
                        return Message;
                    }
                    Messagelength = (UInt64)Message.LongLength * 64;
                    System.GC.Collect();
                }
            }
        }

        private UInt64[] PAR(uint l)
        {
            UInt64 p = (UInt64)Message.LongLength * 64 - Messagelength + ((Message.LongLength % bw) > 0 ? (((UInt64)bw - (UInt64)Message.LongLength % bw) * 64) : 0);

            UInt64 j = (UInt64)(Message.Length / bw) + (UInt64)(Message.LongLength % bw > 0 ? 1 : 0);
            uint z = (uint)(j == 1 ? 1 : 0);
            UInt64 V = 0;
            V |= r;
            V = V << 8;
            V |= L;
            V = V << 4;
            V |= z;
            V = V << 16;
            V |= p;
            V = V << 8;
            V |= keylen;
            V = V << 12;
            V |= d;
            UInt64 noPadding = 0xFFFFFFF0000FFFFF;
            UInt64[] Ci = new UInt64[cw];
            UInt64[] Res = new UInt64[cw * j];
            UInt64[] fVal = new UInt64[n];
            Array.Copy(Q, 0, fVal, 0, Q.Length);
            Array.Copy(Key, 0, fVal, Q.Length, Key.Length);
            for (UInt64 i = 0; i < j; ++i)
            {
                UInt64 localV = V;
                if (i < j - 1)
                    localV &= noPadding;

                UInt64 U = l;
                U = U << 56;
                U |= i;

                fVal[Q.Length + Key.Length] = U;
                fVal[Q.Length + Key.Length + 1] = localV;
                if (i < j - 1)
                {
                    Array.Copy(Message, bw * (Int64)i, fVal, Q.Length + Key.Length + 2, bw);
                }
                else
                {
                    if (Message.LongLength % bw > 0)
                    {
                        Array.Copy(Message, bw * (Int64)i, fVal, Q.Length + Key.Length + 2, Message.LongLength % bw);
                        for (Int64 k = Message.LongLength % bw; k < bw; ++k)
                        {
                            fVal[k + Q.Length + Key.Length + 2] = 0;
                        }
                    }
                    else
                    {
                        Array.Copy(Message, bw * (Int64)i, fVal, Q.Length + Key.Length + 2, bw);
                    }
                }
                //call to compress
                Ci = Compress(ref fVal, r);
                Array.Copy(Ci, 0, Res, cw * (Int64)i, cw);
            }
            return Res;
        }

        private UInt64[] SEQ()
        {
            UInt64 p = (UInt64)Message.LongLength * 64 - Messagelength + ((Message.LongLength % (bw - cw)) > 0 ? (((UInt64)(bw - cw) - (UInt64)Message.LongLength % (bw - cw)) * 64) : 0);

            UInt64 j = (UInt64)(Message.Length / (bw - cw)) + (UInt64)(Message.LongLength % (bw - cw) > 0 ? 1 : 0);

            UInt64 V = 0;
            V |= r;
            V = V << 8;
            V |= L;
            V = V << 4;
            //V |= z;
            V = V << 16;
            V |= p;
            V = V << 8;
            V |= keylen;
            V = V << 12;
            V |= d;

            UInt64 noPadding = 0xFFFFFFF0000FFFFF;
            UInt64 z = 0x0000001000000000;

            UInt64[] C = new UInt64[cw];
            UInt64[] fVal = new UInt64[n];
            Array.Copy(Q, 0, fVal, 0, Q.Length);
            Array.Copy(Key, 0, fVal, Q.Length, Key.Length);
            for (UInt64 i = 0; i < j; ++i)
            {
                UInt64 localV = V;
                if (i < j - 1)
                {
                    localV &= noPadding;
                }
                else
                {
                    localV |= z;
                }
                UInt64 U = (L + 1);
                U = U << 56;
                U |= i;


                fVal[Q.Length + Key.Length] = U;
                fVal[Q.Length + Key.Length + 1] = localV;
                Array.Copy(C, 0, fVal, Q.Length + Key.Length + 2, C.Length);
                if (i < j - 1)
                {
                    Array.Copy(Message, (bw - cw) * (Int64)i, fVal, Q.Length + Key.Length + 2 + C.Length, (bw - cw));
                }
                else
                {
                    if (Message.LongLength % (bw - cw) > 0)
                    {
                        Array.Copy(Message, (bw - cw) * (Int64)i, fVal, Q.Length + Key.Length + 2 + C.Length, Message.LongLength % (bw - cw));
                        for (Int64 k = Message.LongLength % (bw - cw); k < (bw - cw); ++k)
                        {
                            fVal[k + Q.Length + Key.Length + 2 + C.Length] = 0;
                        }
                    }
                    else
                    {
                        Array.Copy(Message, (bw - cw) * (Int64)i, fVal, Q.Length + Key.Length + 2 + C.Length, (bw - cw));
                    }
                }
                //call to compress
                C = Compress(ref fVal, r);
            }
            return C;
        }

        private const uint n = 89;//words

        private const uint t0 = 17;
        private const uint t1 = 18;
        private const uint t2 = 21;
        private const uint t3 = 31;
        private const uint t4 = 67;

        private int[] ri = { 10, 5, 13, 10, 11, 12, 2, 7, 14, 15, 7, 13, 11, 7, 6, 12 };
        private int[] li = { 11, 24, 9, 16, 15, 9, 27, 15, 6, 2, 29, 8, 15, 5, 31, 9 };

        private UInt64 S0 = 0x0123456789abcdef;
        private UInt64 Sdot = 0x7311c2812425cfa0;

        private UInt64[] Compress(ref UInt64[] N, uint r)
        {
            UInt64[] C = new UInt64[cw];

            uint t = r * cw;

            UInt64[] A = new UInt64[t + n];
            Array.Copy(N, 0, A, 0, n);

            UInt64 Si = S0;
            for (uint i = n, j = 0; j < r; ++j)
            {
                for (uint k = 0; k < 16; ++k, ++i)
                {
                    UInt64 x = Si ^ A[i - n] ^ A[i - t0];
                    x ^= (A[i - t1] & A[i - t2]) ^ (A[i - t3] & A[i - t4]);
                    x ^= (x >> ri[(i - n) % 16]);
                    A[i] = x ^ (x << li[(i - n) % 16]);
                }
                Si = (Si << 1 | Si >> 63) ^ (Si & Sdot);
            }
            Array.Copy(A, t + n - cw, C, 0, cw);
            return C;
        }
    }
}
