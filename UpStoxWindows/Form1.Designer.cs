namespace UpStoxWindows
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
            this.Button19 = new System.Windows.Forms.Button();
            this.Button18 = new System.Windows.Forms.Button();
            this.Button17 = new System.Windows.Forms.Button();
            this.Button16 = new System.Windows.Forms.Button();
            this.Button8 = new System.Windows.Forms.Button();
            this.Button4 = new System.Windows.Forms.Button();
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.button24 = new System.Windows.Forms.Button();
            this.dataGridViewStock = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStock)).BeginInit();
            this.SuspendLayout();
            // 
            // Button19
            // 
            this.Button19.Location = new System.Drawing.Point(335, 156);
            this.Button19.Name = "Button19";
            this.Button19.Size = new System.Drawing.Size(190, 23);
            this.Button19.TabIndex = 69;
            this.Button19.Text = "Download Symbols";
            this.Button19.UseVisualStyleBackColor = true;
            this.Button19.Click += new System.EventHandler(this.Button19_Click);
            // 
            // Button18
            // 
            this.Button18.Location = new System.Drawing.Point(335, 129);
            this.Button18.Name = "Button18";
            this.Button18.Size = new System.Drawing.Size(190, 23);
            this.Button18.TabIndex = 68;
            this.Button18.Text = "Access Token Status";
            this.Button18.UseVisualStyleBackColor = true;
            this.Button18.Click += new System.EventHandler(this.Button18_Click);
            // 
            // Button17
            // 
            this.Button17.Location = new System.Drawing.Point(335, 101);
            this.Button17.Name = "Button17";
            this.Button17.Size = new System.Drawing.Size(190, 23);
            this.Button17.TabIndex = 67;
            this.Button17.Text = "Get Access Token";
            this.Button17.UseVisualStyleBackColor = true;
            this.Button17.Click += new System.EventHandler(this.Button17_Click);
            // 
            // Button16
            // 
            this.Button16.Location = new System.Drawing.Point(433, 75);
            this.Button16.Name = "Button16";
            this.Button16.Size = new System.Drawing.Size(92, 23);
            this.Button16.TabIndex = 66;
            this.Button16.Text = "Logout Status";
            this.Button16.UseVisualStyleBackColor = true;
            // 
            // Button8
            // 
            this.Button8.Location = new System.Drawing.Point(335, 75);
            this.Button8.Name = "Button8";
            this.Button8.Size = new System.Drawing.Size(92, 23);
            this.Button8.TabIndex = 54;
            this.Button8.Text = "Login Status";
            this.Button8.UseVisualStyleBackColor = true;
            // 
            // Button4
            // 
            this.Button4.Location = new System.Drawing.Point(335, 185);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(190, 35);
            this.Button4.TabIndex = 52;
            this.Button4.Text = "Symbol Download Status";
            this.Button4.UseVisualStyleBackColor = true;
            this.Button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // RichTextBox1
            // 
            this.RichTextBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.RichTextBox1.Location = new System.Drawing.Point(0, 469);
            this.RichTextBox1.Name = "RichTextBox1";
            this.RichTextBox1.ReadOnly = true;
            this.RichTextBox1.Size = new System.Drawing.Size(917, 264);
            this.RichTextBox1.TabIndex = 48;
            this.RichTextBox1.Text = "";
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(433, 47);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(92, 23);
            this.Button2.TabIndex = 46;
            this.Button2.Text = "Logout";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click_1);
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(335, 46);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(92, 23);
            this.Button1.TabIndex = 45;
            this.Button1.Text = "Login";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click_1);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(127, 4);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(398, 13);
            this.Label1.TabIndex = 39;
            this.Label1.Text = "This is a Sample App based UpstoxNet Library to Demonstrate basic Functionalities" +
    "";
            // 
            // button24
            // 
            this.button24.Location = new System.Drawing.Point(335, 238);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(190, 35);
            this.button24.TabIndex = 76;
            this.button24.Text = "Get History";
            this.button24.UseVisualStyleBackColor = true;
            this.button24.Click += new System.EventHandler(this.button24_Click);
            // 
            // dataGridViewStock
            // 
            this.dataGridViewStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewStock.Location = new System.Drawing.Point(8, 357);
            this.dataGridViewStock.Name = "dataGridViewStock";
            this.dataGridViewStock.Size = new System.Drawing.Size(897, 364);
            this.dataGridViewStock.TabIndex = 77;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 733);
            this.Controls.Add(this.dataGridViewStock);
            this.Controls.Add(this.button24);
            this.Controls.Add(this.Button19);
            this.Controls.Add(this.Button18);
            this.Controls.Add(this.Button17);
            this.Controls.Add(this.Button16);
            this.Controls.Add(this.Button8);
            this.Controls.Add(this.Button4);
            this.Controls.Add(this.RichTextBox1);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.Label1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.Button Button19;
        internal System.Windows.Forms.Button Button18;
        internal System.Windows.Forms.Button Button17;
        internal System.Windows.Forms.Button Button16;
        internal System.Windows.Forms.Button Button8;
        internal System.Windows.Forms.Button Button4;
        internal System.Windows.Forms.RichTextBox RichTextBox1;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button button24;
        private System.Windows.Forms.DataGridView dataGridViewStock;
    }
}

