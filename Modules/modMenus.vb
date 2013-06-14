
Imports System.Data.SqlClient
Imports DevExpress.XtraBars
Imports DevExpress.XtraLayout
Imports DevExpress.XtraNavBar

Module modMenus

    Dim ds As DataSet
    Dim da As SqlDataAdapter
    Dim com As String
    Dim con As String = GetSysCon()


    Dim WithEvents bar As New Bar
    Dim WithEvents barl0 As BarSubItem
    Dim WithEvents barl1 As BarSubItem
    Dim WithEvents barlb1 As BarButtonItem



    Sub CreateTopMenuFromDb()

        If My.Settings.RunMode.ToLower = "mobile" Then
            App.Objects.myBarManager.Bars(0).Visible = False
            App.Objects.myBarManager.Bars(1).Visible = False
            App.Objects.myBarManager.Bars(2).Visible = False
        Else


            Dim b1 As New BarStaticItem() With {.Name = "LogInfo", .Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder, .Alignment = BarItemLinkAlignment.Left, .Caption = GetStatusBarText()}
            App.Objects.myBarManager.Bars(2).AddItem(b1)

            App.Objects.myBarManager.Bars(0).Visible = False


            com = "select aa,MenuTree, Name, Caption, Icon from xBarmnu order by MenuTree"

            ds = New DataSet
            da = New SqlDataAdapter(com, con)
            da.Fill(ds)
            da.Dispose()
            ds.Dispose()


            bar = App.Objects.myBarManager.Bars("Main menu")
            bar.OptionsBar.AllowQuickCustomization = False

            For i = 0 To ds.Tables(0).Rows.Count - 1

                Dim currentName As String = ds.Tables(0).Rows(i)("MenuTree").ToString
                Dim nextName As String ' = IIf(i >= ds.Tables(0).Rows.Count - 1, "", ds.Tables(0).Rows(i + 1)("MenuTree").ToString)

                If i >= ds.Tables(0).Rows.Count - 1 Then
                    nextName = ""
                Else
                    nextName = ds.Tables(0).Rows(i + 1)("MenuTree").ToString
                End If

                Dim caption As String = ds.Tables(0).Rows(i)("Caption").ToString
                Dim name As String = ds.Tables(0).Rows(i)("Name").ToString

                'Console.WriteLine(currentName, nextName)

                If Len(currentName) = 2 Then
                    Dim subitem As BarSubItem = New BarSubItem(App.Objects.myBarManager, caption)
                    subitem.Name = currentName
                    subitem.Id = CInt(ds.Tables(0).Rows(i)("aa"))
                    bar.AddItem(subitem)

                ElseIf nextName.StartsWith(currentName) Then
                    Dim subitem As BarSubItem = New BarSubItem(App.Objects.myBarManager, caption)
                    subitem.Name = currentName
                    subitem.Id = CInt(ds.Tables(0).Rows(i)("aa"))

                    _getParent(App.Objects.myBarManager, currentName, ds.Tables(0)).AddItem(subitem)
                Else
                    Dim subButton As BarButtonItem = New BarButtonItem(App.Objects.myBarManager, caption)
                    subButton.Name = currentName
                    subButton.Tag = currentName
                    subButton.Id = ds.Tables(0).Rows(i)("aa")

                    AddHandler subButton.ItemClick, AddressOf BarButtonItem_ItemClick
                    _getParent(App.Objects.myBarManager, currentName, ds.Tables(0)).AddItem(subButton)


                End If
            Next

        End If

    End Sub


    Private Sub BarButtonItem_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
        Dim subMenu As BarSubItem = TryCast(e.Item, BarSubItem)
        If Not subMenu Is Nothing Then Return

        Select Case e.Item.Name
            Case "mnuEnd"

                If Message("msgQuit") = "Yes" Then

                    For Each lc In App.Objects.myMainForm.Controls
                        If TypeOf lc Is LayoutControl Then
                            If lc.ismodified Then SaveLayoutToDb(lc)
                        End If
                    Next

                    FullAppExit()
                End If


            Case "mnuAppPath"
                Process.Start("explorer.exe", Application.StartupPath)

            Case "mnuAbout"
                Message("msgGeneral", "Product Version: " & GetSoftVersion() & vbNewLine & "Registered to: " & App.Constants.RegisteredTo)

            Case "mnuViewLog"
                Dim f As String = Application.StartupPath & "\MediaFrame.log"

                If IO.File.Exists(f) Then
                    Try
                        Process.Start("notepad++.exe", f)
                    Catch ex As Exception
                        WriteLogEntry("notepad++ not installed")
                        Process.Start("notepad.exe", f)
                    End Try
                End If

            Case "mnuAdmin"
                Using op As New xfrmOptions()
                    op.ShowDialog()
                End Using

            Case "mnuRelogin"

            Case "mnuDotMatrixTest"
                DotMatrixPrint("Hello World\r\n")
            Case "mnuShowNavBar"
                Try
                    App.Objects.myDockManager.HiddenPanels.Item(0).Show()
                Catch ex As Exception

                End Try

            Case "mnuNavBarShowAllGroups"
                For Each g As NavBarGroup In App.Objects.myMainNavBar.Groups
                    g.Visible = True
                Next

            Case Else
                ' mnu einai 12 enniaria sto telos
                Dim g As Guid = Guid.Parse("00000000-0000-0000-0000-999999999999")
                DoActions(g, e.Link.Item.Tag, "clk")
                'DoActions("mnu", e.Item.Tag, "clk")

        End Select

    End Sub

    Sub CreateNavMenuFromDb()

        com = "select * from xBarnvb"

        ds = New DataSet
        da = New SqlDataAdapter(com, con)
        da.Fill(ds)
        da.Dispose()
        ds.Dispose()


        For i = 0 To ds.Tables(0).Rows.Count - 1

            If Len(ds.Tables(0).Rows(i)("MenuTree")) = 2 Then

                Dim navBarGroup As DevExpress.XtraNavBar.NavBarGroup = App.Objects.myMainNavBar.Groups.Add()

                If My.Settings.RunMode.ToLower = "remote" OrElse My.Settings.RunMode.ToLower = "mobile" Then
                    navBarGroup.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.SmallIconsText
                End If

                navBarGroup.Name = ds.Tables(0).Rows(i)("Name")
                navBarGroup.Caption = ds.Tables(0).Rows(i)("Caption")
                navBarGroup.Expanded = ds.Tables(0).Rows(i)("Expanded")
                navBarGroup.LargeImage = GetImageFromDB(ds.Tables(0).Rows(i)("Icon"))
                App.Objects.myMainNavBar.ActiveGroup = App.Objects.myMainNavBar.Groups(navBarGroup.Name)

            ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 5 Then

                Dim item As DevExpress.XtraNavBar.NavBarItem = App.Objects.myMainNavBar.Items.Add()
                item.Name = ds.Tables(0).Rows(i)("Name")
                item.Caption = ds.Tables(0).Rows(i)("Caption")
                item.Tag = ds.Tables(0).Rows(i)("MenuTree")
                item.SmallImage = GetImageFromDB(ds.Tables(0).Rows(i)("Icon"))

                App.Objects.myMainNavBar.ActiveGroup.ItemLinks.Add(item)

                AddHandler item.LinkClicked, AddressOf Item_LinkClicked

            End If

        Next

    End Sub

    Public Sub Item_LinkClicked(ByVal sender As Object, ByVal e As DevExpress.XtraNavBar.NavBarLinkEventArgs)

        ' nvb einai 12 assoi sto tellos
        Dim g As Guid = Guid.Parse("00000000-0000-0000-0000-111111111111")
        DoActions(g, e.Link.Item.Tag, "clk")
        'DoActions("nvb", e.Link.Item.Tag, "clk")


    End Sub

    Private Function _getParent(ByVal bm As BarManager, ByVal current As String, ByVal tbl As DataTable) As BarSubItem

        Dim par As String = current.Substring(0, Len(current) - 3)

        tbl.DefaultView.Sort = "MenuTree"
        Dim intRow = tbl.DefaultView.Find(par)
        Dim parID = CInt(tbl.Rows(intRow)("aa"))

        Dim subItem As BarSubItem = bm.Items.FindById(parID)
        Return subItem
    End Function
End Module
