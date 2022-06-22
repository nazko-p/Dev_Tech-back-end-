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
    public partial class Receptio_Window : Form
    {
        string connectionString;
        public Receptio_Window(string str)
        {
            connectionString = str;
            InitializeComponent();

            FillComboBoxes();
        }

        public void FillComboBoxes()
        {
            Doctor doctor = new Doctor();
            Person person = new Person();
            Type_of_Reception TOP = new Type_of_Reception();

            string sql = "select Surname, Firstname, Secondname from Doctor";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        doctor.Surname = reader.GetString(0);
                        doctor.Firstname = reader.GetString(1);
                        doctor.Secondname = reader.GetString(2);

                        comboBox1.Items.Add(doctor.FIO);
                    }
                    reader.Close();

                    sql = "select Surname, Firstname, Secondname from Patient";
                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        person.Surname = reader.GetString(0);
                        person.Firstname = reader.GetString(1);
                        person.Secondname = reader.GetString(2);

                        comboBox2.Items.Add(person.FIO);
                    }
                    reader.Close();

                    sql = "select TypeName from Type_of_Reception";
                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TOP.TypeName = reader.GetString(0);

                        comboBox3.Items.Add(TOP.TypeName);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error2 " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Person person = new Person();

            person.Surname = textBox1.Text;
            person.Firstname = textBox2.Text;
            person.Secondname = textBox3.Text;

            person.DateOfBirth = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            person.Adress = textBox4.Text;
            person.PhoneNum = Int32.Parse(textBox5.Text);

            string sql = "insert into Patient (Surname, Firstname, Secondname, DateOfBirth, PT_Adress, PT_Phone) " +
                        "values (@Surname, @Firstname, @Secondname, @Date, @Adress, @Phone)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@Surname", SqlDbType.VarChar, 25);
                    command.Parameters.Add("@Firstname", SqlDbType.VarChar, 25);
                    command.Parameters.Add("@Secondname", SqlDbType.VarChar, 25);
                    command.Parameters.Add("@Date", SqlDbType.Date);
                    command.Parameters.Add("@Adress", SqlDbType.VarChar, 100);
                    command.Parameters.Add("@Phone", SqlDbType.Int);

                    command.Parameters["@Surname"].Value = person.Surname;
                    command.Parameters["@Firstname"].Value = person.Firstname;
                    command.Parameters["@Secondname"].Value = person.Secondname;
                    command.Parameters["@Date"].Value = person.DateOfBirth;
                    command.Parameters["@Adress"].Value = person.Adress;
                    command.Parameters["@Phone"].Value = person.PhoneNum;

                    command.ExecuteNonQuery();
                    MessageBox.Show("Пациент успешно добавлен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error2 " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Reception reception = new Reception();
            reception.date = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            reception.time = textBox6.Text;
            reception.cab_num = Int32.Parse(textBox9.Text);

            string sql = "insert into Reception (ReceptionDay, ReceptionTime, CabNum, DoctorId, PatientId, TypeId, DiagnosisId, Discription) " +
                        "values (@Date, @Time, @Cab, @DID, @PID, @TID, @DiagID, @Discription)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@Date", SqlDbType.Date);
                    command.Parameters.Add("@Time", SqlDbType.VarChar, 5);
                    command.Parameters.Add("@Cab", SqlDbType.Int);
                    command.Parameters.Add("@DID", SqlDbType.Int);
                    command.Parameters.Add("@PID", SqlDbType.Int);
                    command.Parameters.Add("@TID", SqlDbType.Int);
                    command.Parameters.Add("@DiagID", SqlDbType.Int);
                    command.Parameters.Add("@Discription", SqlDbType.VarChar, 225);

                    command.Parameters["@Date"].Value = reception.date;
                    command.Parameters["@Time"].Value = reception.time;
                    command.Parameters["@Cab"].Value = reception.cab_num;
                    command.Parameters["@DID"].Value = doctor_id();
                    command.Parameters["@PID"].Value = patient_id();
                    command.Parameters["@TID"].Value = type_id();
                    command.Parameters["@DiagID"].Value = 2;
                    command.Parameters["@Discription"].Value = "Неопределено";

                    command.ExecuteNonQuery();
                    MessageBox.Show("Прием успешно добавлен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error2 " + ex.Message);
                }
            }

            int doctor_id()
            {
                Doctor d = new Doctor();
                sql = "select DoctorId, Surname, Firstname, Secondname from Doctor";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sql, connection);
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            d.Id = reader.GetInt32(0);
                            d.Surname = reader.GetString(1);
                            d.Firstname = reader.GetString(2);
                            d.Secondname = reader.GetString(3);

                            if (d.FIO.Equals(comboBox1.Text))
                            {
                                return d.Id;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error1 " + ex.Message);
                    }
                }

                return 0;
            }
            int patient_id()
            {
                Person p = new Person();
                sql = "select PatientId, Surname, Firstname, Secondname from Patient";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sql, connection);
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            p.Id = reader.GetInt32(0);
                            p.Surname = reader.GetString(1);
                            p.Firstname = reader.GetString(2);
                            p.Secondname = reader.GetString(3);

                            if (p.FIO.Equals(comboBox2.Text))
                            {
                                return p.Id;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error1 " + ex.Message);
                    }
                }
                return 0;
            }
            int type_id()
            {
                Type_of_Reception t = new Type_of_Reception();
                sql = "select TypeId, TypeName from Type_of_Reception";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sql, connection);
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            t.Id = reader.GetInt32(0);
                            t.TypeName = reader.GetString(1);

                            if (t.TypeName.Equals(comboBox3.Text))
                            {
                                return t.Id;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error1 " + ex.Message);
                    }
                }
                return 0;
            }
        }
    }
}
