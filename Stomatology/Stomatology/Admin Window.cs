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
    public partial class Admin_Window : Form
    {
        string connectionString;
        Doctor doctor = new Doctor();
        Users users = new Users();
        bool userisexist = false;
        public Admin_Window(string str)
        {
            connectionString = str;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            doctor.Surname = textBox1.Text;
            doctor.Firstname = textBox2.Text;
            doctor.Secondname = textBox3.Text;

            doctor.DateOfBirth = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            doctor.Adress = textBox4.Text;
            doctor.PhoneNum = Int32.Parse(textBox5.Text);
            doctor.Salary = textBox6.Text;

            string sql = "select UserNickName from Users";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        users.NickName = reader.GetString(0);
                        if (users.NickName.Equals(textBox7.Text))
                        {
                            userisexist = true;
                            break;
                        }
                    }
                    reader.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error1 " + ex.Message);
                }
            }

            if (userisexist)
            {
                MessageBox.Show("Пользователь с таким именем уже существует!");
            }
            else
            {
                users.NickName = textBox7.Text;
                if (textBox8.Text.Equals(textBox9.Text))
                {
                    users.Password = textBox8.Text;
                    sql = "insert into Doctor (Surname, Firstname, Secondname, DateOfBirth, D_Adress, D_Phone, Salary) " +
                        "values (@Surname, @Firstname, @Secondname, @Date, @Adress, @Phone, @Salary)";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(sql, connection);
                            command.Parameters.Add("@Surname",SqlDbType.VarChar, 25);
                            command.Parameters.Add("@Firstname", SqlDbType.VarChar, 25);
                            command.Parameters.Add("@Secondname", SqlDbType.VarChar, 25);
                            command.Parameters.Add("@Date",SqlDbType.Date);
                            command.Parameters.Add("@Adress",SqlDbType.VarChar, 100);
                            command.Parameters.Add("@Phone",SqlDbType.Int);
                            command.Parameters.Add("@Salary",SqlDbType.VarChar,15);

                            command.Parameters["@Surname"].Value = doctor.Surname;
                            command.Parameters["@Firstname"].Value = doctor.Firstname;
                            command.Parameters["@Secondname"].Value = doctor.Secondname;
                            command.Parameters["@Date"].Value = doctor.DateOfBirth;
                            command.Parameters["@Adress"].Value = doctor.Adress;
                            command.Parameters["@Phone"].Value = doctor.PhoneNum;
                            command.Parameters["@Salary"].Value = doctor.Salary;

                            command.ExecuteNonQuery();

                            sql = "select DoctorId from Doctor";
                            command = new SqlCommand(sql, connection);
                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                doctor.Id = reader.GetInt32(0);
                            }
                            reader.Close();

                            sql = "insert into Users (UserNickName, UserPassword) values (@NickName,@Password)";
                            command = new SqlCommand(sql, connection);
                            command.Parameters.Add("@NickName",SqlDbType.VarChar, 20);
                            command.Parameters.Add("@Password",SqlDbType.VarChar, 20);

                            command.Parameters["@NickName"].Value = users.NickName;
                            command.Parameters["@Password"].Value = users.Password;

                            command.ExecuteNonQuery();

                            sql = "select UserId from Users";
                            command = new SqlCommand(sql, connection);
                            reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                users.Id = reader.GetInt32(0);
                            }
                            reader.Close();

                            sql = "insert into Connection (UserId,PermissionId,DoctorId) values (@UID,@PID,@DID)";
                            command = new SqlCommand(sql, connection);
                            command.Parameters.Add("@UID",SqlDbType.Int);
                            command.Parameters.Add("@PID",SqlDbType.Int);
                            command.Parameters.Add("@DID",SqlDbType.Int);

                            command.Parameters["@UID"].Value = users.Id;
                            command.Parameters["@PID"].Value = 1;
                            command.Parameters["@DID"].Value = doctor.Id;

                            command.ExecuteNonQuery();
                            MessageBox.Show("Пользаватель успешно создан.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error2 " + ex.Message);
                        }
                    }
                }
                else MessageBox.Show("Пароли не совпадают!");
            }
        }
    }
}
