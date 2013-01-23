Imports DevExpress.XtraEditors

Public Class xcntlAxCrystalViewer
    Inherits XtraUserControl


    Public Sub New(ByVal _rpt As String, ByVal _prms As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        LabelControl1.Text = _rpt
        axcrv.ReportSource = CreateAxReport(_rpt, _prms)
        axcrv.ViewReport()


    End Sub

End Class