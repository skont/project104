Imports System.Data.SqlClient

Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata
Imports DevExpress.XtraGrid

Public Class xfrmGeneralSearch

    Private _SelCmd As String

    Public Property SelCmd() As String
        Get
            SelCmd = _SelCmd
        End Get
        Set(ByVal value As String)
            _SelCmd = value
        End Set
    End Property

    Private _Filter As String
    Public Property Filter() As String
        Get
            Filter = _Filter
        End Get
        Set(ByVal value As String)
            _Filter = value
        End Set
    End Property


    Private _ValueColumn As String
    Public Property ValueColumn() As String
        Get
            Return _ValueColumn
        End Get
        Set(ByVal value As String)
            _ValueColumn = value
        End Set
    End Property

    Private _SearchFormMode As String
    Public Property SearchFormMode() As String
        Get
            Return _SearchFormMode
        End Get
        Set(ByVal value As String)
            _SearchFormMode = value
        End Set
    End Property


    Private _ResChecks As ArrayList
    Public Property ResChecks() As ArrayList
        Get
            Return _ResChecks
        End Get
        Set(ByVal value As ArrayList)
            _ResChecks = value
        End Set
    End Property




    Private _MultiSelect As Boolean
    Public Property Multiselect() As Boolean
        Get
            Return _MultiSelect
        End Get
        Set(ByVal value As Boolean)
            _MultiSelect = value
        End Set
    End Property



    Private Sub xfrmGeneralSearch_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        GridControl1.ForceInitialize()

        GridView1.FocusedRowHandle = GridControl.AutoFilterRowHandle
        GridView1.FocusedColumn = GridView1.VisibleColumns(1)
        GridView1.ShowEditor()

    End Sub

    Public Sub New(ByVal scmd As String, ByVal SM As String)

        '' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        If SM.ToUpper = "SERVER" Then

            ' Generate the connection string to the AdventureWorks database on local SQL Server.
            XpoDefault.ConnectionString = GetXPOCon()

            ' Create a Session object.
            Using session1 As Session = New Session()
                ' Create an XPClassInfo object corresponding to the Person_Contact class.
                Dim classInfo As XPClassInfo = session1.GetClassInfo(GetType(clsSearchView_L))
                classInfo.AddAttribute(New PersistentAttribute(scmd))
                ' Create an XPServerCollectionSource object.
                Dim xpServerCollectionSource1 As XPServerCollectionSource = New XPServerCollectionSource(session1, classInfo) With {.AllowEdit = True}
                ' Enable server mode.
                'GridControl1.ServerMode = True
                ' Bind the grid control to the data source.
                GridControl1.DataSource = xpServerCollectionSource1
            End Using

            GridView1.OptionsView.ShowAutoFilterRow = True

        Else
            Dim com As String = scmd
            Using ds As New DataSet()
                Dim da As New SqlDataAdapter(com, GetCon)
                da.Fill(ds)
                GridView1.OptionsView.ShowGroupPanel = False
                GridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None
                GridView1.OptionsView.ShowAutoFilterRow = True
                GridView1.OptionsBehavior.Editable = Multiselect
                GridControl1.DataSource = ds.Tables(0)
                da.Dispose()
            End Using
        End If

    End Sub



    Private Sub xfrmSearch_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        CenterToScreen()
        KeyPreview = True

        MaximizeBox = False
        MinimizeBox = False

        AcceptButton = btnOk
        CancelButton = btnCancel

        Dim xTag As New clsTagExtender() With {.SenderMaster = Name}

        GridView1.Tag = xTag


        If _MultiSelect = True Then
            AddCheckColumn()
        End If

        LoadLayoutFromDb(GridView1)
        GridControl1.ForceInitialize()
        GridView1.OptionsBehavior.Editable = True

        GridView1.SetRowCellValue(GridControl.AutoFilterRowHandle, ValueColumn, Filter)

    End Sub


    Public checks As New ArrayList()

    Sub AddCheckColumn()
        GridControl1.ForceInitialize()
        GridView1.BeginUpdate()
        Dim ColumnCheck As DevExpress.XtraGrid.Columns.GridColumn = GridView1.Columns.AddField("Include")
        Dim RepositoryCheck As New DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
        ColumnCheck.ColumnEdit = RepositoryCheck
        ColumnCheck.Name = "Include"
        ColumnCheck.UnboundType = DevExpress.Data.UnboundColumnType.Boolean
        ColumnCheck.VisibleIndex = 0 'Integer.MaxValue
        checks.Clear()
        AddHandler GridView1.CustomUnboundColumnData, AddressOf GridView1_CustomUnboundColumnData
        GridView1.EndUpdate()
    End Sub

    Private Sub GridView1_CustomUnboundColumnData(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs)
        If e.Column.FieldName <> "Include" Then Return

        If e.IsGetData Then
            e.Value = checks.Contains(e.ListSourceRowIndex)
        ElseIf e.IsSetData Then
            Dim c As Boolean = CType(e.Value, Boolean)
            If c Then
                checks.Add(e.ListSourceRowIndex)
            Else
                checks.Remove(e.ListSourceRowIndex)
            End If
        End If
    End Sub


    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        ' User clicked the OK button

        ResChecks = checks
        SaveLayoutToDb(GridView1)
        DialogResult = DialogResult.OK


    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ' User clicked the Cancel button

        SaveLayoutToDb(GridView1)
        DialogResult = DialogResult.Cancel

    End Sub


End Class
