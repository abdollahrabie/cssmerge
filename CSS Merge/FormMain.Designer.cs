namespace CSS_Merge
{
    partial class FormMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.cboSorting = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkOptimize = new System.Windows.Forms.CheckBox();
			this.lblDragDrop = new System.Windows.Forms.Label();
			this.listViewFiles = new System.Windows.Forms.ListView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.btnMerge = new System.Windows.Forms.Button();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnAdd);
			this.groupBox1.Controls.Add(this.btnRemove);
			this.groupBox1.Controls.Add(this.cboSorting);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.chkOptimize);
			this.groupBox1.Controls.Add(this.lblDragDrop);
			this.groupBox1.Controls.Add(this.listViewFiles);
			this.groupBox1.Location = new System.Drawing.Point(4, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(339, 194);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.Location = new System.Drawing.Point(16, 108);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(75, 23);
			this.btnAdd.TabIndex = 6;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRemove.Location = new System.Drawing.Point(96, 108);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(75, 23);
			this.btnRemove.TabIndex = 5;
			this.btnRemove.Text = "Remove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// cboSorting
			// 
			this.cboSorting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboSorting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSorting.FormattingEnabled = true;
			this.cboSorting.Items.AddRange(new object[] {
            "By type then name",
            "By name"});
			this.cboSorting.Location = new System.Drawing.Point(62, 160);
			this.cboSorting.Name = "cboSorting";
			this.cboSorting.Size = new System.Drawing.Size(121, 21);
			this.cboSorting.TabIndex = 4;
			this.cboSorting.SelectedIndexChanged += new System.EventHandler(this.cboSorting_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 164);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Sorting";
			// 
			// chkOptimize
			// 
			this.chkOptimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.chkOptimize.AutoSize = true;
			this.chkOptimize.Location = new System.Drawing.Point(241, 164);
			this.chkOptimize.Name = "chkOptimize";
			this.chkOptimize.Size = new System.Drawing.Size(87, 17);
			this.chkOptimize.TabIndex = 2;
			this.chkOptimize.Text = "Optimize size";
			this.chkOptimize.UseVisualStyleBackColor = true;
			this.chkOptimize.CheckedChanged += new System.EventHandler(this.chkOptimize_CheckedChanged);
			// 
			// lblDragDrop
			// 
			this.lblDragDrop.AllowDrop = true;
			this.lblDragDrop.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblDragDrop.AutoSize = true;
			this.lblDragDrop.BackColor = System.Drawing.SystemColors.Window;
			this.lblDragDrop.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.lblDragDrop.Location = new System.Drawing.Point(98, 56);
			this.lblDragDrop.Name = "lblDragDrop";
			this.lblDragDrop.Size = new System.Drawing.Size(144, 13);
			this.lblDragDrop.TabIndex = 1;
			this.lblDragDrop.Text = "Drag and drop CSS files here";
			this.lblDragDrop.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragDrop);
			this.lblDragDrop.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragEnter);
			// 
			// listViewFiles
			// 
			this.listViewFiles.AllowDrop = true;
			this.listViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewFiles.LargeImageList = this.imageList;
			this.listViewFiles.Location = new System.Drawing.Point(16, 24);
			this.listViewFiles.Name = "listViewFiles";
			this.listViewFiles.Size = new System.Drawing.Size(311, 77);
			this.listViewFiles.TabIndex = 0;
			this.listViewFiles.UseCompatibleStateImageBehavior = false;
			this.listViewFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragDrop);
			this.listViewFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewFiles_DragEnter);
			this.listViewFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewFiles_KeyDown);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "file.gif");
			// 
			// btnMerge
			// 
			this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMerge.Location = new System.Drawing.Point(267, 202);
			this.btnMerge.Name = "btnMerge";
			this.btnMerge.Size = new System.Drawing.Size(75, 23);
			this.btnMerge.TabIndex = 1;
			this.btnMerge.Text = "Merge";
			this.btnMerge.UseVisualStyleBackColor = true;
			this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "css";
			this.saveFileDialog.Filter = "CSS files (*.css)|*.css|All files (*.*)|*.*";
			this.saveFileDialog.Title = "Select destination...";
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSS files (*.css)|*.css|All files (*.*)|*.*";
			this.openFileDialog.Multiselect = true;
			this.openFileDialog.Title = "Select CSS files...";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(349, 232);
			this.Controls.Add(this.btnMerge);
			this.Controls.Add(this.groupBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 268);
			this.Name = "FormMain";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "CSS Merge";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblDragDrop;
		private System.Windows.Forms.ListView listViewFiles;
		private System.Windows.Forms.CheckBox chkOptimize;
		private System.Windows.Forms.Button btnMerge;
		private System.Windows.Forms.ComboBox cboSorting;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}