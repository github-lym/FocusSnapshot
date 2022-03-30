using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

//using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FocusSnapshot
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        public UInt32 cbSize;
        public IntPtr hwnd;
        public UInt32 dwFlags;
        public UInt32 uCount;
        public UInt32 dwTimeout;
    }

    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowText(IntPtr hWnd, string text);

        public const UInt32 FLASHW_ALL = 3;

        // Flash continuously until the window comes to the foreground.
        public const UInt32 FLASHW_TIMERNOFG = 12;

        //private string md5_fileList = @"./md5_fileList.txt";

        private List<int> pID_list = new List<int>();
        private Dictionary<int, string> pID_file = new Dictionary<int, string>();
        private string timestamp = string.Empty;
        private string folderPath = string.Empty;
        private const Int32 WM_MOUSEWHEEL = 522; //0x0115;
        private string exePath = string.Empty;
        private const int MaxNum = 20;

        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.ToolTip toolTip1 = new System.Windows.Forms.ToolTip();
            System.Windows.Forms.ToolTip toolTip2 = new System.Windows.Forms.ToolTip();
            //System.Windows.Forms.ToolTip toolTip3 = new System.Windows.Forms.ToolTip();
            toolTip1.SetToolTip(Btn_Browse, "瀏覽並執行批次檔清單");
            toolTip2.SetToolTip(Btn_Snapshot, "擷圖啊  懷疑唷");
            //toolTip3.SetToolTip(Btn_MD5_Check, "計算md5_fileList.txt內的MD5");

            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        // Do the flashing - this does not involve a raincoat.
        public static bool FlashWindowEx(Form form)
        {
            IntPtr hWnd = form.Handle;
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //To scroll
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam);

        // To support flashing.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        private void Btn_Browse_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files|*.txt";
            openFileDialog1.Title = "Select a Text File";

            // Show the Dialog.
            // If the user clicked OK in the dialog and
            // a .CUR file was selected, open it.
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.
                //this.Cursor = new Cursor(openFileDialog1.OpenFile());
                ReadListExecute(openFileDialog1.FileName);
            }
        }

        private void Btn_Snapshot_Click(object sender, EventArgs e)
        {
            IntPtr hWnd; //change this to IntPtr
            Process[] processRunning = Process.GetProcesses();
            Point p = new Point(0, 0);
            string titleText = string.Empty;
            foreach (Process pr in processRunning)
            {
                if (pr.ProcessName == "cmd" && pID_list.Contains(pr.Id))
                {
                    if (!"C:\\Windows\\system32\\cmd.exe".Equals(pr.MainWindowTitle.ToString()))
                    {
                        hWnd = pr.MainWindowHandle; //use it as IntPtr not int

                        ShowWindow(hWnd, 9);
                        ShowWindow(hWnd, 5);
                        SetForegroundWindow(hWnd); //set to topmost

                        return;
                    }
                }
            }

            foreach (Process pr in processRunning)
            {
                if (pr.ProcessName == "cmd" && pID_list.Contains(pr.Id))
                {
                    hWnd = pr.MainWindowHandle; //use it as IntPtr not int

                    ShowWindow(hWnd, 9);
                    ShowWindow(hWnd, 5);
                    SetForegroundWindow(hWnd); //set to topmost

                    SetWindowText(hWnd, pID_file[pr.Id]);
                    ScrollWindow(hWnd, 15000);

                    Thread.Sleep(100);

                    PrintScreen(System.IO.Path.GetFileName(pID_file[pr.Id]).Replace("tbqb.", "").Replace("tcqb.", "").Replace("_tpe", "").Replace("_edu", "").Replace(".bat", ""));

                    Thread.Sleep(100);

                    pr.CloseMainWindow();
                }
            }

            FlashWindowEx(this);
        }

        private void PrintScreen(string jpgFileName)
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            Graphics graphics = Graphics.FromImage(printscreen as Image);

            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

            if (!Directory.Exists(Path.Combine(exePath, folderPath)))
            {
                //新增資料夾
                Directory.CreateDirectory(Path.Combine(exePath, folderPath));
            }

            //string fileFullPath = @".\\" + folderPath + "\\" + DateTime.Now.ToString("yyyyMMdd_HHmmssffffff") + ".jpg";
            //string fileFullPath = @".\\" + folderPath + "\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + jpgFileName + ".jpg";
            string fileFullPath = $"{Path.Combine(exePath, folderPath, timestamp)}_{jpgFileName}.jpg";
            printscreen.Save(fileFullPath, ImageFormat.Jpeg);
        }

        private void PrintTotalScreen()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            Graphics graphics = Graphics.FromImage(printscreen as Image);

            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

            if (!Directory.Exists(Path.Combine(exePath, folderPath)))
            {
                //新增資料夾
                Directory.CreateDirectory(Path.Combine(exePath, folderPath));
            }

            //string fileFullPath = @".\\" + folderPath + "\\" + DateTime.Now.ToString("yyyyMMdd_HHmmssffffff") + ".jpg";
            string fileFullPath = $"{Path.Combine(exePath, folderPath, timestamp)}_total.jpg";

            printscreen.Save(fileFullPath, ImageFormat.Jpeg);
        }

        private void ScrollWindow(IntPtr hwnd, int scrolls)
        {
            SendMessage(hwnd, 522U, scrolls << 16);
        }

        /// <summary>
        /// RedirectStandardOutput + Thread
        /// </summary>

        private void ReadListExecute(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                MessageBox.Show(fullPath + "=>檔案不存在");
                return;
            }

            DialogResult result = MessageBox.Show($"確定執行\n{fullPath}", "請好好確定執行檔案是否正確", MessageBoxButtons.YesNoCancel);

            //string batPath = string.Empty;
            timestamp = string.Empty;
            if (result == DialogResult.Yes)
            {
                string line = string.Empty;
                exePath = System.IO.Directory.GetCurrentDirectory();
                timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                folderPath = DateTime.Now.ToString("yyyy-MM-dd");
                string pTitle = string.Empty;
                int lineCount = 0;
                //StringBuilder retryStr;

                if (!Directory.Exists(Path.Combine(exePath, folderPath)))
                {
                    //新增資料夾
                    Directory.CreateDirectory(Path.Combine(exePath, folderPath));
                }

                try
                {
                    using (StreamReader reader = new StreamReader(fullPath, Encoding.UTF8))
                    {
                        int pID = -1;

                        pID_list.Clear();
                        pID_file.Clear();

                        //string batPath = System.IO.Path.GetFullPath(line).Replace(System.IO.Path.GetFileName(line), "");

                        while ((line = reader.ReadLine()) != null)
                        {
                            string line_cmd = line;

                            if ("--".Equals(line_cmd.Substring(0, 2)))
                            {
                                WriteList($"{line_cmd}", Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"));
                                continue;
                            }

                            if (!File.Exists(line_cmd))
                            {
                                //MessageBox.Show(line_cmd + "=>檔案不存在");

                                //using (FileStream fs = new FileStream(Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                //{
                                //    using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                                //    {
                                //        retryStr = new StringBuilder();
                                //        retryStr.AppendLine($"-- ==> 檔案不存在 {line_cmd}");
                                //        srOutFile.Write(retryStr.ToString());
                                //    }
                                //}

                                WriteList($"-- ==> 檔案不存在 {line_cmd}", Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"));

                                continue;
                            }

                            lineCount++;
                            if (lineCount > MaxNum)
                            {
                                //using (FileStream fs = new FileStream(Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                //{
                                //    using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                                //    {
                                //        retryStr = new StringBuilder();
                                //        retryStr.AppendLine($"{line_cmd}");
                                //        srOutFile.Write(retryStr.ToString());
                                //    }
                                //}

                                WriteList($"{line_cmd}", Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"));

                                continue;
                            }

                            var thread = new Thread(new ThreadStart(() =>
                            {
                                string errMsg = string.Empty;
                                string output = string.Empty;
                                //string batPath = System.IO.Path.GetFullPath(line_cmd).Replace(System.IO.Path.GetFileName(line_cmd), "");
                                //StringBuilder sb;
                                //StringBuilder retryStr;
                                //StringBuilder doneStr;

                                Process p = new Process();
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.RedirectStandardError = true;
                                //p.StartInfo.RedirectStandardInput = true;
                                p.StartInfo.RedirectStandardOutput = true;
                                //p.EnableRaisingEvents = true;

                                p.StartInfo.FileName = line_cmd;
                                p.Start();

                                pID = p.Id;
                                //pID = System.Diagnostics.Process.Start(line).Id;
                                pID_list.Add(pID);
                                pID_file.Add(pID, line_cmd);

                                pTitle = p.MainWindowTitle.ToString();

                                //p.BeginOutputReadLine();
                                //p.BeginErrorReadLine();

                                errMsg = p.StandardError.ReadToEnd();
                                output = p.StandardOutput.ReadToEnd();

                                if (output.Contains("BUILD SUCCESSFUL"))
                                {
                                    //sb = new StringBuilder();
                                    //using (FileStream fs = new FileStream(Path.Combine(folderPath, $"{timestamp}_{System.IO.Path.GetFileName(line_cmd)}.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                    //using (FileStream fs = new FileStream(Path.Combine(exePath, folderPath, $"{timestamp}_{System.IO.Path.GetFileName(pID_file[p.Id]).Replace("tbqb.", "").Replace("tcqb.", "").Replace("_tpe", "").Replace("_edu", "").Replace(".bat", "")}.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                    //{
                                    //    using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                                    //    {
                                    //        sb.AppendLine($"{line_cmd.Replace(exePath, "")} => {output.Replace(exePath.Remove(exePath.Length - 1), "")} : {errMsg}");
                                    //        srOutFile.Write(sb.ToString());
                                    //    }
                                    //}
                                    WriteList($@"{line_cmd.Replace(exePath, "")} => {output.Replace(exePath, "")} : {errMsg}", Path.Combine(exePath, folderPath, $@"{timestamp}_{System.IO.Path.GetFileName(pID_file[p.Id]).Replace("tbqb.", "").Replace("tcqb.", "").Replace("_tpe", "").Replace("_edu", "").Replace(".bat", "")}.txt"));

                                    //doneStr = new StringBuilder();
                                    //using (FileStream fs = new FileStream(Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                    //{
                                    //    using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                                    //    {
                                    //        doneStr.AppendLine($"--{line_cmd}");
                                    //        srOutFile.Write(doneStr.ToString());
                                    //    }
                                    //}

                                    WriteList($@"--{line_cmd}", Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"));
                                }
                                //else if (!string.IsNullOrEmpty(errMsg))
                                else
                                {
                                    //sb = new StringBuilder();
                                    //using (FileStream fs = new FileStream(Path.Combine(folderPath, $"{timestamp}_{System.IO.Path.GetFileName(line_cmd)}_err.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                    //using (FileStream fs = new FileStream(Path.Combine(exePath, folderPath, $"{timestamp}_{System.IO.Path.GetFileName(pID_file[p.Id]).Replace("tbqb.", "").Replace("tcqb.", "").Replace("_tpe", "").Replace("_edu", "").Replace(".bat", "")}_err.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                    //{
                                    //    using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                                    //    {
                                    //        sb.AppendLine($"{line_cmd.Replace(exePath, "")} => {output.Replace(exePath.Remove(exePath.Length - 1), "")} : {errMsg}");
                                    //        srOutFile.Write(sb.ToString());
                                    //    }
                                    //}
                                    //MessageBox.Show("line_cmd :" + line_cmd);
                                    //MessageBox.Show($"0.{line_cmd}");
                                    //MessageBox.Show($"0.{exePath}");
                                    //MessageBox.Show($"1.{line_cmd.Replace(exePath, "")}");
                                    //MessageBox.Show($"2.{output.Replace(exePath, "")}");
                                    //MessageBox.Show($"3.{errMsg}");
                                    //MessageBox.Show($"4.{timestamp}_{System.IO.Path.GetFileName(pID_file[p.Id]).Replace("tbqb.", "").Replace("tcqb.", "").Replace("_tpe", "").Replace("_edu", "").Replace(".bat", "")}_err.txt");

                                    WriteList($"{line_cmd.Replace(exePath, "")} => {output.Replace(exePath, "")} : {errMsg}", Path.Combine(exePath, folderPath, $@"{timestamp}_{System.IO.Path.GetFileName(pID_file[p.Id]).Replace("tbqb.", "").Replace("tcqb.", "").Replace("_tpe", "").Replace("_edu", "").Replace(".bat", "")}_err.txt"));

                                    //retryStr = new StringBuilder();
                                    //using (FileStream fs = new FileStream(Path.Combine(batPath, folderPath, $"{timestamp}_retry.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                    //{
                                    //    using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                                    //    {
                                    //        retryStr.AppendLine($"{line_cmd}");
                                    //        srOutFile.Write(retryStr.ToString());
                                    //    }
                                    //}

                                    //retryStr = new StringBuilder();
                                    //using (FileStream fs = new FileStream(Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                    //{
                                    //    using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                                    //    {
                                    //        retryStr.AppendLine($"{line_cmd}");
                                    //        srOutFile.Write(retryStr.ToString());
                                    //    }
                                    //}

                                    WriteList(line_cmd, Path.Combine(exePath, folderPath, $"{timestamp}_List.txt"));
                                }
                                //else
                                //{
                                //    sb = new StringBuilder();
                                //    //using (FileStream fs = new FileStream(Path.Combine(folderPath, $"{timestamp}_{System.IO.Path.GetFileName(line_cmd)}.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                //    using (FileStream fs = new FileStream(Path.Combine(batPath, folderPath, $"{timestamp}_{System.IO.Path.GetFileName(pID_file[p.Id]).Replace("tbqb.", "").Replace("tcqb.", "").Replace("_tpe", "").Replace("_edu", "").Replace(".bat", "")}.txt"), FileMode.Append, FileAccess.Write, FileShare.None))
                                //    {
                                //        using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                                //        {
                                //            sb.AppendLine($"{line_cmd.Replace(batPath, "")} => {output.Replace(batPath.Remove(batPath.Length - 1), "")} : {errMsg}");
                                //            srOutFile.Write(sb.ToString());
                                //        }
                                //    }
                                //}

                                p.WaitForExit(1000);
                            }));

                            thread.Start();
                            Thread.Sleep(200);
                        }

                        PrintTotalScreen();
                    }
                }
                catch (Exception ex)
                {
                    //string ss = ex.ToString();
                    //string sss = ex.ToString();
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
            }
        }

        private void WriteList(string content, string listPath)
        {
            using (FileStream fs = new FileStream(listPath, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter srOutFile = new StreamWriter(fs, Encoding.Unicode))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(content);
                    srOutFile.Write(sb.ToString());
                    srOutFile.Close();
                    srOutFile.Dispose();
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 導向輸出版本
        /// </summary>
        /*
        private void ReadListExecute(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                MessageBox.Show(fullPath + "=>檔案不存在");
                return;
            }

            DialogResult result = MessageBox.Show("確定執行\n" + fullPath, "請好好確定執行檔案是否正確", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                folderPath = DateTime.Now.ToString("yyyy-MM-dd");
                string line = string.Empty;
                string exePath = System.IO.Directory.GetCurrentDirectory();
                timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                using (StreamReader reader = new StreamReader(fullPath, Encoding.UTF8))
                {
                    int pID = -1;

                    pID_list.Clear();
                    pID_file.Clear();

                    while ((line = reader.ReadLine()) != null)
                    {
                        string errMsg = string.Empty;
                        string batPath = System.IO.Path.GetFullPath(line).Replace(System.IO.Path.GetFileName(line), "");
                        StringBuilder sb = new StringBuilder();

                        if (!File.Exists(line))
                        {
                            MessageBox.Show(line + "=>檔案不存在");
                            return;
                        }
                        Process p = new Process();
                        p.StartInfo.FileName = $"{line} > {Path.Combine(batPath, folderPath, timestamp)}{System.IO.Path.GetFileName(line).Replace("tbqb.", "").Replace("tcqb.", "").Replace("_tpe", "").Replace("_edu", "").Replace(".bat", "")}.txt";

                        p.Start();

                        pID = p.Id;
                        pID_list.Add(pID);
                        pID_file.Add(pID, line);
                    }

                    PrintTotalScreen();
                }
            }
            else
            {
            }
        }
        */
        /// <summary>
        /// 正常輸出版本
        /// </summary>
        /*
    private void ReadListExecute_bak(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            MessageBox.Show(fullPath + "=>檔案不存在");
            return;
        }

        DialogResult result = MessageBox.Show("確定執行\n" + fullPath, "請好好確定執行檔案是否正確", MessageBoxButtons.YesNoCancel);

        if (result == DialogResult.Yes)
        {
            string line = string.Empty;
            string exePath = System.IO.Directory.GetCurrentDirectory();
            string nowTime = DateTime.Now.ToString("yyyyMMdd_HHmm");
            using (StreamReader reader = new StreamReader(fullPath, Encoding.UTF8))
            {
                int pID = -1;

                pID_list.Clear();
                pID_file.Clear();

                while ((line = reader.ReadLine()) != null)
                {
                    string errMsg = string.Empty;
                    string batPath = System.IO.Path.GetFullPath(line).Replace(System.IO.Path.GetFileName(line), "");
                    StringBuilder sb = new StringBuilder();

                    if (!File.Exists(line))
                    {
                        MessageBox.Show(line + "=>檔案不存在");
                        return;
                    }
                    Process p = new Process();
                    p.StartInfo.FileName = line;

                    p.Start();

                    pID = p.Id;
                    pID_list.Add(pID);
                    pID_file.Add(pID, line);
                }
            }
        }
        else
        {
        }
    }
    */

        //private void Btn_MD5_Check_Click(object sender, EventArgs e)
        //{
        //    if (!File.Exists(md5_fileList))
        //    {
        //        MessageBox.Show(md5_fileList + "=>檔案不存在");
        //        return;
        //    }

        //    string line = string.Empty;

        //    using (StreamReader reader = new StreamReader(md5_fileList, Encoding.UTF8))
        //    {
        //        File.Create(".\\MD5_Result.txt").Close();

        //        using (TextWriter tw = new StreamWriter(".\\MD5_Result.txt", true))
        //        {
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                if (!File.Exists(line))
        //                {
        //                    MessageBox.Show(line + "=>檔案不存在");
        //                    return;
        //                }

        //                using (var md5 = MD5.Create())
        //                {
        //                    using (var stream = File.OpenRead(line))
        //                    {
        //                        tw.WriteLine(BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", ""));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    MessageBox.Show("MD5 Checksum完成!!" + "\n" + "產生MD5_Result.text");
        //}
    }
}