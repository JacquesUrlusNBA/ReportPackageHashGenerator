'==================================
' 2021-02-02: j.urlus@nba.nl
' Got the code for the custom progres bar from https://stackoverflow.com/questions/3529928/how-do-i-put-text-on-progressbar
' The code is adjusted to our needs
'==================================

Public Class CustomProgressBar
    Inherits ProgressBar

    Public Property CustomText() As String

    Public Sub New()

        ' Modify the ControlStyles flags
        'http://msdn.microsoft.com/en-us/library/system.windows.forms.controlstyles.aspx
        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint, True)

    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim rect As Rectangle = ClientRectangle
        Dim g As Graphics = e.Graphics

        ProgressBarRenderer.DrawHorizontalBar(g, rect)
        rect.Inflate(-3, -3)
        If Value > 0 Then
            ' As we doing this ourselves we need to draw the chunks on the progress bar
            Dim clip As New Rectangle(rect.X, rect.Y, CInt(Math.Round((CSng(Value) / Maximum) * rect.Width)), rect.Height)
            ProgressBarRenderer.DrawHorizontalChunks(g, clip)
        End If

        ' Set the Display text
        Dim text As String = CustomText

        Using f As New Font(FontFamily.GenericSerif, 7.8)

            Dim len As SizeF = g.MeasureString(text, f)
            ' Calculate the location of the text (the middle of progress bar)
            Dim location As New Point(Convert.ToInt32((Width / 2) - len.Width \ 2), Convert.ToInt32((Height / 2) - len.Height \ 2))
            ' Draw the custom text
            g.DrawString(text, f, Brushes.Black, location)
        End Using
    End Sub

End Class
