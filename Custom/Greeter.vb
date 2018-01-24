Namespace Greeter

	Public Class Export

		Public Function Greet(name As String, region As String, respect As Boolean) As String
			Dim greeting As String = IIf(respect, "Hello", "Hi")
			Return String.Format("{0} {1} from the {2}.", greeting, name, region)
		End Function

	End Class

End Namespace
