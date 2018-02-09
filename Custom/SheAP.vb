Imports System.Reflection

Namespace Scripting.Interaction

	Public Class ActionProvider 
	Implements IActionProvider

		Private Function Action(script As Object, name As String, arguments As Object()) Implements IActionProvider.Action
			Try
				Dim extAsm As [Assembly] = [Assembly].LoadFrom(name & ".dll") 
				Dim extType As Type = extAsm.GetType("Scripting.HostExtension")
				Dim invField As FieldInfo = extType.GetField("Invoker", BindingFlags.NonPublic Or BindingFlags.Static)
				If invField IsNot Nothing Then invField.SetValue(Nothing, script)
				Return extType.InvokeMember("Invoke", BindingFlags.Public Or BindingFlags.InvokeMethod Or BindingFlags.Static Or BindingFlags.OptionalParamBinding, Nothing, Nothing, arguments)
			Catch ex As Exception
				Return False
			End Try		
		End Function

	End Class

End Namespace
