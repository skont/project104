' Ftiaxnei ta running sums 

Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraPivotGrid


Module modRunningSums

    '' GridView RunningSums
    'Dim GridRSField(0) As String
    'Dim cums As New List(Of Hashtable)
    Dim cums As New Collection
    Dim fields As New Collection

    Private lockUpdates As Boolean


    Public Sub GridviewRunningSums(ByVal gView As GridView, ByVal fld As String)

        gView.GridControl.ForceInitialize()
        Dim rsfld() As String = Split(fld, ";")
        Dim l As Integer = UBound(rsfld)

        If fields.Contains(gView.Name) Then fields.Remove(gView.Name)
        fields.Add(rsfld, gView.Name)

        For x As Integer = 0 To l
            ' Create an unbound column.
            Dim unbColumn As GridColumn = gView.Columns.AddField("R" & rsfld(x))
            unbColumn.VisibleIndex = gView.Columns.Count
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal
            ' Disable editing.
            unbColumn.OptionsColumn.AllowEdit = False
            ' Specify format settings.
            ' unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric

            FillCumulativeData(gView, fld)

        Next




        AddHandler gView.Layout, AddressOf gvRS_Layout
        AddHandler gView.CustomUnboundColumnData, AddressOf gvRS_CustomUnboundColumnData


    End Sub

    Private Sub gvRS_CustomUnboundColumnData(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs)

        For j As Integer = 0 To UBound(fields(sender.name))

            If e.Column.FieldName = "R" & fields(sender.name)(j) Then
                Try
                    e.Value = cums(sender.Name & fields(sender.name)(j))(e.ListSourceRowIndex)
                Catch ex As Exception
                    WriteLogEntry("CustomUnboundData: " & sender.Name & fields(sender.name)(j) & e.ListSourceRowIndex)
                End Try
            End If
        Next
       
    End Sub


    Private Sub FillCumulativeData(ByVal gv As GridView, ByVal fld As String)

        If cums.Contains(gv.Name & fld) Then cums.Remove(gv.Name & fld)
        Dim cumulatives As New Hashtable()
        'cumulatives.Clear()
        Dim sum As Double = 0
        For i As Integer = 0 To gv.DataRowCount - 1
            sum += CDbl(gv.GetRowCellValue(i, fld))
            cumulatives.Add(gv.GetDataSourceRowIndex(i), sum)
        Next


        cums.Add(cumulatives, gv.Name & fld)

    End Sub


    Private Sub gvRS_Layout(ByVal sender As Object, ByVal e As EventArgs)
        Dim view As GridView = CType(sender, GridView)
        For j As Integer = 0 To UBound(fields(sender.name))
            If Not lockUpdates Then
                lockUpdates = True
                view.BeginUpdate()
                FillCumulativeData(view, fields(sender.name)(j))
                view.EndUpdate()
                lockUpdates = False
            End If
        Next
    End Sub


    ''PivotGridControl RunningSums
    Dim f(0) As String

    Public Sub PivotRunningSums(ByVal grid As PivotGridControl, ByVal fld As String)
        Dim rsflds() As String = Split(fld, ";")
        Dim l As Integer = UBound(rsflds)
        Dim ind As Integer = 0

        ReDim f(l)

        For x As Integer = 0 To l
            grid.Fields.Add("RS" & rsflds(x), PivotArea.FilterArea)

            grid.Fields(rsflds(x)).SetAreaPosition(PivotArea.DataArea, ind)
            grid.Fields("RS" & rsflds(x)).SetAreaPosition(PivotArea.DataArea, ind + 1)

            f(x) = rsflds(x)

            'grid.Fields("RS" & rsflds(x)).Options.ShowGrandTotal = False
            grid.Fields(rsflds(x)).Options.AllowDrag = DevExpress.Utils.DefaultBoolean.False
            grid.Fields("RS" & rsflds(x)).Options.AllowDrag = DevExpress.Utils.DefaultBoolean.False

            ind += 2
        Next

        AddHandler grid.CustomCellDisplayText, AddressOf rspivot_CustomCellDisplayText

    End Sub


    Private Sub rspivot_CustomCellDisplayText(ByVal sender As Object, ByVal e As PivotCellDisplayTextEventArgs)

        Dim pivot As PivotGridControl = CType(sender, PivotGridControl)
        For i As Integer = 0 To UBound(f)
            Customcell(pivot, e, f(i))
        Next
    End Sub

    Dim processingCustomCellDisplayText As Boolean
    Private Sub Customcell(ByVal obj As PivotGridControl, ByVal arg As PivotCellDisplayTextEventArgs, ByVal rsfldname As String)

        If arg.DataField.FieldName = "RS" & rsfldname Then
            If processingCustomCellDisplayText Then Exit Sub
            processingCustomCellDisplayText = True
            Dim total As Decimal = 0

            For i As Integer = 0 To arg.RowIndex
                Dim cell As PivotCellEventArgs = obj.Cells.GetCellInfo(arg.ColumnIndex - 1, i)

                If cell.RowField Is arg.RowField AndAlso cell.DataField.FieldName = rsfldname Then
                    total += Convert.ToDecimal(cell.Value)
                End If
            Next
            arg.DisplayText = total.ToString("c2")
            processingCustomCellDisplayText = False
        End If

    End Sub

