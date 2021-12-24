using LiveCharts;
using LiveCharts.Wpf;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;


namespace SCS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public DockPanel currentPanel;
        Window dialog = null;
        Task currentTask;
        Orders currentOrder;
        Client currentClient;
        Device currentDevice;
        Spare currentSpare;
        Manager currentManager;
        Master currentMaster;
        Service currentService;
        public DataGrid currentDataGrid;
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public List<string> Labeles = new List<string>();
        public Func<double, string> Formatter { get; set; }
        ModelSCSDB db = new ModelSCSDB();

        public MainWindow(Manager manager) 
        {
            InitializeComponent();

            currentDataGrid = OrderGrid;

            currentManager = manager;
            currentDataGrid.SelectedCellsChanged += SetButtonText;
            CurrentUserInfo.CurrentUser.Content = CurrentUserInfo.UserPicture.DisplayName = currentManager.ToString();
            currentPanel = OrderPanel;

            DataContext = this;

            ShowGrid(AnalyticsPanel);
        }

        void SetButtonText(object sender, SelectedCellsChangedEventArgs e)
        {
            if (currentDataGrid.SelectedIndex == -1)
                AddButton.Content = "Add";
            else
                AddButton.Content = "Edit";
        }

        void AnalyticsGridButton_1_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(AnalyticsPanel);
        }

        void TaskGridButton_2_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(TaskPanel);
        }

        void OrderGridButton_3_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(OrderPanel);
        }

        void ClientGridButton_4_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(ClientPanel);
        }

        void DeviceGridButton_5_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(DevicePanel);
        }

        void SpareGridButton_6_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(SparePanel);
        }

        void EmployeeGridButton_7_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(EmployeePanel);
        }

        void ServiceGridButton_8_Click(object sender, RoutedEventArgs e)
        {
            ShowGrid(ServicePanel);
        }


        void ClientGrid_Loaded(object sender, RoutedEventArgs e)
        {
            TaskGrid.ItemsSource = db.Task.Where(o => o.Deleted != true).ToList();
            OrderGrid.ItemsSource = db.Orders.Where(o => o.Deleted != true).ToList();
            ClientGrid.ItemsSource = db.Client.Where(o => o.Deleted != true).ToList();
            DeviceGrid.ItemsSource = db.Device.Where(o => o.Deleted != true).ToList();
            SpareGrid.ItemsSource = db.Spare.Where(o => o.Deleted != true).ToList();
            MasterGrid.ItemsSource = db.Master.Where(o => o.Deleted != true).ToList();
            ServiceGrid.ItemsSource = db.Service.Where(o => o.Deleted != true).ToList();
        }

        void ShowGrid(DockPanel currentPanel)
        {
            PrintDocButton.Visibility = currentPanel.Name == "OrderPanel" || currentPanel.Name == "ServicePanel" ? Visibility.Visible : Visibility.Hidden;
            AddButton.Visibility = SearchPanel.Visibility = currentPanel.Name != "AnalyticsPanel" ? Visibility.Visible : Visibility.Hidden;

            foreach (var item in ContentGrid.Children)
            {
                if (item is DockPanel && (item as DockPanel).Name != "SearchPanel")
                    (item as DockPanel).Visibility = Visibility.Hidden;

            }
            currentPanel.Visibility = Visibility.Visible;
            this.currentPanel = currentPanel;
            currentDataGrid.SelectedCellsChanged += SetButtonText;
            switch (currentPanel.Name)
            {
                case "AnalyticsPanel":
                    PanelName.Content = "Аналитика";
                    SetActivePanel(AnalyticsGridButton_1, "analytics-active.png");
                    break;
                case "TaskPanel":
                    currentDataGrid = TaskGrid;
                    PanelName.Content = "Задачи";
                    SetActivePanel(TaskGridButton_2, "task-active.png");
                    break;
                case "OrderPanel":
                    currentDataGrid = OrderGrid;
                    PanelName.Content = "Заказы";
                    SetActivePanel(OrderGridButton_3, "order-active.png");
                    break;
                case "ClientPanel":
                    currentDataGrid = ClientGrid;
                    PanelName.Content = "Клиенты";
                    SetActivePanel(ClientGridButton_4, "client-active.png");
                    break;
                case "DevicePanel":
                    currentDataGrid = DeviceGrid;
                    PanelName.Content = "Устройства";
                    SetActivePanel(DeviceGridButton_5, "device-active.png");
                    break;
                case "SparePanel":
                    currentDataGrid = SpareGrid;
                    PanelName.Content = "Запчасти";
                    SetActivePanel(SpareGridButton_6, "spare-active.png");
                    break;
                case "EmployeePanel":
                    currentDataGrid = MasterGrid;
                    PanelName.Content = "Работники";
                    SetActivePanel(EmployeeGridButton_7, "employee-active.png");
                    break;
                case "ServicePanel":
                    currentDataGrid = ServiceGrid;
                    PanelName.Content = "Услуги";
                    SetActivePanel(ServiceGridButton_8, "service-active.png");
                    break;
            }
            using (ModelSCSDB db = new ModelSCSDB())
            {
                RefreshChart(db.Orders.ToList());
            }
        }

        void SetActivePanel(Button currentBtn, string image)
        {
            Uri resourceUri = new Uri($@"Images\{image}", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;
            TransformGroup tg = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            st.CenterX = 0.5;
            st.CenterY = 0.5;
            st.ScaleX = 0.4;
            st.ScaleY = 0.4;
            tg.Children.Add(st);

            brush.RelativeTransform = tg;
            SetImage();
            currentBtn.Background = brush;
        }

        void SetImage()
        {
            string[] images = { "analytics.png", "task.png", "order.png", "client.png", "device.png", "spare.png", "employee.png", "service.png" };
            Uri[] resourceUri = new Uri[8];
            StreamResourceInfo[] streamInfo = new StreamResourceInfo[8];
            BitmapFrame[] temp = new BitmapFrame[8];
            ImageBrush[] brush = new ImageBrush[8];
            TransformGroup tg = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            st.CenterX = 0.5;
            st.CenterY = 0.5;
            st.ScaleX = 0.4;
            st.ScaleY = 0.4;
            tg.Children.Add(st);
            for (int i = 0; i < images.Length; i++)
            {
                resourceUri[i] = new Uri($@"Images\{images[i]}", UriKind.Relative);
                streamInfo[i] = Application.GetResourceStream(resourceUri[i]);
                temp[i] = BitmapFrame.Create(streamInfo[i].Stream);
                brush[i] = new ImageBrush();
                brush[i].ImageSource = temp[i];
                brush[i].RelativeTransform = tg;
            }

            AnalyticsGridButton_1.Background = brush[0];
            TaskGridButton_2.Background = brush[1];
            OrderGridButton_3.Background = brush[2];
            ClientGridButton_4.Background = brush[3];
            DeviceGridButton_5.Background = brush[4];
            SpareGridButton_6.Background = brush[5];
            EmployeeGridButton_7.Background = brush[6];
            ServiceGridButton_8.Background = brush[7];
        }

        void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (dialog != null)
                dialog.Close();
            switch (currentPanel.Name)
            {
                case "TaskPanel":
                    dialog = (currentTask != null) ? new AddTaskDialog(currentTask, currentManager) : new AddTaskDialog(currentManager);
                    break;
                case "OrderPanel":
                    dialog = (currentOrder != null) ? new AddOrderDialog(currentOrder, currentManager) : new AddOrderDialog(currentManager);
                    break;
                case "ClientPanel":
                    dialog = (currentClient != null) ? new AddClientDialog(currentClient) : new AddClientDialog();
                    break;
                case "DevicePanel":
                    dialog = (currentDevice != null) ? new AddDeviceDialog(currentDevice) : new AddDeviceDialog();
                    break;
                case "SparePanel":
                    dialog = (currentSpare != null) ? new AddSpareDialog(currentSpare) : new AddSpareDialog();
                    break;
                case "EmployeePanel":
                    dialog = (currentMaster != null) ? new AddMasterDialog(currentMaster) : new AddMasterDialog();
                    break;
                case "ServicePanel":
                    dialog = (currentService != null) ? new AddServiceDialog(currentService) : new AddServiceDialog();
                    break;
                default:
                    dialog = null;
                    break;
            }
            if (dialog != null)
                dialog.Show();
            dialog.Closed += DialogClose;
        }

        void DialogClose(object sender, EventArgs e)
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                TaskGrid.ItemsSource = db.Task.Where(o => o.Deleted != true).ToList();
                OrderGrid.ItemsSource = db.Orders.Where(o => o.Deleted != true).ToList();
                ClientGrid.ItemsSource = db.Client.Where(o => o.Deleted != true).ToList();
                DeviceGrid.ItemsSource = db.Device.Where(o => o.Deleted != true).ToList();
                SpareGrid.ItemsSource = db.Spare.Where(o => o.Deleted != true).ToList();
                MasterGrid.ItemsSource = db.Master.Where(o => o.Deleted != true).ToList();
                ServiceGrid.ItemsSource = db.Service.Where(o => o.Deleted != true).ToList();
            }

            currentDataGrid.SelectedIndex = -1;
        }

        void TaskGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             
            currentTask = TaskGrid.SelectedItem as Task;
        }

        void OrderGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentOrder = OrderGrid.SelectedItem as Orders;
        }

        void ClientGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentClient = ClientGrid.SelectedItem as Client;
        }

        void MasterGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentMaster = MasterGrid.SelectedItem as Master;
        }

        void DeviceGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentDevice = DeviceGrid.SelectedItem as Device;
        }

        void SpareGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSpare = SpareGrid.SelectedItem as Spare;
        }

        void ServiceGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentService = ServiceGrid.SelectedItem as Service;
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dialog != null)
                dialog.Close();
            dialog = (currentManager != null) ? new AddManagerDialog(currentManager) : new AddManagerDialog();
            if (dialog != null)
            {
                Close();
                dialog.Show();
            }
            dialog.Closing += SetCurrentManager;
            
        }

        void SetCurrentManager(object sender, System.ComponentModel.CancelEventArgs e)
        {
            currentManager = (dialog as AddManagerDialog).currentManager;
            dialog.Closing -= SetCurrentManager;
        }

        void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                if (currentDataGrid != null)
                {
                    currentDataGrid.SelectedItems.Clear();
                    if (!string.IsNullOrEmpty(SearchBox.Text.ToLower()))
                    {
                        switch (currentDataGrid.Name)
                        {
                            case "TaskGrid":
                                foreach (Task task in TaskGrid.ItemsSource)
                                    if (task.Manager.FName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        task.Manager.SName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        task.Manager.TName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        task.Master.FName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        task.Master.SName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        task.Master.TName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        task.Status.Name.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        task.TaskText.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        task.TermDate.ToString().ToLower().Contains(SearchBox.Text.ToLower()))
                                        TaskGrid.SelectedItems.Add(task);
                                break;

                            case "OrderGrid":
                                foreach (Orders order in OrderGrid.Items)
                                    if (order.Manager.FName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Manager.SName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Manager.TName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Master.FName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Master.SName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Master.TName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Device.Client.FName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Device.Client.SName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Device.Client.TName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Status.Name.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Device.Name.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Device.Brand.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Device.Model.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Device.SerialNumber.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Device.Defect.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.Price.ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        order.OrderDate.ToString().ToLower().Contains(SearchBox.Text.ToLower()))
                                        OrderGrid.SelectedItems.Add(order);
                                break;
                            case "ClientGrid":
                                foreach (Client client in ClientGrid.Items)
                                    if (client.FName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        client.SName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        client.TName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        client.Phone.ToLower().Contains(SearchBox.Text.ToLower()))
                                        ClientGrid.SelectedItems.Add(client);
                                break;
                            case "DeviceGrid":
                                foreach (Device device in DeviceGrid.Items)
                                    if (device.Client.FName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        device.Client.SName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        device.Client.TName[0].ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        device.Name.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        device.Brand.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        device.Model.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        device.SerialNumber.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        device.Defect.ToLower().Contains(SearchBox.Text.ToLower()))
                                        DeviceGrid.SelectedItems.Add(device);
                                break;
                            case "SpareGrid":
                                foreach (Spare spare in SpareGrid.Items)
                                    if (spare.Name.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        spare.Brand.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        spare.Model.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        spare.Counts.ToString().Contains(SearchBox.Text.ToLower()) ||
                                        spare.Provider.ToLower().Contains(SearchBox.Text.ToLower()))
                                        SpareGrid.SelectedItems.Add(spare);
                                break;
                            case "MasterGrid":
                                foreach (Master master in MasterGrid.Items)
                                    if (master.FName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        master.SName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        master.TName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        master.Phone.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        master.Address.ToLower().Contains(SearchBox.Text.ToLower()))
                                        MasterGrid.SelectedItems.Add(master);
                                break;
                            case "ServiceGrid":
                                foreach (Service service in ServiceGrid.Items)
                                    if (service.Name.ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        service.Price.ToString().ToLower().Contains(SearchBox.Text.ToLower()) ||
                                        service.Description.ToLower().Contains(SearchBox.Text.ToLower()))
                                        ServiceGrid.SelectedItems.Add(service);
                                break;
                        }
                    }
                }
            }
        }

        void PrintDocButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPanel.Name == "OrderPanel")
            {
                try
                {
                    List<ServiceOrder> servicesList = new List<ServiceOrder>();
                    servicesList = db.ServiceOrder.Where(o => o.OrderID == currentOrder.ID).ToList();

                    Dictionary<int, object> services = new Dictionary<int, object>();

                    for (int i = 0; i < servicesList.Count; i++)
                    {
                        services.Add(i, servicesList[i]);
                    }
                    WordHelper wordHelper = new WordHelper(currentOrder, services);
                }
                catch
                {
                    MessageBox.Show("Заказ не выбран.");
                }
            }

            if (currentPanel.Name == "ServicePanel")
            {
                try
                {
                    using (ExcelHelper helper = new ExcelHelper())
                    {
                        if (helper.Open(filePath: System.IO.Path.Combine(@"C:\Users\MSI\Desktop", "Прайслист" + ".xlsx")))
                        {
                            for (int i = 2; i < db.Service.ToList().Count + 2; i++)
                            {
                                Service service = db.Service.ToList()[i - 2];
                                helper.Set(column: "A", row: 1, data: "№");
                                helper.Set(column: "B", row: 1, data: "Услуга");
                                helper.Set(column: "C", row: 1, data: "Цена");
                                helper.Set(column: "A", row: i, data: i - 1);
                                helper.Set(column: "B", row: i, data: service.Name);
                                helper.Set(column: "C", row: i, data: service.Price);
                            }

                            helper.Save();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                if (currentDataGrid != null)
                {
                    currentDataGrid.SelectedItems.Clear();
                    switch (currentDataGrid.Name)
                    {
                        case "TaskGrid":
                            var filteredTask = db.Task.Where(task => task.Manager.FName.Contains(FilterBox.Text) ||
                                                             task.Manager.SName.Contains(FilterBox.Text) ||
                                                             task.Manager.TName.Contains(FilterBox.Text) ||
                                                             task.Master.FName.Contains(FilterBox.Text) ||
                                                             task.Master.SName.Contains(FilterBox.Text) ||
                                                             task.Master.TName.Contains(FilterBox.Text) ||
                                                             task.Status.Name.Contains(FilterBox.Text) ||
                                                             task.TaskText.Contains(FilterBox.Text) &&
                                                             task.Deleted != true).ToList();
                            TaskGrid.ItemsSource = filteredTask;
                            break;

                        case "OrderGrid":
                            var filteredOrders = db.Orders.Where(order => order.Manager.FName.Contains(FilterBox.Text) ||
                                                                 order.Manager.SName.Contains(FilterBox.Text) ||
                                                                 order.Manager.TName.Contains(FilterBox.Text) ||
                                                                 order.Master.FName.Contains(FilterBox.Text) ||
                                                                 order.Master.SName.Contains(FilterBox.Text) ||
                                                                 order.Master.TName.Contains(FilterBox.Text) ||
                                                                 order.Device.Client.FName.Contains(FilterBox.Text) ||
                                                                 order.Device.Client.SName.Contains(FilterBox.Text) ||
                                                                 order.Device.Client.TName.Contains(FilterBox.Text) ||
                                                                 order.Status.Name.Contains(FilterBox.Text) ||
                                                                 order.Device.Name.Contains(FilterBox.Text) ||
                                                                 order.Device.Brand.Contains(FilterBox.Text) ||
                                                                 order.Device.Model.Contains(FilterBox.Text) ||
                                                                 order.Device.SerialNumber.Contains(FilterBox.Text) ||
                                                                 order.Device.Defect.Contains(FilterBox.Text) &&
                                                                 order.Deleted != true).ToList();
                            OrderGrid.ItemsSource = filteredOrders;
                            break;
                        case "ClientGrid":
                            var filteredClient = db.Client.Where(client => client.FName.Contains(FilterBox.Text) ||
                                                                 client.SName.Contains(FilterBox.Text) ||
                                                                 client.TName.Contains(FilterBox.Text) ||
                                                                 client.Phone.Contains(FilterBox.Text) &&
                                                                 client.Deleted != true);
                            ClientGrid.ItemsSource = filteredClient;
                            break;
                        case "DeviceGrid":
                            var filteredDevice = db.Device.Where(device => device.Client.FName.Contains(FilterBox.Text) ||
                                                                 device.Client.SName.Contains(FilterBox.Text) ||
                                                                 device.Client.TName.Contains(FilterBox.Text) ||
                                                                 device.Name.Contains(FilterBox.Text) ||
                                                                 device.Brand.Contains(FilterBox.Text) ||
                                                                 device.Model.Contains(FilterBox.Text) ||
                                                                 device.SerialNumber.Contains(FilterBox.Text) ||
                                                                 device.Defect.Contains(FilterBox.Text) &&
                                                                 device.Deleted != true).ToList();
                            DeviceGrid.ItemsSource = filteredDevice;
                            break;
                        case "SpareGrid":
                            var filteredSpare = db.Spare.Where(spare => spare.Name.Contains(FilterBox.Text) ||
                                                               spare.Brand.Contains(FilterBox.Text) ||
                                                               spare.Model.Contains(FilterBox.Text) ||
                                                               spare.Provider.Contains(FilterBox.Text) &&
                                                               spare.Deleted != true).ToList();
                            SpareGrid.ItemsSource = filteredSpare;
                            break;
                        case "MasterGrid":
                            var filteredMaster = db.Master.Where(master => master.FName.Contains(FilterBox.Text) ||
                                                                 master.SName.Contains(FilterBox.Text) ||
                                                                 master.TName.Contains(FilterBox.Text) ||
                                                                 master.Phone.Contains(FilterBox.Text) ||
                                                                 master.Address.Contains(FilterBox.Text) &&
                                                                 master.Deleted != true).ToList();
                            MasterGrid.ItemsSource = filteredMaster;
                            break;
                        case "ServiceGrid":
                            var filteredService = db.Service.Where(service => service.Name.Contains(FilterBox.Text) ||
                                                                   service.Description.Contains(FilterBox.Text) &&
                                                                   service.Deleted != true).ToList();
                            ServiceGrid.ItemsSource = filteredService;
                            break;
                    }
                }
            }
            
        }

        void SearchBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SearchBox.Text == "Поиск...")
                SearchBox.Text = "";
            SearchBox.SelectAll();
        }

        void FilterBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (FilterBox.Text == "Фильтрация...")
                FilterBox.Text = "";
            FilterBox.SelectAll();
        }

        void MainClass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                currentDataGrid.SelectedIndex = -1;
            }
        }

        void RefreshChart(List<Orders> orders)
        {
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Заказы",
                    Values = new ChartValues<int> { }
                }
            };

            foreach (Orders order in orders)
            {
                if (!Labeles.Contains(order.OrderDate.ToString("dd.MM.yyyy")))
                    Labeles.Add(order.OrderDate.ToString("dd.MM.yyyy"));
            }

            foreach (string date in Labeles)
            {
                int k = 0;
                foreach (Orders order in orders)
                {
                    if (order.OrderDate.ToString("dd.MM.yyyy") == date)
                        ++k;
                }
                SeriesCollection[0].Values.Add(k);
            }


            Labeles.Sort();
            Labels = Labeles.ToArray();
            Formatter = value => value.ToString("N");

            for (int i = 0; i < OrdersChart.Series.Count; i++)
            {
                double k = 0;
                foreach (Orders order in orders)
                {
                    if (order.Status.Name == OrdersChart.Series[i].Title)
                        ++k;
                }
                OrdersChart.Series[i].Values[0] = k;
            }
        }

        void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate != null && EndtDatePicker.SelectedDate != null)
                RefreshPieChart(db.Orders.ToList(), (DateTime)StartDatePicker.SelectedDate, (DateTime)EndtDatePicker.SelectedDate);
            else if (StartDatePicker.SelectedDate != null && EndtDatePicker.SelectedDate == null)
                RefreshPieChart(db.Orders.ToList(), (DateTime)StartDatePicker.SelectedDate, DateTime.Now);
            else if (StartDatePicker.SelectedDate == null && EndtDatePicker.SelectedDate != null)
                RefreshPieChart(db.Orders.ToList(), DateTime.MinValue, (DateTime)EndtDatePicker.SelectedDate);
            else
                RefreshPieChart(db.Orders.ToList(), DateTime.MinValue, DateTime.Now);
        }

        void RefreshPieChart(List<Orders> orders, DateTime startDate, DateTime endDate)
        {
            for (int i = 0; i < OrdersChart.Series.Count; i++)
            {
                double k = 0;
                foreach (Orders order in orders)
                {
                    if (order.Status.Name == OrdersChart.Series[i].Title && order.OrderDate > startDate && order.OrderDate < endDate)
                        ++k;
                }
                OrdersChart.Series[i].Values[0] = k;
            }
        }
    }
}
