using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TextEditorFormApp.Util;

namespace TextEditorFormApp
{
    public partial class Editor : Form
    {
		/*
		###########################
		#      STATIC FIELDS      #
		###########################
		*/

		private static readonly string defaultName = "untitled";

		private static readonly string filter = "txt files (*.txt)|*.txt";

		private static readonly string iPath = @"C:\users\" + Environment.UserName + "\\Desktop\\test";

		/*
		###########################
		#          FIELDS         #
		###########################
		*/

		private string path;

		private string name;

		private string title;

		private string lastContent;

		private bool _cancel;

		private bool Cancel
		{
			get 
			{
				bool val = _cancel;
				_cancel = false;
				return val;
			}
			set 
			{
				_cancel = value;
			}
		}

		/*
		###########################
		#       CONSTRUCTORS      #
		###########################
		*/

		public Editor()
        {
            InitializeComponent();
			Init(
				defaultName, null, string.Empty, true
				);
		}
		
		/*
		###########################
		#          METHODS        #
		###########################
		*/

		/*
		 * init should only be set to true the first time this method is called.
		 */
		private void Init(string name, string path, string content, bool init = false)
		{
			this.name = name;
			this.path = path;
			title = name.ReplaceFirst('*');
			lastContent = content;
			SetText(content);
			if (!init)
			{
				SetTitle(null, null);
			}
		}

		private void SetText(string text)
		{
			string content = RichTextBox.Text;

			if (!content.Equals(text))
			{
				RichTextBox.Text = text;
			}
		}

		private void Form_Load(object sender, EventArgs e)
		{
			Activated += new System.EventHandler(SetTitle);
		}

		private void SetTitle(object sender, EventArgs e)
		{
			Text = title;
		}

		private DialogResult ShowShouldSaveChangesDialog()
		{
			string content = RichTextBox.Text;

			if (!content.Equals(lastContent))
			{
				var result = MessageBox.Show(
					string.Format(
							"Do you want to save changes to {0}", path ?? name
						), "Save Changes", MessageBoxButtons.YesNoCancel);
				return result;
			}
			else
			{
				return DialogResult.No;
			}
		}

		private void New_Click(object sender, EventArgs e)
		{
			switch (ShowShouldSaveChangesDialog())
			{
				case DialogResult.Yes:
					if (File.Exists(path))
					{
						SaveFile_Click(sender, e);
					}
					else
					{
						SaveFileAs_Click(sender, e);
						if (Cancel) 
						{
							return;
						}
					}
					break;
				case DialogResult.Cancel:
					return;
			}
			Init(
				defaultName, null, string.Empty
				);
		}

		private void OpenFile_Click(object sender, EventArgs e)
        {
			switch (ShowShouldSaveChangesDialog())
			{
				case DialogResult.Yes:
					if (File.Exists(path))
					{
						SaveFile_Click(sender, e);
					}
					else
					{
						SaveFileAs_Click(sender, e);
						if (Cancel)
						{
							return;
						}
					}
					break;
				case DialogResult.Cancel:
					return;
			}

			OpenFileDialog dialog = new();
			dialog.InitialDirectory = path?.GetDirectory() ?? iPath;
			dialog.Filter = filter;
			var result = dialog.ShowDialog();

			switch (result)
			{
				// OPEN FILE
				case DialogResult.OK:
					string path = dialog.FileName;
					string name = path.PathToFileName();
					string content = File.ReadAllText(path, Encoding.UTF8);
					Init(name, path, content);
					break;
			}
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
			if (path != null)
			{
				// SAVE FILE
				string content = RichTextBox.Text;
				File.WriteAllText(path, content);
				Init(name, path, content);
			}
			else
			{
				SaveFileAs_Click(sender, e);
			}
		}

        private void SaveFileAs_Click(object sender, EventArgs e)
        {
			SaveFileDialog dialog = new();
			dialog.InitialDirectory = path?.GetDirectory() ?? iPath;
			dialog.FileName = (path != null) ? name : "*.txt";
			dialog.Filter = filter;
			var result = dialog.ShowDialog();

			switch (result)
			{
				// SAVE FILE AS
				case DialogResult.OK:
					path = dialog.FileName;
					name = path.PathToFileName();
					SaveFile_Click(sender, e);
					Cancel = false;
					break;
				default:
					Cancel = true;
					break;
			}
		}

