Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Public Class FrmMain

    Private Sub CmdOpen_Click(sender As Object, e As EventArgs) Handles CmdOpen.Click

        Try

            'Declare the DLL for calculating the hashes

            Dim RPHG As New ReportingPackageHashGenerator.ClsRPHashGenerator

            'Declare the hash hresults

            Dim HashResult As New ReportingPackageHashGenerator.ClsRPHashGenerator.ClsHashResult

        'Add an event handler to show the progress (event is raised from the DLL)

        AddHandler RPHG.EventFileProcessed, AddressOf Event_RPHGFileProcessed

            'Selet the RP

            Dim OpenFileDialog As New OpenFileDialog With {
            .Title = "Select Report Package or stand-alone document",
            .Filter = "All files|*.*"
        }

            Dim IntDialog As Integer = OpenFileDialog.ShowDialog()

        'Íf nothing selected then exit procedure. otherwise continue

        If IntDialog = DialogResult.Cancel Then Exit Sub

        'Refresh form

        TxtSelectedRP.Text = Path.GetFileName(OpenFileDialog.FileName)
        LVOverview.Items.Clear()
        TxtHash.Text = ""
        CustomProgressBar.Value = 0

        Application.DoEvents()

            'Get the extension

            Dim StrExtension = Path.GetExtension(OpenFileDialog.FileName).ToUpper()

            '==================================
            ' 2025-01-27: j.urlus@nba.nl
            ' Added support for Report Package 1.0., now also supporting files with extension .xbri (Inline XBRL report package) and .xbr (Non-Inline XBRL report package)
            ' We should ignore the fact that the extensions are supported when written with capitals
            '==================================

            If String.Equals(StrExtension, ".ZIP") Or String.Equals(StrExtension, ".XBRI") Or String.Equals(StrExtension, ".XBR") Then

                'Calculated the hash results for the RP

                HashResult = RPHG.CreateHash(OpenFileDialog.FileName)

            Else

                HashResult = RPHG.CreateHashFromSingleFile(OpenFileDialog.FileName)

        End If

        'Show overall hash

        TxtHash.Text = HashResult.ReportingPackageHash

        'Show individiual hashes

        Dim Listitem As New ListViewItem

        For Each FileItem As ReportingPackageHashGenerator.ClsRPHashGenerator.ClsFile In HashResult.FileList

            Listitem = LVOverview.Items.Add(FileItem.FileName)
            Listitem.SubItems.Add(FileItem.FileHash)

        Next

        'Clear all

        RPHG = Nothing
        HashResult = Nothing

        Catch ex As Exception

        Dim st As New StackTrace(True)
            st = New StackTrace(ex, True)

            Dim StrErrorBuilder As New StringBuilder

            StrErrorBuilder.AppendLine($"An error has been detected in the Report Package Hash Generator.")
            StrErrorBuilder.AppendLine()
            StrErrorBuilder.AppendLine($"Please send the details and the circumstances about the error To sbr@nba.nl")
            StrErrorBuilder.AppendLine()
            StrErrorBuilder.AppendLine($"Version : {My.Application.Info.Version}")
            StrErrorBuilder.AppendLine($"Module : {ex.TargetSite.DeclaringType.Name}")
            StrErrorBuilder.AppendLine($"Procedure : {ex.TargetSite.Name}")
            StrErrorBuilder.AppendLine($"Row : {st.GetFrame(0).GetFileLineNumber()}")
            StrErrorBuilder.Append($"Description : {ex.Message}")

            MsgBox(StrErrorBuilder.ToString, MsgBoxStyle.Critical, "Report Package Hash Generator")

        End Try

        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click

        Me.Close()

    End Sub

    Private Sub CmdExportToClipboard_Click(sender As Object, e As EventArgs) Handles CmdExportToClipboard.Click

        Dim Table As String = ""

        'Got the idea from https://social.msdn.microsoft.com/Forums/vstudio/en-US/90290aa0-c8a0-4781-8ebf-8aa61b8b7921/how-to-use-clipboard-copy-listview-data-to-a-word-document-table-in-vbnet?forum=vbgeneral
        'This creates a table from the listview content, which is copied to the clipboard

        Dim Headers = (From ch As ColumnHeader In LVOverview.Columns
                       Let header = DirectCast(ch, ColumnHeader)
                       Select header.Text).ToArray()

        Dim Items = (From item As ListViewItem In LVOverview.Items
                     Let lvi = DirectCast(item, ListViewItem)
                     Select (From subitem In lvi.SubItems
                             Let si = DirectCast(subitem, ListViewItem.ListViewSubItem)
                             Select si.Text).ToArray()).ToArray()

        'If a stand-alone document is selected, the result of the hash of the report package is 'N/A'. If this is the case. don't show the result of the reporting package

        If Not String.Equals(TxtHash.Text, "N/A") Then

            Table = $"Name report package{vbTab}Overall hash{Environment.NewLine}"

            Table &= $"{TxtSelectedRP.Text}{vbTab}{TxtHash.Text}{Environment.NewLine}{Environment.NewLine}"

        End If

        Table &= $"{String.Join(vbTab, Headers)}{Environment.NewLine}"

        For Each a As String() In Items
            Table &= $"{String.Join(vbTab, a)}{Environment.NewLine}"
        Next

        Clipboard.SetText(Table)

    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click

        Me.Close()

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click

        MsgBox($"This tool calculates a hash of a Report Package or a stand-alone file. This hash can be used to authenticate the audit object.{vbNewLine}{vbNewLine}More information about this application can be found at https://www.nba.nl/reporting-package-hash-generator or https://github.com/JacquesUrlusNBA/ReportPackageHashGenerator{vbNewLine}{vbNewLine}Questions or remarks can be sent to sbr@nba.nl.{vbNewLine}{vbNewLine}This is version: {Application.ProductVersion}", MsgBoxStyle.OkOnly, "Report Package Hash Generator")

    End Sub
    Private Sub Event_RPHGFileProcessed(maxFile As Integer, currentFile As String)

        'Refresh custom progress bar(see the public class CustomProgressBar)

        CustomProgressBar.Maximum = maxFile
        CustomProgressBar.Minimum = 0
        CustomProgressBar.Step = 1
        CustomProgressBar.CustomText = currentFile
        CustomProgressBar.PerformStep()

        Application.DoEvents()

    End Sub

End Class
