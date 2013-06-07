﻿
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


            com = "select MenuTree, Name, Caption, Icon from xBarmnu order by MenuTree"

            ds = New DataSet
            da = New SqlDataAdapter(com, con)
            da.Fill(ds)
            da.Dispose()
            ds.Dispose()


            bar = App.Objects.myBarManager.Bars("Main menu")
            bar.OptionsBar.AllowQuickCustomization = False


            For i = 0 To ds.Tables(0).Rows.Count - 1

                If Len(ds.Tables(0).Rows(i)("MenuTree")) = 2 Then

                    barl0 = New BarSubItem
                    barl0.Name = ds.Tables(0).Rows(i)("Name")
                    barl0.Caption = ds.Tables(0).Rows(i)("Caption")
                    barl0.Tag = ds.Tables(0).Rows(i)("MenuTree")
                    barl0.Id = App.Objects.myBarManager.GetNewItemId()
                    barl0.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)

                    bar.AddItem(barl0)

                ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 5 Then

                    If i < ds.Tables(0).Rows.Count - 1 Then
                        If Len(ds.Tables(0).Rows(i + 1)("MenuTree")) = 8 Then

                            barl1 = New BarSubItem
                            barl1.Caption = ds.Tables(0).Rows(i)("Caption").ToString
                            barl1.Id = App.Objects.myBarManager.GetNewItemId()
                            barl1.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)

                            barl0.AddItem(barl1)


                        Else


                            barlb1 = New BarButtonItem
                            barlb1.Name = ds.Tables(0).Rows(i)("Name")
                            barlb1.Caption = ds.Tables(0).Rows(i)("Caption")
                            barlb1.Tag = ds.Tables(0).Rows(i)("MenuTree")
                            barlb1.Id = App.Objects.myBarManager.GetNewItemId()
                            barlb1.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)
                            barl0.AddItem(barlb1)

                            AddHandler barlb1.ItemClick, AddressOf BarButtonItem_ItemClick

                        End If

                    End If


                ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 8 Then

                    Dim barl2 As New BarButtonItem
                    If ds.Tables(0).Rows(i)("caption") = "" Then

                    Else

                        barl2.Name = ds.Tables(0).Rows(i)("Name")
                        barl2.Caption = ds.Tables(0).Rows(i)("Caption")
                        barl2.Tag = ds.Tables(0).Rows(i)("MenuTree")
                        barl2.Id = App.Objects.myBarManager.GetNewItemId()
                        barl2.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)
                        barl1.AddItem(barl2)
                        AddHandler barl2.ItemClick, AddressOf BarButtonItem_ItemClick

                    End If


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


End Module

'-------------------------------------
'Imports System.Data.SqlClient
'Imports DevExpress.XtraBars
'Imports DevExpress.XtraLayout
'Imports DevExpress.XtraNavBar

'Module modMenus

'    Dim ds As DataSet
'    Dim da As SqlDataAdapter
'    Dim com As String
'    Dim con As String = GetSysCon()


'    Dim WithEvents bar As New Bar
'    Dim WithEvents barl0 As BarSubItem
'    Dim WithEvents barl1 As BarSubItem
'    Dim WithEvents barlb1 As BarButtonItem

'-

'    Sub CreateTopMenuFromDb()


'        App.Objects.myBarManager.Bars(0).Visible = False



'        com = "select MenuTree, Name, Caption, Icon from xBarmnu order by MenuTree"

'        ds = New DataSet
'        da = New SqlDataAdapter(com, con)
'        da.Fill(ds)
'        da.Dispose()
'        ds.Dispose()


'        bar = App.Objects.myBarManager.Bars("Main menu")
'        bar.OptionsBar.AllowQuickCustomization = False

'        For i = 0 To ds.Tables(0).Rows.Count - 1


'            If Len(ds.Tables(0).Rows(i)("MenuTree")) = 2 Then

'                barl0 = New BarSubItem
'                barl0.Id = App.Objects.myBarManager.GetNewItemId()
'                barl0.Name = ds.Tables(0).Rows(i)("Name")
'                barl0.Caption = ds.Tables(0).Rows(i)("Caption")
'                barl0.Tag = ds.Tables(0).Rows(i)("MenuTree")
'                barl0.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)

'                bar.AddItem(barl0)

'            ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 5 Then


'                If Len(ds.Tables(0).Rows(i + 1)("MenuTree")) = 8 Then

'                    barl1 = New BarSubItem
'                    barl1.Caption = ds.Tables(0).Rows(i)("Caption").ToString
'                    barl1.Id = App.Objects.myBarManager.GetNewItemId()
'                    barl1.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)

'                    barl0.AddItem(barl1)



'                Else


