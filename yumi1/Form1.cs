using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yumi1
{
    public partial class Form1 : Form, INotifyPropertyChanged
    {

        YuMi robot;
        public event PropertyChangedEventHandler PropertyChanged;

        public Form1()
        {
            InitializeComponent();
            robot = new YuMi();
            label1.DataBindings.Add(new Binding("Text", robot, "Name"));
            linkLabel1.DataBindings.Add(new Binding("Text", robot, "CodeL"));
            linkLabel2.DataBindings.Add(new Binding("Text", robot, "CodeR"));
            
        }

        public void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != String.Empty && textBox2.Text != String.Empty)
            {
                try
                {
                    PfsHandler.ReadReplaceRequest("PfsVerifyUserInput", Properties.Resources.PfsVerifyUserInput, new string[] { textBox1.Text, textBox2.Text }, new string[] { "USER_ID", "PASSWORD" });
                    if (PfsHandler.ResponseOK())
                    {
                        textBox1.Enabled = false;
                        textBox2.Enabled = false;
                        buttonLogin.Enabled = false;
                        MessageBox.Show("Logged in.");
                    }
                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                
            }
            else
            {
                MessageBox.Show("Make sure both fields are not empty.");
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
        // to do : add clear list button and implement
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (robot.status != Status.Connected)
            {
                try
                {
                    statusLabel.Text = "Connecting...";
                    robot.ConnectAndDisplayData();
                }
                catch (Exception s)
                {
                    Console.WriteLine(s.StackTrace);
                    statusLabel.Text = "Not Available";
                }
                finally
                {
                    switch (robot.status)
                    {
                        case Status.Connected:
                            statusLabel.Text = "Connected";
                            // bind the tables to the codes.
                            BindingSource source = new BindingSource(robot, "CodeListL");
                            dataGridView1.DataSource = source;
                            BindingSource source2 = new BindingSource(robot, "CodeListR");
                            dataGridView2.DataSource = source2;
                            dataGridView1.CellMouseDoubleClick += DataGridView_CellMouseDoubleClick;
                            dataGridView2.CellMouseDoubleClick += DataGridView_CellMouseDoubleClick;
                            break;
                        case Status.NotFound:
                            statusLabel.Text = "No controllers found.";
                            Program.NoController();
                            break;
                    }       
                }
            }
            else
            {
                MessageBox.Show("The connection is already established.");
            }
        }

        private void DataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (e.ColumnIndex == 0 && e.RowIndex >=0)
            {
                string statusLabelTemp = statusLabel.Text;
                var item = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                
                try
                {
                    
                    statusLabel.Text = "Processing request...";
                    if (!String.IsNullOrWhiteSpace(item))
                    {
                        statusLabel.Text = String.Format("{0} : {1}", "Saving request using code", item);
                        PfsHandler.ReadReplaceRequest("PfsSendSignOff",Properties.Resources.PfsSendSignOff, item, Requests.SerialNumber);
                        if (PfsHandler.ResponseOK())
                        {
                            bool ok = true;
                        }

                    }
                    else
                        MessageBox.Show("The request could not be empty.");
                }
                catch (Exception s)
                {
                    Console.WriteLine(s.StackTrace);
                    statusLabel.Text = "Error while saving.";
                }
                finally
                {
                    statusLabel.Text = "Done.";
                    MessageBox.Show("Transfer file saved.");
                }
                statusLabel.Text = statusLabelTemp;
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}