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
    /// Логика взаимодействия для AddClientDialog.xaml
    /// </summary>
    public partial class AddClientDialog : Window
    {
        bool editMode;
        Client currentClient;

        public AddClientDialog()
        {
            InitializeComponent();

            DeleteButton.IsEnabled = false;
        }

        public AddClientDialog(Client client)
        {
            InitializeComponent();

            ClientFNameTextBox.Text = client.FName;
            ClientSNameTextBox.Text = client.SName;
            ClientTNameTextBox.Text = client.TName;
            ClientPhoneTextBox.Text = client.Phone;

            currentClient = client;
            editMode = true;
        }

        void Save()
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                Client client;

                if (!editMode)
                    db.Client.Add(client = new Client());
                else
                    client = db.Client.Where(o => o.ID == currentClient.ID).FirstOrDefault();

                client.FName = ClientFNameTextBox.Text;
                client.SName = ClientSNameTextBox.Text;
                client.TName = ClientTNameTextBox.Text;
                client.Phone = ClientPhoneTextBox.Text;

                db.SaveChanges();
            }
        }

        void Delete()
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                Client client;

                if (editMode)
                {
                    client = db.Client.Where(o => o.ID == currentClient.ID).FirstOrDefault();

                    client.Deleted = true;

                    db.SaveChanges();
                }
            }
        }

        void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
            Close();
        }

        void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Delete();
            Close();
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
