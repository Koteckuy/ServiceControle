using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для AddOrderDialog.xaml
    /// </summary>
    public partial class AddOrderDialog : Window
    {
        bool editMode;
        Orders currentOrder;
        Manager currentManager;
        List<Service> services = new List<Service>();
        List<Spare> spares = new List<Spare>();
        ModelSCSDB db = new ModelSCSDB();
        double price = 0;

        public AddOrderDialog(Manager manager)
        {
            InitializeComponent();

            OrderClientComboBox.ItemsSource = db.Client.Where(o => o.Deleted != true).ToList();
            OrderDeviceComboBox.ItemsSource = db.Device.Where(o => o.Deleted != true).ToList();
            OrderMasterComboBox.ItemsSource = db.Master.Where(o => o.Deleted != true).ToList();
            OrderStatusComboBox.ItemsSource = db.Status.ToList();
            OrderServiceComboBox.ItemsSource = db.Service.Where(o => o.Deleted != true).ToList();
            OrderSpareComboBox.ItemsSource = db.Spare.Where(o => o.Deleted != true).ToList();

            currentManager = manager;

            DeleteButton.IsEnabled = false;
        }

        public AddOrderDialog(Orders order, Manager manager)
        {
            InitializeComponent();

            if (order.Device.Client.Deleted != true)
                OrderClientComboBox.ItemsSource = db.Client.Where(o => o.Deleted != true).ToList();
            else
            {
                OrderClientComboBox.ItemsSource = db.Client.ToList();
                OrderClientComboBox.IsEnabled = false;
                AddClientButton.IsEnabled = false;
            }
            if (order.Device.Deleted != true)
                OrderDeviceComboBox.ItemsSource = db.Device.Where(o => o.Deleted != true).ToList();
            else
            {
                OrderDeviceComboBox.ItemsSource = db.Device.ToList();
                OrderDeviceComboBox.IsEnabled = false;
                AddDeviceButton.IsEnabled = false;
            }
            if (order.Master.Deleted != true)
                OrderMasterComboBox.ItemsSource = db.Master.Where(o => o.Deleted != true).ToList();
            else
            {
                OrderMasterComboBox.ItemsSource = db.Master.ToList();
                OrderMasterComboBox.IsEnabled = false;
                AddMasterButton.IsEnabled = false;
            }

            OrderStatusComboBox.ItemsSource = db.Status.ToList();

            OrderClientComboBox.SelectedItem = db.Client.SqlQuery("SELECT * " +
                                                                    "FROM Client, Device " +
                                                                    "WHERE Client.ID = Device.ClientID AND Device.ID = @deviceID", new SqlParameter("@deviceID", order.DeviceID)).ToList()[0];
            OrderDeviceComboBox.SelectedItem = db.Device.Where(o => o.ID == order.DeviceID).FirstOrDefault();
            OrderMasterComboBox.SelectedItem = db.Master.Where(o => o.ID == order.MasterID).FirstOrDefault();
            OrderStatusComboBox.SelectedItem = db.Status.Where(o => o.ID == order.StatusID).FirstOrDefault();

            OrderServiceComboBox.ItemsSource = db.Service.Where(o => o.Deleted != true).ToList();
            OrderSpareComboBox.ItemsSource = db.Spare.Where(o => o.Deleted != true).ToList();



            ServiceOrderGrid.ItemsSource = services = db.Service.SqlQuery("SELECT * " +
                                                                "FROM Service, Orders, ServiceOrder " +
                                                                "WHERE ServiceID = Service.ID AND OrderID = Orders.ID AND OrderID = @orderID", new SqlParameter("@orderID", order.ID)).ToList();
            SpareOrderGrid.ItemsSource = spares = db.Spare.SqlQuery("SELECT * " +
                                                            "FROM Spare, Orders, SpareOrder " +
                                                            "WHERE SpareID = Spare.ID AND OrderID = Orders.ID AND OrderID = @orderID", new SqlParameter("@orderID", order.ID)).ToList();

            price = order.Price;
            currentOrder = order;
            currentManager = manager;
            editMode = true;
            CalcPrice();
        }


        void Save()
        {
            Orders order;

            if (!editMode)
            {
                db.Orders.Add(order = new Orders());
                order.ManagerID = currentManager.ID;
            }
            else
                order = db.Orders.Where(o => o.ID == currentOrder.ID).FirstOrDefault();

            order.MasterID = (OrderMasterComboBox.SelectedItem as Master).ID;
            order.StatusID = (OrderStatusComboBox.SelectedItem as Status).ID;
            order.DeviceID = (OrderDeviceComboBox.SelectedItem as Device).ID;
            order.OrderDate = (editMode) ? order.OrderDate : DateTime.Now;
            order.Price = price;

            var deletedServiceOrders = db.ServiceOrder.Where(o => o.OrderID == order.ID).ToList();
            db.ServiceOrder.RemoveRange(deletedServiceOrders);

            if (services.Count != 0)
                foreach (Service service in services)
                {
                    db.ServiceOrder.Add(new ServiceOrder() 
                    { 
                        OrderID = order.ID,
                        ServiceID = service.ID,
                        Deleted = false
                    });
                }

            var deletedSpareOrders = db.SpareOrder.Where(o => o.OrderID == order.ID).ToList();
            db.SpareOrder.RemoveRange(deletedSpareOrders);

            if (spares.Count != 0)
                foreach (Spare spare in spares)
                {
                    db.SpareOrder.Add(new SpareOrder()
                    {
                        OrderID = order.ID,
                        SpareID = spare.ID,
                        Deleted = false
                    });
                }

            db.SaveChanges();
        }
        void Delete()
        {
            Orders order;

            if (editMode)
            {
                order = db.Orders.Where(o => o.ID == currentOrder.ID).FirstOrDefault();

                order.Deleted = true;

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

        void CalcPrice()
        {
            price = 0;
            foreach (Service service in services)
            {
                price += service.Price;
            }
            OrderPriceTextBox.Text = price.ToString();
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void OrderClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckDeviceClient();
        }

        void CheckDeviceClient()
        {
            Client client = OrderClientComboBox.SelectedItem as Client;
            OrderDeviceComboBox.ItemsSource = db.Device.Where(o => o.Deleted != true && o.ClientID == client.ID).ToList();
            try
            {
                OrderDeviceComboBox.SelectedItem = db.Device.Where(o => o.ClientID == client.ID).FirstOrDefault();
            }
            catch { }
        }

        void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            Service selectedService = OrderServiceComboBox.SelectedItem as Service;
            services.Add(db.Service.Where(o => o.ID == selectedService.ID).FirstOrDefault());
            ServiceOrderGrid.Items.Refresh();
            CalcPrice();
        }

        void DeleteServiceButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedService = ServiceOrderGrid.SelectedIndex;
            services.RemoveAt(selectedService);
            ServiceOrderGrid.Items.Refresh();
            CalcPrice();
        }

        void AddSpareButton_Click(object sender, RoutedEventArgs e)
        {
            Spare selectedSpare = OrderSpareComboBox.SelectedItem as Spare;
            spares.Add(db.Spare.Where(o => o.ID == selectedSpare.ID).FirstOrDefault());
            SpareOrderGrid.Items.Refresh();
        }

        void DeleteSpareButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedSpare = SpareOrderGrid.SelectedIndex;
            spares.RemoveAt(selectedSpare);
            SpareOrderGrid.Items.Refresh();
        }

        void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            AddClientDialog clientDialog = new AddClientDialog();
            clientDialog.Show();
            clientDialog.Closed += RefreshClientComboBox;
            CheckDeviceClient();    
        }

        void RefreshClientComboBox(object sender, EventArgs e)
        {
            OrderClientComboBox.ItemsSource = db.Client.Where(o => o.Deleted != true).ToList();
        }

        void AddDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            AddDeviceDialog DeviceDialog = new AddDeviceDialog();
            DeviceDialog.Show();
            DeviceDialog.Closed += RefreshDeviceComboBox;
            CheckDeviceClient();
        }

        void RefreshDeviceComboBox(object sender, EventArgs e)
        {
            OrderDeviceComboBox.ItemsSource = db.Device.Where(o => o.Deleted != true).ToList();
        }

        void AddMasterButton_Click(object sender, RoutedEventArgs e)
        {
            AddMasterDialog MasterDialog = new AddMasterDialog();
            MasterDialog.Show();
            MasterDialog.Closed += RefreshMasterComboBox;        }

        void RefreshMasterComboBox(object sender, EventArgs e)
        {
            OrderMasterComboBox.ItemsSource = db.Master.Where(o => o.Deleted != true).ToList();
        }
    }
}
