using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stomatology
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    class Person
    {
        private int id;
        public int Id { get { return id; } set { id = value; } }

        private string surname;
        private string firstname;
        private string secondname;
        public string Surname { get { return surname; } set { surname = value; } }
        public string Firstname { get { return firstname; } set { firstname = value; } }
        public string Secondname { get { return secondname; } set { secondname = value; } }
        public string FIO { get { return surname + " " + firstname + " " + secondname; } }

        private int phone_num;
        public int PhoneNum { get { return phone_num; } set { phone_num = value; } }

        private string date_of_birth;
        public string DateOfBirth { get { return date_of_birth; } set { date_of_birth = value; } }

        private string adress;
        public string Adress { get { return adress; } set { adress = value; } }
    }

    class Doctor : Person
    {
        private string salary;
        public string Salary { get { return salary; } set { salary = value; } }

    }

    class Type_of_Reception
    {
        private int id;
        public int Id { get { return id; } set { id = value; } }

        private string typename;
        public string TypeName { get { return typename; } set { typename = value; } }

        private string discription;
        public string Discription { get { return discription; } set { discription = value; } }

        private string cost;
        public string Cost { get { return cost; } set { cost = value; } }
    }

    class Diagnosis 
    {
        private int id;
        public int Id { get { return id; } set { id = value; } }

        private string name;
        public string Name { get { return name; } set { name = value; } }

        private string discription;
        public string Discription { get { return discription; } set { discription = value; } } 
    }

    class Users
    {
        private int id;
        public int Id { get { return id; } set { id = value; } }

        private string nickname;
        public string NickName { get { return nickname; } set { nickname = value; } }

        private string password;
        public string Password { get { return password; } set { password = value; } }

        public bool trueuser = false;
    }

    class Reception
    {
        public string date;
        //public string Date { set { date = value; } }
        public string time;
        public string p_surname;
        public string p_firstname;
        public string p_secondname;
        public string p_full;
        public string d_surname;
        public string d_firstname;
        public string d_secondname;
        public string d_full;
        public string type_name;
        public string diagnosis_name;
        public string discription;
        public int cab_num;
    }

    class DentalCard
    {
        public int id;
        public string name;
        public string date;
        public string discription;
        public string state;

        public int toothid;
    }
}
