Namespace Shila

	Public Class BasicInteractionHost 

		Private Shared title As String = "Shila"

		Public Function Import(scriptFile As String) As Object
			Return Scripting.Integration.ScriptCaller.CallScript(System.IO.File.ReadAllText(scriptFile), Me, Nothing)
		End Function

		Public Function Action(name As String, ParamArray arguments As Object()) As Object
			If name.ToLower() = "identify" Then Return "Shila"
			Return False
		End Function		

		Public Sub Inform(text As String)
			MsgBox(text, vbInformation, title)
		End Sub

		Public Function Concur(confirmation As String) As Boolean
			Return MsgBox(confirmation, vbQuestion + vbYesNo, title) = vbYes
		End Function

		Public Function Accept(prompt As String, Optional value As Object=Nothing) As Object
			Dim input As String = InputBox(prompt, title, value)
			If input = "" Then 
				Return value
			Else
				Dim result As Decimal
				If Decimal.TryParse(input, result) Then Return result Else Return input
			End If		
		End Function

		Public Function Choose(prompt As String, ParamArray choices As Object()) As Object
			Dim value As Object = Nothing
			If choices.Length > 0 Then	
				prompt = prompt & vbCrLf & "[" & String.Join("|", choices) & "]"
				value = choices(0)
			End If
			Return Accept(prompt, value)
		End Function

		Private Sub Start(scriptFile As String)
			Scripting.Integration.ScriptCaller.CallScript(System.IO.File.ReadAllText(scriptFile), Me, "Start")
		End Sub

		Private Shared Sub Execute(scriptFile As String)
			Dim host As New BasicInteractionHost()
			Try
				host.Start(scriptFile)
			Catch ex As Exception
				MsgBox("Error: " & ex.Message, vbExclamation, title)
			End Try
		End Sub

		<STAThread> Public Shared Sub Main(args As String())
			If args.Length < 1 Then
				MsgBox("USAGE: Shila script-file", vbExclamation, title)
			Else
				BasicInteractionHost.Execute(args(0))
			End If
		End Sub

	End Class

End Namespace

'vbc /nologo /t:winexe /d:_MyType=\"Empty\" Shila.vb Saina.vb
'vbc /nologo /t:winexe /d:_MyType=\"Empty\" Shila.vb /r:Mishel.dll
