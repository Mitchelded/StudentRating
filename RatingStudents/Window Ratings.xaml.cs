using System.Windows;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace RatingStudents;

public partial class Window_Ratings : Window
{
    private const string SelectQuery =
        "SELECT dbo.Ratings.*, " +
        "Students.first_name + ' ' + " +
        "ISNULL(Students.second_name, '') + ' ' + " +
        "ISNULL(Students.patronymic, '') AS full_name, " +
        "Subjects.course_name " +
        "FROM dbo.Ratings " +
        "INNER JOIN Students ON dbo.Ratings.student_id = Students.student_id " +
        "INNER JOIN Subjects ON dbo.Ratings.subject_id = Subjects.subject_id;";


    private const string InsertQuery = "INSERT INTO dbo.Ratings VALUES (@param1, @param2, @param3)";

    private const string UpdateQuery = "UPDATE dbo.Ratings SET student_id = @param1, grade = @param2, " +
                                       "subject_id = @param3 " +
                                       "WHERE rating_id = @primaryKeyValue";

    private const string DeleteQuery = "DELETE FROM dbo.Ratings WHERE rating_id = @primaryKeyValue";

    private const string TruncateQuery = $"DELETE FROM dbo.Ratings";

    private const string SelectComboBoxQuerySubjects = "SELECT subject_id, course_name FROM dbo.Subjects";

    private const string SelectComboBoxQueryStudents =
        "SELECT student_id, first_name, second_name, patronymic FROM dbo.Students;";

    private ConnectionDb _conn = new ConnectionDb();

    public Window_Ratings()
    {
        InitializeComponent();
        
    
        
    }

    void Refresh()
    {
        
        DataTable dataTable = _conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
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
                { int.Parse(TbStudent.Text), decimal.Parse(TbGrade.Text), int.Parse(TbSubject.Text) };
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@param1", parameters[0]),
                new SqlParameter("@param2", parameters[1]),
                new SqlParameter("@param3", parameters[2])
            };

            _conn.InsertData(InsertQuery, sqlParameters);
            Refresh();
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
            int value1 = int.Parse(TbStudent.Text); // Первая колонка в строке
            decimal value2 = decimal.Parse(TbGrade.Text); // Вторая колонка в строке
            int value3 = int.Parse(TbSubject.Text); // Первая колонка в строке
            int primaryKeyValue = int.Parse(selectedRow["rating_id"].ToString() ?? string.Empty);

            // Создаем параметры для запроса
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@param1", value1),
                new SqlParameter("@param2", value2),
                new SqlParameter("@param3", value3),
                new SqlParameter("@primaryKeyValue",
                    primaryKeyValue)
            };

            // Выполняем запрос на обновление
            _conn.UpdateData(UpdateQuery, parameters);

            // Обновляем данные в DataGrid
            DataTable dataTable = _conn.GetDataTable(SelectQuery);
            Dg.ItemsSource = dataTable.DefaultView;
            Refresh();
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
                TbStudent.Text = selectedRow["student_id"].ToString() ?? string.Empty;

                TbGrade.Text = selectedRow["grade"].ToString() ?? string.Empty;

                TbSubject.Text = selectedRow["subject_id"].ToString() ?? string.Empty;
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
                int primaryKeyValue = int.Parse(selectedRow["rating_id"].ToString() ?? string.Empty);

                // Создаем параметры для запроса
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@primaryKeyValue", primaryKeyValue)
                };

                // Выполняем запрос на удаление
                _conn.DeleteData(DeleteQuery, parameters);

                // Обновляем данные в DataGrid
                DataTable dataTable = _conn.GetDataTable(SelectQuery);
                Dg.ItemsSource = dataTable.DefaultView;
                Refresh();
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
        try
        {
            TbStudent.Text = string.Empty;
            TbSubject.Text = string.Empty;
            TbGrade.Text = string.Empty;
            Dg.ItemsSource = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void MiWindowSubject_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Application.Current.Windows.OfType<Window_Subjects>().Any())
            {
                WindowManager.windowSubjects.Show();
            }
            else
            {
                Window_Subjects windowSubjects = new Window_Subjects();
                WindowManager.windowSubjects = windowSubjects;
                windowSubjects.Show();
            }

            WindowManager.SubjectsManagerWindow("Open");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MiWindowStudents_OnClick(object sender, RoutedEventArgs e)
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

    private void Window_Ratings_OnUnloaded(object sender, RoutedEventArgs e)
    {
        try
        {
            WindowManager.RatingsManagerWindow("Close");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}