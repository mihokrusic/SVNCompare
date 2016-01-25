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
            // Dodajemo iteme i restartamo labele/rectangle
            for (int i = 0; i < 5; i++)
            {
                // Brišemo labele i rectangle
                Label lblControl = FindName("lblStatus" + (i + 1)) as Label;
                Rectangle rectUpdateStatus = FindName("rectUpdateStatus" + (i + 1)) as Rectangle;
                Rectangle rectCompareStatus = FindName("rectCompareStatus" + (i + 1)) as Rectangle;

                lblControl.Content = "";
                rectUpdateStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                rectCompareStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            }
        }





        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ClearUI();

            // Kreiramo objekt za compare direktorija
            if (_compareGroup == null)
                _compareGroup = new CompareGroup();
            _compareGroup.Items.Clear();

            // Dodajemo iteme 
            for (int i = 0; i < 5; i++)
            {
                TextBox txtControl = FindName("txtFolder" + (i + 1)) as TextBox;

                if (txtControl.Text.Trim() != "")
                    _compareGroup.Items.Add(new CompareItem(txtControl.Text, i + 1));
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

            lbxOutput.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " - " + "SVN update finished");
        }





        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            lbxOutput.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " - " + "Folder compare finished");
        }
    }
}