End Module


'' Ftiaxnei ta running sums 

'Imports DevExpress.XtraGrid.Columns
'Imports DevExpress.XtraGrid.Views.Grid
'Imports DevExpress.XtraPivotGrid

'Module modRunningSums

'    '' GridView RunningSums
'    Public Sub GridviewRunningSums(ByVal gView As GridView, ByVal fld As String)

'        gView.GridControl.ForceInitialize()
'        Dim rsfld() As String = Split(fld, ";")
'        Dim l As Integer = UBound(rsfld)

'        For x As Integer = 0 To l
'            Dim unbColumn As GridColumn = gView.Columns.AddField("R" & rsfld(x))
'            unbColumn.VisibleIndex = gView.Columns.Count
'            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal
'            unbColumn.OptionsColumn.AllowEdit = False
'        Next

'        AddHandler gView.CustomUnboundColumnData, AddressOf gvRS_CustomUnboundColumnData

'    End Sub

'    Private Sub gvRS_CustomUnboundColumnData(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs)
'        If e.Column.FieldName.StartsWith("RrSum") Then
'            Dim sum As Double = 0
'            Dim grid As GridView = TryCast(sender, GridView)

'            For i = 0 To e.ListSourceRowIndex
'                sum += CDbl(grid.GetRowCellValue(i, e.Column.FieldName.Remove(0, 1)))
'            Next

'            e.Value = sum

'        End If

'    End Sub

'    ''PivotGridControl RunningSums
'    Dim f(0) As String

'    Public Sub PivotRunningSums(ByVal grid As PivotGridControl, ByVal fld As String)
'        Dim rsflds() As String = Split(fld, ";")
'        Dim l As Integer = UBound(rsflds)
'        Dim ind As Integer = 0

'        ReDim f(l)

'        For x As Integer = 0 To l
'            grid.Fields.Add("RS" & rsflds(x), PivotArea.FilterArea)

'            grid.Fields(rsflds(x)).SetAreaPosition(PivotArea.DataArea, ind)
'            grid.Fields("RS" & rsflds(x)).SetAreaPosition(PivotArea.DataArea, ind + 1)

'            f(x) = rsflds(x)

'            'grid.Fields("RS" & rsflds(x)).Options.ShowGrandTotal = False
'            grid.Fields(rsflds(x)).Options.AllowDrag = DevExpress.Utils.DefaultBoolean.False
'            grid.Fields("RS" & rsflds(x)).Options.AllowDrag = DevExpress.Utils.DefaultBoolean.False

'            ind += 2
'        Next

'        AddHandler grid.CustomCellDisplayText, AddressOf rspivot_CustomCellDisplayText

'    End Sub


'    Private Sub rspivot_CustomCellDisplayText(ByVal sender As Object, ByVal e As PivotCellDisplayTextEventArgs)

'        Dim pivot As PivotGridControl = CType(sender, PivotGridControl)
'        For i As Integer = 0 To UBound(f)
'            Customcell(pivot, e, f(i))
'        Next
'    End Sub

'    Dim processingCustomCellDisplayText As Boolean
'    Private Sub Customcell(ByVal obj As PivotGridControl, ByVal arg As PivotCellDisplayTextEventArgs, ByVal rsfldname As String)

'        If arg.DataField.FieldName = "RS" & rsfldname Then
'            If processingCustomCellDisplayText Then Exit Sub
'            processingCustomCellDisplayText = True
'            Dim total As Decimal = 0

