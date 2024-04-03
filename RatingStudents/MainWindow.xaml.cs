using System.Windows;

namespace RatingStudents;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
    }

    private void BtnRun_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(TbDataSource.Text))
            {
                MessageBox.Show("PData Source Has Not Been Specified", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                ConnectionDb.DataSource = TbDataSource.Text;
                WindowManager.windowStudents = new Window_Students();
                WindowManager.windowSubjects = new Window_Subjects();
                WindowManager.windowRatings = new Window_Ratings();
                WindowManager.windowStudents.Show();
                this.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}