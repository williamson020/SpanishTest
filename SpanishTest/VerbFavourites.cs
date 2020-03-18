using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishTest
{
    class VerbFavourites
    {

        private List<Verb> _items = new List<Verb>();

        private static VerbFavourites _instance;

        public static void Init()
        {
            if (null == _instance)
                _instance = new VerbFavourites();
        }

        public static VerbFavourites Instance
        {
            get { return _instance; }
        }

        private VerbFavourites()
        {
            var vec = MainWindow.VerbFavourites.ToList<string>();

            foreach(string s in vec)
            {
                var lookup = Verb.InfinitiveLookup(s);
                if (null!=lookup)
                    _items.Add(lookup);
            }
        }

        public bool Contains(Verb b)
        {
            return b==null ? false: _items.IndexOf(b) >= 0;
        }

        public Verb Next(Verb b)
        {
            if (!Contains(b))
                return _items.FirstOrDefault();

            int idx = _items.IndexOf(b);

            if (idx == _items.Count - 1)
                return _items.FirstOrDefault();
            else
                return _items[idx + 1];

        }

        public Verb Prev(Verb b)
        {
            if (!Contains(b))
                return _items.FirstOrDefault();

            int idx = _items.IndexOf(b);

            if (idx == 0)
                return _items.LastOrDefault();
            else
                return _items[idx - 1];

        }

        private string[] ToArray()
        {
            string[] vec = new string[_items.Count];
            int i = 0;

            foreach (var v in _items)
                vec[i++] = v.Infinitive;

            return vec;
        }


        public void Remove(Verb b)
        {
            if (Contains(b))
            {
                _items.Remove(b);
                MainWindow.VerbFavourites = ToArray();
            }
        }

        public void Add(Verb b)
        {
            if (b!= null && !Contains(b))
            {
                _items.Add(b);
                MainWindow.VerbFavourites = ToArray();
            }
        }



    }
}
