Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography
Imports System.Security.Cryptography.Xml
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml


Public Class ClsRPHashGenerator

    Public Event EventFileProcessed(MaxFiles As Integer, CurrentFile As String)
    ReadOnly StrXMLNamespace As String = $"xmlns:xml={Chr(34)}http://www.w3.org/XML/1998/namespace{Chr(34)}"
    Dim StrLastFile As String  'Used for the error message

    Public Function CreateHashFromSingleFile(StrFileLocation As String) As ClsHashResult

        '==================================
        ' 2021-11-07: j.urlus@nba.nl
        ' A new function added on request to calculate a hash from a single file. We use the same structure to get an identical result.
        '==================================

        Try

            Dim HashResult As New ClsHashResult

            Dim LstClsFile As New List(Of ClsFile)

            'Add file to list of files (LstClsFile)

            Dim FileItem As New ClsFile With {
                            .FileLocationAndName = StrFileLocation,
                            .FileName = Path.GetFileName(StrFileLocation)}

            LstClsFile.Add(FileItem)

            StrLastFile = FileItem.FileName 'Used in error message

            'Let the main application know by an event that a file is being processed.

            RaiseEvent EventFileProcessed(LstClsFile.Count, FileItem.FileName)
            Thread.Sleep(500)

            'Create temparary directory

            Dim StrTMPDir As String = ($"{FileItem.FileName}{Format(Now(), ("yyyyMMddHHmmss"))}").GetHashCode

            Directory.CreateDirectory($"{My.Computer.FileSystem.SpecialDirectories.Temp}/{StrTMPDir}")

            'Copy file to temporary location

            File.Copy(FileItem.FileLocationAndName, $"{My.Computer.FileSystem.SpecialDirectories.Temp}/{StrTMPDir}/{FileItem.FileName}")

            'Change the FileLocationAndName atrribute

            FileItem.FileLocationAndName = $"{My.Computer.FileSystem.SpecialDirectories.Temp}/{StrTMPDir}/{FileItem.FileName}"

            Select Case Path.GetExtension(FileItem.FileLocationAndName).ToUpper

                Case ".XML", ".XBRL", ".XSD", ".HTML", ".XHTML" 'These files can be canonicalized

                    '==================================
                    ' 2021-18-01: j.urlus@nba.nl
                    ' .NET Framework: C14N transform not omitting xml default namespace, see https://github.com/dotnet/runtime/issues/28844
                    ' Somehow the .NET Framework does not completely comply with the CN14 standards of W3C. We need to remove the standard XML namespaces by ourselves.
                    '==================================

                    Dim StrFileText As String = File.ReadAllText(FileItem.FileLocationAndName)

                    If StrFileText.Contains(StrXMLNamespace) Then 'Only remove -- > xmlns:xml="http://www.w3.org/XML/1998/namespace"

                        StrFileText = StrFileText.Replace(StrXMLNamespace, "")
                        File.WriteAllText(FileItem.FileLocationAndName, StrFileText)

                    End If

                    '==================================
                    ' 2021-09-01: j.urlus@nba.nl
                    ' The standard approach to canonicalize a file would be to add the file directly to a memory stream. And canonicalize it from there.
                    ' But due to security reasons it is not allowed to transform XML files automatically if they contain a DTD declaration. This is the case with the catalog.xml.
                    ' The workaround is to use a XMLreader to ignore DTD declarations. The file is loaded to the XMLreader and with an extension to the XMLreader, it is copied to a stream
                    '==================================

                    Dim MemoryStream As New MemoryStream

                    Dim Settings As New XmlReaderSettings With {
                        .DtdProcessing = DtdProcessing.Ignore
                    }

                    Dim XMLreader As XmlReader = XmlReader.Create(FileItem.FileLocationAndName, Settings)
                    XMLreader.ReadOuterXml()
                    XMLreader.CopyTo(MemoryStream)

                    MemoryStream.Position = 0

                    Dim DsigC14WCtransform As New XmlDsigC14NWithCommentsTransform()

                    DsigC14WCtransform.LoadInput(MemoryStream)

                    Dim StreamTransformed As Stream = DirectCast(DsigC14WCtransform.GetOutput(GetType(Stream)), Stream)
                    StreamTransformed.Position = 0

                    Dim Algorithm As HashAlgorithm = SHA256.Create()
                    Dim Bytes As Byte() = Algorithm.ComputeHash(StreamTransformed)

                    Dim Hash As String = Convert.ToBase64String(Bytes)

                    FileItem.FileHash = Hash.Substring(Hash.Length - 16, 16) 'Store the last 16 positions of the hash 

                    XMLreader.Close()

                Case Else 'This should be the other files (images, pdf's etc.)

                    Dim StreamInput As Stream = File.OpenRead(FileItem.FileLocationAndName)
                    StreamInput.Position = 0

                    Dim Algorithm As HashAlgorithm = SHA256.Create()
                    Dim Bytes As Byte() = Algorithm.ComputeHash(StreamInput)

                    Dim Hash As String = Convert.ToBase64String(Bytes)

                    FileItem.FileHash = Hash.Substring(Hash.Length - 16, 16) 'Store the last 16 positions of the hash 

                    StreamInput.Close()

            End Select

            HashResult.ReportingPackageHash = "N/A" 'Set Reporting package hash to no available.
            HashResult.FileList = LstClsFile

            'Delete the file on the temporary location

            File.Delete(FileItem.FileLocationAndName)
            Directory.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "/" & StrTMPDir)

            Return HashResult

        Catch ex As Exception

            Dim st As New StackTrace(True)
            st = New StackTrace(ex, True)
            Dim StrError As String = ""

            StrError = "An error has been detected in the Reporting Package Hash Generator." & Environment.NewLine & Environment.NewLine
            StrError &= "Please send the details and the circumstances about the error to esef@nba.nl" & Environment.NewLine & Environment.NewLine
            StrError &= "Version :" & My.Application.Info.Version.ToString & Environment.NewLine
            StrError &= "Module :" & ex.TargetSite.DeclaringType.Name & Environment.NewLine
            StrError &= "Procedure :" & ex.TargetSite.Name & Environment.NewLine
            StrError &= "Row :" & st.GetFrame(0).GetFileLineNumber().ToString & Environment.NewLine

            If Not String.IsNullOrEmpty(StrLastFile) Then

                StrError &= "File :" & StrLastFile & Environment.NewLine

            End If

            StrError &= "Description :" & ex.Message

            MsgBox(StrError, MsgBoxStyle.Critical, "Reporting Package Hash Generator")

        End Try

    End Function


    Public Function CheckCoCFilingDocument(StrFile As String) As Boolean

        '==================================
        ' 2025-11-06: j.urlus@nba.nl
        ' According to the Dutch Reporting Manual  G3-6-3_4: The filename of the separate Inline XBRL document for filing purposes MUST Match the “kvk-{date}-{lang}.html” pattern.
        '==================================

        ' Check the pattern

        Dim BoolPatternMatch = Regex.Match(StrFile, "^kvk-(\d{4}-\d{2}-\d{2})-(en|du|fr|nl)\.html$")

        If Not BoolPatternMatch.Success Then Return False

        ' Check if it is a valid date

        Dim DT As DateTime
        Return DateTime.TryParseExact(BoolPatternMatch.Groups(1).Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, DT)

    End Function

    Public Function CreateHash(StrRPZIPLocation As String) As ClsHashResult

        Dim BoolMainFolder As Boolean = False 'To identify if the variable of the main folder is set
        Dim StrMainFolder As String = ""
        Dim StrFolder As String = ""

        Try

            Dim HashResult As New ClsHashResult

            Dim LstClsFile As New List(Of ClsFile)

            '==================================
            ' 2020-11-09: j.urlus@nba.nl
            ' Extract the reporting package and collect the locations of the files
            '==================================

            Using Archive As ZipArchive = ZipFile.OpenRead(StrRPZIPLocation)

                For Each ZipEntry As ZipArchiveEntry In Archive.Entries

                    '==================================
                    ' 2020-11-09: j.urlus@nba.nl
                    ' Let's assume that the auditors report is a (signed) PDF --> skip this file
                    ' 2025-11-06: jurlus@nba.nl
                    ' No reason for skipping files (unless determined by Regulatory Technical Standards or Reporting Manuals.
                    '==================================

                    '==================================
                    ' 2020-11-09: j.urlus@nba.nl
                    ' To determine the difference between folders and files within a ZIP-file the ExternalAttributes (16=directory, 32=file) could be used.
                    ' Unfortunately not all ZIP programs or -classes support the ExternalAttributes. In this case we use a workaround. We will use the backwardslash (\) at the end to determine if it is a folder or file
                    '==================================

                    'Not all ZIP programs or -classes treats the folder seperator the same way --> noticed during testing

                    Dim StrZipEntry As String = ZipEntry.FullName.Replace("/", "\")

                    If StrZipEntry.Contains("\") Then 'File is in a directory

                        'First create folder

                        StrFolder = StrZipEntry.Substring(0, StrZipEntry.LastIndexOf("\"))

                        'In this case we extract the files to the users Temp folder (C:\Users\<USER>\AppData\Local\Temp)

                        'For deleting the extracted files afterwards, we need to know the main folder

                        If BoolMainFolder = False Then

                            StrMainFolder = StrZipEntry.Substring(0, StrZipEntry.IndexOf("\"))
                            BoolMainFolder = True

                        End If

                        Directory.CreateDirectory(Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, StrFolder))

                        '==================================
                        ' 2025-02-13: j.urlus@nba.nl
                        ' In some cases the archive entries contains empty directories. They can not be copied (as a file) and should not be added to the LstCLSFiles for the calculation of the hash.
                        '==================================

                        If Not StrZipEntry.EndsWith("\") Then


                            'Copy file

                            ZipEntry.ExtractToFile(Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, StrZipEntry), True)

                            'Add file to list of files (LstClsFile)

                            Dim FileItem As New ClsFile With {
                            .FileLocationAndName = Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, StrZipEntry),
                            .FileName = Path.GetFileName(StrZipEntry)
                        }

                            LstClsFile.Add(FileItem)

                        End If

                    Else 'File is in the root of the zip file

                        '==================================
                        ' 2022-08-24: j.urlus@nba.nl
                        ' Support for files in the root directory 
                        '==================================

                        Directory.CreateDirectory(Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "Root"))

                        'Copy file

                        ZipEntry.ExtractToFile(Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "Root\" & Path.GetFileName(StrZipEntry)), True)

                        Dim FileItem As New ClsFile With {
                           .FileLocationAndName = Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "Root\" & Path.GetFileName(StrZipEntry)),
                           .FileName = Path.GetFileName(StrZipEntry)
                       }

                        LstClsFile.Add(FileItem)

                    End If

                Next

            End Using

            '==================================
            ' 2020-11-09: j.urlus@nba.nl
            ' Create hashes from files in list (LstClsFile)
            '==================================

            Dim StrXMLNamespace As String = "xmlns:xml=" & Chr(34) & "http://www.w3.org/XML/1998/namespace" & Chr(34)

            For Each FileItem As ClsFile In LstClsFile

                '==================================
                ' 2020-11-09: j.urlus@nba.nl
                ' According to the ESEF reporting manual images can be stored outside of the Inline XBRL document --> these files can not be canonicalized
                '==================================

                StrLastFile = FileItem.FileLocationAndName

                RaiseEvent EventFileProcessed(LstClsFile.Count, FileItem.FileName)
                Thread.Sleep(500)

                Select Case Path.GetExtension(FileItem.FileLocationAndName).ToUpper

                    Case ".XML", ".XBRL", ".XSD", ".HTML", ".XHTML" 'These files can be canonicalized

                        '==================================
                        ' 2021-18-01: j.urlus@nba.nl
                        ' .NET Framework: C14N transform not omitting xml default namespace, see https://github.com/dotnet/runtime/issues/28844
                        ' Somehow the .NET Framework does not completely comply with the CN14 standards of W3C. We need to remove the standard XML namespaces by ourselves.
                        '==================================

                        Dim StrFileText As String = File.ReadAllText(FileItem.FileLocationAndName)

                        If StrFileText.Contains(StrXMLNamespace) Then 'Only remove -- > xmlns:xml="http://www.w3.org/XML/1998/namespace"

                            StrFileText = StrFileText.Replace(StrXMLNamespace, "")

                            File.WriteAllText(FileItem.FileLocationAndName, StrFileText)

                        End If

                        '==================================
                        ' 2021-09-01: j.urlus@nba.nl
                        ' The standard approach to canonicalize a file would be to add the file directly to a memory stream. And canonicalize it from there.
                        ' But due to security reasons it is not allowed to transform XML files automatically if they contain a DTD declaration. This is the case with the catalog.xml.
                        ' The workaround is to use a XMLreader to ignore DTD declarations. The file is loaded to the XMLreader and with an extension to the XMLreader, it is copied to a stream
                        '==================================

                        Dim MemoryStream As New MemoryStream

                        Dim Settings As New XmlReaderSettings With {
                            .DtdProcessing = DtdProcessing.Ignore
                        }

                        Dim XMLreader As XmlReader = XmlReader.Create(FileItem.FileLocationAndName, Settings)
                        XMLreader.ReadOuterXml()
                        XMLreader.CopyTo(MemoryStream)

                        MemoryStream.Position = 0

                        Dim DsigC14WCtransform As New XmlDsigC14NWithCommentsTransform()

                        DsigC14WCtransform.LoadInput(MemoryStream)

                        Dim StreamTransformed As Stream = DirectCast(DsigC14WCtransform.GetOutput(GetType(Stream)), Stream)
                        StreamTransformed.Position = 0

                        Dim Algorithm As HashAlgorithm = SHA256.Create()
                        Dim Bytes As Byte() = Algorithm.ComputeHash(StreamTransformed)

                        Dim Hash As String = Convert.ToBase64String(Bytes)

                        If CheckCoCFilingDocument(FileItem.FileName) = True Then

                            FileItem.FileHashExcluded = True
                            FileItem.FileHash = "Excluded from hash"

                        Else

                            FileItem.FileHashExcluded = False
                            FileItem.FileHash = Hash.Substring(Hash.Length - 16, 16) 'Store the last 16 positions of the hash 

                        End If

                        XMLreader.Close()

                    Case Else 'This should be the images, pdf's etc.

                        Dim StreamInput As Stream = File.OpenRead(FileItem.FileLocationAndName)
                        StreamInput.Position = 0

                        Dim Algorithm As HashAlgorithm = SHA256.Create()
                        Dim Bytes As Byte() = Algorithm.ComputeHash(StreamInput)

                        Dim Hash As String = Convert.ToBase64String(Bytes)

                        FileItem.FileHash = Hash.Substring(Hash.Length - 16, 16) 'Store the last 16 positions of the hash 

                        StreamInput.Close()

                End Select

            Next

            '==================================
            ' 2020-11-09: j.urlus@nba.nl
            ' We will sort the list on the hashcode. So we do not have to worry about the order and location of folders and files
            '==================================

            Dim LstClsFileSorted = LstClsFile.OrderBy(Function(c) c.FileHash)

            Dim StrTotalHash As String = ""

            'Build a concatenated string of all individual file hashes (sorted by hash)

            For Each FileItem As ClsFile In LstClsFileSorted

                If FileItem.FileHashExcluded = False Then StrTotalHash &= FileItem.FileHash

            Next

            'Create a hash from concatenated hash

            Dim HashStream As Stream = CreateStreamFromText(StrTotalHash)

            Dim AlgorithmTotalHash As HashAlgorithm = SHA256.Create()
            Dim BytesTotalHash As Byte() = AlgorithmTotalHash.ComputeHash(HashStream)

            Dim HashTotalHash As String = Convert.ToBase64String(BytesTotalHash)

            HashResult.ReportingPackageHash = HashTotalHash.Substring(HashTotalHash.Length - 16, 16)
            HashResult.FileList = LstClsFile

            'Delete the temporary files

            Try
                Directory.Delete(Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, StrMainFolder), True)
            Catch ex As Exception

            End Try

            Try
                Directory.Delete(Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "Root"), True)
            Catch ex As Exception

            End Try

            Return HashResult

        Catch ex As Exception

            Dim st As New StackTrace(True)
            st = New StackTrace(ex, True)
            Dim StrError As String = ""

            StrError = "An error has been detected in the Reporting Package Hash Generator." & Environment.NewLine & Environment.NewLine
            StrError &= "Please send the details and the circumstances about the error to sbr@nba.nl" & Environment.NewLine & Environment.NewLine
            StrError &= "Version :" & My.Application.Info.Version.ToString & Environment.NewLine
            StrError &= "Module :" & ex.TargetSite.DeclaringType.Name & Environment.NewLine
            StrError &= "Procedure :" & ex.TargetSite.Name & Environment.NewLine
            StrError &= "Row :" & st.GetFrame(0).GetFileLineNumber().ToString & Environment.NewLine

            If Not String.IsNullOrEmpty(StrLastFile) Then

                StrError &= "File :" & StrLastFile & Environment.NewLine

            End If

            StrError &= "Description :" & ex.Message

            MsgBox(StrError, MsgBoxStyle.Critical, "Reporting Package Hash Generator")

        End Try

    End Function

    Private Function CreateStreamFromText(ByVal TextString As String) As Stream
        Dim Stream As New MemoryStream()
        Dim Writer As New StreamWriter(Stream)
        Writer.Write(TextString)
        Writer.Flush()
        Stream.Position = 0
        Return Stream
    End Function

    Public Class ClsHashResult
        Public Property ReportingPackageHash As String
        Public Property FileList As List(Of ClsFile)

    End Class

    Public Class ClsFile

        Public Property FileLocationAndName As String
        Public Property FileName As String
        Public Property FileHash As String

        '==================================
        ' 2025-11-06: j.urlus@nba.nl
        ' Property added for determining if the file must be excluded from the hash. For example for the file contaning the filing information for the Ducth COC.
        '==================================

        Public Property FileHashExcluded As Boolean

    End Class

End Class