'            For i As Integer = 0 To arg.RowIndex
'                Dim cell As PivotCellEventArgs = obj.Cells.GetCellInfo(arg.ColumnIndex - 1, i)

'                If cell.RowField Is arg.RowField AndAlso cell.DataField.FieldName = rsfldname Then
'                    total += Convert.ToDecimal(cell.Value)
'                End If
'            Next
'            arg.DisplayText = total.ToString("c2")
'            processingCustomCellDisplayText = False
'        End If

'    End Sub

'End Module


' '' Ftiaxnei ta running sums 


''Imports DevExpress.XtraGrid.Columns
''Imports DevExpress.XtraGrid.Views.Grid
''Imports DevExpress.XtraPivotGrid
''Imports DevExpress.XtraGrid.Views.Base

''Module modRunningSums

''    ' GridView RunningSums
''    Dim GridRSField(0) As String
''    Public Sub GridviewRunningSums(ByVal gView As GridView, ByVal fld As String)

''        'gView.GridControl.ForceInitialize()
''        Dim rsfld() As String = Split(fld, ";")
''        Dim l As Integer = UBound(rsfld)

''        ReDim GridRSField(l)

''        For x As Integer = 0 To l
''            GridRSField(x) = rsfld(x)
''            ' Create an unbound column.
''            Dim unbColumn As GridColumn = gView.Columns.AddField("R" & rsfld(x))
''            unbColumn.VisibleIndex = gView.Columns.Count
''            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal
''            ' Disable editing.
''            unbColumn.OptionsColumn.AllowEdit = False
''            ' Specify format settings.
''            ' unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
''        Next


''        AddHandler gView.CustomUnboundColumnData, AddressOf gvRS_CustomUnboundColumnData
''        AddHandler gView.Layout, AddressOf gvRS_Layout

''        'AddHandler gView.CustomColumnDisplayText, AddressOf gvRS_CustomColumnDisplayText


''    End Sub

''    'Private Sub gvRS_CustomColumnDisplayText(ByVal sender As Object, ByVal e As CustomColumnDisplayTextEventArgs)


''    '    For j As Integer = 0 To UBound(GridRSField)
''    '        Dim fld As String = "R" & GridRSField(j)
''    '        Dim sum As Double = 0
''    '        If e.Column.FieldName <> fld Then Return

''    '        Dim view As GridView = CType(sender, GridView)

''    '        For i As Integer = 0 To view.DataRowCount - 1
''    '            sum += CDbl(view.GetRowCellValue(i, GridRSField(j)))
''    '            e.DisplayText = sum.ToString
''    '        Next


''    '    Next

''    'End Sub

''    Private Sub gvRS_CustomUnboundColumnData(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs)
''        Dim cums As New List(Of Hashtable)
''        Dim fld As String = ""
''        Dim ri As Integer = 0
''        Dim view As GridView = CType(sender, GridView)

''        Try
''            For j As Integer = 0 To UBound(GridRSField)

''                'cumulatives.Clear()
''                Dim sum As Double = 0
''                For i As Integer = 0 To view.DataRowCount - 1
''                    sum += CDbl(view.GetRowCellValue(i, GridRSField(j)))
''                    cumulatives.Add(view.GetDataSourceRowIndex(i), sum)
''                Next


''            Next
''        Catch ex As Exception
''            WriteLogEntry("Rsum Error" & fld & "-" & ri & "-" & ex.Message)
''        End Try
''        '    Dim sum As Double = 0
''        '    Dim view As GridView = CType(sender, GridView)
''        '    Dim fld As String = ""
''        '    Dim ri As Integer = 0
''        '    Try
''        '        For j As Integer = 0 To UBound(GridRSField)
''        '            For i As Integer = 0 To view.DataRowCount - 1
''        '                sum += CDbl(view.GetRowCellValue(i, GridRSField(j)))
''        '                ri = i
''        '                fld = "R" & GridRSField(j)
''        '                If e.Column.FieldName = fld Then
''        '                    'e.Value = sum
''        '                    view.SetRowCellValue(i, fld, sum)
''        '                End If

''        '            Next
''        '        Next
''        '    Catch ex As Exception
''        '        WriteLogEntry("Rsum Error" & fld & "-" & ri & "-" & ex.Message)
''        '    End Try

''    End Sub


