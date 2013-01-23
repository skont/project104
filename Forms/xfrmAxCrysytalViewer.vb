Imports DevExpress.XtraEditors

Public Class xfrmAxCrysytalViewer
    Inherits XtraForm

    ' ayto einai copy - paste to xcntlAxCrystalViewer epeidi eprepe na allaxei i dhmioyrgia twn formwn 
    ' twn ektypwsewn (na ginetai me ton geniko tropo) alla o m eipe oti afou douleyoun twra na tis afisoume etsi 
    ' opote epeidi eprepe na mporei na kanei preview kai apo ta parastatika pou ftiaxnontai me ton allo tropo ...

    Public Sub New(ByVal _rpt As String, ByVal _prms As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LabelControl1.Text = _rpt
        axcrv.ReportSource = CreateAxReport(_rpt, _prms)
        axcrv.ViewReport()
    End Sub

End Class