Imports System.Data
Imports System.Runtime.CompilerServices

Public Module SqlNaniteExtension

    <Extension>
    Public Function ExecuteMergeInto(Of TEntity)(Nanite As SqlNanite(Of TEntity), Con As IDbConnection, Entity As TEntity) As Integer
        Using Cmd = Con.CreateCommand() : With Cmd
                .CommandText = Nanite.GetMergeIntoScript(Entity)
                ExecuteMergeInto = .ExecuteNonQuery
            End With : End Using
    End Function

End Module