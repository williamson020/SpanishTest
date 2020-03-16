using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SpanishTest
{
    public class TextLoader
    {
        private List<string> Lines = new List<string>();

        public string Peek()
        {
            if (Lines.Count == 0)
                return null;
            
            return Lines[0];

        }

        public string Next()
        {
            if (Lines.Count == 0)
                return null;

            string ret = Lines[0];
            Lines.RemoveAt(0);
            return ret;

        }


        public TextLoader(string path, bool defaultencoding=false)
        {
            string line;

            StreamReader f = defaultencoding ? new StreamReader(path, Encoding.Default) : new StreamReader(path, Encoding.UTF8);
            do
            {
                line = f.ReadLine();
                if (!String.IsNullOrEmpty(line))
                {
                    Lines.Add(line);
                    //Debug.WriteLine(line);
                }
            } 
            while (line != null);
        }


        internal void Advance()
        {
            var used = this.Next();
        }
    }

    public class TextLoaderDefaultEncoding : TextLoader
    {
        public TextLoaderDefaultEncoding(string path): base(path, true)
        {

        }
    }
}
