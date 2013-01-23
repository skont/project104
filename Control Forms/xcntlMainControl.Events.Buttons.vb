Imports DevExpress.XtraEditors
Imports DevExpress.XtraLayout
Imports DevExpress.XtraPivotGrid
Imports System.ComponentModel


Partial Public Class xcntlMainControl
    Inherits XtraForm


    Private WithEvents backgroundWorker1 As System.ComponentModel.BackgroundWorker


    Private Sub btn_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim b As SimpleButton = TryCast(sender, SimpleButton)

        If b.Name.ToLower = "btnchart" Then

            ' NA to bgalw apo edw.
            Dim pv As PivotGridControl = Me.Controls.Find("gv1", True)(0)
            If Not pv Is Nothing Then
                Dim cf As xfrmChart = New xfrmChart(pv)
                cf.Show()
            End If

        Else
            Dim param As String = "clk"

            If CBool(Control.ModifierKeys And Keys.Shift) Then
                param = "+" & param

            ElseIf CBool(Control.ModifierKeys And Keys.Alt) Then
                param = "!" & param

            ElseIf CBool(Control.ModifierKeys And Keys.Control) Then
                param = "^" & param

            End If
            DoActions(Me.myGuid, b.Name, param)
        End If

    End Sub

End Class
