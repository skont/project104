' Ayto xrisimopoieitai gia na typwthei to parastatiko
' isws twra den xreiazetai na einai class ...


Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Globalization

Public Class clsRepGen

    Dim startPrinter As Integer
    Dim endPrinter As Integer
    Dim seclines(4) As Integer
    Dim query As String = String.Empty
    Dim aa As Integer

    Dim dasp As SqlDataAdapter
    Dim dssp As DataSet

    Dim detcount As Integer

    Public Sub Print2File(ByVal ReportName As String, ByVal Params As Integer)

        ' Create output file directory

        If Directory.Exists(Application.StartupPath & "\Tmp\") = False Then
            Directory.CreateDirectory(Application.StartupPath & "\Tmp\")
        End If

        Dim OutputFname As String = Application.StartupPath & My.Settings.InputFile

        GetReportParams(ReportName)

        Dim cmsp = String.Format("exec {0} {1}", query, Params)
        dasp = New SqlDataAdapter(cmsp, GetCon)
        dssp = New DataSet

        dasp.Fill(dssp)

        detcount = dssp.Tables(0).Rows.Count - 1

        If File.Exists(OutputFname) Then File.Delete(OutputFname)

        For i = 0 To 4
            PrintSection(i, OutputFname, detcount)
        Next

        dasp.Dispose()
        dssp.Dispose()
    End Sub

    Private Sub GetReportParams(ByVal ReportName As String)
        Dim cm As String = String.Format("select * from PrnH where Report='{0}'", ReportName)
        Dim da As New SqlDataAdapter(cm, GetCon)
        Dim ds As New DataSet

        da.Fill(ds)

        With ds.Tables(0)
            aa = .Rows(0)("aa")

            startPrinter = .Rows(0)("Start")
            endPrinter = .Rows(0)("End")

            seclines(0) = .Rows(0)("RH")
            seclines(1) = .Rows(0)("PH")
            seclines(2) = .Rows(0)("D")
            seclines(3) = .Rows(0)("PF")
            seclines(4) = .Rows(0)("RF")

            query = .Rows(0)("Query")

        End With

        da.Dispose()
        ds.Dispose()

    End Sub


    Private Sub PrintSection(ByVal Section As Integer, ByVal filename As String, ByVal dCount As Integer)

        Dim rpDefcm As String = "select * from PrnS where PrnH='" & aa & "' and Section=" & Section + 1 & " order by line,col "
        Using rpDefda As New SqlDataAdapter(rpDefcm, GetCon)
            Dim rpDefds As New DataSet()
            rpDefda.Fill(rpDefds)
            If rpDefds.Tables(0).Rows.Count - 1 < 0 Then Exit Sub
            Dim padding As Integer = 0
            Dim linescounter As Integer = IIf(Section <> 2, seclines(Section) - 1, dCount)
            Dim lines(linescounter) As String

            For i = 0 To rpDefds.Tables(0).Rows.Count - 1
                padding = rpDefds.Tables(0).Rows(i)("PrintLen")
                If Section = 2 Then
                    For k = 0 To dCount

                        lines(rpDefds.Tables(0).Rows(i)("Line") - 1 + k) = lines(rpDefds.Tables(0).Rows(i)("Line") - 1 + k) & GetValues(rpDefds.Tables(0).Rows(i)("FieldName"), k).PadRight(padding)

                    Next

                    'lines(rpDefds.Tables(0).Rows(i)("Line") - 1 + i) = lines(rpDefds.Tables(0).Rows(i)("Line") - 1 + i) & GetValues(rpDefds.Tables(0).Rows(i)("FieldName"), i).PadRight(padding)
                Else
                    lines(rpDefds.Tables(0).Rows(i)("Line") - 1) = lines(rpDefds.Tables(0).Rows(i)("Line") - 1) & GetValues(rpDefds.Tables(0).Rows(i)("FieldName"), 0).PadRight(padding)
                End If
            Next
            Dim culture As CultureInfo = New CultureInfo(My.Settings.Culture)
            '("el-GR")
            Dim codePage As Integer = culture.TextInfo.ANSICodePage
            ' Now we need to make an encoding based on this Greek code page
            Dim grEncoding As Encoding = Encoding.GetEncoding(codePage)
            Using oWrite As New StreamWriter(filename, True, grEncoding)
                For j = 0 To UBound(lines)
                    oWrite.WriteLine(lines(j))
                Next
                oWrite.Close()
            End Using
        End Using


    End Sub


    Private Function GetValues(ByVal FieldName As String, ByVal dcount As Integer) As String
        Dim val As String

        If FieldName.StartsWith("\") Then
            val = " "
        Else
            If dssp.Tables(0).Rows(dcount)(FieldName) Is DBNull.Value Then
                val = ""
            Else
                val = dssp.Tables(0).Rows(dcount)(FieldName)
            End If

        End If

        Return val

    End Function

End Class
