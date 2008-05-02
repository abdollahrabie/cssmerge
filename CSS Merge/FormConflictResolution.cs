using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CSS_Merge
{
	public partial class FormConflictResolution : Form
	{
		private CssConflict[] conflicts;

		public FormConflictResolution()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets a collection of conflicts
		/// to resolve. The form will only show conflicts
		/// that have two or more CSS file/class sets 
		/// in them but will retain and return them all
		/// </summary>
		public CssConflict[] Conflicts
		{
			get { return conflicts; }
			set 
			{ 
				conflicts = value;

				cboSelector.Items.Clear();
				foreach (CssConflict conflict in conflicts)
				{
					if (conflict.CssClasses.Length > 1)
						cboSelector.Items.Add(conflict);
				}

				cboSelector.SelectedIndex = 0;
			}
		}

		private void cboSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			CssConflict conflict = (CssConflict)cboSelector.SelectedItem;

			chklstFiles.Items.Clear();

			foreach (KeyValuePair<CssFile, CssClass> kvp in conflict.CssClasses)
				chklstFiles.Items.Add(kvp.Key);

			chklstFiles.SelectedIndex = conflict.SelectedIndex;
		}
		private void chklstFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			CssConflict conflict = (CssConflict)cboSelector.SelectedItem;

			conflict.SelectedIndex = chklstFiles.SelectedIndex;

			CssClass selectedClass = conflict.CssClasses[conflict.SelectedIndex].Value;
			txtContents.Text = selectedClass.Serialize(false);

			for (int i = 0; i < chklstFiles.Items.Count; i++)
				chklstFiles.SetItemChecked(i, i == chklstFiles.SelectedIndex);
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			if (cboSelector.SelectedIndex > 0)
				cboSelector.SelectedIndex--;
		}
		private void btnNext_Click(object sender, EventArgs e)
		{
			if (cboSelector.SelectedIndex < cboSelector.Items.Count - 1)
				cboSelector.SelectedIndex++;
		}

		private void btnChooseAll_Click(object sender, EventArgs e)
		{
			CssConflict thisConflict = (CssConflict)cboSelector.SelectedItem;
			CssFile thisFile = thisConflict.CssClasses[thisConflict.SelectedIndex].Key;

			foreach (CssConflict conflict in cboSelector.Items)
			{
				for (int i = 0; i < conflict.CssClasses.Length; i++)
				{
					if (conflict.CssClasses[i].Key == thisFile)
					{
						conflict.SelectedIndex = i;
						break;
					}
				}
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}
		private void btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}	
	}
}
