Imports System.Data.SqlClient
Imports System.IO
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors

Public Class xcntlFiles
    'Inherits XtraForm
    Inherits XtraUserControl


    Private Declare Function ShellEx Lib "shell32.dll" Alias "ShellExecuteA" ( _
 ByVal hWnd As Integer, ByVal lpOperation As String, _
 ByVal lpFile As String, ByVal lpParameters As String, _
 ByVal lpDirectory As String, ByVal nShowCmd As Integer) As Integer


    Dim SConnectionString As String = GetCon()
    Dim connection As SqlConnection

    Property FileObject As String


    Private Sub xcntlFiles_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        connection = New SqlConnection(SConnectionString)

        LoadLayoutFromDb(Me.xcntlFilesLC)

        GridView1.OptionsBehavior.Editable = False
        GetImagesFromDatabase()

    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click

        Using ofd As New OpenFileDialog

            ofd.CheckPathExists = True
            ofd.CheckFileExists = True

            ofd.Filter = "All Files (*.*)|*.*|" + _
                "Image Files (*.bmp;*.jpg;*.jpeg;*.GIF)|*.bmp;*.jpg;*.jpeg;*.GIF|" + _
                "PNG files (*.png)|*.png|text files (*.text)|*.txt|doc files (*.doc)|*.doc|" + _
                "pdf files (*.pdf)|*.pdf"


            ofd.Multiselect = False
            ofd.AddExtension = True
            ofd.ValidateNames = True
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)


            If (ofd.ShowDialog = DialogResult.OK) Then
                txtFileToUpload.Text = ofd.FileName

            Else 'Cancel
                Exit Sub
            End If
        End Using

    End Sub

    Private Sub btnUpload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpload.Click
        'If LTrim(RTrim(txtFileToUpload.Text)) = "" Then
        If txtFileToUpload.Text = "" Then
            txtFileToUpload.Focus()
            Exit Sub
        End If

        'Call Upload Images Or File
        Dim sFileToUpload As String = ""

        sFileToUpload = LTrim(RTrim(txtFileToUpload.Text))
        Dim Extension As String = System.IO.Path.GetExtension(sFileToUpload)

        upLoadImageOrFile(sFileToUpload, Extension, txtComments.Text)

        GetImagesFromDatabase()
    End Sub

    Private Sub upLoadImageOrFile(ByVal sFilePath As String, ByVal sFileType As String, ByVal sFileComments As String)
        Dim SqlCom As SqlCommand
        Dim imageData As Byte()
        Dim sFileName As String
        Dim qry As String

        Try
            'Read Image Bytes into a byte array 

            'Initialize SQL Server Connection 
            If connection.State = ConnectionState.Closed Then
                connection.Open()
            End If

            imageData = ReadFile(sFilePath)
            sFileName = System.IO.Path.GetFileName(sFilePath)


            'Set insert query 
            qry = "insert into FileStore (FileObject,FileName,FileType,FileData,FileComments,[Added On]) " & _
            "values(@FileObject,@FileName,@FileType,@FileData,@FileComments,@AddedOn)"

            'Initialize SqlCommand object for insert. 
            SqlCom = New SqlCommand(qry, connection)

            'We are passing File Name and Image byte data as sql parameters. 

            SqlCom.Parameters.Add(New SqlParameter("@FileObject", Me.FileObject))
            SqlCom.Parameters.Add(New SqlParameter("@FileName", sFileName))
            SqlCom.Parameters.Add(New SqlParameter("@FileType", sFileType))
            SqlCom.Parameters.Add(New SqlParameter("@FileData", DirectCast(imageData, Object)))
            SqlCom.Parameters.Add(New SqlParameter("@FileComments", sFileComments))
            SqlCom.Parameters.Add(New SqlParameter("@AddedOn", Now()))

            SqlCom.ExecuteNonQuery()


            Me.txtFileToUpload.Text = ""
            Me.txtComments.Text = ""

        Catch ex As Exception
            MessageBox.Show("File could not uploaded" & ex.ToString())
        End Try

    End Sub

    Private Function GetOpenFileDialog() As OpenFileDialog
        Dim openFileDialog As New OpenFileDialog

        openFileDialog.CheckPathExists = True
        openFileDialog.CheckFileExists = True

        openFileDialog.Filter = "All Files (*.*)|*.*|" + _
            "Image Files (*.bmp;*.jpg;*.jpeg;*.GIF)|*.bmp;*.jpg;*.jpeg;*.GIF|" + _
            "PNG files (*.png)|*.png|text files (*.text)|*.txt|doc files (*.doc)|*.doc|" + _
            "pdf files (*.pdf)|*.pdf"


        openFileDialog.Multiselect = False
        openFileDialog.AddExtension = True
        openFileDialog.ValidateNames = True
        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)

        Return openFileDialog
    End Function

    'Open file in to a filestream and read data in a byte array. 
    Private Function ReadFile(ByVal sPath As String) As Byte()
        'Initialize byte array with a null value initially. 
        Dim data As Byte() = Nothing

        'Use FileInfo object to get file size. 
        Dim fInfo As New FileInfo(sPath)
        Dim numBytes As Long = fInfo.Length

        'Open FileStream to read file 
        Dim fStream As New FileStream(sPath, FileMode.Open, FileAccess.Read)

        'Use BinaryReader to read file stream into byte array. 
        Dim br As New BinaryReader(fStream)

        'When you use BinaryReader, you need to supply number of bytes to read from file. 
        'In this case we want to read entire file. So supplying total number of bytes. 
        data = br.ReadBytes(CInt(numBytes))

        Return data
    End Function

    'Get table rows from sql server to be displayed in Datagrid. 
    Private Sub GetImagesFromDatabase()
        Try

            'Initialize SQL Server Connection 
            If connection.State = ConnectionState.Closed Then
                connection.Open()
            End If

            Dim strSql As String = String.Format("Select FileId,FileName,FileType,FileComments,[Added On] from FileStore where FileObject='{0}'", FileObject)

            'Initialize SQL adapter. 
            Dim ADAP As New SqlDataAdapter(strSql, connection)

            'Initialize Dataset. 
            Dim DS As New DataSet()

            'Fill dataset with FileStore table. 
            ADAP.Fill(DS, "FileStore")

            'Fill Grid with dataset. 
            GridControl1.DataSource = DS.Tables("FileStore")


        Catch ex As Exception
            MessageBox.Show(ex.ToString())
            MessageBox.Show("Could not load the Image")
        End Try
    End Sub

    Private Sub downLoadFile(ByVal iFileId As Long, ByVal sFileName As String, ByVal sFileExtension As String)

        Dim strSql As String
        'For Document
        Try
            'Get image data from gridview column. 
            strSql = "Select FileData from FileStore WHERE FileId=" & iFileId

            Dim sqlCmd As New SqlCommand(strSql, connection)

            'Get image data from DB
            Dim fileData As Byte() = DirectCast(sqlCmd.ExecuteScalar(), Byte())

            Dim sTempFileName As String = Application.StartupPath & "\" & sFileName

            If Not fileData Is Nothing Then
                'Read image data into a file stream 
                Using fs As New FileStream(sFileName, FileMode.OpenOrCreate, FileAccess.Write)
                    fs.Write(fileData, 0, fileData.Length)
                    'Set image variable value using memory stream. 
                    fs.Flush()
                    fs.Close()
                End Using

                'Open File
                ' 10 = SW_SHOWDEFAULT

                ShellEx(Me.Handle, "Open", sFileName, "", "", 10)
            End If

        Catch ex As Exception
            WriteLogEntry("Read File From DB: " & ex.Message)
        End Try

    End Sub

    Private Sub GridControl1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridControl1.DoubleClick

        Dim view As GridView = GridView1

        Try
            downLoadFile(view.GetFocusedRowCellValue("FileId"), view.GetFocusedRowCellValue("FileName"), view.GetFocusedRowCellValue("FileType"))
        Catch ex As Exception

        End Try
    End Sub

    Private Sub GridView1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles GridView1.KeyDown
        Dim v As GridView = sender

        Dim id As Integer = v.GetFocusedRowCellValue("FileId")

        If e.KeyData = Keys.Delete AndAlso sender.State = GridState.Normal Then
            v.DeleteSelectedRows()

            Try
                'Read Image Bytes into a byte array 

                'Initialize SQL Server Connection 
                If connection.State = ConnectionState.Closed Then
                    connection.Open()
                End If

                Dim qry As String = String.Format("delete from FileStore where FileId='{0}'", id)
                'Initialize SqlCommand object for insert. 
                Dim SqlCom As SqlCommand = New SqlCommand(qry, connection)

                SqlCom.ExecuteNonQuery()

            Catch ex As Exception
            End Try

            GetImagesFromDatabase()
        End If
    End Sub


End Class
