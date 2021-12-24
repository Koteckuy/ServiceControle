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

namespace SCS
{
    /// <summary>
    /// Логика взаимодействия для AddManagerDialog.xaml
    /// </summary>
    public partial class AddManagerDialog : Window
    {
        bool editMode;
        public Manager currentManager { get; set; }

        public AddManagerDialog()
        {
            InitializeComponent();
        }

        public AddManagerDialog(Manager manager)
        {
            InitializeComponent();

            ManagerFNameTextBox.Text = manager.FName;
            ManagerSNameTextBox.Text = manager.SName;
            ManagerTNameTextBox.Text = manager.TName;
            ManagerPhoneTextBox.Text = manager.Phone;
            ManagerAddressTextBox.Text = manager.Address;

            currentManager = manager;
            editMode = true;
        }

        void Save()
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                Manager manager;

                if (!editMode)
                    db.Manager.Add(manager = new Manager());
                else
                    manager = db.Manager.Where(o => o.ID == currentManager.ID).FirstOrDefault();

                manager.FName = ManagerFNameTextBox.Text;
                manager.SName = ManagerSNameTextBox.Text;
                manager.TName = ManagerTNameTextBox.Text;
                manager.Phone = ManagerPhoneTextBox.Text;
                manager.Pass = ManagerPasswordTextBox.Text;
                manager.Address = ManagerAddressTextBox.Text;

                db.SaveChanges();
            }

            PhoneTextBox.Text = ManagerPhoneTextBox.Text;
            PasswordBox.Password = ManagerPasswordTextBox.Text;
            Cansel();
        }

        void Cansel()
        {
            ManagerFNameTextBox.Text = "";
            ManagerSNameTextBox.Text = "";
            ManagerTNameTextBox.Text = "";
            ManagerPhoneTextBox.Text = "";
            ManagerPasswordTextBox.Text = "";
            ManagerAddressTextBox.Text = "";

            RegistrationExpander.IsExpanded = false;
        }
        void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                if ((currentManager = db.Manager.Where(o => o.Phone == PhoneTextBox.Text && o.Pass == PasswordBox.Password).FirstOrDefault()) != null)
                {
                    MainWindow mainWindow = new MainWindow(currentManager);
                    mainWindow.Show();
                    Close();
                }
                else
                    MessageBox.Show("Неверное имя пользователя или пароль.");
            }
        }

        void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = 520;
            this.Top = 100;
        }

        private void RegistrationExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Left = 520;
            this.Top = 300;
        }

        void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            Cansel();
        }
    }
}
    