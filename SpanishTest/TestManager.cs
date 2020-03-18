using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Diagnostics;

namespace SpanishTest
{

    public class TestManager : IEnumerable<TestQuestion>
    {
        private static Random rng = new Random();
        private List<TestQuestion> _questions;

        private MainWindow _main;

        public enum Mode { NamedVerbPhrases, AllVerbPhrases, FileSourcePhrases, RandomPhrases, TranslationSource }

        public Verb SelectedVerb { get; private set; }
        private string _selectedFileSource = null;
        private string _selectedTranslationSource = null;
        private string DataFolder { get; set; }

        private Mode _mode = Mode.AllVerbPhrases;


        public enum Language { EN, ES };

        public Language TestLanguage { get; private set; }

        public MainWindow MainWnd
        {
            get { return _main; }
        }
       
        private IEnumerator<TestQuestion> _iter;


        public TestManager(MainWindow mainWnd, string datafolder)
        {

            _main = mainWnd;
            DataFolder = datafolder;

            TestLanguage = Language.ES;

            //STRUCTURE / <
            //  <TESTFOLDER>\SpanishDict\VerbUsage

            LoadVerbMasterTable(DataFolder + @"\VerbConjugation");

            LoadSpanishDictVerbUsage(DataFolder + @"\VerbUsage");

            LoadVerbConjugation(DataFolder + @"\VerbConjugation");

            LoadPhrases(DataFolder + @"\Phrases");

            LoadVocab(VocabFolder);

            VerbFavourites.Init();

        }

        public string VocabFolder
        {
            get { return DataFolder + @"\Vocab"; }
        }

        private void LoadVocab(string folder)
        {
            foreach (var filepath in Directory.EnumerateFiles(folder, "*.txt", SearchOption.TopDirectoryOnly))
            {
                TextLoader loader = new TextLoader(filepath);
                bool reverseOrder = false, firstLine = true;

                while (loader.Peek() != null)
                {
                    string line = loader.Next();
                    if (String.IsNullOrEmpty(line))
                        continue;

                    Debug.WriteLine("PROCESSING LINE " + line);

                    if (firstLine && line.StartsWith(@"//")) //Process DIRECTIVES IN COMMENTS
                    {
                        //LINE 0 CAN CONTAIN EN~ES or ES~EN(default)
                        if ((line.Contains("ES~EN") || line.Contains("EN~ES")))
                            if (line.ToUpper().IndexOf("EN") < line.ToUpper().IndexOf("ES"))
                                reverseOrder = true;
                    }
                    else
                    {
                        Word.Factory(line, reverseOrder);
                    }
                    firstLine = false;
                }
            }

        }

        private void LoadPhrases(string folder)
        {

            foreach (var filepath in Directory.EnumerateFiles(folder, "*.txt", SearchOption.TopDirectoryOnly))
            {
                TextLoader loader = new TextLoader(filepath);
                bool reverseOrder = false, firstLine = true;
                string defn = null;

                while (loader.Peek() != null)
                {
                    string line = loader.Next();
                    if (String.IsNullOrEmpty(line))
                        continue;

                    Debug.WriteLine("PROCESSING LINE " + line);

                    if (firstLine && line.StartsWith(@"//")) //Process DIRECTIVES IN COMMENTS
                    {
                        //LINE 0 CAN CONTAIN EN~ES or ES~EN(default)
                        if ((line.Contains("ES~EN") || line.Contains("EN~ES")))
                            if (line.ToUpper().IndexOf("EN") < line.ToUpper().IndexOf("ES"))
                                 reverseOrder = true;

                        //LINE 0 CAN CONTAIN DEFINIION E.G. DEFN{so, like so, thus, that way, this way}
                        if (line.Contains("DEFN{"))
                        {
                            int i = line.IndexOf("DEFN{")+5;
                            int j = line.IndexOf("}", i);
                            defn = line.Substring(i, j - i);
                        }
                    }
                    else
                    {
                        Phrase.Factory(line, reverseOrder, defn, null/*no verb*/, Path.GetFileNameWithoutExtension(filepath));
                    }
                    firstLine = false;
                }
            }

        }

        internal string ModeToString
        {
            get
            {

                switch (_mode)
                {
                    case Mode.AllVerbPhrases:
                        return "Verb Usage Phrases (All)";
                    case Mode.FileSourcePhrases:
                        return "Phrsases from " + _selectedFileSource;
                    case Mode.NamedVerbPhrases:
                        return "Verb Usage Phrases from " + SelectedVerb.Infinitive;
                    case Mode.RandomPhrases:
                        return "Random Phrase";
                    case Mode.TranslationSource:
                        return "Translation";
                }
                return "UNKOWN MODE";

            }
        }

