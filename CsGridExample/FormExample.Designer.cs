namespace CsGridExample
{
	partial class FormExample
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
			this.gridPanel1 = new CsGrid.GridPanel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel5 = new System.Windows.Forms.Panel();
			this.gridPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridPanel1
			// 
			this.gridPanel1.Controls.Add(this.panel5);
			this.gridPanel1.Controls.Add(this.panel3);
			this.gridPanel1.Controls.Add(this.panel4);
			this.gridPanel1.Controls.Add(this.panel2);
			this.gridPanel1.Controls.Add(this.panel1);
			this.gridPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridPanel1.Location = new System.Drawing.Point(0, 0);
			this.gridPanel1.Name = "gridPanel1";
			this.gridPanel1.Size = new System.Drawing.Size(787, 539);
			this.gridPanel1.TabIndex = 0;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.Lime;
			this.panel3.Location = new System.Drawing.Point(476, 159);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(200, 100);
			this.panel3.TabIndex = 1;
			this.panel3.Tag = "body";
			// 
			// panel4
			// 
			this.panel4.BackColor = System.Drawing.Color.Aqua;
			this.panel4.Location = new System.Drawing.Point(214, 369);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(200, 100);
			this.panel4.TabIndex = 1;
			this.panel4.Tag = "right2";
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.panel2.Location = new System.Drawing.Point(73, 175);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(200, 100);
			this.panel2.TabIndex = 1;
			this.panel2.Tag = "right1";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Red;
			this.panel1.Location = new System.Drawing.Point(128, 53);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(200, 100);
			this.panel1.TabIndex = 0;
			this.panel1.Tag = "header";
			// 
			// panel5
			// 
			this.panel5.BackColor = System.Drawing.Color.Blue;
			this.panel5.Location = new System.Drawing.Point(454, 53);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(200, 100);
			this.panel5.TabIndex = 1;
			this.panel5.Tag = "menubar";
			// 
			// FormExample
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(787, 539);
			this.Controls.Add(this.gridPanel1);
			this.Name = "FormExample";
			this.Text = "Form1";
			this.gridPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private CsGrid.GridPanel gridPanel1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel5;
	}
}