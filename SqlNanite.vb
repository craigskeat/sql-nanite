Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data
Imports System.Reflection
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
    Friend Shared Function GetTableName(Optional Format As String = "[{0}].[{1}]") As String
        Dim Table As String, Schema As String = String.Empty
        Dim Attribute As TableAttribute = GetType(TEntity).GetCustomAttributes(GetType(TableAttribute), True).FirstOrDefault
        If Attribute Is Nothing OrElse String.IsNullOrWhiteSpace(Attribute.Name) Then
            Table = GetType(TEntity).Name
        ElseIf Not String.IsNullOrWhiteSpace(Attribute.Schema) Then
            Schema = Attribute.Schema
            Table = Attribute.Name
        Else
            Table = Attribute.Name
        End If
        GetTableName = String.Format(Format, Schema, Table)
    End Function

    ''' <summary>
    ''' Format Columns
    ''' </summary>
    ''' <param name="Format"></param>
    ''' <param name="OnlyKeys"></param>
    ''' <param name="Entity"></param>
    ''' <returns></returns>
    Friend Shared Function FormatColumns(Format As String, Optional OnlyKeys As Boolean = False, Optional Entity As TEntity = Nothing) As List(Of String)
        Dim SP As List(Of PropertyInfo)

        If OnlyKeys Then
            SP = GetType(TEntity).GetProperties.Where(Function(P) Not P.GetCustomAttribute(Of KeyAttribute) Is Nothing).ToList
        Else
            SP = GetType(TEntity).GetProperties.ToList()
        End If

        Return SP.Select(Function(P) P.FormatColumn(Format, Entity)).ToList

    End Function

    ''' <summary>
    ''' Get PrimaryKey Script
    ''' </summary>
    ''' <returns></returns>
    Friend Shared Function GetPrimaryKeyScript() As String
        Dim Ps = GetType(TEntity).GetProperties.Where(Function(P) Not P.GetCustomAttribute(Of KeyAttribute) Is Nothing).ToList

        Dim cs = Ps.Select(Function(P) New With {.ColName = P.Name, .Key1 = P.GetCustomAttribute(Of KeyAttribute)})


        GetPrimaryKeyScript = $"CONSTRAINT [PK__{GetTableName("{1}")}_{cs.Select(Function(c) c.ColName).Join("_")}] PRIMARY KEY ({cs.Select(Function(c) $"[{c.ColName}]").Join(", ")})"

    End Function

    ''' <summary>
    ''' Get Select From Script
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetSelectFromScript() As String
        GetSelectFromScript = $"SELECT * FROM {GetTableName()}"
    End Function

    ''' <summary>
    ''' Get Merge Into Script
    ''' </summary>
    ''' <param name="Entity"></param>
    ''' <param name="SourceAlias"></param>
    ''' <param name="TargetAlias"></param>
    ''' <returns></returns>
    Public Shared Function GetMergeIntoScript(Entity As TEntity,
                                              Optional SourceAlias As String = "Source",
                                              Optional TargetAlias As String = "Target") As String
        Dim NewLine = vbNewLine
        Dim Indent = Space(7)

        Dim CommaWithNewLineIndent = $",{NewLine}{Indent}"

        GetMergeIntoScript =
            $" MERGE  INTO {GetTableName()} [{TargetAlias}]" & NewLine &
            $" USING(" & NewLine &
            $"SELECT {FormatColumns("{PV} [{PN}]", Entity:=Entity).Join(CommaWithNewLineIndent)})" & NewLine &
            $"    AS [{SourceAlias}]" & NewLine &
            $"      ({FormatColumns("[{PN}]").Join(CommaWithNewLineIndent)})" & NewLine &
            $"    ON {FormatColumns($"[{SourceAlias}].[{{PN}}] = [{TargetAlias}].[{{PN}}]", True).Join($"{NewLine}   AND ")}" & NewLine &
            $"  WHEN  MATCHED THEN" & NewLine &
            $"UPDATE  SET" & NewLine &
            $"       {FormatColumns($"[{{PN}}] = [{SourceAlias}].[{{PN}}]").Join(CommaWithNewLineIndent)}" & NewLine &
            $"  WHEN  NOT MATCHED THEN " & NewLine &
            $"INSERT({FormatColumns("[{PN}]").Join(CommaWithNewLineIndent)})" & NewLine &
            $"VALUES({FormatColumns($"[{SourceAlias}].[{{PN}}]").Join(CommaWithNewLineIndent)});"
    End Function

    ''' <summary>
    ''' Get Delete From Script
    ''' </summary>
    ''' <param name="Entity"></param>
    ''' <returns></returns>
    Public Shared Function GetDeleteFromScript(Entity As TEntity) As String
        GetDeleteFromScript = $"DELETE FROM {GetTableName()} WHERE {FormatColumns("[{PN}] = {PV}", True, Entity).Join(" AND ")}"
    End Function

    ''' <summary>
    ''' Get Create Table Script
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetCreateTableScript() As String
        GetCreateTableScript = $"CREATE TABLE {GetTableName()} ({vbNewLine}{String.Join($",{vbNewLine}    ", FormatColumns("[{PN}] {PT}").Union({GetPrimaryKeyScript()}))})"
    End Function

    ''' <summary>
    ''' Get Drop Table Script
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetDropTableScript() As String
        GetDropTableScript = $"DROP TABLE {GetTableName()}"
    End Function

    ''' <summary>
    ''' Get Truncate TableScript
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetTruncateTableScript() As String
        GetTruncateTableScript = $"TRUNCATE TABLE {GetTableName()}"
    End Function


End Class


