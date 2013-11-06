Imports System.Data.SqlClient
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraGrid
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraVerticalGrid.VGridControl
Imports DevExpress.XtraVerticalGrid
Imports DevExpress


Partial Public Class xcntlMainControl
    Inherits XtraForm

    '--------- GRIDCONTROL EVENTS


    Private Sub gc_GotFocus(sender As Object, e As EventArgs)

        App.Objects.prevFocusedControl = App.Objects.curFocusedControl
        App.Objects.curFocusedGridControl = TryCast(sender, GridControl)
        App.Objects.curFocusedView = sender.focusedview

        DoActions(myGuid, sender.name, "GotFocus")

    End Sub

    Private Sub gc_ViewRegistered(ByVal sender As Object, ByVal e As ViewOperationEventArgs)

        If e.View.IsDetailView AndAlso e.View.Name <> "" Then
            GetViewsDetails(e.View)
            'AddRepItems(e.View)
        End If
    End Sub

    Private Sub gc_ProcessGridKey(ByVal sender As Object, ByVal e As KeyEventArgs)

        Dim grid As GridControl = CType(sender, GridControl)
        Dim gv As GridView = grid.FocusedView

        If (gv.IsEditing = True AndAlso (e.KeyCode = Keys.F8)) OrElse (gv.IsEditing = True AndAlso (e.KeyCode = Keys.F9)) OrElse (gv.IsEditing = True AndAlso (e.KeyCode = Keys.F10)) OrElse (gv.IsEditing = True AndAlso (e.KeyCode = Keys.F11)) Then

            DoActions(myGuid, String.Format("{0}.{1}.{2}", grid.Name, gv.Name, gv.FocusedColumn.FieldName), e.KeyCode.ToString, gv:=gv)
        End If

        If gv.IsEditing = True AndAlso (e.KeyCode = Keys.Escape) Then

            gv.HideEditor()
            'e.Handled = True
        End If
    End Sub

    Private Sub gc_FocusedViewChanged(ByVal sender As Object, ByVal e As ViewFocusEventArgs)

        Try

            If (Not sender.FocusedView.ParentView Is Nothing) Then

                Dim parent As GridView = CType(sender.FocusedView.ParentView, GridView)
                parent.FocusedRowHandle = sender.FocusedView.SourceRowHandle

            End If

            App.Objects.curFocusedGridControl = TryCast(sender, GridControl)
            App.Objects.curFocusedView = sender.focusedview

        Catch ex As Exception

            WriteLogEntry(ex.Message)
        End Try

    End Sub


    '--------- GRIDVIEW EVENTS

    

    Private Sub gv_CustomRowCellEdit(ByVal sender As Object, ByVal e As CustomRowCellEditEventArgs)

        Dim view As GridView = sender

        Const Value_Column As String = "key"

        If view.Name <> "gv4" Then Return
        'If Me.Name <> "Utils" Then Return
        If Name.ToLower.Contains("utils") = False Then Return

        If e.Column.FieldName <> "val" Then Return


        If e.RowHandle = GridControl.NewItemRowHandle Then Return
        Dim Value_Column_Value As String = view.GetRowCellValue(e.RowHandle, view.Columns(Value_Column)).ToString()

        e.RepositoryItem = GetRepItem(Value_Column_Value, e.CellValue)


    End Sub

    'Private Sub gv_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)

    '    App.Objects.curFocusedGridControl = sender.gridcontrol
    '    App.Objects.curFocusedView = sender
    'End Sub

    Private Sub gv_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)

        Dim view As GridView = TryCast(sender, GridView)
        Dim info As GridHitInfo = view.CalcHitInfo(e.Location)
        If info.InColumn AndAlso e.Clicks = 1 AndAlso e.Button = MouseButtons.Left Then
            view.OptionsCustomization.AllowSort = False
        End If

        ' Kalo gia na diaxeiristeis ta click ektos apo ta events !!!
        'If info.InColumn AndAlso e.Clicks = 2 AndAlso e.Button = MouseButtons.Left Then
        '    view.OptionsCustomization.AllowSort = True
        'End If

        If info.InColumn AndAlso e.Button = MouseButtons.Right Then
            view.OptionsCustomization.AllowSort = True
        End If
    End Sub

    Private Sub vgc_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)

        'Dim view As GridView = TryCast(sender, GridView)
        'Dim info As GridHitInfo = view.CalcHitInfo(e.Location)
        'If info.InColumn AndAlso e.Clicks = 1 AndAlso e.Button = MouseButtons.Left Then
        '    view.OptionsCustomization.AllowSort = False
        'End If

        ' Kalo gia na diaxeiristeis ta click ektos apo ta events !!!
        'If info.InColumn AndAlso e.Clicks = 2 AndAlso e.Button = MouseButtons.Left Then
        '    view.OptionsCustomization.AllowSort = True
        'End If

        'If info.InColumn AndAlso e.Button = MouseButtons.Right Then
        '    view.OptionsCustomization.AllowSort = True
        'End If


        Dim vgc As VGridControl = TryCast(sender, VGridControl)
        If e.Button = MouseButtons.Right Then
            Dim hInfo As VGridHitInfo = vgc.CalcHitInfo(e.Location)
            vgc.ContextMenuStrip = Create_VGridcontrolContextMenu(vgc, vgc.Tag.stripMenuName, Me.myViewMode)
        End If

    End Sub
    Private Function Create_VGridcontrolContextMenu(ByVal vgc As VGridControl, ByVal MenuName As String, ByVal ViewMode As String)

        Dim cm As New ContextMenuStrip
        Dim dt As DataTable = CreateContextMenuStripTable(MenuName:=MenuName, ViewMode:=ViewMode)
        For i As Integer = 0 To dt.Rows.Count - 1

            Dim Item As New ToolStripMenuItem
            Item.Text = dt.Rows(i)("Caption")
            Item.Name = dt.Rows(i)("Parameter")
            Item.Enabled = CBool(dt.Rows(i)("enabled"))
            Item.Tag = vgc
            'Item.ShortcutKeys = Keys.Alt Or Keys.A
            cm.Items.Add(Item)

            AddHandler Item.Click, AddressOf vgridviewContextMenu_Item_Click
        Next i

        'Dim cm As New ContextMenuStrip
        'Dim it As New ToolStripMenuItem
        'it.Text = "test"
        'it.Tag = vgc
        'AddHandler it.Click, AddressOf It_Click
        'cm.Items.Add(it)

        Return cm
    End Function
    Private Sub vgridviewContextMenu_Item_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim it As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)
        Dim vgc As VGridControl = TryCast(it.Tag, VGridControl)
        Select Case it.Name
            Case "customization"
                vgc.RowsCustomization()
            Case "savetodb"
                SaveLayoutToDb(vgc)
                MessageBox.Show("OK")
        End Select

    End Sub

    Private Sub gv_DragObjectStart(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.DragObjectStartEventArgs)
        Dim view As GridView = TryCast(sender, GridView)
        view.OptionsCustomization.AllowSort = True
    End Sub

    Private Sub gv_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs)

        Dim v As GridView = sender
        Dim rc As Integer = v.RowCount - 2


        Dim col As XtraGrid.Columns.GridColumn = v.Columns.ColumnByFieldName("LineNumber")
        If Not col Is Nothing Then

            Dim ln = v.GetRowCellValue(rc, col)
            v.SetRowCellValue(e.RowHandle, col, ln + 100)
            'v.SetRowCellValue(e.RowHandle, col, v.RowCount * 100)
        End If
    End Sub

    Private Sub gv_CustomDrawRowIndicator(ByVal sender As Object, ByVal e As RowIndicatorCustomDrawEventArgs)

        If e.Info.IsRowIndicator Then
            If e.RowHandle < 0 Then
                e.Info.DisplayText = ""
            Else
                e.Info.DisplayText = (e.RowHandle + 1).ToString
            End If
        End If

    End Sub

    Private Sub gv_InvalidRowException(ByVal sender As Object, ByVal e As InvalidRowExceptionEventArgs)
        e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction
    End Sub

    Private Sub gv_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)

        Dim v As GridView = sender
        Dim c As GridControl = v.GridControl

        If e.KeyData = Keys.Delete AndAlso sender.State = GridState.Normal AndAlso myViewMode.ToLower = "edit" Then
            DoActions(Me.myGuid, String.Format("{0}.{1}", c.Name, v.Name), "deleterow", gv:=v)


            '    ' TO XREIAOMASTE KAI DOULEYEI PREPEI NA VRW TIS SYNTHIKES
            'ElseIf e.KeyData = Keys.Return AndAlso v.Name.StartsWith("bc") Then
            '    e.Handled = True
            '    v.CloseEditor()
            '    v.UpdateCurrentRow()
            '    v.MoveNext()

        End If


    End Sub

    Private Sub gv_ValidateRow(ByVal sender As Object, ByVal e As ValidateRowEventArgs)

        If sender.OptionsBehavior.readonly = True Then Exit Sub
        If sender.FocusedRowModified Then
            Dim fGuid As Guid = App.Objects.curMainControl.myGuid
            Dim gvName As String = String.Format("{0}.{1}", sender.gridcontrol.name, sender.name)

            DoActions(fGuid, gvName, "RowVal", gv:=sender, rEVA:=e)

        End If

    End Sub

    Public ForceEditorValidation As Boolean = False

    Private Sub gv_ValidatingEditor(ByVal sender As Object, ByVal e As DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs)

        ' na dw ayto http://documentation.devexpress.com/#WindowsForms/CustomDocument773

        Dim gv As GridView = sender

        Dim col = gv.FocusedColumn
        Dim fld = col.FieldName
        Dim gvname As String = String.Format("{0}.{1}.{2}", gv.GridControl.Name, gv.Name, fld)


        If gv.OptionsBehavior.ReadOnly = True Then Exit Sub
        If gv.IsFilterRow(gv.FocusedRowHandle) = True Then Exit Sub


        Dim run As Boolean = False
        Try
            run = e.Value <> gv.ActiveEditor.OldEditValue
        Catch ex As Exception
            If e.Value Is DBNull.Value AndAlso gv.ActiveEditor.OldEditValue Is DBNull.Value Then
                run = False
            Else
                run = True
            End If
        End Try
        'If e.Value Is DBNull.Value Then exit sub
        'If gv.ActiveEditor.OldEditValue Is DBNull.Value Then exit sub

        If run = True OrElse ForceEditorValidation = True Then

            If gv.FocusedRowModified Then

                If CheckForValidation(Me, gvname) = False Then
                    Exit Sub

                Else
                    DoActions(Me.myGuid, gvname, "CellVal", gv:=gv, gEVA:=e)

                    If e.Valid Then
                        Dim row As Integer
                        If gv.FocusedRowHandle < 0 Then
                            row = gv.RowCount - 1
                        Else
                            row = gv.FocusedRowHandle
                        End If

                        'gv.BeginDataUpdate()

                        DoActions(Me.myGuid, gvname, "AfterCellVal", gv:=gv, gEVA:=e)

                        'gv.UpdateCurrentRow()
                        'gv.EndDataUpdate()

                        'gv.FocusedRowHandle = row
                    End If
                End If
            End If
        End If

    End Sub

    Private Sub gv_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Try


            Dim v As GridView = sender

            App.Objects.curFocusedGridControl = v.GridControl
            App.Objects.curFocusedView = v

            Dim pt As Point = v.GridControl.PointToClient(Control.MousePosition)

            Dim info As GridHitInfo = v.CalcHitInfo(pt)
            If info.InColumn Then

                info.Column.BestFit()

            Else

                Dim nm As String = String.Format("{0}.{1}", v.GridControl.Name, v.Name)
                DoActions(Me.myGuid, nm, "dblclick", gv:=v)
            End If
        Catch ex As Exception
            WriteLogEntry("Grid DblClick Error" & ex.Message)
        End Try


    End Sub

    Private Sub gv_MasterRowExpanded(ByVal sender As Object, ByVal e As CustomMasterRowEventArgs)

        Dim detailView As GridView = DirectCast(sender.GetDetailView(e.RowHandle, e.RelationIndex), GridView)

        If Not detailView Is Nothing Then

            Dim g As GridControl = TryCast(sender.gridcontrol, GridControl)

            Dim criteria As String = String.Format("frmName='{0}' and ParentName='{1}' and Type like 'rep%'", Name, detailView.Name)
            Dim RepItems() As DataRow = objectDetailsPropertiesTable.Select(criteria)

            For i = 0 To UBound(RepItems)

                Dim cntl As String = String.Format("{0}.{1}", g.Name, detailView.Name)
                Dim EventsToRun As String = RepItems(i)("EventsToRun")

                Select Case RepItems(i)("Type").tolower
                    Case "replookup"
                        Dim repLookUp As New RepositoryItemLookUpEdit() With {.Name = RepItems(i)("ctrlName")}

                        Dim r As DataRow = GetActionsResult(Me.myGuid, String.Format("{0}.{1}", cntl, repLookUp.Name), "ChangeDatasource")
                        Dim lkda As New SqlDataAdapter(r("f2"), GetCon)
                        Dim lkds As New DataSet
                        lkda.Fill(lkds)
                        lkda.Dispose()
                        lkds.Dispose()

                        repLookUp.DataSource = lkds.Tables(0)
                        repLookUp.DisplayMember = r("f3")
                        repLookUp.ValueMember = r("f4")

                        repLookUp.PopupWidth = RepItems(i)("popWidth")

                        AddRepLKEHandlers(repLookUp, EventsToRun)

                        detailView.Columns(RepItems(i)("TableColumn")).ColumnEdit = repLookUp


                    Case "repgridlookup"
                        Dim repGridLookUp As New RepositoryItemGridLookUpEdit() With {.Name = RepItems(i)("ctrlName")}

                        repGridLookUp.ViewType = GridLookUpViewType.GridView
                        Dim repglkView As GridView = repGridLookUp.View

                        repglkView.Name = "replv" & repGridLookUp.Name
                        Dim xTag As New clsTagExtender() With {.SenderMaster = Me.Name, .vAllowSaveLayout = True}

                        repglkView.Tag = xTag

                        repglkView.OptionsView.ShowAutoFilterRow = True
                        repglkView.OptionsBehavior.ReadOnly = True

                        Dim r As DataRow = GetActionsResult(Me.myGuid, String.Format("{0}.{1}", cntl, repGridLookUp.Name), "ChangeDatasource")
                        Dim glda As New SqlDataAdapter(r("f2"), GetCon)
                        Dim glds As New DataSet
                        glda.Fill(glds)
                        glda.Dispose()
                        glds.Dispose()

                        repGridLookUp.DataSource = glds.Tables(0)
                        repGridLookUp.DisplayMember = r("f3")
                        repGridLookUp.ValueMember = r("f4")

                        repGridLookUp.PopupFormSize = New Size(RepItems(i)("popWidth"), RepItems(i)("popHeight"))

                        AddRepGLKHandlers(repGridLookUp, EventsToRun)
                        detailView.Columns(RepItems(i)("TableColumn")).ColumnEdit = repGridLookUp

                    Case "repcombobox"
                        Using repCombobox As New RepositoryItemComboBox() With {.Name = RepItems(i)("ctrlName")}
                            Dim r As DataRow = GetActionsResult(Me.myGuid, String.Format("{0}.{1}", cntl, repCombobox.Name), "ChangeDatasource")
                            Dim cmbdst As New DataSet()
                            Dim cmbadp As New SqlDataAdapter(r("f2"), GetCon)
                            cmbadp.Fill(cmbdst)
                            cmbadp.Dispose()
                            cmbdst.Dispose()
                            For t As Integer = 0 To cmbdst.Tables(0).Rows.Count - 1
                                repCombobox.Items.Add(cmbdst.Tables(0).Rows(t)(r("f3")))
                            Next
                            detailView.Columns(RepItems(i)("TableColumn")).ColumnEdit = repCombobox
                        End Using

                    Case Else

                End Select

            Next

        End If


    End Sub

    Private Sub gv_ShownEditor(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim v As GridView = TryCast(sender, GridView)
        ' Prepei na to dw kala (einai ola karfwmena
        If TypeOf v.ActiveEditor Is GridLookUpEdit Then
            Dim ed As GridLookUpEdit = TryCast(v.ActiveEditor, GridLookUpEdit)
            Dim n As String = v.FocusedColumn.ColumnEdit.Name
            ed.Name = v.GridControl.Name & "." & v.Name & "." & n
            ChangeDs(ed)
            Dim rephlp As New clsContexMenuHelper(ed.Properties.View)
        End If
        'v.ActiveEditor.IsModified = GetRefreshFields(v.Name, v.FocusedColumn.FieldName)

    End Sub

    Private Sub gv_FocusedRowChanged(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs)

        Dim nm As String = String.Format("{0}.{1}", sender.gridcontrol.name, sender.name)
        DoActions(Me.myGuid, nm, "rowchange", gv:=sender)

    End Sub

    Private Sub gv_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs)

        'Creates Focused Row color based on init values (in up1init)
        Dim view As GridView = sender

        If e.RowHandle = view.FocusedRowHandle Then

            If App.Constants.FocusedForeColor <> "" Then e.Appearance.ForeColor = getColor(App.Constants.FocusedForeColor)
            If App.Constants.FocusedBackColor <> "" Then e.Appearance.BackColor = getColor(App.Constants.FocusedBackColor)
            If App.Constants.FocusedFontFamily <> "" Then e.Appearance.Font = New Font(App.Constants.FocusedFontFamily, e.Appearance.Font.Size)
            If App.Constants.FocusedFontSize <> 0 Then e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, App.Constants.FocusedFontSize)
            If App.Constants.FocusedFontStyle <> "" Then e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, getFontStyle(App.Constants.FocusedFontStyle))
        End If


        'Creates CellColor based on Cellcolor value in table
        If view.Columns("CellColor") Is Nothing Then Return
        Dim styling As String

        If Not view.IsNewItemRow(e.RowHandle) Then
            styling = view.GetRowCellValue(e.RowHandle, "CellColor")
        Else
            styling = ""
        End If
        If styling = "" Then Exit Sub

        Dim cs As String() = Split(styling, "|")

        For i = 0 To cs.Length - 1
            Dim cellstyles = Split(cs(i), ";")

            If e.Column.FieldName = cellstyles(0) Then
             

                Select Case cellstyles.Length - 1
                    Case 0

                    Case 1
                        e.Appearance.ForeColor = If(cellstyles(1) = "", e.Appearance.ForeColor, getColor(cellstyles(1).ToLower))
                    Case 2
                        e.Appearance.ForeColor = If(cellstyles(1) = "", e.Appearance.ForeColor, getColor(cellstyles(1).ToLower))
                        e.Appearance.BackColor = If(cellstyles(2) = "", e.Appearance.BackColor, getColor(cellstyles(2).ToLower))
                    Case 3
                        e.Appearance.ForeColor = If(cellstyles(1) = "", e.Appearance.ForeColor, getColor(cellstyles(1).ToLower))
                        e.Appearance.BackColor = If(cellstyles(2) = "", e.Appearance.BackColor, getColor(cellstyles(2).ToLower))
                        Dim fontfamily As String = If(cellstyles(3).ToLower = "", e.Appearance.Font.FontFamily.Name, cellstyles(3).ToLower)
                        e.Appearance.Font = New Font(fontfamily, e.Appearance.Font.Size)
                    Case 4
                        e.Appearance.ForeColor = If(cellstyles(1) = "", e.Appearance.ForeColor, getColor(cellstyles(1).ToLower))
                        e.Appearance.BackColor = If(cellstyles(2) = "", e.Appearance.BackColor, getColor(cellstyles(2).ToLower))

                        Dim fontfamily As String = If(cellstyles(3).ToLower = "", e.Appearance.Font.FontFamily.Name, cellstyles(3).ToLower)
                        e.Appearance.Font = New Font(fontfamily, e.Appearance.Font.Size)
                        If cellstyles(4) = "" Then
                            e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size)
                        Else
                            e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, CInt(cellstyles(4)))
                        End If
                    Case 5
                        e.Appearance.ForeColor = If(cellstyles(1) = "", e.Appearance.ForeColor, getColor(cellstyles(1).ToLower))
                        e.Appearance.BackColor = If(cellstyles(2) = "", e.Appearance.BackColor, getColor(cellstyles(2).ToLower))
                        Dim fontfamily As String = If(cellstyles(3).ToLower = "", e.Appearance.Font.FontFamily.Name, cellstyles(3).ToLower)
                        e.Appearance.Font = New Font(fontfamily, e.Appearance.Font.Size)
                        If cellstyles(4) = "" Then
                            e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size)
                        Else
                            e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, CInt(cellstyles(4)))
                        End If
                        e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, getFontStyle(cellstyles(5).ToLower))
                End Select

            End If

        Next



    End Sub

    Private Sub gv_RowStyle(ByVal sender As Object, ByVal e As RowStyleEventArgs)

        ' Creates rowcolor based on rowcolor values in table
        Dim View As GridView = sender
        Dim fontsize As Integer = 0

        If View.Columns("RowColor") Is Nothing OrElse View.GetRowCellDisplayText(e.RowHandle, View.Columns("RowColor")) = "" Then Return

        Dim rowcolor As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("RowColor"))


        If (e.RowHandle >= 0) AndAlso (e.RowHandle <> GridControl.NewItemRowHandle) Then

            Dim cellstyles As String() = Split(rowcolor, ";")


            Select Case cellstyles.Length
                Case 0

                Case 1
                    e.Appearance.ForeColor = If(cellstyles(0) = "", e.Appearance.ForeColor, getColor(cellstyles(0).ToLower))
                Case 2
                    e.Appearance.ForeColor = If(cellstyles(0) = "", e.Appearance.ForeColor, getColor(cellstyles(0).ToLower))
                    e.Appearance.BackColor = If(cellstyles(1) = "", e.Appearance.BackColor, getColor(cellstyles(1).ToLower))
                Case 3
                    e.Appearance.ForeColor = If(cellstyles(0) = "", e.Appearance.ForeColor, getColor(cellstyles(0).ToLower))
                    e.Appearance.BackColor = If(cellstyles(1) = "", e.Appearance.BackColor, getColor(cellstyles(1).ToLower))
                    Dim f As String = If(cellstyles(2).ToLower = "", e.Appearance.Font.FontFamily, cellstyles(2).ToLower)
                    e.Appearance.Font = New Font(f, e.Appearance.Font.Size)
                Case 4
                    e.Appearance.ForeColor = If(cellstyles(0) = "", e.Appearance.ForeColor, getColor(cellstyles(0).ToLower))
                    e.Appearance.BackColor = If(cellstyles(1) = "", e.Appearance.BackColor, getColor(cellstyles(1).ToLower))
                    Dim fontfamily As String = If(cellstyles(2).ToLower = "", e.Appearance.Font.FontFamily, cellstyles(2).ToLower)
                    e.Appearance.Font = New Font(fontfamily, e.Appearance.Font.Size)
                    If cellstyles(3) = "" Then
                        e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size)
                    Else
                        e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, CInt(cellstyles(3)))
                    End If
                Case 5
                    e.Appearance.ForeColor = If(cellstyles(0) = "", e.Appearance.ForeColor, getColor(cellstyles(0).ToLower))
                    e.Appearance.BackColor = If(cellstyles(1) = "", e.Appearance.BackColor, getColor(cellstyles(1).ToLower))
                    Dim fontfamily As String = If(cellstyles(2).ToLower = "", e.Appearance.Font.FontFamily.Name, cellstyles(2).ToLower)
                    e.Appearance.Font = New Font(fontfamily, e.Appearance.Font.Size)
                    If cellstyles(3) = "" Then
                        e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size)
                    Else
                        e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, CInt(cellstyles(3)))
                    End If
                    e.Appearance.Font = New Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, getFontStyle(cellstyles(4).ToLower))
            End Select

        End If



    End Sub

   
    Private Sub gv_FilterEditorCreated(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.FilterControlEventArgs)

        e.FilterControl.ShowOperandTypeIcon = True
    End Sub

    '***********************
    Public Function GetRepItem(ByVal cellID As String, ByVal cellValue As String)
        Dim res = Nothing

        Dim com As String = String.Format("exec up1GetRepItems {0}", cellID)
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter(com, GetCon)

        da.Fill(ds)

        da.Dispose()
        ds.Dispose()

        Select Case ds.Tables(0).Rows(0)("editor").tolower

            Case "time"

                Dim tim As New RepositoryItemTimeEdit

                res = tim

            Case "password"

                Dim passtxt As New RepositoryItemTextEdit
                passtxt.PasswordChar = "*"

                res = passtxt
            Case "date"

                Dim dt As New RepositoryItemTextEdit

                ''******************* Douleyei alla prepei na vazeis ta miden
                'dt.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular
                'dt.Mask.UseMaskAsDisplayFormat = True
                'dt.Mask.EditMask = "\d?\d?/\d?\d?/\d\d\d\d"

                '****************** Douleyei kalytera alla prepei pali na vazeis to miden
                dt.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx
                dt.Mask.UseMaskAsDisplayFormat = True
                dt.Mask.EditMask = "([012]?[1-9]|[123]0|31)/(0?[1-9]|1[012])|([012]?[1-9]|[123]0|31)/(0?[1-9]|1[012])/([123][0-9])?[0-9][0-9]"

                AddHandler dt.Validating, AddressOf reptext_Validating

                res = dt

            Case "lookup"
                Dim lk As New RepositoryItemLookUpEdit
                Dim lkds As New DataSet
                Dim lkda As New SqlDataAdapter(ds.Tables(0).Rows(0)("selectcommand"), GetCon)
                lkda.Fill(lkds)
                lkda.Dispose()
                lkds.Dispose()

                lk.DataSource = lkds.Tables(0)
                lk.DisplayMember = ds.Tables(0).Rows(0)("displaymember")
                lk.ValueMember = ds.Tables(0).Rows(0)("valuemember")

                res = lk

            Case "checkcombo"

                Dim chkcmb As New RepositoryItemCheckedComboBoxEdit
                Dim cmbds As New DataSet

                Dim cmbda As New SqlDataAdapter(ds.Tables(0).Rows(0)("selectcommand"), GetCon)
                cmbda.Fill(cmbds)
                cmbda.Dispose()
                cmbds.Dispose()

                chkcmb.DataSource = cmbds.Tables(0)
                chkcmb.DisplayMember = ds.Tables(0).Rows(0)("displaymember")
                chkcmb.ValueMember = ds.Tables(0).Rows(0)("valuemember")

                chkcmb.SelectAllItemVisible = True
                chkcmb.SelectAllItemCaption = "(Επιλογή 'Ολων)"
                'chkcmb.SelectAllItemVisible = False

                res = chkcmb

            Case "combobox"

                Dim combo As New RepositoryItemComboBox
                Dim combods As New DataSet

                Dim comboad As New SqlDataAdapter(ds.Tables(0).Rows(0)("selectcommand"), GetCon)
                comboad.Fill(combods)
                comboad.Dispose()
                combods.Dispose()

                combo.AllowNullInput = True

                For t = 0 To combods.Tables(0).Rows.Count - 1
                    combo.Items.Add(combods.Tables(0).Rows(t)(ds.Tables(0).Rows(0)("displaymember")))
                Next

                res = combo

        End Select


        Return res
    End Function


End Class
