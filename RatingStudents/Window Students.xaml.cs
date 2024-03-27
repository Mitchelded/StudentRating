using System.Data;
using System.Windows;

namespace RatingStudents;

public partial class Window_Students : Window
{
    private const string SelectQuery = "SELECT * FROM dbo.Students;";
    
    public Window_Students()
    {
        InitializeComponent();
        ConnectionDb conn = new();
        DataTable dataTable = conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
    }

    private void miWindowSubject_Click(object sender, RoutedEventArgs e)
    {
        Window_Subjects window = new Window_Subjects();
        window.Show();
    }

    private void MiSelect_OnClick(object sender, RoutedEventArgs e)
    {
        ConnectionDb conn = new();
        DataTable dataTable = conn.GetDataTable(SelectQuery);
        Dg.ItemsSource = dataTable.DefaultView;
    }

    private void MiInsert_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}