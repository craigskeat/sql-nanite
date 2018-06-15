Partial Public Class SqlNanite(Of TEntity)

    Public Function GetMergeIntoScript(Entity As TEntity) As String

    End Function

    Public Function GetDeleteFromScript(Entity As TEntity) As String

    End Function

    ''' <summary>
    ''' 取得建表脚本
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetCreateTableScript() As String

        GetCreateTableScript = $"
            CREATE TABLE {TableName} (
            {String.Join($",{vbNewLine}", Cols)}
        )"

    End Function

    ''' <summary>
    ''' 取得删表脚本
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetDropTableScript() As String
        GetDropTableScript = $"DROP TABLE {TableName}"
    End Function

    ''' <summary>
    ''' 取得清表脚本
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetTruncateTableScript() As String
        GetTruncateTableScript = $"TRUNCATE TABLE {TableName}"
    End Function

End Class