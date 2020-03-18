using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SpanishTest
{

    public class VerbTableEntry : INotifyPropertyChanged
    {
        private VerbTableEntry _master;

        public enum Tense { Present, Preterite, Imperfect, Conditional, Future}

        private string[] _items = new string[5];


        public event PropertyChangedEventHandler PropertyChanged;

        public VerbTableEntry(string person, string pr, string im, string pret, string cond, string fut, bool createcopy=false)
        {
            Person = person;

            _items[0] = pr;
            _items[1] = pret;
            _items[2] = im;
            _items[3] = cond;
            _items[4] = fut;

            SetFlags();

            if (createcopy)
                _master = new VerbTableEntry(person, pr, im, pret, cond, fut);
        }


        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static bool Match(string s1, string s2)
        {
            return (String.Compare(s1, s2, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0);
        }

        public bool PresentMatch
        {
            get { return _master == null ? false : Match(_master.Present , Present); }
        }

        public bool FutureMatch
        {
            get { return _master == null ? false : Match(_master.Future , Future); }
        }

        public bool PreteriteMatch
        {
            get { return _master == null ? false : Match(_master.Preterite, Preterite); }
        }


        public bool ImperfectMatch
        {
            get { return _master == null ? false : Match(_master.Imperfect, Imperfect); }
        }

        public bool ConditionalMatch
        {
            get { return _master == null ? false : Match(_master.Conditional, Conditional); }
        }

        public string Person{ get;  set; }
        public string Present
        {
            get { return _items[0]; }
            set { _items[0] = value; }
        }

        public string Preterite
        {
            get { return _items[1]; }
            set { _items[1] = value; }
        }
        public string Imperfect
        {
            get { return _items[2]; }
            set { _items[2] = value; }
        }

        public string Conditional
        {
            get { return _items[3]; }
            set { _items[3] = value; }
        }
        public string Future
        {
            get { return _items[4]; }
            set { _items[4] = value; }
        }

        public bool PresentIsRegular { get; private set; }
        public bool PreteriteIsRegular { get; private set; }
        public bool ImperfectIsRegular { get; private set; }
        public bool ConditionalIsRegular { get; private set; }
        public bool FutureIsRegular { get; private set; }

        private void SetFlags()
        {
            PresentIsRegular = !Present.StartsWith(".");
            PreteriteIsRegular = !Preterite.StartsWith(".");
            ImperfectIsRegular = !Imperfect.StartsWith(".");
            FutureIsRegular = !Future.StartsWith(".");

            ConditionalIsRegular = true; // hack fix me !Conditional.StartsWith(".");


            if (!PresentIsRegular)
                Present = Present.Substring(1);

            if (!PreteriteIsRegular)
                Preterite = Preterite.Substring(1);

            if (!ImperfectIsRegular)
                Imperfect = Imperfect.Substring(1);

            if (!FutureIsRegular)
                Future = Future.Substring(1);

            if (!ConditionalIsRegular)
                Conditional = Conditional.Substring(1);
        }

        internal static Tense TenseFromColumn(int colIndex)
        {
            return (VerbTableEntry.Tense)colIndex;
        }

        internal string Item(Tense t)
        {
            return _items[(int)t];
        }

        internal void SetValue(Tense t, string s)
        {
            int x = (int)t;

            _items[x] = s;

            if (Match(_items[x], _master.Item(t)))
                _items[x] = _master.Item(t);

        }

        internal void TestMode()
        {
            for (int i=0; i<5; i++)
            {
                _items[i] = "";
            }
        }

        internal void Reset()
        {
            for (int i = 0; i < 5; i++)
            {
                _items[i] = _master.Item((Tense)i);
            }
        }

    }
    /// <summary>
    /// Interaction logic for Conjugation.xaml
    /// </summary>
    public partial class Conjugation : Window
    {

        private Verb _theVerb;

        private MainWindow _mainWnd;

        private ObservableCollection<VerbTableEntry> _tableSource;

        public Conjugation(Verb v)
        {


            InitializeComponent();
            _theVerb = v;
            VerbDataGrid.CellEditEnding += myDG_CellEditEnding;

            
        }




        private void myDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;

                if (column != null)
                {
                    //var bindingPath = (column.Binding as Binding).Path.Path;

                    VerbTableEntry.Tense t = VerbTableEntry.TenseFromColumn(column.DisplayIndex-1);

                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        // rowIndex has the row index
                        // bindingPath has the column's binding
                        // el.Text has the new, user-entered value

                        FrameworkElement fe = e.Column.GetCellContent(e.Row);
                        VerbTableEntry item = (VerbTableEntry)fe.DataContext;
                        item.SetValue(t, el.Text);

                        //if (!item.PresentMatch)
                            //((TextBox)fe).Background = Brushes.PaleVioletRed;

                        rowIndex++;
                        if (rowIndex > 5)
                            rowIndex = 0;

                        //Advance to next row (same column)
                        
                        VerbDataGrid.SelectedCells.Clear();


                        //In order to invoke the cell colour style triggers - null out and reset the DG item source // frig!!
                        FlashGrid();

                        DataGridCell cell = DGUtil.GetCell(VerbDataGrid, rowIndex, column.DisplayIndex);
                        if (cell != null)
                        {
                            cell.IsSelected = true;
                            cell.Focus();
                            VerbDataGrid.BeginEdit();
                        }
                    }
                }
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetVerb(_theVerb);
        }


        private void SetVerb(Verb v)
        {
            _theVerb = v;
            _tableSource = LoadTable(_theVerb);

            tbGerund.Text = _theVerb.Gerund;
            tbPastParticiple.Text = _theVerb.PastParticiple;

            tbInfinitive.Text = _theVerb.Infinitive;
            tbDefinition.Text = _theVerb.Translation;
            FlashGrid();
        }

        
        /*
        private DataGridCell GetDataGridCell(DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

            if (cellContent != null)
                return ((DataGridCell)cellContent.Parent);

            return (null);
        }*/

        private static ObservableCollection<VerbTableEntry> LoadTable(Verb v)
        {
            //Present
            //Preterite
            //Imperfect
            //Conditional
            //Future

            v.Lookup(Verb.Mode.Indicativo, Verb.Tense.Presente);


            var lst = new ObservableCollection<VerbTableEntry>
            {
                MakeEntry(v, "yo", 0),
                MakeEntry(v, "tú", 1),
                MakeEntry(v, "él/ella/Ud.", 2),

                MakeEntry(v, "nosotros", 3),
                MakeEntry(v, "nosotros", 4),
                MakeEntry(v, "éllos/ellas/Uds.", 5)
            };


            return lst;
        }

        private static VerbTableEntry MakeEntry(Verb v, string person, int index)
        {
            var pr =  v.Lookup(Verb.Mode.Indicativo, Verb.Tense.Presente)[index];
            var im = v.Lookup(Verb.Mode.Indicativo, Verb.Tense.Imperfecto)[index];
            var pret = v.Lookup(Verb.Mode.Indicativo, Verb.Tense.Pretérito)[index];
            var cond = v.Lookup(Verb.Mode.Indicativo, Verb.Tense.Condicional)[index];
            var fut= v.Lookup(Verb.Mode.Indicativo, Verb.Tense.Futuro)[index];

            return new VerbTableEntry(person, pr, im, pret,cond,fut,true); //creates a master copy
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FlashGrid()
        {
            VerbDataGrid.ItemsSource = null;
            VerbDataGrid.ItemsSource = _tableSource;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (VerbTableEntry vte in _tableSource)
                vte.Reset();

            FlashGrid();
        }

        private void btnTestMode_Click(object sender, RoutedEventArgs e)
        {
            foreach (VerbTableEntry vte in _tableSource)
                vte.TestMode();

            FlashGrid();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

            var v = VerbFavourites.Instance.Next(_theVerb);

            if (null != v)
                SetVerb(v);

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var v = VerbFavourites.Instance.Prev(_theVerb);

            if (null != v)
                SetVerb(v);

        }
    }
}
