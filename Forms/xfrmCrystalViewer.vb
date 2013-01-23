Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports DevExpress.XtraEditors

Public Class xfrmCrystalViewer
    Inherits XtraForm

    ' ayto einai copy - paste to xcntlCrystalViewer epeidi eprepe na allaxei i dhmioyrgia twn formwn 
    ' twn ektypwsewn (na ginetai me ton geniko tropo) alla o m eipe oti afou douleyoun twra na tis afisoume etsi 
    ' opote epeidi eprepe na mporei na kanei preview kai apo ta parastatika pou ftiaxnontai me ton allo tropo ...
    ' oti katalaves (24/2/11)

    Public Property Report As String
    Public Property ReportParams As String

    Private Sub xcntlCrystalViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        LabelControl1.Text = Report
        'crV.DisplayGroupTree = False

        crV.ReportSource = CreateReport(Report, ReportParams)


    End Sub


End Class
