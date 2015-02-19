Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata
Imports System.Net.Mime.MediaTypeNames


' ayti na doume an theloume na tin xrisimopoiisoume
Public Class clsSearchView_M
    Inherits XPLiteObject

    <Key()> _
    Public Property id() As String

    Public Property pk() As String

    Public Property text000() As String
    Public Property text001() As String
    Public Property text002() As String
    Public Property text003() As String
    Public Property text004() As String
    Public Property text005() As String
    Public Property text006() As String
    Public Property text007() As String
    Public Property text008() As String
    Public Property text009() As String

    Public Property RowColor() As String

    Public Property num000() As Decimal
    Public Property num001() As Decimal
    Public Property num002() As Decimal
    Public Property num003() As Decimal
    Public Property num004() As Decimal
    Public Property num005() As Decimal
    Public Property num006() As Decimal
    Public Property num007() As Decimal
    Public Property num008() As Decimal
    Public Property num009() As Decimal

    Public Property date000() As Date
    Public Property date001() As Date
    Public Property date002() As Date
    Public Property date003() As Date
    Public Property date004() As Date

    Public Property bit000() As Boolean
    Public Property bit001() As Boolean
    Public Property bit002() As Boolean
    Public Property bit003() As Boolean
    Public Property bit004() As Boolean
    Public Property bit005() As Boolean
    Public Property bit006() As Boolean
    Public Property bit007() As Boolean
    Public Property bit008() As Boolean
    Public Property bit009() As Boolean

    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> Public Property binary000() As System.Drawing.Image
    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> Public Property binary001() As System.Drawing.Image


    Sub New()
    End Sub

    Public Sub New(ByVal Session As Session)
        MyBase.New(Session)
    End Sub

    Protected Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
        MyBase.New(session, classInfo)
    End Sub


End Class
