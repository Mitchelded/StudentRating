using System;
using System.Collections.Generic;
using System.Data;
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
using Microsoft.Data.SqlClient;

namespace RatingStudents
{
    /// <summary>
    /// Логика взаимодействия для Window_Subjects.xaml
    /// </summary>
    public partial class Window_Subjects : Window
    {
        
        private const string SelectQuery = "SELECT * FROM dbo.Subjects";

        private const string InsertQuery = "INSERT INTO dbo.Subjects VALUES (@param1, @param2, @param3, @param4)";

        private const string UpdateQuery = "UPDATE dbo.Subjects SET course_name = @param1, description = @param2, " +
                                           "duration = @param3, instructor = @param4 " +
                                           "WHERE subject_id = @primaryKeyValue";

        private const string DeleteQuery = "DELETE FROM dbo.Subjects WHERE subject_id = @primaryKeyValue";
        private const string DeleteQueryChild = "DELETE FROM dbo.Ratings WHERE subject_id = @primaryKeyValue";

        private const string TruncateQuery = "Delete From dbo.Subjects";
        private const string TruncateQueryChild  = "Delete From dbo.Ratings";
        
        private readonly ConnectionDb _conn;
        public Window_Subjects()
        {
            InitializeComponent();
            _conn = new ConnectionDb();
            DataTable dataTable = _conn.GetDataTable(SelectQuery);
            Dg.ItemsSource = dataTable.DefaultView;
        }

        private void miWindowStudents_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, открыто ли уже окно Window_Students
            if (Application.Current.Windows.OfType<Window_Students>().Any())
            {
                // Окно уже открыто, необходимо активировать его
                Window_Students window = Application.Current.Windows.OfType<Window_Students>().First();
                window.Activate();
            }
            else
            {
                // Окно еще не открыто, создаем новый экземпляр и открываем его
                Window_Students window = new Window_Students();
                window.Show();
            }
        }

        private void MiSelect_OnClick(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = _conn.GetDataTable(SelectQuery);
            Dg.ItemsSource = dataTable.DefaultView;
        }

        private void MiInsert_OnClick(object sender, RoutedEventArgs e)
        {
            object[] parameters =
               { TbCourseName.Text, TbDescription.Text, TbDuration.Text, TbInstructor.Text};
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@param1", parameters[0]),
                new SqlParameter("@param2", parameters[1]),
                new SqlParameter("@param3", parameters[2]),
                new SqlParameter("@param4", parameters[3])
            };
            _conn.InsertData(InsertQuery, sqlParameters);
        }

        private void MiUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
            string value1 = TbCourseName.Text; // Первая колонка в строке
            string value2 = TbDescription.Text; // Вторая колонка в строке
            string value3 = TbDuration.Text; // Третья колонка в строке
            string value4 = TbInstructor.Text; // Четвертая колонка в строке
            int primaryKeyValue = int.Parse(selectedRow["subject_id"].ToString());

            // Создаем параметры для запроса
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@param1", value1),
                new SqlParameter("@param2", value2),
                new SqlParameter("@param3", value3),
                new SqlParameter("@param4", value4),
                new SqlParameter("@primaryKeyValue",
                    primaryKeyValue)
            };

            // Выполняем запрос на обновление
            _conn.UpdateData(UpdateQuery, parameters);

            // Обновляем данные в DataGrid
            DataTable dataTable = _conn.GetDataTable(SelectQuery);
            Dg.ItemsSource = dataTable.DefaultView;
        }
        
        private void Dg_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
            if (selectedRow != null)
            {
                TbCourseName.Text = selectedRow["course_name"].ToString() ?? string.Empty;

                TbDescription.Text = selectedRow["description"].ToString() ?? string.Empty;

                TbDuration.Text = selectedRow["duration"].ToString() ?? string.Empty;

                TbInstructor.Text = selectedRow["instructor"].ToString() ?? string.Empty;
                
            }
        }

        private void MiDelete_OnClick(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
            if (selectedRow != null)
            {
                int primaryKeyValue = int.Parse(selectedRow["subject_id"].ToString());

                // Создаем параметры для запроса
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@primaryKeyValue", primaryKeyValue)
                };

                SqlParameter[] parameters2 = new SqlParameter[]
                {
                    new SqlParameter("@primaryKeyValue", primaryKeyValue)
                };

                _conn.DeleteData(DeleteQueryChild, parameters2);

                // Выполняем запрос на удаление
                _conn.DeleteData(DeleteQuery, parameters);

                // Обновляем данные в DataGrid
                DataTable dataTable = _conn.GetDataTable(SelectQuery);
                Dg.ItemsSource = dataTable.DefaultView;
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.");
            }
        }

        private void MiClear_OnClick(object sender, RoutedEventArgs e)
        {
            _conn.TruncateTable(TruncateQueryChild);
            // Выполняем запрос на очистку таблицы
            _conn.TruncateTable(TruncateQuery);
        }


        private void MiWindowRating_OnClick(object sender, RoutedEventArgs e)
        {
            // Проверяем, открыто ли уже окно Window_Ratings
            if (Application.Current.Windows.OfType<Window_Ratings>().Any())
            {
                // Окно уже открыто, необходимо активировать его
                Window_Ratings window = Application.Current.Windows.OfType<Window_Ratings>().First();
                window.Activate();
            }
            else
            {
                // Окно еще не открыто, создаем новый экземпляр и открываем его
                Window_Ratings window = new Window_Ratings();
                window.Show();
            }
        }
    }
}
