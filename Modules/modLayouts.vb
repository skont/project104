Imports System.Data.SqlClient
Imports DevExpress.XtraLayout
Imports System.IO
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.BandedGrid
Imports DevExpress.XtraGrid
Imports DevExpress.XtraPivotGrid
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraBars
Imports DevExpress.XtraNavBar

Module modLayouts

    Public Sub LoadLayoutFromDb(ByVal obj As Object)

        Dim SenderMaster As String = ""
        Dim SenderName As String = ""


        If TypeOf obj Is BarManager Then
            SenderMaster = "MainForm"
            SenderName = "mnu"
        ElseIf TypeOf obj Is NavBarControl Then
            SenderMaster = "MainForm"
            SenderName = "nvb"
        Else

            Try
                SenderMaster = obj.tag.sendermaster
            Catch ex As Exception
                SenderMaster = obj.tag
            End Try

            SenderName = obj.name

        End If


        Dim com As String = String.Format("exec up1getlayout '{0}','{1}'", SenderMaster, SenderName)
        Dim da As New SqlDataAdapter(com, GetSysCon)
        Dim ds As New DataSet

        da.Fill(ds)

        da.Dispose()
        ds.Dispose()

        If ds.Tables(0).Rows.Count > 0 Then
            Try
                Dim ms As New MemoryStream
                Dim data() As Byte

                data = CType(ds.Tables(0).Rows(0).Item("layout"), Byte())
                ms.Write(data, 0, data.GetUpperBound(0))
                ms.Seek(0, System.IO.SeekOrigin.Begin)

                If TypeOf obj Is GridControl Then
                    For Each v In obj.viewcolection
                        v.OptionsLayout.StoreAllOptions = True
                        v.OptionsLayout.Columns.RemoveOldColumns = False
                        v.OptionsLayout.Columns.AddNewColumns = True

                        v.OptionsLayout.Columns.StoreAllOptions = True
                        v.RestoreLayoutFromStream(ms)
                    Next

                ElseIf (TypeOf obj Is GridView) OrElse (TypeOf obj Is AdvBandedGridView) Then

                    obj.OptionsLayout.Columns.RemoveOldColumns = False
                    obj.OptionsLayout.Columns.AddNewColumns = True

                    obj.OptionsLayout.StoreAllOptions = True
                    obj.OptionsLayout.Columns.StoreAllOptions = True
                    obj.RestoreLayoutFromStream(ms)

                ElseIf TypeOf obj Is PivotGridControl Then

                    obj.OptionsLayout.StoreAllOptions = True
                    obj.OptionsLayout.Columns.StoreAllOptions = True
                    obj.RestoreLayoutFromStream(ms)

                ElseIf TypeOf obj Is NavBarControl Then
                    obj.restorefromstream(ms)

                Else
                    obj.RestoreLayoutFromStream(ms)
                End If

                ms.Close()
                ms.Dispose()

                ms = Nothing

            Catch ex As Exception

                If Not TypeOf ex Is ArgumentOutOfRangeException Then
                    WriteLogEntry("Load Layout" & ex.Message)
                End If

            End Try
        End If



    End Sub

    Public Sub SaveLayoutToDb(ByVal obj As Object)
        Dim SenderName = ""

        If TypeOf obj Is GridControl Then
            SenderName = obj.name
            For Each v In obj.viewcollection
                If v.tag.vAllowSaveLayout = False Then Exit Sub
                If Message("msgSaveLayout", v.name) = "No" Then Exit Sub

                v.OptionsLayout.Columns.RemoveOldColumns = False
                v.OptionsLayout.Columns.AddNewColumns = True
                v.OptionsLayout.StoreAllOptions = True
                v.OptionsLayout.Columns.StoreAllOptions = True
                SL(v)
            Next

        ElseIf (TypeOf obj Is GridView) OrElse (TypeOf obj Is AdvBandedGridView) Then
            SenderName = obj.name
            If obj.tag.vAllowSaveLayout = False Then Exit Sub
            If Message("msgSaveLayout", SenderName) = "No" Then Exit Sub

            obj.OptionsLayout.Columns.RemoveOldColumns = False
            obj.OptionsLayout.Columns.AddNewColumns = True
            obj.OptionsLayout.StoreAllOptions = True
            obj.OptionsLayout.Columns.StoreAllOptions = True
            SL(obj)

        ElseIf TypeOf obj Is PivotGridControl Then
            SenderName = obj.name
            If obj.tag.vAllowSaveLayout = False Then Exit Sub
            If Message("msgSaveLayout", SenderName) = "No" Then Exit Sub

            obj.OptionsLayout.StoreAllOptions = True
            obj.OptionsLayout.Columns.StoreAllOptions = True
            SL(obj)

        ElseIf TypeOf obj Is LayoutControl Then
            SenderName = obj.name
            If obj.parent.myAllowSaveLayout = False Then Exit Sub
            If Message("msgSaveLayout", SenderName) = "No" Then Exit Sub
            SL(obj)

        ElseIf TypeOf obj Is BarManager Then
            SL(obj)

        ElseIf TypeOf obj Is NavBarControl Then
            SL(obj)

        Else
            SL(obj)
        End If


    End Sub

    Private Sub SL(ByVal obj As Object)

        Dim SenderMaster As String = ""
        Dim SenderName As String = ""

        If TypeOf obj Is BarManager Then
            SenderMaster = "MainForm"
            SenderName = "mnu"
        ElseIf TypeOf obj Is NavBarControl Then
            SenderMaster = "MainForm"
            SenderName = "nvb"
        Else

            Try
                SenderMaster = obj.tag.sendermaster
            Catch ex As Exception
                SenderMaster = obj.tag
            End Try

            SenderName = obj.name

        End If

        Dim ms As New MemoryStream
        If TypeOf obj Is NavBarControl Then
            obj.savetostream(ms)
        Else
            obj.SaveLayoutToStream(ms)
        End If

        ms.Seek(0, SeekOrigin.Begin)

        Dim data(ms.Length) As Byte
        ms.Position = 0
        ms.Read(data, 0, ms.Length)


        Using con As New SqlConnection(GetSysCon)

            'Create the command object that will do
            'the database work
            Using cmd As New SqlCommand()
                con.Open()

                'Set the properties of ouor connection object
                cmd.Connection = con
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandText = "up1savelayout"

                'Add the parameters for the stored procedure
                cmd.Parameters.AddWithValue("@SenderMaster", SenderMaster)
                cmd.Parameters.AddWithValue("@SenderName", SenderName)
                cmd.Parameters.AddWithValue("@Layout", data)

                'Execute the Stored Procedure
                cmd.ExecuteNonQuery()

            End Using
        End Using



        ms.Close()
        ms.Dispose()
        ms = Nothing


    End Sub

    Public Sub SaveToXML(ByVal view As Object)
        Using sfile As New SaveFileDialog() With {.InitialDirectory = "c:\layouts\", .Filter = "XML files (*.xml)|*.xml", .Title = "Save Layout ..."}
            sfile.ShowDialog()
            If Not sfile.FileName = String.Empty Then
                view.OptionsLayout.Columns.RemoveOldColumns = False
                view.OptionsLayout.Columns.AddNewColumns = True
                view.OptionsLayout.Columns.StoreAllOptions = True
                view.OptionsLayout.StoreAllOptions = True
                view.SaveLayoutToXml(sfile.FileName)
            End If
        End Using

    End Sub

    Public Sub LoadFromXML(ByVal view As Object)

        Using lfile As New OpenFileDialog() With {.InitialDirectory = "c:\layouts\", .Filter = "XML files (*.xml)|*.xml", .Title = "Load Layout ..."}
            lfile.ShowDialog()
            If Not lfile.FileName = String.Empty Then
                view.OptionsLayout.Columns.RemoveOldColumns = False
                view.OptionsLayout.Columns.AddNewColumns = True
                view.OptionsLayout.Columns.StoreAllOptions = True
                view.OptionsLayout.StoreAllOptions = True
                view.RestoreLayoutFromXml(lfile.FileName)
            End If
        End Using
    End Sub

    Public Sub PreviewPrint(ByVal obj As Object)

        Try

            '' Opens the Preview window.
            '' Create a PrintingSystem component. 
            'Dim ps As New PrintingSystem()
            '' Create a link that will print a control. 
            '' Specify the control to be printed. 
            '' Set the paper format. 
            'Dim link As New PrintableComponentLink(ps)
            'link.Component = obj
            'link.PaperKind = System.Drawing.Printing.PaperKind.A4


            '' Subscribe to the CreateReportHeaderArea event used to generate the report header. 
            ''AddHandler link.CreateReportHeaderArea, AddressOf PrintableComponentLink1_CreateReportHeaderArea

            '' Generate the report. 
            'link.CreateDocument()
            '' Show the report. 
            'link.ShowPreviewDialog()


            Dim lnk As New PrintableComponentLink(New PrintingSystem)
            lnk.Component = obj
            lnk.PaperKind = System.Drawing.Printing.PaperKind.A4
            lnk.CreateDocument()
            lnk.ShowPreviewDialog()

        Catch ex As Exception
            WriteLogEntry(ex.Message)
        End Try

    End Sub
End Module
