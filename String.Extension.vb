Imports System.Runtime.CompilerServices

Module StringExtension

    ''' <summary>
    ''' String Join
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="values"></param>
    ''' <param name="separator"></param>
    ''' <returns></returns>
    <Extension>
    Function Join(Of T)(values As IEnumerable(Of T), separator As String)
        Return String.Join(separator, values)
    End Function
End Module
