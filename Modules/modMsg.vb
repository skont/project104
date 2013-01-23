Imports System.Data.SqlClient
Imports DevExpress.XtraEditors


Module modMsg
    Function Message(ByVal msgName As String, ByVal ParamArray Code() As String)

        Dim msg As String
        Dim caption As String
        Dim buttons As MessageBoxButtons
        Dim icon As MessageBoxIcon

        Dim cm As String = String.Format("select * from Messages where Name='{0}'", msgName)
        Dim da As New SqlDataAdapter(cm, GetSysCon)
        Dim ds As New DataSet

        DevExpress.XtraEditors.Controls.Localizer.Active = New clsMyLocalizer

        Try

            da.Fill(ds)

            msg = String.Format(ds.Tables(0).Rows(0)("Message"), Code)
            caption = ds.Tables(0).Rows(0)("Caption")
            buttons = 0
            icon = 0

            Select Case ds.Tables(0).Rows(0)("Buttons")
                Case "OK"
                    buttons = 0
                Case "OKCancel"
                    buttons = 1
                Case "AbortRetryIgnore"
                    buttons = 2
                Case "YesNoCancel"
                    buttons = 3
                Case "YesNo"
                    buttons = 4
                Case "RetryCancel"
                    buttons = 5
                Case Else
                    buttons = 0
            End Select

            Select Case ds.Tables(0).Rows(0)("Icon")
                Case "None"
                    icon = 0
                Case "Error", "Hand", "Stop"
                    icon = 16
                Case "Question"
                    icon = 32
                Case "Exclamation", "Warning"
                    icon = 48
                Case "Asterisc", "Information"
                    icon = 64
                Case Else
                    icon = 0
            End Select


        Catch ex As Exception
            msg = "Are You Sure ?"
            caption = "Are You Sure ?"
            buttons = 4
            icon = 32
        End Try

        Dim res As String = "None"
        Dim Result As DialogResult = XtraMessageBox.Show(msg, caption, buttons, icon)
        'Displays the MessageBox

        If Result = DialogResult.None Then
            res = "None"
        ElseIf Result = DialogResult.OK Then
            res = "OK"
        ElseIf Result = DialogResult.Cancel Then
            res = "Cancel"
        ElseIf Result = DialogResult.Abort Then
            res = "Abort"
        ElseIf Result = DialogResult.Retry Then
            res = "Retry"
        ElseIf Result = DialogResult.Ignore Then
            res = "Ignore"
        ElseIf Result = DialogResult.Yes Then
            res = "Yes"
        ElseIf Result = DialogResult.No Then
            res = "No"
        End If

        da.Dispose()
        ds.Dispose()


        Return res

    End Function
End Module
