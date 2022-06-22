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
    public partial class Doctor_Window : Form
    {
        string connectionString;
        int daycounter = 0;
        int doctor_id;
        int tooth_id = 0;
        int cell1 = 0, cell2 = 0, cell_index1 = -1,cell_index2=-1,PATIENT_ID;
        public Doctor_Window(string str, int IDS)
        {
            connectionString = str;
            doctor_id = IDS;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime daybefore = DateTime.Now.AddDays(daycounter);
            string date = daybefore.ToString("dd-MM-yyyy");
            string revercedate = daybefore.ToString("yyyy-MM-dd");

            DrawTopic(date);
            DrawCalendar(revercedate);
        }

        private void DrawTopic(string str)
        {
            groupBox2.Controls.Clear();

            Panel toppanel = new Panel();
            toppanel.Location = new Point(0, 0);
            toppanel.Size = new Size(1700, 200);
            toppanel.BackColor = Color.LightBlue;

            Label dayLable = new Label();
            dayLable.Text = str;
            dayLable.Location = new Point(600, 100);
            dayLable.Size = new Size(500, 100);
            dayLable.Font = new Font("Arial", 64);

            Button backButton = new Button();
            backButton.Text = "<";
            backButton.Font = new Font("Arial", 64);
            backButton.Location = new Point(450, 100);
            backButton.Size = new Size(100, 100);
            backButton.BackColor = Color.Gray;
            backButton.Click += new EventHandler(back);

            Button forwardButton = new Button();
            forwardButton.Text = ">";
            forwardButton.Font = new Font("Arial", 64);
            forwardButton.Location = new Point(1100, 100);
            forwardButton.Size = new Size(100, 100);
            forwardButton.BackColor = Color.Gray;
            forwardButton.Click += new EventHandler(forward);


            toppanel.Controls.Add(dayLable);
            toppanel.Controls.Add(backButton);
            toppanel.Controls.Add(forwardButton);
            groupBox2.Controls.Add(toppanel);
        }

        private void DrawCalendar(string str)
        {
            Panel calendarpanel = new Panel();
            calendarpanel.Location = new Point(0,200);
            calendarpanel.Size = new Size(1700,800);
            calendarpanel.BackColor = Color.LightYellow;

            DataGridView dataGridView = new DataGridView();
            dataGridView.Width = 1143;
            dataGridView.Height = 700;
            dataGridView.Location = new Point(250,10);

            dataGridView.Columns.Add("column-1", "Время приема");
            dataGridView.Columns.Add("column-2", "Пациент");
            dataGridView.Columns.Add("column-3", "Врач");
            dataGridView.Columns.Add("column-4", "Тип приема");
            dataGridView.Columns.Add("column-5", "Поставленый диагноз");
            dataGridView.Columns.Add("column-6", "Заключение");

            dataGridView.Columns[1].Width = 200;
            dataGridView.Columns[2].Width = 200;
            dataGridView.Columns[3].Width = 200;
            dataGridView.Columns[4].Width = 200;
            dataGridView.Columns[5].Width = 200;

            calendarpanel.Controls.Add(dataGridView);
            groupBox2.Controls.Add(calendarpanel);

            string sql = "select Reception.ReceptionDay,Reception.ReceptionTime,Patient.Surname, Patient.Firstname, Patient.Secondname, Doctor.Surname, Doctor.Firstname, Doctor.Secondname, Type_of_Reception.TypeName, Diagnosis.DiagnosisName, Reception.Discription from Reception,Patient,Doctor, Type_of_Reception, Diagnosis " +
                "where Reception.DoctorId = " + doctor_id + " and Reception.ReceptionDay = '" + str + "' and Patient.PatientId = Reception.PatientId and Doctor.DoctorId = Reception.DoctorId and Reception.typeId = Type_of_Reception.TypeId and Reception.DiagnosisId = Diagnosis.DiagnosisId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    int count = 0;
                    List<Reception> reception = new List<Reception>();

                    while (reader.Read())
                    {
                        reception.Add(new Reception());
                        reception[count].date = reader.GetDateTime(0).ToShortDateString();
                        reception[count].time = reader.GetString(1);
                        reception[count].p_surname = reader.GetString(2);
                        reception[count].p_firstname = reader.GetString(3);
                        reception[count].p_secondname = reader.GetString(4);
                        reception[count].p_full = reception[count].p_surname + " " + reception[count].p_firstname + " " + reception[count].p_secondname;
                        reception[count].d_surname = reader.GetString(5);
                        reception[count].d_firstname = reader.GetString(6);
                        reception[count].d_secondname = reader.GetString(7);
                        reception[count].d_full = reception[count].d_surname + " " + reception[count].d_firstname + " " + reception[count].d_secondname;
                        reception[count].type_name = reader.GetString(8);
                        reception[count].diagnosis_name = reader.GetString(9);
                        reception[count].discription = reader.GetString(10);
                        count++;
                    }
                    if(count > 0)
                    {
                        Reception[] receptions = new Reception[reception.Count];
                        int[,] time_arr = new int[reception.Count, 2];

                        for(int i = 0; i < receptions.Length; i++)
                        {
                            receptions[i] = reception[i];

                            string[] sub = reception[i].time.Split(':');
                            time_arr[i, 0] = Int32.Parse(sub[0]);
                            time_arr[i, 1] = Int32.Parse(sub[1]);
                        }

                        reception.Clear();
                        SortTime(receptions, time_arr);

                        for (int i = 0; i < receptions.Length; i++)
                        {
                            dataGridView.Rows.Add(
                                new object[]
                                {
                                receptions[i].time,
                                receptions[i].p_full,
                                receptions[i].d_full,
                                receptions[i].type_name,
                                receptions[i].diagnosis_name,
                                receptions[i].discription
                                }
                            );

                            dataGridView.Rows.Add(
                                new object[]
                                {
                                null,
                                null,
                                null,
                                null,
                                null,
                                null
                                }
                            );
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void SortTime(Reception[] r, int[,] t)
        {
            for (int i = 0; i < r.Length; i++)
            {
                if (i == r.Length - 1)
                    break;
                for (int iter = i+1; iter < r.Length; iter++)
                {
                    if (t[i, 0] > t[iter, 0] || (t[i, 0] == t[iter, 0] && t[i, 1] > t[iter, 1]))
                    {
                        int a, b;
                        a = t[i, 0];
                        t[i, 0] = t[iter, 0];
                        t[iter, 0] = a;

                        b = t[i, 1];
                        t[i, 1] = t[iter, 1];
                        t[iter, 1] = b;

                        Reception j;
                        j = r[i];
                        r[i] = r[iter];
                        r[iter] = j;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox2.Controls.Clear();

            DrawPatientField();
        }

        protected void back(object sender, EventArgs e)
        {
            daycounter--;
            DateTime daybefore = DateTime.Now.AddDays(daycounter);
            string date = daybefore.ToString("dd-MM-yyyy");
            string revercedate = daybefore.ToString("yyyy-MM-dd");

            DrawTopic(date);
            DrawCalendar(revercedate);
        }

        protected void forward(object sender, EventArgs e)
        {
            daycounter++;
            DateTime daybefore = DateTime.Now.AddDays(daycounter);
            string date = daybefore.ToString("dd-MM-yyyy");
            string revercedate = daybefore.ToString("yyyy-MM-dd");

            DrawTopic(date);
            DrawCalendar(revercedate);
        }

        private void DrawPatientField()
        {

            Panel midlepanel = new Panel();
            midlepanel.BackColor = Color.LightYellow;
            midlepanel.Location = new Point(0, 200);
            midlepanel.Size = new Size(1450, 370);

            Panel bottompanel = new Panel();
            bottompanel.BackColor = Color.LightYellow;
            bottompanel.Location = new Point(0, 570);
            bottompanel.Size = new Size(1700, 500);

            Panel rightpanel = new Panel();
            rightpanel.BackColor = Color.LightYellow;
            rightpanel.Location = new Point(1450, 200);
            rightpanel.Size = new Size(250, 780);

            Panel toppanel = new Panel();
            toppanel.BackColor = Color.LightYellow;
            toppanel.Location = new Point(0, 0);
            toppanel.Size = new Size(1700, 200);
            Label patientLable = new Label();
            patientLable.Text = "Пациент:";
            patientLable.Location = new Point(5, 5);
            patientLable.Font = new Font("Arial", 14);
            toppanel.Controls.Add(patientLable);
            ComboBox comboBox1 = new ComboBox();
            comboBox1.Location = new Point(110, 6);
            comboBox1.Width = 200;
            List<Person> person = new List<Person>();
            FillComboBox1(comboBox1,person);
            toppanel.Controls.Add(comboBox1);
            Button findButton = new Button();
            findButton.Text = "Найти";
            findButton.Location = new Point(320, 5);
            findButton.Width = 70;
            findButton.Height = 25;
            findButton.BackColor = Color.LightGray;
            toppanel.Controls.Add(findButton);
            DataGridView dataGridView = new DataGridView();
            dataGridView.Location = new Point(10,35);
            dataGridView.Columns.Add("column-1", "№");
            dataGridView.Columns[0].Width = 30;
            dataGridView.Columns.Add("column-2", "Дата");
            dataGridView.Columns.Add("column-3", "Время приема");
            dataGridView.Columns.Add("column-4", "Пациент");
            dataGridView.Columns[2].Width = 200;
            dataGridView.Columns.Add("column-5", "Врач");
            dataGridView.Columns[3].Width = 200;
            dataGridView.Columns.Add("column-6", "Тип приема");
            dataGridView.Columns[4].Width = 150;
            dataGridView.Columns.Add("column-7", "Поставленый диагноз");
            dataGridView.Columns[5].Width = 150;
            dataGridView.Columns.Add("column-8", "Заключение");
            dataGridView.Columns[6].Width = 150;
            dataGridView.Width = 1130;
            toppanel.Controls.Add(dataGridView);
            DateTimePicker dateTimePicker = new DateTimePicker();
            dateTimePicker.Location = new Point(1140,5);
            toppanel.Controls.Add(dateTimePicker);
            Label doctorLable = new Label();
            doctorLable.Text = "Врачь:";
            doctorLable.Font = new Font("Arial", 14);
            doctorLable.Location = new Point(1300,25);
            toppanel.Controls.Add(doctorLable);
            ComboBox comboBox2 = new ComboBox();
            comboBox2.Width = 200;
            comboBox2.Location = new Point(1400, 26);
            List<Doctor> doctor = new List<Doctor>();
            FillComboBox2(comboBox2, doctor);
            toppanel.Controls.Add(comboBox2);
            Button updateButton = new Button();
            updateButton.Text = "Обновить";
            updateButton.BackColor = Color.LightGray;
            updateButton.Location = new Point(1300,60);
            toppanel.Controls.Add(updateButton);
            Button resetButton = new Button();
            resetButton.Text = "Сбросить";
            resetButton.BackColor = Color.LightGray;
            resetButton.Location = new Point(1380, 60);
            toppanel.Controls.Add(resetButton);
            int patient_id = 0;
            int reception_id = 0;
            List<Diagnosis> diagnoses = new List<Diagnosis>();
            findButton.Click += new EventHandler(find);
            void find(object sender, EventArgs e)
            {
                dataGridView.Rows.Clear();

                for (int i = 0; i < person.Count; i++)
                {
                    if (comboBox1.Text == person[i].FIO)
                    {
                        patient_id = person[i].Id;
                        PATIENT_ID = patient_id;
                        break;
                    }
                }
                string sql = "select Reception.ReceptionId,Reception.ReceptionDay,Reception.ReceptionTime,Patient.Surname, Patient.Firstname, Patient.Secondname, Doctor.Surname, Doctor.Firstname, Doctor.Secondname, Type_of_Reception.TypeName, Diagnosis.DiagnosisName, Reception.Discription from Reception,Patient,Doctor, Type_of_Reception, Diagnosis " +
                    "where Reception.PatientId = " + patient_id + " and Patient.PatientId = Reception.PatientId and Doctor.DoctorId = Reception.DoctorId and Reception.typeId = Type_of_Reception.TypeId and Reception.DiagnosisId = Diagnosis.DiagnosisId";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sql, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        //List<Reception> reception = new List<Reception>();
                        Reception reception = new Reception();
                        int count = 0;

                        while (reader.Read())
                        {
                            //reception.Add(new Reception());
                            reception_id = reader.GetInt32(0);
                            reception.date = reader.GetDateTime(1).ToShortDateString();
                            reception.time = reader.GetString(2);
                            reception.p_surname = reader.GetString(3);
                            reception.p_firstname = reader.GetString(4);
                            reception.p_secondname = reader.GetString(5);
                            reception.p_full = reception.p_surname + " " + reception.p_firstname + " " + reception.p_secondname;
                            reception.d_surname = reader.GetString(6);
                            reception.d_firstname = reader.GetString(7);
                            reception.d_secondname = reader.GetString(8);
                            reception.d_full = reception.d_surname + " " + reception.d_firstname + " " + reception.d_secondname;
                            reception.type_name = reader.GetString(9);
                            reception.diagnosis_name = reader.GetString(10);
                            reception.discription = reader.GetString(11);

                            dataGridView.Rows.Add(
                                new object[]
                                {
                                    reception_id,
                                    reception.date,
                                    reception.time,
                                    reception.p_full,
                                    reception.d_full,
                                    reception.type_name,
                                    reception.diagnosis_name,
                                    reception.discription
                                }
                            );
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                bottompanel.Controls.Clear();
                Label inspectationLable = new Label();
                inspectationLable.Text = "Обследование";
                inspectationLable.Font = new Font("Arial", 14);
                inspectationLable.Location = new Point(5, 5);
                inspectationLable.Width = 200;
                bottompanel.Controls.Add(inspectationLable);
                DataGridView dataGridView2 = new DataGridView();
                dataGridView2.Location = new Point(5, 30);
                dataGridView2.Width = 845;
                dataGridView2.Columns.Add("column-1", "Шифр");
                dataGridView2.Columns.Add("column-2", "Дата");
                dataGridView2.Columns.Add("column-3", "Услуга");
                dataGridView2.Columns[2].Width = 300;
                dataGridView2.Columns.Add("column-4", "Объект");
                dataGridView2.Columns.Add("column-5", "Статус");
                dataGridView2.Columns.Add("column-6", "Описание");
                bottompanel.Controls.Add(dataGridView2);
                Label treatmentLable = new Label();
                treatmentLable.Text = "Терапия";
                treatmentLable.Font = new Font("Arial", 14);
                treatmentLable.Location = new Point(5, 180);
                bottompanel.Controls.Add(treatmentLable);
                DataGridView dataGridView3 = new DataGridView();
                dataGridView3.Location = new Point(5, 205);
                dataGridView3.Width = 845;
                dataGridView3.Columns.Add("column-1", "Шифр");
                dataGridView3.Columns.Add("column-2", "Дата");
                dataGridView3.Columns.Add("column-3", "Услуга");
                dataGridView3.Columns[2].Width = 300;
                dataGridView3.Columns.Add("column-4", "Объект");
                dataGridView3.Columns.Add("column-5", "Статус");
                dataGridView3.Columns.Add("column-6", "Описание");
                bottompanel.Controls.Add(dataGridView3);
                GroupBox groupBox3 = new GroupBox();
                groupBox3.Text = "";
                groupBox3.Location = new Point(860, 20);
                groupBox3.Height = 330;
                bottompanel.Controls.Add(groupBox3);
                Button addButton = new Button();
                addButton.Text = "Добавить";
                addButton.Location = new Point(25,20);
                addButton.Font = new Font("Arial",14);
                addButton.Width = 150;
                addButton.Height = 30;
                addButton.BackColor = Color.LightGray;
                addButton.Click += new EventHandler(add_click);
                groupBox3.Controls.Add(addButton);
                Button removeButton = new Button();
                removeButton.Text = "Удалить";
                removeButton.Location = new Point(25, 50);
                removeButton.Font = new Font("Arial", 14);
                removeButton.Width = 150;
                removeButton.Height = 30;
                removeButton.BackColor = Color.LightGray;
                removeButton.Click += new EventHandler(remove_click);
                groupBox3.Controls.Add(removeButton);
                RichTextBox richTextBox = new RichTextBox();
                richTextBox.Location = new Point(1100, 25);
                richTextBox.Size = new Size(300,300);
                bottompanel.Controls.Add(richTextBox);
                dataGridView2.CellClick += new DataGridViewCellEventHandler(cellclick1);
                dataGridView3.CellClick += new DataGridViewCellEventHandler(cellclick2);
                void cellclick1(object sender_cell, DataGridViewCellEventArgs e_cell)
                {
                    if (e_cell.RowIndex != -1)
                    {
                        DataGridViewRow dgvRow = dataGridView2.Rows[e_cell.RowIndex];
                        richTextBox.Clear();
                        richTextBox.Text = dgvRow.Cells[5].Value.ToString();
                        cell1 = Int32.Parse(dgvRow.Cells[0].Value.ToString());
                        cell_index1 = e_cell.RowIndex;
                    }
                }
                void cellclick2(object sender_cell, DataGridViewCellEventArgs e_cell)
                {
                    if (e_cell.RowIndex != -1)
                    {
                        DataGridViewRow dgvRow = dataGridView3.Rows[e_cell.RowIndex];
                        richTextBox.Clear();
                        richTextBox.Text = dgvRow.Cells[5].Value.ToString();
                        cell2 = Int32.Parse(dgvRow.Cells[0].Value.ToString());
                        cell_index2 = e_cell.RowIndex;
                    }
                }

                midlepanel.Controls.Clear();
                Tooth_Button[] toothButton = new Tooth_Button[32];
                int teethcount = 1, t_x = 250;
                for (int i = 0; i < toothButton.Length / 2; i++)
                {
                    toothButton[i] = new Tooth_Button();
                    toothButton[i].tooth_button = new Button();
                    toothButton[i].tooth_button.Size = new Size(64, 64);
                    toothButton[i].tooth_button.Text = teethcount.ToString();
                    toothButton[i].tooth_button.Location = new Point(t_x, 80);
                    toothButton[i].tooth_button.BackColor = Color.White;
                    toothButton[i].key = teethcount - 1;
                    toothButton[i].clickevent(toothButton, patient_id, dataGridView2, dataGridView3, connectionString);
                    midlepanel.Controls.Add(toothButton[i].tooth_button);
                    teethcount++;
                    t_x += 64;
                }
                t_x = 250;
                for (int i = toothButton.Length / 2; i < toothButton.Length; i++)
                {
                    toothButton[i] = new Tooth_Button();
                    toothButton[i].tooth_button = new Button();
                    toothButton[i].tooth_button.Size = new Size(64, 64);
                    toothButton[i].tooth_button.Text = teethcount.ToString();
                    toothButton[i].tooth_button.Location = new Point(t_x, 180);
                    toothButton[i].tooth_button.BackColor = Color.White;
                    toothButton[i].key = teethcount - 1;
                    toothButton[i].clickevent(toothButton, patient_id, dataGridView2, dataGridView3, connectionString);
                    midlepanel.Controls.Add(toothButton[i].tooth_button);
                    teethcount++;
                    t_x += 64;
                }

                RichTextBox richTextBox1 = new RichTextBox();
                richTextBox1.Size = new Size(240, 500);
                rightpanel.Controls.Add(richTextBox1);
                Button updateRightButton = new Button();
                updateRightButton.Location = new Point(100, 550);
                updateRightButton.Text = "Обновить";
                updateRightButton.BackColor = Color.LightGray;
                rightpanel.Controls.Add(updateRightButton);
                ComboBox comboBoxDiagnosis = new ComboBox();
                comboBoxDiagnosis.Location = new Point(25,520);
                comboBoxDiagnosis.Width = 200;
                FillComboBox3(comboBoxDiagnosis, diagnoses);
                rightpanel.Controls.Add(comboBoxDiagnosis);
                dataGridView.CellClick += new DataGridViewCellEventHandler(cellclick3);
                void cellclick3(object sender_cell, DataGridViewCellEventArgs e_cell)
                {
                    if (e_cell.RowIndex != -1)
                    {
                        DataGridViewRow dgvRow = dataGridView.Rows[e_cell.RowIndex];
                        richTextBox1.Clear();
                        richTextBox1.Text = dgvRow.Cells[7].Value.ToString();
                        reception_id = Int32.Parse(dgvRow.Cells[0].Value.ToString());
                        comboBoxDiagnosis.Text = "";
                        comboBoxDiagnosis.Text = dgvRow.Cells[6].Value.ToString();
                    }
                }
                updateRightButton.Click += new EventHandler(update_click);
                void update_click(object sender_update, EventArgs e_update)
                {
                    sql = "update Reception set Discription='" + richTextBox1.Text + "', DiagnosisId = " + FindDiagnosisId(comboBoxDiagnosis.Text) + " where ReceptionId = " + reception_id;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(sql, connection);
                            command.ExecuteNonQuery();
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                void remove_click(object sender_update, EventArgs e_update)
                {
                    sql = "delete from Inspectation where InspectationId = " + cell1;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(sql, connection);
                            command.ExecuteNonQuery();

                            sql = "delete from Treatment where TreatmentId = " + cell2;
                            command = new SqlCommand(sql, connection);
                            command.ExecuteNonQuery();

                            sql = "delete from DentalCard where InspectationId = " + cell1 + " and TreatmentId = " + cell2;
                            command = new SqlCommand(sql, connection);
                            command.ExecuteNonQuery();

                            MessageBox.Show("Процедура прошла успешно!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            updateButton.Click += new EventHandler(update);
            void update(object sender, EventArgs e)
            {
                dataGridView.Rows.Clear();
                int doctor_ID = 0;
                string choosen_date = dateTimePicker.Value.ToString("yyyy-MM-dd");

                for (int i = 0; i < person.Count; i++)
                {
                    if (comboBox1.Text == person[i].FIO)
                    {
                        patient_id = person[i].Id;
                        break;
                    }
                }
                for (int i = 0; i < doctor.Count; i++)
                {
                    if (comboBox2.Text == doctor[i].FIO)
                    {
                        doctor_ID = doctor[i].Id;
                        break;
                    }
                }

                string sql = "select ReceptionId,Reception.ReceptionDay,Reception.ReceptionTime,Patient.Surname, Patient.Firstname, Patient.Secondname, Doctor.Surname, Doctor.Firstname, Doctor.Secondname, Type_of_Reception.TypeName, Diagnosis.DiagnosisName, Reception.Discription from Reception,Patient,Doctor, Type_of_Reception, Diagnosis " +
                    "where Reception.DoctorId = " + doctor_ID + " and Reception.PatientId = " + patient_id + " and Reception.ReceptionDay = '" + choosen_date + "' and Patient.PatientId = Reception.PatientId and Doctor.DoctorId = Reception.DoctorId and Reception.typeId = Type_of_Reception.TypeId and Reception.DiagnosisId = Diagnosis.DiagnosisId";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try 
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sql, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        Reception reception = new Reception();

                        while (reader.Read())
                        {
                            //reception.Add(new Reception());
                            int id = reader.GetInt32(0);
                            reception.date = reader.GetDateTime(1).ToShortDateString();
                            reception.time = reader.GetString(2);
                            reception.p_surname = reader.GetString(3);
                            reception.p_firstname = reader.GetString(4);
                            reception.p_secondname = reader.GetString(5);
                            reception.p_full = reception.p_surname + " " + reception.p_firstname + " " + reception.p_secondname;
                            reception.d_surname = reader.GetString(6);
                            reception.d_firstname = reader.GetString(7);
                            reception.d_secondname = reader.GetString(8);
                            reception.d_full = reception.d_surname + " " + reception.d_firstname + " " + reception.d_secondname;
                            reception.type_name = reader.GetString(9);
                            reception.diagnosis_name = reader.GetString(10);
                            reception.discription = reader.GetString(11);

                            dataGridView.Rows.Add(
                                new object[]
                                {
                                    id,
                                    reception.date,
                                    reception.time,
                                    reception.p_full,
                                    reception.d_full,
                                    reception.type_name,
                                    reception.diagnosis_name,
                                    reception.discription
                                }
                            );
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            resetButton.Click += new EventHandler(find);

            groupBox2.Controls.Add(toppanel);
            groupBox2.Controls.Add(midlepanel);
            groupBox2.Controls.Add(rightpanel);
            groupBox2.Controls.Add(bottompanel);
        }

        private void FillComboBox1(ComboBox comboBox, List<Person> p)
        {
            string sql = "select PatientId ,Surname, Firstname, Secondname from Patient";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    int count = 0;

                    while (reader.Read())
                    {
                        p.Add(new Person());
                        p[count].Id = reader.GetInt32(0);
                        p[count].Surname = reader.GetString(1);
                        p[count].Firstname = reader.GetString(2);
                        p[count].Secondname = reader.GetString(3);

                        comboBox.Items.Add(p[count].FIO);
                        count++;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void FillComboBox2(ComboBox comboBox, List<Doctor> d)
        {
            string sql = "select DoctorId ,Surname, Firstname, Secondname from Doctor";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    int count = 0;

                    while (reader.Read())
                    {
                        d.Add(new Doctor());
                        d[count].Id = reader.GetInt32(0);
                        d[count].Surname = reader.GetString(1);
                        d[count].Firstname = reader.GetString(2);
                        d[count].Secondname = reader.GetString(3);

                        comboBox.Items.Add(d[count].FIO);
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void add_click(object sender, EventArgs e)
        {
            AddForm addForm = new AddForm(connectionString,PATIENT_ID);
            addForm.Show();
        }

        private void FillComboBox3(ComboBox comboBox, List<Diagnosis> d)
        {
            string sql = "select DiagnosisId , DiagnosisName from Diagnosis";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    int count = 0;

                    while (reader.Read())
                    {
                        d.Add(new Diagnosis());
                        d[count].Id = reader.GetInt32(0);
                        d[count].Name = reader.GetString(1);

                        comboBox.Items.Add(d[count].Name);
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private int FindDiagnosisId(string str)
        {
            string sql = "select DiagnosisId , DiagnosisName from Diagnosis";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    Diagnosis d = new Diagnosis();

                    while (reader.Read())
                    {
                        d.Id = reader.GetInt32(0);
                        d.Name = reader.GetString(1);

                        if (d.Name.Equals(str))
                            return d.Id;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return 0;
        }
    }

    class Tooth_Button {
        public Button tooth_button;
        public int key;
        public bool switch_triger = false;

        public void clickevent(Tooth_Button[] tb, int p_id,DataGridView dv1, DataGridView dv2,string connectionString)
        {
            this.tooth_button.Click += new EventHandler(sWitch);

            void sWitch(object sender, EventArgs e)
            {
                if (switch_triger)
                {
                    dv1.Rows.Clear();
                    dv2.Rows.Clear();
                    tb[key].tooth_button.BackColor = Color.White;
                    switch_triger = false;
                }
                else
                {
                    //dv1.Rows.Clear();
                    //dv2.Rows.Clear();
                    int f = key + 1;
                    string sql = "select DentalCard.CardId, Inspectation.InspectationDate, Inspectation.InspectationName, DentalCard.ToothId, Inspectation.InspectationState, Inspectation.InspectationDiscription from DentalCard, Inspectation " +
                        "where DentalCard.ToothId = " + f + " and Inspectation.InspectationId = DentalCard.InspectationId and DentalCard.PatientId = " + p_id;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(sql, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            DentalCard dentalCard = new DentalCard();

                            while (reader.Read())
                            {
                                dentalCard.id = reader.GetInt32(0);
                                dentalCard.date = reader.GetDateTime(1).ToShortDateString();
                                dentalCard.name = reader.GetString(2);
                                dentalCard.toothid = reader.GetInt32(3);
                                dentalCard.state = reader.GetString(4);
                                dentalCard.discription = reader.GetString(5);

                                dv1.Rows.Add(
                                new object[]
                                {
                                    dentalCard.id,
                                    dentalCard.date,
                                    dentalCard.name,
                                    dentalCard.toothid,
                                    dentalCard.state,
                                    dentalCard.discription
                                }
                                );
                            }
                            reader.Close();

                            sql = "select DentalCard.CardId, Treatment.TreatmentDate, Treatment.TreatmentName, DentalCard.ToothId, Treatment.TreatmentState, Treatment.TreatmentDiscription from DentalCard, Treatment " +
                                "where DentalCard.ToothId = " + f + " and Treatment.TreatmentId = DentalCard.TreatmentId and DentalCard.PatientId = " + p_id;
                            command = new SqlCommand(sql, connection);
                            reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                dentalCard.id = reader.GetInt32(0);
                                dentalCard.date = reader.GetDateTime(1).ToShortDateString();
                                dentalCard.name = reader.GetString(2);
                                dentalCard.toothid = reader.GetInt32(3);
                                dentalCard.state = reader.GetString(4);
                                dentalCard.discription = reader.GetString(5);

                                dv2.Rows.Add(
                                new object[]
                                {
                                    dentalCard.id,
                                    dentalCard.date,
                                    dentalCard.name,
                                    dentalCard.toothid,
                                    dentalCard.state,
                                    dentalCard.discription
                                }
                                );
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                    tb[key].tooth_button.BackColor = Color.Purple;
                    switch_triger = true;
                }
            }
        }
    }
}