		private new void TextChanged(object sender, EventArgs e)
		{
			string content = RichTextBox.Text;

			if (content.Equals(lastContent) && title.StartsWith("*"))
			{
				title = title.ReplaceFirst('*');
				SetTitle(sender, e);
			}
			else if (!content.Equals(lastContent) && !title.StartsWith("*"))
			{
				title = "*" + title;
				SetTitle(sender, e);
			}

			WhitespaceLabel.Text = content.Occurences(' ').ToString();
			NonWhitespaceLabel.Text = content.Occurences((c) => !char.IsWhiteSpace(c)).ToString();
			WordLabel.Text = content.MySplit(' ', '\n').Length.ToString();
			LineSeparatorLabel.Text = content.Occurences('\n').ToString();
		}

		private void Form_Closing(object sender, FormClosingEventArgs e)
		{
			switch (ShowShouldSaveChangesDialog())
			{
				case DialogResult.Yes:
					if (File.Exists(path))
					{
						SaveFile_Click(sender, e);
					}
					else
					{
						SaveFileAs_Click(sender, e);
						if (Cancel)
						{
							e.Cancel = true;
							return;
						}
					}
					break;
				case DialogResult.Cancel:
					e.Cancel = true;
					return;
			}
		}

        private void Exit_Click(object sender, EventArgs e)
        {
			Close();
		}

        private void Cut_Click(object sender, EventArgs e)
        {
			RichTextBox.Cut();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
			RichTextBox.Copy();
		}

        private void Paste_Click(object sender, EventArgs e)
        {
			RichTextBox.Paste();
		}

        private void Undo_Click(object sender, EventArgs e)
        {
			if (RichTextBox.CanUndo)
			{
				RichTextBox.Undo();
			}
		}

		private void Redo_Click(object sender, EventArgs e)
		{
			if (RichTextBox.CanRedo)
			{
				RichTextBox.Redo();
			}
		}

		private void SelectAll_Click(object sender, EventArgs e)
		{
			if (RichTextBox.CanSelect)
			{
				RichTextBox.SelectAll();
			}
		}

        private void Delete_Click(object sender, EventArgs e)
        {
			string content = RichTextBox.Text;
			int start = RichTextBox.SelectionStart;
			int end = start + RichTextBox.SelectionLength;
			char[] array = content.ToCharArray().CutArray(start, end);
			string text = new(array);
			RichTextBox.Text = text;
			RichTextBox.SelectionLength = 0;
			RichTextBox.SelectionStart = start;
		}

		private void RichTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void RichTextBox_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files.Length == 0)
			{
				return;
			}

			// only the first file is processed
			string file = files[0];

			// CTRL + SHIFT
			if ((e.KeyState & (8 + 4)) == (8 + 4))
			{
				return;
			}
			// ALT 
			if ((e.KeyState & 32) == 32)
			{
				return;
			}
			// SHIFT
			/*
			 * Om "shift" hålls nedtryckt då filen dras in och släpps på textfönstret, 
			 * lägga till innehållet i den indragna filen vid markörens plats i aktuell text.
			 */
			if ((e.KeyState & 4) == 4)
			{
				int start = RichTextBox.SelectionStart;
				string content = File.ReadAllText(file, Encoding.UTF8);
				char[] array = RichTextBox.Text.ToCharArray()
					.InsertArray(start, content.ToCharArray());
				string text = new(array);
				SetText(text);
				RichTextBox.SelectionStart = start + content.Length;
			}
			// CTRL
			/*
			 * Om "ctrl" hålls nedtryckt då filen dras in och släpps på textfönstret, 
			 * lägga till innehållet i den indragna filen sista i aktuell text.
			 */
			else if ((e.KeyState & 8) == 8)
			{
				string content = File.ReadAllText(file, Encoding.UTF8);
				RichTextBox.AppendText(content);
			}
			/*
			 * Om ingen tangent hålls nedtryckt ska den indragna filen öppnas i programmet. 
			 * Programmet ska då fråga om filen ska sparas som när man trycker på krysset 
			 * eller avsluta-knappen.
			 */
			else
			{
				switch (ShowShouldSaveChangesDialog())
				{
					case DialogResult.Yes:
						if (File.Exists(this.path))
						{
							SaveFile_Click(sender, e);
						}
						else
						{
							SaveFileAs_Click(sender, e);
							if (Cancel)
							{
								return;
							}
						}
						break;
					case DialogResult.Cancel:
						return;
				}
				string path = file;
				string name = path.PathToFileName();
				string content = File.ReadAllText(path, Encoding.UTF8);
				Init(name, path, content);
			}
		}
	}
}