        private void LoadVerbConjugation(string folder)
        {

            string filepath = Path.Combine(folder, "Conjugation.csv");

            TextLoader loader = new TextLoaderDefaultEncoding(filepath);
            int lines = 0;

            while (loader.Peek() != null)
            {
                string line = loader.Next();

                if (lines==0)
                {
                    lines++;
                    continue;
                }

                

                string[] parts = line.Split(',');

                
                int CSVlineNum = System.Convert.ToInt32(parts[0]);
                var theVerb = Verb.InfinitiveLookup(parts[1]);

                if(theVerb == null)
                {
                    Debug.WriteLine(String.Format("Line {0} Verb {1} missing from master table", CSVlineNum, parts[1]));
                }
                else
                {
                    Verb.Mode m = Verb.ParseMode(parts[2]);
                    Verb.Tense t = Verb.ParseTense(parts[3]);
                    theVerb.AddConjugation(m, t, parts);
                }
                

                lines++;

            }
        }
       

        private void LoadVerbMasterTable(string folder)
        {
            string filepath = Path.Combine(folder, "VerbTable.csv");

            TextLoader loader = new TextLoaderDefaultEncoding(filepath);
            int lines = 0;
            while (loader.Peek() != null)
            {
                string line = loader.Next();


                //Create verb and add to collection and index
                if (lines>0)
                    Verb.Factory(line.Split(','));

                lines++;

            }
        }

        

        private void LoadSpanishDictVerbUsage(string folder)
        {

            //Load verbs definitions in subfolder /VerbUsage
            foreach (var filepath in  Directory.EnumerateFiles(folder, "*.txt"))
            {
                try
                {
                    var infinitive = Path.GetFileNameWithoutExtension(filepath);

                    if (!infinitive.EndsWith("EJEMPLOS"))
                    {

                        //Find in master table
                        var match = Verb.InfinitiveLookup(infinitive.ToLower());

                        if (match == null)
                        {
                            Debug.WriteLine("MISSING FROM VERB MASTER TABLE " + infinitive);
                            //match = new Verb(infinitive.ToLower(), "MISSING FROM VERB MASTER TABLE");
                            //Verb.Add(match);
                        }
                        else
                            match.LoadActionGroups(filepath);
                    }
                    else
                    {
                        //Skip verb exmaples fo now
                        //TODO - load the additional examples
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error in {0}-{1}" ,filepath, ex.Message)) ;
                }

                
            }
        }

        
        
        internal void Start()
        {
            ChangeMode(Mode.AllVerbPhrases);

           

        }


        public void ChangeMode(Mode m, Verb namedVerb = null, string fileSource = null)
        {
            _mode = m;
            switch (m)
            {
                case Mode.AllVerbPhrases:
                    SelectedVerb = null;
                    GenerateVerbPhraseQuestions();
                    break;
                case Mode.FileSourcePhrases:
                    _selectedFileSource = fileSource;
                    if (string.IsNullOrEmpty(_selectedFileSource))
                        throw new ArgumentException("Filesource cannot be null in this mode");
                    GenerateFilePhrasesQuestions();
                    break;
                case Mode.NamedVerbPhrases:
                    SelectedVerb = namedVerb; 
                    if (namedVerb == null)
                        throw new ArgumentException("Named verb cannot be null in this mode");
                    GenerateVerbPhraseQuestions();
                    break;
                case Mode.RandomPhrases:
                    break;
                case Mode.TranslationSource:
                    _selectedTranslationSource = fileSource;
                    break;

            }

            //Update UI
            _main.TestModeChange();

        }

        private void GenerateFilePhrasesQuestions()
        {
            GenerateQuestions(Phrase.FromFileSource(_selectedFileSource));
        }

        private void GenerateQuestions(List<Phrase> lst)
        {
            Shuffle(lst);

            _questions = new List<TestQuestion>();

            foreach (var p in lst)
                _questions.Add(new PhraseQuestion(p));

            _iter = _questions.GetEnumerator();
        }

        private void GenerateVerbPhraseQuestions(List<Phrase> lst)
        {
            Shuffle(lst);

            _questions = new List<TestQuestion>();

            foreach (var p in lst)
                _questions.Add(new VerbPhraseQuestion(p));

            _iter = _questions.GetEnumerator();
        }

        public void ResetTest()
        {
            if (_questions !=null)
                _iter = _questions.GetEnumerator();
        }

        private void GenerateVerbPhraseQuestions()
        {
            var lst = new List<Phrase>();

            if (SelectedVerb == null)
            {
                foreach (var verb in Verb.Verbs)
                    verb.AppendPhrases(lst);
            }
            else
                SelectedVerb.AppendPhrases(lst);

            GenerateVerbPhraseQuestions(lst);
        }

        internal TestQuestion CurrentQuestion()
        {
            return _iter == null ? null : _iter.Current;
        }

        internal TestQuestion NextQuestion()
        {
            return _iter.MoveNext() ? _iter.Current : null;

        }
       

        internal void SwitchLanguage()
        {
            if (TestLanguage == Language.EN)
                TestLanguage = Language.ES;
            else
                TestLanguage = Language.EN;

        }


        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }



        public IEnumerator<TestQuestion> GetEnumerator()
        {
            return _questions.GetEnumerator();
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return _questions.GetEnumerator();
        }
    }

      
     

}
