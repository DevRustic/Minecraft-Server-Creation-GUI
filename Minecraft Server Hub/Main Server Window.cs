using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minecraft_Server_Hub
{
    public partial class Form1 : Form
    {
        public int version = 1;
        public bool formcreation = true;
        private Point lastPoint;
        public int serversrunning = 0;

        public void SetNames()
        {
            string servername1 = File.ReadLines("servernames.txt").Skip(0).Take(1).First(); string servername2 = File.ReadLines("servernames.txt").Skip(1).Take(1).First(); string servername3 = File.ReadLines("servernames.txt").Skip(2).Take(1).First(); string servername4 = File.ReadLines("servernames.txt").Skip(3).Take(1).First(); string servername5 = File.ReadLines("servernames.txt").Skip(4).Take(1).First(); string servername6 = File.ReadLines("servernames.txt").Skip(5).Take(1).First();
            server1.Text = servername1; server2.Text = servername2; server3.Text = servername3; server4.Text = servername4; server5.Text = servername5; server6.Text = servername6;  
        }



        public Form1()
        {
            InitializeComponent();

            if (version <= 1 || formcreation == true)
            {
                server2.Enabled = false; server3.Enabled = false; server4.Enabled = false; server5.Enabled = false; server6.Enabled = false;
            }

            if (!Directory.Exists("server1")) { Directory.CreateDirectory("server1"); }
            if (!Directory.Exists("server2")) { Directory.CreateDirectory("server2"); }
            if (!Directory.Exists("server3")) { Directory.CreateDirectory("server3"); }
            if (!Directory.Exists("server4")) { Directory.CreateDirectory("server4"); }
            if (!Directory.Exists("server5")) { Directory.CreateDirectory("server5"); }
            if (!Directory.Exists("server6")) { Directory.CreateDirectory("server6"); }

            if (!File.Exists("servernames.txt"))
            {
                File.WriteAllText("servernames.txt", "Server \n UNAVAILABLE \n UNAVAILABLE \n UNAVAILABLE \n UNAVAILABLE \n UNAVAILABLE");
                SetNames();
            } else
            {
                SetNames();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Left = Left + (e.X - lastPoint.X);
                Top = Top + (e.Y - lastPoint.Y);
            }
        }

        public void loadform(object form)
        {
            if (this.mainpanel.Controls.Count > 0)
                this.mainpanel.Controls.RemoveAt(0);
            Form f = form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.mainpanel.Controls.Add(f);
            this.mainpanel.Tag = f;
            f.Show();
        }
        public static bool ConfirmExit()
        {
            const string message = "Are you sure you would like to exit? \nWARNING: THIS WILL TERMINATE ALL SERVER PROCESSES AND DELETE ALL UNSAVED PROGRESS.";
            const string caption = "Close Hub";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
                return true;
            } else
            {
                return false;
            }
        }
        private void close_Click(object sender, EventArgs e)
        {
                ConfirmExit();
        }

        private void minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void server1_Click(object sender, EventArgs e)
        {
            loadform(new serverform1());
        }

        private void minecraftserverhub_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void minecraftserverhub_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left = Left + (e.X - lastPoint.X);
                Top = Top + (e.Y - lastPoint.Y);
            }
        }

        private void reloadnames_Click(object sender, EventArgs e)
        {
            SetNames();
        }
    }
}
