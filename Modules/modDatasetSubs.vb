Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

Module modDatasetSubs
    Public Sub RejectDataset(ByVal tID As Integer)
        Try
           
                If tID = 0 Then
                App.Objects.curMainControl.masterDetailsDSet.RejectChanges()
                 
                Else
                App.Objects.curMainControl.masterDetailsDSet.Tables(tID).RejectChanges()

                End If


        Catch ex As Exception
            MsgBox("Refresh " & ex.Message)
        End Try
    End Sub

    Public Sub RefreshDataset(ByVal tID As Integer)
        Try
            If (App.Objects.curMainControl.masterDetailsDSet Is Nothing) Then

                'mallon einai server mode ara kanei refresh gia SM
                'alliws vgazei minima

                Dim fr As Integer = 0
                For Each c In App.Objects.curMainControl.xcntlMainControlLC.Controls
                    If TypeOf c Is GridControl Then
                        Dim v As GridView = c.mainview

                        If c.servermode = True Then
                            App.Objects.curMainControl.session1.DropIdentityMap()
                            c.datasource.reload()

                            v.SelectRow(0)

                        End If
                    End If
                Next
            Else
                'den einai server mode kanei kanoniko refresh
                If tID = 0 Then
                    App.Objects.curMainControl.masterDetailsDSet.Clear()
                    For i = 0 To App.Objects.curMainControl.Selectcommandcounter
                        App.Objects.curMainControl.MasterDetailsAdapter(i).Fill(App.Objects.curMainControl.masterDetailsDSet, "table" & i.ToString)
                    Next
                Else
                    App.Objects.curMainControl.masterDetailsDSet.Tables(tID).Clear()
                    App.Objects.curMainControl.MasterDetailsAdapter(tID).Fill(App.Objects.curMainControl.masterDetailsDSet, "table" & tID.ToString)

                End If

            End If
        Catch ex As Exception
            MsgBox("Refresh " & ex.Message)
        End Try
    End Sub

    Public Sub UpdateDataset(ByVal tID As Integer)

        If (App.Objects.curMainControl.masterDetailsDSet Is Nothing) Then Exit Sub

        If tID = 0 Then

            For j = 0 To App.Objects.curMainControl.Selectcommandcounter
                App.Objects.curMainControl.bsource(j).EndEdit()
            Next

            If (App.Objects.curMainControl.masterDetailsDSet.HasChanges = False) Then Exit Sub

            Try
                For i = 0 To App.Objects.curMainControl.Selectcommandcounter
                    App.Objects.curMainControl.MasterDetailsAdapter(i).Update(App.Objects.curMainControl.masterDetailsDSet, "table" & i.ToString)
                Next

            Catch ex As Exception
                Message("msgUpdateError", ex.Message)
            End Try
        Else

            App.Objects.curMainControl.bsource(tID).EndEdit()

            If (App.Objects.curMainControl.masterDetailsDSet.HasChanges = False) Then Exit Sub

            Try

                App.Objects.curMainControl.MasterDetailsAdapter(tID).Update(App.Objects.curMainControl.masterDetailsDSet, "table" & tID.ToString)


            Catch ex As Exception
                Message("msgUpdateError", ex.Message)
            End Try
        End If
    End Sub
    
    Public Function ConvertTableToRow(ByVal dt As DataTable) As DataTable

        Dim resdt As New DataTable

        Dim resrow As DataRow = resdt.NewRow

        For i As Integer = 0 To dt.Rows.Count - 1

            Dim col As New DataColumn
            col.Caption = dt.Rows(i)("Caption")
            col.ColumnName = "col" & i

            Select Case dt.Rows(i)("Type").tolower
                Case "date"
                    col.DataType = System.Type.GetType("System.DateTime")
                Case "int"
                    col.DataType = System.Type.GetType("System.Integer")
                Case Else
                    col.DataType = System.Type.GetType("System.String")

            End Select

            resdt.Columns.Add(col)


            Select Case dt.Rows(i)("Type").tolower
                Case "date"
                    resrow.Item("col" & i) = Date.Parse(dt.Rows(i)("Value"))
                Case "int"
                    resrow.Item("col" & i) = Integer.Parse(dt.Rows(i)("Value"))
                Case Else
                    resrow.Item("col" & i) = dt.Rows(i)("Value").ToString

            End Select

        Next

        resdt.Rows.Add(resrow)

        Return resdt
    End Function


End Module
