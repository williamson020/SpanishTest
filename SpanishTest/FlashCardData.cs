using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishTest
{
    class FlashCardData
    {
        public enum Language {  EN, ES}

        public string English { get; private set; }
        public string Spanish { get; private set; }

        public string GetText(Language l)
        {
            return l == Language.EN ? English : Spanish;
        }


        public FlashCardData(Word w)
        {
            if (w!=null)
            {
                English = w.English;
                Spanish = w.Spanish;
            }
            else
            {
                Spanish = "Fin de la lista";
                English = "End of list";
            }
            
        }

        public FlashCardData(Verb v)
        {
            if (v != null)
            {
                Spanish = v.Infinitive;
                English = v.Translation;
            }
            else
            {
                Spanish = "Fin de la lista";
                English = "End of list";
            }
        }

    }
}
