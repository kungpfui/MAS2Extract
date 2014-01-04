namespace MAS2Extract
{
    public class MAS2File
    {
        /// <summary>
        /// The index the file has inside the mother MAS2 file.
        /// </summary>
        public uint FileIndex { get; private set; }

        /// <summary>
        /// The file type (a number) as set by ISI.
        /// 0  = zero sized file
        /// 16 = .VEH, .AIW, .CAM, .GDB, .TDF, ... (text files?)
        /// 17 = .GMT
        /// 18 = ???
        /// 19 = .SCN
        /// 20 = .TGA
        /// 21 = ???
        /// 22 = .JPG
        /// 23 = .DDS                                       
        /// </summary>
        public uint FileType { get; private set; }

        /// <summary>
        /// The name of this file.
        /// </summary>
        public string Filename { get; private set; }        

        /// <summary>
        /// Is this file compressed?
        /// Is TRUE when compressed & uncompressed are equal.
        /// </summary>
        public bool IsCompressed { get { return (CompressedSize != UncompressedSize); } }
        
        /// <summary>
        /// The compressed size (raw) inside the MAS2 file.
        /// </summary>
        public ulong CompressedSize { get; private set; }

        /// <summary>
        /// The uncompressed size outside the MAS2 file.
        /// </summary>
        public ulong UncompressedSize { get; private set; }

        /// <summary>
        /// This is the file offset in the MAS2 file itself. It is used by the 
        /// reader for tracking down where to start reading raw file data.
        /// </summary>
        public ulong FileOffset { get; private set; }

        public MAS2File(uint fileindex, uint filetype, string filename, ulong compressedSize, ulong uncompressedSize, ulong fileOffset)
        {
            FileIndex = FileIndex;
            FileType = filetype;
            Filename = filename;
            CompressedSize = compressedSize;
            UncompressedSize = uncompressedSize;
            FileOffset = fileOffset;
        }
    }
}