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
    /// Логика взаимодействия для AddTaskDialog.xaml
    /// </summary>
    public partial class AddTaskDialog : Window
    {
        ModelSCSDB db = new ModelSCSDB();

        bool editMode;
        Task currentTask;
        Manager currentManager;

        public AddTaskDialog(Manager manager)
        {
            InitializeComponent();

            TaskMasterComboBox.ItemsSource = db.Master.Where(o => o.Deleted != true).ToList();
            TaskStatusComboBox.ItemsSource = db.Status.ToList();

            currentManager = manager;

            DeleteButton.IsEnabled = false;
        }

        public AddTaskDialog(Task task, Manager manager)
        {
            InitializeComponent();
            
            TaskMasterComboBox.ItemsSource = db.Master.Where(o => o.Deleted != true).ToList();
            TaskStatusComboBox.ItemsSource = db.Status.ToList();
            TaskMasterComboBox.SelectedItem = db.Master.Where(o => o.ID == task.MasterID).ToList()[0];
            TaskStatusComboBox.SelectedItem = db.Status.Where(o => o.ID == task.StatusID).ToList()[0];

            TaskDatePicker.SelectedDate = task.TermDate;
            TaskTextBox.Text = task.TaskText;
            currentTask = task;
            currentManager = manager;
            editMode = true;
        }

        void Save()
        {
            Task task;

            if (!editMode)
            {
                db.Task.Add(task = new Task());
                task.ManagerID = currentManager.ID;
            }
            else
                task = db.Task.Where(o => o.ID == currentTask.ID).FirstOrDefault();

            task.MasterID = (TaskMasterComboBox.SelectedItem as Master).ID;
            task.StatusID = (TaskStatusComboBox.SelectedItem as Status).ID;
            task.TermDate = TaskDatePicker.SelectedDate;
            task.TaskText = TaskTextBox.Text;

            db.SaveChanges();
        }

        void Delete()
        {
            Task task;

            if (editMode)
            {
                task = db.Task.Where(o => o.ID == currentTask.ID).FirstOrDefault();

                task.Deleted = true;

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
            AddMasterDialog masterDialog = new AddMasterDialog();
            masterDialog.Show();
            masterDialog.Closed += RefreshComboBox;
        }

        void RefreshComboBox(object sender, EventArgs e)
        {
            TaskMasterComboBox.ItemsSource = db.Master.Where(o => o.Deleted != true).ToList();
        }
    }
}
