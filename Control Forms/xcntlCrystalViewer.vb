Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports DevExpress.XtraEditors

Public Class xcntlCrystalViewer
    Inherits XtraUserControl

    Public Property Report As String
    Public Property ReportParams As String

    Private Sub xcntlCrystalViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        LabelControl1.Text = Report
        'crV.DisplayGroupTree = False

        crV.ReportSource = CreateReport(Report, ReportParams)


    End Sub


End Class
