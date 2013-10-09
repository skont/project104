Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.Utils
Imports DevExpress.Utils.Menu
Imports DevExpress.XtraPivotGrid

Public Class clsContexMenuHelper

    '----------------
    ' Δεν Ειναι Δικο μου.
    ' Χρησιμοποιειται για να μπορει να κανει η κλαση Dispose

    Implements IDisposable

    ' To detect redundant calls
    Private disposed As Boolean = False

    ' IDisposable
    Protected Overridable Sub Dispose( _
       ByVal disposing As Boolean)

        Me.disposed = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to 
    ' correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. 
        ' Put cleanup code in
        ' Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        ' Do not change this code. 
        ' Put cleanup code in
        ' Dispose(ByVal disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub
#End Region
    '----------------



    Private obj As Object

    Private WithEvents PivotControl As PivotGridControl
    Private WithEvents GridView As GridView

    Private headerEditor As TextEdit
    Private editedColumn As GridColumn
    Private mouseHit As Point


    Public ReadOnly Property IsEditing() As Boolean
        Get
            Return editedColumn IsNot Nothing
        End Get
    End Property


    Public Sub New(ByVal view As Object)

        Select Case TypeName(view)

            Case "PivotGridControl"
                PivotControl = view
                obj = view


            Case Else
                GridView = view
                obj = view

        End Select

    End Sub

    ' Create PivotField menu objects

    Private Sub PivotControl_PopupMenuShowing(ByVal sender As Object, ByVal e As DevExpress.XtraPivotGrid.PopupMenuShowingEventArgs) Handles PivotControl.PopupMenuShowing
        mouseHit = e.Point

        If e.MenuType = PivotGridMenuType.Header Then
            Dim RenamePivotField As New DXMenuItem(App.Constants.RenameField, New EventHandler(AddressOf DoRenamePivotField), Nothing) With {.BeginGroup = True}
            e.Menu.Items.Add(RenamePivotField)
            Dim SaveLayoutToXML As New DXMenuItem(App.Constants.SaveToXML, New EventHandler(AddressOf DoSaveLayoutToXML), Nothing) With {.BeginGroup = True}
            e.Menu.Items.Add(SaveLayoutToXML)
            Dim LoadLayoutFromXML As New DXMenuItem(App.Constants.LoadFromXML, New EventHandler(AddressOf DoLoadLayoutFromXML), Nothing)
            e.Menu.Items.Add(LoadLayoutFromXML)
            Dim SaveLayoutToDB As New DXMenuItem(App.Constants.SaveToDB, New EventHandler(AddressOf DoSaveLayoutToDB), Nothing)
            e.Menu.Items.Add(SaveLayoutToDB)
            Dim LoadLayoutFromDB As New DXMenuItem(App.Constants.LoadFromDb, New EventHandler(AddressOf DoLoadLayoutFromDB), Nothing)
            e.Menu.Items.Add(LoadLayoutFromDB)
            Dim PrintPreview As New DXMenuItem(App.Constants.PrintPreview, New EventHandler(AddressOf DoPrintPreview), Nothing)
            e.Menu.Items.Add(PrintPreview)
            Dim FormatPivotField As New DXMenuItem(App.Constants.FormatField, New EventHandler(AddressOf DoFormatPivotField), Nothing) With {.BeginGroup = True}
            e.Menu.Items.Add(FormatPivotField)
            Dim ClearFormatPivotField As New DXMenuItem(App.Constants.ClearFormat, New EventHandler(AddressOf DoClearFormatPivotField), Nothing)
            e.Menu.Items.Add(ClearFormatPivotField)
            If e.Area = PivotArea.ColumnArea Or e.Area = PivotArea.RowArea Then
                Dim RunningSumsPivotField As New DXMenuItem(App.Constants.RunningSums, New EventHandler(AddressOf DoRunningSumsPivotField), Nothing)
                e.Menu.Items.Add(RunningSumsPivotField)
                Dim ToggleFieldTotals As New DXMenuItem(App.Constants.ToggleFieldTotals, New EventHandler(AddressOf DoToggleFieldTotals), Nothing)
                e.Menu.Items.Add(ToggleFieldTotals)
            End If
            If e.Area = PivotArea.ColumnArea Then
                Dim ToggleColumnGrandTotals As New DXMenuItem(App.Constants.ToggleColumnGrandTotals, New EventHandler(AddressOf DoToggleColumnGrandTotals), Nothing)
                e.Menu.Items.Add(ToggleColumnGrandTotals)
            End If
            If e.Area = PivotArea.RowArea Then
                Dim ToggleRowGrandTotals As New DXMenuItem(App.Constants.ToggleRowGrandTotals, New EventHandler(AddressOf DoToggleRowGrandTotals), Nothing)
                e.Menu.Items.Add(ToggleRowGrandTotals)

            End If

        End If
    End Sub



    ' Create GridViewColumn Menu Items


    Private Sub GridView_PopupMenuShowing(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs) Handles GridView.PopupMenuShowing
        mouseHit = e.Point

        Dim hitinfo As GridHitInfo = GridView.CalcHitInfo(mouseHit)

        If e.MenuType = GridMenuType.Column Then
            Dim RenameGridColumn As New DXMenuItem(App.Constants.RenameField, New EventHandler(AddressOf DoRenameGridColumn), Nothing) With {.BeginGroup = True}
            e.Menu.Items.Add(RenameGridColumn)
            Dim SaveLayoutToXML As New DXMenuItem(App.Constants.SaveToXML, New EventHandler(AddressOf DoSaveLayoutToXML), Nothing) With {.BeginGroup = True}
            e.Menu.Items.Add(SaveLayoutToXML)
            'If appUser.CanSaveLayout = False Then e.Menu.Items.Item(0).Enabled = False

            Dim LoadLayoutFromXML As New DXMenuItem(App.Constants.LoadFromXML, New EventHandler(AddressOf DoLoadLayoutFromXML), Nothing)
            e.Menu.Items.Add(LoadLayoutFromXML)
            Dim SaveLayoutToDB As New DXMenuItem(App.Constants.SaveToDB, New EventHandler(AddressOf DoSaveLayoutToDB), Nothing)
            e.Menu.Items.Add(SaveLayoutToDB)
            Dim LoadLayoutFromDB As New DXMenuItem(App.Constants.LoadFromDb, New EventHandler(AddressOf DoLoadLayoutFromDB), Nothing)
            e.Menu.Items.Add(LoadLayoutFromDB)
            Dim PrintPreview As New DXMenuItem(App.Constants.PrintPreview, New EventHandler(AddressOf DoPrintPreview), Nothing)
            e.Menu.Items.Add(PrintPreview)
            Dim FormatGridColumn As New DXMenuItem(App.Constants.FormatField, New EventHandler(AddressOf DoFormatGridColumn), Nothing) With {.BeginGroup = True}
            e.Menu.Items.Add(FormatGridColumn)
            Dim ClearFormatGridColumn As New DXMenuItem(App.Constants.ClearFormat, New EventHandler(AddressOf DoClearFormatGridColumn), Nothing)
            e.Menu.Items.Add(ClearFormatGridColumn)
        End If

    
    End Sub


    ' COMMON EVENTS
    Private Sub DoSaveLayoutToXML(ByVal sender As Object, ByVal e As EventArgs)
        SaveToXML(obj)
        MsgBox("OK")

    End Sub

    Private Sub DoLoadLayoutFromXML(ByVal sender As Object, ByVal e As EventArgs)
        LoadFromXML(obj)
        MsgBox("OK")
    End Sub

    Private Sub DoSaveLayoutToDB(ByVal sender As Object, ByVal e As EventArgs)
        SaveLayoutToDb(obj)
        MsgBox("OK")
    End Sub

    Private Sub DoLoadLayoutFromDB(ByVal sender As Object, ByVal e As EventArgs)
        LoadLayoutFromDb(obj)
        MsgBox("OK")
    End Sub

    Private Sub DoPrintPreview(ByVal sender As Object, ByVal e As EventArgs)

        If TypeOf obj Is PivotGridControl Then
            PreviewPrint(obj)
        Else

            obj.OptionsPrint.PrintDetails = True
            obj.gridcontrol.ShowPreview()


        End If


    End Sub

    Private Sub DoRenameGridColumn(ByVal sender As Object, ByVal e As EventArgs)

        Dim column As GridColumn = GetGridColumn()
        Using cf As New xfrmGridColumnHelperForm With {.Kind = "rename"}
            cf.txtFormatString.EditValue = column.Caption

            cf.ShowDialog()
            If cf.DialogResult = DialogResult.OK Then

                column.Caption = cf.txtFormatString.EditValue

            End If
        End Using

    End Sub

    Private Sub DoFormatGridColumn(ByVal sender As Object, ByVal e As EventArgs)

        Dim column As GridColumn = GetGridColumn()
        Using cf As New xfrmGridColumnHelperForm With {.Kind = "format"}
            cf.ShowDialog()
            If cf.DialogResult = DialogResult.OK Then
                With column.DisplayFormat
                    .FormatType = cf.cmbFormatType.SelectedIndex
                    .FormatString = cf.txtFormatString.EditValue
                End With

            End If
        End Using
    End Sub

    Private Sub DoClearFormatGridColumn(ByVal sender As Object, ByVal e As EventArgs)
        Dim column As GridColumn = GetGridColumn()
        column.DisplayFormat.FormatType = FormatType.None

    End Sub


    Private Function GetGridColumn() As GridColumn
        Dim info As GridHitInfo = GridView.CalcHitInfo(mouseHit)
        If info.InColumnPanel Then
            Return info.Column
        End If

        Return Nothing
    End Function


    ' PivotControl Subs

    Private Sub DoRunningSumsPivotField(ByVal sender As Object, ByVal e As EventArgs)

        Dim field As PivotGridField = GetPivotField()
        field.RunningTotal = IIf(field.RunningTotal = True, False, True)
    End Sub


    Private Sub DoToggleFieldTotals(ByVal sender As Object, ByVal e As EventArgs)

        Dim field As PivotGridField = GetPivotField()
        If field.TotalsVisibility = PivotTotalsVisibility.None Then
            field.TotalsVisibility = PivotTotalsVisibility.AutomaticTotals
        Else
            field.TotalsVisibility = PivotTotalsVisibility.None
        End If
    End Sub

    Private Sub DoToggleRowGrandTotals(ByVal sender As Object, ByVal e As EventArgs)

        If PivotControl.OptionsView.ShowRowGrandTotals = True Then
            PivotControl.OptionsView.ShowRowGrandTotals = False
            PivotControl.OptionsView.ShowRowGrandTotalHeader = False
        Else
            PivotControl.OptionsView.ShowRowGrandTotals = True
            PivotControl.OptionsView.ShowRowGrandTotalHeader = True
        End If
    End Sub

    Private Sub DoToggleColumnGrandTotals(ByVal sender As Object, ByVal e As EventArgs)

        If PivotControl.OptionsView.ShowColumnGrandTotals = True Then
            PivotControl.OptionsView.ShowColumnGrandTotals = False
            PivotControl.OptionsView.ShowColumnGrandTotalHeader = False
        Else
            PivotControl.OptionsView.ShowColumnGrandTotals = True
            PivotControl.OptionsView.ShowColumnGrandTotalHeader = True
        End If
    End Sub

    Private Sub DoRenamePivotField(ByVal sender As Object, ByVal e As EventArgs)

        Dim field As PivotGridField = GetPivotField()
        Using cf As New xfrmGridColumnHelperForm With {.Kind = "rename"}
            cf.txtFormatString.EditValue = field.Caption

            cf.ShowDialog()
            If cf.DialogResult = DialogResult.OK Then

                field.Caption = cf.txtFormatString.EditValue

            End If
        End Using
    End Sub

    Private Sub DoFormatPivotField(ByVal sender As Object, ByVal e As EventArgs)

        Dim field As PivotGridField = GetPivotField()
        Using cf As New xfrmGridColumnHelperForm With {.Kind = "format"}
            cf.ShowDialog()
            If cf.DialogResult = DialogResult.OK Then
                With field.CellFormat
                    'With field.ValueFormat
                    .FormatType = cf.cmbFormatType.SelectedIndex
                    .FormatString = cf.txtFormatString.EditValue
                End With
            End If
        End Using
    End Sub

    Private Sub DoClearFormatPivotField(ByVal sender As Object, ByVal e As EventArgs)
        Dim field As PivotGridField = GetPivotField()
        field.CellFormat.FormatType = DevExpress.Utils.FormatType.None
    End Sub

    Private Function GetPivotField()

        Dim info = PivotControl.CalcHitInfo(mouseHit)
        Return info.HeaderField

    End Function

End Class
