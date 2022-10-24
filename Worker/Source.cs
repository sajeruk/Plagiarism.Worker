using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiarism.Worker
{
    public class Source
    {
        private byte[] Content;
        private string Language;

        public Source(string content, string language)
        {
            Content = Encoding.UTF8.GetBytes(content);
            Language = language;
        }

        public Source(byte[] content, string language)
        {
            Content = content;
            Language = language;
        }

        public byte[] GetRawBytesNullTerminated()
        {
            byte[] result = Content;
            Array.Resize(ref result, result.Length + 1);
            return result;
        }

        public string GetLanguage()
        {
            return Language;
        }
    }
}
