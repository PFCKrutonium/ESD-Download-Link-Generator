﻿Imports System.IO
Imports System.Net.Http

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "ESD Download Link Extractor"
        txtLink.Enabled = False
        rdoFast.Checked = True
        rdoSlow.Checked = False
    End Sub

    Private Sub cmdDownload_Click(sender As Object, e As EventArgs) Handles cmdDownload.Click
        If rdoFast.Checked = True Then
            Call FastRingDL()
        ElseIf rdoSlow.Checked = True Then
            Dim result = MsgBox("Open ESD Database?", MsgBoxStyle.YesNo, "Build List")
            If result = MsgBoxResult.Yes Then
                Process.Start("www.ms-vnext.net/Win10esds/")
            End If
            Call SlowRingDL()
        Else
            MsgBox("Wat.")
        End If
    End Sub
    Public Property CorD As String
    Public Property ESDName As String
    Public Property SHA1 As String = 0
    Public Property Datee As String = 0

    Private Sub SlowRingDL()
        ESDName = InputBox("Filename of the ESD you want.", "ESD File Name")
        Do Until SHA1.Count = 40
            SHA1 = InputBox("SHA1 of the ESD you want.", "SHA1 of the ESD File")
        Loop
        Do Until Datee.Count = 7
            Datee = InputBox("Date of the Build you want 'year/month' like 2015/09", "Date that it was built")
        Loop
        CorD = "/c/updt"
        Dim DLLink As String = "http://b1.download.windowsupdate.com" & CorD & "/" & Datee & "/" & ESDName & "_" & SHA1 & ".esd"
        If RemoteFileOk(DLLink) = False Then
            CorD = "/d/updt"
            DLLink = "http://b1.download.windowsupdate.com" & CorD & "/" & Datee & "/" & ESDName & "_" & SHA1 & ".esd"
            If RemoteFileOk(DLLink) = False Then
                MsgBox("Some of the info you entered appears to be wrong. Please try again!")
            Else
                txtLink.Text = DLLink
                txtLink.Enabled = True
            End If
        Else
            txtLink.Text = DLLink
            txtLink.Enabled = True
        End If
    End Sub

    Private Function RemoteFileOk(ByVal Url As String) As Boolean
        Using client As New HttpClient,
            responseTask As Task(Of HttpResponseMessage) = client.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead)
            responseTask.Wait()
            Using response As HttpResponseMessage = responseTask.Result
                Return response.IsSuccessStatusCode
            End Using
        End Using
    End Function
    Private Sub FastRingDL()
        Dim ProcGo As New ProcessStartInfo
        ProcGo.Arguments = "–ExecutionPolicy Bypass Get-BitsTransfer -AllUsers | Select -ExpandProperty FileList | Select -ExpandProperty RemoteName"
        ProcGo.RedirectStandardOutput = True
        ProcGo.FileName = "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe"
        ProcGo.UseShellExecute = False
        ProcGo.CreateNoWindow = True
        Dim ProcOut As Process = Process.Start(ProcGo)
        ProcOut.WaitForExit()
        Dim OutPut As String = ProcOut.StandardOutput.ReadToEnd
        Dim Split As List(Of String) = OutPut.Split("http").ToList
        For Each item In Split
            If item.ToUpper.Contains(".ESD") Then
                txtLink.Enabled = True
                txtLink.Text = "h" & item
                Exit For
            End If
        Next
        If txtLink.Enabled = False Then
            MsgBox("Sorry, failed to retrieve the link - is it currently downloading? It should be.")
        Else
            MsgBox("Please note that this download link is time sensitive.")
        End If
    End Sub

    Private Sub rdoSlow_CheckedChanged(sender As Object, e As EventArgs) Handles rdoSlow.CheckedChanged
        If rdoSlow.Checked = True Then
            MsgBox("Please keep in mind that this will only work for files that Microsoft has actually released!")
        End If
    End Sub

    Private Sub cmdCpyToClp_Click(sender As Object, e As EventArgs) Handles cmdCpyToClp.Click
        My.Computer.Clipboard.Clear()
        My.Computer.Clipboard.SetText(txtLink.Text)
    End Sub

    Private Sub cmdAbout_Click(sender As Object, e As EventArgs) Handles cmdAbout.Click
        AboutBox1.ShowDialog()
    End Sub
End Class
