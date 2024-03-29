using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace RatingStudents;

public partial class Window_Students : Window
{
    private const string SelectQuery = "SELECT Students.* FROM dbo.Students";

    private const string InsertQuery = "INSERT INTO dbo.Students VALUES (@param1, @param2, @param3, @param4)";

    private const string UpdateQuery = "UPDATE dbo.Students SET first_name = @param1, second_name = @param2 " +
                                       "patronymic = @param3, adress = @param4" +
                                       "WHERE id = @primaryKeyValue";

    private const string DeleteQuery = "DELETE FROM dbo.Students WHERE id = @primaryKeyValue";
    private const string DeleteQueryChild = "DELETE FROM dbo.Ratings WHERE student_id = @primaryKeyValue";

    private const string TruncateQuery = $"DELETE FROM  dbo.Students";


    private readonly ConnectionDb _conn;

    public Window_Students()
    {
        InitializeComponent();
        _conn = new ConnectionDb();
        DataTable dataTable = _conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
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
            { TbFirstName.Text, TbSecondName.Text, TbPatronymic.Text, TbAddress.Text};
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
        string value1 = TbFirstName.Text; // Первая колонка в строке
        string value2 = TbSecondName.Text; // Вторая колонка в строке
        string value3 = TbPatronymic.Text; // Третья колонка в строке
        string value4 = TbAddress.Text; // Четвертая колонка в строке
        int primaryKeyValue = int.Parse(selectedRow["id"].ToString());

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

    private void Dg_OnSelected(object sender, RoutedEventArgs e)
    {
        DataRowView selectedRow = (DataRowView)Dg.SelectedItem;
        if (selectedRow != null)
        {
            TbFirstName.Text = selectedRow["first_name"].ToString() ?? string.Empty;

            TbSecondName.Text = selectedRow["second_name"].ToString() ?? string.Empty;

            TbPatronymic.Text = selectedRow["patronymic"].ToString() ?? string.Empty;

            TbAddress.Text = selectedRow["adress"].ToString() ?? string.Empty;

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
        // Выполняем запрос на очистку таблицы
        _conn.TruncateTable(TruncateQuery);
    }



    private void MiWindowRatings_Click(object sender, RoutedEventArgs e)
    {
        //Window_Ratings window = new Window_Ratings();
        //window.Show();
    }

    private void MiTools_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("1. C# Programming Language\n" +
            "2. Wpf Libriary", "Tools", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MiAbout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Student Rating\n" +
            "Tretyakov Anton Olegovich", "About", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}