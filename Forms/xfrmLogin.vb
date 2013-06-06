Imports System.IO
Imports System.Net
Imports System.Data.SqlClient

Public Class xfrmLogin

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        App = New clsApp(My.Settings.RunMode)


    End Sub

    Private Sub xfrmLogin_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        CenterToScreen()
        Text = My.Settings.Caption & "(RunMode: " & My.Settings.RunMode & ")"
        Icon = My.Resources.mlX

        KeyPreview = True

        SimpleButton1.Text = "OK"
        SimpleButton2.Text = "Cancel"
        Me.AcceptButton = SimpleButton1
        Me.CancelButton = SimpleButton2

        txtPassword.Properties.PasswordChar = "*"


        chkSave.Text = ""
        chkSave.Checked = My.Settings.LoginInfo




        For Each sl In My.Settings.ServerList
            If Not sl Is Nothing Then
                cmbServer.Properties.Items.Add(sl)
            End If
        Next

        For Each dbl In My.Settings.DatabaseList
            If Not dbl Is Nothing Then
                cmbDatabase.Properties.Items.Add(dbl)
            End If
        Next

        For Each ul In My.Settings.UsernameList
            If Not ul Is Nothing Then
                cmbUsername.Properties.Items.Add(ul)
            End If
        Next

        LayoutControlItem1.ControlAlignment = ContentAlignment.MiddleRight
        LayoutControlItem10.ControlAlignment = ContentAlignment.MiddleRight

        With App.Constants
            LayoutControlItem2.Text = .txtSrv
            LayoutControlItem3.Text = .txtDB
            LayoutControlItem5.Text = .txtUsrName
            LayoutControlItem6.Text = .txtPswd
            chkSave.Text = .txtSaveCred


            LabelControl1.Text = My.Settings.Caption

        End With

        LabelControl2.Text = GetSoftVersion()

        If My.Settings.Caption = "<none>" Then
            LayoutControlItem1.ContentVisible = False
            LayoutControlItem10.ContentVisible = False
            Text = "(RunMode: " & My.Settings.RunMode & ")"

        End If

        cmbServer.EditValue = My.Settings.Server
        cmbDatabase.EditValue = My.Settings.Database
        cmbUsername.EditValue = My.Settings.Username

        LayoutControlItem6.Control.Select()
        LayoutControlItem6.Control.Focus()

        Me.cmbDatabase.ToolTip = "Database Name"
        Me.cmbServer.ToolTip = "Server Name"
        Me.cmbUsername.ToolTip = "User Name"
        Me.txtPassword.ToolTip = "Password"

    End Sub

    Private Sub xfrmLogin_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        If e.KeyCode = Keys.Enter Then login()


        If e.Modifiers.ToString = "Control" AndAlso e.KeyCode <> Keys.ControlKey Then
            Dim keycomb = String.Format("{0} + {1}", e.Modifiers, e.KeyCode)
            'If keycomb = "Control + G" Then
            'If MsgBox("Go to config ?", MsgBoxStyle.YesNo, "Configuration") = MsgBoxResult.Yes Then
            '    Dim f As String = Application.StartupPath & "\MediaFrame.exe.config"

            '    If IO.File.Exists(f) Then
            '        Try
            '            Process.Start("notepad++.exe", f)
            '        Catch ex As Exception
            '            WriteLogEntry("notepad++ not installed")
            '            Process.Start("notepad.exe", f)
            '        End Try
            '    End If

            '    Application.Exit()
            'End If
            'End If
            If keycomb = "Control + D" Then
                If MsgBox("RunMode is: " & My.Settings.RunMode & " Change It to Default ?", MsgBoxStyle.YesNo, "RunMode Configuration") = MsgBoxResult.Yes Then
                    My.Settings.RunMode = "Default"
                    My.Settings.Save()
                    Application.Restart()
                End If
            ElseIf keycomb = "Control + R" Then
                If MsgBox("RunMode is: " & My.Settings.RunMode & " Change It to Remote ?", MsgBoxStyle.YesNo, "RunMode Configuration") = MsgBoxResult.Yes Then
                    My.Settings.RunMode = "Remote"
                    My.Settings.Save()
                    Application.Restart()
                End If
            ElseIf keycomb = "Control + M" Then
                If MsgBox("RunMode is: " & My.Settings.RunMode & " Change It to Mobile ?", MsgBoxStyle.YesNo, "RunMode Configuration") = MsgBoxResult.Yes Then
                    My.Settings.RunMode = "Mobile"
                    My.Settings.Save()
                    Application.Restart()
                End If
            End If

        End If


    End Sub

    Public Function TestConnection() As Boolean
        Try
            Using dbcon As New SqlClient.SqlConnection(GetCon)
                dbcon.Open()
                dbcon.Close()


                'Using sysdbcon As New SqlClient.SqlConnection(GetSysCon)
                '    sysdbcon.Open()
                '    sysdbcon.Close()
                'End Using

                ChangeInitValues(GetCon)
            End Using
                Return True

        Catch ex As Exception
            WriteLogEntry("SQL Connection: " & ex.Message)
            txtPassword.Text = ""

            Return False
        End Try
    End Function

    Private Sub ChangeInitValues(ByVal connectionstring As String)
        Dim initds As New DataSet
        Dim initda As New SqlDataAdapter("exec up1Init", connectionstring)
        initda.Fill(initds, "InitialValues")
        initda.Dispose()
        initds.Dispose()

        With App.Constants
            .MainFormCaption = initds.Tables("InitialValues").Rows(0)("f0")

            .RegisteredTo = initds.Tables("InitialValues").Rows(0)("f1")
            .UpdateFTP = initds.Tables("InitialValues").Rows(0)("f2")
            .UpdateURL = initds.Tables("InitialValues").Rows(0)("f3")
            .FocusedForeColor = initds.Tables("InitialValues").Rows(0)("f4")
            .FocusedBackColor = initds.Tables("InitialValues").Rows(0)("f5")
        End With

    End Sub

    Private Sub login()

        App.UserInfo.UserName = My.Settings.Username
        App.UserInfo.PassWord = txtPassword.EditValue
        App.UserProperties.CanSaveLayout = False


        If File.Exists(String.Format("{0}\{1}", Application.StartupPath, My.Settings.LogFile)) Then
            File.Delete(String.Format("{0}\{1}", Application.StartupPath, My.Settings.LogFile))
        End If

        If My.Settings.ServerList.Contains(cmbServer.EditValue) Then

        Else
            My.Settings.ServerList.Add(cmbServer.EditValue)
            cmbServer.Properties.Items.Add(cmbServer.EditValue)
        End If

        If My.Settings.DatabaseList.Contains(cmbDatabase.EditValue) Then

        Else
            My.Settings.DatabaseList.Add(cmbDatabase.EditValue)
            cmbDatabase.Properties.Items.Add(cmbDatabase.EditValue)
        End If

        If My.Settings.UsernameList.Contains(cmbUsername.EditValue) Then

        Else
            My.Settings.UsernameList.Add(cmbUsername.EditValue)
            cmbUsername.Properties.Items.Add(cmbUsername.EditValue)
        End If

        My.Settings.Server = cmbServer.EditValue
        My.Settings.Database = cmbDatabase.EditValue
        My.Settings.Username = cmbUsername.EditValue

        My.Settings.LoginInfo = chkSave.Checked

        If TestConnection() = True Then

            Try
                RunSP("exec up1Login")
            Catch ex As Exception
                WriteLogEntry("Login Error: " & ex.Message)
            End Try

            DialogResult = Windows.Forms.DialogResult.OK

            Hide()


            If My.Settings.RegisterDlls = False Then
                RegisterDlls()
                'InstallPrerequisites()
                My.Settings.RegisterDlls = True
            End If

            Dim main As xfrmMainForm
            main = New xfrmMainForm
            main.Show()

        End If

        If chkSave.Checked = True Then
            My.Settings.Save()
        End If


    End Sub


    Private Sub SimpleButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SimpleButton1.Click

        login()
    End Sub

    Private Sub SimpleButton2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SimpleButton2.Click
        Select Case Message("msgLoginCancel")
            Case "Yes"
                Application.Exit()
            Case "No"
                Exit Sub

        End Select

    End Sub



End Class
