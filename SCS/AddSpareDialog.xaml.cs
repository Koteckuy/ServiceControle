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
    /// Логика взаимодействия для AddSpareDialog.xaml
    /// </summary>
    public partial class AddSpareDialog : Window
    {
        bool editMode;
        Spare currentSpare;
        ModelSCSDB db = new ModelSCSDB();

        public AddSpareDialog()
        {
            InitializeComponent();

            DeleteButton.IsEnabled = false;     
        }

        public AddSpareDialog(Spare spare)
        {
            InitializeComponent();

            SpareNameTextBox.Text = spare.Name;
            SpareBrandTextBox.Text = spare.Brand;
            SpareModelTextBox.Text = spare.Model;
            SpareProviderTextBox.Text = spare.Provider;

            currentSpare = spare;
            editMode = true;
        }

        void Save()
        {
            Spare spare;

            if (!editMode)
                db.Spare.Add(spare = new Spare());
            else
                spare = db.Spare.Where(o => o.ID == currentSpare.ID).FirstOrDefault();

            spare.Name = SpareNameTextBox.Text;
            spare.Brand = SpareBrandTextBox.Text;
            spare.Model = SpareModelTextBox.Text;
            spare.Provider = SpareProviderTextBox.Text;

            db.SaveChanges();
        }

        void Delete()
        {
            Spare spare;

            if (editMode)
            {
                spare = db.Spare.Where(o => o.ID == currentSpare.ID).FirstOrDefault();

                spare.Deleted = true;

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
    }
}
