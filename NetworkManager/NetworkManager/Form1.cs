using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace NetworkManager
{
    public partial class Form1 : Form
    {

        private List<string> previousCommands;
        private int commandListPos;
        private Configuration config;
        private bool confLoaded = false;

        private Manager NetManager;
        private Logs logs;
        
        public Form1()
        {
            InitializeComponent();
            SendToAll.Enabled = false;
            Start.Enabled = false;
            LoadCommandScript.Enabled = false;

            previousCommands = new List<string>();
            commandListPos = 0;
            logs = new Logs(this.Logs);
            config = new Configuration(this.logs);
            Start.Enabled = true;

            if (confLoaded == false)
            {
                var path = @"Config/ManagerConfig.xml";
                config.loadConfiguration(path);
                afterConfiguration();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {


            if (NetManager.startManager(config.ManagerPort))
                afterStarted();
            
        }

        private void afterStarted()
        {

            Start.Enabled = false;
            SendToAll.Enabled = true;
            LoadConfigurationButton.Enabled = false;
            LoadCommandScript.Enabled = true;

        }
        private void afterConfiguration()
        {
            Start.Enabled = true;
            NetManager = new Manager(logs);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (NetManager.sendCommandToAll(textBox1.Text))
            {
                previousCommands.Add(textBox1.Text);
                commandListPos = previousCommands.Count;
                textBox1.Text = "";
            }

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            NetManager.stopManager();
            base.OnFormClosing(e);
            
                
        }

        private void Logs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LoadConfigurationButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }


        private void openFileDialog_FileOk_1(object sender, CancelEventArgs e)
        {
            config.loadConfiguration(openFileDialog.FileName);
            afterConfiguration();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (NetManager != null)
            {
                try
                {
                    NetManager.stopManager();
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.StackTrace);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            loadScript.ShowDialog();
        }

        private void loadScript_FileOk(object sender, EventArgs e)
        {
            string[] lines = System.IO.File.ReadAllLines(loadScript.FileName);
            Thread scriptThread = new Thread(new ThreadStart(delegate
            {
                foreach (string line in lines)
                {
                    if (NetManager.sendCommandToAll(line))
                    {
                        previousCommands.Add(line);
                        Thread.Sleep(200);
                    }
                }
            }));
            scriptThread.IsBackground = true;
            scriptThread.Start();
        }

    }
}
