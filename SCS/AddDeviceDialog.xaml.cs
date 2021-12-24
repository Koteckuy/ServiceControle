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
    /// Логика взаимодействия для AddDeviceDialog.xaml
    /// </summary>
    public partial class AddDeviceDialog : Window
    {
        bool editMode;
        Device currentDevice;
        ModelSCSDB db = new ModelSCSDB();

        public AddDeviceDialog()
        {
            InitializeComponent();

            DeleteButton.IsEnabled = false;

            DeviceClientComboBox.ItemsSource = db.Client.Where(o => o.Deleted != true).ToList();
        }

        public AddDeviceDialog(Device device)
        {
            InitializeComponent();

            if (device.Client.Deleted != true)
            {
                DeviceClientComboBox.ItemsSource = db.Client.Where(o => o.Deleted != true).ToList();
            }
            else
            {
                DeviceClientComboBox.ItemsSource = db.Client.ToList();
                DeviceClientComboBox.IsEnabled = false;
                AddClientButton.IsEnabled = false;
            }
            DeviceClientComboBox.SelectedItem = db.Client.Where(c => c.ID == device.ClientID).FirstOrDefault();
            DeviceNameTextBox.Text = device.Name;
            DeviceBrandTextBox.Text = device.Brand;
            DeviceModelTextBox.Text = device.Model;
            DeviceSerialNumberTextBox.Text = device.SerialNumber;
            DeviceDefectTextBox.Text = device.Defect;

            currentDevice = device;
            editMode = true;
        }

        void Save()
        {
            Device device;

            if (!editMode)
                db.Device.Add(device = new Device());
            else
                device = db.Device.Where(o => o.ID == currentDevice.ID).FirstOrDefault();

            device.ClientID = (DeviceClientComboBox.SelectedItem as Client).ID;
            device.Name = DeviceNameTextBox.Text;
            device.Brand = DeviceBrandTextBox.Text;
            device.Model = DeviceModelTextBox.Text;
            device.SerialNumber = DeviceSerialNumberTextBox.Text;
            device.Defect = DeviceDefectTextBox.Text;


            db.SaveChanges();
        }

        void Delete()
        {
            Device device;

            if (editMode)
            {
                device = db.Device.Where(o => o.ID == currentDevice.ID).FirstOrDefault();

                device.Deleted = true;

                db.SaveChanges();
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

        void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            AddClientDialog clientDialog = new AddClientDialog();
            clientDialog.Show();
            clientDialog.Closed += RefreshComboBox;
        }

        void RefreshComboBox(object sender, EventArgs e)
        {
            DeviceClientComboBox.ItemsSource = db.Client.Where(o => o.Deleted != true).ToList();
        }
    }
}
