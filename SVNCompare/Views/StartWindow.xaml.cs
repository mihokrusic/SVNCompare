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
using System.IO;
using System.Diagnostics;
using SVNCompare;

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

            lbxOutput.Items.Clear();
        }
        private void AddToOutput(string line)
        {
            //lbxOutput.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " - " + line);
            lbxOutput.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + line);
        }
        private void LoadIgnoreFilters(ref CompareGroupArguments args)
        {
            try
            {   
                using (StreamReader sr = new StreamReader("IgnoreFilters.txt"))
                {
                    String line = sr.ReadLine();
                    while (line != null)
                    {
                        Console.WriteLine(line);
                        switch (line.Substring(0, 1))
                        {
                            case "D":
                                args.IgnoreDirectories.Add(line.Substring(3));
                                break;
                            case "F":
                                args.IgnoreFiles.Add(line.Substring(3));
                                break;
                            default:
                                break;
                        }

                        line = sr.ReadLine();
                    }
                }
            }
            catch (FileNotFoundException e)
            {
            }
        }





        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            // Čistimo UI
            ClearUI();


            // Kreiramo objekt za compare direktorija
            if (_compareGroup == null)
            {                
                _compareGroup = new CompareGroup();
            }
            _compareGroup.Items.Clear();


            // Dodajemo iteme 
            for (int i = 1; i <= 5; i++)
            {
                TextBox txtControl = FindName("txtFolder" + i) as TextBox;

                if (txtControl.Text.Trim() != "")
                    _compareGroup.Items.Add(new CompareItem(txtControl.Text, i));
            }


            // Radimo SVN update
            if (chkUpdateSVN.IsChecked == true)
            {
                _compareGroup.UpdateSVN();


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


            // Uspoređivanje foldera
            List<CompareResultItem> compareResults;
            int sourceIndex = 0;

            if (rbtnDefault1.IsChecked == true)
                sourceIndex = 0;
            else if (rbtnDefault2.IsChecked == true)
                sourceIndex = 1;
            else if (rbtnDefault3.IsChecked == true)
                sourceIndex = 2;
            else if (rbtnDefault4.IsChecked == true)
                sourceIndex = 3;
            else if (rbtnDefault5.IsChecked == true)
                sourceIndex = 4;

            
            // Punimo ignore filtere
            CompareGroupArguments args = new CompareGroupArguments();
            LoadIgnoreFilters(ref args);

            _compareGroup.Compare(sourceIndex, args, out compareResults);


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
                AddToOutput(String.Format("  Comparing \"{0}\" with \"{1}\"", resultItem.source.path, resultItem.target.path));
                foreach (string line in resultItem.Log)
                    AddToOutput("    " + line);
                AddToOutput(String.Format("    Total files:         {0}", resultItem.totalFiles));
                AddToOutput(String.Format("    Identical files:     {0}", resultItem.identicalFiles));
                AddToOutput(String.Format("    Different files:     {0}", resultItem.differentFiles));
                AddToOutput(String.Format("    Left unique files:   {0}", resultItem.leftUniqueFiles));
                AddToOutput(String.Format("    Right unique files:  {0}", resultItem.rightUniqueFiles));
               
            }

            AddToOutput("Folder compare finished");
        }





        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            WorkingDialog dialog = new WorkingDialog();
            dialog.Title = "Working...";
            dialog.Owner = this;
            dialog.ShowDialog();            
        }

        private void btnIgnoreFilters_Click(object sender, RoutedEventArgs e)
        {
            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            Process.Start("IgnoreFilters.txt");
        }
    }
}
