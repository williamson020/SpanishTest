using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishTest
{
    public class Verb
    {
        public static List<Verb> Verbs { get; private set; } = new List<Verb>();

        //Maps verb infinitive to Verb instance
        private static Dictionary<string, Verb> _verbDict = new Dictionary<string, Verb>();

        public static Verb InfinitiveLookup(string inf)
        {
            inf = inf.ToLower();

            if (_verbDict.ContainsKey(inf))
                return _verbDict[inf];

            return null;
        }

        public enum Mode { Indicativo , Condicional, Subjuntivo, Imperativo }
        public enum Tense { Presente, Pretérito, Futuro , Condicional, Imperfecto, PresenteProgresivo, PretéritoPerfecto, Pluscuamperfecto,
            FuturoPerfecto, CondicionalPerfecto, PretéritoAnterior, SubjuntivoPresente, SubjuntivoImperfecto, SubjuntivoFuturo,
            SubjuntivoPretéritoPerfecto, SubjuntivoPluscuamperfecto, SubjuntivoFuturoPerfecto, ImperativoPositivo, ImperativoNegativo
        }

        private Dictionary<Mode, Dictionary<Tense, string[]>> _conj = new Dictionary<Mode, Dictionary<Tense, string[]>>();


        public string RegularVerbColour
        {
            get
            {
                return Regular ? "Black" : "Red";
            }
        }


        public string Gerund { get; set; }  
        public string PastParticiple { get; set; }

        public bool Irregular { get; set; }

        public bool Regular { get { return !Irregular; } }

        public bool Reflexive { get; set; }

        public bool Common { get; set; }

        public bool HasPhrases
        {
            //If there was a VerbUsage file for this verb loaded
            get { return DocSections.Count > 0; }
        }


        public string Infinitive { get; private set; }
        public string Translation { get; private set; }

        public List<DocumentSection> DocSections = new List<DocumentSection>();

        /// FORMAT AS PASTED FROM SPANISHDICT.COM
        /// 

        /* TRANSITIVE VERB
                1. (to arrange) 
                a. to place 
                ...
                b. to put 
                ...
                c. to lay 
                ...
                2. (finance) 
                a. to place 
                ...
                b. to invest 
                ...
                3. (to get somebody a job) 
        */

        public static Verb Factory(string[] parts) // from verb master table VerbTable.csv
        {

            //id,verbo 1,gerund 2,partpasado 3,irregular 4,tipo 5,reflexivo 6,comun 7, trad_en8
            
            var v = new Verb(parts[1], parts[8])
            {
                Gerund = parts[2], //present participle
                PastParticiple = parts[3],
                Irregular = parts[4] == "1",
                Reflexive = parts[6] == "1",
                Common = parts[7] == "1"
            };

            _verbDict[v.Infinitive] = v;

            //Add this infinirive to SpanishDict also
            

            Verbs.Add(v);

            Word.Factory(v);

            return v;

        }

        private Verb(string inf, string translation)
        {
            Infinitive = inf.ToLower() ;
            Translation = translation;

            _conj[Mode.Condicional] = new Dictionary<Tense, string[]>();
            _conj[Mode.Imperativo] = new Dictionary<Tense, string[]>();
            _conj[Mode.Indicativo] = new Dictionary<Tense, string[]>();
            _conj[Mode.Subjuntivo] = new Dictionary<Tense, string[]>();


        }


        internal string[] Lookup(Mode m, Tense t)
        {
            var p = _conj[m];

            if (!p.ContainsKey(t))
                return new string[6];


            return p[t];
       }


        /// <summary>
        /// SPANISH DICT FORMAT
        /// </summary>
        /// <param name="path"></param>
        public void LoadActionGroups(string filepath)
        {
            TextLoader loader = new TextLoader(filepath);
            while (loader.Peek() != null)
            {
                string line = loader.Next();
                DocumentSection grp = DocumentSection.Factory(this, line);

                grp.LoadFrom(loader);

                if (null != grp)
                    DocSections.Add(grp);

            }
        }

        public void AppendPhrases(List<Phrase> lp)
        {
            foreach( var ds in DocSections)
            {
                //Enumerate definition groups
                foreach( var dg in ds) //
                {
                    //Enumerate VerbDefinition 
                    foreach (var defn in dg)
                        lp.AddRange(defn);
                }
            }
        }

        internal void AddConjugation(Mode mod, Tense tense, string[] parts)
        {

            var m = _conj[mod];

            if (!m.ContainsKey(tense))
                m.Add(tense, new string[6]);

            string[] qq = new string[6];


            //1,abandonar,Indicativo,Presente,abandono,abandonas,abandona,abandonamos,abandonáis,abandonan
            qq[0] = parts[4]; //yo
            qq[1] = parts[5]; //tu
            qq[2] = parts[6]; //el/ella/usted
            qq[3] = parts[7]; //nosostros
            qq[4] = parts[8]; //vosostros
            qq[5] = parts[9]; //ellos /ellas / ustedes

            m[tense] = qq;

        }
        public static Verb.Mode ParseMode(string v)
        {
            if (v == "Indicativo")
                return Verb.Mode.Indicativo;

            if (v == "Condicional")
                return Verb.Mode.Condicional;

            if (v == "Subjuntivo")
                return Verb.Mode.Subjuntivo;

            if (v == "Imperativo")
                return Verb.Mode.Imperativo;

            throw new InvalidDataException("INVALID VERB MODE:" + v);
        }
        public static Verb.Tense ParseTense(string v)
        {
            if (v == "Presente")
                return Verb.Tense.Presente;

            if (v == "Pretérito")
                return Verb.Tense.Pretérito;

            if (v == "Imperfecto")
                return Verb.Tense.Imperfecto;

            if (v == "Futuro")
                return Verb.Tense.Futuro;

            if (v == "Condicional")
                return Verb.Tense.Condicional;

            if (v == "Presente progresivo")
                return Verb.Tense.PresenteProgresivo;

            if (v == "Pretérito perfecto")
                return Verb.Tense.PretéritoPerfecto;

            if (v == "Pluscuamperfecto")
                return Verb.Tense.Pluscuamperfecto;

            if (v == "Futuro perfecto")
                return Verb.Tense.FuturoPerfecto;

            if (v == "Condicional perfecto")
                return Verb.Tense.CondicionalPerfecto;

            if (v == "Pretérito anterior")
                return Verb.Tense.PretéritoAnterior;

            if (v == "Subjuntivo presente")
                return Verb.Tense.SubjuntivoPresente;

            if (v == "Subjuntivo imperfecto")
                return Verb.Tense.SubjuntivoImperfecto;


            if (v == "Subjuntivo futuro")
                return Verb.Tense.SubjuntivoFuturo;


            if (v == "Subjuntivo pretérito perfecto")
                return Verb.Tense.SubjuntivoPretéritoPerfecto;

            if (v == "Subjuntivo pluscuamperfecto")
                return Verb.Tense.SubjuntivoPluscuamperfecto;

            if (v == "Subjuntivo futuro perfecto")
                return Verb.Tense.SubjuntivoFuturoPerfecto;

            if (v == "Imperativo positivo")
                return Verb.Tense.ImperativoPositivo;

            if (v == "Imperativo negativo")
                return Verb.Tense.ImperativoNegativo;

            throw new InvalidDataException("INVALID VERB TENSE:" + v);
        }
    }
}
