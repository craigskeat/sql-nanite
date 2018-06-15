Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Reflection
Imports System.Runtime.CompilerServices

Public Module PropertyInfoExtension

    <Extension>
    Public Function GetInfo(P As PropertyInfo) As String

        Dim PT As Type
        Dim AllowNull As Boolean = False

        If P.PropertyType.IsGenericType AndAlso P.PropertyType.GetGenericTypeDefinition Is GetType(Nullable(Of)) Then
            PT = P.PropertyType.GetGenericArguments.FirstOrDefault
            AllowNull = True
        Else
            PT = P.PropertyType
        End If

        Dim t As String

        Select Case PT
            Case GetType(Integer)
                t = $"[INT]"
            Case GetType(DateTime)
                t = $"{P.GetCustomAttributeProperty(Of ColumnAttribute)("TypeName", "DATETIME")}"
            Case GetType(String)
                Dim CT = P.GetCustomAttributeProperty(Of ColumnAttribute)("TypeName", "NVARCHAR")
                Dim CL = P.GetCustomAttributeProperty(Of StringLengthAttribute)("MaximumLength", "MAX")
                t = $"[{CT}]({CL})"
            Case Else
                Throw New NotSupportedException(P.PropertyType.ToString)

        End Select

        Return $"[{P.Name}] {t}"
    End Function

    <Extension>
    Friend Function GetCustomAttributeProperty(Of TAttribute As Attribute)(ByRef P As PropertyInfo, AttributePropertyName As String, Optional DefaultValue As String = "")
        Dim Attrib = P.GetCustomAttribute(GetType(TAttribute))
        If Attrib Is Nothing Then Return DefaultValue
        GetCustomAttributeProperty = GetType(TAttribute).InvokeMember(AttributePropertyName, BindingFlags.GetProperty, Nothing, Attrib, Nothing)
    End Function

End Module
