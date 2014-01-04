namespace MAS2Extract
{
    public class MAS2File
    {
        /// <summary>
        /// The index the file has inside the mother MAS2 file.
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

        public MAS2File(uint filetype, string filename, ulong compressedSize, ulong uncompressedSize, ulong fileOffset)
        {
            FileType = filetype;
            Filename = filename;
            CompressedSize = compressedSize;
            UncompressedSize = uncompressedSize;
            FileOffset = fileOffset;
        }
    }
}