Imports System.Data.SqlClient
Imports DevExpress.XtraEditors


Module modMsgUP
    Public Function messageUP(ByVal r As DataRow)

        Dim msg As String = ""
        Dim caption As String = ""
        Dim buttons As MessageBoxButtons = 0
        Dim icon As MessageBoxIcon = 0
        Dim def As MessageBoxDefaultButton = 0


        DevExpress.XtraEditors.Controls.Localizer.Active = New clsMyLocalizer

        Try

            Select Case r("f0")
                Case "OK"
                    buttons = 0
                    def = 0

                Case "OKCancel"
                    buttons = 1

                    Select Case r("f4")
                        Case "OK"
                            def = 0
                        Case "Cancel"
                            def = 256
                    End Select

                Case "AbortRetryIgnore"
                    buttons = 2

                    Select Case r("f4")
                        Case "Abort"
                            def = 0
                        Case "Retry"
                            def = 256
                        Case "Ignore"
                            def = 512
                    End Select

                Case "YesNoCancel"
                    buttons = 3

                    Select Case r("f4")
                        Case "Yes"
                            def = 0
                        Case "No"
                            def = 256
                        Case "Cancel"
                            def = 512
                    End Select

                Case "YesNo"
                    buttons = 4

                    Select Case r("f4")
                        Case "Yes"
                            def = 0
                        Case "No"
                            def = 256
                    End Select

                Case "RetryCancel"
                    buttons = 5

                    Select Case r("f4")
                        Case "Retry"
                            def = 0
                        Case "Cancel"
                            def = 256
                    End Select

                Case Else
                    buttons = 0
                    def = 0
            End Select

            Select Case r("f1")
                Case "None"
                    icon = 0
                Case "Error" ', "Hand", "Stop"
                    icon = 16
                Case "Question"
                    icon = 32
                Case "Warning" ', "Exclamation"
                    icon = 48
                Case "Info" ', "Asterisc", "Information"
                    icon = 64
                Case Else
                    icon = 0
            End Select

            msg = r("f2")
            caption = r("f3")


        Catch ex As Exception
            msg = String.Format("{0}{1} Are You Sure ?", ex, vbCrLf)
            caption = "Are You Sure ?"
            buttons = 4
            icon = 32
        End Try

        Dim res As String = "None"
        Dim Result As DialogResult = XtraMessageBox.Show(msg, caption, buttons, icon, def)
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


        Return res

    End Function
End Module

