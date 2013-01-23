Imports System.Threading

Public Class clsWait
    Inherits DevExpress.Utils.WaitDialogForm

    Private form As clsWait
    Private _thread As Thread
    Private _caption As String, _title As String

    Private Sub New(ByVal caption As String, ByVal title As String)
        MyBase.New(caption, title)
    End Sub
    Public Sub New()
    End Sub
    Public Sub New(ByVal caption As String)
        MyBase.New(caption)
    End Sub
    Public Sub New(ByVal caption As String, ByVal size As Size)
        MyBase.New(caption, size)
    End Sub
    Public Sub New(ByVal caption As String, ByVal title As String, ByVal size As Size)
        MyBase.New(caption, title, size)
    End Sub
    Public Sub New(ByVal caption As String, ByVal title As String, ByVal size As Size, ByVal parent As Form)
        MyBase.New(caption, title, size, parent)
    End Sub

    Private Sub CreateInstance()
        form = New clsWait(_caption, _title)
        Application.Run(form)
    End Sub

    Public Overloads Sub Show(ByVal caption As String, ByVal title As String)
        _caption = caption
        _title = title

        If form Is Nothing Then
            _thread = New Thread(AddressOf CreateInstance)
            _thread.IsBackground = True
            _thread.Start()
        End If
    End Sub

    Public Overloads Sub SetCaption(ByVal caption As String)
        If form IsNot Nothing Then
            _caption = caption
            form.SetFormCaption()
        Else
            Show(caption, "")
        End If
    End Sub

    Public Overloads Sub Close()
        If form IsNot Nothing Then
            form.CloseForm()
            form = Nothing
        End If
    End Sub

    Private Sub CloseForm()
        If InvokeRequired Then
            Invoke(New MethodInvoker(AddressOf CloseForm))
            Return
        End If
        Application.ExitThread()
    End Sub

    Private Sub SetFormCaption()
        If InvokeRequired Then
            Invoke(New MethodInvoker(AddressOf SetFormCaption))
            Return
        End If

        MyBase.SetCaption(_caption)
    End Sub

End Class


