Public Class xfrmGridColumnHelperForm

    Private _Kind As String
    Public Property Kind() As String
        Get
            Return _Kind
        End Get
        Set(ByVal value As String)
            _Kind = value
        End Set
    End Property


    Private Sub xfrmGridColumnFormat_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        CenterToScreen()
        KeyPreview = True

        MaximizeBox = False
        MinimizeBox = False

        Select Case Kind.ToLower
            Case "rename"
                Text = "Rename Στήλης"
                LayoutControlItem3.HideToCustomization()
                LayoutControlItem4.Text = "Τίτλος"

            Case "format"
                Text = "Format Στήλης"
                LayoutControlItem3.Text = "Format Type"
                LayoutControlItem4.Text = "Format String"

        End Select

        btnCancel.Text = "Ακύρο"
        btnOK.Text = "OK"

        With cmbFormatType.Properties.Items
            .Add("None")
            .Add("Numeric")
            .Add("DateTime")
            .Add("Custom")
        End With

        cmbFormatType.SelectedIndex = 0

        AcceptButton = btnOK
        CancelButton = btnCancel

    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        ' User clicked the OK button
        DialogResult = DialogResult.OK
        Close()

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ' User clicked the Cancel button
        DialogResult = DialogResult.Cancel
        Close()

    End Sub
End Class
