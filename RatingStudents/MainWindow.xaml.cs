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
        
        // if (String.IsNullOrEmpty(TbPort.Text) && String.IsNullOrEmpty(PbPassword.Password))
        // {
        //     MessageBox.Show("Port & Password Has Not Been Specified", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        // } 
        // else if (String.IsNullOrEmpty(TbPort.Text) || String.IsNullOrEmpty(PbPassword.Password))
        // {
        //     MessageBox.Show($"{(String.IsNullOrEmpty(TbPort.Text) ? "Port" : "Password")} Has Not Been Specified", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        // }
        if (String.IsNullOrEmpty(TbDataSource.Text))
        {
            MessageBox.Show("PData Source Has Not Been Specified", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        } 
        else
        {
            ConnectionDb.DataSource = TbDataSource.Text;
            Window_Students windowStudents = new Window_Students();
            windowStudents.Show();
            this.Close();
        }
    }
}