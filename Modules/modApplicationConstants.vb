Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

Module modApplicationConstants

    Public App As New clsApp(My.Settings.RunMode) 'clsUser

    Public WithEvents _testGC As GridControl
    Public WithEvents _testGV As GridView

End Module
