Imports DevExpress.Skins
Imports DevExpress.XtraEditors
Imports System.IO
Imports System.Data.SqlClient

Imports System.Deployment.Application
Imports System.Net

Imports Microsoft.SqlServer.Management.Smo
Imports Microsoft.SqlServer.Management.Common
Imports System.Threading

Imports System.Text.RegularExpressions

Public Class xfrmOptions

    Dim skinName As String = My.Settings.AppColor

    Private Sub xfrmOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CenterToScreen()

        gcUpdates.Enabled = False

        If My.Settings.Username.ToLower <> "admin" Then

            btnDnlScripts.Enabled = False
            btnDBUpd.Enabled = False
        End If

        'gcUpdateFill()

        'Colors Tab
        listColors.MultiColumn = True

        For Each cnt As SkinContainer In SkinManager.Default.Skins
            listColors.Items.Add(cnt.SkinName)
        Next cnt
        listColors.SelectedItem = My.Settings.AppColor

        ' DB Tab
        'cmbDB.Properties.Items.Add(My.Settings.SystemDatabase)
        cmbDB.Properties.Items.Add(My.Settings.Database)
        cmbDB.SelectedText = My.Settings.Database

        cmbTasks.Properties.Items.Add("DB BackUP")
        cmbTasks.Properties.Items.Add("DB Restore")
        cmbTasks.Properties.Items.Add("DB Execute Script")


        If cmbTasks.SelectedIndex <> 2 Then
            btnScriptChoose.Enabled = False
            MemoEdit1.Enabled = False
        End If

        'Settings tab
        'Me.LabelControl1.Text = ""
        PropertyGrid1.SelectedObject = My.Settings


    End Sub

    'Start Colors Subs
    Private Sub btnSaveColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveColor.Click
        My.Settings.AppColor = skinName
        My.Settings.Save()

    End Sub

    Private Sub listColors_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles listColors.SelectedValueChanged
        Dim lst As ListBoxControl = sender
        skinName = lst.SelectedValue
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = skinName
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        listColors.SelectedValue = My.Settings.AppColor
        Me.Dispose()
    End Sub
    'END Colors Subs

    'START SETTINGS Subs
    Private Sub btnResetSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResetSettings.Click
        My.Settings.Reset()
    End Sub

    Private Sub btnSaveSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveSettings.Click
        My.Settings.Save()
        Me.Dispose()
    End Sub
    'END SETTINGS Subs

    'START DB Subs
    Dim ofd As OpenFileDialog

    Private Sub btnScriptChoose_ButtonClick(ByVal sender As Object, ByVal e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles btnScriptChoose.ButtonClick
        ofd = New OpenFileDialog
        ofd.InitialDirectory = "C:\"
        ofd.ShowDialog()

        btnScriptChoose.Text = ofd.FileName

        Dim file As New FileInfo(btnScriptChoose.Text)
        Dim script As String = file.OpenText().ReadToEnd()
        MemoEdit1.Text = script

    End Sub


    Private Sub btnDBOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBOK.Click
        Dim cn As String = ""

        Select Case cmbTasks.SelectedIndex

            Case 0 'backup

            Case 1 'restore

            Case 2 'execute script
                If btnScriptChoose.Enabled = False Then Exit Sub
                If MemoEdit1.Enabled = False Then Exit Sub

                If cmbDB.SelectedText = My.Settings.Database Then
                    cn = GetCon()

                End If

                RunSP(MemoEdit1.Text)

        End Select
    End Sub

    Private Sub btnDBCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBCancel.Click
        Me.Dispose()
    End Sub
    Private Sub cmbTasks_EditValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbTasks.EditValueChanged
        If sender.SelectedIndex = 2 Then
            btnScriptChoose.Enabled = True
            MemoEdit1.Enabled = True

        Else
            btnScriptChoose.Enabled = False
            MemoEdit1.Enabled = False
        End If
    End Sub
    ' END DB Subs

    'START UPDATE Subs
    Private Sub btnSoftUpd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSoftUpd.Click
        SoftUpdate()
    End Sub

    Private Sub btnDBUpd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBUpd.Click
        If My.Settings.Username.ToLower = "admin" Then

            Try
                GetFtpDirList(App.Constants.UpdateFTP & "/mediaframe_DBScripts/",
                              Application.StartupPath & "\downloads\db\" & GetSoftVersion() & "\",
                              False)

                Dim di As New IO.DirectoryInfo(Application.StartupPath & "\downloads\db\" & GetSoftVersion())
                Dim aryFi As IO.FileInfo() = di.GetFiles("*.sql")
                Dim fi As IO.FileInfo



                For Each fi In aryFi
                    'MsgBox(fi.FullName)
                    ExecuteSql(GetCon, fi.FullName)
                Next

                MsgBox("Finished Updating DataBase")

            Catch ex As Exception
                WriteLogEntry("ERROR Update DataBase: " & ex.Message)
            End Try

        End If
    End Sub


    Private Sub btnDnlScripts_Click(sender As System.Object, e As System.EventArgs) Handles btnDnlScripts.Click
        If My.Settings.Username.ToLower = "admin" Then
            Try
                GetFtpDirList(App.Constants.UpdateFTP & "/mediaframe_GeneralScripts/",
                              Application.StartupPath & "\downloads\general\" & GetSoftVersion() & "\",
                              False)
                MsgBox("Finished Downloading Scripts")
            Catch ex As Exception
                WriteLogEntry("ERROR Download Scripts: " & ex.Message)
            End Try

        End If
    End Sub
    'END UPDATE Subs

    ' Start Diafores Subs
    Public Sub SoftUpdate()
        Try
            Dim ad As ApplicationDeployment
            Dim info As UpdateCheckInfo

            ad = ApplicationDeployment.CurrentDeployment
            info = ad.CheckForDetailedUpdate

            If Not info.UpdateAvailable Then
                Message("msgNoUpd")
                Return
            End If

            Dim doUpdate As Boolean = True

            If info.IsUpdateRequired Then
                Message("msgRequired")
            Else
                If Message("msgOptional") = "No" Then
                    doUpdate = False
                End If
            End If

            If doUpdate = True Then
                App.Objects.myMainForm.Cursor = Cursors.WaitCursor

                ad.Update()

                App.Objects.myMainForm.Cursor = Cursors.Default

                Message("msgUpdFinish")
                Application.Restart()
            End If
        Catch ex As Exception
            WriteLogEntry("Update Error: " & ex.Message)

        End Try

    End Sub

    'Dim host As String = "ftp://www.medialab.gr/httpdocs/mediaframe_DBScripts/" ' prosoxi to underscore
    'Dim username As String = "medialab"
    'Dim password As String = "sti4fr"

    'Dim scripts_host As String = "ftp://62.38.144.96/mf_DBScripts/" ' prosoxi to underscore

    'Dim scripts_host As String = "ftp://62.38.144.96/mf_req/" ' prosoxi to underscore
    'Dim username As String = "anonymous"
    'Dim password As String = ""
    'Dim downloads_dir As String = "C:\mf_downloads\"


    'Dim scripts_host As String = "ftp://www.medialab.gr/httpdocs/mediaframe_req/" ' prosoxi to underscore
    'Dim username As String = "medialab"
    'Dim password As String = "sti4fr"
    'Dim downloads_dir As String = "C:\mf_downloads\"


    Dim ftp_username As String = "medialab"
    Dim ftp_password As String = "sti4fr"


    Public Sub GetFtpDirList(ByVal SourceDir As String, ByVal DestDir As String, Optional ByVal CheckBeforeDownload As Boolean = True)

        ' Get the object used to communicate with the server.
        Dim request As FtpWebRequest = DirectCast(WebRequest.Create(SourceDir), FtpWebRequest)
        request.Method = WebRequestMethods.Ftp.ListDirectory

        ' This example assumes the FTP site uses anonymous logon.
        request.Credentials = New NetworkCredential(ftp_username, ftp_password)

        Dim response As FtpWebResponse = DirectCast(request.GetResponse(), FtpWebResponse)

        Dim responseStream As Stream = response.GetResponseStream()
        Dim reader As New StreamReader(responseStream)

        ' While Not reader.EndOfStream
        While Not reader.EndOfStream
            If CheckBeforeDownload = True Then
                WriteLogEntry("FIX DOWNLOAD PROBLEM")
            ElseIf CheckBeforeDownload = False Then
                DownloadFTPFile(SourceDir, DestDir, reader.ReadLine)
            End If

        End While

        reader.Close()
        response.Close()

    End Sub



    Private Sub DownloadFTPFile(ByVal SourceDir As String, ByVal DestDir As String, ByVal file As String)

        'Values to use

        If Not Directory.Exists(DestDir) Then Directory.CreateDirectory(DestDir)

        Dim locFile As String = DestDir & file
        Dim remoteFile As String = file

        Dim URI As String = SourceDir & remoteFile
        Dim ftp As FtpWebRequest = CType(FtpWebRequest.Create(URI), FtpWebRequest)

        ftp.Credentials = New NetworkCredential(ftp_username, ftp_password)

        ftp.KeepAlive = False
        ftp.UseBinary = True
        ftp.Method = WebRequestMethods.Ftp.DownloadFile

        Using response As FtpWebResponse = CType(ftp.GetResponse, FtpWebResponse)

            Using responseStream As IO.Stream = response.GetResponseStream

                'loop to read & write to file

                Using fs As New IO.FileStream(locFile, IO.FileMode.Create)
                    Dim buffer(2047) As Byte
                    Dim read As Integer = 0

                    Do
                        read = responseStream.Read(buffer, 0, buffer.Length)
                        fs.Write(buffer, 0, read)
                    Loop Until read = 0 'see Note(1)

                    responseStream.Close()
                    fs.Flush()
                    fs.Close()

                End Using
                responseStream.Close()

            End Using
            response.Close()

        End Using


        'gvUpdateAddrow(file)
    End Sub

    Public Sub ExecuteSql(constring As String, sqlFile As String)

        Dim connection As New SqlConnection
        connection.ConnectionString = constring
        connection.Open()

        Dim sql As String = ""

        Using strm As FileStream = File.OpenRead(sqlFile)
            Dim reader As New StreamReader(strm)
            sql = reader.ReadToEnd()
        End Using


        Dim regex As New Regex("^GO", RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Dim lines As String() = regex.Split(sql)

        Dim transaction As SqlTransaction = connection.BeginTransaction()
        Using cmd As SqlCommand = connection.CreateCommand()
            cmd.Connection = connection
            cmd.Transaction = transaction

            For Each line As String In lines
                If line.Length > 0 Then
                    cmd.CommandText = line
                    cmd.CommandType = CommandType.Text

                    Try
                        cmd.ExecuteNonQuery()
                    Catch generatedExceptionName As SqlException
                        transaction.Rollback()
                        Throw
                    End Try
                End If
            Next
        End Using

        transaction.Commit()
        connection.Close()

    End Sub
    '-----------------------------
    'Private Sub CompareFile2DB(ByVal fileName As String)

    '    Dim da As SqlDataAdapter = Nothing

    '    Dim cm As String = String.Format("select ScrName,done from _upd where ScrName='{0}' and done=1", fileName)

    '    If fileName.StartsWith("Main") Then
    '        da = New SqlDataAdapter(cm, GetCon)
    '    ElseIf fileName.StartsWith("Sys") Then
    '        da = New SqlDataAdapter(cm, GetSysCon)
    '    End If

    '    Dim ds As New DataSet

    '    da.Fill(ds)


    '    If ds.Tables(0).Rows.Count - 1 >= 0 Then
    '        Message("msgNoUpd")
    '        Exit Sub
    '    ElseIf ds.Tables(0).Rows.Count - 1 < 0 Then
    '        DownloadFTPFile(fileName)
    '    End If

    '    da.Dispose()
    '    ds.Dispose()

    '    'updatedb <-- prepei na ginei sub pou na kanei to update twn vasewn afou exei katevasei 
    '    'ta arxeia apo to ftp !

    '    UpdateDB()

    'End Sub

    'Private Sub UpdateDB()
    '    For Each File In Directory.GetFiles(Dir)

    '        Dim filename As String = File

    '        Using objReader As New StreamReader(filename)
    '            Dim script As String = objReader.ReadToEnd
    '            objReader.Close()
    '            Dim conn As SqlConnection = Nothing
    '            If File.StartsWith("Main") Then
    '                conn = New SqlConnection(GetCon)
    '            ElseIf File.StartsWith("Sys") Then
    '                conn = New SqlConnection(GetSysCon)
    '            End If
    '            Dim server As New Microsoft.SqlServer.Management.Smo.Server(New ServerConnection(conn))
    '            Try
    '                server.ConnectionContext.ExecuteNonQuery(script)
    '                'LabelControl1.Text += filename & " Executed Succesfully !"
    '            Catch ex As Exception
    '                'LabelControl1.Text += " Error Executing :" & filename & "!"
    '                MsgBox(ex.Message)
    '            End Try
    '        End Using

    '        gvUpdateAddrow(File)

    '    Next
    'End Sub


    'Dim Tbl As New DataTable()
    'Dim Col1 As New DataColumn()
    'Dim Col2 As New DataColumn()
    'Dim Col3 As New DataColumn()

    ''In the general section declare a procedure to format the datatable
    'Private Sub gcUpdateFill()

    '    'Define column 1
    '    'Specify the column data type  
    '    Col1.DataType = System.Type.GetType("System.Int32")
    '    'This name is to identify the column
    '    Col1.ColumnName = "ID"
    '    'This will enable automatically increment the data 
    '    Col1.AutoIncrement = True
    '    'This value will be displayed in the header of the column
    '    Col1.Caption = "ID"
    '    'This will make this column read only,Since it is autoincrement
    '    Col1.ReadOnly = True
    '    'Now add this column to the datatable
    '    Tbl.Columns.Add(Col1)
    '    'Repeat the same procedure         
    '    'Define Column 2
    '    Col2.DataType = System.Type.GetType("System.String")
    '    Col2.ColumnName = "ScriptName"
    '    Col2.Caption = "Script Name"
    '    Col2.ReadOnly = False
    '    Tbl.Columns.Add(Col2)
    '    'Define Column 3
    '    Col3.DataType = System.Type.GetType("System.Boolean")
    '    Col3.ColumnName = "Executed"
    '    Col3.Caption = "Executed"
    '    Col3.ReadOnly = False
    '    Tbl.Columns.Add(Col3)
    '    'BIND THE DATATABLE TO THE DATAGRID
    '    gcUpdates.DataSource = Tbl
    'End Sub
    ''now call this procedure in the load event of the form
    ''FormatGrid()

    '' To add rows dynamically in the click event of the button, 
    '' include the code in the click event of the btnAdd
    ''Create a row type variable
    'Public Sub gvUpdateAddrow(ByVal filename As String)
    '    Dim r As DataRow = Tbl.NewRow()
    '    'Add a new row
    '    'Specify the  col name to add value for the row
    '    r("ScriptName") = filename
    '    r("Executed") = True
    '    'Add the row to the tabledata
    '    Tbl.Rows.Add(r)
    'End Sub


    Private Sub btnUPD_Exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateExit.Click
        Me.Close()
    End Sub


    Private Sub SaveAll(ByVal dir As String)
        Using da As New SqlDataAdapter("SELECT * from Layouts", GetCon)
            Dim ds As New DataSet()
            da.Fill(ds)
            If ds.Tables(0).Rows.Count - 1 < 0 Then Exit Sub
            For i = 0 To ds.Tables(0).Rows.Count - 1
                Dim fullPath = String.Format(dir & "\{0} - {1}.xml", ds.Tables(0).Rows(i)("SenderMaster"), ds.Tables(0).Rows(i)("SenderName"))
                Using ms As New IO.MemoryStream()
                    Dim data() As Byte
                    Try
                        data = CType(ds.Tables(0).Rows(i).Item("Layout"), Byte())
                        ms.Write(data, 0, data.GetUpperBound(0))
                        ms.Seek(0, System.IO.SeekOrigin.Begin)
                        '---
                        Dim pos = ms.Position
                        ms.Position = 0
                        Dim reader As New StreamReader(ms)
                        Dim str = reader.ReadToEnd()
                        ' Reset the position so that subsequent writes are correct.
                        ms.Position = pos
                        '---
                        Dim objReader As StreamWriter
                        objReader = New StreamWriter(fullPath)
                        objReader.Write(str)
                        objReader.Close()
                    Catch ex As Exception
                    End Try
                End Using
            Next
        End Using

    End Sub

    Private Sub btSaveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btSaveAll.Click
        SaveAll(txtDirectory.Text)
        MsgBox("OK")
        Me.Close()

    End Sub

End Class


