using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiarism.Worker
{
    public class Source
    {
        private byte[] Content;

        public Source(string content)
        {
            Content = Encoding.UTF8.GetBytes(content);
        }

        public Source(byte[] content)
        {
            Content = content;
        }

        public byte[] GetRawBytesNullTerminated()
        {
            byte[] result = Content;
            Array.Resize(ref result, result.Length + 1);
            return result;
        }
    }
}
