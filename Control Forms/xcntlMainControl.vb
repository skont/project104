Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.DXErrorProvider
Imports System.Data.SqlClient
Imports DevExpress.Xpo
Imports DevExpress.XtraPivotGrid
Imports DevExpress.XtraGrid
Imports System.Threading
Imports DevExpress.XtraVerticalGrid



Public Class xcntlMainControl
    Inherits XtraForm


    ' EDITORS Decalrations

    Dim WithEvents txtEd As TextEdit
    Dim WithEvents mtxEd As MemoEdit
    Dim WithEvents chkEd As CheckEdit
    Dim WithEvents lkeEd As LookUpEdit
    Dim WithEvents dtpEd As DateEdit
    Dim WithEvents btn As SimpleButton
    Dim WithEvents glkEd As GridLookUpEdit
    Dim WithEvents lbl As LabelControl
    Dim WithEvents timeEd As TimeEdit
    Dim WithEvents picEd As PictureEdit
    Dim WithEvents gridC As GridControl
    Dim WithEvents cmbEd As ComboBoxEdit
    Dim WithEvents pvg As PivotGridControl
    Dim WithEvents crViewer As xcntlCrystalViewer
    Dim WithEvents axcrViewer As xcntlAxCrystalViewer
    Dim WithEvents filesManip As xcntlFiles
    Dim WithEvents vgridC As VGridControl


    Dim masterdetailsCMBuilder(0) As SqlCommandBuilder


    ' PROPERTIES Declarations
    Public Property bsource() As BindingSource()
    Public Property Selectcommandcounter() As Integer
    Public Property masterDetailsDSet() As DataSet
    Public Property MasterDetailsAdapter() As SqlDataAdapter()
    Public Property HasChanges As Boolean
    Public Property session1() As Session
    Public Property myViewMode As String

    'Public Property myCriteria() As String

    Public Property myName As String
    'Public Property myCaption() As String
    Public Property myImage As String
    Public Property mySelectCommand As String
    Public Property myJoins As String
    Public Property myRelatedColumns As String
    Public Property myAllowSaveLayout As Boolean
    Public Property objectDetailsPropertiesTable As DataTable
    Public Property objectPropertiesTable As DataTable

    Public Property myGuid As Guid
    Public Property myParentGuid As Guid

    Public Property myCancelCloseForm As Boolean

    'when in POPUP Form ...or maybe not
    Public Property myControlsToUpdate As String

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        App.Objects.curMainControl = Me

    End Sub

    Public Sub New(ByVal pg As Guid)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        myGuid = Guid.NewGuid
        myParentGuid = pg
        myCancelCloseForm = False
        App.Collections.MainControl.Add(myGuid, Me)



        App.Objects.curMainControl = Me

    End Sub


    Public Sub New(ByVal pg As Guid, ByVal PropertiesRow As DataRow)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        myName = PropertiesRow("f0")
        Name = PropertiesRow("f0")
        ' myCaption = PropertiesRow("f2")
        myImage = PropertiesRow("f3")
        myViewMode = PropertiesRow("f4")
        mySelectCommand = PropertiesRow("f5")
        'myCriteria = PropertiesRow("f6")
        myRelatedColumns = PropertiesRow("f7")
        myJoins = PropertiesRow("f8")
        myAllowSaveLayout = PropertiesRow("f9")
        myGuid = Guid.NewGuid
        myParentGuid = pg
        myCancelCloseForm = False


        Dim q(9) As String
        For i = 0 To 9
            Dim prop As String = IIf(PropertiesRow("f" & (i + 10).ToString) Is Nothing, "", PropertiesRow("f" & (i + 10).ToString))
            q(i) = prop
        Next


        Try
            myControlsToUpdate = PropertiesRow("props")
        Catch ex As Exception
            myControlsToUpdate = ""
        End Try


        objectDetailsPropertiesTable = CreateObjectDetailsPropertiesTable(Me, myViewMode, q)
        objectPropertiesTable = CreateObjectPropertiesTable(Me)

        App.Objects.errProvider = New DXErrorProvider
        App.Collections.MainControl.Add(myGuid, Me)




        App.Objects.curMainControl = Me


    End Sub

    Private Sub xcntlMainControl_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        'If e.KeyCode = Keys.Return OrElse e.KeyCode = Keys.Enter Then
        '    DoActions(Me.myGuid, App.Objects.curFocusedControl.name.ToString, e.KeyCode.ToString)
        'End If
    End Sub

    Private Sub xcntlMainControl_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Dim wait As New clsWait("Παρακαλώ Περιμένετε ...", "Συλλογή Δεδομένων", New Size(250, 50))

        App.Objects.myMainForm.Cursor = Cursors.WaitCursor

        xcntlMainControlLC.Tag = myName

        'WriteLogEntry("Before Create Dataset")
        CreateDataSet()

        'WriteLogEntry("Before Do Actions")
        DoActions(Me.myGuid, "", "load")
        'WriteLogEntry("After Do Actions")
        App.Objects.myMainForm.Cursor = Cursors.Default
        'wait.Dispose()


    End Sub


    Private Sub xcntlMainControl_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

        ' ayti i allagi egine giati otan allaxa to xcntlmaincontrol apo user control se forma 
        ' den rwtage na swsei to layout etsi douleyei kai vlepoume (28/2/11)
        If Me.xcntlMainControlLC.IsModified Then
            SaveLayoutToDb(Me.xcntlMainControlLC)
        End If


        'If Me.masterDetailsDSet Is Nothing Then Exit Sub

        Dim CancelAction As Boolean = False
        e.Cancel = CancelAction

        Try

            For count As Integer = 0 To Me.Selectcommandcounter
                Me.bsource(count).EndEdit()
            Next

            CancelAction = Me.masterDetailsDSet().HasChanges()

        Catch ex As Exception
            CancelAction = False
        End Try

        Select Case CancelAction
            Case True
                DoActions(Me.myGuid, "X-True", "Close")
            Case False
                DoActions(Me.myGuid, "X-False", "Close")
        End Select

        e.Cancel = CancelAction


    End Sub
End Class
