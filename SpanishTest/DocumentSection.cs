using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace SpanishTest
{
    /// <summary> 
    /// A ActionGroup contains a collection of definitions of the verb used in the same action ie.  Transitive, Intransitive, Reflexive
    /// </summary>
    public class DocumentSection : IEnumerable<DefinitionGroup>
    {
        public enum WordType { VerbTransitive, VerbIntransitive, VerbReflexive, VerbImpersonal, VerbPronominal, AuxilliaryVerb, CopularVerb, NounMasculine, NounFeminine };

        public List<DefinitionGroup> DefinitionGroups = new List<DefinitionGroup>();

        private WordType _vtype;
        public Verb TheVerb { get; private set;}

        public DocumentSection(Verb v, WordType vt)
        {
            _vtype = vt;
            TheVerb = v;
        }

        public override string ToString()
        {
            return String.Format("{0}({1})", TheVerb.Infinitive, _vtype);
        }
        
        public static DocumentSection Factory(Verb v, string type)
        {
            if (type.ToUpper().StartsWith("TRANSITIVE"))
                return new DocumentSection(v, WordType.VerbTransitive);

            if (type.ToUpper().StartsWith("INTRANSITIVE"))
                return new DocumentSection(v, WordType.VerbIntransitive);

            if (type.ToUpper().StartsWith("IMPERSONAL"))
                return new DocumentSection(v, WordType.VerbImpersonal);

            if (type.ToUpper().StartsWith("PRONOMINAL"))
                return new DocumentSection(v, WordType.VerbPronominal);

            if (type.ToUpper().StartsWith("REFLEXIVE"))
                return new DocumentSection(v, WordType.VerbReflexive);

            if (type.ToUpper().StartsWith("AUXILIARY"))
                return new DocumentSection(v, WordType.AuxilliaryVerb);
            if (type.ToUpper().StartsWith("COPULAR"))
                return new DocumentSection(v, WordType.CopularVerb);
            

            if (type.ToUpper().StartsWith("MASCULINE NOUN"))
                return new DocumentSection(v, WordType.NounMasculine);


            throw new ArgumentException("UNKNOWN USAGE TYPE:" + type);
        }

        private string DefinitionGroupHeader(string line)
        {
            if (String.IsNullOrEmpty(line))
                return null;

            //if starts with a numer followd by period...
            int x = line.IndexOf('.');
            if (x == -1)
                return null;

            string sub = line.Substring(0, x);
            int y = 0;
            if (Int32.TryParse(sub, out y))
                return line.Substring(x + 1);

            return null;
        }

        internal void LoadFrom(TextLoader loader)
        {
            /*
             *  1. (to arrange) 
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
                3. (to get somebody a job) */

            while (DefinitionGroupHeader(loader.Peek()) != null)
            {
                var dg = new DefinitionGroup(this, DefinitionGroupHeader(loader.Next()));
                dg.LoadFrom(loader);
                DefinitionGroups.Add(dg);
            }


        }

        public IEnumerator<DefinitionGroup> GetEnumerator()
        {
            return DefinitionGroups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return DefinitionGroups.GetEnumerator();
        }
    }

  
}
