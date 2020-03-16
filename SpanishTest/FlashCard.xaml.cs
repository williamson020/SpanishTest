using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpanishTest
{
    /// <summary>
    /// Interaction logic for FlashCard.xaml
    /// </summary>
    public partial class FlashCard : Window
    {

        private enum Mode { Verbs, VerbsCommon, Vocab}

        private bool Manual { get; set; } = true; //advance mode

        private List<Word> _vocabList = Word.SpanishDict.Values.ToList();

        private List<Verb> _verbs = Verb.Verbs;

        private List<Word>.Enumerator _vocabEnum;
        private List<Verb>.Enumerator _verbEnum;
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        private int _interval = 1000; //milliseconds


        private static Random rng = new Random();

        private Mode FlashMode = Mode.Vocab;
        public FlashCard()
        {
            InitializeComponent();

            // The usual WPF timer is the DispatcherTimer, 
            //which is not a control but used in code.//It basically works the same way like the WinForms timer:

            Shuffle(_verbs);
            Shuffle(_vocabList);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!Manual)
                 NextFlashCard();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0,_interval);
            Reset();
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


        private void ResetIterator()
        {
            switch(FlashMode)
            {
                case Mode.Verbs:
                case Mode.VerbsCommon:
                    _verbEnum = _verbs.GetEnumerator();
                    break;

                case Mode.Vocab:
                    _vocabEnum = _vocabList.GetEnumerator();
                    break;
            }
                
        }

        private void Reset()
        {
            _dispatcherTimer.Stop();

            tbEnglish.Text = "";
            tbSpanish.Text = "";

            //Shuffle
            ResetIterator();

            if (Manual)
                NextFlashCard();
            else 
                _dispatcherTimer.Start();
        }

        private void NextFlashCard()
        {
            //advance iterator
            switch (FlashMode)
            {
                case Mode.Verbs:
                case Mode.VerbsCommon:
                    _verbEnum.MoveNext();
                    if (_verbEnum.Current == null)
                    {
                        ResetIterator();
                        _verbEnum.MoveNext();
                    }
                    break;
                case Mode.Vocab:
                    _vocabEnum.MoveNext();
                    if (_vocabEnum.Current == null)
                    {
                        ResetIterator();
                        _vocabEnum.MoveNext();
                    }
                    break;

            }

            var data = GenerateFlashCard();

            tbEnglish.Text = data.English;
            tbSpanish.Text = data.Spanish;
        }

        private FlashCardData GenerateFlashCard()
        {
            if (FlashMode == Mode.Verbs || FlashMode == Mode.VerbsCommon)
                return new FlashCardData(_verbEnum.Current);

            if (FlashMode == Mode.Vocab)
                return new FlashCardData(_vocabEnum.Current);

            return null;
        }

        private void tbInterval_LostFocus(object sender, RoutedEventArgs e)
        {
            int res = 0;
            if ( Int32.TryParse(tbInterval.Text, out res))
            {
                _interval = res;
                _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, _interval);
            }

        }

        private void bnVerbs_Click(object sender, RoutedEventArgs e)
        {
            FlashMode = Mode.Verbs;
            Reset();
        }

        private void bnVocab_Click(object sender, RoutedEventArgs e)
        {
            FlashMode = Mode.Vocab;
            Reset();
        }

        private void bnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bnManual_Click(object sender, RoutedEventArgs e)
        {
            Manual = true;
            _dispatcherTimer.Stop();
        }


        private void bnAuto_Click(object sender, RoutedEventArgs e)
        {
            Manual = false;
            _dispatcherTimer.Start();
        }

        private void bnVerbsCommon_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (Manual)
                NextFlashCard();
        }
    }
}