'                    barlb1 = New BarButtonItem
'                    barlb1.Name = ds.Tables(0).Rows(i)("Name")
'                    barlb1.Caption = ds.Tables(0).Rows(i)("Caption")
'                    barlb1.Tag = ds.Tables(0).Rows(i)("MenuTree")
'                    barlb1.Id = App.Objects.myBarManager.GetNewItemId()
'                    barlb1.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)
'                    barl0.AddItem(barlb1)


'                    AddHandler barlb1.ItemClick, AddressOf BarButtonItem_ItemClick

'                End If
'+        '    If i < ds.Tables(0).Rows.Count - 1 Then
'+        '        If Len(ds.Tables(0).Rows(i + 1)("MenuTree")) = 8 Then

'-                    End If
'+        '            barl1 = New BarSubItem
'+        '            barl1.Caption = ds.Tables(0).Rows(i)("Caption").ToString
'+        '            barl1.Id = App.Objects.myBarManager.GetNewItemId()
'+        '            barl1.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)

'+        '            barl0.AddItem(barl1)

'-                ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 8 Then
'+        '        Else

'-                    Dim barl2 As New BarButtonItem
'-                    If ds.Tables(0).Rows(i)("caption") = "" Then

'-                    Else
'+        '            barlb1 = New BarButtonItem
'+        '            barlb1.Name = ds.Tables(0).Rows(i)("Name")
'+        '            barlb1.Caption = ds.Tables(0).Rows(i)("Caption")
'+        '            barlb1.Tag = ds.Tables(0).Rows(i)("MenuTree")
'+        '            barlb1.Id = App.Objects.myBarManager.GetNewItemId()
'+        '            barlb1.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)

'-                        barl2.Name = ds.Tables(0).Rows(i)("Name")
'-                        barl2.Caption = ds.Tables(0).Rows(i)("Caption")
'-                        barl2.Tag = ds.Tables(0).Rows(i)("MenuTree")
'-                        barl2.Id = App.Objects.myBarManager.GetNewItemId()
'-                        barl2.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)
'-                        barl1.AddItem(barl2)
'-                        AddHandler barl2.ItemClick, AddressOf BarButtonItem_ItemClick
'+        '            barl0.AddItem(barlb1)

'-                    End If
'+        '            AddHandler barlb1.ItemClick, AddressOf BarButtonItem_ItemClick

'+        '        End If

'-                End If
'+        '    End If
'+
'+
'+        'ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 8 Then
'+
'+        '    Dim barl2 As New BarButtonItem
'+        '    If ds.Tables(0).Rows(i)("caption") = "" Then
'+
'+        '    Else
'+
'+        '        barl2.Name = ds.Tables(0).Rows(i)("Name")
'+        '        barl2.Caption = ds.Tables(0).Rows(i)("Caption")
'+        '        barl2.Tag = ds.Tables(0).Rows(i)("MenuTree")
'+        '        barl2.Id = App.Objects.myBarManager.GetNewItemId()
'+        '        barl2.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)
'+        '        barl1.AddItem(barl2)
'+        '        AddHandler barl2.ItemClick, AddressOf BarButtonItem_ItemClick
'+
'+        '    End If
'+
'+
'+        'End If
'+
'+        'Next
'+
'+        'End If

'-            Next

'-        End If

'    End Sub

'    'Dim con As String = GetSysCon()


'    ''Dim WithEvents bar As New Bar
'    ''Dim WithEvents barl0 As BarSubItem
'    'Dim WithEvents barl1 As BarSubItem
'    'Dim WithEvents barlb1 As BarButtonItem

'    'Dim i As Integer


'    'Sub CreateTopMenuFromDb()

'    '    If My.Settings.RunMode.ToLower = "mobile" Then
'    '        App.Objects.myBarManager.Bars(0).Visible = False
'    '        App.Objects.myBarManager.Bars(1).Visible = False
'    '        App.Objects.myBarManager.Bars(2).Visible = False
'    '    Else


'    '        Dim b1 As New BarStaticItem() With {.Name = "LogInfo", .Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder, .Alignment = BarItemLinkAlignment.Left, .Caption = GetStatusBarText()}
'    '        App.Objects.myBarManager.Bars(2).AddItem(b1)

'    '        App.Objects.myBarManager.Bars(0).Visible = False


'    '        com = "select MenuTree, Name, Caption, Icon from xBarmnu order by MenuTree"

'    '        ds = New DataSet
'    '        da = New SqlDataAdapter(com, con)
'    '        da.Fill(ds)
'    '        da.Dispose()
'    '        ds.Dispose()


'    '        Dim bar As New Bar
'    '        bar = App.Objects.myBarManager.Bars("Main menu")
'    '        bar.OptionsBar.AllowQuickCustomization = False
'    '        'Dim barl0 As New BarSubItem

'    '        bar.AddItem(addBarItem(bar, ds.Tables(0), 0))

