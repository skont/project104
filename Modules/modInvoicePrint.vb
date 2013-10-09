Imports CrystalDecisions.CrystalReports.Engine
Imports System.Data.SqlClient

Module modInvoicePrint

    Private Sub PrintReport(ByVal Code As String, ByVal Frm As String, ByVal Row As Integer, Optional ByVal PrinterName As String = "")

        Dim rpRow = GetReportDS(Code, Frm)
        Using CR As New ReportDocument()
            CR.Load(rpRow.Rows(Row)("rptFile"))
            Dim ConInfo As New CrystalDecisions.Shared.TableLogOnInfo()
            Dim crTables As Tables = CR.Database.Tables
            For Each crTable As Table In crTables
                With ConInfo.ConnectionInfo
                    .ServerName = My.Settings.Server
                    .DatabaseName = My.Settings.Database
                    .UserID = My.Settings.Username
                    .Password = App.UserInfo.PassWord 'mlPass
                    '.IntegratedSecurity = True
                End With
                crTable.ApplyLogOnInfo(ConInfo)
            Next
            CR.SetParameterValue(0, Code)
            If PrinterName <> "" Then
                Try
                    CR.PrintOptions.PrinterName = PrinterName
                Catch
                End Try
            End If
            CR.PrintToPrinter(1, False, 0, 0)
        End Using

    End Sub

    Public Function doCheckKBS(ByVal s As String, Optional ByVal msg As Boolean = True)
        Dim res As Boolean = False
        Dim kbs = spExec("upInvCheckKBS", s)

        If Len(kbs(0)) <> 0 Then
            If msg Then Message("msgSP", kbs(0))
            res = False
        Else
            res = True
        End If

        Return res
    End Function


    Public Function GetReportDS(ByVal Code As String, ByVal Frm As String) As DataTable

        Dim cm As String = String.Format("exec upInvPrint '{0}','{1}'", Code, Frm)
        Dim da As New SqlDataAdapter(cm, GetCon)
        Dim ds As New DataSet

        da.Fill(ds)

        da.Dispose()
        ds.Dispose()

        Return ds.Tables(0)

    End Function


End Module

