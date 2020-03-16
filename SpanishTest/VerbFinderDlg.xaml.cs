using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SpanishTest
{
    /// <summary>
    /// Interaction logic for VerbFinderDlg.xaml
    /// </summary>
    public partial class VerbFinderDlg : Window
    {
        private TestManager _tm;

        private string MatchFilter = null;

        public Verb Selection { get; set; }

        public VerbFinderDlg(TestManager tm)
        {
            InitializeComponent();

            _tm = tm;
            cbReflexive.IsChecked = false;
            cbRegular.IsChecked = true;
            cbIrregular.IsChecked = true;
            cbCommon.IsChecked = true;
            cbHasPhrases.IsChecked = true;
            //Load verbs

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lvVerbList.ItemsSource = FilterVerbs();
        }

        private List<Verb> FilterVerbs()
        {
           List<Verb> matchlist = Verb.Verbs;

            //If reflexive is unchecked , exclude reflexive
            if (!cbReflexive.IsChecked == true)
                matchlist = matchlist.Where(x => !x.Reflexive).ToList();


            //If Regular is unchecked, exclude Regular
            if (cbRegular.IsChecked == false)
                matchlist = matchlist.Where(x => !x.Regular).ToList();

            //If Irregular is unchecked, exclude irregular
            if (cbIrregular.IsChecked == false)
                matchlist = matchlist.Where(x => !x.Irregular).ToList();

            //Common
            if (cbCommon.IsChecked == true)
                matchlist = matchlist.Where(x => x.Common).ToList();

            //Has Phrases

            if (cbHasPhrases.IsChecked == true)
                matchlist = matchlist.Where(x => x.HasPhrases).ToList();


            if (!string.IsNullOrEmpty(MatchFilter) && MatchFilter.Length >= 2)
                matchlist = matchlist.Where(x => x.Infinitive.StartsWith(MatchFilter)).ToList();

            return matchlist;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Selection = null;
            this.DialogResult = false;
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Selection = null;

            Verb v = lvVerbList.SelectedItem as Verb;
            if (v != null)
                Selection = v;

            this.DialogResult = true;
            this.Close();
        }


        private void cbCommon_Click(object sender, RoutedEventArgs e)
        {
            lvVerbList.ItemsSource = FilterVerbs();
        }

        private void cbHasPhrases_Click(object sender, RoutedEventArgs e)
        {
            lvVerbList.ItemsSource = FilterVerbs();
        }
        private void cbReflexive_Click(object sender, RoutedEventArgs e)
        {
            lvVerbList.ItemsSource = FilterVerbs();
        }

        private void cnIrregular_Click(object sender, RoutedEventArgs e)
        {
            lvVerbList.ItemsSource = FilterVerbs();
        }

       

        private void cbRegular_Click(object sender, RoutedEventArgs e)
        {
            lvVerbList.ItemsSource = FilterVerbs();
        }


        private void btnConjugate_Click(object sender, RoutedEventArgs e)
        {

            Verb v = lvVerbList.SelectedItem as Verb;
            if (v != null)
                Selection = v;

            if (Selection != null)
            {


                Conjugation dlg = new Conjugation(Selection)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dlg.ShowDialog();
            }

            e.Handled = true;

        }

        private void tbFilter_KeyUp(object sender, KeyEventArgs e)
        {
            MatchFilter = tbFilter.Text.ToLower();
            lvVerbList.ItemsSource = FilterVerbs();
            
        }
    }
}
