Imports DevExpress.XtraTab
Imports DevExpress.XtraNavBar
Imports DevExpress.XtraGrid
Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraBars.Docking
Imports DevExpress.XtraBars
Imports DevExpress.XtraEditors
Imports DevExpress.XtraTabbedMdi
Imports DevExpress.XtraBars.Controls
Imports DevExpress.Skins
Imports DevExpress.LookAndFeel
Imports System.Data.SqlClient

Public Class clsApp
    Public UserInfo As appUserInfo
    Public UserProperties As appUserProperties
    Public AppInfo As applInfo
    Public AppProperties As applProperties
    Public Constants As appConstants
    Public Objects As appObjects
    Public Collections As appCollections

    Public Sub New(ByVal Visual As String)



        UserInfo = New appUserInfo
        UserProperties = New appUserProperties

        Constants = New appConstants


        With Constants
            .applicationName = "MediaFrame"
            .MainFormCaption = ""

            .RegisteredTo = ""
            .UpdateFTP = ""
            .UpdateURL = ""

            .RenameField = "RenameField"
            .SaveToXML = "SaveLayout (to xml)"
            .LoadFromXML = "LoadLayout (from xml)"
            .SaveToDB = "SaveLayout (to DB)"
            .LoadFromDb = "LoadLayout (from DB)"
            .PrintPreview = "PrintPreview"
            .FormatField = "FormatField"
            .ClearFormat = "Clear FormatField"
            .InsertRow = "Insert Row Above"
            .RefreshRow = "Refresh Row"
            .RunningSums = "Toggle Running Totals"
            .ToggleRowGrandTotals = "Toggle Row Grand Totals"
            .ToggleColumnGrandTotals = "Toggle Column Grand Totals"
            .ToggleFieldTotals = "Toggle Field Totals "

            .txtSrv = "Server"
            .txtDB = "DataBase"
            .txtSysDB = "System DataBase"
            .txtUsrName = "UserName"
            .txtPswd = "Password"
            .txtSaveCred = "Save Credentials"

        End With


        AppInfo = New applInfo
        AppProperties = New applProperties

        Objects = New appObjects
        Collections = New appCollections


        Select Case Visual.ToLower
            Case "remote", "mobile"
                'Me.RemoteVisual()
                Me.InitFastOptions(Nothing)
            Case Else
                Me.FullVisual()
        End Select

    End Sub

    Public Class appUserInfo

        Public Property UserName As String
        Public Property PassWord As String = ""

    End Class

    Public Class appUserProperties
        Public Property CanSaveLayout As Boolean

    End Class
    Public Class applInfo

    End Class

    Public Class applProperties

    End Class
    Public Class appConstants

        Public Property applicationName As String
        Public Property MainFormCaption As String

        Public Property RegisteredTo As String
        Public Property UpdateFTP As String
        Public Property UpdateURL As String

        Public Property RenameField As String
        Public Property SaveToXML As String
        Public Property LoadFromXML As String
        Public Property SaveToDB As String
        Public Property LoadFromDb As String
        Public Property PrintPreview As String
        Public Property FormatField As String
        Public Property ClearFormat As String
        Public Property InsertRow As String
        Public Property RefreshRow As String
        Public Property RunningSums As String
        Public Property ToggleRowGrandTotals As String
        Public Property ToggleColumnGrandTotals As String
        Public Property ToggleFieldTotals As String


        Public Property txtSrv As String
        Public Property txtDB As String
        Public Property txtSysDB As String
        Public Property txtUsrName As String
        Public Property txtPswd As String
        Public Property txtSaveCred As String

    End Class

    Public Class appObjects

        Public Property prevFocusedControl As Object
        Public Property curFocusedControl As Object

        Public Property myMainForm As XtraForm ' einai i main forma

        Public Property myMainNavBar As NavBarControl
        Public Property myDockManager As DockManager
        Public Property myBarManager As BarManager
        Public Property myStyleController As StyleController

        Public Property myTabManager As XtraTabbedMdiManager
        Public Property curMainControl As xcntlMainControl


        Public Property curFocusedGridControl As GridControl
        Public Property curFocusedView = Nothing


        Public Property errProvider As DXErrorProvider

    End Class

    Public Class appCollections
        Public Property MainControl As New Hashtable

    End Class


    Private Sub FullVisual()
        DevExpress.UserSkins.BonusSkins.Register()
        DevExpress.UserSkins.OfficeSkins.Register()

        DevExpress.Skins.SkinManager.EnableFormSkins()
        DevExpress.LookAndFeel.LookAndFeelHelper.ForceDefaultLookAndFeelChanged()
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = My.Settings.AppColor

    End Sub

    Private Sub RemoteVisual()
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetWindowsXPStyle()
    End Sub


    Private Sub InitFastOptions(ByVal manager As BarManager)
        FastOptions()
        BarFastOptions(manager)
    End Sub
    Private Sub FastOptions()
        Animator.AllowFadeAnimation = False
        SkinManager.DisableFormSkins()
        SkinManager.DisableMdiFormSkins()
        UserLookAndFeel.Default.SetStyle3D()
        BarAndDockingController.Default.PropertiesBar.MenuAnimationType = AnimationType.None
        BarAndDockingController.Default.PropertiesBar.SubmenuHasShadow = False
        BarAndDockingController.Default.PropertiesBar.AllowLinkLighting = False
        Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled
    End Sub

    Private Sub BarFastOptions(ByVal manager As BarManager)
        If manager IsNot Nothing Then
            manager.AllowItemAnimatedHighlighting = False
        End If
    End Sub
End Class
