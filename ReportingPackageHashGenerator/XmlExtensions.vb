Imports System.IO
Imports System.Xml

Public Module XmlExtensions
    <System.Runtime.CompilerServices.Extension>
    Public Function ToStream(ByVal reader As XmlReader) As MemoryStream
        Dim ms As New MemoryStream()
        reader.CopyTo(ms)
        Return ms
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Sub CopyTo(ByVal reader As XmlReader, ByVal s As Stream)
        Dim settings As New XmlWriterSettings With {
            .CheckCharacters = False,
            .CloseOutput = False
        }
        Using writer As XmlWriter = XmlWriter.Create(s, settings)
            writer.WriteNode(reader, True)
        End Using
    End Sub
End Module
