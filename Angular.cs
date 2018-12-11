using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace Angulardb
{
    class Angular
    {
        string[] Database_name = new string[8];
        SqlConnection[] Angular_connection_group = new SqlConnection[8];
        SqlCommand[] Angular_cmd = new SqlCommand[8];
        SqlDataAdapter[] angular_adapter = new SqlDataAdapter[8];
        DataTable[] angular_datatable = new DataTable[8];
        private int Count_connection = 0;
        SqlConnection Angular_connection = new SqlConnection();
        SqlCommand angular_CMD = new SqlCommand();
        SqlDataAdapter angulr_ADAPTER = new SqlDataAdapter();
        DataTable angular_DATATABLE = new DataTable();

        private string servername = "";
        private bool Multithreading = false;
        public bool Error = false;
        public string msg_Error = "";


        private bool is_group(string x)
        {
            if ((x[0] == '{' && x[x.Length - 1] == '}'))
            {
                return true;
            }
            else { return false; }
        }

        private bool tryConnection(SqlConnection x)
        {
            try
            {
                x.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void bind_command()
        {
            for (int i = 0; i < Count_connection; i++)
            {
                Angular_cmd[i] = new SqlCommand();
                Angular_cmd[i].Connection = Angular_connection_group[i];
                // MessageBox.Show();
                angular_adapter[i] = new SqlDataAdapter();
                angular_datatable[i] = new DataTable();

            }
        }

        private void Parsing(string x)
        {
            string tmp5 = "";
            for (int i = 1; i <= x.Length - 1; i++)
            {
                if (x[i] == ',' || x[i] == '}')
                {
                    Angular_connection_group[Count_connection] = new SqlConnection("Server=" + servername + ";Database=" + tmp5 + ";Trusted_Connection=true;");
                    //MessageBox.Show(tmp5);
                    tmp5 = "";
                    //Database_name[Count_connection] = "Server=" +servername + ";Database=" + tmp5 + ";Trusted_Connection=true;";
                    Count_connection++;


                }
                else
                {
                    tmp5 += x[i];
                }
            }
        }
        public Angular(string SERVERNAME, string DATABASE)
        {
            if (is_group(DATABASE))
            {
                // MessageBox.Show("");
                servername = SERVERNAME;
                Parsing(DATABASE);
                for (int i = 0; i < Count_connection; i++)
                {
                    if (!tryConnection(Angular_connection_group[i]))
                    {
                        MessageBox.Show("Can not connect to SQL SERVER !!!");
                        Application.Exit();
                    }
                }

                bind_command();


            }
            else
            {
                Angular_connection.ConnectionString = "Server=" + SERVERNAME + ";Database=" + DATABASE + ";Trusted_Connection=true;";
                if (!tryConnection(Angular_connection))
                {
                    Error = true;
                    msg_Error = "Can Not Connect to SQL SERVER !!!";
                }
                else { angular_CMD.Connection = Angular_connection; }
            }
        }

        public void Query(String x, int y = 0)
        {
            if (y == 0)
            {
                try
                {
                    angular_CMD.CommandText = x;
                    angulr_ADAPTER.SelectCommand = angular_CMD;
                    angulr_ADAPTER.Fill(angular_DATATABLE);
                }
                catch
                {
                    MessageBox.Show("Query Error!!!");
                }
            }
            else if ((y - 1) <= Count_connection)
            {

                Angular_cmd[y - 1].CommandText = x;
                angular_adapter[y - 1].SelectCommand = Angular_cmd[y - 1];
                angular_adapter[y - 1].Fill(angular_datatable[y - 1]);


            }
            else
            {
                MessageBox.Show("Error!!!");
            }

        }

        public DataTable Result(int y = 0)
        {
            DataTable tmp = new DataTable();
            if (y == 0)
            {
                tmp = angular_DATATABLE;
            }
            else if ((y - 1) <= Count_connection)
            {
                MessageBox.Show("okkkk");
                tmp = angular_datatable[y - 1];
            }
            return tmp;
        }

        public void GridBind(DataGridView Grid, string x, int y = 0)
        {
            if (y == 0)
            {
                angular_DATATABLE.Rows.Clear();
                Query(x);
                Grid.DataSource = angular_DATATABLE;
                Grid.Refresh();
            }
            else if ((y - 1) <= Count_connection)
            {
                angular_datatable[y - 1].Rows.Clear();
                Query(x, y);
                Grid.DataSource = angular_datatable[y - 1];
                Grid.Refresh();
            }

        }

        public void Status(string msg, DataGridView grid = null, string name_table = "", int y = 0)
        {
            try
            {
                if (y == 0)
                {
                    if (grid == null && name_table == "")
                    {
                        //MessageBox.Show(msg)
                        if (angular_DATATABLE.Rows.Count >= 1)
                        {
                            MessageBox.Show(msg);
                        }

                    }
                    else
                    {
                        if (name_table != "" && grid != null)
                        {
                            GridBind(grid, "select * from " + name_table);
                            if (angular_DATATABLE.Rows.Count >= 1) { MessageBox.Show(msg); }
                        }
                    }
                }

                else if ((y - 1) <= Count_connection)
                {
                    if (grid == null && name_table == "")
                    {
                        //MessageBox.Show(msg)
                        if (angular_datatable[y - 1].Rows.Count >= 1)
                        {
                            MessageBox.Show(msg);
                        }

                    }
                    else
                    {
                        if (name_table != "" && grid != null)
                        {
                            GridBind(grid, "select * from " + name_table, y);
                            if (angular_datatable[y - 1].Rows.Count >= 1) { MessageBox.Show(msg); }
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
