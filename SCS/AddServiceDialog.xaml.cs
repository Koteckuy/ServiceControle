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
    /// Логика взаимодействия для AddServiceDialog.xaml
    /// </summary>
    public partial class AddServiceDialog : Window
    {
        bool editMode;
        Service currentService;

        public AddServiceDialog()
        {
            InitializeComponent();

            DeleteButton.IsEnabled = false;
        }

        public AddServiceDialog(Service service)
        {
            InitializeComponent();

            ServiceNameTextBox.Text         = service.Name;
            ServicePriceTextBox.Text        = service.Price.ToString();
            ServiceDescriptionTextBox.Text  = service.Description;

            currentService = service;
            editMode = true;
        }

        void Save()
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                Service service;

                if (!editMode)
                    db.Service.Add(service = new Service());
                else
                    service = db.Service.Where(o => o.ID == currentService.ID).FirstOrDefault();

                service.Name = ServiceNameTextBox.Text;
                service.Price = double.Parse(ServicePriceTextBox.Text);
                service.Description = ServiceDescriptionTextBox.Text;

                db.SaveChanges();
            }
        }

        void Delete()
        {
            using (ModelSCSDB db = new ModelSCSDB())
            {
                Service service;

                if (editMode)
                {
                    service = db.Service.Where(o => o.ID == currentService.ID).FirstOrDefault();

                    service.Deleted = true;

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
