using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Scripting.Interaction
{

	public class DialogInteractivityProvider : InteractiveScriptHost
	{
		private static string title = null;

		public string TargetName {get; private set;}

		public string DisplayTargetName {get; private set;}

		protected DialogInteractivityProvider(string targetName)
		{
			this.TargetName = targetName;
			this.DisplayTargetName = targetName.EndsWith(".js") ? targetName.Replace(".js", "") : title;
		}

		protected override bool Output(string message, bool yesno)
		{
	    		Form box = new Form();
			box.BackColor = Color.White;
			box.Text = DisplayTargetName;

			Panel panel = new Panel();
			panel.BackColor = Color.DimGray;
			panel.Size = new Size(340, 40);
			panel.Location = new Point(0, 80);
            		Button buttonOK = new Button();

			Panel messagePanel = new Panel();
			messagePanel.AutoScroll = true;
			messagePanel.Location = new Point(5, 10);
			messagePanel.Size = new Size(330, 60);
 			Label messageLabel = new Label();
			messageLabel.BackColor = Color.White;
			messageLabel.Font = SystemFonts.MessageBoxFont;
			messageLabel.AutoSize = true;
			messageLabel.Location = new Point(0, 0);
			messageLabel.MaximumSize = new Size(320, 0);
			messageLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            		messageLabel.Text = message;
			messagePanel.Controls.Add(messageLabel);
			
            		buttonOK.DialogResult = DialogResult.OK;
            		buttonOK.Location = yesno ? new Point(160, 8) : new Point(250, 8);
           		buttonOK.Size = new Size(80, 25);
            		buttonOK.Text = yesno ? "Yes" : "OK";
			buttonOK.BackColor = SystemColors.ButtonFace;
            		box.AcceptButton = buttonOK;
			panel.Controls.Add(buttonOK);

			PictureBox iconPictureBox = new PictureBox();
			iconPictureBox.Size = new Size(32, 32);
			iconPictureBox.Location = new Point(8, 4);

			if(yesno)
			{
           			Button buttonCancel = new Button();
            			buttonCancel.DialogResult = DialogResult.Cancel;
            			buttonCancel.Location = new Point(250, 8);
            			buttonCancel.Size = new Size(80, 25);
            			buttonCancel.Text = "No";
				buttonCancel.BackColor = SystemColors.ButtonFace;
            			box.CancelButton = buttonCancel;
				panel.Controls.Add(buttonCancel);
				iconPictureBox.Image = SystemIcons.Question.ToBitmap();
			}
			else
			{
           			Button buttonCopy = new Button();
            			buttonCopy.Location = new Point(160, 8);
            			buttonCopy.Size = new Size(80, 25);
            			buttonCopy.Text = "Copy";
				buttonCopy.BackColor = SystemColors.ButtonFace;
				buttonCopy.Click += (s, e) => Clipboard.SetText(message);
				panel.Controls.Add(buttonCopy);
				iconPictureBox.Image = SystemIcons.Information.ToBitmap();
			}
			panel.Controls.Add(iconPictureBox);

            		box.ClientSize = new Size(340, 120);
			box.Controls.Add(panel);
            		box.Controls.Add(messagePanel);
            		box.FormBorderStyle = FormBorderStyle.FixedSingle;
			box.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            		box.MaximizeBox = false;
            		box.MinimizeBox = false;
            		box.StartPosition = FormStartPosition.CenterScreen;
            		
	    		DialogResult dr = box.ShowDialog(null);
	    		box.Dispose();	

	    		return dr == DialogResult.OK;
		}

		protected override string Input(string prompt, string[] values)
		{
	    		Form box = new Form();
			box.BackColor = Color.White;
			box.Text = DisplayTargetName;

			Control inputControl = null;
			Label label = new Label();
			label.Location = new Point(10, 10);
			label.Size = new Size(320, 20);
			label.Font = SystemFonts.MessageBoxFont;
			label.Text = prompt;
			box.Controls.Add(label);

			if(values.Length > 1)
			{
				ComboBox comboBox = new ComboBox();
				comboBox.Location = new Point(10, label.Bottom);
				comboBox.Size = new Size(320, 20);
				foreach(string value in values)
					comboBox.Items.Add(value);
				comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				comboBox.SelectedIndex = 0;
				box.Controls.Add(comboBox);
				inputControl = comboBox;
			}
			else
			{
				TextBox textBox = new TextBox();
				textBox.Location = new Point(10, label.Bottom);
				textBox.Size = new Size(320, 20);
				textBox.Text = values[0] != null ? values[0].ToString() : String.Empty;
				box.Controls.Add(textBox);
				inputControl = textBox;
			}

			Panel panel = new Panel();
			panel.BackColor = Color.DimGray;
			panel.Size = new Size(340, 40);
			panel.Location = new Point(0, 80);
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
			box.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
 			box.FormBorderStyle = FormBorderStyle.FixedSingle;
            		box.MaximizeBox = false;
            		box.MinimizeBox = false;
             		box.StartPosition = FormStartPosition.CenterScreen;
			box.ClientSize = new Size(340, 120);
            		box.AcceptButton = buttonOK;
            		box.CancelButton = buttonCancel;

			string input = null;
			if(box.ShowDialog(null) == DialogResult.OK)
				input = inputControl.Text;
			box.Dispose();	

			return input;		
		}

		private void Start(string scriptFile, string task)
		{
			object script = ActivateScript(scriptFile, true);
			try	
			{
				script.GetType().InvokeMember("Start", BindingFlags.InvokeMethod, null, this.Active, new object[]{task});
			}
			catch(TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private static void Run(string scriptFile, string task, Type aspectProviderType=null)
		{
			DialogInteractivityProvider host = new DialogInteractivityProvider(scriptFile);
			if(aspectProviderType != null) host.Aspect = (AspectProviderBase)Activator.CreateInstance(aspectProviderType);
			host.Start(scriptFile, task);
		}

		[STAThread]
		public static void Main(string[] args)
		{
			title = Assembly.GetExecutingAssembly().GetName().Name;
			if(args.Length < 1)
			{
				MessageBox.Show("Please specify the script-file", title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			string curDir = Directory.GetCurrentDirectory();
			try
			{
				Type apType = null;
				int si = 0;
				if(args.Length > 2 && args[0] == "/ap")
				{
					Assembly apAsm = Assembly.LoadFrom(args[1]);
					apType = apAsm.GetType("Scripting.Interaction.AspectProvider");
					si = 2; 
				}
				FileInfo scriptFile = new FileInfo(args[si]);
				Directory.SetCurrentDirectory(scriptFile.DirectoryName);
				DialogInteractivityProvider.Run(scriptFile.Name, args.Length > si + 1 ? args[si + 1] : null, apType);				
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Directory.SetCurrentDirectory(curDir);
		}
    	}

}