''    Dim cums As New List(Of Hashtable)
''    Private Sub gvRS_Layout(ByVal sender As Object, ByVal e As EventArgs)

''        Dim view As GridView = CType(sender, GridView)
''        For j As Integer = 0 To UBound(GridRSField)

''            Dim cumulatives As New Hashtable()
''            'cumulatives.Clear()
''            Dim sum As Double = 0
''            For i As Integer = 0 To view.DataRowCount - 1
''                sum += CDbl(view.GetRowCellValue(i, GridRSField(j)))
''                cumulatives.Add(view.GetDataSourceRowIndex(i), sum)
''            Next

''            cums.Insert(j, cumulatives)
''        Next
''    End Sub

''    Private Sub FillCumulativeData(ByVal gv As GridView, ByVal fld As String, ByVal ind As Integer)
''        Dim cumulatives As New Hashtable()
''        cumulatives.Clear()
''        Dim sum As Double = 0
''        For i As Integer = 0 To gv.DataRowCount - 1
''            sum += CDbl(gv.GetRowCellValue(i, fld))
''            cumulatives.Add(gv.GetDataSourceRowIndex(i), sum)
''        Next

''        cums.Insert(ind, cumulatives)
''    End Sub


''    'Private lockUpdates As Boolean
''    'Private Sub gvRS_Layout(ByVal sender As Object, ByVal e As EventArgs)
''    '   
''    '    Dim view As GridView = CType(sender, GridView)
''    '    For j As Integer = 0 To UBound(GridRSField)
''    '        If Not lockUpdates Then
''    '            lockUpdates = True
''    '            view.BeginUpdate()
''    '            FillCumulativeData(view, GridRSField(j), j)
''    '            view.EndUpdate()
''    '            lockUpdates = False
''    '        End If
''    '    Next
''    'End Sub


''    ''PivotGridControl RunningSums
''    Dim f(0) As String

''    Public Sub PivotRunningSums(ByVal grid As PivotGridControl, ByVal fld As String)
''        Dim rsflds() As String = Split(fld, ";")
''        Dim l As Integer = UBound(rsflds)
''        Dim ind As Integer = 0

''        ReDim f(l)

''        For x As Integer = 0 To l
''            grid.Fields.Add("RS" & rsflds(x), PivotArea.FilterArea)

''            grid.Fields(rsflds(x)).SetAreaPosition(PivotArea.DataArea, ind)
''            grid.Fields("RS" & rsflds(x)).SetAreaPosition(PivotArea.DataArea, ind + 1)

''            f(x) = rsflds(x)

''            'grid.Fields("RS" & rsflds(x)).Options.ShowGrandTotal = False
''            grid.Fields(rsflds(x)).Options.AllowDrag = DevExpress.Utils.DefaultBoolean.False
''            grid.Fields("RS" & rsflds(x)).Options.AllowDrag = DevExpress.Utils.DefaultBoolean.False

''            ind += 2
''        Next

''        AddHandler grid.CustomCellDisplayText, AddressOf rspivot_CustomCellDisplayText

''    End Sub


''    Private Sub rspivot_CustomCellDisplayText(ByVal sender As Object, ByVal e As PivotCellDisplayTextEventArgs)

''        Dim pivot As PivotGridControl = CType(sender, PivotGridControl)
''        For i As Integer = 0 To UBound(f)
''            Customcell(pivot, e, f(i))
''        Next
''    End Sub

''    Dim processingCustomCellDisplayText As Boolean
''    Private Sub Customcell(ByVal obj As PivotGridControl, ByVal arg As PivotCellDisplayTextEventArgs, ByVal rsfldname As String)

''        If arg.DataField.FieldName = "RS" & rsfldname Then
''            If processingCustomCellDisplayText Then Exit Sub
''            processingCustomCellDisplayText = True
''            Dim total As Decimal = 0

''            For i As Integer = 0 To arg.RowIndex
''                Dim cell As PivotCellEventArgs = obj.Cells.GetCellInfo(arg.ColumnIndex - 1, i)

''                If cell.RowField Is arg.RowField AndAlso cell.DataField.FieldName = rsfldname Then
''                    total += Convert.ToDecimal(cell.Value)
''                End If
''            Next
''            arg.DisplayText = total.ToString("c2")
''            processingCustomCellDisplayText = False
''        End If

''    End Sub

''End Module
