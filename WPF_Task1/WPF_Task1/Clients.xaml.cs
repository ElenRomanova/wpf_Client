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
    /// Логика взаимодействия для Clients.xaml
    /// </summary>
    public partial class Clients : Page
    {

        private string fnd = "";
        private int order = 0;
        private int start = 0;
        private int fullCount = 0;
        private string gender = "0";
        private int fullCountD = 0;
        private bool birthday = false;
        private int step = 10;
        Frame fr;


        public Clients(Frame frame)
        {
            fr = frame;

            InitializeComponent();
            List<Gender> genders = new List<Gender> { };
            try
            {
                genders = MainWindow.HelpClass.GetContext().Gender.ToList();
                genders.Add(new Gender { Name = "Все", Code = "0" });
                Gender.ItemsSource = genders.OrderBy(Gender => Gender.Code);
            }
            catch { }
            Load();

        }

        public void Load()
        {
            List<Client> clients = new List<Client>();
            try
            {
                fullCountD = MainWindow.HelpClass.GetContext().Client.Count();

                var ag = MainWindow.HelpClass.GetContext().Client.Where(c => (c.FirstName.Contains(fnd)) || (c.LastName.Contains(fnd)) || (c.Patronymic.Contains(fnd)) || (c.Phone.Contains(fnd)) || (c.Email.Contains(fnd)));
                if (birthday) ag = ag.Where(c => c.Birthday.Value.Month == DateTime.Now.Month);
                if (gender != "0") ag = ag.Where(c => c.GenderCode == gender);
                fullCount = ag.Count();
                clients.Clear();
                foreach (Client client in ag)
                {
                    client.ClServCn = client.ClientService.Count();
                    client.ClServDt = DateTime.MinValue;
                    foreach (ClientService clientService in client.ClientService)
                    {
                        if (clientService.StartTime > client.ClServDt) client.ClServDt = clientService.StartTime;
                    }
                    clients.Add(client);
                };
                if (order == 0) clientGrid.ItemsSource = ag.OrderBy(Client => Client.ID).Skip(start * step).Take(step).ToList();
                if (order == 1) clientGrid.ItemsSource = ag.OrderBy(Client => Client.FirstName).Skip(start * step).Take(step).ToList();
                if (order == 2 || order == 3)
                {
                    clients.Sort(comp);
                    clientGrid.ItemsSource = clients.Skip(start * step).Take(step).ToList();
                }
            }
            catch { };

            full.Text = fullCountD.ToString();
            fullr.Text = fullCount.ToString();
            int ost = fullCount % step;
            int pag = (fullCount - ost) / step;
            if (ost > 0) pag++;
            Paging.Children.Clear();

            for (int i = 0; i < pag; i++)
            {
                Button ChPag = new Button();
                ChPag.Height = 30;
                ChPag.Width = 20;
                ChPag.Content = i + 1;
                ChPag.HorizontalAlignment = HorizontalAlignment.Center;
                ChPag.Tag = i;
                ChPag.Click += new RoutedEventHandler(ChPag_Click);
                Paging.Children.Add(ChPag);
            }
            turnButton();

        }

        private void ChPag_Click(object sender, RoutedEventArgs e)
        {
            start = Convert.ToInt32(((Button)sender).Tag.ToString());
            turnButton();
            Load();

        }

        private int comp(Client x, Client y)
        {
            //throw new NotImplementedException();
            if (order == 3)
            {
                if (x.ClServCn == y.ClServCn) return 0;
                if (x.ClServCn > y.ClServCn) return -1;
                return 1;
            }
            if (x.ClServDt == y.ClServDt) return 0;
            if (x.ClServDt > y.ClServDt) return -1;
            return 1;
        }

        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            gender = ((Gender)comboBox.SelectedItem).Code;
            Load();
        }

        private void clientGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Client client = (Client)e.Row.DataContext;
            TextBlock panel = new TextBlock();
            panel.TextWrapping = TextWrapping.Wrap;
            var converter = new System.Windows.Media.BrushConverter();
            foreach (Tag tag in client.Tag)
            {
                Run run = new Run();
                run.Text = tag.Title + " ";
                try
                {
                    run.Foreground = (Brush)converter.ConvertFromString("#" + tag.Color);
                }
                catch { };
                panel.Inlines.Add(run);
            };
            client.tags = panel;

        }

        private void BirthDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            int i = Convert.ToInt32(selectedItem.Tag.ToString());
            if (i == 0) { birthday = false; }
            else { birthday = true; };
            Load();

        }

        public void turnButton()
        {
            if (start == 0) { Back.IsEnabled = false; }
            else { Back.IsEnabled = true; }
            if ((start + 1) * 10< fullCount) { forward.IsEnabled = true; }
            else { forward.IsEnabled =false ; }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            start--;
            turnButton();
            Load();
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            start++;
            turnButton();
            Load();
        }

        private void poisk_TextChanged(object sender, TextChangedEventArgs e)
        {
            fnd = ((TextBox)sender).Text;
            Load();
        }

        private void Step_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            step = Convert.ToInt32(selectedItem.Tag.ToString());
            Load();

        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem comboBoxItem = (ComboBoxItem)comboBox.SelectedItem;
            order = Convert.ToInt32(comboBoxItem.Tag.ToString());
            Load();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            fr.Content = new AddUpdate(null);
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientGrid.SelectedItems.Count > 0)
            {
                Client client = clientGrid.SelectedItems[0] as Client;

                if (client != null)
                {
                    fr.Content = new AddUpdate(client);
                }

            }
        }

        private void clientGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void visit_Click(object sender, RoutedEventArgs e)
        {
            if (clientGrid.SelectedItems.Count > 0)
            {
                Client client = clientGrid.SelectedItems[0] as Client;

                if (client != null)
                {
                    fr.Content = new Visits(client);
                }
            }
        }
    }

}
