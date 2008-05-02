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
                FileInfo fileInfo = new FileInfo(argument);
                if (fileInfo.Exists && fileInfo.Extension.ToLower() == ".css")
                    AddFile(argument);
            }
        }

        private void listViewFiles_DragEnter(object sender, DragEventArgs dragEventArgs)
        {
            if (GetDropFilenames(dragEventArgs) != null)
                dragEventArgs.Effect = DragDropEffects.Move;
            else
                dragEventArgs.Effect = DragDropEffects.None;
        }
        private void listViewFiles_DragDrop(object sender, DragEventArgs dragEventArgs)
        {
            string[] filenames = GetDropFilenames(dragEventArgs);

            if (filenames == null)
                return;

            foreach (string filename in filenames)
                AddFile(filename);
        }
        private string[] GetDropFilenames(DragEventArgs dragEventArgs)
        {
            if (!dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                return null;

            string[] filenames = (string[])dragEventArgs.Data.GetData("FileDrop");

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
            foreach (ListViewItem listViewItem in listViewFiles.Items)
            {
                if ((string)listViewItem.Tag == filename)
                    alreadyAdded = true;
            }

            if (alreadyAdded)
                return;

            FileInfo fileInfo = new FileInfo(filename);
            ListViewItem listViewItemToAdd = new ListViewItem(fileInfo.Name, 0);
            listViewItemToAdd.Tag = filename;
            listViewFiles.Items.Add(listViewItemToAdd);

            lblDragDrop.Visible = (listViewFiles.Items.Count == 0);
        }

        private void cboSorting_SelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            Properties.Settings.Default["sort"] = cboSorting.SelectedIndex;
            Properties.Settings.Default.Save();
        }
        private void chkOptimize_CheckedChanged(object sender, EventArgs eventArgs)
        {
            Properties.Settings.Default["optimize"] = chkOptimize.Checked;
            Properties.Settings.Default.Save();
        }

        private void btnAdd_Click(object sender, EventArgs eventArgs)
        {
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                    AddFile(filename);
            }
        }

        private void btnRemove_Click(object sender, EventArgs eventArgs)
        {
            if (listViewFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("To remove files from the list, select files and click Remove.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ListViewItem[] selectedListViewItem = new ListViewItem[listViewFiles.SelectedItems.Count];
            listViewFiles.SelectedItems.CopyTo(selectedListViewItem, 0);

            foreach (ListViewItem listViewItem in selectedListViewItem)
                listViewFiles.Items.Remove(listViewItem);

            lblDragDrop.Visible = (listViewFiles.Items.Count == 0);
        }

        private void listViewFiles_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Delete && listViewFiles.SelectedItems.Count > 0)
                btnRemove_Click(this, keyEventArgs);
        }

        private void btnMerge_Click(object sender, EventArgs eventArgs)
        {
            if (listViewFiles.Items.Count == 0)
            {
                MessageBox.Show("To merge CSS files, add one or more CSS files to the list and click Merge.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<CssFile> cssFiles = new List<CssFile>();

            foreach (ListViewItem listViewItem in listViewFiles.Items)
            {
                string filename = (string)listViewItem.Tag;

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

            bool conflictsToResolve = false;
            foreach (CssConflict conflict in conflicts)
            {
                // Only need to go to conflict resolution if
                // any one class appeared in more than one file

                if (conflict.CssClasses.Length > 1)
                {
                    conflictsToResolve = true;
                    break;
                }
            }

            DialogResult dialogResult = DialogResult.OK;

            if (conflictsToResolve)
            {
                FormConflictResolution conflictResolutionForm = new FormConflictResolution();
                conflictResolutionForm.Conflicts = conflicts;
                dialogResult = conflictResolutionForm.ShowDialog(this);
            }

            if (dialogResult == DialogResult.OK)
            {
                CssClass[] mergedClasses = CssConflict.GetMergedClassSet(conflicts);

                dialogResult = saveFileDialog.ShowDialog(this);

                if (dialogResult == DialogResult.OK)
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
