Imports System.Reflection
Imports System.Runtime.CompilerServices

''' <summary>
''' Type Extension
''' </summary>
Public Module TypeExtension

    ''' <summary>
    ''' Create a instance of T
    ''' </summary>
    ''' <param name="Arguments">Arguments</param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateInstance(T As Type, ParamArray Arguments As Object()) As Object
        Return T.InvokeMember("", BindingFlags.CreateInstance, Nothing, Nothing, Arguments)
    End Function

    ''' <summary>
    ''' Set Property
    ''' </summary>
    ''' <param name="T"></param>
    ''' <param name="Target"></param>
    ''' <param name="PropertyName"></param>
    ''' <param name="PropertyValue"></param>
    <Extension>
    Public Sub SetProperty(T As Type, ByRef Target As Object, PropertyName As String, ByRef PropertyValue As Object)
        If PropertyValue Is DBNull.Value Then Return
        T.InvokeMember(PropertyName, BindingFlags.SetProperty, Nothing, Target, {PropertyValue})
    End Sub

    ''' <summary>
    ''' Get Property
    ''' </summary>
    ''' <param name="T"></param>
    ''' <param name="Target"></param>
    ''' <param name="PropertyName"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetProperty(T As Type, ByRef Target As Object, PropertyName As String) As Object
        Return T.InvokeMember(PropertyName, BindingFlags.GetProperty, Nothing, Target, Nothing)
    End Function


End Module
