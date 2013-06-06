Imports DevExpress.XtraEditors
Imports System.Data.SqlClient
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors.Repository
Imports System.Globalization

Partial Public Class xcntlMainControl
    Inherits XtraForm

    Private CurrentValue As Object = Nothing

    Private Sub ctr_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)

        App.Objects.errProvider.ClearErrors()

        If (sender.editvalue Is DBNull.Value) OrElse (sender.oldeditvalue Is DBNull.Value) OrElse (sender.OldEditValue = sender.EditValue) Then
            Exit Sub
        Else
            DoActions(Me.myGuid, sender.name, "EditVal", cEVA:=e)
        End If


    End Sub


    Private Sub ctr_Validated(ByVal sender As Object, ByVal e As System.EventArgs)

        '' den einai poly swsto 
        '' to evala gia na antimetwpisw to fainomeno 
        '' otan oi hmerominies einai null

        'If (sender.oldeditvalue Is DBNull.Value) Then Return
        'If (sender.editvalue Is DBNull.Value) Then Return

        ''WriteLogEntry("Name:" & sender.name & "-EditValue: " & sender.editvalue & "-Old: " & sender.oldeditvalue & "-Current: " & CurrentValue.ToString)

        '' vgazei ena error edw 
        '' ayto symvainei giati me to focus allazei i timi !!!! tou current value 
        'Try
        '    If TypeOf sender Is GridLookUpEdit Then
        '        CurrentValue = sender.valuemember
        '    Else
        '        CurrentValue = sender.editvalue
        '    End If

        'Catch

        'End Try

        'If (sender.OldEditValue = sender.EditValue) AndAlso (sender.EditValue = CurrentValue) Then Return

        If (sender.editvalue Is DBNull.Value) OrElse (sender.oldeditvalue Is DBNull.Value) OrElse (sender.OldEditValue = sender.EditValue) Then
            Exit Sub
        Else
            DoActions(Me.myGuid, sender.name, "AfterEditVal")
        End If

    End Sub


    Private Sub ctr_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)

        App.Objects.prevFocusedControl = App.Objects.curFocusedControl
        App.Objects.curFocusedControl = sender
        App.Objects.curMainControl = Me
        Try
            If TypeOf sender Is GridLookUpEdit Then
                CurrentValue = sender.valuemember
            Else
                CurrentValue = sender.editvalue
            End If

        Catch

        End Try


        DoActions(Me.myGuid, sender.name, "GotFocus")

    End Sub

    Private Sub ctr_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim param As String = "clk"
        ' just a comment for git
        If CBool(Control.ModifierKeys And Keys.Shift) Then
            param = "+" & param

        ElseIf CBool(Control.ModifierKeys And Keys.Alt) Then
            param = "!" & param

        ElseIf CBool(Control.ModifierKeys And Keys.Control) Then
            param = "^" & param
        End If
        DoActions(Me.myGuid, sender.name, param)
    End Sub

    Private Sub glk_QueryPopUp(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)

        Dim edit = TryCast(sender, GridLookUpEdit)
        ChangeDs(edit)
        LoadLayoutFromDb(sender.properties.View)

    End Sub

    Private Sub lke_QueryPopUp(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        Dim edit = TryCast(sender, LookUpEdit)
        ChangeDs(edit)

    End Sub

    Private Sub cmb_QueryPopUp(sender As Object, e As System.ComponentModel.CancelEventArgs)
        Dim edit = TryCast(sender, ComboBoxEdit)
        ChangeDs(edit)
    End Sub


    Private Sub ChangeDs(ByVal ed As Object)
        If ed.Name <> "" Then
            ' an den kanw lathos ayto to xreiazetai an einai gridlookup kanoniko 
            '(sti forma kai oxi se gridview)

            Try
                Dim r As DataRow = GetActionsResult(Me.myGuid, ed.Name, "ChangeDatasource")
                If r("act").tolower = "noaction" Then Exit Sub
                Dim da As New SqlDataAdapter(r("f2"), GetCon)
                Dim ds As New DataSet

                da.Fill(ds)
                da.Dispose()
                ds.Dispose()

                If TypeOf ed Is ComboBoxEdit Then
                    ed.properties.items.clear()
                    For t = 0 To ds.Tables(0).Rows.Count - 1
                        ed.Properties.Items.Add(ds.Tables(0).Rows(t)(r("f3")))
                    Next
                Else
                    ed.Properties.DataSource = ds.Tables(0)
                    ed.Properties.DisplayMember = r("f3")
                    ed.Properties.ValueMember = r("f4")
                End If


            Catch ex As Exception
                WriteLogEntry("Error Changing datasource: " & ed.name & ex.Message)
            End Try
        End If
    End Sub


    Private Sub ctr_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)

        If e.KeyCode = Keys.F8 OrElse e.KeyCode = Keys.F9 OrElse e.KeyCode = Keys.F10 OrElse e.KeyCode = Keys.F11 OrElse e.KeyCode = Keys.F12 Then
            DoActions(Me.myGuid, sender.name, e.KeyCode.ToString)
        End If

        If e.KeyData = Keys.Delete Then
            If (TypeOf sender Is LookUpEdit) OrElse (TypeOf sender Is GridLookUpEdit) Then
                sender.EditValue = ""
            End If
        End If

    End Sub

    Private Sub ctr_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs)

        DoActions(Me.myGuid, sender.name, "dblclick")

    End Sub


    Private Sub reptext_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        Dim MyCultureInfo As CultureInfo = New CultureInfo(My.Settings.Culture)
        Dim MyDateTime As DateTime = DateTime.Parse(sender.EditValue, MyCultureInfo)

        sender.editvalue = MyDateTime.ToShortDateString()

    End Sub


End Class
