using System;

sealed class MD5
{
    protected readonly static uint[] T = new uint[] 
		{	
            0xd76aa478,0xe8c7b756,0x242070db,0xc1bdceee,
			0xf57c0faf,0x4787c62a,0xa8304613,0xfd469501,
            0x698098d8,0x8b44f7af,0xffff5bb1,0x895cd7be,
            0x6b901122,0xfd987193,0xa679438e,0x49b40821,
			0xf61e2562,0xc040b340,0x265e5a51,0xe9b6c7aa,
            0xd62f105d,0x2441453,0xd8a1e681,0xe7d3fbc8,
            0x21e1cde6,0xc33707d6,0xf4d50d87,0x455a14ed,
			0xa9e3e905,0xfcefa3f8,0x676f02d9,0x8d2a4c8a,
            0xfffa3942,0x8771f681,0x6d9d6122,0xfde5380c,
            0xa4beea44,0x4bdecfa9,0xf6bb4b60,0xbebfbc70,
            0x289b7ec6,0xeaa127fa,0xd4ef3085,0x4881d05,
			0xd9d4d039,0xe6db99e5,0x1fa27cf8,0xc4ac5665,
            0xf4292244,0x432aff97,0xab9423a7,0xfc93a039,
            0x655b59c3,0x8f0ccc92,0xffeff47d,0x85845dd1,
            0x6fa87e4f,0xfe2ce6e0,0xa3014314,0x4e0811a1,
			0xf7537e82,0xbd3af235,0x2ad7d2bb,0xeb86d391
        };

    // Zie Wikipedia
    protected readonly static int[] s = new int[]
    {
        7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
        5,  9, 14, 20, 5,  9, 14, 20, 5,  9, 14, 20, 5,  9, 14, 20, 
        4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 
        6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21
    };


    // Standaard waarden
    uint A = 0x67452301;
    uint B = 0xEFCDAB89;
    uint C = 0x98BADCFE;
    uint D = 0X10325476;

    protected uint[] X = new uint[16];
    private readonly byte[] m_byteInput;

    public MD5(string s)
    {
        m_byteInput = new byte[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            m_byteInput[i] = (byte)s[i];
        }

    }

    public string createMD5()
    {
        byte[] paddedBuffer = PadBuffer();

        uint N = (uint)(paddedBuffer.Length * 8) / 32;

        for (uint i = 0; i < N / 16; i++)
        {
            MakeBlock(paddedBuffer, i);
            CalculateTransform();
        }

        return  ReverseByte(A).ToString("X8") +
                ReverseByte(B).ToString("X8") +
                ReverseByte(C).ToString("X8") +
                ReverseByte(D).ToString("X8") ;

    }

    private static uint ReverseByte(uint uiNumber)
    {
        return (((uiNumber & 0x000000ff) << 24) |
                (  uiNumber               >> 24) |
                (( uiNumber & 0x00ff0000) >> 8)  |
                (( uiNumber & 0x0000ff00) << 8)) ;
    }

    private void CalculateTransform()
    {
        uint AA = A;
        uint BB = B;
        uint CC = C;
        uint DD = D;
        uint F = 0;
        int g = 0;

        for (int i = 0; i < 64; i++)
        {
            if (i <= 15)
            {
                F = ((BB & CC) | (~(BB) & DD));
                g = i;
            }
            else if (i <= 31)
            {
                F = ((BB & DD) | (CC & ~DD));
                g = (5 * i + 1) % 16;
            }
            else if (i <= 47)
            {
                F = (BB ^ CC ^ DD);
                g = (3 * i + 5) % 16;
            }
            else if (i <= 63)
            {
                F = (CC ^ (BB | ~DD));
                g = (7 * i) % 16;
            }
            uint currentDD = DD;
            DD = CC;
            CC = BB;
            uint calculatedValue = AA + F + T[i] + X[g];
            calculatedValue = (calculatedValue << s[i]) | (calculatedValue >> (32 - s[i]));
            BB = BB + calculatedValue;
            AA = currentDD;
        }

        A += AA;
        B += BB;
        C += CC;
        D += DD;
    }

    protected void MakeBlock(byte[] bMsg, uint block)
    {
        block = block << 6;
        for (uint i = 0; i < 61; i += 4)
        {
            X[i>>2] = (((uint)bMsg[block+(i+3)]) << 24) |
                        (((uint)bMsg[block+(i+2)]) << 16) |
                        (((uint)bMsg[block+(i+1)]) << 8)  |
                        (((uint)bMsg[block+(i)] ))        ;
        }
    }
    private byte[] PadBuffer()
    {

        uint pad = (uint)(((448 - ((m_byteInput.Length * 8) % 512)) + 512) % 512);

        if (pad == 0)
        {
            pad = 512;
        }

        uint bufferSize = (uint)((m_byteInput.Length) + (pad / 8) + 8);
        ulong messageSize = (ulong)m_byteInput.Length * 8;

        byte[] paddedBuffer = new byte[bufferSize];

        Array.Copy(m_byteInput, paddedBuffer, m_byteInput.Length);

        paddedBuffer[m_byteInput.Length] |= 0x80;

        for (int i = 8; i > 0; i--)
        {
            paddedBuffer[bufferSize - i] = (byte)(messageSize >> ((8 - i) * 8) & 0xff);
        }

        return paddedBuffer;

    }
}

