using System.Windows;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows.Controls;

namespace RatingStudents;

public partial class Window_Ratings : Window
{
    private const string SelectQuery = "SELECT dbo.Ratings.*, Students.first_name + ' ' + Students.second_name + ' ' + " +
                                       "Students.patronymic AS full_name, Subjects.course_name FROM dbo.Ratings " +
                                       "INNER JOIN Students ON dbo.Ratings.student_id = Students.id " +
                                       "INNER JOIN Subjects ON dbo.Ratings.subject_id = Subjects.id;";

    private const string InsertQuery = "INSERT INTO dbo.Ratings VALUES (@param1, @param2, @param3)";

    private const string UpdateQuery = "UPDATE dbo.Ratings SET student_id = @param1, grade = @param2, " +
                                       "student_id = @param3 " +
                                       "WHERE id = @primaryKeyValue";

    private const string DeleteQuery = "DELETE FROM dbo.Ratings WHERE id = @primaryKeyValue";

    private const string TruncateQuery = $"DELETE FROM dbo.Ratings";
    
    private const string SelectComboBoxQuerySubjects = "SELECT course_name FROM dbo.Subjects";
    private const string SelectComboBoxQueryStudents = "SELECT id, first_name, second_name, patronymic FROM dbo.Students;";

    private readonly ConnectionDb _conn;
    
    
    public Window_Ratings()
    {
        InitializeComponent();
        _conn = new ConnectionDb();
        DataTable dataTable = _conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
        CbSubject.ItemsSource = _conn.FillComboBoxSubjects(SelectComboBoxQuerySubjects);
        CbStudent.ItemsSource = _conn.FillComboBoxStudents(SelectComboBoxQueryStudents);
    }

    private void MiSelect_OnClick(object sender, RoutedEventArgs e)
    {
        DataTable dataTable = _conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
    }


    private void MiInsert_OnClick(object sender, RoutedEventArgs e)
    {
        object[] parameters =
            [int.Parse(TbStudent.Text), decimal.Parse(TbGrade.Text), int.Parse(TbSubject.Text)];
        SqlParameter[] sqlParameters = new SqlParameter[]
        {
            new SqlParameter("@param1", parameters[0]),
            new SqlParameter("@param2", parameters[1]),
            new SqlParameter("@param3", parameters[2])
        };

        _conn.InsertData(InsertQuery, sqlParameters);
    }

    private void MiUpdate_OnClick(object sender, RoutedEventArgs e)
    {
        DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
        int value1 = int.Parse(TbStudent.Text.ToString()); // Первая колонка в строке
        decimal value2 =  decimal.Parse(TbGrade.Text.ToString()); // Вторая колонка в строке
        int value3 = int.Parse(TbSubject.Text.ToString()); // Первая колонка в строке
        int primaryKeyValue = int.Parse(selectedRow["id"].ToString() ?? string.Empty);

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
    }

    private void Dg_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
        if (selectedRow != null)
        {
            TbStudent.Text = selectedRow["student_id"].ToString() ?? string.Empty;

            TbGrade.Text = selectedRow["grade"].ToString() ?? string.Empty;

            TbSubject.Text = selectedRow["subject_id"].ToString() ?? string.Empty;
        }
    }

    private void CbSubject_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string? selectedCourse = CbSubject.SelectedItem.ToString();
        int subjectId = _conn.GetSubjectsId(selectedCourse, "Subjects");
        TbSubject.Text = subjectId.ToString();
    }

    private void CbStudent_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selectedStudent = CbStudent.SelectedItem.ToString() ?? string.Empty;
        int studentId = _conn.GetStudentId(selectedStudent);
        TbStudent.Text = studentId.ToString();
    }

    private void TbStudent_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        int selectedId = Convert.ToInt32(TbStudent.Text);
        string? courseName = _conn.GetSubjectsName(selectedId, "Students", "second_name");
        foreach (var item in CbStudent.Items)
        {
            // Преобразуем элемент в строку
            string? itemText = item.ToString();

            // Проверяем, содержит ли текст элемента искомое слово
            if (courseName != null && itemText != null && itemText.Contains(courseName))
            {
                // Если да, выделяем элемент и выходим из цикла
                CbStudent.SelectedItem = item;
                break;
            }
        }
    }

    private void TbSubject_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        int selectedId = Convert.ToInt32(TbSubject.Text);
        string? courseName = _conn.GetSubjectsName(selectedId, "Subjects", "course_name");
        if (!string.IsNullOrEmpty(courseName))
        {
            CbSubject.SelectedItem = courseName;
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
}