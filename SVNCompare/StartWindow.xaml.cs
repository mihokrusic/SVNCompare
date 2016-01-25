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
using SVNCompare.Models;

namespace SVNCompare
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private CompareGroup _compareGroup;




        public StartWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }




        private void ClearUI()
        {
            for (int i = 0; i < 5; i++)
            {
                Label lblControl = FindName("lblStatus" + (i + 1)) as Label;
                Rectangle rectUpdateStatus = FindName("rectUpdateStatus" + (i + 1)) as Rectangle;
                Rectangle rectCompareStatus = FindName("rectCompareStatus" + (i + 1)) as Rectangle;

                lblControl.Content = "";
                rectUpdateStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                rectCompareStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            }
        }
        private void ClearCompareUI()
        {
            for (int i = 0; i < 5; i++)
            {
                Rectangle rectCompareStatus = FindName("rectCompareStatus" + (i + 1)) as Rectangle;

                rectCompareStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            }
        }
        private void AddToOutput(string line)
        {
            //lbxOutput.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " - " + line);
            lbxOutput.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + line);
        }





        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Čistimo UI
            ClearUI();

            // Kreiramo objekt za compare direktorija
            if (_compareGroup == null)
                _compareGroup = new CompareGroup();
            _compareGroup.Items.Clear();

            // Dodajemo iteme 
            for (int i = 1; i <= 5; i++)
            {
                TextBox txtControl = FindName("txtFolder" + i) as TextBox;

                if (txtControl.Text.Trim() != "")
                    _compareGroup.Items.Add(new CompareItem(txtControl.Text, i));
            }

            // Radimo SVN update
            _compareGroup.SVNUpdate();

            // Zapisujemo rezultate SVN update-a
            for (int i = 0; i < _compareGroup.Items.Count; i++)
            {
                Label lblControl = FindName("lblStatus" + _compareGroup.Items[i].position) as Label;
                Rectangle rectUpdateStatus = FindName("rectUpdateStatus" + _compareGroup.Items[i].position) as Rectangle;

                lblControl.Content = _compareGroup.Items[i].lastUpdateMessage;

                switch (_compareGroup.Items[i].updateResult)
                {
                    case CompareItemSVNUpdateResult.Success:
                        rectUpdateStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF14B222"));
                        break;
                    case CompareItemSVNUpdateResult.Error:
                        rectUpdateStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB95B3F"));
                        break;
                    default:
                        rectUpdateStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                        break;
                }
            }

            AddToOutput("SVN update finished");
        }





        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            // Čistimo UI
            ClearCompareUI();


            // Uspoređivanje foldera
            List<CompareResultItem> compareResults;
            int defaultIndex = 0;

            if (rbtnDefault1.IsChecked == true)
                defaultIndex = 0;
            else if (rbtnDefault2.IsChecked == true)
                defaultIndex = 1;
            else if (rbtnDefault2.IsChecked == true)
                defaultIndex = 2;
            else if (rbtnDefault2.IsChecked == true)
                defaultIndex = 3;
            else if (rbtnDefault2.IsChecked == true)
                defaultIndex = 4;

            _compareGroup.Compare(defaultIndex, out compareResults);


            // Ispisivanje rezultata
            foreach (CompareResultItem resultItem in compareResults)
            {
                Rectangle rectCompareStatus = FindName("rectCompareStatus" + resultItem.target.position) as Rectangle;

                switch (resultItem.result)
                {
                    case ECompareResult.Identical:
                        rectCompareStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF14B222"));
                        break;
                    case ECompareResult.Different:
                        rectCompareStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB95B3F"));
                        break;
                    default:
                        rectCompareStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                        break;
                }

                // Zapisujemo u output
                AddToOutput(String.Format("Comparing \"{0}\" with \"{1}\"", resultItem.source.path, resultItem.target.path));
                //foreach (string line in resultItem.Log)
                //    AddToOutput(line);
                AddToOutput(String.Format("Identical files:     {0}", resultItem.identicalFiles));
                AddToOutput(String.Format("Different files:     {0}", resultItem.differentFiles));
                AddToOutput(String.Format("Left unique files:   {0}", resultItem.leftUniqueFiles));
                AddToOutput(String.Format("Right unique files:  {0}", resultItem.rightUniqueFiles));
               
            }

            AddToOutput("Folder compare finished");
        }
    }
}
