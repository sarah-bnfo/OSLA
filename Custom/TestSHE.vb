Imports System.Reflection

Namespace Scripting.Interaction

	Public Class ConsoleInteractivityProviderEx 
	Inherits ConsoleInteractivityProvider

		Private Shared scriptFile As String

		Public ReadOnly Property TargetName
			Get
				Return scriptFile
			End Get
		End Property

		Public Overrides Function Action(name As String, ParamArray arguments() As Object)
			Try
				Dim extAsm As [Assembly] = [Assembly].LoadFrom(name & ".dll") 
				Dim extType As Type = extAsm.GetType("Scripting.HostExtension")
				Dim invField As FieldInfo = extType.GetField("Invoker", BindingFlags.NonPublic Or BindingFlags.Static)
				If invField IsNot Nothing Then invField.SetValue(Nothing, Me.Active)
				Return extType.InvokeMember("Invoke", BindingFlags.Public Or BindingFlags.InvokeMethod Or BindingFlags.Static Or BindingFlags.OptionalParamBinding, Nothing, Nothing, arguments)
			Catch ex As Exception
				Return MyBase.Action(name, arguments)
			End Try		
		End Function

	Public Shared Sub Main(args As String())
		If args.Length = 1 Then
			scriptFile = args(0)
			ConsoleInteractivityProvider.Run(args(0), GetType(ConsoleInteractivityProviderEx))
		End If
	End Sub

	End Class

End Namespace
