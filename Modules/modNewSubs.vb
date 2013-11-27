Imports System.Data.SqlClient
Imports DevExpress.XtraEditors
Imports Microsoft.SqlServer.Management.Smo
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid
Imports System.IO
Imports DevExpress.XtraLayout
Imports Microsoft.SqlServer.Management.Common
Imports DevExpress.Data
Imports System.Globalization


Module modNewSubs

    Public Sub WriteLogEntry(ByVal msg As String)
        Using sw As New StreamWriter(String.Format("{0}\{1}", Application.StartupPath, My.Settings.Caption & ".log"), True)
            Dim dateValue As Date = Now
            sw.WriteLine(String.Format("/* {0} */ {1}", dateValue.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), msg))
            'sw.WriteLine(String.Format("/* {0} {1} */ {2}", Now.ToShortDateString, Now.ToShortTimeString, msg))
            sw.Close()
        End Using
    End Sub

    Public Function RemoveLastDelimiter(ByVal input As String) As String
        Dim output As String = input.Substring(0, IIf(input.Length <= 0, 0, input.Length - 1))
        Return output
    End Function

    Public Function StoreValues(ByVal f As xcntlMainControl,
                                ByVal controlname As String,
                                ByVal eventname As String
                                ) As String()


        Dim dt0 As DataTable = UP1(f, controlname, eventname, "get")
        'Dim q(My.Settings.up1params) As String
        Dim q(dt0.Rows.Count - 1) As String

        Dim i As Integer = 0


        For i = 0 To dt0.Rows.Count - 1
            If dt0.Rows(i)("act").tolower = "fetch" OrElse dt0.Rows(i)("act").tolower = "getvalues" Then

                Dim f0 As String = ""
                Dim f1 As String = ""


                f0 = dt0.Rows(i)("f0")

                Dim frm As xcntlMainControl
                If f0.StartsWith("prev.") Then
                    frm = App.Collections.MainControl(App.Objects.curMainControl.myParentGuid)
                    f0 = Replace(f0, "prev.", "")
                    dt0.Rows(i)("f0") = f0
                Else
                    frm = f
                End If


                If f0.ToString.ToLower.Contains("prop.") = False Then

                    Dim cn = Split(f0, ".")
                    Select Case UBound(cn)

                        Case 0 'einai tis morfis PEDIO
                            q(i) = GetVal_0(frm, dt0.Rows(i))

                        Case 2 'einai tis morfis GRIDCONTROL.GRIDVIEW.PEDIO
                            q(i) = GetVal_2(frm, dt0.Rows(i), controlname)

                        Case 3 ' einai tis morfis sum.mdg1.gv1.POSO
                            q(i) = GetVal_3(frm, dt0.Rows(i))

                    End Select

                Else ' periexei prop
                    q(i) = GetVal_Prop(frm, dt0.Rows(i))

                End If

            ElseIf dt0.Rows(i)("act").tolower = "getfileconts" Then
                q(i) = readText(dt0.Rows(i)("f0"), dt0.Rows(i)("f1"), dt0.Rows(i)("f2"))

            ElseIf dt0.Rows(i)("act").tolower = "find" Then

                Dim f0 As String = ""
                Dim f1 As String = ""
                Dim what As String = ""

                f0 = dt0.Rows(i)("f0") '<-- where
                f1 = dt0.Rows(i)("f1") '<-- what(control)

                Dim frm As xcntlMainControl
                If f0.StartsWith("prev.") Then
                    frm = App.Collections.MainControl(App.Objects.curMainControl.myParentGuid)
                    f0 = Replace(f0, "prev.", "")
                    dt0.Rows(i)("f0") = f0
                Else
                    frm = f
                End If


                dt0.Rows(i)("f0") = f1
                dt0.Rows(i)("f1") = "nvarchar"

                If f1.ToString.ToLower.Contains("prop.") = False Then

                    Dim cn = Split(f1, ".")
                    Select Case UBound(cn)

                        Case 0 'einai tis morfis PEDIO
                            what = GetVal_0(frm, dt0.Rows(i))

                        Case 2 'einai tis morfis GRIDCONTROL.GRIDVIEW.PEDIO
                            what = GetVal_2(frm, dt0.Rows(i), controlname)

                        Case 3 ' einai tis morfis sum.mdg1.gv1.POSO
                            what = GetVal_3(frm, dt0.Rows(i))

                    End Select


                Else ' periexei prop
                    what = GetVal_Prop(frm, dt0.Rows(i))

                End If

                q(i) = Find(frm, where:=f0, what:=what).ToString

            ElseIf dt0.Rows(i)("act").tolower.startswith("getlistparams") Then

                Dim frm As xcntlMainControl
                Dim f0 As String = dt0.Rows(i)("f0")

                If f0.StartsWith("prev.") Then
                    frm = App.Collections.MainControl(App.Objects.curMainControl.myParentGuid)
                    f0 = Replace(f0, "prev.", "")
                    dt0.Rows(i)("f0") = f0
                Else
                    frm = f
                End If

                q(i) = GetListParams(frm, dt0.Rows(i), dt0.Rows(i)("act").tolower)


            End If

        Next

        Return q

    End Function

    Private Function GetListParams(ByVal frm As xcntlMainControl, ByVal r As DataRow, ByVal type As String)
        Dim res As String = ""
        Dim cnt() As String = Split(r("f0"), ".")
        Dim gridc As GridControl = frm.Controls.Find(cnt(0), True)(0)


        For Each gridv As GridView In gridc.Views
            If gridv.Name = cnt(1) Then

                For i As Integer = 0 To gridv.RowCount - 1


                    If Not gridv.GetRowCellValue(i, cnt(2)) Is Nothing Then
                        Dim fieldvalue As String = ""

                        If type = "getlistparamssp" Then
                            fieldvalue = gridv.GetRowCellValue(i, cnt(2))
                            If gridv.GetRowCellValue(i, "type") = "d" Then

                                Dim f = Split(fieldvalue, "/")
                                If Len(f(0)) = 1 Then f(0) = "0" & f(0)
                                If Len(f(1)) = 1 Then f(1) = "0" & f(1)

                                Dim fres = String.Format("#{2}{1}{0}", f(0), f(1), f(2))

                                fieldvalue = fres
                            End If

                        ElseIf type = "getlistparamsrpt" Then
                            fieldvalue = gridv.GetRowCellValue(i, "type") & "|" & gridv.GetRowCellValue(i, cnt(2))
                        End If


                        If i = 0 Then
                            res = fieldvalue
                        Else
                            res = res & ";" & fieldvalue
                        End If

                    End If
                Next

                gridv.SelectRow(gridv.FocusedRowHandle)
            End If
        Next



        Return res
    End Function


    Private Function GetVal_0(ByVal frm As xcntlMainControl, ByVal r As DataRow)
        Dim res = Nothing
        Dim o As Object = Nothing


        Try
            o = frm.Controls.Find(r("f0"), True)(0)

            If r("f1") = "datetime" Then
                res = ParseDate(o.editvalue)
            Else
                If TypeOf o Is CheckEdit Then
                    res = CInt(o.Checked)
                Else
                    res = Null2(r("f1"), o.editvalue)
                End If

            End If

        Catch ex As Exception
            ' se periptwsi pou den vrie to control epistrefei tin timi pou exei sto fetch

            res = r("f0")
            WriteLogEntry("Can Not Find: " & r("f0"))
        End Try



        Return res
    End Function


    Private Function GetVal_2(ByVal frm As xcntlMainControl,
                              ByVal r As DataRow,
                              ByVal controlname As String)

        Dim res = Nothing

        Try
            Dim p() As String = Split(r("f0"), ".")
            Dim ctrlvalue = Nothing

            Dim row As Integer = 0

            If App.Objects.curFocusedGridControl IsNot Nothing Then
                If App.Objects.curFocusedGridControl.Name = p(0) Then

                    If App.Objects.curFocusedView.name = p(1) Then
                        row = App.Objects.curFocusedView.focusedrowhandle

                        If r("f0") = controlname Then
                            If Not App.Objects.curFocusedView.ActiveEditor Is Nothing Then
                                ctrlvalue = App.Objects.curFocusedView.ActiveEditor.EditValue
                            Else
                                ctrlvalue = App.Objects.curFocusedView.GetRowCellValue(row, p(2))
                            End If

                        Else
                            ctrlvalue = App.Objects.curFocusedView.GetRowCellValue(row, p(2))
                        End If


                    ElseIf App.Objects.curFocusedView.name <> p(1) Then
                        For Each gv In App.Objects.curFocusedGridControl.Views
                            If gv.name = p(1) Then
                                row = App.Objects.curFocusedView.sourcerowhandle
                                ctrlvalue = gv.getrowcellvalue(row, p(2))
                            End If
                        Next

                    End If

                    res = ctrlvalue

                Else
                    ' kanonika ayto den prokeitai na symvei pote !

                    Dim g As GridControl = frm.controls.find(p(0), True)(0)
                    For Each gv In g.Views
                        If Not gv.IsFilterRow(gv.FocusedRowHandle) Then
                            If gv.Name = p(1).ToString Then
                                row = gv.focusedrowhandle

                                If r("f0") = controlname Then
                                    ctrlvalue = gv.ActiveEditor.EditValue
                                Else
                                    ctrlvalue = gv.GetRowCellValue(row, p(2))
                                End If

                            End If
                        End If
                    Next

                    res = ctrlvalue
                End If

            Else
                ' curFocusGridControl IS NOTHING
                ' oute ayto prepei na symvei pote 
                Dim g As GridControl = frm.controls.find(p(0), True)(0)
                For Each gv In g.Views
                    If Not gv.IsFilterRow(gv.FocusedRowHandle) Then
                        If gv.Name = p(1).ToString Then
                            row = gv.focusedrowhandle
                            ctrlvalue = gv.GetRowCellValue(row, p(2))

                        End If
                    End If
                Next

                res = ctrlvalue

            End If


            If r("f1") = "datetime" Then
                res = ParseDate(ctrlvalue)
            Else
                res = Null2(r("f1"), ctrlvalue)
            End If

        Catch ex As Exception

            'res = r("f0")
            WriteLogEntry("Can Not Find: " & r("f0") & "-" & ex.Message)

        End Try


        Return res

    End Function


    ''----- TEST DATE
    'Dim MyCultureInfo As CultureInfo = New CultureInfo("el-GR")
    'Dim MyString As String = "31/03/2010"
    'Dim MyDateTime As DateTime = DateTime.Parse(MyString, MyCultureInfo)
    'WriteLogEntry(MyDateTime)
    ''---------------

    Public Function ParseDate(Optional ByVal inDate As Object = Nothing) As Object
        Dim culture As New CultureInfo("el-GR")
        Dim res As Object = Nothing

        If TypeOf inDate Is Date Then
            Dim dt As Date = inDate
       
                res = dt.ToString("yyyyMMdd", culture)



        ElseIf TypeOf inDate Is String Then
            If inDate = "" Then
                res = DBNull.Value
            Else
                res = Date.Parse(inDate, culture)
            End If


        End If

        Return res
    End Function

    Private Function GetVal_3(ByVal frm As xcntlMainControl, ByVal r As DataRow)
        Dim res As Object = Nothing
        Dim gc As GridControl = Nothing
        Dim p() As String = Split(r("f0"), ".")

        Try
            gc = frm.Controls.Find(p(1), True)(0)
        Catch ex As Exception
           
        End Try

        If Not gc Is Nothing Then
            For Each gv In gc.Views
                If gv.name = p(2).ToString Then
                    Select Case p(0)

                        Case "sum"
                            ' Epistrefei to athroisma twn timwn mias stilis
                            Dim sumit As GridSummaryItem = gv.Columns(p(3)).SummaryItem
                            sumit.SummaryType = SummaryItemType.Sum
                            Dim s As Double = Convert.ToDouble(sumit.SummaryValue)
                            res = s

                        Case "count"
                            ' Epistrefei ton arithmo twn grammwn tou grid
                            res = gv.RowCount

                        Case "multi"
                            Dim tmp As String = ""

                            If gv.GetSelectedRows() Is Nothing Then res = ""
                            For Each rs As Integer In gv.GetSelectedRows()
                                tmp += gv.GetRowCellValue(rs, p(3)) & ";"
                            Next

                            res = RemoveLastDelimiter(tmp)

                        Case "table"
                            Dim tmpRows As String = ""
                            Dim cols() As String = Split(p(3), ";")

                            For j As Integer = 0 To gv.rowcount - 1 'To thelei gia kapoio logo gia na metrisei swsta tis grammes alla den xerw giati 

                                Dim tmpCols As String = ""
                                For i As Integer = 0 To UBound(cols)
                                    tmpCols += gv.getrowcellvalue(j, cols(i)) & ";"
                                Next

                                tmpRows += RemoveLastDelimiter(tmpCols) & "|"
                            Next

                            res = RemoveLastDelimiter(tmpRows)

                    End Select

                End If
            Next

        ElseIf gc Is Nothing Then
            Try
                If p(0) = "table" Then
                    Dim tmpRows As String = ""
                    Dim cols() As String = Split(p(3), ";")

                    If p(1) = "data" Then
                        Dim tid As Integer = Convert.ToInt16(p(2))

                        Dim tmpds As New DataSet
                        tmpds = frm.masterDetailsDSet.Copy
                        tmpds.AcceptChanges()

                        Dim tbl As DataTable = tmpds.Tables(tid)

                        If tbl.Rows.Count - 1 >= 0 Then

                            For t As Integer = 0 To tbl.Rows.Count - 1
                                Dim tmpCols As String = ""

                                For i As Integer = 0 To UBound(cols)
                                    tmpCols += tbl.Rows(t)(cols(i)) & ";"
                                Next

                                tmpRows += RemoveLastDelimiter(tmpCols) & "|"
                            Next
                        End If

                        res = RemoveLastDelimiter(tmpRows)

                        'tbl.Dispose()
                        tmpds.Dispose()
                    End If

                End If


            Catch ex As Exception
                WriteLogEntry("Error In table values: " & ex.Message)
                'WriteLogEntry(String.Format("table {0} does not exist or is empty ", p(2)))
            End Try

        Else
                res = r("f0")

        End If

        Return res


    End Function


    Private Function GetVal_Prop(ByVal frm As xcntlMainControl, ByVal r As DataRow)

        Dim res = Nothing
        Try
            Dim p() As String = Split(r("f0"), "prop.")
            Dim ctr = Split(p(0), ".")

            Dim o As Object = Nothing

            If p(0) = "" Then
                o = frm
            Else
                o = frm.Controls.Find(ctr(0), True)(0)
            End If


           if p(1).ToLower="haschanges"

                If Not o.masterdetailsDSet Is Nothing Then
                    For count As Integer = 0 To o.Selectcommandcounter
                        o.bsource(count).EndEdit()
                    Next
                End If

                res = o.masterDetailsDSet().HasChanges()

            Else
                Dim prop() As String = Split(p(1), ".")
                Dim tmp As String

                Select Case UBound(prop)
                    Case 0
                        tmp = prop(0)
                        If tmp.StartsWith("myP") Then
                            res = CallByName(o, "myControlsToUpdate", CallType.Get)

                            Dim pr = Split(res, ";")
                            Dim id As Integer = Convert.ToInt16(Replace(tmp, "myP", ""))

                            If id <= UBound(pr) Then
                                res = pr(id)
                            End If


                        Else
                            res = CallByName(o, tmp, CallType.Get)
                        End If


                    Case 1
                        tmp = prop(1)
                        res = CallByName(o.properties, tmp, CallType.Get)

                    Case 2
                        If TypeOf o Is GridControl Then
                            For Each v In o.views
                                If v.name = ctr(1).ToString Then
                                    tmp = prop(1)
                                    ' An den kanw lathos ayto prepei na ginei:
                                    'res = CallByName(o.OptionsBehavior, tmp, CallType.Get)

                                    res = CallByName(o.properties, tmp, CallType.Get)

                                End If
                            Next

                        End If
                End Select
            End If


        

        Catch ex As Exception
            res = r("f0")
            WriteLogEntry("Can Not Find: " & r("f0"))
        End Try

        Return res
    End Function

    Private Sub SetVal_0(ByVal frm As xcntlMainControl, ByVal r As DataRow)

        Dim o As Object = frm.Controls.Find(r("f0"), True)(0)
        If r("f1") = "datetime" Then
            o.editvalue = ParseDate(r("f2"))
        Else
            If TypeOf o Is CheckEdit Then
                o.Checked = CBool(r("f2"))
            Else
                o.editvalue = Null2(r("f1"), r("f2"))
            End If

        End If

    End Sub

    Private Sub SetVal_2(ByVal frm As xcntlMainControl, ByVal r As DataRow, ByVal PU As Boolean)

        Dim p() As String = Split(r("f0"), ".")
        Dim g As GridControl = frm.Controls.Find(p(0), True)(0)


        For Each v In g.Views
            If v.Name = p(1).ToString Then
                Select Case r("f1")
                    Case "datetime"
                        v.HideEditor()
                        v.SetRowCellValue(v.FocusedRowHandle, p(2), ParseDate(r("f2")))

                    Case Else
                        If PU = True Then
                            v.EditingValue = Null2(r("f1"), r("f2"))
                        ElseIf PU = False Then
                            v.HideEditor()
                            v.SetRowCellValue(v.FocusedRowHandle, p(2), Null2(r("f1"), r("f2")))

                        End If

                End Select
            End If
        Next

    End Sub

    Private Sub SetVal_Prop(ByVal frm As xcntlMainControl, ByVal r As DataRow)

        Dim p() As String = Split(r("f0"), "prop.")
        Dim o As Object = Nothing

        Dim ctr() As String = Split(p(1), ".")

        If p(0) = "" Then
            o = frm
        Else
            o = frm.Controls.Find(ctr(0), True)(0)
        End If

        Select Case UBound(ctr)
            Case 0
                CallByName(o, ctr(0), CallType.Set, CBool(r("f2")))
            Case 1
                CallByName(o.properties, ctr(1), CallType.Set, CBool(r("f2")))
            Case 2
                If TypeOf o Is GridControl Then
                    For Each v In o.views
                        If v.name = ctr(1).ToString Then
                            CallByName(o.properties, ctr(1), CallType.Set, CBool(r("f2")))

                        End If
                    Next
                End If

                'Ayto kanei editable i oxi
                'Case 2
                '    If TypeOf o Is GridControl Then
                '        For Each v In o.views
                '            If v.name = ctr(1).ToString Then
                '                CallByName(v.OptionsBehavior, ctr(2), CallType.Set, CBool(r("f2")))
                '                If r("f2") = 1 Then v.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom
                '                If r("f2") = 0 Then v.OptionsView.NewItemRowPosition = NewItemRowPosition.None
                '            End If
                '        Next


                'End If
        End Select
    End Sub

    Public Sub Set_Update(ByVal frm As xcntlMainControl, ByVal r As DataRow, Optional ByVal PU As Boolean = False)



        If r("f0").ToLower.Contains("prop.") = False Then
            Dim cn1 = Split(r("f0"), ".")
            Select Case UBound(cn1)

                Case 0 ' einai tis morfis PEDIO
                    SetVal_0(frm, r)
                Case 2 ' einai tis morfis gridcontrol.gridview.PEDIO
                    SetVal_2(frm, r, PU)
            End Select

        Else
            'Periexei prop

            SetVal_Prop(frm, r)

        End If

    End Sub

    '------------------------

    Public Sub ChangeDsGridView(ByVal f As xcntlMainControl, ByVal f0 As String, ByVal f2 As String)
        If f0 <> "" Then

            Dim gc As GridControl = f.Controls.Find(f0, True)(0)

            Try

                Dim da As New SqlDataAdapter(f2, GetCon)
                Dim ds As New DataSet

                da.Fill(ds)

                gc.DataSource = ds.Tables(0)

                da.Dispose()
                ds.Dispose()
            Catch ex As Exception

            End Try
        End If
    End Sub


    Public Function UP1(ByVal frm As xcntlMainControl, ByVal ctrl As String, ByVal evnt As String, ByVal inputaction As String, ByVal ParamArray query() As String) As DataTable


        Dim frmname As String = ""
        Dim sp2run As String = "" ' = "up1"
        Dim criteria As String = ""
        Dim controlname As String = ""


        If ctrl.StartsWith("nvb;") OrElse ctrl.StartsWith("mnu;") Then
            Dim menu() As String = Split(ctrl, ";")

            If menu(0) = "nvb" Then
                sp2run = "up1nvb"
                frmname = "nvb"
                controlname = menu(1)

            ElseIf menu(0) = "mnu" Then
                sp2run = "up1mnu"
                frmname = "mnu"
                controlname = menu(1)
            End If


        ElseIf ctrl = "" OrElse evnt.ToLower.StartsWith("close") Then
            Dim sp() As DataRow = frm.objectPropertiesTable.Select()
            sp2run = "up1" + sp(0)("obj_behavior")
            frmname = frm.myName
            controlname = ctrl
        Else
            Dim cnt() As String = Split(ctrl, ".")
            If cnt.Length - 1 = 0 Then
                criteria = String.Format("ctrlName ='{0}' ", cnt(0))
            Else
                criteria = String.Format("ctrlName ='{0}' ", cnt(1))
            End If

            Dim sp() As DataRow = frm.objectDetailsPropertiesTable.Select(criteria)
            sp2run = sp(0)("Behavior")
            frmname = frm.myName
            controlname = ctrl
        End If

        If sp2run = "up1inv" Then sp2run = "up1k"

        If inputaction = "get" Then inputaction = "getvalues"

        Dim s As String = String.Format("exec {0} '{1}','{2}','{3}','{4}'", sp2run, frmname, controlname, evnt, inputaction)
        Dim up1_cm As String = ""

        If query Is Nothing Then

        Else

            For i = 0 To UBound(query)

                s = String.Format("{0},'{1}'", s, query(i))

            Next

        End If

        WriteLogEntry(s)


        Dim up1_da As New SqlDataAdapter(s, GetCon)
        Dim up1_ds As New DataSet

        up1_da.Fill(up1_ds)

        up1_da.Dispose()
        up1_ds.Dispose()

        Dim res As DataTable = Nothing
        Try
            res = up1_ds.Tables(0)
        Catch ex As Exception
            WriteLogEntry("UP1: No Table returned " & ex.Message)
        End Try

        Return res

    End Function

    Public Function CreateObjectPropertiesTable(ByVal frm As xcntlMainControl) As DataTable

        Dim s As String = String.Format("select * from Objects where obj_Name='{0}'", frm.myName)
        WriteLogEntry("Executes: " & s)


        Dim ctrlda As New SqlDataAdapter(s, GetSysCon)
        Dim ctrlds As New DataSet


        ctrlda.Fill(ctrlds)

        ctrlda.Dispose()
        ctrlds.Dispose()

        Return ctrlds.Tables(0)

    End Function

    Public Function CreateObjectDetailsPropertiesTable(ByVal frm As xcntlMainControl, ByVal viewmode As String, ByVal ParamArray prm() As String) As DataTable

        Dim formname As String = frm.myName

        Dim s As String = String.Format("exec up1getctrls '{0}','{1}'", formname, viewmode)
        WriteLogEntry("Executes: " & s)

        If prm Is Nothing Then

        Else
            For i = 0 To UBound(prm)
                If prm(i).StartsWith(Chr(34)) Then
                    s = String.Format("{0}, {1}", s, prm(i))
                Else
                    s = String.Format("{0}, '{1}'", s, prm(i))
                End If
            Next

        End If


        Dim ctrlda As New SqlDataAdapter(s, GetSysCon)
        Dim ctrlds As New DataSet


        ctrlda.Fill(ctrlds)

        ctrlda.Dispose()
        ctrlds.Dispose()

        Return ctrlds.Tables(0)

    End Function

    Public Function CreateContextMenuStripTable(ByVal MenuName As String, ByVal ViewMode As String) As DataTable

        Dim s As String = String.Format("exec up1getShortMenu '{0}','{1}'", MenuName, ViewMode)

        WriteLogEntry(s)

        Dim ctrlda As New SqlDataAdapter(s, GetSysCon)
        Dim ctrlds As New DataSet


        ctrlda.Fill(ctrlds)

        ctrlda.Dispose()
        ctrlds.Dispose()

        Return ctrlds.Tables(0)

    End Function

    Public Function Null2(ByVal Type As String, ByVal value As Object)
        Dim res As Object = Nothing

        If value Is DBNull.Value Then
            Select Case Type.ToLower
                Case "float", "bit"
                    res = 0
                Case Else
                    res = ""
            End Select

        Else
            If Type = "bit" Then
                res = CBool(value)
            Else
                res = value
            End If

        End If

        Return res
    End Function


    Public Sub RunSP(ByVal r As String)

        Using conn As New SqlConnection(GetCon)
            Dim srv As New Server(New ServerConnection(conn))
            Try
                srv.ConnectionContext.ExecuteNonQuery(r)

                WriteLogEntry("RunSP: " & r)
            Catch ex As Exception
                WriteLogEntry("RunSP Exception: " & ex.Message)
            End Try
        End Using

    End Sub


    Public Sub AddRemoveRows(ByVal r As DataRow, ByVal v As GridView)


        Dim tableID As Integer = 0
        Dim relationID As Integer = 0

        Try
            tableID = Convert.ToInt16(r("f0"))
            relationID = Convert.ToInt16(r("f1"))
        Catch ex As Exception
            WriteLogEntry("remove all error" & ex.ToString)
            tableID = 0
            relationID = 0
        End Try


        Dim view As New GridView
        Try
            view = v.GetDetailView(v.FocusedRowHandle, relationID)
            If view Is Nothing Then view = v

        Catch ex As Exception
            WriteLogEntry("view can not be found")
            view = v
        End Try


        Select Case r("act").tolower
            Case "remove_all"
                Try
                    'prepei na to dw
                    'If view.RowCount <= 1 Then Exit Sub

                    view.OptionsSelection.MultiSelect = True
                    view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect
                    view.SelectAll()
                    view.DeleteSelectedRows()

                    view.OptionsSelection.MultiSelect = False
                    view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect


                Catch ex As Exception
                    WriteLogEntry("remove all error: " & ex.Message)
                End Try
            Case "remove_currentrow"
                view.DeleteRow(view.FocusedRowHandle)

            Case "add_newrow"

                'Add a new row
                view.AddNewRow()

                'Get the handle of the new row
                Dim newRowHandle = view.FocusedRowHandle

                Dim s() As String = Split(r("f2"), "|")

                For i As Integer = 0 To UBound(s)
                    Dim fld() = Split(s(i), ";")

                    For j As Integer = 0 To UBound(fld)

                        Dim f = view.Columns(fld(0))
                        Dim val = fld(1)
                        view.SetRowCellValue(newRowHandle, f, val)

                    Next

                Next

                'Accept the new row
                'The row moves to a new position according to the current group settings

                view.UpdateCurrentRow()


        End Select

    End Sub


    Public Sub ctr_Refresh(ByVal form As Object, ByVal ctrlname As String)
        Try
            Dim ctr As Object = form.Controls.Find(ctrlname, True)(0)
            Dim dets() As String = Split(ctr.Tag, ".")

            Dim cm As String = dets(0)
            Dim da As New SqlDataAdapter(cm, GetCon)
            Dim ds As New DataSet

            da.Fill(ds)
            da.Dispose()
            ds.Dispose()


            ctr.Properties.DataSource = Nothing
            ctr.Properties.DataSource = ds.Tables(0)
            ctr.Properties.DisplayMember = dets(1)
            ctr.Properties.ValueMember = dets(2)
        Catch ex As Exception

        End Try
    End Sub


    Public Function Find(ByVal f As xcntlMainControl, ByVal what As String, ByVal where As String) As Boolean
        Dim res As Boolean = False

        Dim s = Split(where, ".")

        Dim gc As GridControl = f.Controls.Find(s(0).ToString, True)(0)
        For Each gv In gc.Views
            If gv.name = s(1).ToString Then
                Dim rowHandle As Integer = GetRowHandleByColumnValue(gv, SearchValue:=what, ColumnFieldName:=s(2))
                If rowHandle <> GridControl.InvalidRowHandle Then
                    res = True
                End If

            End If
        Next

        Return res

    End Function


    Public Sub FindAnd(ByVal f As xcntlMainControl, ByVal r As DataRow, ByVal opt As String)

        Dim s = Split(r("f0"), ".")

        Dim gc As GridControl = f.Controls.Find(s(0).ToString, True)(0)
        For Each gv In gc.Views
            If gv.name = s(1).ToString Then
                Dim rowHandle As Integer = GetRowHandleByColumnValue(gv, SearchValue:=r("f1"), ColumnFieldName:=s(2))
                If rowHandle <> GridControl.InvalidRowHandle Then

                    'to original elege na kanei focus sto r("f2") 
                    'nomizw den exeio noima
                    'gia ayto to evgala. 
                    SetFocus(f, r("f0"), rowHandle, opt)

                End If

            End If
        Next


    End Sub


    Public Sub SetFocus(ByVal f As xcntlMainControl, ByVal ctrl As String, Optional ByVal Rowhandle As Integer = 0, Optional ByVal FocusWhatOnGrid As String = "value")

        Dim s = Split(ctrl, ".")
        Dim o As Object = f.Controls.Find(s(0), True)(0)

        Select Case UBound(s)
            Case 0
                ' ayti i allagi egine giati otan allaxa to xcntlmaincontrol apo user control se forma 
                ' den ekane focus me to setfocus etsi douleyei kai vlepoume (28/2/11)

                'o.focus()
                f.ActiveControl = o

            Case 2
                For Each v In o.views
                    If v.name = s(1) Then
                        If FocusWhatOnGrid = "value" Then
                            v.FocusedColumn = v.Columns.ColumnByFieldName(s(2))
                            v.FocusedRowHandle = Rowhandle
                            v.ShowEditor()
                        ElseIf FocusWhatOnGrid = "row" Then
                            v.FocusedRowHandle = Rowhandle
                        End If

                    End If

                Next

        End Select


    End Sub


    Public Function GetRowHandleByColumnValue(ByVal view As GridView, ByVal ColumnFieldName As String, ByVal SearchValue As Object) As Integer
        Dim result As Integer = GridControl.InvalidRowHandle
        Try
            For i As Integer = 0 To view.RowCount - 1
                If view.GetDataRow(i)(ColumnFieldName).Equals(SearchValue) Then
                    result = i
                End If
            Next
        Catch ex As Exception


        End Try
        Return result
    End Function

    Public Function FindControl(ByVal f As xcntlMainControl, ByVal ctrl As String) As Object
        Dim res As Object = Nothing

        Dim s = Split(ctrl, ".")
        Dim o As Object = f.Controls.Find(s(0), True)(0)

        Select Case UBound(s)
            Case 0
                res = o
            Case 1
                For Each v In o.views
                    If v.name = s(1) Then
                        res = v

                    End If

                Next

        End Select


        Return res
    End Function

    Public Function CheckForValidation(ByVal frm As xcntlMainControl, ByVal controlname As String) As Boolean
        Dim res As Boolean = False

        Dim dt As DataTable = UP1(frm, controlname, "", "ChkCtrl")
        Dim val = IIf(dt.Rows(0)("f0") Is DBNull.Value, 0, dt.Rows(0)("f0"))

        res = CBool(val)

        Return res
    End Function


    Public Sub SignInvoice(ByVal r As DataRow)

        Dim rp As New clsRepGen
        Dim paips As String = ""

        rp.Print2File(r("f2"), r("aa"))
        SignFile(r("f0"), r("f1"))


        Dim objReader As New System.IO.StreamReader(Application.StartupPath & My.Settings.OutputFile)

        Do While objReader.Peek() <> -1
            paips = objReader.ReadLine()
        Loop

        objReader.Dispose()

        Dim q As String = "'" & r("aa") & "'" & "," & "'" & paips & "'" & "," & 0
        ' m = spExec("upInvUpdateEAFDSS", q)

        RunSP(String.Format("exec upInvUpdateEAFDSS {0}", q))

    End Sub



    Public Function GetCellEditors(ByVal GridViewName As String, ByVal FormName As String) As String
        Dim res As String = ""

        Dim com As String = String.Format("exec up1GetCellEditors '{0}','{1}'", GridViewName, FormName)
        WriteLogEntry(com)

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter(com, GetCon)

        da.Fill(ds)

        da.Dispose()
        ds.Dispose()

        res = ds.Tables(0).Rows(0)("CellEditors")

        Return res
    End Function

    '--------------------------------------

    Public Sub GridVisible(ByVal f As xcntlMainControl, ByVal cnt As String, ByVal value As String)
        Dim c() As String = Split(cnt, ".")

        For Each it In f.xcntlMainControlLC.Items

            If TypeOf it Is LayoutControlItem Then
                If it.control.name = c(0) Then
                    If value = "true" Then
                        it.ContentVisible = True
                    Else
                        it.contentvisible = False
                    End If
                End If
            End If
        Next
    End Sub


    Public Sub GridReadOnly(ByVal f As xcntlMainControl, ByVal cnt As String, ByVal value As String)
        Dim c() As String = Split(cnt, ".")

        Dim gControl As Object = f.Controls.Find(c(0), True)(0)

        If TypeOf gControl Is GridControl Then
            For Each gView In gControl.views
                If gView.name = c(1) Then
                    If value = "true" Then
                        'gView.OptionsBehavior.ReadOnly = True
                        gView.OptionsBehavior.Editable = False

                    Else
                        'gView.OptionsBehavior.ReadOnly = False
                        gView.OptionsBehavior.Editable = True
                        gView.OptionsView.NewItemRowPosition = NewItemRowPosition.None
                    End If
                End If
            Next
        End If
    End Sub

    Public Sub GridColumnReadOnly(ByVal f As xcntlMainControl, ByVal cnt As String, ByVal value As String)
        Dim c() As String = Split(cnt, ".")

        Dim gControl As Object = f.Controls.Find(c(0), True)(0)

        If TypeOf gControl Is GridControl Then
            For Each gView In gControl.views
                If gView.name = c(1) Then
                    If value = "true" Then
                        gView.Columns(c(2).ToString).OptionsColumn.ReadOnly = True
                    Else
                        gView.Columns(c(2).ToString).OptionsColumn.ReadOnly = False

                    End If
                End If
            Next
        End If

    End Sub

    Public Sub GridColumnVisible(ByVal f As xcntlMainControl, ByVal cnt As String, ByVal value As String)
        Dim c() As String = Split(cnt, ".")

        Dim gControl As Object = f.Controls.Find(c(0), True)(0)

        If TypeOf gControl Is GridControl Then
            For Each gView In gControl.views
                If gView.name = c(1) Then
                    If value = "true" Then
                        gView.Columns(c(2).ToString).Visible = True
                    Else
                        gView.Columns(c(2).ToString).Visible = False


                    End If
                End If
            Next
        End If

    End Sub


    Public Sub GridNewRowPossition(ByVal f As xcntlMainControl, ByVal cnt As String, ByVal value As String)

        Dim c() As String = Split(cnt, ".")

        Dim gControl As Object = f.Controls.Find(c(0), True)(0)

        If TypeOf gControl Is GridControl Then
            For Each gView In gControl.views
                If gView.name = c(1) Then
                    If gView.optionsbehavior.editable = True Then
                        If value = "bottom" Then
                            gView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom
                        ElseIf value = "top" Then
                            gView.OptionsView.NewItemRowPosition = NewItemRowPosition.Top
                        Else
                            gView.OptionsView.NewItemRowPosition = NewItemRowPosition.None
                        End If
                    End If
                End If
            Next
        End If

    End Sub


    Public Sub ControlVisible(ByVal f As xcntlMainControl, ByVal cnt As String, ByVal value As String)

        Try
            For Each it In f.xcntlMainControlLC.Items

                If TypeOf it Is LayoutControlItem Then
                    If Not TypeOf it Is EmptySpaceItem Then
                        If it.Control.Name = cnt Then
                            If value = "true" Then
                                it.ContentVisible = True
                            Else
                                it.ContentVisible = False
                            End If
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            'WriteLogEntry(ex.Message)
        End Try

    End Sub

    Public Sub ControlReadOnly(ByVal f As xcntlMainControl, ByVal cnt As String, ByVal value As String)

        Dim cntl As Object = f.Controls.Find(cnt, True)(0)

        If value = "true" Then
            cntl.properties.readonly = True
        Else
            cntl.properties.readonly = False
        End If

    End Sub

    Public Sub ButtonToggle(ByVal f As xcntlMainControl, ByVal cnt As String, ByVal value As String)
        Dim btn As SimpleButton = f.Controls.Find(cnt, True)(0)

        If value = "true" Then
            btn.Enabled = True
        Else
            btn.Enabled = False
        End If

    End Sub

End Module
