Namespace Scripting

	Public Module HostExtension

		Private Invoker

		Public Sub Invoke(fmt As String, ParamArray args As Object())
			Dim text As String = String.Format(fmt, args)
			Invoker.Host.Inform(text)
		End Sub

	End Module

End Namespace