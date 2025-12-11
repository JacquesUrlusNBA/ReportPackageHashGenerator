<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        Me.CmdOpen = New System.Windows.Forms.Button()
        Me.TxtHash = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CmdClose = New System.Windows.Forms.Button()
        Me.LVOverview = New System.Windows.Forms.ListView()
        Me.CHFilename = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.CHHash = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.CmdExportToClipboard = New System.Windows.Forms.Button()
        Me.TxtSelectedRP = New System.Windows.Forms.TextBox()
        Me.GbRP = New System.Windows.Forms.GroupBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.MenuToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GbHashes = New System.Windows.Forms.GroupBox()
        Me.GbProgress = New System.Windows.Forms.GroupBox()
        Me.CustomProgressBar = New Reporting_package_hash_generator.CustomProgressBar()
        Me.GbRP.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.GbHashes.SuspendLayout()
        Me.GbProgress.SuspendLayout()
        Me.SuspendLayout()
        '
        'CmdOpen
        '
        Me.CmdOpen.Location = New System.Drawing.Point(15, 18)
        Me.CmdOpen.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.CmdOpen.Name = "CmdOpen"
        Me.CmdOpen.Size = New System.Drawing.Size(106, 26)
        Me.CmdOpen.TabIndex = 0
        Me.CmdOpen.Text = "Select"
        Me.CmdOpen.UseVisualStyleBackColor = True
        '
        'TxtHash
        '
        Me.TxtHash.Location = New System.Drawing.Point(80, 18)
        Me.TxtHash.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.TxtHash.Name = "TxtHash"
        Me.TxtHash.ReadOnly = True
        Me.TxtHash.Size = New System.Drawing.Size(182, 20)
        Me.TxtHash.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 20)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(69, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Overall hash:"
        '
        'CmdClose
        '
        Me.CmdClose.Location = New System.Drawing.Point(636, 449)
        Me.CmdClose.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.CmdClose.Name = "CmdClose"
        Me.CmdClose.Size = New System.Drawing.Size(72, 23)
        Me.CmdClose.TabIndex = 3
        Me.CmdClose.Text = "Close"
        Me.CmdClose.UseVisualStyleBackColor = True
        '
        'LVOverview
        '
        Me.LVOverview.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.CHFilename, Me.CHHash})
        Me.LVOverview.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.LVOverview.FullRowSelect = True
        Me.LVOverview.HideSelection = False
        Me.LVOverview.Location = New System.Drawing.Point(3, 47)
        Me.LVOverview.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LVOverview.MultiSelect = False
        Me.LVOverview.Name = "LVOverview"
        Me.LVOverview.Size = New System.Drawing.Size(690, 250)
        Me.LVOverview.TabIndex = 4
        Me.LVOverview.UseCompatibleStateImageBehavior = False
        Me.LVOverview.View = System.Windows.Forms.View.Details
        '
        'CHFilename
        '
        Me.CHFilename.Text = "File name"
        Me.CHFilename.Width = 400
        '
        'CHHash
        '
        Me.CHHash.Text = "Individual hash"
        Me.CHHash.Width = 200
        '
        'CmdExportToClipboard
        '
        Me.CmdExportToClipboard.Location = New System.Drawing.Point(15, 449)
        Me.CmdExportToClipboard.Name = "CmdExportToClipboard"
        Me.CmdExportToClipboard.Size = New System.Drawing.Size(174, 23)
        Me.CmdExportToClipboard.TabIndex = 5
        Me.CmdExportToClipboard.Text = "Copy result to clipboard"
        Me.CmdExportToClipboard.UseVisualStyleBackColor = True
        '
        'TxtSelectedRP
        '
        Me.TxtSelectedRP.Location = New System.Drawing.Point(126, 22)
        Me.TxtSelectedRP.Name = "TxtSelectedRP"
        Me.TxtSelectedRP.Size = New System.Drawing.Size(564, 20)
        Me.TxtSelectedRP.TabIndex = 6
        '
        'GbRP
        '
        Me.GbRP.Controls.Add(Me.CmdOpen)
        Me.GbRP.Controls.Add(Me.TxtSelectedRP)
        Me.GbRP.Location = New System.Drawing.Point(12, 27)
        Me.GbRP.Name = "GbRP"
        Me.GbRP.Size = New System.Drawing.Size(696, 54)
        Me.GbRP.TabIndex = 7
        Me.GbRP.TabStop = False
        Me.GbRP.Text = "Reporting package/stand-alone document"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(3, 1, 0, 1)
        Me.MenuStrip1.Size = New System.Drawing.Size(716, 24)
        Me.MenuStrip1.TabIndex = 8
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'MenuToolStripMenuItem
        '
        Me.MenuToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem, Me.CloseToolStripMenuItem})
        Me.MenuToolStripMenuItem.Name = "MenuToolStripMenuItem"
        Me.MenuToolStripMenuItem.Size = New System.Drawing.Size(50, 22)
        Me.MenuToolStripMenuItem.Text = "Menu"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.CloseToolStripMenuItem.Text = "Close"
        '
        'GbHashes
        '
        Me.GbHashes.Controls.Add(Me.LVOverview)
        Me.GbHashes.Controls.Add(Me.TxtHash)
        Me.GbHashes.Controls.Add(Me.Label1)
        Me.GbHashes.Location = New System.Drawing.Point(12, 143)
        Me.GbHashes.Name = "GbHashes"
        Me.GbHashes.Size = New System.Drawing.Size(696, 300)
        Me.GbHashes.TabIndex = 9
        Me.GbHashes.TabStop = False
        Me.GbHashes.Text = "Hashes"
        '
        'GbProgress
        '
        Me.GbProgress.Controls.Add(Me.CustomProgressBar)
        Me.GbProgress.Location = New System.Drawing.Point(12, 87)
        Me.GbProgress.Name = "GbProgress"
        Me.GbProgress.Size = New System.Drawing.Size(696, 50)
        Me.GbProgress.TabIndex = 10
        Me.GbProgress.TabStop = False
        Me.GbProgress.Text = "Progress"
        '
        'CustomProgressBar
        '
        Me.CustomProgressBar.CustomText = Nothing
        Me.CustomProgressBar.Location = New System.Drawing.Point(9, 19)
        Me.CustomProgressBar.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.CustomProgressBar.Name = "CustomProgressBar"
        Me.CustomProgressBar.Size = New System.Drawing.Size(680, 19)
        Me.CustomProgressBar.TabIndex = 0
        '
        'FrmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(716, 478)
        Me.Controls.Add(Me.GbProgress)
        Me.Controls.Add(Me.GbHashes)
        Me.Controls.Add(Me.GbRP)
        Me.Controls.Add(Me.CmdExportToClipboard)
        Me.Controls.Add(Me.CmdClose)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "FrmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Reporting Package Hash Generator"
        Me.GbRP.ResumeLayout(False)
        Me.GbRP.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.GbHashes.ResumeLayout(False)
        Me.GbHashes.PerformLayout()
        Me.GbProgress.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CmdOpen As Button
    Friend WithEvents TxtHash As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents CmdClose As Button
    Friend WithEvents LVOverview As ListView
    Friend WithEvents CHFilename As ColumnHeader
    Friend WithEvents CHHash As ColumnHeader
    Friend WithEvents CmdExportToClipboard As Button
    Friend WithEvents TxtSelectedRP As TextBox
    Friend WithEvents GbRP As GroupBox
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents MenuToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GbHashes As GroupBox
    Friend WithEvents GbProgress As GroupBox
    Friend WithEvents CustomProgressBar As CustomProgressBar
End Class
