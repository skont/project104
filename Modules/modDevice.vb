Imports System.Runtime.InteropServices
Imports System.Data.SqlClient
Imports System.IO

Module modDevice


    '-------------------------------------------------------------------------------
    'DEVICE FILE FORMAT:
    '-------------------------------------------------------------------------------
    '
    '  The FSL_Command() requires a filename specifying a 'device descriptor file'.
    'This file contains the information required for opening the physical connection
    'with the device, and the device's identification information. The format of
    'a device file is one line of plain ascii, with all information serarated by
    'spaces. For example (quotes not included in the actual file):
    '
    '   "ABC02000001 S 1" <CR/LF>
    '
    'Where:
    '   * The first  element is a string with the expected signature device serial
    '     number (ex: ABC02000001).
    '   * The second element is a character specifying the device type, can be
    '     either 'S' (serial) or 'E' (Ethernet), but currently only 'S' is
    '     supported.
    '   * The third element is the port or address number. For serial type of
    '     connections this should be from 1 (for COM1) to N (N being the max COM
    '     number available to the specific system). For ethernet type of devices,
    '     this will be the IP address (currently unsupported).
    '
    '
    '-------------------------------------------------------------------------------
    'FSL_COMMAND PARAMETERS:
    '-------------------------------------------------------------------------------
    '
    ''strBaseDir'    Must be a string pointer to an absolute directory path, in
    '                which the library will create the files, depending on command.
    '
    ''strCommand'    Must be a string (upper or lower case) identifying the command.
    '                Valid command strings are:
    '
    '                * "SIGN" for generating the signature for a file. The sign
    '                  Command requires 'strParam1' to specify a filename for the
    '                  input file and 'strParam2' to specify the output filename
    '                  that will receive the signature stream that is generated.
    '                  Input files *MUST* be in ELOT-928 encoding to satisfy
    '                  specification. On successful completion of the command,
    '                  the output file contains the string to be included at the
    '                  last line of the printed fiscal document.
    '
    '                * "Z" for issuing the daily Z closure report. In this command,
    '                  the 'strParam1' and 'strParam2' may be set to NULL (are
    '                  ignored by the function).
    '
    '                * "CTL" for opening a control dialog box with the device.
    '                  the 'strParam1' and 'strParam2' may be set to NULL (are
    '                  ignored by the function).
    '
    '                * All other command identifiers are invalid producing a
    '                  'ERR_BADARGUMENT' error
    '
    ''strDevFile'    Must be a valid device descriptor file. (See 'device files'
    '                for details)
    '
    ''strParam1'     The 'input file' for signing when command is "SIGN", or NULL
    '                otherwise. Setting this parameter to NULL when required by
    '                a command, produces a 'ERR_BADARGUMENT' error.
    '
    ''strParam2'     The 'output file' that will be generated after successful
    '                completion of the "SIGN" command, or NULL otherwise. Setting
    '                this parameter to NULL when required by a command, produces a
    '                'ERR_BADARGUMENT' error.

    'Private Const iDeviceType As Long = 2
    'Dim WorkingDir As String = eafdssPath
    'Dim DeviceFile As String = App.Constants.DeviceDir & "\Admin\Device.dat"
    'Dim InputFile As String = App.Constants.DeviceDir & "\Admin\OutBeforeSign.txt"
    'Dim OutputFile As String = App.Constants.DeviceDir & "\Admin\OutAfterSign.txt"
    'Dim DllDir As String = App.Constants.DeviceDir & "\Dll\rapidsign.dll"


    <DllImport("\Dll\rapidsign.dll")> _
    Public Function FSL_Command(ByVal strBase As String, ByVal strCmd As String, ByVal strDevFile As String, ByVal strP1 As String, ByVal strP2 As String) As Long
    End Function

    'Public dd As String = App.Constants.DeviceDir & My.Settings.DeviceFile

    Public Sub ControlDevice(ByVal eafdssPath As String, ByVal devicedat As String) '(ByVal WorkingDir As String, ByVal DeviceFile As String)

        If eafdssPath Is Nothing Or eafdssPath = "" Then
            Message("msgEafdss")
            Exit Sub
        End If

        Dim lRet As Long

        Select Case My.Settings.DeviceType
            Case 0 ' EAFDSS_TYPE_UNKNOWN
                Message("msgUD")
                Exit Sub
            Case 1 ' EAFDSS_TYPE_SYNTHEX
                lRet = FSL_Command(eafdssPath, "CTL", devicedat, "", "")
                Exit Sub
        End Select


    End Sub

    Public Sub SignFile(ByVal eafdssPath As String, ByVal devicedat As String) '(ByVal WorkingDir As String, ByVal DeviceFile As String, ByVal InputFile As String, ByVal OutputFile As String) 'As String

        Dim sDevNo As String
        Dim sHost As String = My.Computer.Name
        Dim lRet As Int64


        Select Case My.Settings.DeviceType
            Case 0 ' EAFDSS_TYPE_UNKNOWN
                Message("msgUD")
                Exit Sub
            Case 1 ' EAFDSS_TYPE_SYNTHEX

                Dim FILE_NAME As String = devicedat 'App.Constants.DeviceDir & My.Settings.DeviceFile
                Dim TextLine As String = ""

                Try
                    Dim objReader As New StreamReader(FILE_NAME)

                    Do While objReader.Peek() <> -1
                        TextLine = objReader.ReadLine()
                    Loop

                    sDevNo = Left(TextLine, 11)

                    objReader.Dispose()

                    'Signing
                    If DeviceLocked(sDevNo, sHost) Then
                        If Message("msgDevLocked", sHost) = "Retry" Then
                            SignFile(eafdssPath, devicedat) '(WorkingDir, DeviceFile, InputFile, OutputFile)
                        Else
                            'UnLockIt(sDevNo)
                            Exit Sub
                        End If
                    Else
                        DeviceLock(sDevNo)

                        'Delete previous output files, if any.

                        If File.Exists(Application.StartupPath & My.Settings.OutputFile) Then File.Delete(Application.StartupPath & My.Settings.OutputFile)

                        'lRet = FSL_Command(eafdssPath, "SIGN", App.Constants.DeviceDir & My.Settings.DeviceFile, App.Constants.DeviceDir & My.Settings.InputFile, App.Constants.DeviceDir & My.Settings.OutputFile)
                        lRet = FSL_Command(eafdssPath, "SIGN", devicedat, Application.StartupPath & My.Settings.InputFile, Application.StartupPath & My.Settings.OutputFile)
                        lRet = Nothing
                        DeviceUnLock(sDevNo)
                    End If



                Catch ex As Exception
                    WriteLogEntry("Device Error: " & ex.Message)

                End Try
                'End If
        End Select

    End Sub

    Public Sub IssueZ(ByVal eafdssPath As String, ByVal devicedat As String) '(ByVal WorkingDir As String, ByVal DeviceFile As String)

        If eafdssPath Is Nothing Or eafdssPath = "" Then
            Message("msgEafdss")
            Exit Sub
        End If

        Dim lRet As Int64

        Select Case My.Settings.DeviceType
            Case 0 ' EAFDSS_TYPE_UNKNOWN
                Message("msgUD")
                Exit Sub
            Case 1 'EAFDSS_TYPE_SYNTHEX
                lRet = FSL_Command(eafdssPath, "Z", devicedat, "", "")
                Exit Sub
        End Select


    End Sub

    Private Function DeviceLocked(ByVal myS As String, ByVal theHost As String) As Boolean


        Dim cm As String = String.Format("select * from DLOCKS where serno='{0}' and cn='{1}'", myS, theHost)
        Dim da As New SqlDataAdapter(cm, GetCon)
        Dim ds As New DataSet

        da.Fill(ds)

        If ds.Tables(0).Rows.Count - 1 < 0 Then
            Return False
        Else
            Return True
        End If

        da.Dispose()
        ds.Dispose()


    End Function

    Private Sub DeviceLock(ByVal myS As String)

        Dim da As New SqlDataAdapter("select * from DLOCKS", GetCon)
        Dim ds As New DataSet
        Dim cmb As New SqlCommandBuilder(da)

        da.Fill(ds)

        Dim newRow As DataRow = ds.Tables(0).NewRow

        newRow("serno") = myS
        newRow("cn") = My.Computer.Name

        ds.Tables(0).Rows.Add(newRow)

        da.Update(ds)

        da.Dispose()
        ds.Dispose()


    End Sub

    Private Sub DeviceUnLock(ByVal myS As String)

        Dim cm As String = String.Format("select * from DLOCKS where cn='{0}' and serno='{1}'", My.Computer.Name, myS)
        Dim da As New SqlDataAdapter(cm, GetCon)
        Dim ds As New DataSet
        Dim cmb As New SqlCommandBuilder(da)

        da.Fill(ds)

        If ds.Tables(0).Rows.Count - 1 < 0 Then
            Exit Sub
        Else
            For i = 0 To ds.Tables(0).Rows.Count - 1
                ds.Tables(0).Rows(i).Delete()
            Next
        End If

        da.Update(ds)

        da.Dispose()
        ds.Dispose()


    End Sub

    Public Sub CheckFile() '(ByVal InputFile As String)

        Dim FILE_NAME As String = Application.StartupPath & My.Settings.InputFile
        Dim TextLine As String
        Dim linenumber As Integer = 0
        Dim i As Integer
        Dim X As String

        If File.Exists(FILE_NAME) = True Then

            Using objReader As New StreamReader(FILE_NAME)
                Do While objReader.Peek() <> -1
                    TextLine = objReader.ReadLine()
                    linenumber = linenumber + 1
                    For i = 1 To Len(TextLine)
                        X = Mid(TextLine, i, 1)
                        Select Case Asc(X)
                            Case 127, 129 To 160, 173, 210, 255
                                MsgBox(String.Format("Line {0} Column {1} char {2} {3}", linenumber, i, Asc(X), X))
                        End Select
                    Next
                Loop
            End Using

            MsgBox("File OK")

        Else

            MsgBox("File Does Not Exist")

        End If


    End Sub


End Module
