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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Content = new Clients(frame);

        }

        public class HelpClass
        {
            public static AvtoServisEntities db;
            public static bool flag = false;
            public static int prioritet = 0;

            public static AvtoServisEntities GetContext()
            {

                if (db == null)
                {
                    db = new AvtoServisEntities();
                }
                return db;
            }
        }

        private void frame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                Clients pg = (Clients)e.Content;
                pg.Load();
            }
            catch { };
        }
    }
}
