Imports System.Data
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Partial Public Module DbConnectionExtension

    ''' <summary>
    ''' Executes an SQL statement against the Connection object of a .NET Framework data provider, and returns the number of rows affected.
    ''' </summary>
    ''' <param name="Con">Connection.</param>
    ''' <param name="CommandText">SQL Command Text.</param>
    ''' <returns>The number of rows affected.</returns>
    <Extension>
    Function ExecuteNonQuery(ByRef Con As IDbConnection, CommandText As String) As Integer
        Using Cmd = Con.CreateCommand() : With Cmd
                .CommandText = CommandText
                ExecuteNonQuery = .ExecuteNonQuery()
            End With : End Using
    End Function

    ''' <summary>
    ''' Executes an SQL statement against the Connection object of a .NET Framework data provider, and returns the number of rows affected.
    ''' </summary>
    ''' <param name="Con">Connection.</param>
    ''' <param name="CommandText">SQL Command Text.</param>
    ''' <returns></returns>
    <Extension>
    Function ExecuteNonQuery(ByRef Con As IDbConnection, ParamArray CommandTexts() As String) As List(Of Integer)
        ExecuteNonQuery = New List(Of Integer)
        Using Cmd = Con.CreateCommand() : With Cmd : For Each CommandText In CommandTexts
                    .CommandText = CommandText
                    ExecuteNonQuery.Add(.ExecuteNonQuery())
                Next : End With : End Using
    End Function

    ''' <summary>
    ''' Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
    ''' </summary>
    ''' <param name="Con">Connection.</param>
    ''' <param name="CommandText">SQL Command Text.</param>
    ''' <returns>The first column of the first row in the resultset.</returns>
    <Extension>
    Function ExecuteScalar(ByRef Con As IDbConnection, CommandText As String) As Object
        Using Cmd = Con.CreateCommand() : With Cmd
                .CommandText = CommandText
                ExecuteScalar = .ExecuteScalar()
            End With : End Using
    End Function

    ''' <summary>
    ''' Executes the query, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
    ''' </summary>
    ''' <param name="Con">Connection.</param>
    ''' <param name="CommandText">SQL Command Text.</param>
    ''' <returns>The first column of the first row in the resultset.</returns>
    <Extension>
    Function ExecuteScalar(Of T)(ByRef Con As IDbConnection, CommandText As String) As T
        Using Cmd = Con.CreateCommand() : With Cmd
                .CommandText = CommandText
                ExecuteScalar = .ExecuteScalar()
            End With : End Using
    End Function

    ''' <summary>
    ''' Executes the CommandText against the Connection and builds an IDataReader.
    ''' </summary>
    ''' <param name="Con">Connection.</param>
    ''' <param name="CommandText">SQL Command Text.</param>
    ''' <param name="ReadingAction">Reading Action.</param>
    <Extension>
    Sub ExecuteReader(ByRef Con As IDbConnection, CommandText As String, ReadingAction As Action(Of IDataReader))
        Using Cmd = Con.CreateCommand() : With Cmd
                .CommandText = CommandText
                Using Reader = .ExecuteReader()
                    ReadingAction?.Invoke(Reader)
                End Using
            End With : End Using
    End Sub

    ''' <summary>
    ''' Select From
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Con"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SelectFrom(Of T)(ByRef Con As IDbConnection) As IList(Of T)
        SelectFrom = New List(Of T)
        Con.ExecuteReader(
            SqlNanite(Of T).GetSelectFromScript(),
            Sub(Reader)
                While Reader.Read()
                    Dim Item = GetType(T).CreateInstance()
                    GetType(T).GetProperties.ToList.ForEach(Sub(P) GetType(T).SetProperty(Item, P.Name, Reader(P.Name)))
                    SelectFrom.Add(Item)
                End While
            End Sub)
    End Function

    ''' <summary>
    ''' Load Serialized Data (From File)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Con"></param>
    ''' <param name="FilePath"></param>
    <Extension>
    Public Sub LoadXmlData(Of T)(ByRef Con As IDbConnection, FilePath As String)
        Dim List As List(Of T)
        Dim sr As New XmlSerializer(GetType(List(Of T)))
        Using fs As New FileStream(FilePath, FileMode.Open)
            List = sr.Deserialize(fs)
        End Using

        For Each Item In List
            Item.MergeInto(Con)
        Next
    End Sub

    ''' <summary>
    ''' Load Serialized Data (From Manifest Resource)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Con"></param>
    <Extension>
    Public Sub LoadXmlData(Of T)(ByRef Con As IDbConnection)
        Dim List As List(Of T)
        Dim sr As New XmlSerializer(GetType(List(Of T)))
        Dim ManiKey = $"{GetType(T).FullName}.xml"

        If Not GetType(T).Assembly.GetManifestResourceNames.Contains(ManiKey) Then Exit Sub

        Using str = GetType(T).Assembly.GetManifestResourceStream(ManiKey)
            List = sr.Deserialize(str)
        End Using

        For Each Item In List
            Item.MergeInto(Con)
        Next
    End Sub

    ''' <summary>
    ''' Create Table
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Con"></param>
    ''' <returns></returns>
    <Extension>
    Function CreateTable(Of T)(ByRef Con As IDbConnection)
        Return Con.ExecuteNonQuery(SqlNanite(Of T).GetCreateTableScript())
    End Function

    ''' <summary>
    ''' Drop All Tables
    ''' </summary>
    ''' <param name="Con"></param>
    ''' <returns></returns>
    <Extension>
    Public Function DropAllTables(ByRef Con As IDbConnection) As (Phase1 As Integer, Phase2 As Integer)
        Dim Result = Con.ExecuteNonQuery("EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'",
                                         "EXEC sp_MSForEachTable @command1 = ""DROP TABLE ?""")
        DropAllTables.Phase1 = Result(0)
        DropAllTables.Phase2 = Result(1)
    End Function

    ''' <summary>
    ''' Merge Into
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Con"></param>
    ''' <param name="Item"></param>
    ''' <returns></returns>
    <Extension>
    Function MergeInto(Of T)(ByRef Con As IDbConnection, Item As T) As Integer
        Return Con.ExecuteNonQuery(SqlNanite(Of T).GetMergeIntoScript(Item))
    End Function

    ''' <summary>
    ''' Delete From
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Con"></param>
    ''' <param name="Item"></param>
    ''' <returns></returns>
    <Extension>
    Function DeleteFrom(Of T)(ByRef Con As IDbConnection, Item As T) As Integer
        Return Con.ExecuteNonQuery(SqlNanite(Of T).GetDeleteFromScript(Item))
    End Function

    ''' <summary>
    ''' Merge Into
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Item"></param>
    ''' <param name="Con"></param>
    ''' <returns></returns>
    <Extension>
    Function MergeInto(Of T)(Item As T, ByRef Con As IDbConnection) As Integer
        Return Con.MergeInto(Item)
    End Function

    ''' <summary>
    ''' Delete From
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Item"></param>
    ''' <param name="Con"></param>
    ''' <returns></returns>
    <Extension>
    Function DeleteFrom(Of T)(Item As T, ByRef Con As IDbConnection) As Integer
        Return Con.DeleteFrom(Item)
    End Function

End Module
