Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.BandedGrid
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid
Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraLayout

Module modNewSubsDoActions

    Private Function ContinueExecution(ByVal resp_from_pu As String, ByVal resp_from_msg As String, ByVal resp_to_continue As String) As Boolean

        Dim result As Boolean = False


        'Dim tmpresult = Split(resp_to_continue, ";")
        'For j As Integer = 0 To UBound(tmpresult)
        '    If resp_from_msg = tmpresult(j) Then result = True
        'Next

        If resp_to_continue.Contains(resp_from_msg) Or resp_to_continue.Contains(resp_from_pu) Then
            result = True

        End If
        Return result

    End Function

    Dim response_from_pu As String = "notset"

    Public Sub DoActions(ByVal frmGuid As Guid,
                         ByVal ControlName As String,
                         ByVal evnt As String,
                         Optional ByVal gv As GridView = Nothing,
                         Optional ByVal rEVA As ValidateRowEventArgs = Nothing,
                         Optional ByVal gEVA As BaseContainerValidateEditorEventArgs = Nothing,
                         Optional ByVal cEVA As System.ComponentModel.CancelEventArgs = Nothing)


        '*******************************************************************
        'Ayto egine gia na mpoun ta click twn menu sto kyklwma ths DoActions
        'arxika itan mono to kommati me to else kai to frmguid itan guid
        '*******************************************************************
        Dim tbl As DataTable
        Dim frm As xcntlMainControl = Nothing

        If frmGuid.ToString = "00000000-0000-0000-0000-999999999999" Then
            tbl = UP1(Nothing, "mnu;" & ControlName, evnt, "set", Nothing)
            frmGuid = Nothing
            If tbl.Rows.Count - 1 < 0 Then Exit Sub

        ElseIf frmGuid.ToString = "00000000-0000-0000-0000-111111111111" Then
            tbl = UP1(Nothing, "nvb;" & ControlName, evnt, "set", Nothing)
            frmGuid = Nothing
            If tbl.Rows.Count - 1 < 0 Then Exit Sub

            'ElseIf frmGuid.ToString = "00000000-0000-0000-0000-222222222222" Then
            '    tbl = UP1(Nothing, "main", evnt, "set", Nothing)
            '    frmGuid = Nothing
            '    If tbl.Rows.Count - 1 < 0 Then Exit Sub

        Else
            frm = App.Collections.MainControl(frmGuid)
            If frm Is Nothing Then Return
            tbl = UP1(frm, ControlName, evnt, "set", StoreValues(frm, ControlName, evnt))
            If tbl.Rows.Count - 1 < 0 Then Exit Sub
        End If

        '*******************************************************************


        If tbl.Rows.Count - 1 < 0 Then Exit Sub

        App.Objects.myMainForm.Cursor = Cursors.WaitCursor

        Dim response_from_msg As String = "*" 'arxikh timh
        Dim response_to_continue As String


        For i = 0 To tbl.Rows.Count - 1
            'auto to try ...catch to evala
            'gia na piasw tin periptwsi pou 
            'den erthei to pedio res 
            Try
                response_to_continue = tbl.Rows(i)("res")
                If response_to_continue = "" Then response_to_continue = "*"
            Catch ex As Exception
                response_to_continue = "*"
            End Try


            ' WriteLogEntry(response_to_continue & " " & response_from_msg & " " & response_from_pu)

            'If response_from_pu <> "notset" Then
            '    response_from_msg = response_from_pu
            '    'response_from_pu = "notset"

            'End If


            If ContinueExecution(response_from_pu, response_from_msg, response_to_continue) = True Then
                'If response = tmpRes OrElse tmpRes = "*" Then
                Select Case tbl.Rows(i)("act").tolower
                    Case "openfrm"
                        ' Anoigei Forma
                        response_from_pu = "notset"
                        Openfrm(frmGuid, tbl.Rows(i))

                    Case "exit"
                        ' Kleinei Current TabPage
                        If String.IsNullOrEmpty(tbl.Rows(i)("f1").ToString) Then
                            CloseCurrentForm(frmGuid)

                        ElseIf tbl.Rows(i)("f1").tolower = "pop" Then
                            Dim f As xcntlMainControl = App.Collections.MainControl(frmGuid)
                            App.Objects.curMainControl = TryCast(App.Collections.MainControl(f.myParentGuid), xcntlMainControl)
                            f.Dispose()

                        ElseIf tbl.Rows(i)("f1").tolower = "modal" Then
                            Dim f As xcntlMainControl = App.Collections.MainControl(frmGuid)
                            App.Objects.curMainControl = TryCast(App.Collections.MainControl(f.myParentGuid), xcntlMainControl)
                            If Not (tbl.Rows(i)("f2") Is DBNull.Value) Then
                                response_from_pu = tbl.Rows(i)("f2").tolower
                            End If

                            WriteLogEntry("Response from PU: " & response_from_pu)
                            f.DialogResult = DialogResult.OK


                            'f.Dispose()


                        End If
                        ' CloseCurrentForm(frmGuid)


                    Case "app_exit"
                        ' Kleinei tin Efarmogh
                        FullAppExit()

                    Case "save"
                        ' Save Record kai kanei Refresh (Olous tous pinakes)
                        If (tbl.Rows(i)("f0") Is DBNull.Value) OrElse (tbl.Rows(i)("f0") Is Nothing) Then
                            UpdateDataset(0)
                            RefreshDataset(0)
                        Else
                            ' Save Record kai kanei Refresh 1 pinaka (f0)
                            Dim tID As Integer = Convert.ToInt16(tbl.Rows(i)("f0"))
                            UpdateDataset(tID)
                            RefreshDataset(tID)
                        End If


                        For Each lc In frm.Controls
                            If TypeOf lc Is LayoutControl Then
                                For Each it In lc.Items
                                    If TypeOf it Is LayoutControlItem Then
                                        If TypeOf it.Control Is GridControl Then
                                            it.control.mainview.collapsealldetails()
                                        End If
                                    End If
                                Next
                            End If
                        Next



                    Case "refresh"
                        '' Kanei Refresh olous tous pinakes
                        'If (tbl.Rows(i)("f0") Is DBNull.Value) OrElse (tbl.Rows(i)("f0") Is Nothing) Then
                        '    RejectDataset(0)
                        'Else
                        '    ' Kanei Refresh 1 (f0) pinaka
                        '    Dim tID As Integer = Convert.ToInt16(tbl.Rows(i)("f0"))
                        '    RejectDataset(tID)
                        'End If

                        ' Kanei Refresh olous tous pinakes
                        If (tbl.Rows(i)("f0") Is DBNull.Value) OrElse (tbl.Rows(i)("f0") Is Nothing) Then
                            RefreshDataset(0)
                        Else
                            ' Kanei Refresh 1 (f0) pinaka
                            Dim tID As Integer = Convert.ToInt16(tbl.Rows(i)("f0"))
                            RefreshDataset(tID)
                        End If

                    Case "upd"
                        ' Bazei tis times sta pedia

                        Dim f As xcntlMainControl


                        Dim popup As Boolean = False
                        If tbl.Rows(i)("f0").startswith("prev.") Then
                            Dim f0 = Replace(tbl.Rows(i)("f0"), "prev.", "")
                            tbl.Rows(i)("f0") = f0
                            f = App.Collections.MainControl(App.Objects.curMainControl.myParentGuid)
                            popup = True
                        Else
                            f = frm
                        End If
                        'If f.myViewMode.ToLower = "edit" Then Set_Update(f, tbl.Rows(i), popup)

                        'Ayto xreiazetai gia na kanei update sta pedia pou einai unbound !
                        Set_Update(f, tbl.Rows(i), popup)


                    Case "msg"
                        ' bgazei mynima
                        response_from_msg = messageUP(tbl.Rows(i))


                    Case "runsp"
                        ' Ektelei tin StoredProcedure pou tou leei sto f0
                        RunSP(tbl.Rows(i)("f0"))

                    Case "paips"
                        ' kanei simansi
                        SignInvoice(tbl.Rows(i))

                    Case "issuez"
                        ' vgazei Z
                        IssueZ(tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "chkdev"
                        ' kanei check ti syskeyi
                        ControlDevice(tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "opensbg"
                        ' Ayto Kanei Expand to Master Record
                        If gv Is Nothing Then
                            Exit Sub
                        ElseIf (TypeOf gv Is GridView) OrElse (TypeOf gv Is AdvBandedGridView) Then
                            gv.UpdateCurrentRow()
                            gv.ExpandMasterRow(gv.FocusedRowHandle, Convert.ToInt16(tbl.Rows(i)("f1")))

                        End If

                        'Case "closesbg"
                        '    If gv Is Nothing Then
                        '        Exit Sub
                        '    ElseIf (TypeOf gv Is GridView) OrElse (TypeOf gv Is AdvBandedGridView) Then
                        '        gv.CollapseAllDetails()
                        '    End If

                    Case "add_newrow"
                        ' Vazei nea grammi
                        Dim f As xcntlMainControl
                        If gv Is Nothing Then

                            Dim popup As Boolean = False
                            If tbl.Rows(i)("f3").startswith("prev.") Then
                                Dim f3 = Replace(tbl.Rows(i)("f3"), "prev.", "")
                                tbl.Rows(i)("f3") = f3
                                f = App.Collections.MainControl(App.Objects.curMainControl.myParentGuid)
                                popup = True
                            Else
                                f = frm
                            End If
                            gv = FindControl(f, tbl.Rows(i)("f3"))


                        End If
                        AddRemoveRows(tbl.Rows(i), gv)
                        'gv.updatecurrentrow()

                    Case "remove_all"
                        If gv Is Nothing Then
                            Dim f As xcntlMainControl
                            Dim popup As Boolean = False
                            If tbl.Rows(i)("f3").startswith("prev.") Then
                                Dim f3 = Replace(tbl.Rows(i)("f3"), "prev.", "")
                                tbl.Rows(i)("f3") = f3
                                f = App.Collections.MainControl(App.Objects.curMainControl.myParentGuid)
                                popup = True
                            Else
                                f = frm
                            End If
                            gv = FindControl(f, tbl.Rows(i)("f3"))
                        End If

                        'Dim tID As Integer = Convert.ToInt16(tbl.Rows(i)("f0"))
                        AddRemoveRows(tbl.Rows(i), gv)

                        'RefreshDataset(9)
                        'UpdateDataset(tID)

                    Case "remove_currentrow"
                        AddRemoveRows(tbl.Rows(i), gv)

                    Case "cancel_currentrow"
                        gv.CancelUpdateCurrentRow()

                    Case "update_currentrow"
                        gv.UpdateCurrentRow()

                    Case "changedatasource"
                        'Allazei to datasource se grid
                        ChangeDsGridView(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f2"))

                    Case "prnrpt"
                        ' Profanws typwnei ena report
                        CrystalReportPrint(tbl.Rows(i))

                    Case "focusvalue"
                        FindAnd(frm, tbl.Rows(i), "value")

                    Case "focusrow"
                        FindAnd(frm, tbl.Rows(i), "row")

                    Case "setfocus"
                        SetFocus(frm, tbl.Rows(i)("f0"))

                    Case "cell_error_pu"
                        gEVA.Valid = CBool(tbl.Rows(i)("f3"))

                    Case "cell_error"
                        gEVA.Valid = CBool(tbl.Rows(i)("f3"))
                        gEVA.ErrorText = tbl.Rows(i)("f1")


                    Case "cell_valid"
                        gEVA.Valid = True

                    Case "cell_value"
                        gEVA.Value = tbl.Rows(i)("f0")


                    Case "row_error"
                        Dim ctrName = tbl.Rows(i)("f0")
                        Dim errortext = tbl.Rows(i)("f1")
                        Dim errortype = tbl.Rows(i)("f2")
                        Dim validation As Boolean = CBool(tbl.Rows(i)("f3"))

                        Dim w = Split(ctrName, ".")

                        rEVA.Valid = validation
                        Select Case errortype
                            Case "critical"
                                gv.SetColumnError(gv.Columns(w(2)), errortext, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical)
                            Case "info"
                                gv.SetColumnError(gv.Columns(w(2)), errortext, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Information)
                            Case "warning"
                                gv.SetColumnError(gv.Columns(w(2)), errortext, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning)

                        End Select


                    Case "row_valid"
                        rEVA.Valid = True

                    Case "editor_error"

                        Dim o As Object = frm.Controls.Find(tbl.Rows(i)("f0"), True)(0)
                        Try
                            cEVA.Cancel = IIf(tbl.Rows(i)("f3") = 0, True, False)
                        Catch ex As Exception
                        End Try

                        Select Case tbl.Rows(i)("f2").tolower
                            Case "critical"
                                App.Objects.errProvider.SetError(o, tbl.Rows(i)("f1"), ErrorType.Critical)

                            Case "warning"
                                App.Objects.errProvider.SetError(o, tbl.Rows(i)("f1"), ErrorType.Warning)

                            Case "info"
                                App.Objects.errProvider.SetError(o, tbl.Rows(i)("f1"), ErrorType.Information)
                            Case Else

                        End Select

                    Case "deleterow"
                        gv.DeleteSelectedRows()

                        ' -------- 
                    Case "gridvisible"
                        GridVisible(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "gridreadonly"
                        GridReadOnly(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "controlvisible"
                        ControlVisible(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "controlreadonly"
                        ControlReadOnly(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "buttontoggle"
                        ButtonToggle(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "gridnewrowpossition"
                        GridNewRowPossition(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "gridcolumnreadonly"
                        GridColumnReadOnly(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "gridcolumnvisible"
                        GridColumnVisible(frm, tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "closeformevent"
                        If tbl.Rows(i)("f0").tolower = "cancel" Then frm.myCancelCloseForm = True

                        '---------

                    Case "changecaption"
                        App.Objects.myMainForm.Text = tbl.Rows(i)("f0")

                    Case "dotmatrixprint"
                        DotMatrixPrint(tbl.Rows(i)("f0"), tbl.Rows(i)("f1"))

                    Case "sendkeys"
                        SendKeys.Send(tbl.Rows(i)("f0"))
                    Case "sendkeyswait"
                        SendKeys.SendWait(tbl.Rows(i)("f0"))

                End Select

            End If

        Next


        App.Objects.myMainForm.Cursor = Cursors.Default


    End Sub

    'einai event ousiastika 
    Public Function GetActionsResult(ByVal frmGuid As Guid, ByVal ControlName As String, ByVal evnt As String, Optional ByVal gv As GridView = Nothing)
        Dim result = Nothing

        Dim frm As xcntlMainControl = App.Collections.MainControl(frmGuid)

        Dim tbl As DataTable = UP1(frm, ControlName, evnt, "set", StoreValues(frm, ControlName, evnt))

        If tbl.Rows.Count - 1 < 0 Then Return result

        For i = 0 To tbl.Rows.Count - 1
            'auto to try ...catch to evala
            'gia na piasw tin periptwsi pou 
            'den erthei to pedio res 

            Dim tmpRes As String = ""
            Try
                tmpRes = tbl.Rows(i)("res")
            Catch ex As Exception
                tmpRes = ""
            End Try

            If "" = tmpRes Then
                Select Case tbl.Rows(i)("act").tolower
                    Case "changedatasource"
                        result = tbl.Rows(i)
                    Case "noaction"
                        result = tbl.Rows(i)
                        'Case "closeformevent"
                        '    result = tbl.Rows(i)("res")
                End Select

            End If

        Next

        Return result

    End Function


    Public Sub DoRefreshRow(ByVal gv As GridView)

        Dim g As GridControl = TryCast(gv.GridControl, GridControl)
        Dim fCol As GridColumn = Nothing

        If gv.FocusedColumn Is Nothing Then
            fCol = gv.VisibleColumns(0)
        Else
            fCol = gv.FocusedColumn
        End If

        'Dim controlname As String = String.Format("{0}.{1}.{2}", g.Name, gv.Name, gv.FocusedColumn.FieldName)
        'WriteLogEntry(controlname)

        gv.ShowEditor()
        gv.ActiveEditor.IsModified = True

        gv.ValidateEditor()
        'gv.ActiveEditor.Hide()
        gv.FocusedRowHandle = gv.FocusedRowHandle
        gv.FocusedColumn = fCol

        'gv.ShowEditor()
    End Sub

    Public Sub DoInsertRow(ByVal gv As GridView)

        Dim pos As Integer = gv.FocusedRowHandle
        If pos < 0 Then Exit Sub



        Dim prevLinenumber As Integer = gv.GetRowCellValue(pos - 1, "LineNumber")
        Dim nextLinenumber As Integer = gv.GetRowCellValue(pos, "LineNumber")
        Dim newLinenumber As Integer = (prevLinenumber + nextLinenumber) / 2



        gv.AddNewRow()
        Dim col = gv.Columns.ColumnByFieldName("LineNumber")
        If Not col Is Nothing Then
            gv.SetFocusedRowCellValue(col, newLinenumber)
        End If


        gv.FocusedRowHandle = pos
        gv.FocusedColumn = gv.VisibleColumns(0)

        gv.ShowEditor()

        SendKeys.Send("{DOWN}")

        gv.FocusedRowHandle = pos - 1
        gv.FocusedColumn = gv.VisibleColumns(0)
        gv.ShowEditor()



    End Sub

End Module
