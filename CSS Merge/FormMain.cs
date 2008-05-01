using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CSS_Merge
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

			// Get from user-level settings
			cboSorting.SelectedIndex = (int)Properties.Settings.Default["sort"];
			chkOptimize.Checked = (bool)Properties.Settings.Default["optimize"];

			// Add any files supplied from command-line arguments
			foreach (string argument in Environment.GetCommandLineArgs())
			{
				FileInfo fi = new FileInfo(argument);
				if (fi.Exists && fi.Extension.ToLower() == ".css")
					AddFile(argument);
			}
        }

		private void listViewFiles_DragEnter(object sender, DragEventArgs e)
		{
			if (GetDropFilenames(e) != null)
				e.Effect = DragDropEffects.Move;
			else
				e.Effect = DragDropEffects.None;
		}
		private void listViewFiles_DragDrop(object sender, DragEventArgs e)
		{
			string[] filenames = GetDropFilenames(e);

			if (filenames == null)
				return;

			foreach (string filename in filenames)
				AddFile(filename);
		}
		private string[] GetDropFilenames(DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return null;

			string[] filenames = (string[])e.Data.GetData("FileDrop");

			foreach (string filename in filenames)
			{
				if (!filename.ToLower().EndsWith(".css"))
					return null;
			}

			return filenames;
		}

		private void AddFile(string filename)
		{
			// Add a file to the drag-drop list. This can be
			// called from drag-drop, Add file button or
			// command-line arguments

			bool alreadyAdded = false;
			foreach (ListViewItem lvi in listViewFiles.Items)
			{
				if ((string)lvi.Tag == filename)
					alreadyAdded = true;
			}

			if (alreadyAdded)
				return;

			FileInfo fi = new FileInfo(filename);
			ListViewItem lviNew = new ListViewItem(fi.Name, 0);
			lviNew.Tag = filename;
			listViewFiles.Items.Add(lviNew);

			lblDragDrop.Visible = (listViewFiles.Items.Count == 0);
		}

		private void cboSorting_SelectedIndexChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default["sort"] = cboSorting.SelectedIndex;
			Properties.Settings.Default.Save();
		}
		private void chkOptimize_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default["optimize"] = chkOptimize.Checked;
			Properties.Settings.Default.Save();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			DialogResult res = openFileDialog.ShowDialog();
			
			if (res == DialogResult.OK)
			{
				foreach (string filename in openFileDialog.FileNames)
					AddFile(filename);
			}
		}
		private void btnRemove_Click(object sender, EventArgs e)
		{
			if (listViewFiles.SelectedItems.Count == 0)
			{
				MessageBox.Show("To remove files from the list, select files and click Remove.",
					this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			ListViewItem[] selected = new ListViewItem[listViewFiles.SelectedItems.Count];
			listViewFiles.SelectedItems.CopyTo(selected, 0);

			foreach (ListViewItem lvi in selected)
				listViewFiles.Items.Remove(lvi);

			lblDragDrop.Visible = (listViewFiles.Items.Count == 0);
		}
		private void listViewFiles_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && listViewFiles.SelectedItems.Count > 0)
				btnRemove_Click(this, e);
		}

		private void btnMerge_Click(object sender, EventArgs e)
		{
			if (listViewFiles.Items.Count == 0)
			{
				MessageBox.Show("To merge CSS files, add one or more CSS files to the list and click Merge.",
					this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			List<CssFile> cssFiles = new List<CssFile>();

			foreach (ListViewItem lvi in listViewFiles.Items)
			{
				string filename = (string)lvi.Tag;

				try
				{
					cssFiles.Add(CssIO.ReadFile(filename));

				}
				catch (Exception ex)
				{
					MessageBox.Show("Aborting - an error occured while interpreting file '" + filename
						+ "':\r\n\r\n" + ex.Message,
						this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
			}

			CssConflict[] conflicts = CssConflict.GetConflictSet(cssFiles.ToArray());

			bool resolveConflicts = false;
			foreach (CssConflict conflict in conflicts)
			{
				// Only need to go to conflict resolution if
				// any one class appeared in more than one file

				if (conflict.CssClasses.Length > 1)
				{
					resolveConflicts = true;
					break;
				}
			}

			DialogResult res = DialogResult.OK;

			if (resolveConflicts)
			{
				FormConflictResolution frm = new FormConflictResolution();
				frm.Conflicts = conflicts;
				res = frm.ShowDialog(this);
			}

			if (res == DialogResult.OK)
			{
				CssClass[] mergedClasses = CssConflict.GetMergedClassSet(conflicts);

				res = saveFileDialog.ShowDialog(this);

				if (res == DialogResult.OK)
				{
					CssFile cssFile = new CssFile(saveFileDialog.FileName);
					cssFile.CssClasses.AddRange(mergedClasses);

					if (cboSorting.SelectedIndex == 0)
						cssFile.Sort(Sorting.ByTypeThenName);
					else
						cssFile.Sort(Sorting.ByName);
					
					CssIO.WriteFile(cssFile, (chkOptimize.Checked ? Formatting.Optimized : Formatting.Readable));

					MessageBox.Show("Merged CSS file saved to '" + saveFileDialog.FileName + "'.",
						this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}
    }
}
