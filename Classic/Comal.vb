Namespace Comal

	Public Class BasicInteractionHost 

		Private Shared title As String = "Comal"

		Public Function Import(scriptFile As String) As Object
			Return Scripting.Integration.ScriptCaller.CallScript(System.IO.File.ReadAllText(scriptFile), Me, Nothing)
		End Function

		Public Function Action(name As String, ParamArray arguments As Object()) As Object
			Select Case name.ToLower()
				Case "identify"
					Return title
				case "clearscreen"
					Console.Clear()
					Return true
				case "blankline"
					Console.WriteLine()
					Return true
			End Select			
			Return False
		End Function		

		Public Function Accept(text As String, Optional value As Object=Nothing) As Object
			If value IsNot Nothing Then
				text = text & " [" & value & "]"
			End If
			Console.Write("{0}: ", text)
			Dim input As String = Console.ReadLine()
			Console.WriteLine()
			If input = "" Then Return value
			Dim result As Decimal
			If Decimal.TryParse(input, result) Then Return result Else Return input
		End Function

		Public Sub Inform(text As String)
			Console.WriteLine(text)			
		End Sub

		Public Function Confirm(text As String) As Boolean
			Console.Write("{0} (y/n): ", text)
			Dim result As String = Console.ReadLine()
			Console.WriteLine()
			Return result.ToLower() = "y"
		End Function

		Private Sub Start(scriptFile As String)
			Scripting.Integration.ScriptCaller.CallScript(System.IO.File.ReadAllText(scriptFile), Me, "Start")
		End Sub

		Private Shared Sub Execute(scriptFile As String)
			Console.Clear()
			Dim host As New BasicInteractionHost()
			Try
				host.Start(scriptFile)
			Catch ex As Exception
				Console.WriteLine("ERROR: " & ex.Message)
			End Try
			Console.WriteLine()
			Console.Write("Enter any key to exit...")
			Console.ReadLine()
		End Sub

		<STAThread> Public Shared Sub Main(args As String())
			If args.Length < 1 Then
				Console.WriteLine("USAGE: Shila script-file")
			Else
				BasicInteractionHost.Execute(args(0))
			End If
		End Sub

	End Class

End Namespace

'vbc /nologo /d:_MyType=\"Empty\" Comal.vb Saina.vb
'vbc /nologo /d:_MyType=\"Empty\" Comal.vb /r:Mishel.dll
