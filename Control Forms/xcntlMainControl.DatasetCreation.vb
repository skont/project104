Imports System.Data.SqlClient
Imports DevExpress.XtraEditors

Partial Public Class xcntlMainControl
    Inherits XtraForm

    Public Sub CreateDataSet()

        Dim tabs() = Split(mySelectCommand, ";")
        ReDim Preserve bsource(UBound(tabs))

        Selectcommandcounter = UBound(tabs)
        If Not mySelectCommand = "" Then

            Dim cols() = Split(myRelatedColumns, "|")
            Dim col(UBound(cols), 1) As String
            For k = 0 To UBound(col)
                Dim c() = Split(cols(k), ";")
                col(k, 0) = c(0)
                col(k, 1) = c(1)
            Next

            Dim j1() = Split(myJoins, "|")
            Dim join(UBound(j1), 1) As Integer
            For j = 0 To UBound(j1)
                Dim t() = Split(j1(j), ";")
                join(j, 0) = CInt(t(0))
                join(j, 1) = CInt(t(1))
            Next

            masterDetailsDSet = New DataSet

            For i = 0 To UBound(tabs)

                ReDim Preserve MasterDetailsAdapter(i)
                MasterDetailsAdapter(i) = New SqlDataAdapter(tabs(i), GetCon)
                MasterDetailsAdapter(i).Fill(masterDetailsDSet, "table" & i.ToString)

                ReDim Preserve masterdetailsCMBuilder(i)
                masterdetailsCMBuilder(i) = New SqlCommandBuilder(MasterDetailsAdapter(i))

                bsource(i) = New BindingSource
                If i = 0 Then
                    bsource(i).DataSource = masterDetailsDSet.Tables(i)
                Else

                    Dim keycol As DataColumn = masterDetailsDSet.Tables(join(i, 0)).Columns(col(i, 0))
                    Dim fkeycol As DataColumn = masterDetailsDSet.Tables(join(i, 1)).Columns(col(i, 1))

                    'Dim foreignKeyConstraint As ForeignKeyConstraint
                    'foreignKeyConstraint = New ForeignKeyConstraint("Constraint" & i.ToString, keycol, fkeycol)
                    'foreignKeyConstraint.UpdateRule = Rule.Cascade

                    masterDetailsDSet.Relations.Add("Relmdg" & i.ToString, keycol, fkeycol, False)



                    bsource(i).DataMember = "Relmdg" & i.ToString

                    bsource(i).DataSource = bsource(join(i, 0))

                End If

            Next
        End If



        getControls(xcntlMainControlLC, bsource, masterDetailsDSet)


    End Sub

End Class