'    '    End If


'    '    'ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 5 Then

'    '    '    If i < ds.Tables(0).Rows.Count - 1 Then
'    '    '        If Len(ds.Tables(0).Rows(i + 1)("MenuTree")) = 8 Then

'    '    '            barl1 = New BarSubItem
'    '    '            barl1.Caption = ds.Tables(0).Rows(i)("Caption").ToString
'    '    '            barl1.Id = App.Objects.myBarManager.GetNewItemId()
'    '    '            barl1.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)

'    '    '            barl0.AddItem(barl1)

'    '    '        Else


'    '    '            barlb1 = New BarButtonItem
'    '    '            barlb1.Name = ds.Tables(0).Rows(i)("Name")
'    '    '            barlb1.Caption = ds.Tables(0).Rows(i)("Caption")
'    '    '            barlb1.Tag = ds.Tables(0).Rows(i)("MenuTree")
'    '    '            barlb1.Id = App.Objects.myBarManager.GetNewItemId()
'    '    '            barlb1.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)

'    '    '            barl0.AddItem(barlb1)

'    '    '            AddHandler barlb1.ItemClick, AddressOf BarButtonItem_ItemClick

'    '    '        End If

'    '    '    End If


'    '    'ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 8 Then

'    '    '    Dim barl2 As New BarButtonItem
'    '    '    If ds.Tables(0).Rows(i)("caption") = "" Then

'    '    '    Else

'    '    '        barl2.Name = ds.Tables(0).Rows(i)("Name")
'    '    '        barl2.Caption = ds.Tables(0).Rows(i)("Caption")
'    '    '        barl2.Tag = ds.Tables(0).Rows(i)("MenuTree")
'    '    '        barl2.Id = App.Objects.myBarManager.GetNewItemId()
'    '    '        barl2.Glyph = GetImageFromDB(ds.Tables(0).Rows(i)("Icon").ToString)
'    '    '        barl1.AddItem(barl2)
'    '    '        AddHandler barl2.ItemClick, AddressOf BarButtonItem_ItemClick

'    '    '    End If


'    '    'End If

'    '    'Next

'    '    'End If


'    'End Sub
'    'Private Function addBarItem(ByVal bar As Bar, ByVal tbl As DataTable, ByVal i As Integer) As BarSubItem

'    '    Dim barl0 As BarSubItem = New BarSubItem
'    '    barl0.Id = App.Objects.myBarManager.GetNewItemId()
'    '    barl0.Name = tbl.Rows(i)("Name")
'    '    barl0.Caption = tbl.Rows(i)("Caption")
'    '    barl0.Tag = tbl.Rows(i)("MenuTree")
'    '    barl0.Glyph = GetImageFromDB(tbl.Rows(i)("Icon").ToString)

'    '    If Len(tbl.Rows(i + 1)("MenuTree")) = 2 Then
'    '        'add another bar item
'    '        bar.AddItem(addBarItem(bar, tbl, i + 1))

'    '    Else
'    '        If checklevel(tbl.Rows(i + 1)("MenuTree"), tbl.Rows(i + 2)("MenuTree")) Then
'    '            'adds a barsubitem
'    '            barl0.AddItem(addBarSubItem(tbl, i + 1))
'    '        Else
'    '            'adds a bar button
'    '            barl0.AddItem(addBarButtonItem(tbl.Rows(i + 1)))
'    '        End If
'    '    End If

'    '    Return barl0
'    'End Function

'    'Private Function addBarSubItem(ByVal parent As Bar, ByVal tbl As DataTable, ByVal c As Integer) As BarSubItem

'    '    Dim bsi As BarSubItem = New BarSubItem

'    '    bsi.Id = App.Objects.myBarManager.GetNewItemId()
'    '    bsi.Name = tbl.Rows(c)("Name")
'    '    bsi.Caption = tbl.Rows(c)("Caption").ToString
'    '    bsi.Glyph = GetImageFromDB(tbl.Rows(c)("Icon").ToString)

'    '    Return bsi
'    'End Function

'    'Private Function addBarButtonItem(ByVal row As DataRow) As BarButtonItem
'    '    Dim bbi As BarButtonItem = New BarButtonItem

'    '    bbi.Id = App.Objects.myBarManager.GetNewItemId()
'    '    bbi.Name = row("Name")
'    '    bbi.Caption = row("Caption")
'    '    bbi.Tag = row("MenuTree")
'    '    bbi.Glyph = GetImageFromDB(row("Icon").ToString)

'    '    AddHandler bbi.ItemClick, AddressOf BarButtonItem_ItemClick

'    '    'i = i + 1
'    '    Return bbi
'    'End Function

'    'Private Function checklevel(ByVal current As String, ByVal following As String) As Boolean
'    '    Dim res As Boolean = False
'    '    If following.StartsWith(current) Then res = True

