Imports System
Imports System.IO
Imports System.Reflection

Namespace Rose

	#If CORE

	Public MustInherit Class InteractionHost

		Public MustOverride Function Inform(text As String)

		Public MustOverride Function Accept(text As String, Optional value As Object=Nothing) As Object

		Public Overridable Function Action(name As String, ParamArray options As Object()) As Object		
			If name.ToLower() = "identify" Then Return "Rose"
			Return False
		End Function

		Public Overridable Function Import(source As String)
			Return Scripting.Integration.ScriptCaller.CallScript(File.ReadAllText(source), Me)
		End Function

		Public Overridable Sub Run(script As String)
			Scripting.Integration.ScriptCaller.CallScript(File.ReadAllText(script), Me, "Start")
		End Sub

	End Class

	Public Class ConsoleInteractionHost
	Inherits InteractionHost

		Public Overrides Function Accept(text As String, Optional value As Object=Nothing) As Object
			If value IsNot Nothing Then
				Console.Write("{0} [{1}]: ", text, value)
			Else
				Console.Write("{0}: ", text)				
			End If
			Dim input As String = Console.ReadLine()
			If input.Length = 0 Then input = Nothing
			Dim result As Decimal
			If Decimal.TryParse(input, result) Then Return result Else Return input		
		End Function

		Public Overrides Function Inform(text As String)
			Console.WriteLine(text)
			Return True
		End Function

	End Class

	Public Class DialogInteractionHost 
	Inherits InteractionHost

		Public Overrides Function Accept(text As String, Optional value As Object=Nothing) As Object
			Dim input As String = InputBox(text, "Rose", value)
			If input = "" Then Return Nothing
			Dim result As Decimal
			If Decimal.TryParse(input, result) Then Return result Else Return input
		End Function

		Public Overrides Function Inform(text As String)
			Return MsgBox(text, vbInformation + vbOkCancel, "Rose") = vbOK
		End Function

	End Class

	#ELSE

	Module Launcher

		<STAThread> Public Sub Main(args As String())
			If args.Length > 0 Then
				Dim host As InteractionHost
				Dim script As String = args(0)
				#If CONS
				Console.Clear()
				host = New ConsoleInteractionHost()
				#Else
				host = New DialogInteractionHost()
				#End If
				Try
					host.Run(script)
				Catch ex As Exception
					#If CONS
					Console.WriteLine(ex.ToString())
					#Else
					MsgBox(ex.ToString(), vbExclamation, "Rose")
					#End If
				End Try
			End If
		End Sub

	End Module
	
	#End If

End Namespace

'vbc /out:RoseCore.dll /t:library /d:CORE Rose.vb /r:MishelQ.dll
'vbc /t:winexe Rose.vb /r:RoseCore.dll
'vbc /d:CONS /out:Rose.com Rose.vb /r:RoseCore.dll
'ren Rose.com.exe Rose.com