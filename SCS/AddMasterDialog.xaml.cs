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
    /// Логика взаимодействия для AddMasterDialog.xaml
    /// </summary>
    public partial class AddMasterDialog : Window
    {
        bool editMode;
        Master currentMaster;

        public AddMasterDialog()
        {
            InitializeComponent();

            DeleteButton.IsEnabled = false;
        }

        public AddMasterDialog(Master master)
        {
            InitializeComponent();

            MasterFNameTextBox.Text = master.FName;
            MasterSNameTextBox.Text = master.SName;
            MasterTNameTextBox.Text = master.TName;
            MasterPhoneTextBox.Text = master.Phone;
            MasterAddressTextBox.Text = master.Address;

            currentMaster = master;
            editMode = true;
        }

        void Save()
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                Master Master;

                if (!editMode)
                    db.Master.Add(Master = new Master());
                else
                    Master = db.Master.Where(o => o.ID == currentMaster.ID).FirstOrDefault();

                Master.FName = MasterFNameTextBox.Text;
                Master.SName = MasterSNameTextBox.Text;
                Master.TName = MasterTNameTextBox.Text;
                Master.Phone = MasterPhoneTextBox.Text;
                Master.Address = MasterAddressTextBox.Text;

                db.SaveChanges();
            }
        }

        void Delete()
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                Master Master;

                if (editMode)
                {
                    Master = db.Master.Where(o => o.ID == currentMaster.ID).FirstOrDefault();

                    Master.Deleted = true;

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
