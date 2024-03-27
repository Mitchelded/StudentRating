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

namespace RatingStudents
{
    /// <summary>
    /// Логика взаимодействия для Window_Subjects.xaml
    /// </summary>
    public partial class Window_Subjects : Window
    {
        public Window_Subjects()
        {
            InitializeComponent();
        }

        private void miWindowStudents_Click(object sender, RoutedEventArgs e)
        {
            Window_Students window = new Window_Students();
            window.Show();
        }
    }
}
