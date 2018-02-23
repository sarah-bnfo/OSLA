Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms

Namespace Scripting.Interaction

    Public Class ConsoleInteractivityProvider
    Inherits Scripting.Interaction.InteractiveScriptHost

        Private Declare Sub AllocConsole Lib "Kernel32"()

	Private Function Assign(ByVal prompt As String, items As String()) As Object
            Console.WriteLine(prompt)
	    Dim pb As Integer = Console.CursorTop
	    For Each item In items
		Console.WriteLine("  {0}", item)
	    Next
	    Dim pe As Integer = Console.CursorTop - 1
	    Dim pc As Integer = pb
	    Console.SetCursorPosition(0, pc)
	    Console.Write("> ")
	    Dim key As ConsoleKey
	    Do
		key = Console.ReadKey(True).Key
		If key = ConsoleKey.UpArrow AndAlso pc > pb Then
		    Console.SetCursorPosition(0, pc)
		    Console.Write("  ")
		    pc = pc - 1
		    Console.SetCursorPosition(0, pc)
		    Console.Write("> ")
		Else If key = ConsoleKey.DownArrow AndAlso pc < pe Then
		    Console.SetCursorPosition(0, pc)
		    Console.Write("  ")
		    pc = pc + 1
		    Console.SetCursorPosition(0, pc)
		    Console.Write("> ")
		End If
	    Loop Until key = ConsoleKey.Enter OrElse key = ConsoleKey.Escape
	    Console.SetCursorPosition(0, pe + 1)
	    Console.WriteLine()
	    If key = ConsoleKey.Enter Then Return items(pc - pb)
	    Return Nothing	
	End Function

	Private Function Assure(ByVal text As String) As Boolean
	    Console.Write("{0} ", text)
	    Dim pt As Integer = Console.CursorTop
	    Dim pl As Integer = Console.CursorLeft
	    Dim state As Boolean = True 
	    Console.Write("[Yes] / No")
	    Dim key As ConsoleKey
	    Do
		key = Console.ReadKey(True).Key
		If key = ConsoleKey.LeftArrow Then
		    state = True
		    Console.SetCursorPosition(pl, pt)
		    Console.Write("[Yes] / No")
		Else If key = ConsoleKey.RightArrow Then
		    state = False
		    Console.SetCursorPosition(pl, pt)
		    Console.Write("Yes / [No]")
		End If
	    Loop Until key = ConsoleKey.Enter OrElse key = ConsoleKey.Escape
	    Console.SetCursorPosition(0, pt + 1)
	    Console.WriteLine()	
	    Return state
	End Function

	Protected Overrides Function Output(ByVal message As String, ByVal yesno As Boolean) As Boolean
		If yesno Then Return Assure(message)
		Console.WriteLine(message)
		Return False
	End Function

	Protected Overrides Function Input(ByVal prompt As String, ByVal values As String()) As String
		If values.Length > 1 Then Return Assign(prompt, values)
		If values(0) IsNot Nothing Then My.Computer.Keyboard.SendKeys(values(0), True)
            	Console.WriteLine(prompt)
            	Console.Write("> ")
            	Dim text As String = Console.ReadLine()
            	Console.WriteLine()
		Return text
	End Function

        Public Overrides Function Action(ByVal name As String, ParamArray arguments As Object()) As Object
            Select Case name.ToLower()
                Case "clearscreen"
                    Console.Clear()
                    Return True
                Case "endline"
                    Console.WriteLine()
                    Return True
            End Select
            Return MyBase.Action(name, arguments)
        End Function

        Private Sub Start(ByVal scriptFile As String, ByVal task As String)
            Dim script As Object = ActivateScript(scriptFile, True)
	    CallByName(script, "Start", CallType.Method, task)
        End Sub

        Protected Friend Shared Sub Run(ByVal scriptFile As String, Optional aspectProviderType As Type=Nothing)
            AllocConsole()
            Dim writer As TextWriter = New StreamWriter(Console.OpenStandardOutput()) With {.AutoFlush = True}
            Console.SetOut(writer)
            Dim reader As TextReader = New StreamReader(Console.OpenStandardInput())
            Console.SetIn(reader)
            Console.BackgroundColor = ConsoleColor.White
            Console.ForegroundColor = ConsoleColor.Black
            Console.Clear()
            Console.Title = "Executing script - Elsa"
            Dim host As New ConsoleInteractivityProvider
	    If aspectProviderType IsNot Nothing Then host.Aspect = Activator.CreateInstance(aspectProviderType)
            Try
                host.Start(scriptFile, Nothing)
            Catch ex As Exception
                Console.WriteLine("ERROR: {0}", ex.Message)
            End Try

            Console.Title = "Script exited - Elsa"
            Console.ReadKey()
        End Sub
    End Class

    Public Class MainForm
        Inherits Form

        Private Shared defaultScript As FileInfo = Nothing

        Private Shared scriptLauncher As FileInfo = Nothing

        Public Sub New()
            InitializeComponent()
            executeToolStripMenuItem.Enabled = True
            If defaultScript IsNot Nothing AndAlso File.Exists(defaultScript.FullName) Then scriptTextBox.Text = File.ReadAllText(defaultScript.FullName)
        End Sub

        Private Sub executeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles executeToolStripMenuItem.Click
            Dim scriptFile As String = If(defaultScript IsNot Nothing, defaultScript.FullName, Path.GetTempFileName())
            File.WriteAllText(scriptFile, scriptTextBox.Text)
            Dim title As String = Me.Text
            Me.Text += " - Running"
            Try
                Dim runner = System.Diagnostics.Process.Start(scriptLauncher.FullName, scriptFile)
                runner.WaitForExit()
            Catch ex As Exception
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.[Error])
            End Try

            Me.Text = title
            If defaultScript Is Nothing Then File.Delete(scriptFile)
        End Sub

        Private Sub exitToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles exitToolStripMenuItem.Click
            Me.Close()
        End Sub

        Private components As System.ComponentModel.IContainer = Nothing

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.scriptTextBox = New System.Windows.Forms.TextBox()
            Me.menuMain = New System.Windows.Forms.MenuStrip()
            Me.sourceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.executeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.toolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
            Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.menuMain.SuspendLayout()
            Me.SuspendLayout()
            Me.scriptTextBox.AcceptsTab = True
            Me.scriptTextBox.Dock = System.Windows.Forms.DockStyle.Fill
            Me.scriptTextBox.Font = New System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (CByte((0))))
            Me.scriptTextBox.Location = New System.Drawing.Point(0, 24)
            Me.scriptTextBox.Multiline = True
            Me.scriptTextBox.Name = "scriptTextBox"
            Me.scriptTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
            Me.scriptTextBox.Size = New System.Drawing.Size(692, 481)
            Me.scriptTextBox.TabIndex = 0
            Me.scriptTextBox.Text = "function Start(){ " & vbCrLf & vbCrLf & "}" & vbCrLf
            Me.scriptTextBox.WordWrap = False
            Me.menuMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.sourceToolStripMenuItem})
            Me.menuMain.Location = New System.Drawing.Point(0, 0)
            Me.menuMain.Name = "menuMain"
            Me.menuMain.Size = New System.Drawing.Size(692, 24)
            Me.menuMain.TabIndex = 1
            Me.menuMain.Text = "menuStrip1"
            Me.sourceToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.executeToolStripMenuItem, Me.toolStripMenuItem1, Me.exitToolStripMenuItem})
            Me.sourceToolStripMenuItem.Name = "sourceToolStripMenuItem"
            Me.sourceToolStripMenuItem.Size = New System.Drawing.Size(52, 20)
            Me.sourceToolStripMenuItem.Text = "&Script"
            Me.executeToolStripMenuItem.Name = "executeToolStripMenuItem"
            Me.executeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5
            Me.executeToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
            Me.executeToolStripMenuItem.Text = If(scriptLauncher.Name = "Elsa.exe", "&Execute", "&Execute with " & Path.GetFileNameWithoutExtension(scriptLauncher.Name))
            Me.toolStripMenuItem1.Name = "toolStripMenuItem1"
            Me.toolStripMenuItem1.Size = New System.Drawing.Size(160, 6)
            Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
            Me.exitToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
            Me.exitToolStripMenuItem.Text = "E&xit"
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(692, 505)
            Me.Controls.Add(Me.scriptTextBox)
            Me.Controls.Add(Me.menuMain)
            Me.MainMenuStrip = Me.menuMain
            Me.Name = "MainForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = If(defaultScript IsNot Nothing, defaultScript.Name & " - Elsa", "Elsa")
            Me.menuMain.ResumeLayout(False)
            Me.menuMain.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()
            Me.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location)
        End Sub

        Private scriptTextBox As System.Windows.Forms.TextBox

        Private menuMain As System.Windows.Forms.MenuStrip

        Private sourceToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

        Private WithEvents executeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

        Private toolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator

        Private WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

        <STAThread>
        Public Shared Sub Main(ByVal args As String())
            If args.Length = 1 Then
                ConsoleInteractivityProvider.Run(args(0))
		Return
	    End If
	    If args.Length = 3 AndAlso args(0) = "/ap" Then
		Dim apAsm As [Assembly] = [Assembly].LoadFrom(args(1))
		Dim apTyp As Type = apAsm.GetType("Scripting.Interaction.AspectProvider")
		ConsoleInteractivityProvider.Run(args(2), apTyp)
	        Return		 
            End If

            Environment.SetEnvironmentVariable("TMP", Environment.CurrentDirectory, EnvironmentVariableTarget.Process)
            Dim i As Integer = Array.IndexOf(args, "/edit")
            If i >= 0 AndAlso i < args.Length - 1 Then defaultScript = New FileInfo(args(i + 1))
            i = Array.IndexOf(args, "/launch")
            If i >= 0 AndAlso i < args.Length - 1 Then scriptLauncher = New FileInfo(args(i + 1)) Else scriptLauncher = New FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)
            Dim form As MainForm = New MainForm()
            form.ShowDialog()
        End Sub

    End Class

End Namespace

