Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports DevExpress.XtraTab

Module modNewCrystalPrinting

    Public Sub CrystalReportPrint(ByVal r As DataRow)

        Dim rpt = CreateReport(r("f0"), r("f1"))

        If r("f2") <> "" Then
            Try
                rpt.PrintOptions.PrinterName = r("f2")
            Catch
            End Try
        End If
        rpt.PrintToPrinter(1, False, 0, 0)

    End Sub


    Public Function CreateReport(ByVal ReportPath As String, ByVal ReportParameters As String) As ReportDocument

        Dim CR As New ReportDocument
        CR.Load(ReportPath)

        Dim params = Split(ReportParameters, ";")

        Dim crParameterDiscreteValue As ParameterDiscreteValue
        Dim crParameterFieldDefinitions As ParameterFieldDefinitions = CR.DataDefinition.ParameterFields
        Dim crParameterFieldLocation As ParameterFieldDefinition
        Dim crParameterValues As ParameterValues



        'For i = 0 To crParameterFieldDefinitions.Count - 1
        For i = 0 To UBound(params)

            crParameterFieldLocation = crParameterFieldDefinitions.Item(i)
            crParameterValues = crParameterFieldLocation.CurrentValues
            crParameterDiscreteValue = New ParameterDiscreteValue


            If params(i).StartsWith("d|") Then
                crParameterDiscreteValue.Value = Date.Parse(Replace(params(i), "d|", ""))

            ElseIf params(i).StartsWith("f|") Then
                crParameterDiscreteValue.Value = Double.Parse(Replace(params(i), "f|", ""))

            ElseIf params(i).StartsWith("i|") Then
                crParameterDiscreteValue.Value = Integer.Parse(Replace(params(i), "i|", ""))

            ElseIf params(i).StartsWith("s|") Then
                crParameterDiscreteValue.Value = Replace(params(i), "s|", "")

            End If


            crParameterValues.Add(crParameterDiscreteValue)
            crParameterFieldLocation.ApplyCurrentValues(crParameterValues)

        Next

        Dim ConInfo As New TableLogOnInfo
        Dim crTables As Tables = CR.Database.Tables

        For Each crTable As Table In crTables
            With ConInfo.ConnectionInfo
                .ServerName = My.Settings.Server
                .DatabaseName = My.Settings.Database
                .UserID = App.UserInfo.UserName
                .Password = App.UserInfo.PassWord

            End With
            crTable.ApplyLogOnInfo(ConInfo)
        Next


        Return CR
    End Function

    Public Function CreateAxReport(ByVal ReportPath As String, ByVal ReportParameters As String)

        ' edw to vrika: http://www.tek-tips.com/viewthread.cfm?qid=1147478&page=1
        ' to original exei kai subreports na to dw

        Dim crxApp As CRAXDDRT.Application = New CRAXDDRT.Application
        Dim crxRpt As CRAXDDRT.Report = New CRAXDDRT.Report
        Dim crxTables As CRAXDDRT.DatabaseTables
        Dim crxTable As CRAXDDRT.DatabaseTable
        Dim crxSubReportObject As CRAXDDRT.SubreportObject
        Dim crxSubReport As CRAXDDRT.Report
        Dim crxSections As CRAXDDRT.Sections
        Dim crxSection As CRAXDDRT.Section


        'Variable declarations
        Dim strServerOrDSNName As String
        Dim strDBNameOrPath As String
        Dim strUserID As String
        Dim strPassword As String

        strServerOrDSNName = My.Settings.Server
        strDBNameOrPath = My.Settings.Database
        strUserID = App.UserInfo.UserName
        strPassword = App.UserInfo.PassWord

        crxRpt = crxApp.OpenReport(ReportPath)
        For j As Integer = 0 To crxRpt.Database.Tables.Count - 1
            crxRpt.Database.Tables(j + 1).SetLogOnInfo(strServerOrDSNName, strDBNameOrPath, strUserID, strPassword)
        Next

        crxTables = crxRpt.Database.Tables
        For Each crxTable In crxTables

            With crxTable
                .Location = .Location
            End With
        Next
        '---
        'Loop through the Report's Sections to find any subreports, and change them as well
        crxSections = crxRpt.Sections

        For i = 1 To crxSections.Count
            crxSection = crxSections(i)

            For j = 1 To crxSection.ReportObjects.Count

                If crxSection.ReportObjects(j).Kind = ReportObjectKind.SubreportObject Then
                    crxSubReportObject = crxSection.ReportObjects(j)

                    'Open the subreport, and treat like any other report
                    crxSubReport = crxSubReportObject.OpenSubreport
                    '*****************************************
                    crxTables = crxSubReport.Database.Tables

                    For Each crxTable In crxTables
                        With crxTable
                            .SetLogOnInfo(strServerOrDSNName, _
                                strDBNameOrPath, strUserID, strPassword)
                            .Location = .Name
                        End With
                    Next
                    '****************************************
                End If

            Next j

        Next i
   
        '---

        Dim params = Split(ReportParameters, ";")
        Dim val As Object = Nothing

        For i = 0 To UBound(params)
            If params(i).StartsWith("d|") Then
                val = Date.Parse(Replace(params(i), "d|", ""))

            ElseIf params(i).StartsWith("f|") Then
                val = Double.Parse(Replace(params(i), "f|", ""))

            ElseIf params(i).StartsWith("i|") Then
                val = Integer.Parse(Replace(params(i), "i|", ""))

            ElseIf params(i).StartsWith("s|") Then
                val = Replace(params(i), "s|", "")

            End If

            If crxRpt.ParameterFields.Count > 0 Then
                crxRpt.ParameterFields.Item(i + 1).AddCurrentValue(val)
            End If
        Next


        Return crxRpt
    End Function
End Module
