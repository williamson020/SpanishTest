using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishTest
{
    public class DefinitionGroup : IEnumerable<VerbDefinition>
    {
        public DocumentSection Parent { get; private set; }

        public string Title { get; private set; }
        private List<VerbDefinition> Definitions = new List<VerbDefinition>();

        public DefinitionGroup(DocumentSection ag, string title)
        {
            Title = title;
            Parent = ag;
        }

        public override string ToString()
        {
            return Parent.ToString() + "//" + Title;
        }


        private string DefinitionHeader(string line)
        {
            if (String.IsNullOrEmpty(line))
                return null;

            //if starts with a letter followd by period...
            int x = line.IndexOf('.');
            if (x != 1)
                return null;

            return line.Substring(0, 2);
        }

        private VerbDefinition CurrentDefinition
        {
            get
            {
                return Definitions.Last();
            }
        }

        public void LoadFrom(TextLoader loader)
        {
            while (true)
            {
                if (DefinitionHeader(loader.Peek()) != null)
                {
                    Definitions.Add(new VerbDefinition(this, loader.Next().Substring(2)));
                }
                else
                    if (Phrase.IsPhrase(loader.Peek()))
                    {
                        CurrentDefinition.AddExample(Phrase.Factory(loader.Next(), false,  null, CurrentDefinition));
                    }
                    else
                        break;
            }
        }

        public IEnumerator<VerbDefinition> GetEnumerator()
        {
            return Definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Definitions.GetEnumerator();
        }
    }

}
