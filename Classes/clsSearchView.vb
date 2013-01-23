Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata

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


    Public Sub New(ByVal Session As Session)
        MyBase.New(Session)
    End Sub

    Sub New()
    End Sub
    Protected Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
        MyBase.New(session, classInfo)
    End Sub

End Class

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

    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> _
    Public Property binary000() As Image
    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> _
    Public Property binary001() As Image

    Public Sub New(ByVal Session As Session)
        MyBase.New(Session)
    End Sub

    Sub New()
    End Sub
    Protected Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
        MyBase.New(session, classInfo)
    End Sub

End Class


' Ayti tin xrisimopoioume stis search formes
Public Class clsSearchView_L
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
    Public Property text010() As String
    Public Property text011() As String
    Public Property text012() As String
    Public Property text013() As String
    Public Property text014() As String
    Public Property text015() As String
    Public Property text016() As String
    Public Property text017() As String
    Public Property text018() As String
    Public Property text019() As String
    Public Property text020() As String
    Public Property text021() As String
    Public Property text022() As String
    Public Property text023() As String
    Public Property text024() As String

    Public Property RowColor() As String
    Public Property CellColor() As String

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
    Public Property num010() As Decimal
    Public Property num011() As Decimal
    Public Property num012() As Decimal
    Public Property num013() As Decimal
    Public Property num014() As Decimal

    Public Property date000() As Date
    Public Property date001() As Date
    Public Property date002() As Date
    Public Property date003() As Date
    Public Property date004() As Date
    Public Property date005() As Date
    Public Property date006() As Date
    Public Property date007() As Date
    Public Property date008() As Date
    Public Property date009() As Date

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
    Public Property bit010() As Boolean
    Public Property bit011() As Boolean
    Public Property bit012() As Boolean
    Public Property bit013() As Boolean
    Public Property bit014() As Boolean
    Public Property bit015() As Boolean
    Public Property bit016() As Boolean
    Public Property bit017() As Boolean
    Public Property bit018() As Boolean
    Public Property bit019() As Boolean

    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> _
    Public Property binary000() As Image
    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> _
    Public Property binary001() As Image
    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> _
    Public Property binary002() As Image
    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> _
    Public Property binary003() As Image
    <DevExpress.Xpo.ValueConverter(GetType(Metadata.ImageValueConverter))> _
    Public Property binary004() As Image


    Public Sub New(ByVal Session As Session)
        MyBase.New(Session)
    End Sub

    Sub New()
    End Sub
    Protected Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
        MyBase.New(session, classInfo)
    End Sub

End Class

