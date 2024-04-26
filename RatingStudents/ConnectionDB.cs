using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace RatingStudents;

public class ConnectionDb
{
    public static string? DataSource; // REVISION-PC
    


    private static string ConnectionString =>
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
            MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    
    public List<string?> FillComboBoxSubjects(string selectQuery)
    {
        // Создаем список для хранения значений course_name
        List<string?> courseNames = new List<string?>();

        try
        {
            // Получаем данные из базы данных
            using DataTable dataTable = GetDataTable(selectQuery);
            // Добавляем каждое значение course_name в список
            foreach (DataRow row in dataTable.Rows)
            {
                    
                // Получаем значение course_name и проверяем на null
                object courseNameInfo = $"{row["subject_id"]} {row["course_name"]}";
                string? courseName = courseNameInfo != DBNull.Value ? courseNameInfo.ToString() : null;
                courseNames.Add(courseName);
            }
        }
        catch (Exception ex)
        {
            // Обработка исключения
            MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // Можно выбросить исключение или сделать что-то другое в зависимости от требований
        }

        // Возвращаем список значений course_name
        return courseNames;
    }

    public List<string?> FillComboBoxStudents(string selectQuery)
    {
        // Создаем список для хранения значений студентов
        List<string?> studentNames = new List<string?>();

        try
        {
            // Получаем данные из базы данных
            using DataTable dataTable = GetDataTable(selectQuery);
            // Добавляем каждого студента в список
            foreach (DataRow row in dataTable.Rows)
            {   
                // Формируем строку с данными студента
                string studentInfo = $"{row["student_id"]} {row["first_name"]} {row["second_name"]} {row["patronymic"]}";
                studentNames.Add(studentInfo);
            }
        }
        catch (Exception ex)
        {
            // Обработка исключения
            MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // Можно выбросить исключение или сделать что-то другое в зависимости от требований
        }

        // Возвращаем список значений студентов
        return studentNames;
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
            MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }
    
    public int GetSubjectsId(string? courseName, string table)
    {
        // Запрос для получения id по course_name из таблицы Subjects
        string selectQuery = $"SELECT subject_id FROM dbo.{table} WHERE subject_id = @courseName";
        using SqlConnection connection = new SqlConnection(ConnectionString);
        // Параметр для передачи значения courseName в запрос
        SqlParameter parameter = new SqlParameter("@courseName", courseName);

        // Получаем id из базы данных
        object? result = ExecuteScalar(selectQuery, parameter);

        // Проверяем, что результат не является null и преобразуем его в int
        int id = result != null ? Convert.ToInt32(result) : -1; // Если результат null, вернем -1

        return id;
    }
    
    public int GetStudentId(string studentInfo)
    {
        // Разделить строку на части, используя пробел в качестве разделителя
        string[] parts = studentInfo.Split(' ');

        // Проверить, что строка содержит необходимое количество частей
        if (parts.Length != 4)
        {
            return -1; // Возвращаем -1, если строка имеет неверный формат
        }

        // Получаем значения id, first_name, second_name и patronymic из разделенных частей
        string id = parts[0];
        string firstName = parts[1];
        string secondName = parts[2];
        string patronymic = parts[3];

        // Запрос для получения id студента
        string selectQuery = $"SELECT student_id FROM dbo.Students WHERE student_id = @id";

        using SqlConnection connection = new SqlConnection(ConnectionString);
        // Параметры для передачи значений в запрос
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@id", id),
            new SqlParameter("@firstName", firstName),
            new SqlParameter("@secondName", secondName),
            new SqlParameter("@patronymic", patronymic)
        };

        // Получаем id из базы данных
        object? result = ExecuteScalar(selectQuery, parameters);

        // Проверяем, что результат не является null и преобразуем его в int
        int studentId = result != null ? Convert.ToInt32(result) : -1; // Если результат null, вернем -1

        return studentId;
    }

    
    public string? GetSubjectsName(int id, string table, string column, string id_name)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        string query = $"SELECT {column} FROM dbo.{table} WHERE {id_name} = @id";

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
            MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return null; // Возвращаем null, если что-то пошло не так или запись не найдена
    }
}