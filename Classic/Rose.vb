Imports System
Imports System.IO
Imports System.Reflection
Imports System.Drawing
Imports System.Windows.Forms
Imports Scripting.Integration

Namespace Rose

	Public MustInherit Class InteractionHost

		Public MustOverride Sub Inform(text As String)

		Public MustOverride Function Accept(text As String, ParamArray values As Object()) As Object

		Public MustOverride Function Confirm(text As String) As Boolean

		Public Overridable Function Action(name As String, ParamArray options As Object()) As Object		
			If name.ToLower() = "identify" Then Return "Rose"
			Return False
		End Function

		Public Overridable Function Import(source As String)
			Return ScriptCaller.CallScript(File.ReadAllText(source), Me)
		End Function

		Friend Overridable Sub Run(script As String)
			ScriptCaller.CallScript(File.ReadAllText(script), Me, "Start")
		End Sub

	End Class

	Public Class ConsoleInteractionHost
	Inherits InteractionHost

		Declare Sub AllocConsole Lib "Kernel32"()

		Private Function Choose(Accept As String, ParamArray items() As Object) As String
			Dim count As Integer = items.Length
			Console.WriteLine(Accept)
			For i As Integer = 0 To count - 1
				Console.WriteLine("{0}. {1}", i + 1, items(i))
			Next
			Console.Write("Choose [1 - {0}]: ", count)
			Dim choice As Integer = 1
			Dim text As String = Console.ReadLine()
			If text.Length > 0 Then choice = CInt(text)
			Console.WriteLine()
			If choice < 1 OrElse choice > count Then Return Nothing
			Return items(choice - 1)
		End Function

		Public Overrides Function Accept(text As String, ParamArray values() As Object) As Object
			If values.Length > 1 Then Return Choose(text, values)
			If values.Length = 1 Then
				Console.Write("{0} [{1}]: ", text, values(0))
			Else
				Console.Write("{0}: ", text)				
			End If
			Dim input As String = Console.ReadLine()
			Console.WriteLine()
			If input.Length = 0 Then input = IIf(values.Length = 1, values(0), Nothing)
			Dim result As Decimal
			If Decimal.TryParse(input, result) Then Return result Else Return input		
		End Function

		Public Overrides Sub Inform(text As String)
			Console.WriteLine(text)
			Console.WriteLine()
		End Sub

		Public Overrides Function Confirm(text As String) As Boolean
			Console.Write("{0} (y/n): ", text)
			Dim input As String = Console.ReadLine()
			Console.WriteLine()
			Return input.ToLower() = "y"
		End Function

		Friend Overrides Sub Run(script As String)
			AllocConsole()
			Console.SetOut(New System.IO.StreamWriter(Console.OpenStandardOutput()) With {.AutoFlush=True})
			Console.SetIn(New System.IO.StreamReader(Console.OpenStandardInput()))
			Console.Title = "Rose (Console)"
			MyBase.Run(script)
			Console.Write("Enter any key to exit...")
			Console.ReadLine()
		End Sub


	End Class

	Public Class DialogInteractionHost 
	Inherits InteractionHost

		Private Function Choose(Accept As String, ParamArray items As Object()) As String
			Dim box As Form = New Form()
			Dim buttonCancel As Button = New Button()
			Dim valueComboBox As ComboBox = New ComboBox()
			Dim buttonOK As Button = New Button()
			Dim messageLabel As Label = New Label()
			Dim result As String = Nothing
			messageLabel.Location = New Point(5, 10)
			messageLabel.Size = New Size(265, 100)
			messageLabel.AutoSize = False
			messageLabel.Text = Accept
			valueComboBox.Location = New Point(5, 90)
			valueComboBox.Size = New Size(330, 20)
			For i As Integer = 0 To items.Length - 1
				valueComboBox.Items.Add(items(i))
			Next			
			valueComboBox.SelectedIndex = 0
			valueComboBox.DropDownStyle = ComboBoxStyle.DropDownList
			buttonOK.DialogResult = DialogResult.OK
			buttonOK.Location = New Point(275, 10)
			buttonOK.Size = New Size(60, 25)
			buttonOK.Text = "OK"
			buttonCancel.DialogResult = DialogResult.Cancel
			buttonCancel.Location = New Point(275, 40)
			buttonCancel.Size = New Size(60, 25)
			buttonCancel.Text = "Cancel"
			box.AcceptButton = buttonOK
			box.CancelButton = buttonCancel
			box.ClientSize = New Size(340, 120)
			box.Controls.Add(valueComboBox)
			box.Controls.Add(buttonOK)
			box.Controls.Add(buttonCancel)
			box.Controls.Add(messageLabel)
			box.FormBorderStyle = FormBorderStyle.FixedDialog
			box.MaximizeBox = False
			box.MinimizeBox = False
			box.StartPosition = FormStartPosition.CenterScreen
			box.Text = "Rose"
			If box.ShowDialog(Nothing) = DialogResult.OK Then result = valueComboBox.Text
			box.Dispose()
			Return result
		End Function

		Public Overrides Function Accept(text As String, ParamArray values() As Object) As Object
			If values.Length > 1 Then Return Choose(text, values)
			Dim input As String = InputBox(text, "Rose", IIf(values.Length = 1, values(0), ""))
			If input = "" Then Return Nothing
			Dim result As Decimal
			If Decimal.TryParse(input, result) Then Return result Else Return input
		End Function

		Public Overrides Sub Inform(text As String)
			MsgBox(text, vbInformation, "Rose")
		End Sub

		Public Overrides Function Confirm(text As String) As Boolean
			Return MsgBox(text, vbQuestion + vbYesNo, "Rose") = vbYes
		End Function

	End Class

	Module Launcher

		<STAThread> Public Sub Main(args As String())
			If args.Length > 0 Then
				Dim host As InteractionHost
				Dim script As String
				If args.Length > 1 AndAlso args(0) = "/c" Then
					host = New ConsoleInteractionHost()
					script = args(1)
				Else
					host = New DialogInteractionHost()
					script = args(0)
				End If
				Try
					host.Run(script)
				Catch ex As Exception
					MsgBox(ex.ToString(), vbExclamation, "Rose")
				End Try
			End If
		End Sub

	End Module	

End Namespace

'vbc /t:winexe /win32icon:..\Resource\rose.ico Rose.vb /r:Mishel64.dll