Imports System.Data.SqlClient

Public Class clsMyLocalizer
    Inherits DevExpress.XtraEditors.Controls.Localizer
    Public Overrides Function GetLocalizedString(ByVal id As DevExpress.XtraEditors.Controls.StringId) As String

        Dim btnOK = "OK"
        Dim btnCancel = "Cancel"
        Dim btnAbort = "Abort"
        Dim btnRetry = "Retry"
        Dim btnIgnore = "Ignore"
        Dim btnYes = "Yes"
        Dim btnNo = "No"

        Try

            Dim loccm As String = "select * from MessageButtons"
            Dim locda As New SqlDataAdapter(loccm, GetSysCon)
            Dim locds As New DataSet

            locda.Fill(locds)


            btnOK = locds.Tables(0).Rows(0)("OK")
            btnCancel = locds.Tables(0).Rows(0)("Cancel")
            btnAbort = locds.Tables(0).Rows(0)("Abort")
            btnRetry = locds.Tables(0).Rows(0)("Retry")
            btnIgnore = locds.Tables(0).Rows(0)("Ignore")
            btnYes = locds.Tables(0).Rows(0)("Yes")
            btnNo = locds.Tables(0).Rows(0)("No")

            locda.Dispose()
            locds.Dispose()
        Catch

        End Try


        Select Case id
            Case DevExpress.XtraEditors.Controls.StringId.XtraMessageBoxOkButtonText
                Return btnOK
            Case DevExpress.XtraEditors.Controls.StringId.XtraMessageBoxNoButtonText
                Return btnNo
            Case DevExpress.XtraEditors.Controls.StringId.XtraMessageBoxAbortButtonText
                Return btnAbort
            Case DevExpress.XtraEditors.Controls.StringId.XtraMessageBoxCancelButtonText
                Return btnCancel
            Case DevExpress.XtraEditors.Controls.StringId.XtraMessageBoxRetryButtonText
                Return btnRetry
            Case DevExpress.XtraEditors.Controls.StringId.XtraMessageBoxIgnoreButtonText
                Return btnIgnore
            Case DevExpress.XtraEditors.Controls.StringId.XtraMessageBoxYesButtonText
                Return btnYes
        End Select


        'Return btnText
        Return MyBase.GetLocalizedString(id)
    End Function
End Class
