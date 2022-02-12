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
using System.Windows.Shapes;

namespace WPF_Task1
{
    /// <summary>
    /// Логика взаимодействия для Visits.xaml
    /// </summary>
    public partial class Visits : Page
    {
        Client client;
        public Visits(Client cl)
        {
            client = cl;
            InitializeComponent();
            Prosmotr();
            //IngredientTable();
            FirN.Text = client.FirstName;
            
        }

        private void fileGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        public void Prosmotr()
        {
            //visitGrid.ItemsSource = MainWindow.HelpClass.GetContext().ClientService.Where(ClientService => ClientService.ServiceID==).ToList();

            try
            {
                List<ClientService> TableData = MainWindow.HelpClass.GetContext().ClientService.Where(cl=>cl.ClientID==client.ID).ToList();
                visitGrid.ItemsSource = TableData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        

        private void visitGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (visitGrid.SelectedItems.Count > 0)
            {
                ClientService clSrv = visitGrid.SelectedItems[0] as ClientService;
                fileGrid.ItemsSource = clSrv.DocumentByService.ToList();
            }
        }

        private void visitGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ClientService clSrv = (ClientService)e.Row.DataContext;
            clSrv.files = clSrv.DocumentByService.Count();
        }
    }
}
