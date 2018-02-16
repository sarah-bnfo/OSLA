Imports System.Reflection

Namespace Scripting.Interaction

	Public Class AspectProvider 
	Inherits AspectProviderBase

		Public Overrides Function OnAction(scriptObj As Object, name As String, arguments As Object(), ByRef result As Object) As Boolean
			Try
				Dim extAsm As [Assembly] = [Assembly].LoadFrom(name & ".dll") 
				Dim extType As Type = extAsm.GetType("Scripting.HostExtension")
				Dim invField As FieldInfo = extType.GetField("Invoker", BindingFlags.NonPublic Or BindingFlags.Static)
				If invField IsNot Nothing Then invField.SetValue(Nothing, scriptObj)
				result = extType.InvokeMember("Invoke", BindingFlags.Public Or BindingFlags.InvokeMethod Or BindingFlags.Static Or BindingFlags.OptionalParamBinding, Nothing, Nothing, arguments)
				Return True
			Catch ex As Exception
				result = Nothing
				Return False
			End Try		
		End Function

	End Class

End Namespace
