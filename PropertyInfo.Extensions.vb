Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Craig.Data

Public Module PropertyInfoExtension

    ''' <summary>
    ''' Resolve DBMS Data Type
    ''' </summary>
    Public ResolveDbmsDataType As Func(Of PropertyInfo, String) =
        Function(P)

            Dim PT = ExtractType(P)

            Dim t As String

            Select Case PT
                Case GetType(DateTimeOffset)
                    t = "[DATETIMEOFFSET]"
                Case GetType(Byte)
                    t = "[TINYINT]"
                Case GetType(Byte())
                    t = "[BINARY]"
                Case GetType(Boolean)
                    t = "[BIT]"
                Case GetType(Guid)
                    t = $"[UNIQUEIDENTIFIER]"
                Case GetType(Long)
                    t = $"[BIGINT]"
                Case GetType(Integer)
                    t = $"[INT]"
                Case GetType(Date)
                    t = $"[{P.GetCustomAttributeProperty(Of ColumnAttribute)("TypeName", "DATETIME")}]"
                Case GetType(String)
                    Dim CT = P.GetCustomAttributeProperty(Of ColumnAttribute)("TypeName", "NVARCHAR")
                    Dim CL = P.GetCustomAttributeProperty(Of StringLengthAttribute)("MaximumLength"， "")
                    If Not String.IsNullOrEmpty(CL) Then
                        t = $"[{CT}]({CL})"
                    Else
                        t = $"[{CT}]"
                    End If
                Case GetType(Byte())
                    Dim CT = UCase(P.GetCustomAttributeProperty(Of ColumnAttribute)("TypeName"))
                    If CT = "IMAGE" Then t = "[IMAGE]" _
                    Else Throw New NotSupportedException(P.PropertyType.ToString)
                Case Else
                    Throw New NotSupportedException(P.PropertyType.ToString)
            End Select

            Return t
        End Function

    ''' <summary>
    ''' Extract Type
    ''' </summary>
    Public ExtractType As Func(Of PropertyInfo, Type) =
        Function(P)
            Dim PT As Type
            Dim AllowNull As Boolean = False

            If P.PropertyType.IsGenericType AndAlso P.PropertyType.GetGenericTypeDefinition Is GetType(Nullable(Of)) Then
                PT = P.PropertyType.GetGenericArguments.FirstOrDefault
                AllowNull = True
            Else
                PT = P.PropertyType
            End If
            Return PT
        End Function

    ''' <summary>
    ''' Format Column
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="P"></param>
    ''' <param name="Fmt"></param>
    ''' <param name="Entity"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FormatColumn(Of TEntity)(P As PropertyInfo, Fmt As String, Optional Entity As TEntity = Nothing) As String

        Dim PN As String = Nothing, PC As Type = Nothing, PT As String = Nothing, PV As Object = Nothing, NN As String = Nothing

        If Fmt.Contains("{PN}") Then PN = P.Name
        If Fmt.Contains("{PC}") Then PC = P.PropertyType
        If Fmt.Contains("{PT}") Then PT = ResolveDbmsDataType(P)
        If Fmt.Contains("{PV}") Then
            PV = GetType(TEntity).GetProperty(Entity, P.Name)
            If IsNothing(PV) Then
                PV = " NULL"
            Else
                Select Case ExtractType(P)
                    Case GetType(String)
                        PV = $"'{PV}'"
                    Case GetType(Date)
                        PV = $"'{PV}'"
                    Case GetType(DateTime)
                        PV = $"'{PV}'"
                    Case Else
                        PV = $" {PV}"
                End Select
            End If
        End If
        If Fmt.Contains("{NN}") Then _
            NN = IIf(Not IsNothing(P.GetCustomAttribute(Of KeyAttribute)) OrElse
                     Not IsNothing(P.GetCustomAttribute(Of RequiredAttribute)), "NOT ", String.Empty) & "NULL"

        Fmt = Fmt.Replace("{PN}", "{0}").Replace("{PC}", "{1}").Replace("{PT}", "{2}").Replace("{PV}", "{3}").Replace("{NN}", "{4}")

        Return String.Format(Fmt, PN, PC, PT, PV, NN)

    End Function

    ''' <summary>
    ''' Get CustomAttribute Property
    ''' </summary>
    ''' <typeparam name="TAttribute"></typeparam>
    ''' <param name="P"></param>
    ''' <param name="AttributePropertyName"></param>
    ''' <param name="DefaultValue"></param>
    ''' <returns></returns>
    <Extension>
    Friend Function GetCustomAttributeProperty(Of TAttribute As Attribute)(ByRef P As PropertyInfo, AttributePropertyName As String, Optional DefaultValue As String = "") As Object
        Dim Attrib = P.GetCustomAttribute(GetType(TAttribute))
        If Attrib Is Nothing Then Return DefaultValue
        GetCustomAttributeProperty = GetType(TAttribute).InvokeMember(AttributePropertyName, BindingFlags.GetProperty, Nothing, Attrib, Nothing)
    End Function

End Module
