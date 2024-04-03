using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private const string TruncateQueryChild = "Delete From dbo.Ratings";

        private readonly ConnectionDb _conn;

        public Window_Subjects()
        {
            InitializeComponent();
            try
            {
                _conn = new ConnectionDb();
                DataTable dataTable = _conn.GetDataTable(SelectQuery);
                Dg.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miWindowStudents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Application.Current.Windows.OfType<Window_Students>().Any())
                {
                    WindowManager.windowStudents.Show();
                }
                else
                {
                    Window_Students windowStudents = new Window_Students();
                    WindowManager.windowStudents = windowStudents;
                    windowStudents.Show();
                }

                WindowManager.StudentsManagerWindow("Open");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MiSelect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dataTable = _conn.GetDataTable(SelectQuery);
                Dg.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MiInsert_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                object[] parameters =
                    { TbCourseName.Text, TbDescription.Text, TbDuration.Text, TbInstructor.Text };
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@param1", parameters[0]),
                    new SqlParameter("@param2", parameters[1]),
                    new SqlParameter("@param3", parameters[2]),
                    new SqlParameter("@param4", parameters[3])
                };
                _conn.InsertData(InsertQuery, sqlParameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MiUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Dg_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MiDelete_OnClick(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MiClear_OnClick(object sender, RoutedEventArgs e)
        {
            Dg.ItemsSource = null;
        }


        private void MiWindowRating_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Application.Current.Windows.OfType<Window_Ratings>().Any())
                {
                    WindowManager.windowRatings.Show();
                }
                else
                {
                    Window_Ratings windowRatings = new Window_Ratings();
                    WindowManager.windowRatings = windowRatings;
                    windowRatings.Show();
                }

                WindowManager.RatingsManagerWindow("Open");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Subjects_OnUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowManager.SubjectsManagerWindow("Close");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}