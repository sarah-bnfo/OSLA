using System;
using System.Drawing;
using System.Windows.Forms;

namespace SSHStep
{

	public class TextProcessorHost : Scripting.Integration.SimpleScriptHost
	{
		private MainForm form;
		private dynamic script;

		public string ReadInput()
		{
			return form.inputTextBox.Text;
		}

		public void WriteOutput(string text)
		{
			form.outputTextBox.Text = text;
		}

		public void CanProcess(bool yes)
		{
			form.processButton.Enabled = yes;
		}

		internal void Execute(string task)
		{
			try
			{
				script.Start(task);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "SSHStep", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		internal static TextProcessorHost Create(MainForm mainForm, string scriptFile)
		{
			TextProcessorHost host = new TextProcessorHost();

			host.form = mainForm;
			host.script = host.ActivateScript(scriptFile, true);

			return host;
		}
	}

	public class MainForm : Form
	{
		internal TextBox inputTextBox;		
		internal TextBox outputTextBox;
		internal Button processButton;
	
		public MainForm(string scriptFile)
		{
			TextProcessorHost host = TextProcessorHost.Create(this, scriptFile);

			this.Text = "SSHStep";
			this.Size = new Size(400, 230);
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.MaximizeBox = false;
			this.Load += delegate(object sender, EventArgs e){
				host.Execute("Initialize");
			};

			Label label1 = new Label();
			label1.Text = "Input Text";
			label1.Location = new Point(20, 10);
			label1.Size = new Size(360, 20);
			this.Controls.Add(label1);

			inputTextBox = new TextBox();
			inputTextBox.Multiline = true;
			inputTextBox.Location = new Point(20, 30);
			inputTextBox.Size = new Size(360, 40);
			inputTextBox.TextChanged += delegate(object sender, EventArgs e){
				host.Execute("Update");
			};
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
			processButton.Click += delegate(object sender, EventArgs e){
				host.Execute("Action");
			};
			this.Controls.Add(processButton);
		}
	
		[STAThread]
		public static void Main(string[] args)
		{
			MainForm form = new MainForm(args.Length > 0 ? args[0] : "stepssh.js");
			form.ShowDialog();
		}
	}
	
}
