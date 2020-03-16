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
using System.Windows.Navigation;
using System.Configuration;
using System.Windows.Shapes;

namespace SpanishTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TestManager tm;

        private bool AddWordActive { get; set; }
       

        public MainWindow()
        {
            InitializeComponent();

            Register(new CommandBinding(AppCommands.VerbFinder, VerbFinder));
            Register(new CommandBinding(AppCommands.Phrases, PhrasesMode));
            Register(new CommandBinding(AppCommands.Translations, TranslationsMode));
            Register(new CommandBinding(AppCommands.FlashCards, FlashCards));
            Register(new CommandBinding(AppCommands.Quit, Quit));
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tm = new TestManager(this, TestPath);
            tm.Start();
            tbLookup.Visibility = Visibility.Hidden;

            tbDefnLabel.Visibility = Visibility.Hidden;
            tbDefinition.Visibility = Visibility.Hidden;
            btnAddWord.Visibility = Visibility.Hidden;

        }

        private void Register(CommandBinding cb)
        {
            CommandManager.RegisterClassCommandBinding(typeof(Window), cb);
        }

        
        private void TranslationsMode(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }


        private void FlashCards(object sender, ExecutedRoutedEventArgs e)
        {
            FlashCard dlg = new FlashCard()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();

            e.Handled = true;
        }

        internal void TestModeChange()
        {
            tbSourceMode.Text = tm.ModeToString;
           Update(tm.NextQuestion());
        }


        private void PhrasesMode(object sender, ExecutedRoutedEventArgs e)
        {
            PhrasesSources dlg = new PhrasesSources()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dlg.ShowDialog().Value)
            {
                //Set phrase source
                tm.ChangeMode(TestManager.Mode.FileSourcePhrases, null, dlg.FileSource);
            }
            e.Handled = true;
        }

        private void VerbFinder(object sender, ExecutedRoutedEventArgs e)
        {

            VerbFinderDlg dlg = new VerbFinderDlg(tm)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dlg.ShowDialog().Value)
            {
                tm.ChangeMode(TestManager.Mode.NamedVerbPhrases, dlg.Selection);
            }

            e.Handled = true;

        }

        private void Quit(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }


        public static string TestPath
        {
            get { return ConfigurationManager.AppSettings["test_path"]; }

        }

        private void Update(TestQuestion q)
        {
            if (q is VerbPhraseQuestion)
            {
                Update((VerbPhraseQuestion)q);
                return;
            }

            BLOCK_PASTPARTICPLE.Visibility = Visibility.Hidden;
            BLOCK_GEROUND.Visibility = Visibility.Hidden;

            if (null == q)
            {
                BLOCK1.Text = "End of test";
                BLOCK2.Text = "";
            }
            else
            {
                BLOCK1.Text = q.InfoBlock();
                BLOCK2.Text = q.MainBlock(tm.TestLanguage);
            }
        }

        private void Update(VerbPhraseQuestion q)
        {
            BLOCK_PASTPARTICPLE.Visibility = Visibility.Visible;
            BLOCK_GEROUND.Visibility = Visibility.Visible;

            if (null == q)
            {
                BLOCK1.Text = "End of test";
                BLOCK2.Text = "";
                BLOCK_PASTPARTICPLE.Visibility = Visibility.Hidden;
                BLOCK_GEROUND.Visibility = Visibility.Hidden;
            }
            else
            {
                BLOCK1.Text = q.Infinitive.ToUpper();
                BLOCK1A.Text = q.InfoBlock();
                BLOCK_GEROUND.Text = q.Gerund;
                BLOCK_PASTPARTICPLE.Text = q.PastParticiple;
                BLOCK2.Text = q.MainBlock(tm.TestLanguage);
            }
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (cbRevertEngish.IsChecked.Value && tm.TestLanguage==TestManager.Language.ES)
            {
                tm.SwitchLanguage();
            }

            var q = tm.NextQuestion();
            if (null == q)
                tm.ResetTest();

            Update(q);
        }

        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            tm.SwitchLanguage();
            btnLanguage.Content = tm.TestLanguage.ToString();

            Update(tm.CurrentQuestion());

    
        }

        private void btnConj_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (!AddWordActive)
            {
                tbLookup.Clear();
                if (e.Key == Key.NumPad0)
                    tbLookup.Visibility = Visibility.Hidden;
            }
        }


        private void ShowLookup(string text)
        {
            tbLookup.Visibility = Visibility.Visible;
            //vocab loopup
            text = text.Trim();
            if (Word.SpanishDict.ContainsKey(text))
            {
                tbLookup.Text = Word.SpanishDict[text].ToString();
            }
            else
                tbLookup.Text = text + ": no match";
        }


        private void AddWord(string text)
        {

            text = text.Trim();
            if (Word.SpanishDict.ContainsKey(text))
            {
                ShowLookup(text);
                return;
            }

            

            tbLookup.Visibility = Visibility.Visible;
            tbLookup.Text = text;
            tbDefnLabel.Visibility = Visibility.Visible;
            tbDefinition.Visibility = Visibility.Visible;
            btnAddWord.Visibility = Visibility.Visible;
            tbDefinition.Clear();
            tbDefinition.Focus();
            AddWordActive = true;

            this.KeyDown -= Window_KeyDown;
        }

        private void EscapeAddWordMode()
        {
            tbLookup.Visibility = Visibility.Hidden;
            tbDefnLabel.Visibility = Visibility.Hidden;
            tbDefinition.Visibility = Visibility.Hidden;
            btnAddWord.Visibility = Visibility.Hidden;
            AddWordActive = false;

            this.KeyDown += Window_KeyDown;
        }

        private void btnAddWord_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(tbDefinition.Text))
            {
                //Add entry and save to vocab
                Word.Factory(String.Format("{0}~{1}", tbLookup.Text, tbDefinition.Text), false, true, tm.VocabFolder);
                EscapeAddWordMode();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
           

            if (!String.IsNullOrEmpty(BLOCK2.SelectedText) && (e.Key == Key.NumPad0))
            {
                ShowLookup(BLOCK2.SelectedText);
                return;
            }

            if (!String.IsNullOrEmpty(BLOCK2.SelectedText) && (e.Key == Key.Tab))
                AddWord(BLOCK2.SelectedText);
        }

        private void tbDefinition_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key== Key.Escape)
            {
                EscapeAddWordMode();
                e.Handled = true;
            }
        }
    }

   

}
