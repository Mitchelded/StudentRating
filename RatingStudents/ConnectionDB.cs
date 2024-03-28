using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace RatingStudents;

public class ConnectionDb
{
    public static string? DataSource; // REVISION-PC
    


    private static readonly string ConnectionString =
        $"Data Source={DataSource};Database=StudentRating;Integrated Security=True;TrustServerCertificate=True";

    public DataTable GetDataTable(string queryString)
    {
        DataTable dataTable = new DataTable();

        using SqlConnection connection = new SqlConnection(ConnectionString);
        SqlCommand command = new SqlCommand(queryString, connection);
        SqlDataAdapter adapter = new SqlDataAdapter(command);
        try
        {
            connection.Open();
            adapter.Fill(dataTable);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return dataTable;
    }
    
    public void InsertData(string insertQuery, params SqlParameter[] parameters)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        SqlCommand command = new SqlCommand(insertQuery, connection);
        command.Parameters.AddRange(parameters);

        try
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // Обработка исключения
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    
    public void UpdateData(string updateQuery, params SqlParameter[] parameters)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        SqlCommand command = new SqlCommand(updateQuery, connection);
        // Добавляем параметры к команде
        command.Parameters.AddRange(parameters);

        try
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // Обработка исключения
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    
    public void DeleteData(string deleteQuery, params SqlParameter[] parameters)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        SqlCommand command = new SqlCommand(deleteQuery, connection);
        // Добавляем параметры к команде
        command.Parameters.AddRange(parameters);

        try
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // Обработка исключения
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    
    public void TruncateTable(string truncateQuery)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        SqlCommand command = new SqlCommand(truncateQuery, connection);

        try
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // Обработка исключения
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    
    
    public List<string?> FillComboBox(string selectQuery)
    {
        // Получаем данные из базы данных
        DataTable dataTable = GetDataTable(selectQuery);

        // Создаем список для хранения значений course_name
        List<string?> courseNames = new List<string?>();

        // Добавляем каждое значение course_name в список
        foreach (DataRow row in dataTable.Rows)
        {
            string? courseName = row["course_name"].ToString();
            courseNames.Add(courseName);
        }

        // Возвращаем список значений course_name
        return courseNames;
    }

    private object? ExecuteScalar(string query, params SqlParameter[] parameters)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddRange(parameters);

        try
        {
            connection.Open();
            return command.ExecuteScalar();
        }
        catch (Exception ex)
        {
            // Обработка исключения
            Console.WriteLine("Error: " + ex.Message);
            return null;
        }
    }
    
    public int GetSubjectId(string? courseName)
    {
        // Запрос для получения id по course_name из таблицы Subjects
        string selectQuery = "SELECT id FROM dbo.Subjects WHERE course_name = @courseName";
        using SqlConnection connection = new SqlConnection(ConnectionString);
        // Параметр для передачи значения courseName в запрос
        SqlParameter parameter = new SqlParameter("@courseName", courseName);

        // Получаем id из базы данных
        object? result = ExecuteScalar(selectQuery, parameter);

        // Проверяем, что результат не является null и преобразуем его в int
        int id = result != null ? Convert.ToInt32(result) : -1; // Если результат null, вернем -1

        return id;
    }
    
    public string? GetCourseName(int id)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            string query = "SELECT course_name FROM dbo.Subjects WHERE id = @id";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            try
            {
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        return null; // Возвращаем null, если что-то пошло не так или запись не найдена
    }
}