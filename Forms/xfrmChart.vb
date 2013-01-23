Imports DevExpress.XtraPivotGrid
Imports DevExpress.XtraCharts
Imports DevExpress.XtraEditors

Public Class xfrmChart

    Public Sub New(ByVal pv As PivotGridControl)
        InitializeComponent()

        SearchLookUpEdit1.Properties.DataSource = System.Enum.GetValues(GetType(ViewType))

        SearchLookUpEdit1.SelectedText = "Line"


        Me.Size = New Size(800, 600)
        Me.Text = "Γράφημα"
        ' na ta kanw checkboxes stin forma 

        pv.OptionsChartDataSource.ProvideDataByColumns = True

        'pv.OptionsChartDataSource.ChartDataVertical = True ' POLY VASIKO OPTION 'einai obsolete 
        pv.OptionsChartDataSource.SelectionOnly = True ' POLY VASIKO OPTION
        'pv.OptionsChartDataSource.ShowColumnGrandTotals = False
        'pv.OptionsChartDataSource.ShowRowGrandTotals = False

        ' As is !!
        chart.DataSource = pv
        chart.SeriesDataMember = "Series"
        chart.SeriesTemplate.ArgumentDataMember = "Arguments"
        chart.SeriesTemplate.ValueDataMembers.AddRange(New String() {"Values"})
        chart.SeriesTemplate.LegendPointOptions.PointView = PointView.ArgumentAndValues

    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Dispose()
    End Sub

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        PreviewPrint(chart)
    End Sub

    Private Sub SearchLookUpEdit1_EditValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchLookUpEdit1.EditValueChanged
        Dim sl As SearchLookUpEdit = TryCast(sender, SearchLookUpEdit)

        Try
            chart.SeriesTemplate.ChangeView(sl.EditValue)
        Catch ex As Exception
            WriteLogEntry("Search LookUp EditValueChanged")
            sl.EditValue = sl.OldEditValue
        End Try

    End Sub
End Class