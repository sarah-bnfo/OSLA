Imports System
Imports System.IO
Imports System.Reflection
Imports System.Drawing
Imports Scripting.Integration

Namespace Rose

	Public MustInherit Class InteractionHost

		Public MustOverride Sub Inform(text As String)

		Public MustOverride Function Accept(text As String, Optional value As Object=Nothing) As Object

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

		Public Overrides Function Accept(text As String, Optional value As Object=Nothing) As Object
			If value IsNot Nothing Then
				Console.Write("{0} [{1}]: ", text, value)
			Else
				Console.Write("{0}: ", text)				
			End If
			Dim input As String = Console.ReadLine()
			Console.WriteLine()
			If input.Length = 0 Then input = value
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

		Public Overrides Function Accept(text As String, Optional value As Object=Nothing) As Object
			Dim input As String = InputBox(text, "Rose", value)
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