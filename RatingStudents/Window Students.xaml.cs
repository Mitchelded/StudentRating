using System.Windows;

namespace RatingStudents;

public partial class Window_Students : Window
{
    public Window_Students()
    {
        InitializeComponent();
    }

    private void miWindowSubject_Click(object sender, RoutedEventArgs e)
    {
        Window_Subjects window = new Window_Subjects();
        window.Show();
    }
}