Module modNewSubsFormsManipulation
    Public Sub Openfrm(ByVal ParentGuid As Guid, ByVal r As DataRow)
        Select Case r("f1").tolower
            Case "tab"
                Dim tabForm As New xcntlMainControl(ParentGuid, r)
                tabForm.Text = r("f2")
                tabForm.MdiParent = App.Objects.myMainForm

                tabForm.Show()

            Case "pop"
                Dim popForm As New xcntlMainControl(ParentGuid, r)
                popForm.Height = r("h")
                popForm.Width = r("w")
                popForm.Text = r("f2")
                popForm.StartPosition = FormStartPosition.CenterScreen

                popForm.Show()


            Case "modal"
                Dim modForm As xcntlMainControl = New xcntlMainControl(ParentGuid, r)
                modForm.Height = r("h")
                modForm.Width = r("w")
                modForm.Text = r("f2")
                modForm.StartPosition = FormStartPosition.CenterScreen
                'modForm.ShowDialog()

                If modForm.ShowDialog() = DialogResult.OK Then
                    modForm.Dispose()
                End If
        End Select

    End Sub

    Public Sub CloseCurrentForm(ByVal formguid As Guid)

        If Not App.Collections.MainControl(formguid) Is Nothing Then
            Dim f As xcntlMainControl = TryCast(App.Collections.MainControl(formguid), xcntlMainControl)
            Try
                App.Collections.MainControl.Remove(f.myGuid)
                CloseCurrentFormChildForms(f.myGuid)

                App.Objects.curMainControl = TryCast(App.Collections.MainControl(f.myParentGuid), xcntlMainControl)

                WriteLogEntry("Closing: " & f.myName)
                f.Dispose()


            Catch ex As Exception
                WriteLogEntry("Exception: " & ex.Message)
            End Try

        End If
    End Sub

    Public Sub CloseCurrentFormChildForms(ByVal currentformguid As Guid)
        Dim l As New List(Of Guid)

        'poly kalos tropos me to dictionaryentry
        For Each de As DictionaryEntry In App.Collections.MainControl
            Dim f As xcntlMainControl = TryCast(de.Value, xcntlMainControl)
            If f.myParentGuid = currentformguid Then
                'App.Objects.curMainControl = TryCast(App.Collections.MainControl(f.myGuid), xcntlMainControl)
                'DoActions(f.myGuid, "childformclose", "close")

                l.Add(f.myGuid)

            End If
        Next

        For Each g In l
            App.Objects.curMainControl = TryCast(App.Collections.MainControl(g), xcntlMainControl)
            DoActions(g, "childformclose", "close")
        Next


    End Sub

    Public Sub CloseAllOpenForms()


    End Sub

    Public Sub FullAppExit()

        Try
            RunSP("exec up1Logout")

            Application.Exit()
        Catch ex As Exception
            WriteLogEntry("Logout Error: " & ex.Message)
        End Try

    End Sub

    Public Sub OpenHideInitReport()

        Try
            Dim a As New xfrmAxCrysytalViewer(GetAppIniReportPath() & "dummy.rpt", "")
            a.Show()
            a.Hide()
        Catch ex As Exception
            WriteLogEntry("Dummy Report Error: " & ex.Message)
        End Try

    End Sub

End Module
