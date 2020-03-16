using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SpanishTest
{
    public class VerbDefinition : IEnumerable<Phrase>
    {
        public DefinitionGroup Parent { get; private set; }
        public string Definition { get; private set; }
        private List<Phrase> _examples = new List<Phrase>();

        public VerbDefinition(DefinitionGroup x, string definition)
        {
            Parent = x;
            Definition = definition;

        }

        public override string ToString()
        {
            return Parent.ToString() + "//" + Definition;
        }

        
        public IEnumerator<Phrase> GetEnumerator()
        {
            return Examples.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Examples.GetEnumerator();
        }

        internal void AddExample(Phrase phrase)
        {
            _examples.Add(phrase);
            Debug.WriteLine(phrase.ToString());
        }


        internal List<Phrase> Examples
        {
            get { return _examples; }
        }
    }
}
