using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace RatingStudents;

public partial class Window_Students : Window
{
    private const string SelectQuery = "SELECT Students.*, Subjects.course_name FROM dbo.Students INNER JOIN dbo.Subjects ON Students.subjects_id = Subjects.id;";

    private const string InsertQuery = "INSERT INTO dbo.Students VALUES (@param1, @param2, @param3, @param4, @param5)";

    private const string UpdateQuery = "UPDATE dbo.Students SET first_name = @param1, second_name = @param2, " +
                                       "patronymic = @param3, adress = @param4, subjects_id = @param5 " +
                                       "WHERE id = @primaryKeyValue";

    private const string DeleteQuery = "DELETE FROM dbo.Students WHERE id = @primaryKeyValue";

    private const string TruncateQuery = $"TRUNCATE TABLE dbo.Students";
    
    private const string SelectComboBoxQuery = "SELECT course_name FROM dbo.Subjects";

    private readonly ConnectionDb _conn;

    public Window_Students()
    {
        InitializeComponent();
        _conn = new ConnectionDb();
        DataTable dataTable = _conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
        CbSubject.ItemsSource = _conn.FillComboBox(SelectComboBoxQuery);
    }

    private void miWindowSubject_Click(object sender, RoutedEventArgs e)
    {
        Window_Subjects window = new Window_Subjects();
        window.Show();
    }

    private void MiSelect_OnClick(object sender, RoutedEventArgs e)
    {
        DataTable dataTable = _conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
    }

    private void MiInsert_OnClick(object sender, RoutedEventArgs e)
    {
        object[] parameters =
            [TbFirstName.Text, TbSecondName.Text, TbPatronymic.Text, TbAddress.Text, int.Parse(TbSubject.Text)];
        SqlParameter[] sqlParameters = new SqlParameter[]
        {
            new SqlParameter("@param1", parameters[0]),
            new SqlParameter("@param2", parameters[1]),
            new SqlParameter("@param3", parameters[2]),
            new SqlParameter("@param4", parameters[3]),
            new SqlParameter("@param5", parameters[4])
        };

        _conn.InsertData(InsertQuery, sqlParameters);
    }

    private void MiUpdate_OnClick(object sender, RoutedEventArgs e)
    {
        DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
        string value1 = TbFirstName.Text; // Первая колонка в строке
        string value2 = TbSecondName.Text; // Вторая колонка в строке
        string value3 = TbPatronymic.Text; // Третья колонка в строке
        string value4 = TbAddress.Text; // Четвертая колонка в строке
        int value5 = int.Parse(TbSubject.Text); // Пятая колонка в строке
        int primaryKeyValue = int.Parse(selectedRow["id"].ToString());

        // Создаем параметры для запроса
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@param1", value1),
            new SqlParameter("@param2", value2),
            new SqlParameter("@param3", value3),
            new SqlParameter("@param4", value4),
            new SqlParameter("@param5", value5),
            new SqlParameter("@primaryKeyValue",
                primaryKeyValue)
        };

        // Выполняем запрос на обновление
        _conn.UpdateData(UpdateQuery, parameters);

        // Обновляем данные в DataGrid
        DataTable dataTable = _conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
    }

    private void Dg_OnSelected(object sender, RoutedEventArgs e)
    {
        DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
        if (selectedRow != null)
        {
            TbFirstName.Text = selectedRow["first_name"].ToString() ?? string.Empty;

            TbSecondName.Text = selectedRow["second_name"].ToString() ?? string.Empty;

            TbPatronymic.Text = selectedRow["patronymic"].ToString() ?? string.Empty;

            TbAddress.Text = selectedRow["adress"].ToString() ?? string.Empty;

            TbSubject.Text = selectedRow["subjects_id"].ToString() ?? string.Empty;
        }
    }

    private void MiDelete_OnClick(object sender, RoutedEventArgs e)
    {
        DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
        if (selectedRow != null)
        {
            int primaryKeyValue = int.Parse(selectedRow["id"].ToString());

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
        }
        else
        {
            MessageBox.Show("Выберите строку для удаления.");
        }
    }

    private void MiClear_OnClick(object sender, RoutedEventArgs e)
    {
        // Выполняем запрос на очистку таблицы
        _conn.TruncateTable(TruncateQuery);
    }

    private void CbSubject_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string? selectedCourse = CbSubject.SelectedItem.ToString();
        int subjectId = _conn.GetSubjectId(selectedCourse);
        TbSubject.Text = subjectId.ToString();
    }


    private void TbSubject_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        int selectedId = Convert.ToInt32(TbSubject.Text);
        string? courseName = _conn.GetCourseName(selectedId);
        if (!string.IsNullOrEmpty(courseName))
        {
            CbSubject.SelectedItem = courseName;
        }
    }
}