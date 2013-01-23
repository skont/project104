Imports DevExpress.Xpo
Imports DevExpress.Utils
Imports System.Data.SqlClient
Imports DevExpress.XtraLayout
Imports DevExpress.XtraEditors
Imports DevExpress.Xpo.Metadata
Imports DevExpress.Data.Filtering
Imports DevExpress.XtraGrid.Views
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Card
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraGrid.Views.BandedGrid
Imports DevExpress.XtraPivotGrid
Imports DevExpress.XtraGrid
Imports DevExpress.Data.PivotGrid
Imports DevExpress.XtraVerticalGrid
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Columns

Partial Public Class xcntlMainControl
    Inherits XtraForm

    Private Sub getControls(ByVal MainLC As LayoutControl, _
                      ByVal BS() As BindingSource, _
                      ByVal DataSt As DataSet)



        MainLC.BeginUpdate()

        ' ayto pairnei to align mode apo to my.settings
        MainLC.OptionsItemText.TextAlignMode = My.Settings.LayoutAlignMode

        Dim piccounter As Integer = 0

        For i = 0 To objectDetailsPropertiesTable.Rows.Count - 1
            Dim objDetType = objectDetailsPropertiesTable.Rows(i).Item("Type")

            If objDetType.ToString.ToLower.EndsWith("view") Or
                objDetType.ToString.ToLower.StartsWith("rep") Or
                objDetType.ToString.ToLower.StartsWith("stnd_rep") Or
                 objDetType.ToString.ToLower.StartsWith("ds_rep") Or
                objDetType.ToString.ToLower.StartsWith("coldet") Then

            Else

                Dim objName = objectDetailsPropertiesTable.Rows(i).Item("ctrlName")
                Dim objCaption = objectDetailsPropertiesTable.Rows(i).Item("btnCaption")
                Dim objColumn = objectDetailsPropertiesTable.Rows(i).Item("TableColumn")
                Dim objTooltip = objectDetailsPropertiesTable.Rows(i).Item("Tooltip")
                Dim objMask = objectDetailsPropertiesTable.Rows(i).Item("Format_MaskType")
                Dim objFormat = objectDetailsPropertiesTable.Rows(i).Item("Format_MaskEdit")
                Dim txtAlign = objectDetailsPropertiesTable.Rows(i).Item("Text_Align")
                Dim txtFontFamily As String = objectDetailsPropertiesTable.Rows(i).Item("Font_Family")
                Dim txtFontSize = objectDetailsPropertiesTable.Rows(i).Item("Font_Size")
                Dim txtForeColor As String = objectDetailsPropertiesTable.Rows(i).Item("ForeColor").tolower
                Dim txtBackColor As String = objectDetailsPropertiesTable.Rows(i).Item("BackColor").tolower
                Dim objSelectCommand = objectDetailsPropertiesTable.Rows(i).Item("SelectCommand")
                Dim objDisplayMember = objectDetailsPropertiesTable.Rows(i).Item("DisplayMember")
                Dim objValueMember = objectDetailsPropertiesTable.Rows(i).Item("ValueMember")
                Dim objEditable = CBool(objectDetailsPropertiesTable.Rows(i).Item("Editable"))
                Dim objImage = objectDetailsPropertiesTable.Rows(i).Item("ctrlImage")
                Dim BoundMode = objectDetailsPropertiesTable.Rows(i).Item("BoundMode")
                Dim objDetVisible = CBool(objectDetailsPropertiesTable.Rows(i).Item("Visible"))
                Dim SMCriteria = objectDetailsPropertiesTable.Rows(i).Item("SMCriteria")
                Dim RelationTableID = objectDetailsPropertiesTable.Rows(i).Item("GC_RelationID")
                Dim PopWidth As Integer = Convert.ToInt16(objectDetailsPropertiesTable.Rows(i).Item("popWidth"))
                Dim PopHeight As Integer = Convert.ToInt16(objectDetailsPropertiesTable.Rows(i).Item("popHeight"))
                Dim RSum As String = objectDetailsPropertiesTable.Rows(i).Item("rsumFld")
                Dim crRepPath As String = objectDetailsPropertiesTable.Rows(i).Item("CrRepPath")
                Dim crRepParams As String = objectDetailsPropertiesTable.Rows(i).Item("CrRepParams")
                Dim fmFilesObject As String = objectDetailsPropertiesTable.Rows(i).Item("FilesObject")
                Dim btnBehavior As String = objectDetailsPropertiesTable.Rows(i).Item("defBtnBehavior")
                Dim EventsToRun As String = objectDetailsPropertiesTable.Rows(i).Item("EventsToRun")

                Dim item As New LayoutControlItem() With {.Name = "Item" & objName}


                Dim FontDecoration As String = objectDetailsPropertiesTable.Rows(i).Item("Font_Decoration")
                Dim ff As System.Drawing.FontStyle
                Select Case FontDecoration.ToLower
                    Case "bold"
                        ff = System.Drawing.FontStyle.Bold
                    Case "italics"
                        ff = System.Drawing.FontStyle.Italic
                    Case "strikeout"
                        ff = System.Drawing.FontStyle.Strikeout
                    Case "underline"
                        ff = System.Drawing.FontStyle.Underline
                    Case Else
                        ff = System.Drawing.FontStyle.Regular

                End Select


                Select Case objDetType.tolower

                    Case "label"
                        lbl = New LabelControl
                        lbl.Name = objName
                        lbl.Text = ""
                        lbl.Appearance.TextOptions.HAlignment = txtAlign
                        lbl.Appearance.Font = New Font(txtFontFamily, txtFontSize)
                        lbl.ForeColor = getColor(txtForeColor)
                        lbl.BackColor = getColor(txtBackColor)


                        item.Control = (lbl)

                    Case "text", "password"
                        txtEd = New TextEdit
                        txtEd.StyleController = App.Objects.myStyleController
                        txtEd.Name = objName
                        txtEd.ToolTip = objTooltip

                        If objDetType.tolower = "password" Then txtEd.Properties.PasswordChar = "*"

                        txtEd.EnterMoveNextControl = True
                        txtEd.Properties.Mask.MaskType = objMask
                        txtEd.Properties.Mask.EditMask = objFormat
                        txtEd.Properties.Mask.UseMaskAsDisplayFormat = True
                        txtEd.Properties.Appearance.TextOptions.HAlignment = txtAlign
                        txtEd.Properties.Appearance.Font = New Font(txtFontFamily, txtFontSize, ff)
                        txtEd.ForeColor = getColor(txtForeColor)
                        txtEd.BackColor = getColor(txtBackColor)

                        txtEd.Properties.ReadOnly = IIf(objEditable = True, False, True)


                        Try
                            txtEd.DataBindings.Add(New Binding("EditValue", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))
                        Catch ex As Exception

                        End Try

                        AddTXTHandlers(txtEd, EventsToRun)
                        item.Control = (txtEd)

                    Case "pivot"
                        pvg = New PivotGridControl
                        Dim xTag As New clsTagExtender() With {.SenderMaster = Me.Name, .vAllowSaveLayout = True}
                        Dim hlper As New clsContexMenuHelper(pvg)

                        pvg.Name = objName
                        pvg.Tag = xTag



                        Using da As New SqlDataAdapter(objSelectCommand, GetCon())

                            'Exei provlima me ta remote olap 
                            'na doume an to ftiaxnei

                            da.SelectCommand.CommandTimeout = 6000
                            Using ds As New DataSet()
                                da.Fill(ds)
                                pvg.DataSource = ds.Tables(0)
                            End Using
                        End Using
                        pvg.RetrieveFields()
                        pvg.OptionsData.DataFieldUnboundExpressionMode = DataFieldUnboundExpressionMode.UseSummaryValues

                        GetPivotDetails(pvg)
                        If RSum <> "" Then
                            PivotRunningSums(pvg, RSum)
                        End If

                        'ayto to xreiazetai gia na swsei kai na diavasei to layout !! 
                        'des: http://www.devexpress.com/Support/Center/p/Q140419.aspx

                        For Each f As PivotGridField In pvg.Fields
                            f.Name = "pvf_" & f.FieldName
                        Next f
                        LoadLayoutFromDb(pvg)


                        AddPivotHandlers(pvg, EventsToRun)
                        item.Control = (pvg)

                    Case "memo"
                        mtxEd = New MemoEdit
                        mtxEd.StyleController = App.Objects.myStyleController
                        mtxEd.Name = objName
                        mtxEd.ToolTip = objTooltip

                        mtxEd.EnterMoveNextControl = True
                        mtxEd.Properties.Mask.MaskType = objMask
                        mtxEd.Properties.Mask.EditMask = objFormat
                        mtxEd.Properties.Appearance.Font = New Font(txtFontFamily, txtFontSize, ff)
                        mtxEd.ForeColor = getColor(txtForeColor)
                        mtxEd.BackColor = getColor(txtBackColor)
                        mtxEd.Properties.ReadOnly = IIf(objEditable = True, False, True)

                        Try
                            mtxEd.DataBindings.Add(New Binding("EditValue", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))
                        Catch ex As Exception

                        End Try
                        AddMemoEditHandlers(mtxEd, EventsToRun)


                        item.Control = (mtxEd)

                    Case "check"
                        chkEd = New CheckEdit
                        chkEd.StyleController = App.Objects.myStyleController
                        chkEd.ToolTip = objTooltip
                        chkEd.Name = objName
                        chkEd.Text = ""
                        chkEd.Properties.ReadOnly = IIf(objEditable = True, False, True)
                        chkEd.ForeColor = getColor(txtForeColor)
                        chkEd.BackColor = getColor(txtBackColor)

                        'chkEd.Properties.ValueChecked = True
                        'chkEd.Properties.ValueUnchecked = False

                        Try
                            chkEd.DataBindings.Add(New Binding("Checked", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))
                        Catch ex As Exception

                        End Try

                        AddCheckEditHandlers(chkEd, EventsToRun)
                        item.Control = (chkEd)

                    Case "button"
                        btn = New SimpleButton
                        btn.Name = objName
                        btn.Text = objCaption

                        btn.Tag = objColumn
                        btn.Image = GetImageFromDB(objImage)

                        If btnBehavior.ToLower = "accept" Then Me.AcceptButton = btn
                        If btnBehavior.ToLower = "cancel" Then Me.CancelButton = btn

                        btn.Enabled = objEditable

                        AddButtonHandlers(btn, EventsToRun)

                        btn.ToolTip = objTooltip
                        item.Control = (btn)

                    Case "date"
                        dtpEd = New DateEdit
                        dtpEd.StyleController = App.Objects.myStyleController
                        dtpEd.ToolTip = objTooltip
                        dtpEd.Name = objName


                        dtpEd.Properties.Appearance.Font = New Font(txtFontFamily, txtFontSize, ff)
                        dtpEd.ForeColor = getColor(txtForeColor)
                        dtpEd.BackColor = getColor(txtBackColor)

                        dtpEd.Properties.ReadOnly = IIf(objEditable = True, False, True)

                        Try
                            dtpEd.DataBindings.Add(New Binding("EditValue", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))
                        Catch ex As Exception

                        End Try

                        AddDateEditHandlers(dtpEd, EventsToRun)
                        item.Control = (dtpEd)


                    Case "time"
                        timeEd = New TimeEdit
                        timeEd.StyleController = App.Objects.myStyleController
                        timeEd.ToolTip = objTooltip
                        timeEd.Name = objName
                        timeEd.EditValue = Today
                        timeEd.Properties.Appearance.Font = New Font(txtFontFamily, txtFontSize, ff)
                        timeEd.ForeColor = getColor(txtForeColor)
                        timeEd.BackColor = getColor(txtBackColor)
                        timeEd.Properties.ReadOnly = IIf(objEditable = True, False, True)

                        Try
                            timeEd.DataBindings.Add(New Binding("EditValue", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))
                        Catch ex As Exception

                        End Try

                        AddtimeEditHandlers(timeEd, EventsToRun)
                        item.Control = (timeEd)

                    Case "combobox"
                        cmbEd = New ComboBoxEdit
                        cmbEd.StyleController = App.Objects.myStyleController
                        cmbEd.Name = objName
                        cmbEd.ToolTip = objTooltip

                        cmbEd.EnterMoveNextControl = True
                        cmbEd.Properties.Appearance.Font = New Font(txtFontFamily, txtFontSize, ff)
                        cmbEd.ForeColor = getColor(txtForeColor)
                        cmbEd.BackColor = getColor(txtBackColor)
                        cmbEd.Properties.ReadOnly = IIf(objEditable = True, False, True)

                        cmbEd.Properties.AllowNullInput = DefaultBoolean.True

                        ' Adding Data for the lookupedit

                        Using cmbdst As New DataSet()
                            Using cmbadp As New SqlDataAdapter(objSelectCommand, GetCon)
                                cmbadp.Fill(cmbdst)
                            End Using
                            For t = 0 To cmbdst.Tables(0).Rows.Count - 1
                                cmbEd.Properties.Items.Add(cmbdst.Tables(0).Rows(t)(objDisplayMember))
                            Next
                        End Using

                        Try
                            cmbEd.DataBindings.Add(New Binding("Text", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))
                        Catch ex As Exception

                        End Try

                        AddCMBHandlers(cmbEd, EventsToRun)
                        item.Control = (cmbEd)

                    Case "lookup"
                        lkeEd = New LookUpEdit
                        lkeEd.StyleController = App.Objects.myStyleController
                        lkeEd.Name = objName
                        lkeEd.ToolTip = objTooltip

                        lkeEd.EnterMoveNextControl = True
                        lkeEd.Properties.Appearance.Font = New Font(txtFontFamily, txtFontSize, ff)
                        lkeEd.ForeColor = getColor(txtForeColor)
                        lkeEd.BackColor = getColor(txtBackColor)
                        lkeEd.Properties.ReadOnly = IIf(objEditable = True, False, True)

                        lkeEd.Properties.AllowNullInput = DefaultBoolean.True


                        lkeEd.Tag = String.Format("{0}.{1}.{2}", objSelectCommand, objDisplayMember, objValueMember)
                        ' Adding Data for the lookupedit

                        Using lkedst As New DataSet()
                            Using lkeadp As New SqlDataAdapter(objSelectCommand, GetCon)
                                lkeadp.Fill(lkedst)
                            End Using
                            lkeEd.Properties.DataSource = lkedst.Tables(0)
                        End Using
                        lkeEd.Properties.DisplayMember = objDisplayMember
                        lkeEd.Properties.ValueMember = objValueMember
                        lkeEd.Properties.PopupWidth = PopWidth

                        Try
                            lkeEd.DataBindings.Add(New Binding("EditValue", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))
                        Catch ex As Exception

                        End Try


                        AddLKEHandlers(lkeEd, EventsToRun)
                        item.Control = (lkeEd)

                    Case "gridlookup"

                        glkEd = New GridLookUpEdit
                        glkEd.StyleController = App.Objects.myStyleController
                        glkEd.Name = objName
                        glkEd.ToolTip = objTooltip
                        glkEd.EnterMoveNextControl = True
                        glkEd.Properties.Appearance.Font = New Font(txtFontFamily, txtFontSize, ff)
                        glkEd.ForeColor = getColor(txtForeColor)
                        glkEd.BackColor = getColor(txtBackColor)
                        glkEd.Properties.ReadOnly = IIf(objEditable = True, False, True)

                        Dim glkView As GridView = glkEd.Properties.View
                        glkView.Name = "lv" & objName
                        Dim xTag As New clsTagExtender() With {.SenderMaster = Me.Name, .vAllowSaveLayout = True}

                        Dim hlp As New clsContexMenuHelper(glkView)
                        glkView.Tag = xTag

                        ' Giati einai edw ayto ?
                        glkEd.Tag = String.Format("{0}.{1}.{2}", objSelectCommand, objDisplayMember, objValueMember)


                        ' Adding Data for the gridlookupedit
                        Select Case BoundMode
                            Case "unbound"
                                Using glkdst As New DataSet()
                                    Using glkadp As New SqlDataAdapter(objSelectCommand, GetCon)
                                        glkView.OptionsView.ShowAutoFilterRow = True
                                        glkEd.Properties.ShowFooter = True
                                        glkEd.Properties.PopupFormSize = New Size(PopWidth, PopHeight)
                                        glkView.OptionsView.ColumnAutoWidth = True
                                        glkView.BestFitColumns()
                                        glkadp.Fill(glkdst)
                                    End Using
                                    glkEd.Properties.DataSource = glkdst.Tables(0)
                                End Using
                                glkEd.Properties.DisplayMember = objDisplayMember
                                glkEd.Properties.ValueMember = objValueMember

                            Case "bound"
                                Using glkdst As New DataSet()
                                    Using glkadp As New SqlDataAdapter(objSelectCommand, GetCon)
                                        glkView.OptionsView.ShowAutoFilterRow = True
                                        glkEd.Properties.ShowFooter = True
                                        glkEd.Properties.PopupFormSize = New Size(PopWidth, PopHeight)
                                        glkView.OptionsView.ColumnAutoWidth = True
                                        glkView.BestFitColumns()
                                        glkadp.Fill(glkdst)
                                    End Using
                                    glkEd.Properties.DataSource = glkdst.Tables(0)
                                End Using
                                glkEd.Properties.DisplayMember = objDisplayMember
                                glkEd.Properties.ValueMember = objValueMember

                                glkEd.DataBindings.Add(New Binding("EditValue", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))

                            Case "server"
                                ' Enable server mode.
                                'glkEd.Properties.ServerMode = True
                                'Generate the connection string to the database on  SQL Server.
                                XpoDefault.ConnectionString = GetXPOCon()
                                ' Create Session
                                session1 = New Session
                                Dim classInfo As XPClassInfo = session1.GetClassInfo(GetType(clsSearchView_L))
                                classInfo.AddAttribute(New PersistentAttribute(objSelectCommand))
                                ' Create an XPServerCollectionSource object.
                                Dim criteria As CriteriaOperator = CriteriaOperator.Parse(SMCriteria)
                                Dim xpServerCollectionSource1 = New XPServerCollectionSource(session1, classInfo, criteria)

                                ' Bind the grid control to the data source.
                                glkEd.Properties.DataSource = xpServerCollectionSource1
                                glkEd.Properties.DisplayMember = objDisplayMember
                                glkEd.Properties.ValueMember = objValueMember

                                glkEd.DataBindings.Add(New Binding("EditValue", BS(0), DataSt.Tables(0).Columns(objColumn).ColumnName, True))

                        End Select


                        AddGLKHandlers(glkEd, EventsToRun)

                        item.Control = (glkEd)

                    Case "crystalviewer"
                        ' KANONIKA prepei na ftiaxtei oli i forma (to label, o crystalviewer klp)
                        crViewer = New xcntlCrystalViewer
                        crViewer.Report = crRepPath
                        crViewer.ReportParams = crRepParams

                        item.Control = crViewer

                    Case "axcrystalviewer"
                        ' KANONIKA prepei na ftiaxtei oli i forma (to label, o crystalviewer klp)
                        'axcrViewer = New xcntlAxCrystalViewer

                        axcrViewer = New xcntlAxCrystalViewer(crRepPath, crRepParams)
                        item.Control = axcrViewer


                    Case "filesmanipulation"
                        ' KANONIKA prepei na ftiaxtei oli i forma (to label, ta opendialog klp)
                        filesManip = New xcntlFiles
                        filesManip.FileObject = fmFilesObject

                        item.Control = filesManip


                    Case "image"
                        piccounter = piccounter + 1
                        picEd = New PictureEdit
                        picEd.StyleController = App.Objects.myStyleController
                        picEd.Name = objName
                        picEd.Properties.ReadOnly = IIf(objEditable = True, False, True)
                        picEd.ToolTip = objTooltip
                        picEd.Properties.SizeMode =
                            DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze

                        Try
                            picEd.DataBindings.Add(New Binding("Image", DataSt.Tables(0), objColumn, True))
                        Catch ex As Exception

                        End Try

                        AddPictureEditHandlers(picEd, EventsToRun)

                        item.Control = (picEd)

                    Case "gridcontrol"
                        gridC = New GridControl
                        gridC.Name = objName

                        AddGridControlHandlers(gridC, EventsToRun)

                        Select Case BoundMode
                            Case "server"
                                ' Enable server mode.
                                'gridC.ServerMode = True
                                'Generate the connection string to the database on  SQL Server.
                                XpoDefault.ConnectionString = GetXPOCon()
                                ' Create Session
                                session1 = New Session
                                Dim classInfo As XPClassInfo = session1.GetClassInfo(GetType(clsSearchView_L))
                                classInfo.AddAttribute(New PersistentAttribute(objSelectCommand))
                                ' Create an XPServerCollectionSource object.
                                Dim criteria As CriteriaOperator = CriteriaOperator.Parse(SMCriteria)
                                Dim xpServerCollectionSource1 = New XPServerCollectionSource(session1, classInfo, criteria)

                                ' Bind the grid control to the data source.
                                gridC.DataSource = xpServerCollectionSource1


                            Case "bound"
                                gridC.DataSource = BS(0)


                            Case "relation"

                                gridC.DataSource = BS(Convert.ToInt16(RelationTableID))
                                gridC.ShowOnlyPredefinedDetails = True

                            Case "unbound" ' den einai stin pragmatikotita unbound.

                                Dim udgDa As New SqlDataAdapter(objSelectCommand, GetCon) '(objSelectCommand.replace("@k", "'" & Me.Code & "'"), GetCon)
                                Dim udgDs As New DataSet
                                udgDa.Fill(udgDs)
                                udgDa.Dispose()
                                udgDs.Dispose()

                                gridC.DataSource = udgDs.Tables(0)
                        End Select

                        item.Control = gridC

                    Case "vgridcontrol"

                        Dim xTag As New clsTagExtender() With {.SenderMaster = myName, .stripMenuName = objectDetailsPropertiesTable.Rows(i).Item("ShortMenu")}

                        vgridC = New VGridControl
                        vgridC.Name = objName
                        vgridC.LayoutStyle = LayoutViewStyle.MultiRecordView
                        vgridC.Tag = xTag

                        vgridC.OptionsBehavior.DragRowHeaders = True
                        vgridC.OptionsMenu.EnableContextMenu = True

                        AddVGridControlHandlers(vgridC, EventsToRun)

                        Select Case BoundMode
                            Case "bound"
                                vgridC.DataSource = BS(0)


                            Case "relation"
                                'Dim int As Integer = Convert.ToInt16(RelationTableID)

                                'vgridC.DataSource = ConvertTableToRow(DataSt.Tables(int))

                                'vgridC.DataSource = ConvertTableToRow(BS(Convert.ToInt16(RelationTableID)))
                                'vgridC.ShowOnlyPredefinedDetails = True

                            Case "unbound" ' den einai stin pragmatikotita unbound.

                                Dim udgDa As New SqlDataAdapter(objSelectCommand, GetCon) '(objSelectCommand.replace("@k", "'" & Me.Code & "'"), GetCon)
                                Dim udgDs As New DataSet
                                udgDa.Fill(udgDs)
                                udgDa.Dispose()
                                udgDs.Dispose()

                                'vgridC.DataSource = ConvertTableToRow(udgDs.Tables(0))

                                vgridC.DataSource = udgDs.Tables(0)

                        End Select

                        LoadLayoutFromDb(vgridC)
                        item.Control = vgridC


                    Case Else
                        WriteLogEntry("Controls Creation 'Else' " & objDetType.tolower)

                End Select


                item.ContentVisible = objDetVisible
                MainLC.AddItem(item)

            End If

        Next i




        'Load Layouts 

        LoadLayoutFromDb(MainLC)
        MainLC.AllowCustomizationMenu = myAllowSaveLayout

        MainLC.EndUpdate()


        'ayta prepei na ta valw ekei pou ftiaxnw ta gridcontrols

        For Each grc In MainLC.Controls
            If TypeOf grc Is GridControl Then
                GetViews(grc)
            End If
        Next


    End Sub
    ' -------------------
    Private Sub GetViews(ByVal gc As GridControl)

        Dim criteria As String = String.Format("frmName='{0}' and ParentName='{1}' and ctrlName <>'{1}' and Type not like 'rep%' and Type not like 'coldet'", Me.Name, gc.Name)
        Dim Views() As DataRow = objectDetailsPropertiesTable.Select(criteria)

        For i = 0 To UBound(Views)

            Dim nm = Views(i)("ctrlName")
            Dim tg = Views(i)("frmName")
            Dim mnuname = Views(i)("ShortMenu")
            Dim HasEditors As Boolean = CBool(Views(i)("hasEditors"))
            Dim EventsToRun As String = Views(i)("EventsToRun")

            Dim xTag As New clsTagExtender() With {.vhasEditors = HasEditors, .SenderMaster = tg}

            Select Case Views(i)("Type")

                'mainviews
                Case "gridview"
                    Dim gv As New GridView With {.Name = nm, .Tag = xTag}
                    gc.MainView = gv

                    GetViewsDetails(gv)
                    AddGridViewhandlers(gv, EventsToRun)

                    gv.FocusedRowHandle = 0
                    gv.SelectRow(gv.FocusedRowHandle)

                    If mnuname <> String.Empty Then gc.ContextMenuStrip = Create_GridViewContextMenuStrip(gv, mnuname, Me.myViewMode)
                Case "bandview"
                    Dim bv As New AdvBandedGridView With {.Name = nm, .Tag = xTag}

                    Dim BandGeneral As GridBand = New GridBand
                    BandGeneral = bv.Bands.AddBand("test")

                    gc.MainView = bv

                    GetViewsDetails(bv)
                    AddBandViewhandlers(bv, EventsToRun)

                    bv.FocusedRowHandle = 0
                    bv.SelectRow(bv.FocusedRowHandle)

                Case "cardview"
                    Dim cv As New CardView With {.Name = nm, .Tag = xTag}
                    gc.MainView = cv

                    GetViewsDetails(cv)
                    AddCardViewHandlers(cv, EventsToRun)

                    cv.FocusedRowHandle = 0
                    cv.SelectRow(cv.FocusedRowHandle)

                    'subviews
                Case "subgridview"
                    Dim sgv As New GridView(gc) With {.Name = nm, .Tag = xTag}

                    gc.LevelTree.Nodes.Add("Relmdg" & Views(i)("GC_RelationID").ToString, sgv)
                    sgv.ViewCaption = Views(i)("GC_SG_Caption")


                    sgv.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom

                    AddGridViewhandlers(sgv, EventsToRun)

                    sgv.FocusedRowHandle = 0
                    sgv.SelectRow(sgv.FocusedRowHandle)



                Case "subbandview"
                    Dim sbv As New AdvBandedGridView(gc) With {.Name = nm, .Tag = xTag}

                    Dim BandGeneral As New GridBand
                    BandGeneral = sbv.Bands.AddBand("test")

                    gc.LevelTree.Nodes.Add("Relmdg" & Views(i)("GC_relationID").ToString, sbv)
                    sbv.ViewCaption = Views(i)("GC_SG_Caption")

                    AddBandViewhandlers(sbv, EventsToRun)

                    sbv.FocusedRowHandle = 0
                    sbv.SelectRow(sbv.FocusedRowHandle)

                Case "subcardview"
                    Dim scv As New CardView(gc) With {.Name = nm, .Tag = xTag}

                    gc.LevelTree.Nodes.Add("Relmdg" & Views(i)("GC_relationID").ToString, scv)
                    scv.ViewCaption = Views(i)("GC_SG_Caption")

                    AddCardViewHandlers(scv, EventsToRun)

                    scv.FocusedRowHandle = 0
                    scv.SelectRow(scv.FocusedRowHandle)

                Case Else
                    WriteLogEntry("Views Creation 'Else' " & nm)

            End Select

        Next

    End Sub
   
    Private Sub GetViewsDetails(ByVal v As Object)

        Dim criteria As String = String.Format("frmName='{0}' and ctrlName='{1}'", Name, v.name)
        Dim ViewDets() As DataRow = objectDetailsPropertiesTable.Select(criteria)


        Dim RSum As String = ViewDets(0)("rsumFld")
        If RSum <> "" Then
            Dim view As GridView = DirectCast(v, GridView)



            GridviewRunningSums(view, RSum)

        End If

        v.beginupdate()
        LoadLayoutFromDb(v)
        Dim hlper As New clsContexMenuHelper(v)
        v.endupdate()



        AddRepItems(v)

        Dim Editable As Boolean = CBool(ViewDets(0)("Editable"))
        Dim ShowScroll As Boolean = CBool(ViewDets(0)("ShowScroll"))
        Dim ShowSearch As Boolean = CBool(ViewDets(0)("ShowSearch"))
        Dim ShowFooter As Boolean = CBool(ViewDets(0)("ShowFooter"))
        Dim MultiSelect As Boolean = CBool(ViewDets(0)("MultiSelect"))
        Dim AllowSaveLayout As Boolean = CBool(ViewDets(0)("SecLayout"))
        Dim indWidth As Integer = ViewDets(0)("IndicatorWidth")
        Dim indShow As Boolean = CBool(ViewDets(0)("ShowIndicator"))
        Dim FontFamily As String = ViewDets(0)("Font_Family")
        Dim FontSize As Integer = CInt(ViewDets(0)("Font_Size"))
        Dim FontDecoration As String = ViewDets(0)("Font_Decoration")
        Dim Filter As String = ViewDets(0)("Filter")
        Dim ExpandGroupRows As Boolean = CBool(ViewDets(0)("ExpandGroupRows"))
        Dim AutoRowHeight As Boolean = CBool(ViewDets(0)("AutoRowHeight"))


        v.OptionsSelection.EnableAppearanceFocusedRow = False
        v.Appearance.FocusedRow.BackColor = Color.LightGray
        v.Appearance.SelectedRow.BackColor = Color.LightGray

        v.OptionsNavigation.EnterMoveNextColumn = True

        Dim AllowRowSizing As Boolean = CBool(ViewDets(0)("AllowRowSize"))
        v.OptionsCustomization.AllowRowSizing = AllowRowSizing
        v.OptionsView.RowAutoHeight = AutoRowHeight




        'With v.OptionsFind
        '    .AllowFindPanel = True
        '    .ClearFindOnClose = True
        '    .FindMode = FindMode.Always
        '    .FindDelay = 1
        '    .FindFilterColumns = "*"
        '    .HighlightFindResults = True

        'End With



        If ExpandGroupRows = True Then
            v.ExpandAllGroups()
        End If

        If Filter <> "" Then
            Dim filterPairs As String() = Split(Filter, "|")
            For i As Integer = 0 To UBound(filterPairs)
                Dim values As String() = Split(filterPairs(i), ";")
                If values(0).StartsWith("bit") Then
                    v.SetRowCellValue(v.gridcontrol.AutoFilterRowHandle, values(0), CBool(values(1)))
                Else
                    v.SetRowCellValue(v.gridcontrol.AutoFilterRowHandle, values(0), values(1))
                End If
            Next
        End If


        v.IndicatorWidth = indWidth
        v.OptionsView.ShowIndicator = indShow

        If Editable = True Then
            v.OptionsBehavior.Editable = Editable
            v.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom
            v.OptionsDetail.AllowExpandEmptyDetails = True
        Else
            v.OptionsBehavior.Editable = Editable
            v.OptionsView.NewItemRowPosition = NewItemRowPosition.None
        End If



        If ShowScroll = True Then
            v.OptionsView.ColumnAutoWidth = False
            v.VertScrollVisibility = Base.ScrollVisibility.Always
        End If


        v.OptionsView.ShowAutoFilterRow = ShowSearch
        v.OptionsView.ShowFooter = ShowFooter
        v.OptionsSelection.MultiSelect = MultiSelect
        v.tag.vallowsavelayout = AllowSaveLayout
        v.OptionsDetail.AllowOnlyOneMasterRowExpanded = True

        'tha to protimousa visible if expanded
        v.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways
        v.OptionsMenu.ShowGroupSummaryEditorItem = True


        Dim ff As System.Drawing.FontStyle
        Select Case FontDecoration.ToLower
            Case "bold"
                ff = System.Drawing.FontStyle.Bold
            Case "italics"
                ff = System.Drawing.FontStyle.Italic
            Case "strikeout"
                ff = System.Drawing.FontStyle.Strikeout
            Case "underline"
                ff = System.Drawing.FontStyle.Underline
            Case Else
                ff = System.Drawing.FontStyle.Regular

        End Select
        v.Appearance.Row.Font = New Font(FontFamily, FontSize, ff)

        GetViewColumnProperties(v)

    End Sub

    Private Sub GetPivotDetails(ByVal p As PivotGridControl)
        Dim criteria As String = String.Format("frmName='{0}' and ParentName='{1}'", Name, p.Name)
        Dim PivDets() As DataRow = objectDetailsPropertiesTable.Select(criteria)

        For c = 0 To UBound(PivDets)


            Dim fldcount As Integer = Convert.ToInt16(PivDets(c)("Level"))
            Dim nm As String = PivDets(c)("ctrlName")
            Dim typ As String = PivDets(c)("DisplayMember")
            Dim sumtype As Integer = Convert.ToInt16(PivDets(c)("ValueMember"))

            For i = 0 To fldcount - 1

                Dim UnbFld As PivotGridField = _
                New PivotGridField(nm & i.ToString, PivotArea.FilterArea) _
                With {.UnboundType = DevExpress.Data.UnboundColumnType.Object}

                UnbFld.Options.ShowUnboundExpressionMenu = True

                Select Case typ.ToLower
                    Case "display"
                        UnbFld.SummaryDisplayType = CType(sumtype, PivotSummaryDisplayType)
                    Case "type"
                        UnbFld.SummaryType = CType(sumtype, PivotSummaryType)

                    Case Else
                End Select
                p.Fields.Add(UnbFld)

            Next i

        Next c


    End Sub


    Public Sub GetViewColumnProperties(ByVal gv As GridView)
        Dim criteria As String = String.Format("frmName='{0}' and ctrlName='{1}' and type='coldet'", Me.Name, gv.Name)
        Dim Columndetails() As DataRow = objectDetailsPropertiesTable.Select(criteria)

        For i = 0 To UBound(Columndetails)
            Dim colname As String = Columndetails(i)("TableColumn")

            Dim ro As Boolean = CBool(Columndetails(i)("Editable"))
            gv.Columns(colname).OptionsColumn.ReadOnly = IIf(ro = True, False, True)

            Dim af As Boolean = CBool(Columndetails(i)("cAllowFocus"))
            gv.Columns(colname).OptionsColumn.AllowFocus = af

            Dim fm As String = Columndetails(i)("cFilterPopupMode")
            Select Case fm.ToLower
                Case "check"
                    gv.Columns(colname).OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList
                Case "list"
                    gv.Columns(colname).OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.List
                Case "date"
                    gv.Columns(colname).OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.Date
                Case "datealt"
                    gv.Columns(colname).OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateAlt
                Case Else
                    gv.Columns(colname).OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.Default
            End Select

            'gv.Columns(colname).OptionsFilter.AutoFilterCondition = Columns.AutoFilterCondition.Contains

            gv.Columns(colname).AppearanceCell.BackColor = getColor(Columndetails(i)("BackColor").tolower)
            gv.Columns(colname).AppearanceCell.ForeColor = getColor(Columndetails(i)("ForeColor").tolower)

        Next
    End Sub

    Public Sub AddRepItems(ByVal v As Object)

        Dim g As GridControl = v.gridcontrol

        Dim criteria As String = String.Format("frmName='{0}' and ParentName='{1}' and (Type like 'stnd_rep%' or Type like 'ds_rep%')", Me.Name, v.Name)
        Dim RepItems() As DataRow = objectDetailsPropertiesTable.Select(criteria)


        For i = 0 To UBound(RepItems)

            Dim objMask = RepItems(i)("Format_MaskType")
            Dim objFormat = RepItems(i)("Format_MaskEdit")
            Dim txtAlign = RepItems(i)("Text_Align")
            Dim txtFontFamily As String = RepItems(i)("Font_Family")
            Dim txtFontSize = RepItems(i)("Font_Size")
            Dim EventsToRun As String = RepItems(i)("EventsToRun")

            Select Case RepItems(i)("Type").tolower
                Case "stnd_repmemo"
                    Dim repmemo As New RepositoryItemMemoEdit With {.Name = RepItems(i)("ctrlName"), .NullText = " "}

                    If v.OptionsView.RowAutoHeight = True Then
                        repmemo.LinesCount = 0
                    End If

                    g.RepositoryItems.Add(repmemo)


                Case "stnd_repimage"
                    Dim repImage As New RepositoryItemPictureEdit With
                        {.Name = RepItems(i)("ctrlName"), .NullText = " ", .SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze}

                    g.RepositoryItems.Add(repImage)

                Case "stnd_replookup"
                    Dim repLookUp As New RepositoryItemLookUpEdit() With {.Name = RepItems(i)("ctrlName")}

                    Dim cm As String = RepItems(i)("SelectCommand")
                    Using da As New SqlDataAdapter(cm, GetCon)
                        Using ds As New DataSet()
                            da.Fill(ds)
                            repLookUp.DataSource = ds.Tables(0)
                        End Using
                    End Using
                    repLookUp.DisplayMember = RepItems(i)("DisplayMember")
                    repLookUp.ValueMember = RepItems(i)("ValueMember")

                    repLookUp.PopupWidth = RepItems(i)("popWidth")

                    g.RepositoryItems.Add(repLookUp)

                Case "stnd_repgridlookup"
                    Dim repGridLookUp As New RepositoryItemGridLookUpEdit() With {.Name = RepItems(i)("ctrlName")}

                    Dim repglkView As GridView = repGridLookUp.View
                    repglkView.Name = "sreplv" & repGridLookUp.Name
                    Dim xTag As New clsTagExtender() With {.SenderMaster = Me.Name, .vAllowSaveLayout = True}

                    repglkView.Tag = xTag

                    repglkView.OptionsView.ShowAutoFilterRow = True
                    repglkView.OptionsBehavior.ReadOnly = True

                    Try

                        Dim Boundmode = RepItems(i)("BoundMode")
                        Dim objSelectCommand = RepItems(i)("SelectCommand")
                        Dim PopWidth = RepItems(i)("popWidth")
                        Dim PopHeight = RepItems(i)("popHeight")
                        Dim objDisplaymember = RepItems(i)("DisplayMember")
                        Dim objValueMember = RepItems(i)("ValueMember")
                        Dim SMCriteria = RepItems(i)("SMCriteria")


                        Select Case Boundmode
                            Case "bound"
                                Using repglkdst As New DataSet()
                                    Using repglkadp As New SqlDataAdapter(objSelectCommand, GetCon)
                                        repglkView.OptionsView.ShowAutoFilterRow = True
                                        repGridLookUp.ShowFooter = True
                                        repGridLookUp.PopupFormSize = New Size(PopWidth, PopHeight)
                                        repglkView.OptionsView.ColumnAutoWidth = True
                                        repglkView.BestFitColumns()
                                        repglkadp.Fill(repglkdst)
                                    End Using
                                    repGridLookUp.DataSource = repglkdst.Tables(0)
                                End Using
                                repGridLookUp.DisplayMember = objDisplaymember
                                repGridLookUp.ValueMember = objValueMember

                            Case "server"
                                ' Enable server mode.
                                'repGridLookUp.ServerMode = True
                                'Generate the connection string to the database on  SQL Server.
                                XpoDefault.ConnectionString = GetXPOCon()
                                ' Create Session
                                session1 = New Session
                                Dim classInfo As XPClassInfo = session1.GetClassInfo(GetType(clsSearchView_L))
                                classInfo.AddAttribute(New PersistentAttribute(objSelectCommand))
                                ' Create an XPServerCollectionSource object.
                                Dim crit As CriteriaOperator = CriteriaOperator.Parse(SMCriteria)
                                Dim xpServerCollectionSource1 = New XPServerCollectionSource(session1, classInfo, crit)

                                ' Bind the grid control to the data source.
                                repGridLookUp.DataSource = xpServerCollectionSource1
                                repGridLookUp.DisplayMember = objDisplaymember
                                repGridLookUp.ValueMember = objValueMember

                        End Select


                    Catch ex As Exception
                        WriteLogEntry(ex.Message)
                    End Try

                    AddRepGLKHandlers(repGridLookUp, EventsToRun)
                    g.RepositoryItems.Add(repGridLookUp)

                Case "ds_repgridlookup"
                    'ok 

                    Dim repGridLookUp As New RepositoryItemGridLookUpEdit() With {.Name = RepItems(i)("ctrlName")}

                    repGridLookUp.ViewType = GridLookUpViewType.GridView
                    Dim repglkView As GridView = repGridLookUp.View

                    repglkView.Name = "dsreplv" & repGridLookUp.Name
                    Dim xTag As New clsTagExtender() With {.SenderMaster = Me.Name, .vAllowSaveLayout = True}

                    repglkView.Tag = xTag

                    repglkView.OptionsView.ShowAutoFilterRow = True
                    repglkView.OptionsBehavior.ReadOnly = True

                    Dim r As DataRow = GetActionsResult(Me.myGuid, String.Format("{0}.{1}", v.name, repGridLookUp.Name), "ChangeDatasource")
                    WriteLogEntry(String.Format("{0}.{1}", v.name, repGridLookUp.Name))
                    Dim glda As New SqlDataAdapter(r("f2"), GetCon)
                    Dim glds As New DataSet
                    glda.Fill(glds)
                    glda.Dispose()
                    glds.Dispose()

                    repGridLookUp.DataSource = glds.Tables(0)
                    repGridLookUp.DisplayMember = r("f3")
                    repGridLookUp.ValueMember = r("f4")

                    repGridLookUp.PopupFormSize = New Size(RepItems(i)("popWidth"), RepItems(i)("popHeight"))

                    AddRepGLKHandlers(repGridLookUp, EventsToRun)
                    g.RepositoryItems.Add(repGridLookUp)

                Case "stnd_repcombobox"
                    Dim repCombo As New RepositoryItemComboBox() With {.Name = RepItems(i)("ctrlName")}
                    Dim it As String = ""

                    Using cmbdst As New DataSet()
                        Using cmbadp As New SqlDataAdapter(RepItems(i)("SelectCommand"), GetCon)
                            cmbadp.Fill(cmbdst)
                        End Using
                        For t = 0 To cmbdst.Tables(0).Rows.Count - 1
                            it = cmbdst.Tables(0).Rows(t)(RepItems(i)("DisplayMember"))
                            repCombo.Items.Add(it)
                        Next
                    End Using

                    AddRepCMBHandlers(repCombo, EventsToRun)
                    g.RepositoryItems.Add(repCombo)

                Case "stnd_reptext"

                    Dim repText As New RepositoryItemTextEdit With {.Name = RepItems(i)("ctrlName")}

                    repText.Mask.MaskType = objMask
                    repText.Mask.EditMask = objFormat
                    repText.Mask.UseMaskAsDisplayFormat = True
                    repText.Appearance.TextOptions.HAlignment = txtAlign
                    repText.Appearance.Font = New Font(txtFontFamily, txtFontSize)

                    AddRepTXTHandlers(repText, EventsToRun)
                    g.RepositoryItems.Add(repText)

                Case "stnd_reptime"

                    Dim repTime As New RepositoryItemTimeEdit With {.Name = RepItems(i)("ctrlName")}

                    repTime.Mask.MaskType = objMask
                    repTime.Mask.EditMask = objFormat
                    repTime.Mask.UseMaskAsDisplayFormat = True
                    repTime.Appearance.TextOptions.HAlignment = txtAlign
                    repTime.Appearance.Font = New Font(txtFontFamily, txtFontSize)

                    AddRepTimeEditHandlers(repTime, EventsToRun)
                    g.RepositoryItems.Add(repTime)

                Case "stnd_repdate"

                    Dim repDate As New RepositoryItemDateEdit With {.Name = RepItems(i)("ctrlName")}
                    'repDate.NullDate = "1/1/1900"
                    'repDate.NullText = ""
                    'repDate.Mask.MaskType = objMask
                    'repDate.Mask.EditMask = objFormat
                    'repDate.Mask.UseMaskAsDisplayFormat = True
                    'repDate.Appearance.TextOptions.HAlignment = txtAlign
                    'repDate.Appearance.Font = New Font(txtFontFamily, txtFontSize)

                    AddRepDateEditHandlers(repDate, EventsToRun)
                    g.RepositoryItems.Add(repDate)

                Case Else
                    WriteLogEntry("Case Else in repository items creation")

            End Select

            Try
                Dim colname As String = RepItems(i)("TableColumn")
                Dim editorname As String = RepItems(i)("ctrlName")
                v.Columns(colname).ColumnEditName = editorname

                ' WriteLogEntry(RepItems(i)("TableColumn") & RepItems(i)("ctrlName"))
            Catch ex As Exception

                WriteLogEntry("column editor error" & RepItems(i)("-TableColumn") & RepItems(i)("-ctrlName") & ex.Message)
            End Try

        Next

    End Sub

    Public Function Create_GridViewContextMenuStrip(ByVal gv As GridView, ByVal MenuName As String, ByVal ViewMode As String) As ContextMenuStrip
        'Public Function CreateContextMenuStrip() As ContextMenuStrip
        Dim cms As New ContextMenuStrip

        Dim dt As DataTable = CreateContextMenuStripTable(MenuName:=MenuName, ViewMode:=ViewMode)
        For i As Integer = 0 To dt.Rows.Count - 1

            Dim Item As New ToolStripMenuItem
            Item.Text = dt.Rows(i)("Caption")
            Item.Name = dt.Rows(i)("Parameter")
            Item.Enabled = CBool(dt.Rows(i)("enabled"))
            Item.Tag = gv
            'Item.ShortcutKeys = Keys.Alt Or Keys.A
            cms.Items.Add(Item)

            AddHandler Item.Click, AddressOf gridviewContextMenu_Item_Click
        Next i

        Return cms
    End Function
    Private Sub gridviewContextMenu_Item_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim it As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)
        Dim gv As GridView = TryCast(it.Tag, GridView)

        Dim gridviewcontrolName As String = (gv.GridControl.Name & "." & gv.Name).ToString

        Select Case it.Name.ToLower
            Case "insertrow"
                DoInsertRow(gv)
            Case "refreshrow"
                ForceEditorValidation = True
                DoRefreshRow(gv)

                'gv.ValidateEditor()
            Case "deleterow"
                gv.DeleteSelectedRows()
            Case Else
                DoActions(Me.myGuid, gridviewcontrolName, it.Name)
        End Select

        ' MessageBox.Show(it.Name)

    End Sub


End Class
