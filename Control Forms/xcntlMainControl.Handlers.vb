Imports DevExpress.XtraGrid.Views.Card
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid.Views.BandedGrid
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid
Imports DevExpress.XtraPivotGrid
Imports DevExpress.XtraVerticalGrid

Partial Public Class xcntlMainControl
    Inherits XtraForm

    Private Sub AddRepGLKHandlers(ByVal repGLK As RepositoryItemGridLookUpEdit, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler repGLK.QueryPopUp, AddressOf glk_QueryPopUp

    End Sub

    Private Sub AddGLKHandlers(ByVal glk As GridLookUpEdit, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler glk.Validating, AddressOf ctr_Validating
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler glk.Validated, AddressOf ctr_Validated
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler glk.GotFocus, AddressOf ctr_GotFocus
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler glk.Click, AddressOf ctr_Click
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler glk.KeyDown, AddressOf ctr_KeyDown
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler glk.QueryPopUp, AddressOf glk_QueryPopUp

    End Sub
    Private Sub AddRepLKEHandlers(ByVal repLKE As RepositoryItemLookUpEdit, ByVal opt As String)

    End Sub
    Private Sub AddLKEHandlers(ByVal lke As LookUpEdit, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler lke.Validating, AddressOf ctr_Validating
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler lke.Validated, AddressOf ctr_Validated
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler lke.GotFocus, AddressOf ctr_GotFocus
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler lke.Click, AddressOf ctr_Click
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler lke.KeyDown, AddressOf ctr_KeyDown
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler lke.QueryPopUp, AddressOf lke_QueryPopUp

    End Sub


    Private Sub AddRepCMBHandlers(ByVal repcmb As RepositoryItemComboBox, ByVal opt As String)
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler repcmb.QueryPopUp, AddressOf cmb_QueryPopUp
        'If opt.ToLower.Contains("validating") OrElse opt.ToLower.Contains("all") Then AddHandler repcmb.Validating, AddressOf ctr_Validating
        'If opt.ToLower.Contains("validated") OrElse opt.ToLower.Contains("all") Then AddHandler repcmb.Validated, AddressOf ctr_Validated

    End Sub
    Private Sub AddCMBHandlers(ByVal cmb As ComboBoxEdit, ByVal opt As String)
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler cmb.QueryPopUp, AddressOf cmb_QueryPopUp
        If opt.ToLower.Contains("validating") OrElse opt.ToLower.Contains("all") Then AddHandler cmb.Validating, AddressOf ctr_Validating
        If opt.ToLower.Contains("validated") OrElse opt.ToLower.Contains("all") Then AddHandler cmb.Validated, AddressOf ctr_Validated
    End Sub


    Private Sub AddTXTHandlers(ByVal txt As TextEdit, ByVal opt As String)

        If opt.ToLower.Contains("validating") OrElse opt.ToLower.Contains("all") Then AddHandler txt.Validating, AddressOf ctr_Validating
        If opt.ToLower.Contains("validated") OrElse opt.ToLower.Contains("all") Then AddHandler txt.Validated, AddressOf ctr_Validated
        If opt.ToLower.Contains("gotfocus") OrElse opt.ToLower.Contains("all") Then AddHandler txt.GotFocus, AddressOf ctr_GotFocus
        If opt.ToLower.Contains("click") OrElse opt.ToLower.Contains("all") Then AddHandler txt.Click, AddressOf ctr_Click
        If opt.ToLower.Contains("keydown") OrElse opt.ToLower.Contains("all") Then AddHandler txt.KeyDown, AddressOf ctr_KeyDown
        If opt.ToLower.Contains("doubleclick") OrElse opt.ToLower.Contains("all") Then AddHandler txt.DoubleClick, AddressOf ctr_DoubleClick

    End Sub

    Private Sub AddRepTXTHandlers(ByVal reptxt As RepositoryItemTextEdit, ByVal opt As String)

    End Sub

    Private Sub AddButtonHandlers(ByVal bt As SimpleButton, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bt.Click, AddressOf btn_Click

    End Sub

    Private Sub AddMemoEditHandlers(ByVal mem As MemoEdit, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler mem.GotFocus, AddressOf ctr_GotFocus
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler mem.Click, AddressOf ctr_Click
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler mem.Validating, AddressOf ctr_Validating
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler mem.Validated, AddressOf ctr_Validated
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler mem.DoubleClick, AddressOf ctr_DoubleClick

    End Sub

    Private Sub AddCheckEditHandlers(ByVal chk As CheckEdit, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler chk.GotFocus, AddressOf ctr_GotFocus
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler chk.Click, AddressOf ctr_Click
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler chk.Validating, AddressOf ctr_Validating
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler chk.Validated, AddressOf ctr_Validated
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler chk.DoubleClick, AddressOf ctr_DoubleClick

    End Sub

    Private Sub AddDateEditHandlers(ByVal dat As DateEdit, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler dat.GotFocus, AddressOf ctr_GotFocus
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler dat.Click, AddressOf ctr_Click
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler dat.Validating, AddressOf ctr_Validating
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler dat.Validated, AddressOf ctr_Validated
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler dat.DoubleClick, AddressOf ctr_DoubleClick

    End Sub

    Private Sub AddRepDateEditHandlers(ByVal repdat As RepositoryItemDateEdit, ByVal opt As String)

        'If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler repdat.GotFocus, AddressOf ctr_GotFocus

    End Sub

    Private Sub AddtimeEditHandlers(ByVal tim As TimeEdit, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler tim.GotFocus, AddressOf ctr_GotFocus
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler tim.Click, AddressOf ctr_Click
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler tim.Validating, AddressOf ctr_Validating
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler tim.Validated, AddressOf ctr_Validated
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler tim.DoubleClick, AddressOf ctr_DoubleClick

    End Sub

    Private Sub AddRepTimeEditHandlers(ByVal repTIM As RepositoryItemTimeEdit, ByVal opt As String)

        ' If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler repGLK.QueryPopUp, AddressOf glk_QueryPopUp

    End Sub

    Private Sub AddPictureEditHandlers(ByVal pic As PictureEdit, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler pic.GotFocus, AddressOf ctr_GotFocus
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler pic.Click, AddressOf ctr_Click
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler pic.DoubleClick, AddressOf ctr_DoubleClick

    End Sub

    ' VGridControl
    Private Sub AddVGridControlHandlers(ByVal vgc As VGridControl, ByVal opt As String)
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler vgc.MouseDown, AddressOf vgc_MouseDown

    End Sub

    ' GridControl
    Private Sub AddGridControlHandlers(ByVal gc As GridControl, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gc.ViewRegistered, AddressOf gc_ViewRegistered
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gc.ProcessGridKey, AddressOf gc_ProcessGridKey
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gc.FocusedViewChanged, AddressOf gc_FocusedViewChanged
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gc.GotFocus, AddressOf gc_GotFocus

    End Sub

    'GridViews
    Private Sub AddCardViewHandlers(ByVal cview As CardView, ByVal opt As String)

    End Sub
    Private Sub AddGridViewhandlers(ByVal gview As GridView, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.ValidateRow, AddressOf gv_ValidateRow
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.ValidatingEditor, AddressOf gv_ValidatingEditor
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.InvalidRowException, AddressOf gv_InvalidRowException
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.CustomDrawRowIndicator, AddressOf gv_CustomDrawRowIndicator
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.KeyDown, AddressOf gv_KeyDown
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.DoubleClick, AddressOf gv_DoubleClick
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.MasterRowExpanded, AddressOf gv_MasterRowExpanded
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.ShownEditor, AddressOf gv_ShownEditor
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.InitNewRow, AddressOf gv_InitNewRow
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.FocusedRowChanged, AddressOf gv_FocusedRowChanged
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.MouseDown, AddressOf gv_MouseDown
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.DragObjectStart, AddressOf gv_DragObjectStart
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.CustomRowCellEdit, AddressOf gv_CustomRowCellEdit
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.RowStyle, AddressOf gv_RowStyle
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.RowCellStyle, AddressOf gv_RowCellStyle
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler gview.FilterEditorCreated, AddressOf gv_FilterEditorCreated

    End Sub

    Private Sub AddBandViewhandlers(ByVal bview As AdvBandedGridView, ByVal opt As String)

        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.ValidateRow, AddressOf gv_ValidateRow
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.ValidatingEditor, AddressOf gv_ValidatingEditor
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.InvalidRowException, AddressOf gv_InvalidRowException
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.CustomDrawRowIndicator, AddressOf gv_CustomDrawRowIndicator
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.KeyDown, AddressOf gv_KeyDown
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.DoubleClick, AddressOf gv_DoubleClick
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.MasterRowExpanded, AddressOf gv_MasterRowExpanded
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.ShownEditor, AddressOf gv_ShownEditor
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.InitNewRow, AddressOf gv_InitNewRow
        If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler bview.FocusedRowChanged, AddressOf gv_FocusedRowChanged

    End Sub

    Private Sub AddPivotHandlers(ByVal pvg As PivotGridControl, ByVal opt As String)

        'If opt.ToLower.Contains("") OrElse opt.ToLower.Contains("all") Then AddHandler pvg.PopupMenuShowing, AddressOf pvg_PopupMenuShowing
    End Sub
End Class
