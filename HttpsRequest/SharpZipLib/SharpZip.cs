using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

namespace HttpsRequest.SharpZipLib
{
    public static class SharpZip
    {
        public static string StringToStringZip(string str)
        {
            string largeCompressedText = string.Empty;
            using (MemoryStream source = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                using (MemoryStream target = new MemoryStream())
                {
                    BZip2.Compress(source, target, true, 3);
                    byte[] targetByteArray = target.ToArray();
                    largeCompressedText = Convert.ToBase64String(targetByteArray);
                }
            }

            return largeCompressedText;
        }

        public static string StringZipToString(string str)
        {
            byte[] largeCompressedTextAsBytes = Convert.FromBase64String(str);
            string uncompressedString = string.Empty;
            using (MemoryStream source = new MemoryStream(largeCompressedTextAsBytes))
            {
                using (MemoryStream target = new MemoryStream())
                {
                    BZip2.Decompress(source, target, true);
                    uncompressedString = Encoding.UTF8.GetString(target.ToArray());
                }
            }

            return uncompressedString;
        }
    }
}
