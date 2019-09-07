using System.IO;
using System.IO.Compression;
using System.Text;

namespace Penguin.IO.Compression
{
    /// <summary>
    /// Simplifys data compression using GZip
    /// </summary>
    public static class GZip
    {
        /// <summary>
        /// Compresses the input string
        /// </summary>
        /// <param name="source">The string to compress</param>
        /// <returns>A GZipped byte array representing the string</returns>
        public static byte[] Compress(string source)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(source);

            using (MemoryStream msi = new MemoryStream(bytes))
            {
                using (MemoryStream mso = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        //msi.CopyTo(gs);
                        CopyTo(msi, gs);
                    }

                    return mso.ToArray();
                }
            }
        }

        /// <summary>
        /// Copies from one stream to another
        /// </summary>
        /// <param name="src">The source stream</param>
        /// <param name="dest">The destination stream</param>
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Decompresses the provided byte array into its source string representation
        /// </summary>
        /// <param name="source">The byte array to decompress</param>
        /// <returns>The original string before it was compressed</returns>
        public static string Decompress(byte[] source)
        {
            using (MemoryStream msi = new MemoryStream(source))
            {
                using (MemoryStream mso = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        //gs.CopyTo(mso);
                        CopyTo(gs, mso);
                    }

                    return Encoding.UTF8.GetString(mso.ToArray());
                }
            }
        }
    }
}