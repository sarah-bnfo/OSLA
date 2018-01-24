using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;

namespace Scripting
{

	public class InputDialog
	{
		private Form box;
		private int count = 0;
		private string targetName;

		protected bool Prepared;

		protected internal InputDialog(string target)
		{
			targetName = target;
			box = new Form();	
		}

		protected int AddTextEntry(string prompt, string value, int state)
		{

			if(Prepared) return -1;

			Label label = new Label();
			label.Location = new Point(10, 25 * count + 10);
			label.Size = new Size(320, 20);
			label.Text = prompt;
			box.Controls.Add(label);

			TextBox textBox = new TextBox();
			textBox.Location = new Point(10, label.Bottom);
			textBox.Size = new Size(320, 20);
			textBox.Text = value;
			textBox.ReadOnly = state != 0;
			box.Controls.Add(textBox);

			count += 2;
		
			return box.Controls.IndexOf(textBox);
		}

		protected int AddChoiceEntry(string prompt, string choices, int state)
		{
			if(Prepared) return -1;

			if(choices.ToLower() == "false|true")
			{
				CheckBox checkBox = new CheckBox();
				checkBox.Location = new Point(10, 25 * count + 10);
				checkBox.Size = new Size(320, 20);
				checkBox.Text = prompt;
				checkBox.Checked = state != 0;
				box.Controls.Add(checkBox);
				
				count += 1;

				return box.Controls.IndexOf(checkBox);
								
			}
			else
			{
				Label label = new Label();
				label.Location = new Point(10, 25 * count + 10);
				label.Size = new Size(320, 20);
				label.Text = prompt;
				box.Controls.Add(label);

				ComboBox comboBox = new ComboBox();
				comboBox.Location = new Point(10, label.Bottom);
				comboBox.Size = new Size(320, 20);
				foreach(string item in choices.Split('|'))
					comboBox.Items.Add(item);
				comboBox.SelectedIndex = state;
				comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				box.Controls.Add(comboBox);

				count += 2;

				return box.Controls.IndexOf(comboBox);
			}
			
		}

		protected void Prepare()
		{
			Panel panel = new Panel();
			panel.BackColor = Color.DimGray;
			panel.Size = new Size(340, 40);
			panel.Location = new Point(0, 25 * count + 30);
			PictureBox iconPictureBox = new PictureBox();
			iconPictureBox.Size = new Size(32, 32);
			iconPictureBox.Location = new Point(8, 4);
			iconPictureBox.Image = SystemIcons.Question.ToBitmap();
			panel.Controls.Add(iconPictureBox);

			Button buttonOK = new Button();
            		buttonOK.DialogResult = DialogResult.OK;
            		buttonOK.Location = new Point(160, 8);
           		buttonOK.Size = new Size(80, 25);
			buttonOK.BackColor = SystemColors.ButtonFace;
            		buttonOK.Text = "OK";

			panel.Controls.Add(buttonOK);

			Button buttonCancel = new Button();
            		buttonCancel.DialogResult = DialogResult.Cancel;
            		buttonCancel.Location = new Point(250, 8);
            		buttonCancel.Size = new Size(80, 25);
            		buttonCancel.Text = "Cancel";
			buttonCancel.BackColor = SystemColors.ButtonFace;
			panel.Controls.Add(buttonCancel);

			box.Controls.Add(panel);
   			box.BackColor = Color.White;
			box.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
 			box.FormBorderStyle = FormBorderStyle.FixedSingle;
            		box.MaximizeBox = false;
            		box.MinimizeBox = false;
             		box.StartPosition = FormStartPosition.CenterScreen;
			box.ClientSize = new Size(340, 25 * count + 70);
            		box.AcceptButton = buttonOK;
            		box.CancelButton = buttonCancel;
	
			Prepared = true;		
		}

		public int AddInputEntry(string prompt, string value, int state=0)
		{

			if(value != null && value.IndexOf('|') >= 0)
				return AddChoiceEntry(prompt, value, state);

			return AddTextEntry(prompt, value, state);
		}


		public string GetInputValue(int entry)
		{
			if(entry < 0 || entry >= box.Controls.Count) return null;

			return box.Controls[entry].Text;
		}

		public int GetInputIndex(int entry)
		{
			if(entry < 0 || entry >= box.Controls.Count) return -1;

			ComboBox comboBox = box.Controls[entry] as ComboBox;
			if(comboBox != null) return comboBox.SelectedIndex;

			CheckBox checkBox = box.Controls[entry] as CheckBox;
			if(checkBox != null) return checkBox.Checked ? 1 : 0;

			return 0;
		}

		public void Destroy()
		{
			box.Dispose();
		}

		public bool Accept(string title)
		{
			if(!Prepared) Prepare();

			box.Text = title;
			
			return box.ShowDialog(null) == DialogResult.OK;
		}

	}

	public class AcceptManyResult
	{
		internal class Entry
		{
			internal string prompt;
			internal string value;
			internal int index;
		}

		internal Entry[] entries;

		public object ForPrompt(object entry, bool choice = false)
		{
			int id;
			if(entry is string)
			{
				for(id = 0; id < entries.Length; ++id)
					if(entries[id].prompt == entry.ToString().ToLower()) break;
			}
			else
				id = (int)entry;

			if(id < 0 || id >= entries.Length)
				return null;
			
			if(choice)
				return entries[id].index;
	
			decimal result;
			if(Decimal.TryParse(entries[id].value, out result))
				return result;
			
			return entries[id].value;		
		}

	}

	public static class HostExtension
	{
		public static object Invoke(string title, params object[] info)
		{
			InputDialog dialog = new InputDialog(title);
			int[] entries = new int[info.Length];	
			string[] prompts = new string[info.Length];
			for(int i = 0; i < entries.Length; ++i)
			{
				string value = null;
				int state = 0;
				try
				{
					var args = new Scripting.Integration.ScriptObjectAccessor(info[i]);
					prompts[i] = args[0].ToString();
					value = args[1].ToString(); 
					state = Convert.ToInt32(args[2].ToString());
				}
				catch {}
				entries[i] = dialog.AddInputEntry(prompts[i], value, state);
			}
			AcceptManyResult result = null;
			if(dialog.Accept(title))
			{
				result = new AcceptManyResult();
				result.entries = new AcceptManyResult.Entry[entries.Length];
				for(int i = 0; i < entries.Length; ++i)
				{
					result.entries[i] = new AcceptManyResult.Entry();
					result.entries[i].prompt = prompts[i].ToLower();
					result.entries[i].value = dialog.GetInputValue(entries[i]);
					result.entries[i].index = dialog.GetInputIndex(entries[i]);
				}
					
			}
			dialog.Destroy();

			return result;
		}
	}
}
