using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddUpdate.xaml
    /// </summary>
    public partial class AddUpdate : Page
    {

        public List<Tag> tg = new List<Tag>();
        public Client client;
        public AddUpdate(Client cl)
        {
            if (cl == null) client = new Client();
            else client = cl;
            this.DataContext = client;
            InitializeComponent();
          
            try
            {
                List<Gender> genders = new List<Gender> { };
                genders = MainWindow.HelpClass.GetContext().Gender.ToList();
                for (int i = 0; i < 2; i++)
                {
                    Gender cln = genders[i];
                    ((RadioButton)Gender.Children[i]).Content = cln.Name;
                    ((RadioButton)Gender.Children[i]).Tag = cln.Code;
                    if (client.GenderCode == cln.Code) ((RadioButton)Gender.Children[i]).IsChecked = true;
                }
            }
            catch { };


            if (client.ID == 0)
            {
                btnDelAg.IsEnabled = false;
                btnWritHi.IsEnabled = false;
                btnDelHi.IsEnabled = false;
                btnUpdateAg.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnWriteAg.Visibility = Visibility.Collapsed;
                //Prosmotr();
                historyGrid.ItemsSource = client.Tag.ToList();
                if (client.PhotoPath.Length != 0)
                {
                    FileInfo fileInfN = new FileInfo("../.");
                    string pathn = fileInfN.DirectoryName;
                    pathn = pathn + "\\" + client.PhotoPath;
                    BitmapImage myBitmapImage1 = new BitmapImage();
                    myBitmapImage1.BeginInit();
                    myBitmapImage1.UriSource = new Uri(@pathn, UriKind.Absolute);
                    myBitmapImage1.EndInit();
                    PhotoS.Source = myBitmapImage1;
                }
            }

            try
            {
                tg.Clear();
                tg = MainWindow.HelpClass.GetContext().Tag.ToList();
            }
            catch { };

            foreach (Tag tag in tg)
            {
                TextBlock block = new TextBlock();
                block.Text = tag.Title;
                block.Tag = tag.ID;
                var converter = new System.Windows.Media.BrushConverter();
                SolidColorBrush hb = (SolidColorBrush)(Brush)converter.ConvertFromString("#" + tag.Color);
                block.Foreground = hb;
                product.Items.Add(block);
            }

        }

        private void btnWritHi_Click(object sender, RoutedEventArgs e)
        {
            if (product.SelectedItem != null)
            {
                string text = ((TextBlock)product.SelectedItem).Text;
                int id = Convert.ToInt32(((TextBlock)product.SelectedItem).Tag);
                Tag tag = tg.Find(item => item.ID == id);
                client.Tag.Add(tag);
                historyGrid.ItemsSource = client.Tag.ToList();

            }
            else
            {
                MessageBox.Show("Выберите тэг!");
            }
        }

        private void btnDelHi_Click(object sender, RoutedEventArgs e)
        {
            if (historyGrid.SelectedItems.Count > 0)
            {
                Tag tag = historyGrid.SelectedItems[0] as Tag;
                client.Tag.Remove(tag);
                historyGrid.ItemsSource = client.Tag.ToList();
            }
        }

        private void product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = ((TextBlock)product.SelectedItem).Text;
            int id = Convert.ToInt32(((TextBlock)product.SelectedItem).Tag);
        }

        private void historyGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Tag tag = (Tag)e.Row.DataContext;
            var converter = new System.Windows.Media.BrushConverter();
            SolidColorBrush hb = (SolidColorBrush)(Brush)converter.ConvertFromString("#" + tag.Color);
            e.Row.Foreground = hb;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            client.GenderCode = ((RadioButton)sender).Tag.ToString();
        }

        private void btnDelAg_Click(object sender, RoutedEventArgs e)
        {
            if (client.ClientService.Count > 0)
            {
                MessageBox.Show("Информация о клиенте не может быть удалена");
                return;
            }
            MainWindow.HelpClass.GetContext().Client.Remove(client);
            MainWindow.HelpClass.GetContext().SaveChanges();
            MessageBox.Show("Удаление информации об клиенте завешено!");
            this.NavigationService.GoBack();

        }

        private void btnUpdateAg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( this.LastName.Text!= null && Patronymic.Text != null && FirstName.Text != null && Email.Text != null)
                {

                    MainWindow.HelpClass.GetContext().Entry(client).State = EntityState.Modified;
                    MainWindow.HelpClass.GetContext().SaveChanges();
                    MessageBox.Show("Обновление информации об агенте завершено");
                }
                else
                {
                    MessageBox.Show("Заполните пустые поля!");

                }

            }
            catch
            {
                return;
            }
        }

        private void btnWriteAg_Click(object sender, RoutedEventArgs e)
        {

       
            //if (client.RegistrationDate == DateTime.MinValue || client.RegistrationDate.ToString().Length == 0) return;
            //if ((client.Email != null) && (client.Email != "") && (!(new Regex(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)")).IsMatch(client.Email))) return;


            try
            {
                if (this.LastName.Text != null && Patronymic.Text != null && FirstName.Text != null && Email.Text != null && Phone.Text != null)
                {
                    if (Gender1.IsChecked == true || Gender2.IsChecked == true)
                    {
                        if ((new Regex(@"^\+?\d{0,2}\-?\d{3}\-?\d{3}\-?\d{4}")).IsMatch(Phone.Text))
                        {
                            if ((new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")).IsMatch(Email.Text))
                            {
                                MainWindow.HelpClass.GetContext().Client.Add(client);
                                MainWindow.HelpClass.GetContext().SaveChanges();
                                MessageBox.Show("Добавление информации об агенте завершено!");


                                btnDelAg.IsEnabled = true;
                                btnWritHi.IsEnabled = true;
                                btnDelHi.IsEnabled = true;
                            }
                            else MessageBox.Show("Неверный адрес электронной почты! Email должен иметь формат: x@gmail.com");
                        }
                        else MessageBox.Show("Неправильный формат телефона!");
                    }
                    else MessageBox.Show("Выберите пол!");
                }
                else
                {
                    MessageBox.Show("Заполните пустые поля!");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage myBitmapImage1 = new BitmapImage();
                string path = openFileDialog.FileName;
                string filename = openFileDialog.SafeFileName;
                myBitmapImage1.BeginInit();
                myBitmapImage1.UriSource = new Uri(@openFileDialog.FileName, UriKind.Absolute);
                myBitmapImage1.EndInit();
                FileInfo fileInf = new FileInfo(@path);
                if (fileInf.Length > 2000000)
                {
                    MessageBox.Show("Размер файла больше 2 М");
                    return;
                }
                Photo.Source = myBitmapImage1;
                string message = "Сохранить изображение?";
                string caption = "Сохранение";
                MessageBoxButton buttons = MessageBoxButton.YesNo;

                if (MessageBox.Show(message, caption, buttons, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    FileInfo fileInfN = new FileInfo("../.");
                    string pathn = fileInfN.DirectoryName;
                    string tagpath = (pathn + "\\images\\" + filename);
                    try
                    {
                        fileInf.CopyTo(@tagpath, true);
                    }
                    catch { };
                    PhotoS.Source = Photo.Source;
                    Photo.Source = null;
                    client.PhotoPath = "\\images\\" + filename;
                }
                else
                {
                    Photo.Source = null;
                };
            }
        }
    }
}
