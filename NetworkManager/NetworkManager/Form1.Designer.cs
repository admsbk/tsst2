namespace NetworkManager
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Start = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SendToAll = new System.Windows.Forms.Button();
            this.Logs = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.LoadConfigurationButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.LoadCommandScript = new System.Windows.Forms.Button();
            this.loadScript = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(129, 268);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(88, 23);
            this.Start.TabIndex = 0;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 297);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(205, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // SendToAll
            // 
            this.SendToAll.Location = new System.Drawing.Point(223, 297);
            this.SendToAll.Name = "SendToAll";
            this.SendToAll.Size = new System.Drawing.Size(121, 20);
            this.SendToAll.TabIndex = 2;
            this.SendToAll.Text = "Send";
            this.SendToAll.UseVisualStyleBackColor = true;
            this.SendToAll.Click += new System.EventHandler(this.button2_Click);
            // 
            // Logs
            // 
            this.Logs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.Logs.FullRowSelect = true;
            this.Logs.Location = new System.Drawing.Point(12, 12);
            this.Logs.Name = "Logs";
            this.Logs.Size = new System.Drawing.Size(506, 250);
            this.Logs.TabIndex = 8;
            this.Logs.Tag = "Logs";
            this.Logs.UseCompatibleStateImageBehavior = false;
            this.Logs.View = System.Windows.Forms.View.Details;
            this.Logs.SelectedIndexChanged += new System.EventHandler(this.Logs_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Logs";
            this.columnHeader1.Width = 664;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // LoadConfigurationButton
            // 
            this.LoadConfigurationButton.Location = new System.Drawing.Point(12, 268);
            this.LoadConfigurationButton.Name = "LoadConfigurationButton";
            this.LoadConfigurationButton.Size = new System.Drawing.Size(111, 23);
            this.LoadConfigurationButton.TabIndex = 9;
            this.LoadConfigurationButton.Text = "Load Configuration";
            this.LoadConfigurationButton.UseVisualStyleBackColor = true;
            this.LoadConfigurationButton.Click += new System.EventHandler(this.LoadConfigurationButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk_1);
            // 
            // LoadCommandScript
            // 
            this.LoadCommandScript.Location = new System.Drawing.Point(223, 268);
            this.LoadCommandScript.Name = "LoadCommandScript";
            this.LoadCommandScript.Size = new System.Drawing.Size(121, 23);
            this.LoadCommandScript.TabIndex = 11;
            this.LoadCommandScript.Text = "Load command script";
            this.LoadCommandScript.UseVisualStyleBackColor = true;
            this.LoadCommandScript.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // loadScript
            // 
            this.loadScript.FileName = "commandscript.txt";
            this.loadScript.FileOk += new System.ComponentModel.CancelEventHandler(this.loadScript_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 323);
            this.Controls.Add(this.LoadCommandScript);
            this.Controls.Add(this.LoadConfigurationButton);
            this.Controls.Add(this.Logs);
            this.Controls.Add(this.SendToAll);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Start);
            this.Name = "Form1";
            this.Text = "NetworkManager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SendToAll;
        private System.Windows.Forms.ListView Logs;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button LoadConfigurationButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button LoadCommandScript;
        private System.Windows.Forms.OpenFileDialog loadScript;

    }
}

