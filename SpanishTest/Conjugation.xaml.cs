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

    public class VerbTableEntry
    {
        public VerbTableEntry(string person, string pr, string im, string pret, string cond, string fut)
        {
            Person = person;

            Present = pr;
            Imperfect = im;
            Preterite = pret;
            Conditional = cond;
            Future = fut;

            SetFlags();
        }

        public VerbTableEntry(VerbTableEntry x)
        {
            Person = x.Person;

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
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    if (bindingPath == "Present")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        // rowIndex has the row index
                        // bindingPath has the column's binding
                        // el.Text has the new, user-entered value

                        rowIndex++;
                        if (rowIndex > 5)
                            rowIndex = 0;

                        //Advance to next row (same column)
                        
                        VerbDataGrid.SelectedCells.Clear();
                        //Set selected cell
                        //var cInfo = new DataGridCellInfo(VerbDataGrid.Items[rowIndex], VerbDataGrid.Columns[column.DisplayIndex]);
                        //VerbDataGrid.SelectedCells.Add(cInfo);

                        //Set keyboard focus to same
                        DataGridCell cell = DGUtil.GetCell(VerbDataGrid, rowIndex, column.DisplayIndex);
                        //Keyboard.Focus(cell);

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

            VerbDataGrid.ItemsSource = LoadTable();

            //Keyboard.Focus(GetDataGridCell(VerbDataGrid.SelectedCells[0]));

        }

        

        private DataGridCell GetDataGridCell(DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

            if (cellContent != null)
                return ((DataGridCell)cellContent.Parent);

            return (null);
        }


        

       

        private List<VerbTableEntry> LoadTable()
        {
            //Present
            //Preterite
            //Imperfect
            //Conditional
            //Future

            _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Presente);


            var lst = new List<VerbTableEntry>();

            lst.Add(MakeEntry("yo", 0));
            lst.Add(MakeEntry("tú", 1));
            lst.Add(MakeEntry("él/ella/Ud.", 2));

            lst.Add(MakeEntry("nosotros", 3));
            lst.Add(MakeEntry("nosotros", 4));
            lst.Add(MakeEntry("éllos/ellas/Uds.",5));


            return lst;
        }

        private VerbTableEntry MakeEntry(string person, int index)
        {
            var pr =  _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Presente)[index];
            var im = _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Imperfecto)[index];
            var pret = _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Pretérito)[index];
            var cond = _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Condicional)[index];
            var fut= _theVerb.Lookup(Verb.Mode.Indicativo, Verb.Tense.Futuro)[index];

            

            return new VerbTableEntry(person, pr, im, pret,cond,fut);
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
