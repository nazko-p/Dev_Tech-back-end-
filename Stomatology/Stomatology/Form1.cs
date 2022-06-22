using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace Stomatology
{
    public partial class Form1 : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["StomatologyConnection"].ConnectionString;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //bool useristrue = false;
            string sql = "SELECT * FROM Users";
            Users user = new Users();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        user.Id = reader.GetInt32(0);
                        user.NickName = reader.GetString(1);
                        user.Password = reader.GetString(2);
                        if (nickNameBox.Text.Equals(user.NickName) && passwordBox.Text.Equals(user.Password))
                        {
                            user.trueuser = true;
                            reader.Close();
                            break;
                        }
                    }

                    if (user.trueuser)
                    {
                        sql = "select distinct Permission.PermissionName, Connection.DoctorId from Connection, Permission, Users where Connection.UserId = " 
                            + user.Id + " and Connection.PermissionId = Permission.PermissionId";
                        command = new SqlCommand(sql, connection);
                        reader = command.ExecuteReader();
                        string permision = "";
                        int id = 0;
                        reader.Read();
                        permision = reader.GetString(0);
                        if(permision.Equals("Врачь"))
                            id = reader.GetInt32(1);
                        reader.Close();

                        if (permision.Equals("Врачь"))
                        {
                            //MessageBox.Show(id.ToString());
                            Doctor_Window dw = new Doctor_Window(connectionString, id);
                            dw.Show();
                        }
                        else if (permision.Equals("Администратор"))
                        {
                            Admin_Window aw = new Admin_Window(connectionString);
                            aw.Show();
                        }
                        else if (permision.Equals("Ресепшн"))
                        {
                            Receptio_Window rw = new Receptio_Window(connectionString);
                            rw.Show();
                        }
                    }
                    else MessageBox.Show("Incorrect");

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Не получилось прочитать из БД. " + ex.Message);
                }
            }
        }
    }
}
