namespace ClientNode
{
    partial class Logs
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
            this.logsListView = new System.Windows.Forms.ListView();
            this.logColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // logsListView
            // 
            this.logsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.logColumn});
            this.logsListView.Location = new System.Drawing.Point(3, 5);
            this.logsListView.Name = "logsListView";
            this.logsListView.Size = new System.Drawing.Size(640, 253);
            this.logsListView.TabIndex = 0;
            this.logsListView.UseCompatibleStateImageBehavior = false;
            this.logsListView.View = System.Windows.Forms.View.List;
            // 
            // logColumn
            // 
            this.logColumn.Width = 1000;
            // 
            // Logs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.logsListView);
            this.Name = "Logs";
            this.Text = "Logs";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView logsListView;
        private System.Windows.Forms.ColumnHeader logColumn;
    }
}