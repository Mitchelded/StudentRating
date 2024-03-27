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
    
    public bool InsertData(string insertQuery)
    {
        bool success = false;

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(insertQuery, connection);

            try
            {
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                success = rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        return success;
    }
    
}