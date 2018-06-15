Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data
Imports System.Runtime.CompilerServices

''' <summary>
''' SQL 语句生成机器人
''' </summary>
''' <typeparam name="TEntity">实体类型</typeparam>
Partial Public Class SqlNanite(Of TEntity)

    ''' <summary>
    ''' 表名
    ''' </summary>
    ''' <returns></returns>
    Friend Shared ReadOnly Property TableName() As String
        Get

            Dim Attribute As TableAttribute = GetType(TEntity).GetCustomAttributes(GetType(TableAttribute), True).FirstOrDefault
            If Attribute Is Nothing OrElse String.IsNullOrWhiteSpace(Attribute.Name) Then
                Return $"[{GetType(TEntity).Name}]"
            ElseIf Not String.IsNullOrWhiteSpace(Attribute.Schema) Then
                Return $"[{Attribute.Schema}].[{Attribute.Name}]"
            Else
                Return $"[{Attribute.Name}]"
            End If
        End Get
    End Property


    Friend Shared Function Cols() As List(Of String)


        Cols = GetType(TEntity).GetProperties.Select(Function(P) P.GetInfo()).ToList



    End Function




End Class


