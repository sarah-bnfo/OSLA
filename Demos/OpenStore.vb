Imports System
Imports System.IO
Imports System.Xml.Serialization
Imports System.Collections.Generic

Namespace Persistence

	Public Class Item
	Implements IEquatable(Of Item)

		<XmlAttribute("key")> Public Property Key As String

		<XmlAttribute("value")> Public Property Value As String

		Private Overloads Function Equals(other As Item) As Boolean Implements IEquatable(Of Item).Equals
			Return Key = other.Key
		End Function

	End Class

	Public Class Group
	Implements IEquatable(Of Group)

		Friend Store As Store

		<XmlAttribute("id")> Public Property Id As String

		<XmlArray("items"), XmlArrayItem("item")> Public Property Items As New List(of Item)

		Private Function Find(key As String) As Item
			Return Items.Find(Function(e) e.Key = key)						
		End Function

		Public Function Peek(key As String) As String
			Dim item As Item = Find(key)
			If item Is Nothing Then Return Nothing
			Return item.Value
		End Function

		Public Function Put(key As String, value As String) As Group	
			Dim item As Item
			Dim dirty As Boolean = True
			If value Is Nothing
				item = New Item
				item.Key = key
				dirty = Items.Remove(item)
			Else
				item = Find(key)
				If item Is Nothing Then
					item = New Item
					item.Key = key
					item.Value = value
					Items.Add(item)
				Else
					item.Value = value
				End If
			End If
			If dirty Then Store.OnModify()
			Return Me				
		End Function

		Public Sub Purge()
			Store.Clear(Me)
		End Sub

		Private Overloads Function Equals(other As Group) As Boolean Implements IEquatable(Of Group).Equals
			Return Id = other.Id
		End Function

	End Class

	<XmlRoot("store")> Public Class Store

		Friend AutoCommit As Boolean

		Private modified As Boolean = False

		<XmlIgnore> Public Property Name As String

		<XmlArray("groups"), XmlArrayItem("group")> Public Property Groups As New List(Of Group)

		Friend Sub OnModify()
			If AutoCommit Then Commit() Else modified = True
		End Sub

		Friend Sub Clear(group As Group)
			If Groups.Remove(group) Then OnModify()
		End Sub

		Public Function Choose(id As String, Optional allowCreate As Boolean = False) As Group
			Dim group As Group = Groups.Find(Function(e) e.Id = id)
			If group Is Nothing AndAlso allowCreate = True Then
				group = New Group
				group.Id = id
				Groups.Add(group)
				OnModify()
			End If
			group.Store = Me
			Return group
		End Function
				
		Public Function Changed() As Boolean
			Return modified
		End Function

		Public Sub Commit()
			Dim serializer As New XmlSerializer(GetType(Store)) 
			Dim ns As New XmlSerializerNamespaces()
			ns.Add("", "")
			Using writer As New StreamWriter(Name + ".kvs")
				serializer.Serialize(writer, Me, ns)
			End Using
			modified = True	
		End Sub

	End Class

End Namespace	

Namespace Scripting

	Public Module HostExtension

		Public Function Invoke(storeName As String, Optional storeAutoCommit As Boolean=True) As Persistence.Store
			Dim serializer As New XmlSerializer(GetType(Persistence.Store))
			Dim store As Persistence.Store 
			Try
				Using reader As New StreamReader(storeName + ".kvs")
					store = CType(serializer.Deserialize(reader), Persistence.Store)
				End Using
			Catch
				store = New Persistence.Store	
			End Try
			store.Name = storeName
			store.AutoCommit = storeAutoCommit
			Return store			
		End Function

	End Module

End Namespace
