using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class frm_main : Form
    {
        string temp = "Температура";
        string hum = "Отн. влажност";
        string pressure = "Налягане";
        string windSpeed = "Скорост на вятъра";
        string windDirection = "Посока на вятъра";
        string rain = "Валеж";
        string rainIntensity = "Интензивност на валежа";
        string radiation = "Слънчева радиация";

        private SQLiteConnection Connection;

        public frm_main()
        {
            InitializeComponent();


            //timer1.Start();
            comboBox1.Items.Add(temp);
            comboBox1.Items.Add(hum);
            comboBox1.Items.Add(pressure);
            comboBox1.Items.Add(windSpeed);
            comboBox1.Items.Add(windDirection);
            comboBox1.Items.Add(rain);
            comboBox1.Items.Add(rainIntensity);
            comboBox1.Items.Add(radiation);

            listBox3.Hide();
            label6.Hide();
            label7.Hide();
            chart1.Hide();


            SQLiteConnection.CreateFile("meteodb.sqlite");
            Connection = new SQLiteConnection("Data Source=meteodb.sqlite;Version=3;");
            Connection.Open();

            string createTable = ("CREATE TABLE meteodata (Station INT(5) NOT NULL, Dat datetime NOT NULL, Temp FLOAT(7, 1), Hum FLOAT(7, 1), Press FLOAT(7, 1), windSpeed FLOAT(7, 1), windDirection FLOAT(7, 1), Rain FLOAT(7, 1), rainIntensity FLOAT(7, 1), notDraw FLOAT(7, 1), notDraw1 FLOAT(7, 1), sunRad FLOAT(7, 1), PRIMARY KEY(Station, Dat));");
            SQLiteCommand createHydDnev = new SQLiteCommand(createTable, Connection);
            createHydDnev.ExecuteNonQuery();
            Connection.Close();
        }

        int i = 0;
        List<DateTime> TimeList = new List<DateTime>();


        private void instructions_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Инструкции за употреба !" + Environment.NewLine +
            Environment.NewLine + "1. Проверете текстовия файл в който се указва директория с файловете.'" +
            Environment.NewLine + "2. Изберете № на станция." +
            Environment.NewLine + "3. Изберете начална и крайна дата от календара." +
            Environment.NewLine + "4. Изберете мерна величина." +
            Environment.NewLine + "5. Натиснете бутона 'Покажи'" +
            Environment.NewLine + "При запитвания/въпроси/проблеми:" +
            Environment.NewLine +
            Environment.NewLine + "E-mail: blagovest.pizhev@meteo.bg");
        }

        private void back_to_main_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_login login = new frm_login();
            login.Show();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void frm_main_Load(object sender, EventArgs e)
        {

            string path = @"C:\Meteo";

            if (Directory.Exists(path))
            {
               // listBox1.Items.Clear();
                string[] files = Directory.GetFiles(path);

                string[] dirs = Directory.GetDirectories(path);

                foreach (string file in files)
                {
                    var formattedName = Path.GetFileName(file).Split('_').First();


                    //Показва и годината и месеца
                    var formattedDates = string.Join("_", Path.GetFileName(file).Split('_', '.').Skip(1).Take(3));
                    var formattedDates2 = string.Join("_", Path.GetFileName(file).Split('_', '.').Skip(1).Take(3));

                    if (!comboBox2.Items.Contains(formattedName))
                    {
                        comboBox2.Items.Add(formattedName);
                    }

                    if (!comboBox3.Items.Contains(formattedDates))
                    {
                        comboBox3.Items.Add(formattedDates);
                    }

                    if (!comboBox4.Items.Contains(formattedDates2))
                    {
                        comboBox4.Items.Add(formattedDates2);
                    }
                        
                    //listBox1.Items.Add(Directory.GetFiles(path));
                    // listBox1.Items.Add(formattedDates);
                    // listBox2.Items.Add(formattedDates);
                }

            }

            else

            {
                MessageBox.Show("Директорията Meteo не е октирта в системен диск 'C:\'");
                Application.ExitThread();
            }

            var pathFiles = Directory.EnumerateFiles(@"C:\Meteo", "*.dat");

            List<string> lines = Directory.EnumerateFiles(@"C:\Meteo", "*.dat").SelectMany(file => File.ReadLines(file)).ToList();



            //Дисплейва само имената на файловете в директорията
            foreach (string file in Directory.EnumerateFiles(@"C:\Meteo", "*.dat"))
            {
                listBox1.Items.Add(file);
            }

            foreach (string file in Directory.EnumerateFiles(@"C:\Meteo", "*.dat"))
            {
                // read each line
                foreach (string line in File.ReadLines(file))
                {
                    // and show file name and line in a message box
                    // MessageBox.Show(line, file);
                    //listBox1.Items.Add(file);
                }
            }

            var data = Directory
           .EnumerateFiles(@"C:\Meteo", "*.dat")
           .SelectMany(file => File.ReadLines(file))
           .Select(line => line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries))
           .ToList();

            //date = DateTime.ParseExact(items[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            //hours = items[2],
            var result = Directory
            .EnumerateFiles(@"C:\Meteo", "*.dat")
            .SelectMany(file => File.ReadLines(file))
            .Select(line => line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(items => new {
                id = items[0],
                date = DateTime.ParseExact(items[1], "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(Convert.ToDouble(items[2].Substring(0, 2))),
               // date = items[1],
                temperature = items[3],
                hum = items[4],
                presure = items[5],
                windSpeed = items[6],
                windDirect = items[7],
                rain = items[8],
                rainIntensity = items[9],
                notDraw = items[10],
                notDraw1 = items[11],
                sunRadiation = items[12],


                /* etc. */
            })
              .ToList();


            var dates = result
           .Select(item => item.date)
           .ToArray();

            if (Directory.Exists(path))
            {
                
                string[] files = Directory.GetFiles(path);

                string[] dirs = Directory.GetDirectories(path);

                foreach (string file in files)
                {
                    var formattedName = Path.GetFileName(file).Split('_').First();



                    foreach (var item in result)
                    {
                        
                        listBox2.Items.Add($"{item.date.ToString()}");

                        //MessageBox.Show($"{item.date}".ToString());

                        //string insertData = ("INSERT INTO meteodata (Station, Dat, Temp, Hum, Press, windSpeed, windDirection, Rain, rainIntensity, notDraw, notDraw1, sunRad) " +
                        //    "values (" + item.id + ",'" + item.date + "'," + item.temperature + ",'" + item.hum + ",'" + item.presure + "," + item.windSpeed + "," + item.windDirect + "," + item.rain + "," + item.rainIntensity + "," + item.notDraw + "," +  item.notDraw1 + "," + item.sunRadiation + ");");


                        const string insertData = "INSERT OR IGNORE INTO meteodata (Station, Dat, Temp, Hum, Press, windSpeed, windDirection, Rain, rainIntensity, notDraw, notDraw1, sunRad) " +
                        "values (@Station, @Dat, @Temp, @Hum, @Press, @windSpeed, @windDirection, @Rain, @rainIntensity, @notDraw, @notDraw1, @sunRad)";

                        SQLiteCommand fillData = new SQLiteCommand(insertData, Connection);

                        fillData.Parameters.AddWithValue("@Station", formattedName);
                        fillData.Parameters.AddWithValue("@Dat", item.date);
                        //fillData.Parameters.AddWithValue("@Hours", item.hours);
                        fillData.Parameters.AddWithValue("@Temp", item.temperature);
                        fillData.Parameters.AddWithValue("@Hum", item.hum);
                        fillData.Parameters.AddWithValue("@Press", item.presure);
                        fillData.Parameters.AddWithValue("@windSpeed", item.windSpeed);
                        fillData.Parameters.AddWithValue("@windDirection", item.windDirect);
                        fillData.Parameters.AddWithValue("@Rain", item.rain);
                        fillData.Parameters.AddWithValue("@rainIntensity", item.rainIntensity);
                        fillData.Parameters.AddWithValue("@notDraw", item.notDraw);
                        fillData.Parameters.AddWithValue("@notDraw1", item.notDraw1);
                        fillData.Parameters.AddWithValue("@sunRad", item.sunRadiation);

                        //SQLiteCommand cmd = new SQLiteCommand(insertData);
                        //cmd.Connection = Connection;

                        //SQLiteTransaction trans;
                        Connection.Open();

                        //trans = Connection.BeginTransaction();

                        //SQLiteCommand sqlComm;
                        //sqlComm = new SQLiteCommand("begin", Connection);
                        //sqlComm = new SQLiteCommand(insertData, Connection);
                        var transaction = Connection.BeginTransaction();
                        fillData.ExecuteNonQuery();
                        transaction.Commit();
                        //sqlComm = new SQLiteCommand("end", Connection);
                        Connection.Close();


                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


            if (comboBox1.Text == temp)
            {
                try
                {
                    Connection.Open();
                    var transaction = Connection.BeginTransaction();

                    listBox3.Items.Clear();
                    var ca = chart1.ChartAreas["ChartArea1"];
                    ca.CursorX.IsUserEnabled = false;
                    ca.CursorX.IsUserSelectionEnabled = false;
                    //SQLiteTransaction trans;
                    //Connection.Open();
                    //trans = Connection.BeginTransaction();
                    chart1.Series["Temp"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
                    string selectTemp = "SELECT Dat,Temp FROM meteodata WHERE Station= " + comboBox2.SelectedItem.ToString() + " AND Dat BETWEEN '" + comboBox3.SelectedItem.ToString().Replace("_", "-") + "' AND '" + comboBox4.SelectedItem.ToString().Replace("_", "-") + "24:00" + "' ORDER by Dat";

                    SQLiteCommand cmd = new SQLiteCommand(selectTemp, Connection);
                    
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        transaction.Commit();
                        if (dr.HasRows)
                        {
                            listBox3.Show();
                            label6.Show();
                            chart1.Show();

                        

                            chart1.Series["WindSpeed"].Enabled = false;
                            chart1.Series["WindDirection"].Enabled = false;
                            chart1.Series["Pressure"].Enabled = false;
                            chart1.Series["Hum"].Enabled = false;
                            chart1.Series["Rain"].Enabled = false;
                            chart1.Series["RainIntensity"].Enabled = false;
                            chart1.Series["SunRadiation"].Enabled = false;
                            chart1.Series["Temp"].Enabled = true;
                            while (dr.Read())
                            {
                                //bool iii = dr.Read();
                                {
                                    chart1.Series["Temp"].Points.AddXY(dr["Dat"].ToString(), Convert.ToDouble(dr["Temp"].ToString()));

                                    listBox3.Items.Add(dr["Dat"].ToString() + " - " + dr["Temp"].ToString());
                                    //MessageBox.Show("Дата"+ dr["Dat"].ToString() + "Температура: " + dr["Temp"].ToString());

                                }
                            }
                            dr.Close();
                            Connection.Close();
                            
                        }
                        else
                        {

                            dr.Close();
                            Connection.Close();
                        }

                    }

                   // Connection.Close();
                }

                catch (SQLiteException err)
                {
                    MessageBox.Show("Caught exception: " + err.Message);
                    Connection.Close();

                }


            }



            if (comboBox1.Text == hum)
            {
                try
                {
                    Connection.Open();
                    var transaction = Connection.BeginTransaction();

                    listBox3.Items.Clear();

                    //SQLiteTransaction trans;
                    //Connection.Open();
                    //trans = Connection.BeginTransaction();
                    chart1.Series["Hum"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
                    string selectHum = "SELECT Dat,Hum FROM meteodata WHERE Station= " + comboBox2.SelectedItem.ToString() + " AND Dat BETWEEN '" + comboBox3.SelectedItem.ToString().Replace("_", "-") + "' AND '" + comboBox4.SelectedItem.ToString().Replace("_", "-") + "24:00" + "' ORDER by Dat";

                    SQLiteCommand cmd = new SQLiteCommand(selectHum, Connection);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        transaction.Commit();
                        if (dr.HasRows)
                        {
                            listBox3.Show();
                            label6.Show();
                            chart1.Show();
                            chart1.Series["WindSpeed"].Enabled = false;
                            chart1.Series["WindDirection"].Enabled = false;
                            chart1.Series["Pressure"].Enabled = false;
                            chart1.Series["Hum"].Enabled = true;
                            chart1.Series["Rain"].Enabled = false;
                            chart1.Series["RainIntensity"].Enabled = false;
                            chart1.Series["SunRadiation"].Enabled = false;
                            chart1.Series["Temp"].Enabled = false;
                            while (dr.Read())
                            {
                                //bool iii = dr.Read();
                                {
                                    chart1.Series["Hum"].Points.AddXY(dr["Dat"].ToString(), dr["Hum"].ToString());

                                    listBox3.Items.Add(dr["Dat"].ToString() + " - " + dr["Hum"].ToString());
                                    //MessageBox.Show("Дата"+ dr["Dat"].ToString() + "Температура: " + dr["Temp"].ToString());

                                }
                            }
                            dr.Close();
                            Connection.Close();

                        }
                        else
                        {

                            dr.Close();
                            Connection.Close();
                        }

                    }

                    // Connection.Close();
                }

                catch (SQLiteException err)
                {
                    MessageBox.Show("Caught exception: " + err.Message);
                    Connection.Close();

                }

            }

            if (comboBox1.Text == pressure)
            {
                try
                {
                    Connection.Open();
                    var transaction = Connection.BeginTransaction();

                    listBox3.Items.Clear();

                    //SQLiteTransaction trans;
                    //Connection.Open();
                    //trans = Connection.BeginTransaction();
                    chart1.Series["Pressure"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
                    string selectPress = "SELECT Dat,Press FROM meteodata WHERE Station= " + comboBox2.SelectedItem.ToString() + " AND Dat BETWEEN '" + comboBox3.SelectedItem.ToString().Replace("_", "-") + "' AND '" + comboBox4.SelectedItem.ToString().Replace("_", "-") + "24:00" + "' ORDER by Dat";

                    SQLiteCommand cmd = new SQLiteCommand(selectPress, Connection);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        transaction.Commit();
                        if (dr.HasRows)
                        {
                            listBox3.Show();
                            label6.Show();
                            chart1.Show();
                            chart1.Series["WindSpeed"].Enabled = false;
                            chart1.Series["WindDirection"].Enabled = false;
                            chart1.Series["Pressure"].Enabled = true;
                            chart1.Series["Hum"].Enabled = false;
                            chart1.Series["Rain"].Enabled = false;
                            chart1.Series["RainIntensity"].Enabled = false;
                            chart1.Series["SunRadiation"].Enabled = false;
                            chart1.Series["Temp"].Enabled = false;
                            while (dr.Read())
                            {
                                //bool iii = dr.Read();
                                {
                                    chart1.Series["Pressure"].Points.AddXY(dr["Dat"].ToString(), dr["Press"].ToString());

                                    listBox3.Items.Add(dr["Dat"].ToString() + " - " + dr["Press"].ToString());
                                    //MessageBox.Show("Дата"+ dr["Dat"].ToString() + "Температура: " + dr["Temp"].ToString());

                                }
                            }
                            dr.Close();
                            Connection.Close();

                        }
                        else
                        {

                            dr.Close();
                            Connection.Close();
                        }

                    }

                    // Connection.Close();
                }

                catch (SQLiteException err)
                {
                    MessageBox.Show("Caught exception: " + err.Message);
                    Connection.Close();

                }
            }
            

            if (comboBox1.Text == windSpeed)
            {
                try
                {

                    Connection.Open();
                    var transaction = Connection.BeginTransaction();
    
                    chart1.Series["WindSpeed"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
                    string selectWindSpeed = "SELECT Dat, windSpeed FROM meteodata WHERE Station= " + comboBox2.SelectedItem.ToString() + " AND Dat BETWEEN '" + comboBox3.SelectedItem.ToString().Replace("_", "-") + "' AND '" + comboBox4.SelectedItem.ToString().Replace("_", "-") + "24:00" + "' ORDER by Dat";
                    //SQLiteCommand sqlComm;
                    //sqlComm = new SQLiteCommand("begin", Connection);
                    SQLiteCommand cmd = new SQLiteCommand(selectWindSpeed, Connection);
                    
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                        transaction.Commit();
                        if (dr.HasRows)
                            {
                            listBox3.Show();
                            chart1.Show();
                            chart1.Series["WindSpeed"].Enabled = true;
                            chart1.Series["WindDirection"].Enabled = false;
                            chart1.Series["Pressure"].Enabled = false;
                            chart1.Series["Hum"].Enabled = false;
                            chart1.Series["Rain"].Enabled = false;
                            chart1.Series["RainIntensity"].Enabled = false;
                            chart1.Series["SunRadiation"].Enabled = false;
                            chart1.Series["Temp"].Enabled = false;
                            while (dr.Read())
                                {
                                    //bool iii = dr.Read();
                                    {
                                    
                                     chart1.Series["WindSpeed"].Points.AddXY(dr["Dat"].ToString(), dr["windSpeed"].ToString());
                                    //dr["Dat"].ToString() + " - " +
                                    listBox3.Items.Add(dr["Dat"].ToString() + " - " +  dr["windSpeed"].ToString());
                                    //MessageBox.Show(dr["windSpeed"].ToString());

                                    }
                                }
                                dr.Close();
                            Connection.Close();
                            }
                            else
                            {

                                dr.Close();
                            }
                        }
                    
                   
                }

                catch (SQLiteException err)
                {
                    MessageBox.Show("Caught exception: " + err.Message);

                }

                Connection.Close();
            }

            if (comboBox1.Text == windDirection)
            {
                try
                {
                    Connection.Open();
                    var transaction = Connection.BeginTransaction();

                    listBox3.Items.Clear();

                    //SQLiteTransaction trans;
                    //Connection.Open();
                    //trans = Connection.BeginTransaction();
                    chart1.Series["WindDirection"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
                    string selectWindDir = "SELECT Dat,windDirection FROM meteodata WHERE Station= " + comboBox2.SelectedItem.ToString() + " AND Dat BETWEEN '" + comboBox3.SelectedItem.ToString().Replace("_", "-") + "' AND '" + comboBox4.SelectedItem.ToString().Replace("_", "-") + "24:00" + "' ORDER by Dat";

                    SQLiteCommand cmd = new SQLiteCommand(selectWindDir, Connection);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        transaction.Commit();
                        if (dr.HasRows)
                        {
                            listBox3.Show();
                            label6.Show();
                            chart1.Show();
                            chart1.Series["WindSpeed"].Enabled = false;
                            chart1.Series["WindDirection"].Enabled = true;
                            chart1.Series["Pressure"].Enabled = false;
                            chart1.Series["Hum"].Enabled = false;
                            chart1.Series["Rain"].Enabled = false;
                            chart1.Series["RainIntensity"].Enabled = false;
                            chart1.Series["SunRadiation"].Enabled = false;
                            chart1.Series["Temp"].Enabled = false;
                            while (dr.Read())
                            {
                                //bool iii = dr.Read();
                                {
                                    chart1.Series["WindDirection"].Points.AddXY(dr["Dat"].ToString(), dr["windDirection"].ToString());

                                    listBox3.Items.Add(dr["Dat"].ToString() + " - " + dr["windDirection"].ToString());
                                    //MessageBox.Show("Дата"+ dr["Dat"].ToString() + "Температура: " + dr["Temp"].ToString());

                                }
                            }
                            dr.Close();
                            Connection.Close();

                        }
                        else
                        {

                            dr.Close();
                            Connection.Close();
                        }

                    }

                    // Connection.Close();
                }

                catch (SQLiteException err)
                {
                    MessageBox.Show("Caught exception: " + err.Message);
                    Connection.Close();

                }
            }

            if (comboBox1.Text == rain)
            {
                try
                {
                    Connection.Open();
                    var transaction = Connection.BeginTransaction();

                    listBox3.Items.Clear();

                    //SQLiteTransaction trans;
                    //Connection.Open();
                    //trans = Connection.BeginTransaction();
                    chart1.Series["Rain"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
                    string selectRain = "SELECT Dat,Rain FROM meteodata WHERE Station= " + comboBox2.SelectedItem.ToString() + " AND Dat BETWEEN '" + comboBox3.SelectedItem.ToString().Replace("_", "-") + "' AND '" + comboBox4.SelectedItem.ToString().Replace("_", "-") + "24:00" + "' ORDER by Dat";

                    SQLiteCommand cmd = new SQLiteCommand(selectRain, Connection);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        transaction.Commit();
                        if (dr.HasRows)
                        {
                            listBox3.Show();
                            label6.Show();
                            chart1.Show();
                            chart1.Series["WindSpeed"].Enabled = false;
                            chart1.Series["WindDirection"].Enabled = false;
                            chart1.Series["Pressure"].Enabled = false;
                            chart1.Series["Hum"].Enabled = false;
                            chart1.Series["Rain"].Enabled = true;
                            chart1.Series["RainIntensity"].Enabled = false;
                            chart1.Series["SunRadiation"].Enabled = false;
                            chart1.Series["Temp"].Enabled = false;

                            while (dr.Read())
                            {
                                //bool iii = dr.Read();
                                {
                                    chart1.Series["Rain"].Points.AddXY(dr["Dat"].ToString(), dr["Rain"].ToString());

                                    listBox3.Items.Add(dr["Dat"].ToString() + " - " + dr["Rain"].ToString());
                                    //MessageBox.Show("Дата"+ dr["Dat"].ToString() + "Температура: " + dr["Temp"].ToString());

                                }
                            }
                            dr.Close();
                            Connection.Close();

                        }
                        else
                        {

                            dr.Close();
                            Connection.Close();
                        }

                    }

                    // Connection.Close();
                }

                catch (SQLiteException err)
                {
                    MessageBox.Show("Caught exception: " + err.Message);
                    Connection.Close();

                }
            }

            if (comboBox1.Text == rainIntensity)
            {
                try
                {
                    Connection.Open();
                    var transaction = Connection.BeginTransaction();

                    listBox3.Items.Clear();

                    //SQLiteTransaction trans;
                    //Connection.Open();
                    //trans = Connection.BeginTransaction();
                    chart1.Series["RainIntensity"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
                    string selectRain = "SELECT Dat,rainIntensity FROM meteodata WHERE Station= " + comboBox2.SelectedItem.ToString() + " AND Dat BETWEEN '" + comboBox3.SelectedItem.ToString().Replace("_", "-") + "' AND '" + comboBox4.SelectedItem.ToString().Replace("_", "-") + "24:00" + "' ORDER by Dat";

                    SQLiteCommand cmd = new SQLiteCommand(selectRain, Connection);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        transaction.Commit();
                        if (dr.HasRows)
                        {
                            listBox3.Show();
                            label6.Show();
                            chart1.Show();
                            chart1.Series["WindSpeed"].Enabled = false;
                            chart1.Series["WindDirection"].Enabled = false;
                            chart1.Series["Pressure"].Enabled = false;
                            chart1.Series["Hum"].Enabled = false;
                            chart1.Series["Rain"].Enabled = false;
                            chart1.Series["RainIntensity"].Enabled = true;
                            chart1.Series["SunRadiation"].Enabled = false;
                            chart1.Series["Temp"].Enabled = false;

                            while (dr.Read())
                            {
                                //bool iii = dr.Read();
                                {
                                    chart1.Series["RainIntensity"].Points.AddXY(dr["Dat"].ToString(), dr["rainIntensity"].ToString());

                                    listBox3.Items.Add(dr["Dat"].ToString() + " - " + dr["rainIntensity"].ToString());
                                    //MessageBox.Show("Дата"+ dr["Dat"].ToString() + "Температура: " + dr["Temp"].ToString());

                                }
                            }
                            dr.Close();
                            Connection.Close();

                        }
                        else
                        {

                            dr.Close();
                            Connection.Close();
                        }

                    }

                    // Connection.Close();
                }

                catch (SQLiteException err)
                {
                    MessageBox.Show("Caught exception: " + err.Message);
                    Connection.Close();

                }
            }

            if (comboBox1.Text == radiation)
            {
                try
                {
                    Connection.Open();
                    var transaction = Connection.BeginTransaction();

                    listBox3.Items.Clear();

                    //SQLiteTransaction trans;
                    //Connection.Open();
                    //trans = Connection.BeginTransaction();
                    chart1.Series["SunRadiation"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
                    string selectSunRad = "SELECT Dat,sunRad FROM meteodata WHERE Station= " + comboBox2.SelectedItem.ToString() + " AND Dat BETWEEN '" + comboBox3.SelectedItem.ToString().Replace("_", "-") + "' AND '" + comboBox4.SelectedItem.ToString().Replace("_", "-") + "24:00" + "' ORDER by Dat";

                    SQLiteCommand cmd = new SQLiteCommand(selectSunRad, Connection);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        transaction.Commit();
                        if (dr.HasRows)
                        {
                            listBox3.Show();
                            label6.Show();
                            chart1.Show();
                            chart1.Series["WindSpeed"].Enabled = false;
                            chart1.Series["WindDirection"].Enabled = false;
                            chart1.Series["Pressure"].Enabled = false;
                            chart1.Series["Hum"].Enabled = false;
                            chart1.Series["Rain"].Enabled = false;
                            chart1.Series["RainIntensity"].Enabled = false;
                            chart1.Series["SunRadiation"].Enabled = true;
                            chart1.Series["Temp"].Enabled = false;

                            while (dr.Read())
                            {
                                //bool iii = dr.Read();
                                {
                                    chart1.Series["SunRadiation"].Points.AddXY(dr["Dat"].ToString(), dr["sunRad"].ToString());

                                    listBox3.Items.Add(dr["Dat"].ToString() + " - " + dr["sunRad"].ToString());
                                    //MessageBox.Show("Дата"+ dr["Dat"].ToString() + "Температура: " + dr["Temp"].ToString());

                                }
                            }
                            dr.Close();
                            Connection.Close();

                        }
                        else
                        {

                            dr.Close();
                            Connection.Close();
                        }

                    }

                    // Connection.Close();
                }

                catch (SQLiteException err)
                {
                    MessageBox.Show("Caught exception: " + err.Message);
                    Connection.Close();

                }

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime date0, date1;
            string dat0, dat1;
            dat0 = comboBox3.Text;
            dat1 = comboBox4.Text;
            var d0 = DateTime.TryParse(dat0.Replace("_","/"),out date0);//"07-06-2019"
            var d1 = DateTime.TryParse(dat1.Replace("_", "/"), out date1);//"06-06-2019"

            int value = DateTime.Compare(date0, date1);

            if (value > 0)
            {
                MessageBox.Show("Първата дата е по-малка от втората");
                //date0 > date1
            }
            else
            {
                if (value < 0)
                {
                   // MessageBox.Show("Грешка в датите");
                    //date0 < date1
                }
                else
                {
                    //date0 == date1
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.ZoomReset(1);
            chart1.ChartAreas["ChartArea1"].AxisY.ScaleView.ZoomReset(1);
        }
    }
}


