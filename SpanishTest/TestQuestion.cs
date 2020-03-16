using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishTest
{
    public abstract class TestQuestion
    {
        public TestQuestion()
        {

        }

        public abstract string  InfoBlock();
        public abstract string MainBlock(TestManager.Language lang);

    }

    public class PhraseQuestion : TestQuestion
    { 
        private string Info;
        protected Phrase ThePhrase;

        public PhraseQuestion(Phrase p)
        {
            Info = p.AssociatedVerb == null ? p.Notes : p.AssociatedVerb.ToString();

            ThePhrase = p;
        }


        public override string InfoBlock()
        {
            return Info;
        }

        public override string MainBlock(TestManager.Language lang)
        {
            return ThePhrase.GetText(lang);

        }


    }

    public class VerbPhraseQuestion : PhraseQuestion
    {
        public VerbPhraseQuestion(Phrase p):base(p)
        {
        }

        public string Infinitive
        {
            get { return ThePhrase.AssociatedVerb.Parent.Parent.TheVerb.Infinitive; }
        }

        public string Gerund
        {
            get { return ThePhrase.AssociatedVerb.Parent.Parent.TheVerb.Gerund; }
        }

        public string PastParticiple
        {
            get { return ThePhrase.AssociatedVerb.Parent.Parent.TheVerb.PastParticiple; }
        }
    }

}
