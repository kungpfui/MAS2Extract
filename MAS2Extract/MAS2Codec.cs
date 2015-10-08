namespace MAS2Extract
{
    /// <summary>
    /// ISI introduced there own compression algorithm for MAS2-filetype 0x1000x.
    /// At the moment the encoder is missing ... but it is not difficult to write one.
    ///
    /// This code was reversed engineered by reviewing encoded and decoded files.
    /// It was not so diffult to guess what the additional bytes do and the table
    /// could be created by a lot of try and error cycles.
    /// </summary>
    public class MAS2Codec
    {
        /// <summary>
        /// table with character-length and 256-byte page offset values
        /// </summary>
        static public uint[,] IndexTable = new uint[256-0x20,2] {
            { 4, 0},{ 4, 1},{ 4, 2},{ 4, 3},{ 4, 4},{ 4, 5},{ 4, 6},{ 4, 7},{ 4, 8},{ 4, 9},{ 4,10},{ 4,11},{ 4,12},{ 4,13},{ 4,14},{ 4,15},
            { 5, 0},{ 5, 1},{ 5, 2},{ 5, 3},{ 5, 4},{ 5, 5},{ 5, 6},{ 5, 7},{ 5, 8},{ 5, 9},{ 5,10},{ 5,11},{ 5,12},{ 5,13},{ 5,14},{ 5,15},
            { 6, 0},{ 6, 1},{ 6, 2},{ 6, 3},{ 6, 4},{ 6, 5},{ 6, 6},{ 6, 7},{ 6, 8},{ 6, 9},{ 6,10},{ 6,11},{ 6,12},{ 6,13},{ 6,14},{ 6,15},
            { 7, 0},{ 7, 1},{ 7, 2},{ 7, 3},{ 7, 4},{ 7, 5},{ 7, 6},{ 7, 7},{ 7, 8},{ 7, 9},{ 7,10},        { 7,12},{ 7,13},{ 7,14},
		    { 8, 0},{ 8, 1},{ 8, 2},{ 8, 3},{ 8, 4},{ 8, 5},{ 8, 6},{ 8, 7},        { 8, 9},        { 8,11},{ 8,12},                { 8,15},
            { 9, 0},{ 9, 1},{ 9, 2},{ 9, 3},{ 9, 4},{ 9, 5},{ 9, 6},{ 9, 7},{ 9, 8},        { 9,10},                { 9,13},
            {10, 0},{10, 1},{10, 2},{10, 3},{10, 4},{10, 5},{10, 6},{10, 7},{10, 8},{10, 9},        {10,11},                {10,14},{10,15},
            {11, 0},{11, 1},{11, 2},{11, 3},{11, 4},{11, 5},                                {11,10},        {11,12},
            {12, 0},{12, 1},{12, 2},{12, 3},{12, 4},{12, 5},{12, 6},{12, 7},{12, 8},{12, 9},        {12,11},        {12,13},
            {13, 0},{13, 1},{13, 2},{13, 3},{13, 4},                                                                        {13,14},
            {14, 0},{14, 1},{14, 2},                {14, 5},{14, 6},                        {14,10},        {14,12},
			{15, 0},{15, 1},{15, 2},{15, 3},{15, 4},                {15, 7},{15, 8},{15, 9},                                        {15,15},
            {16, 0},{16, 1},{16, 2},{16, 3},{16, 4},{16, 5},{16, 6},                                {16,11},        {16,13},
            {17, 0},{17, 1},{17, 2},
            {18, 0},{18, 1},{18, 2},{18, 3},{18, 4},{18, 5},        {18, 7},{18, 8},{18, 9},{18,10},                        {18,14},
            {19, 0},{19, 1},        {19, 3},                {19, 6},                                        {19,12},
            {20, 0},{20, 1},{20, 2},
            {21, 0},{21, 1},{21, 2},{21, 3},{21, 4},{21, 5},        {21, 7},                        {21,11},                        {21,15},
            {22, 0},
            {23, 0},{23, 1},{23, 2},                        {23, 6},        {23, 8},        {23,10},                {23,13},
            {24, 0},{24, 1},        {24, 3},{24, 4},                                {24, 9},
            {25, 0},{25, 1},{25, 2},                {25, 5},
		    {26, 0},{26, 1},        {26, 3},                        {26, 7},                                {26,12},
            {27, 0},        {27, 2},        {27, 4},        {27, 6},                                                         {27,14},
		    {28, 0},{28, 1},
            {29, 0},{29, 1},{29, 2},{29, 3},        {29, 5},                {29, 8},                {29,11},
		    {30, 0},
            {31, 0},{31, 1},                {31, 4},                                {31, 9},{31,10},
            {32, 0},        {32, 2}
        };


        /// <summary>
        /// The Decoder
        /// </summary>
        /// <param name="data">byte string</param>
        /// <param name="uncompressedSize">length to check at the end</param>
        static public void Decode(byte[] data, byte[] rawData)
        {
		    // count the number of plain characters to read
            uint n = 0;
            // number ob charachters written into @text
            uint textPos = 0;
		    byte tblIndex = 0;

            foreach (var c in data)
            {
                if (n != 0)
                {
                    // just read this byte, it's a plain character
                    rawData[textPos++] = c;
                    n -= 1;
                }
                else
                {
                    if (tblIndex != 0)
                    {
                        uint length = IndexTable[tblIndex - 0x20, 0];
                        uint pageOffset = IndexTable[tblIndex - 0x20, 1];

                        uint textIdx = textPos - (pageOffset * 256 + (uint)c);
                        // copy n(length) bytes
                        for (uint i = textIdx; i < textIdx + (uint)length; i++)
                        {
                            rawData[textPos++] = rawData[i];
                        }
                        // reset table index
                        tblIndex = 0;
                    }
                    else if (c < 0x20)
                    {
                        // just read n (plain) bytes
                        n = (uint)c + 1;
                    }
                    else
                    {
                        // thats a table index, remember it for next time
                        tblIndex = c;
                    }
                }
            }
            if (textPos != rawData.Length)
            {
                throw new System.Exception(string.Format("Uncompressed size does not match ({0}!={1})", textPos, rawData.Length));
            }
        }
    }
}
