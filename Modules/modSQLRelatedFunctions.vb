Imports System.Data.SqlClient
Imports System.IO

Module modSQLRelatedFunctions

    Function GetCon() As String

        Dim Connection As String = String.Format("server={0};database={1};uid={2};Password={3}", My.Settings.Server, My.Settings.Database, My.Settings.Username, App.UserInfo.PassWord) 'mlPass)
        Return Connection

    End Function

    Function GetSysCon() As String

        'Dim SystemdbCon As String = String.Format("server={0};database={1};uid={2};Password={3}", My.Settings.Server, My.Settings.SystemDatabase, My.Settings.Username, App.UserInfo.PassWord) 'mlPass)

        Dim SystemdbCon As String = String.Format("server={0};database={1};uid={2};Password={3}", My.Settings.Server, My.Settings.Database, My.Settings.Username, App.UserInfo.PassWord) 'mlPass)
        Return SystemdbCon

    End Function


    Function GetXPOCon() As String

        Dim XPOConnection As String = String.Format("data source={0};user id={1};Password={2};initial catalog={3}", My.Settings.Server, My.Settings.Username, App.UserInfo.PassWord, My.Settings.Database)
        Return XPOConnection

    End Function

    Function GetStatusBarText() As String

        Dim str As String = String.Format("Login Info: SERVER={0};UserNAME={1};DATABASE={2}  Login Time={3}", My.Settings.Server, My.Settings.Username, My.Settings.Database, Date.Now)
        Return str

    End Function

    Function GetReportPath(ByVal table As String) As String
        Dim Path As String = ""

        Dim com As String = String.Format("SELECT CRYSTALPATH from [ΕΚΤΥΠΩΣΕΙΣ ΛΙΣΤΑ] where ΠΙΝΑΚΑΣ='{0}'", table)
        Dim ds As New DataSet
        Dim ad As New SqlDataAdapter(com, GetCon)

        ad.Fill(ds)

        Path = ds.Tables(0).Rows(0)(0)

        Return Path

        ds.Dispose()
        ad.Dispose()
    End Function

    Function GetAppIniReportPath() As String
        Dim Path As String = ""

        Dim com As String = "select reportpath from [appini]"
        Dim ds As New DataSet
        Dim ad As New SqlDataAdapter(com, GetCon)

        ad.Fill(ds)

        Path = ds.Tables(0).Rows(0)(0)

        Return Path

        ds.Dispose()
        ad.Dispose()
    End Function

    Function spExec(ByVal spName As String, ByVal inputValues As String) As Object

        Dim inValues() As String = Split(inputValues, ";")

        Using sqlcmd As New SqlCommand()
            Dim SQLCon As New SqlClient.SqlConnection() With {.ConnectionString = GetCon()}
            sqlcmd.CommandText = spName
            ' Stored Procedure to Call
            sqlcmd.CommandType = CommandType.StoredProcedure
            'Setup Command Type
            sqlcmd.Connection = SQLCon
            'Active Connection
            SQLCon.Open()
            SqlCommandBuilder.DeriveParameters(sqlcmd)
            Dim i As Integer = 0
            For a = 1 To sqlcmd.Parameters.Count - 1 ' Paraleipoume to 0 giati h 0 variable einai mia default @ReturnValue

                If sqlcmd.Parameters.Item(a).Direction = ParameterDirection.Input Then
                    Select Case sqlcmd.Parameters.Item(a).DbType.ToString
                        Case "String"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = CStr(inValues(a - 1))
                        Case "Single", "Int16", "Int32", "Int64"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = CInt(Val(inValues(a - 1)))
                        Case "Double"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = CDbl(Val(inValues(a - 1)))
                        Case "DateTime"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = CDate(inValues(a - 1))
                        Case "Boolean"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = CBool(inValues(a - 1))
                    End Select
                ElseIf sqlcmd.Parameters.Item(a).Direction = ParameterDirection.Output Or sqlcmd.Parameters.Item(a).Direction = ParameterDirection.InputOutput Then
                    Select Case sqlcmd.Parameters.Item(a).DbType.ToString
                        Case "String"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = CStr("")
                        Case "Single", "Double", "Int16", "Int32", "Int64"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = 0
                        Case "DateTime"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = DBNull.Value 'CDate("")
                        Case "Boolean"
                            sqlcmd.Parameters(sqlcmd.Parameters.Item(a).ParameterName).Value = False
                    End Select
                    i = i + 1
                End If
            Next
            sqlcmd.ExecuteNonQuery()
            Dim outValues(i) As Object
            Dim l As Integer = 0
            For k = 0 To sqlcmd.Parameters.Count - 1
                If sqlcmd.Parameters(k).Direction = ParameterDirection.InputOutput Then
                    outValues(l) = sqlcmd.Parameters(k).Value
                    l = l + 1
                End If
            Next
            SQLCon.Close()
            Return outValues
        End Using

    End Function



    Public Function GetImageFromDB(ByVal imgname As String)

        Dim img As Image = Nothing

        Dim connection As SqlConnection = New SqlConnection(GetCon())

        Dim strSql As String
        'For Document
        Try
            'Initialize SQL Server Connection 
            If connection.State = ConnectionState.Closed Then
                connection.Open()
            End If

            'Get image data from gridview column. 
            strSql = String.Format("exec upGetPicture '{0}'", imgname)

            Dim sqlCmd As New SqlCommand(strSql, connection)

            'Get image data from DB
            Dim fileData As [Byte]() = New [Byte](-1) {}
            fileData = DirectCast(sqlCmd.ExecuteScalar(), [Byte]())

            If Not fileData Is Nothing Then

                Dim ms As New MemoryStream(fileData)
                img = Image.FromStream(ms)

            End If

        Catch ex As Exception
            WriteLogEntry("Read File From DB: " & ex.Message)
            img = Nothing
        Finally

            connection.Close()
        End Try



        Return img

    End Function


    Public Function GetImageFromDB_(ByVal imgname As String) As Image

        Dim img As Image = Nothing

        Dim com As String = String.Format("exec upGetPicture '{0}'", imgname)
        Dim ds As New DataSet
        Dim ad As New SqlDataAdapter(com, GetCon)

        ad.Fill(ds)

        ad.Dispose()
        ds.Dispose()

        Dim dr As DataRow

        If ds.Tables(0).Rows.Count > 0 Then
            dr = ds.Tables(0).Rows(0)
        Else
            Return img
        End If

        Dim data() As Byte
        Dim ms As New IO.MemoryStream

        Try

            If Not (dr("Image") Is DBNull.Value) Then

                'data = CType(dt.Rows(0).Item(Field), Byte())

                data = CType(dr("Image"), Byte())
                ms.Write(data, 0, data.GetUpperBound(0))
                ms.Seek(0, System.IO.SeekOrigin.Begin)

                If ms.Length > 1 Then
                    img = Image.FromStream(ms)
                End If


            End If

        Catch ex As Exception
            'handle any errors other than no picture set in db yet

            WriteLogEntry("Image Read: " & ex.Message)

            img = Nothing
        Finally
            ms.Close()
            ms = Nothing

        End Try



        Return img

    End Function


    ' DEN XERW An tha douleyei giati exoume allaxei ta panta kai den to exoume dokimasei !

    Function GetSelectCommand(ByVal SenderName As String, ByVal ParamArray Code() As String) As String

        Dim com As String = String.Format("SELECT obj_SelectCommand from Objects where obj_Name='{0}'", SenderName)
        Dim ds As New DataSet
        Dim ad As New SqlDataAdapter(com, GetSysCon)

        Dim SelCom As String

        ad.Fill(ds)

        SelCom = String.Format(ds.Tables(0).Rows(0)("obj_SelectCommand"), Code)

        Return SelCom

        ds.Dispose()
        ad.Dispose()

    End Function




End Module

