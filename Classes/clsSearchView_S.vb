Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata
Imports System.Net.Mime.MediaTypeNames


' na tin valoume sta combo pou einai server mode
Public Class clsSearchView_S
    Inherits XPLiteObject

    <Key()> _
    Public Property id() As String

    Public Property text000() As String
    Public Property text001() As String
    Public Property text002() As String

    Public Property RowColor() As String

    Public Property num000() As Decimal
    Public Property num001() As Decimal
    Public Property num002() As Decimal

    Public Property date000() As Date
    Public Property date001() As Date


    Public Property bit000() As Boolean
    Public Property bit001() As Boolean


    Sub New()
    End Sub

    Public Sub New(ByVal Session As Session)
        MyBase.New(Session)
    End Sub

    Protected Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
        MyBase.New(session, classInfo)
    End Sub

End Class
