using System;
using System.IO;

namespace Penguin.IO.Objects
{
    /// <summary>
    /// A stream reader that buffers and chunks out the source file to prevent slowdowns when requesting single characters
    /// </summary>
    [ObsoleteAttribute("This doesn't need to exist")]
    public class BufferedFileReader : StreamReader
    {
        /// <summary>
        /// True if the base reader has reached the end of the stream, and the reader has reached the end of its buffer
        /// </summary>
        public new bool EndOfStream => this.BufferPointer == this.Buffer.Length && base.EndOfStream;

        /// <summary>
        /// A decimal representing the % of the way the pointer is through the base stream AND buffer
        /// </summary>
        public decimal Progress
        {
            get
            {
                long Position = base.BaseStream.Position - (this.Buffer.Length - this.BufferPointer);

                return (Position / (decimal)this.BaseStream.Length) * 100;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path of the file to read</param>
        /// <param name="BufferSize">The number of characters of the file to buffer into memory</param>
        public BufferedFileReader(string path, long BufferSize = 20000000) : base(path)
        {
            this.Buffer = new char[BufferSize];
            this.FillBuffer();
        }

        /// <summary>
        /// Reads the next int from the buffer and if necessary, refills it
        /// </summary>
        /// <returns>The next int from the buffer</returns>
        public override int Read()
        {
            if (this.BufferPointer == this.Buffer.Length)
            {
                this.FillBuffer();
            }

            return this.Buffer[this.BufferPointer++];
        }

        private char[] Buffer;
        private long BufferPointer = 0;

        private void FillBuffer()
        {
            this.BufferPointer = 0;

            long ReadLength = this.Buffer.Length;

            if (base.BaseStream.Position + (ReadLength * sizeof(char)) > this.BaseStream.Length)
            {
                ReadLength = base.BaseStream.Length - base.BaseStream.Position;
                this.Buffer = new char[ReadLength];
            }

            base.Read(this.Buffer, 0, (int)ReadLength);
        }
    }
}