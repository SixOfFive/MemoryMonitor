Imports System.ComponentModel
Imports System.IO

Public Class Form1
    Public isRunning As Boolean = False
    Public ProcChecked As Integer = 0
    'create log file for the current user
    Dim strFile As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\MemoryMonitor.txt"
    Public UserMemoryUsed As Double
    Public PeakUsed As Double

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        On Error Resume Next
        Me.Timer1.Interval = My.Settings.CheckInterval * 1000 'default is 60 seconds
        Debug.Print("Set Interval to " & Me.Timer1.Interval)
        Debug.Print(strFile)
        Dim sw As StreamWriter = File.AppendText(strFile)
        sw.WriteLine(DateTime.Now.ToString & " Startup") 'write out startup time
        sw.Close()
        NotifyIcon1.Icon = Me.Icon
        NotifyIcon1.Visible = True
        NotifyIcon1.Text = "Memory Monitor"

        CheckNow()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer2.Enabled = False
        CheckNow()

    End Sub

    Public Sub CheckNow()
        If isRunning = True Then Exit Sub
        UserMemoryUsed = 0
        isRunning = True
        Dim CurProcCheck As Integer = 0
        Dim tprocName As String = ""
        Dim tprocSize As Double = 0

        Dim AllP() As Process = Process.GetProcesses()
        Dim CurP As Process = Process.GetCurrentProcess()
        Dim lst = (From x As Process In AllP Where x.SessionId = CurP.SessionId Select x).ToArray()
        For Each p As Process In lst
            ListBox1.Items.Add(p.ProcessName)
            CurProcCheck = CurProcCheck + 1
            ProcChecked = ProcChecked + 1
            Me.Label1.Text = ProcChecked & " Processes Checked" & vbCrLf & CurProcCheck & " Processes Scanned"
            Dim z As Long = p.WorkingSet64
            Dim n As String = p.ProcessName
            Debug.Print(p.ProcessName & " " & p.WorkingSet64.ToString)
            UserMemoryUsed = UserMemoryUsed + z

            If tprocSize < z Then
                tprocSize = z
                tprocName = n
            End If
            Dim x As Integer = p.Id
            If CDbl(z) > My.Settings.MemoryLimit Then 'default is 4gb
                ListBox1.Items.Add(n & " " & z & " " & x)
                p.Kill()
                Dim sw As StreamWriter = File.AppendText(strFile)
                sw.WriteLine(DateTime.Now.ToString & " Killed Process " & n & "(" & x & ") Using: " & z)
                sw.Close()
                NotifyIcon1.BalloonTipText = "" & n & " Killed due to exceeding ram usage." & vbCrLf & "Using: " & ((z / 1024000) / 1024).ToString.Substring(0, 4) & " GB"
                NotifyIcon1.ShowBalloonTip(5000)
            End If

        Next
        NotifyIcon1.Text = "" & DateTime.Now.ToString & " " & ((UserMemoryUsed / 1024000) / 1024).ToString.Substring(0, 4) & " GB total (" & tprocName & "/" & ((tprocSize / 1024000) / 1024).ToString.Substring(0, 4) & " GB) "
        Debug.Print("" & DateTime.Now.ToString & " " & ((UserMemoryUsed / 1024000) / 1024).ToString.Substring(0, 4) & " GB used total (" & tprocName & "/" & ((tprocSize / 1024000) / 1024).ToString.Substring(0, 4) & " GB)")
        Debug.Print(DateTime.Now.ToString & " Pass Done " & ProcChecked & " Processes Scanned Total " & CurProcCheck & " Processes Scanned")
        isRunning = False

    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        NotifyIcon1.Visible = False
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        'use a timer to set the window invisible right away
        Me.Visible = False
        MyBase.SetVisibleCore(False)
    End Sub

End Class
