namespace WallpaperChanger
{
	partial class AboutForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.labelAppName = new System.Windows.Forms.Label();
			this.labelAppVersion = new System.Windows.Forms.Label();
			this.labelCopyright = new System.Windows.Forms.Label();
			this.labelAppGHLink = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(207, 104);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(48, 48);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// labelAppName
			// 
			this.labelAppName.AutoSize = true;
			this.labelAppName.Location = new System.Drawing.Point(66, 12);
			this.labelAppName.Name = "labelAppName";
			this.labelAppName.Size = new System.Drawing.Size(94, 13);
			this.labelAppName.TabIndex = 2;
			this.labelAppName.Text = "{applicationName}";
			// 
			// labelAppVersion
			// 
			this.labelAppVersion.AutoSize = true;
			this.labelAppVersion.Location = new System.Drawing.Point(66, 32);
			this.labelAppVersion.Name = "labelAppVersion";
			this.labelAppVersion.Size = new System.Drawing.Size(101, 13);
			this.labelAppVersion.TabIndex = 3;
			this.labelAppVersion.Text = "{applicationVersion}";
			// 
			// labelCopyright
			// 
			this.labelCopyright.AutoSize = true;
			this.labelCopyright.Location = new System.Drawing.Point(12, 63);
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Size = new System.Drawing.Size(110, 13);
			this.labelCopyright.TabIndex = 4;
			this.labelCopyright.Text = "{applicationCopyright}";
			// 
			// labelAppGHLink
			// 
			this.labelAppGHLink.AutoSize = true;
			this.labelAppGHLink.Location = new System.Drawing.Point(9, 109);
			this.labelAppGHLink.Name = "labelAppGHLink";
			this.labelAppGHLink.Size = new System.Drawing.Size(102, 13);
			this.labelAppGHLink.TabIndex = 5;
			this.labelAppGHLink.TabStop = true;
			this.labelAppGHLink.Text = "{applicationGHLink}";
			// 
			// AboutForm
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(294, 134);
			this.Controls.Add(this.labelAppGHLink);
			this.Controls.Add(this.labelCopyright);
			this.Controls.Add(this.labelAppVersion);
			this.Controls.Add(this.labelAppName);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Wallpaper updater";
			this.Load += new System.EventHandler(this.AboutForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelAppName;
		private System.Windows.Forms.Label labelAppVersion;
		private System.Windows.Forms.Label labelCopyright;
		private System.Windows.Forms.LinkLabel labelAppGHLink;
	}
}
