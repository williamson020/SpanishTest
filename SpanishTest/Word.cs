using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishTest
{
    class Word
    {

        public enum Classification {  Unknown, VerbInfinitive, VerbConjugation, Noun, NounMasc, NounFem, Adverb, Adjective}
        public string Spanish { get; private set; }
        public string English { get; private set; }

        public Classification Classfic { get; private set; }

        public Verb AssociatedVerb { get; private set; }
        //statics
        public static  Dictionary<string, Word> SpanishDict = new Dictionary<string, Word>();

        public override string ToString()
        {
            if (Classfic != Classification.Unknown)
                return String.Format("{0} {1} : {2}", Spanish, FormatClassification, English);

            return String.Format("{0}: {1}", Spanish, English);
        }

        private string ToFileFormatString()
        {
            if (Classfic != Classification.Unknown)
                return String.Format("{0}~{1}{2}", English, Spanish, FormatClassification);

            return String.Format("{0}~{1}", English,Spanish);
        }

        public string FormatClassification
        {
            get
            {
                string x = "classification";

                switch (Classfic)
                {
                    case Classification.Unknown:
                        x = "(?)";
                        break;
                    case Classification.VerbInfinitive:
                        x = "(vi)";
                        break;
                    case Classification.VerbConjugation:
                        x = "(conj)";
                        break;
                    case Classification.Noun:
                        x = "(n)";
                        break;
                    case Classification.NounMasc:
                        x = "(nm)";
                        break;
                    case Classification.NounFem:
                        x = "(nf)";
                        break;
                    case Classification.Adverb:
                        x = "(adv)";
                        break;
                    case Classification.Adjective:
                        x = "(adj)";
                        break;
                }

                return x;
            }
        }

        private Word(string es, string en, Classification c = Classification.Unknown, Verb av = null)
        {
            if (String.IsNullOrEmpty(es))
                throw new ArgumentNullException("NULL SPANISH KEY");

            if (String.IsNullOrEmpty(en))
                throw new ArgumentNullException("NULL ENGLISH DEFN");

            Spanish = es;
            English = en;

            Classfic = c;
            AssociatedVerb = av;
        }


        internal static void Factory(Verb v)
        {
            SpanishDict[v.Infinitive] = new Word(v.Infinitive, v.Translation, Classification.VerbInfinitive, v);
        }

        internal static void Factory(string line, bool reverseOrder=false, bool save=false, string savefolder=null)
        {
            //Default order is ES~EN

            if (line.IndexOf("~") < 1)
                throw new ArgumentException("ArgumentException in Word.Factory");

            string[] parts = line.Split('~');

            string es = reverseOrder ? parts[1] : parts[0];
            string en = reverseOrder ? parts[0] : parts[1];


            //Scan for classification. Can appear in either side but not both
            string cleanstr = null;
            Classification c= ParseForClassification(es, out cleanstr);

            if (c != Classification.Unknown)
            {
                if (cleanstr != null)
                    es = cleanstr;
            }
            else
            {
                //try english side
                c = ParseForClassification(en, out cleanstr);
                if (c != Classification.Unknown)
                {
                    if (cleanstr != null)
                        en = cleanstr;
                }
            }

            var w = new Word(es.Trim(), en.Trim(), c);

            SpanishDict[es] = w;

            if (save)
                Save(w, savefolder);

        }

        private static void Save(Word w, string path)
        {
            string p = Path.Combine(path, "VOCAB - AUTOSAVE.txt");
            File.AppendAllText(p, Environment.NewLine + w.ToFileFormatString());
        }

        private static Classification ParseForClassification(string str, out string cleanstr)
        {
            // (adj) (adv) (v) (nm) (nf)

            cleanstr = null;
            str = str.ToLower();

            int i = str.IndexOf("(");
            int j = str.IndexOf(")");

            if ((j-i > 1)  && (i>0))
            {
                string wordtype = str.Substring(i + 1, j - i - 1);

                if (wordtype == "adj")
                {
                    cleanstr = str.Replace("(adj)", "");
                    return Classification.Adjective;
                }

                if (wordtype == "adv")
                {
                    cleanstr = str.Replace("(adv)", "");
                    return Classification.Adverb;
                }

                if (wordtype == "v")
                {
                    cleanstr = str.Replace("(v)", "");
                    return Classification.VerbInfinitive;
                }

                if (wordtype == "n")
                {
                    cleanstr = str.Replace("(n)", "");
                    return Classification.Noun;
                }

                if (wordtype == "nf")
                {
                    cleanstr = str.Replace("(nf)", "");
                    return Classification.NounFem;
                }

                if (wordtype == "nm")
                {
                    cleanstr = str.Replace("(nm)", "");
                    return Classification.NounMasc;
                }

            }

            return Classification.Unknown;

        }
    }
}
