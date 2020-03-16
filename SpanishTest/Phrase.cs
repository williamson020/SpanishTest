using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishTest
{
    public class Phrase
    {
        //All phrases from files and verbs
        private static List<Phrase> _allPhrases = new List<Phrase>();

        //Maps phrases from files to that collection
        private static Dictionary<string/*filename*/, List<Phrase>> _phraseSourceMap = new Dictionary<string, List<Phrase>>();


        public string English { get; private set; }
        public string Spanish { get; private set; }
        public string Notes { get; private set; }


        public VerbDefinition AssociatedVerb { get; private set; }

        private Phrase(string EN, string ES, VerbDefinition av = null, string notes = null, string filesource=null)
        {
            if (String.IsNullOrEmpty(EN))
                throw new ArgumentException("English segment of phrase is null");

            if (String.IsNullOrEmpty(ES))
                throw new ArgumentException("Spanish segment of phrase is null");

            English = EN; 
            Spanish = ES;
            AssociatedVerb = av;
            Notes = notes;

            //look for additional EN notes
            int i = English.IndexOf("{");
            int j = English.IndexOf("}");
            if (i>=0 && j>0 && j>i)
            {
                Notes+= ">>" + English.Substring(i + 1, j -1- i);
                //Remove from English segment
                English = EN.Substring(0, i);
            }
        }


        public string GetText(TestManager.Language lang)
        {
            return lang == TestManager.Language.ES ? Spanish : English;
        }

        //Expeced SPANISH~ENGLISH
        internal static Phrase Factory(string p, bool reverseOrder, string defn, VerbDefinition associatedVerb=null, string filesource=null)
        {

            p=p.TrimStart(' ');
            p = p.TrimEnd(' ');

            int f = p.IndexOf("~");

            string es = p.Substring(0, f);
            string en = p.Substring(f+1);

            Phrase ph = reverseOrder ? new Phrase(es, en,  associatedVerb, associatedVerb != null ? associatedVerb.ToString() : defn) :
                new Phrase(en, es,  associatedVerb, associatedVerb != null ? associatedVerb.ToString() : defn);

            _allPhrases.Add(ph);

            if (!String.IsNullOrEmpty(filesource))
            {
                if (!_phraseSourceMap.ContainsKey(filesource))
                    _phraseSourceMap[filesource] = new List<Phrase>();

                _phraseSourceMap[filesource].Add(ph);
            }

            
           

            return ph;
        }

        internal static string[] GetFilesources()
        {
            return _phraseSourceMap.Keys.ToArray();
        }

        internal static List<Phrase> FromFileSource(string src)
        {
            if (!_phraseSourceMap.ContainsKey(src))
                throw new ArgumentException("UNKNOWN FILE SRC " + src);

            return _phraseSourceMap[src];

        }
        internal static bool IsPhrase(string p)
        {
            if (p == null)
                return false;

            return p.IndexOf('~') > 1;
        }

        public override string ToString()
        {
            return String.Format("{0} >> {1}", Spanish, English );
        }
    }
}
