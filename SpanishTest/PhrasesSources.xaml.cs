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

namespace SpanishTest
{
    /// <summary>
    /// Interaction logic for PhrasesSources.xaml
    /// </summary>
    public partial class PhrasesSources : Window
    {
        public PhrasesSources()
        {
            InitializeComponent();

            
        }

        public string FileSource { get; private set; }

        private void lbPhraseSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bnSelect.IsEnabled = lbPhraseSources.SelectedItem != null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbPhraseSources.ItemsSource = Phrase.GetFilesources();

            lbPhraseSources.SelectedItem = lbPhraseSources.Items[0];
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            FileSource = lbPhraseSources.SelectedItem.ToString();
            this.DialogResult = true;
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
