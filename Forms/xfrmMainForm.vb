Imports System.Data.SqlClient

Public Class xfrmMainForm

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        '---------

        'Ayto endexomenws allazei ola ta fonts tou programmatos
        'thelei psaximo 

        'DevExpress.Utils.AppearanceObject.DefaultFont = New System.Drawing.Font("Calibri", 14)
        'XtraTabbedMdiManager1.AppearancePage.Header.Font = New Font("", 18)

        OpenHideInitReport()



        With App.Objects
            .myMainForm = Me
            .myMainNavBar = Me.NavBarControl1
            .myDockManager = Me.DockManager1
            .myBarManager = Me.BarManager1
            .myStyleController = Me.StyleController1
            .myTabManager = Me.XtraTabbedMdiManager1
            .myBarManager.AllowCustomization = False
        End With

        ' Other Options 
        App.Objects.myBarManager.UseAltKeyForMenu = True

        Dim mainlcTag As New clsTagExtender() With {.SenderMaster = Name}


        Icon = My.Resources.mlX
        Text = App.Constants.MainFormCaption 'My.Settings.Caption & " | " & My.Settings.Company & " - " & My.Settings.Database

        LabelControl1.Text = My.Settings.Caption
        If My.Settings.Caption = "<none>" Then
            PanelControl1.Hide()
        End If


        CreateNavMenuFromDb()
        CreateTopMenuFromDb()



        WindowState = FormWindowState.Maximized
        KeyPreview = True



    End Sub

    Private Sub xfrmMainForm_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            Select Case Message("msgQuit")
                Case "Yes"
                    FullAppExit()
                Case "No"
                    e.Cancel = True
            End Select
        End If
    End Sub

    Private Sub xfrmMainForm_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Dim keycomb = Nothing

        If e.Modifiers.ToString = "Control" AndAlso e.KeyCode <> Keys.ControlKey Then
            keycomb = String.Format("{0} + {1}", e.Modifiers, e.KeyCode)
            ' keycomb="^" & e.KeyCode.ToString
        ElseIf e.Modifiers.ToString = "Alt" AndAlso e.KeyCode <> Keys.Menu Then
            keycomb = String.Format("{0} + {1}", e.Modifiers, e.KeyCode)
            ' keycomb="!" & e.KeyCode.ToString
        ElseIf e.Modifiers.ToString = "Shift" AndAlso e.KeyCode <> Keys.ShiftKey Then
            keycomb = String.Format("{0} + {1}", e.Modifiers, e.KeyCode)
            ' keycomb="+" & e.KeyCode.ToString
        ElseIf e.Modifiers.ToString = "None" Then
            keycomb = e.KeyCode.ToString
        End If

        ' THELEI PSAXIMO NA DW TI THA KANW KAI TI THA STELNW
        'If Not keycomb Is Nothing Then MsgBox(keycomb)
        'Try
        '    MsgBox(CurrentMainControl.Name & " " & CurrentFocusedControl.name & " " & keycomb)
        'Catch ex As Exception

        'End Try


        If e.KeyCode = Keys.F5 Then
            Try
                ctr_Refresh(App.Objects.curMainControl, App.Objects.curFocusedControl.Name)
            Catch ex As Exception
            End Try
        ElseIf e.KeyCode = Keys.F12 Then
            App.Objects.myBarManager.Customize()
        End If
    End Sub

    Private Sub xfrmMainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Provide Localized Assemblies
        System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(My.Settings.Culture)
        System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(My.Settings.UICulture)

        If My.Settings.RunMode.ToLower = "mobile" Then
            PanelControl1.Visible = False
            NavBarPanel.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide

        End If
    End Sub



    Private Sub XtraTabbedMdiManager1_SelectedPageChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles XtraTabbedMdiManager1.SelectedPageChanged
        Try

            App.Objects.curMainControl = XtraTabbedMdiManager1.SelectedPage.MdiChild


        Catch ex As Exception

        End Try

    End Sub


End Class