using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data;
using System.Data;
using MySql.Data.Types;
using System.Data.Odbc;
using static Mysqlx.Notice.Frame.Types;
using MySql.Data.MySqlClient;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Controls.Primitives;
using System.IO;
using StudentRating;

namespace RUN
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>


    class Record

    {
        public int Id { get; set; }

        public String Surname { get; set; }

        public String Name { get; set; }

        public String Patronymic { get; set; }
        
        public String Object { get; set; }

        public int Score { get; set; }


        public Record(int Id, String Surname, String Name, String Patronymic, String Object, int Score)

        {
            this.Id = Id;

            this.Surname = Surname;

            this.Name = Name;

            this.Patronymic = Patronymic;
            this.Object = Object;
            this.Score = Score;
            
        }
    }

    /*================================================================================*/

    public partial class MainWindow : Window
    {
        MySqlConnection conn;

        MySqlCommand cmd;

        MySqlDataAdapter adapter;

        DataTable dt;

        DataRow dr;


        string ConnStr;

        String SelectText = "Select * From StudentRating Order By id";

        String SelectObjectText = "Select * From StudentRating " +
            "Where Object = ?";

        String InsertText = "Insert Into StudentRating Values " +
                             " ( ? , ? , ? , ?, ?, ? ) ";

        String UpdateText = "Update StudentRating Set " +
                             "Name = ? , " +
                             "Surname = ? , " +
                             "Patronymic = ? ," +
                             "Object = ? " +
                             "Score = ? " +
                             "Where Id = ? ";

        String DeleteText = "Delete From StudentRating " +
                             "Where Id = ? ";

        List<Record> RecordList;

        int i, n;


        /*================================================================================*/

        public MainWindow()
        {
            InitializeComponent();
            radioonline.IsChecked = true;

        }

        /*================================================================================*/
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radiolocal.IsChecked == true)
            {
                // Устанавливаем значение String ConnStr для первого RadioButton
                ConnStr = "Server=localhost;" +
                         "Port=3306;" +
                         "DataBase=StudentRating;" +
                         "UId=root;" +
                         "PassWord=K1aqv#YZ7#hjsKSd~J{8XAZLRBIl5hEcCf9iMGTq56z0feH926fT$vPligGeIXNvUKbx1ZsR7nr*wr%f7hdaqkZ*7Mp?Hbuj%qE;";
                radioonline.IsChecked = false;
                radioMelnichenko.IsChecked = false;
                Refresh();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioonline.IsChecked==true)
            {
                // Устанавливаем значение String ConnStr для второго RadioButton
                ConnStr = "Server=sql7.freemysqlhosting.net;" +
                          "Port=3306;" +
                          "DataBase=sql7611480;" +
                          "UId=sql7611480;" +
                          "PassWord=i9tZyP4N83;";

                // Сбрасываем состояние первого RadioButton
                radiolocal.IsChecked = false;
                radioMelnichenko.IsChecked = false;
                Refresh();
            }
        }

        private void radioButton3_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (radioMelnichenko.IsChecked == true)
            {
                // Устанавливаем значение String ConnStr для второго RadioButton
                ConnStr = "Server=localhost;" +
                          "Port=3306;" +
                          "DataBase=StudentRating;" +
                          "UId=sql7611480;" +
                          "PassWord=3;";

                // Сбрасываем состояние первого RadioButton
                radiolocal.IsChecked = false;
                radioonline.IsChecked = false;
                Refresh();
            }
        }


        /*================================================================================*/

        private void miSelect_Click(object sender, RoutedEventArgs e)

        {
            
            Refresh();
        }

        /*================================================================================*/

        private void BtnTruncate_Click(object sender, RoutedEventArgs e)
        {
            Truncate();
            Refresh();
        }

        /*================================================================================*/

        private void miInsert_Click(object sender, RoutedEventArgs e)

        {
            Insert();
            Refresh();
        }

        /*================================================================================*/

        private void miDelete_Click(object sender, RoutedEventArgs e)

        {
            Delete();
            Refresh();
        }

        /*================================================================================*/

        private void miUpdate_Click(object sender, RoutedEventArgs e)

        {

            Update();
            Refresh();
        }

        /*================================================================================*/

        private void dg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)

        {
            n = dg.SelectedIndex;

            if (n == -1) return;

            EId.Text = Convert.ToString(dt.Rows[n][0]);

            ESurname.Text = (String)dt.Rows[n][1];

            EName.Text = (String)dt.Rows[n][2];

            EPatronymic.Text = (String)dt.Rows[n][3];

            EObject.Text = (String)dt.Rows[n][4];

            EScore.Text = Convert.ToString(dt.Rows[n][5]);
        }

        /*================================================================================*/

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            Import();
            Refresh();
        }

        /*================================================================================*/

        void Truncate()
        {
            try
            {
                
                conn = new MySqlConnection(ConnStr);
                conn.Open();
                string truncateQuery = $"TRUNCATE TABLE StudentRating";
                cmd = new MySqlCommand(truncateQuery, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Таблица успешно обрезана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                conn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*================================================================================*/

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            Export();
            Refresh();
        }

        /*================================================================================*/
        void Refresh()

        {
            try

            {

                dg.ItemsSource = null;

                conn = new MySqlConnection();

                conn.ConnectionString = ConnStr;

                cmd = new MySqlCommand();

                cmd.Connection = conn;

                cmd.CommandText = SelectText;

                adapter = new MySqlDataAdapter();

                adapter.SelectCommand = cmd;

                dt = new DataTable();

                RecordList = new List<Record>();

                adapter.Fill(dt);

                for (i = 0; i <= dt.Rows.Count - 1; i++)

                    RecordList.Add(new Record((int)dt.Rows[i][0],
                                                (String)dt.Rows[i][1],
                                                (String)dt.Rows[i][2],
                                                (String)dt.Rows[i][3],
                                                (String)dt.Rows[i][4],
                                                (int)dt.Rows[i][5]
                                              )
                                  );

                dg.ItemsSource = RecordList;
            }


            catch (Exception exc) { MessageBox.Show(exc.Message); }

            RecordList = null;

            GC.Collect();
        }

        /*================================================================================*/
        void Import()
        {
            try
            {
                string filePath = TxtFilePath.Text;
                if (File.Exists(filePath))
                {
                    conn = new MySqlConnection(ConnStr);
                    conn.Open();

                    string loadDataQuery = $"LOAD DATA INFILE '{filePath}' " +
                        $"INTO TABLE StudentRating " +
                        $"FIELDS TERMINATED BY '\t' " +
                        $"LINES TERMINATED BY '\n' " +
                        $"(Id, Name, Surname, Patronymic, Object, Score);";

                    cmd = new MySqlCommand(loadDataQuery, conn);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Data imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("File not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*================================================================================*/
        void Export()
        {
            try
            {
                string filePath = TxtFilePath_Copy.Text;


                conn = new MySqlConnection(ConnStr);
                conn.Open();


                string loadDataQuery = $"SELECT Id, Name, Surname, Patronymic, Object, Score From StudentRating INTO OUTFILE '{filePath}' FIELDS TERMINATED BY '\t' LINES TERMINATED BY '\n'";
                cmd = new MySqlCommand(loadDataQuery, conn);

                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Data imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*================================================================================*/
        void Insert()
        {
            conn = new MySqlConnection();
            conn.ConnectionString = ConnStr;

            cmd = new MySqlCommand();

            cmd.Connection = conn;

            cmd.CommandText = SelectText;

            adapter = new MySqlDataAdapter();

            adapter.SelectCommand = cmd;

            dt = new DataTable();

            adapter.Fill(dt);



            cmd.CommandText = InsertText;

            cmd.Parameters.Add("@Id", MySqlDbType.Int32, 4, "Id");
            cmd.Parameters.Add("@Name", MySqlDbType.VarChar, 45, "Name");
            cmd.Parameters.Add("@Surname", MySqlDbType.VarChar, 45, "Surname");  
            cmd.Parameters.Add("@Patronymic", MySqlDbType.VarChar, 45, "Patronymic");
            cmd.Parameters.Add("@Object", MySqlDbType.VarChar, 45, "Object");
            cmd.Parameters.Add("@Score", MySqlDbType.Int32, 4, "Score");

            dr = dt.NewRow();

            dr[0] = Convert.ToInt32(EId.Text);

            dr[1] = ESurname.Text;

            dr[2] = EName.Text;

            dr[3] = EPatronymic.Text;

            dr[4] = EObject.Text;

            dr[5] = Convert.ToInt32(EScore.Text);

            dt.Rows.Add(dr);

            adapter.InsertCommand = cmd;

            adapter.Update(dt);



        }

        /*================================================================================*/
        void Update()
        {
            conn = new MySqlConnection();

            conn.ConnectionString = ConnStr;

            cmd = new MySqlCommand();

            cmd.Connection = conn;

            cmd.CommandText = SelectText;

            adapter = new MySqlDataAdapter();

            adapter.SelectCommand = cmd;

            dt = new DataTable();

            adapter.Fill(dt);

            cmd.CommandText = UpdateText;

            cmd.Parameters.Add("@Name", MySqlDbType.VarChar, 45, "Name");

            cmd.Parameters.Add("@Surname", MySqlDbType.VarChar, 45, "Surname");

            cmd.Parameters.Add("@Patronymic", MySqlDbType.VarChar, 45, "Patronymic");

            cmd.Parameters.Add("@Score", MySqlDbType.Int32, 4, "Score");

            cmd.Parameters.Add("@Object", MySqlDbType.VarChar, 45, "Object");

            cmd.Parameters.Add("@Id", MySqlDbType.Int32, 4, "Id");


            n = dg.SelectedIndex;

            dt.Rows[n][1] = EName.Text;

            dt.Rows[n][2] = ESurname.Text;

            dt.Rows[n][3] = EPatronymic.Text;

            dt.Rows[n][4] = EObject.Text;

            dt.Rows[n][5] = EScore.Text;


            adapter.UpdateCommand = cmd;

            adapter.Update(dt);
        }


        void Easter()
        {
            Window1 Easter1 = new Window1();
            Window2 Easter2 = new Window2();
            if (EName.Text == "Steve" && ESurname.Text == "Harvey")
            {
                Easter1.Show();
            }
            else if (EName.Text == "Walter" && ESurname.Text == "White")
            {
                Easter2.Show();
            }
        }

        private void EasterButton_Click(object sender, RoutedEventArgs e)
        {
            Easter();

        }




        /*================================================================================*/

        void Delete()
        {
            conn = new MySqlConnection();

            conn.ConnectionString = ConnStr;

            cmd = new MySqlCommand();

            cmd.Connection = conn;

            cmd.CommandText = SelectText;

            adapter = new MySqlDataAdapter();

            adapter.SelectCommand = cmd;

            dt = new DataTable();

            adapter.Fill(dt);

            cmd.CommandText = DeleteText;

            cmd.Parameters.Add("@Id", MySqlDbType.Int32, 4, "Id");

            n = dg.SelectedIndex;

            dt.Rows[n].Delete();

            adapter.DeleteCommand = cmd;

            adapter.Update(dt);

        }

    }
}
