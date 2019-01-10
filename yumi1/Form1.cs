using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

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
                    statusLabel.Text = "Connected";
                    BindingSource source = new BindingSource(robot, "CodeListL");
                    dataGridView1.DataSource = source;
                    BindingSource source2 = new BindingSource(robot, "CodeListR");
                    dataGridView2.DataSource = source2;
                }
            }
            else
            {
                MessageBox.Show("The connection is already established.");
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