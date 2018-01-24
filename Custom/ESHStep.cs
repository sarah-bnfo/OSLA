using System;
using System.Drawing;
using System.Windows.Forms;

namespace ESHStep
{
	public class TextProcessorHost : Scripting.Integration.EventfulScriptHost
	{
		public TextBox inputBox, outputBox;
		public object actionButton;

		internal static TextProcessorHost Create(MainForm form, string scriptFile)
		{
			try
			{
				TextProcessorHost host = (TextProcessorHost) ActivateScript(scriptFile, typeof(TextProcessorHost), null);

				host.inputBox = form.inputTextBox;
				host.outputBox = form.outputTextBox;
				host.actionButton = form.processButton;
				host.Load();

				return host;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ESHStep", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
		}

		protected override object CallMethod(string name, object[] arguments)
		{
			MessageBox.Show(name + " - " + arguments[0]);
			return "Done";
		}

		protected override object GetPropertyValue(string name)
		{
			return name;
		}
	}

	public class MainForm : Form
	{
		internal TextBox inputTextBox;		
		internal TextBox outputTextBox;
		internal Button processButton;
		internal TextProcessorHost host;

		public MainForm(string scriptFile)
		{
			this.Text = "ESHStep";
			this.Size = new Size(400, 230);
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.MaximizeBox = false;

			Label label1 = new Label();
			label1.Text = "Input Text";
			label1.Location = new Point(20, 10);
			label1.Size = new Size(360, 20);
			this.Controls.Add(label1);

			inputTextBox = new TextBox();
			inputTextBox.Multiline = true;
			inputTextBox.Location = new Point(20, 30);
			inputTextBox.Size = new Size(360, 40);
			this.Controls.Add(inputTextBox);


			Label label2 = new Label();
			label2.Text = "Output Text";
			label2.Location = new Point(20, 85);
			label2.Size = new Size(360, 20);
			this.Controls.Add(label2);

			outputTextBox = new TextBox();
			outputTextBox.Multiline = true;
			outputTextBox.Location = new Point(20, 105);
			outputTextBox.Size = new Size(360, 40);
			outputTextBox.ReadOnly = true;
			this.Controls.Add(outputTextBox);

			processButton = new Button();
			processButton.Text = "Process";
			processButton.Location = new Point(300, 160);
			processButton.Size = new Size(80, 25);
			this.Controls.Add(processButton);

			host = TextProcessorHost.Create(this, scriptFile);

		}


		[STAThread]
		public static void Main(string[] args)
		{
			MainForm form = new MainForm(args.Length > 0 ? args[0] : "stepesh.js");
			form.ShowDialog();
		}
	}
	


}

