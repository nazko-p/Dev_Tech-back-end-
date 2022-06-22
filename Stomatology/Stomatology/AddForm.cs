using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Stomatology
{
    public partial class AddForm : Form
    {
        string connectionString;
        int patient_id;
        public AddForm(string str, int p_id)
        {
            connectionString = str;
            patient_id = p_id;
            InitializeComponent();

            comboBox1.Items.Add("Выполнено");
            comboBox1.Items.Add("Не выполнено");

            comboBox2.Items.Add("Выполнено");
            comboBox2.Items.Add("Не выполнено");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DentalCard[] dentalCard = new DentalCard[2];
            int id;
            dentalCard[0] = new DentalCard();
            dentalCard[1] = new DentalCard();

            dentalCard[0].date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            dentalCard[1].date = dateTimePicker2.Value.ToString("yyyy-MM-dd");

            id = Int32.Parse(textBox4.Text);

            dentalCard[0].name = textBox3.Text;
            dentalCard[1].name = textBox6.Text;

            dentalCard[0].state = comboBox1.Text;
            dentalCard[1].state = comboBox2.Text;

            dentalCard[0].discription = textBox5.Text;
            dentalCard[1].discription = textBox1.Text;

            string sql = "insert into Inspectation (InspectationName, InspectationDate, InspectationDiscription, InspectationState) " +
                        "values (@Name, @Date, @Discription, @State)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@Name", SqlDbType.VarChar, 100);
                    command.Parameters.Add("@Date", SqlDbType.Date);
                    command.Parameters.Add("@Discription", SqlDbType.VarChar, 225);
                    command.Parameters.Add("@State", SqlDbType.VarChar, 20);

                    command.Parameters["@Name"].Value = dentalCard[0].name;
                    command.Parameters["@Date"].Value = dentalCard[0].date;
                    command.Parameters["@Discription"].Value = dentalCard[0].discription;
                    command.Parameters["@State"].Value = dentalCard[0].state;

                    command.ExecuteNonQuery();

                    sql = "select InspectationId from Inspectation";
                    command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        dentalCard[0].id = reader.GetInt32(0);
                    }
                    reader.Close();

                    sql = "insert into Treatment (TreatmentName, TreatmentDate, TreatmentDiscription, TreatmentState) " +
                            "values (@Name, @Date, @Discription, @State)";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@Name", SqlDbType.VarChar, 100);
                    command.Parameters.Add("@Date", SqlDbType.Date);
                    command.Parameters.Add("@Discription", SqlDbType.VarChar, 225);
                    command.Parameters.Add("@State", SqlDbType.VarChar, 20);

                    command.Parameters["@Name"].Value = dentalCard[1].name;
                    command.Parameters["@Date"].Value = dentalCard[1].date;
                    command.Parameters["@Discription"].Value = dentalCard[1].discription;
                    command.Parameters["@State"].Value = dentalCard[1].state;

                    command.ExecuteNonQuery();

                    sql = "select TreatmentId from Treatment";
                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        dentalCard[1].id = reader.GetInt32(0);
                    }
                    reader.Close();

                    sql = "insert into DentalCard (InspectationId, TreatmentId, PatientId, ToothId) " +
                            "values (@IID, @TID, @PID, @TOOTHID)";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@IID", SqlDbType.Int);
                    command.Parameters.Add("@TID", SqlDbType.Int);
                    command.Parameters.Add("@PID", SqlDbType.Int);
                    command.Parameters.Add("@TOOTHID", SqlDbType.Int);

                    command.Parameters["@IID"].Value = dentalCard[0].id;
                    command.Parameters["@TID"].Value = dentalCard[1].id;
                    command.Parameters["@PID"].Value = patient_id;
                    command.Parameters["@TOOTHID"].Value = id;
                    
                    command.ExecuteNonQuery();

                    MessageBox.Show("Терапия успешно создана.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error2 " + ex.Message);
                }
            }
        }
    }
}