'    '    Return res
'    'End Function

'    Private Sub BarButtonItem_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
'        Dim subMenu As BarSubItem = TryCast(e.Item, BarSubItem)
'        If Not subMenu Is Nothing Then Return

'        Select Case e.Item.Name
'            Case "mnuEnd"

'                If Message("msgQuit") = "Yes" Then

'                    For Each lc In App.Objects.myMainForm.Controls
'                        If TypeOf lc Is LayoutControl Then
'                            If lc.ismodified Then SaveLayoutToDb(lc)
'                        End If
'                    Next

'                    FullAppExit()
'                End If


'            Case "mnuAppPath"
'                Process.Start("explorer.exe", Application.StartupPath)

'            Case "mnuAbout"
'                Message("msgGeneral", "Product Version: " & GetSoftVersion() & vbNewLine & "Registered to: " & App.Constants.RegisteredTo)

'            Case "mnuViewLog"
'                Dim f As String = Application.StartupPath & "\MediaFrame.log"

'                If IO.File.Exists(f) Then
'                    Try
'                        Process.Start("notepad++.exe", f)
'                    Catch ex As Exception
'                        WriteLogEntry("notepad++ not installed")
'                        Process.Start("notepad.exe", f)
'                    End Try
'                End If

'            Case "mnuAdmin"
'                Using op As New xfrmOptions()
'                    op.ShowDialog()
'                End Using

'            Case "mnuRelogin"

'            Case "mnuDotMatrixTest"
'                DotMatrixPrint("Hello World\r\n")

'            Case "mnuShowNavBar"
'                Try
'                    App.Objects.myDockManager.HiddenPanels.Item(0).Show()
'                Catch ex As Exception

'                End Try

'            Case "mnuNavBarShowAllGroups"
'                For Each g As NavBarGroup In App.Objects.myMainNavBar.Groups
'                    g.Visible = True
'                Next

'            Case Else
'                ' mnu einai 12 enniaria sto telos
'                Dim g As Guid = Guid.Parse("00000000-0000-0000-0000-999999999999")
'                DoActions(g, e.Link.Item.Tag, "clk")
'                'DoActions("mnu", e.Item.Tag, "clk")

'        End Select

'    End Sub

'    Sub CreateNavMenuFromDb()

'        com = "select * from xBarnvb"

'        ds = New DataSet
'        da = New SqlDataAdapter(com, con)
'        da.Fill(ds)
'        da.Dispose()
'        ds.Dispose()


'        For i = 0 To ds.Tables(0).Rows.Count - 1

'            If Len(ds.Tables(0).Rows(i)("MenuTree")) = 2 Then

'                Dim navBarGroup As DevExpress.XtraNavBar.NavBarGroup = App.Objects.myMainNavBar.Groups.Add()

'                If My.Settings.RunMode.ToLower = "remote" OrElse My.Settings.RunMode.ToLower = "mobile" Then
'                    navBarGroup.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.SmallIconsText
'                End If

'                navBarGroup.Name = ds.Tables(0).Rows(i)("Name")
'                navBarGroup.Caption = ds.Tables(0).Rows(i)("Caption")
'                navBarGroup.Expanded = ds.Tables(0).Rows(i)("Expanded")
'                navBarGroup.LargeImage = GetImageFromDB(ds.Tables(0).Rows(i)("Icon"))
'                App.Objects.myMainNavBar.ActiveGroup = App.Objects.myMainNavBar.Groups(navBarGroup.Name)

'            ElseIf Len(ds.Tables(0).Rows(i)("MenuTree")) = 5 Then

'                Dim item As DevExpress.XtraNavBar.NavBarItem = App.Objects.myMainNavBar.Items.Add()
'                item.Name = ds.Tables(0).Rows(i)("Name")
'                item.Caption = ds.Tables(0).Rows(i)("Caption")
'                item.Tag = ds.Tables(0).Rows(i)("MenuTree")
'                item.SmallImage = GetImageFromDB(ds.Tables(0).Rows(i)("Icon"))

'                App.Objects.myMainNavBar.ActiveGroup.ItemLinks.Add(item)

'                AddHandler item.LinkClicked, AddressOf Item_LinkClicked

'            End If

'        Next

'    End Sub

'    Public Sub Item_LinkClicked(ByVal sender As Object, ByVal e As DevExpress.XtraNavBar.NavBarLinkEventArgs)

'        ' nvb einai 12 assoi sto tellos
'        Dim g As Guid = Guid.Parse("00000000-0000-0000-0000-111111111111")
'        DoActions(g, e.Link.Item.Tag, "clk")
'        'DoActions("nvb", e.Link.Item.Tag, "clk")


'    End Sub


'End Module
