Namespace Scripting.Integration

	Public Module ScriptCaller

		Public Function CallScript(code As String, host As Object, Optional entry As String=Nothing)
			Dim sct As Type = Type.GetTypeFromProgID("MSScriptControl.ScriptControl")
			Dim sco = Activator.CreateInstance(sct)
			sco.Timeout = -1
			sco.Language = "JScript"
			sco.AddObject("Host", host)
			sco.AddCode(code)
			If entry Is Nothing Then Return sco.CodeObject
			Try
				Return sco.Run(entry)
			Catch ex As System.Runtime.InteropServices.COMException
				If ex.ErrorCode = -2147352570 Then
					Throw New ArgumentException(entry & " function missing")
				Else
					Throw New ArgumentException(ex.Message)
				End If
			End Try	
		End Function
	

	End Module

End Namespace
