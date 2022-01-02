using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;

namespace Currency_Convertor_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Root values = new Root();
        public class Root
        {
            public Rate rates { get; set; }
            public long timestamp;
            public string license;
        }

        SqlConnection sqlConnection = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter SqlDataAdapter = new SqlDataAdapter();

        private int CurrencyId = 0;
        private double FromAmmount = 0;
        private double ToAmmount = 0;
        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            GetValue();
            GetData1();

        }
        public void myConnection()
        {
            String Connection = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(Connection);
            sqlConnection.Open();

        }
        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");

            dtCurrency.Rows.Add("--SELECT--",0);
            dtCurrency.Rows.Add("PLN", 1);
            dtCurrency.Rows.Add("USD",4.19);
            dtCurrency.Rows.Add("EUR",4.56);
            dtCurrency.Rows.Add("SAR",20);
            dtCurrency.Rows.Add("PUND",5.4);
            dtCurrency.Rows.Add("DEM",43);

            combFromCurrency.ItemsSource = dtCurrency.DefaultView;
            combFromCurrency.DisplayMemberPath = "Text";
            combFromCurrency.SelectedValuePath = "Value";
            combFromCurrency.SelectedIndex = 0;

            combToCurrency.ItemsSource = dtCurrency.DefaultView;
            combToCurrency.DisplayMemberPath = "Text";
            combToCurrency.SelectedValuePath = "Value";
            combToCurrency.SelectedIndex = 0;
        }
        private void NewBindCurrency()
        {
           // myConnection();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", sqlConnection);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter = new SqlDataAdapter(cmd);
            SqlDataAdapter.Fill(dt);

            DataRow newRow = dt.NewRow();
            newRow["Id"] = 0;
            newRow["CurrencyName"] = "--SELECT--";

            dt.Rows.InsertAt(newRow, 0);

            if(dt!=null && dt.Rows.Count > 0)
            {
                combFromCurrency.ItemsSource = dt.DefaultView;

                combToCurrency.ItemsSource = dt.DefaultView;
            }
            sqlConnection.Close();

            
            combFromCurrency.DisplayMemberPath = "Text";
            combFromCurrency.SelectedValuePath = "Value";
            combFromCurrency.SelectedIndex = 0;

            
            combToCurrency.DisplayMemberPath = "Text";
            combToCurrency.SelectedValuePath = "Value";
            combToCurrency.SelectedIndex = 0;
        }
        public void BindCurrencyAPI()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Text");
            dt.Columns.Add("Value");


            dt.Rows.Add("--SELECT--", 0);
            dt.Rows.Add("INR", values.rates.INR);
            dt.Rows.Add("USD", values.rates.USD);
            dt.Rows.Add("NZD", values.rates.NZD);
            dt.Rows.Add("JPY", values.rates.JPY);
            dt.Rows.Add("EUR", values.rates.EUR);
            dt.Rows.Add("CAD", values.rates.CAD);
            dt.Rows.Add("ISK", values.rates.ISK);
            dt.Rows.Add("PHP", values.rates.PHP);
            dt.Rows.Add("DKK", values.rates.DKK);
            dt.Rows.Add("CZK", values.rates.CZK);
            combFromCurrency.ItemsSource = dt.DefaultView;
            combFromCurrency.DisplayMemberPath = "Text";
            combFromCurrency.SelectedValuePath = "Value";
            combFromCurrency.SelectedIndex = 0;

            combToCurrency.ItemsSource = dt.DefaultView;
            combToCurrency.DisplayMemberPath = "Text";
            combToCurrency.SelectedValuePath = "Value";
            combToCurrency.SelectedIndex = 0;
        }
       

        private void txtCurrency_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;
            if(txtCurrency.Text==null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }
            else if(combFromCurrency.SelectedValue==null || combFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Enter Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                combFromCurrency.Focus();
                return;
            }
            if (combFromCurrency.Text == combToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);

                lblCurrency.Content = combToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                ConvertedValue = (double.Parse(combToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text))/
                    double.Parse(combFromCurrency.SelectedValue.ToString());

                lblCurrency.Content = combToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
            GetValue();
        }
        private void ClearControls()
        {
            txtCurrency.Text = String.Empty;
            if (combFromCurrency.Items.Count > 0)
                combFromCurrency.SelectedIndex = 0;
            if (combToCurrency.Items.Count > 0)
                combToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "0.0";
            txtCurrency.Focus();
        }

        private void Savet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please eneter any amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please eneter currency Name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    if (CurrencyId > 0)
                    {
                        if (MessageBox.Show("Confrim that you want to update please.", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);
                        {
                            myConnection();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id",sqlConnection);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            sqlConnection.Close();

                            MessageBox.Show("Data updated Successfuly", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Do you want to Save ?.", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            myConnection();
                            cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", sqlConnection);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            sqlConnection.Close();

                            MessageBox.Show("Data Saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearMAster();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ClearMAster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                Save.Content = "Save";
                GetData1();
                CurrencyId = 0;
                NewBindCurrency();
                txtAmount.Focus();
            }
            catch (Exception  ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
               
            }
        }

        private void GetData1()
        {
            myConnection();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Currency_Master", sqlConnection);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter = new SqlDataAdapter(cmd);
            SqlDataAdapter.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
                dgvCurrency.ItemsSource = dt.DefaultView;
            else
                dgvCurrency.ItemsSource = null;
            sqlConnection.Close();

        }

        private void Cancelr_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;
                DataRowView row_selected = grd.CurrentItem as DataRowView;

                if (row_selected!=null)
                {
                    if (dgvCurrency.Items.Count>0)
                    {
                        if (grd.SelectedCells.Count>0)
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());
                            if (grd.SelectedCells[0].Column.DisplayIndex==0)
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                Save.Content = "Update";
                            }
                            if (grd.SelectedCells[0].Column.DisplayIndex==1)
                            {
                                if (MessageBox.Show("Are you sure you want to Delete ?.", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    myConnection();
                                    DataTable dt = new DataTable();
                                    cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", sqlConnection);
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);                                 
                                    cmd.ExecuteNonQuery();
                                    sqlConnection.Close();


                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMAster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception es)
            {

                MessageBox.Show(es.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // API
        public static async Task<Root> GetData<T>(string uri)
        {
            var myRoot = new Root();
            try
            {
                using(var client=new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage message = await client.GetAsync(uri);
                    if(message.StatusCode == System.Net.HttpStatusCode.OK){
                        var ResponceString = await message.Content.ReadAsStringAsync();
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString);
                        MessageBox.Show("TimeStamp :"+ResponceString, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        return ResponceObject;
                    }
                    return myRoot;
                }
            }
            catch 
            {

                return myRoot;
            }
        }
        private async void GetValue()
        {
            values = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=9637b4764a0f4f2c978d2f3e6deac91a");
            BindCurrencyAPI();
        }
        
    }
}
