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

        public event PropertyChangedEventHandler PropertyChanged;

        public VerbTableEntry(string person, string pr, string im, string pret, string cond, string fut, bool createcopy=false)
        {
            Person = person;

            Present = pr;
            Imperfect = im;
            Preterite = pret;
            Conditional = cond;
            Future = fut;

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

        private  bool Match(string s1, string s2)
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
        public string Present { get;  set; }
        public string Preterite { get;  set; }
        public string Imperfect { get;  set; }

        public string Conditional { get;  set; }
        public string Future { get;  set; }

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

    }
    /// <summary>
    /// Interaction logic for Conjugation.xaml
    /// </summary>
    public partial class Conjugation : Window
    {

        private Verb _theVerb;

        private ObservableCollection<VerbTableEntry> _tableSource;

        public Conjugation(Verb v)
        {
            InitializeComponent();
            _theVerb = v;
            VerbDataGrid.CellEditEnding += myDG_CellEditEnding;
        }

        protected  void OnExecutedCommitEdit(System.Windows.Input.ExecutedRoutedEventArgs e)
        {
        }

            //protected virtual void OnExecutedCommitEdit(System.Windows.Input.ExecutedRoutedEventArgs e);

        private void myDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                

                var column = e.Column as DataGridBoundColumn;

                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;

                    //if (bindingPath == "Present")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        // rowIndex has the row index
                        // bindingPath has the column's binding
                        // el.Text has the new, user-entered value

                        FrameworkElement fe = e.Column.GetCellContent(e.Row);
                        VerbTableEntry item = (VerbTableEntry)fe.DataContext;
                        item.Present = el.Text;

                        if (!item.PresentMatch)
                            ((TextBox)fe).Background = Brushes.PaleVioletRed;

                        rowIndex++;
                        if (rowIndex > 5)
                            rowIndex = 0;

                        //Advance to next row (same column)
                        
                        VerbDataGrid.SelectedCells.Clear();
                        //Set selected cell
                        //var cInfo = new DataGridCellInfo(VerbDataGrid.Items[rowIndex], VerbDataGrid.Columns[column.DisplayIndex]);
                        //VerbDataGrid.SelectedCells.Add(cInfo);

                        //Set keyboard focus to same
                       
                        //Keyboard.Focus(cell);

                        VerbDataGrid.ItemsSource = null;

                        VerbDataGrid.ItemsSource = _tableSource
                        ;
                        //VerbDataGrid.Dispatcher.BeginInvoke(new Action(() => VerbDataGrid.Items.Refresh()), System.Windows.Threading.DispatcherPriority.Background);

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
            _tableSource = LoadTable();

            VerbDataGrid.ItemsSource = _tableSource;

            //Keyboard.Focus(GetDataGridCell(VerbDataGrid.SelectedCells[0]));

        }

        

        private DataGridCell GetDataGridCell(DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

            if (cellContent != null)
                return ((DataGridCell)cellContent.Parent);

            return (null);
        }


        

       

        private ObservableCollection<VerbTableEntry> LoadTable()
        {
            //Present
            //Preterite
            //Imperfect
            //Conditional
            //Future

            _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Presente);


            var lst = new ObservableCollection<VerbTableEntry>
            {
                MakeEntry("yo", 0),
                MakeEntry("tú", 1),
                MakeEntry("él/ella/Ud.", 2),

                MakeEntry("nosotros", 3),
                MakeEntry("nosotros", 4),
                MakeEntry("éllos/ellas/Uds.", 5)
            };


            return lst;
        }

        private VerbTableEntry MakeEntry(string person, int index)
        {
            var pr =  _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Presente)[index];
            var im = _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Imperfecto)[index];
            var pret = _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Pretérito)[index];
            var cond = _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Condicional)[index];
            var fut= _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Futuro)[index];

            

            return new VerbTableEntry(person, pr, im, pret,cond,fut,true); //creates a master copy
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
