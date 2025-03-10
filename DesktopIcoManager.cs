using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace DesktopICO
{
    /// <summary>
    /// 桌面图标布局管理器
    /// </summary>
    internal class DesktopIcoManager
    {
        #region Windows API 导入

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("kernel32.dll", EntryPoint = "VirtualAllocEx", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int flAllocationType, int flProtect);

        [DllImport("kernel32.dll", EntryPoint = "VirtualFreeEx", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int dwFreeType);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);

        [DllImport("kernel32.dll", EntryPoint = "OpenProcess", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory", SetLastError = true)]
        private static extern bool ReadProcessMemoryPoint(IntPtr hProcess, IntPtr lpBaseAddress, out Point lpBuffer, int nSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        #endregion

        #region 常量定义

        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_VM_OPERATION = 0x0008;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int MEM_COMMIT = 0x1000;
        private const int PAGE_READWRITE = 0x04;
        private const int LVM_GETITEMPOSITION = 0x1010;
        private const int LVM_SETITEMPOSITION = 0x100F;
        private const int LVM_GETITEMCOUNT = 0x1004;
        private const int MAX_PATH = 260;
        private const int LVM_GETITEMTEXT = 0x1073;
        private const int LVIF_TEXT = 0x0001;

        #endregion

        #region 数据结构

        [Serializable]
        private class IconInfo
        {
            public required string Name { get; set; }
            public Point Position { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LVITEM
        {
            public uint mask;
            public int iItem;
            public int iSubItem;
            public uint state;
            public uint stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public uint cColumns;
            public IntPtr puColumns;
        }

        #endregion

        #region 公共方法

        public string SaveLayout()
        {
            var icons = GetDesktopIcons();
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(icons, options);
        }

        /// <summary>
        /// 根据布局数据恢复桌面图标布局
        /// </summary>
        /// <param name="layoutData">布局数据的JSON字符串</param>
        public void RestoreLayout(string layoutData)
        {
            var icons = JsonSerializer.Deserialize<List<IconInfo>>(layoutData);
            SetDesktopIcons(icons);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取桌面所有图标的信息
        /// </summary>
        private List<IconInfo> GetDesktopIcons()
        {
            var icons = new List<IconInfo>();

            try
            {
                // 获取桌面ListView句柄
                IntPtr hDesktop = FindWindow("progman", null);
                if (hDesktop == IntPtr.Zero)
                {
                    throw new Exception("无法找到 Progman 窗口");
                }

                hDesktop = FindWindowEx(hDesktop, IntPtr.Zero, "shelldll_defview", null);
                if (hDesktop == IntPtr.Zero)
                {
                    throw new Exception("无法找到 Shelldll_DefView 窗口");
                }

                hDesktop = FindWindowEx(hDesktop, IntPtr.Zero, "syslistview32", null);
                if (hDesktop == IntPtr.Zero)
                {
                    throw new Exception("无法找到 SysListView32 窗口");
                }

                // 获取explorer.exe进程句柄
                GetWindowThreadProcessId(hDesktop, out int processId);
                IntPtr hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, processId);
                if (hProcess == IntPtr.Zero)
                {
                    throw new Exception($"无法打开进程，错误代码：{Marshal.GetLastWin32Error()}");
                }

                try
                {
                    // 分配内存用于存储位置信息
                    IntPtr pv = VirtualAllocEx(hProcess, IntPtr.Zero, Marshal.SizeOf<Point>(), MEM_COMMIT, PAGE_READWRITE);
                    if (pv == IntPtr.Zero)
                    {
                        throw new Exception($"无法分配内存，错误代码：{Marshal.GetLastWin32Error()}");
                    }

                    try
                    {
                        // 获取图标总数
                        int count = SendMessage(hDesktop, LVM_GETITEMCOUNT, 0, 0);
                        if (count == 0)
                        {
                            return icons;
                        }

                        // 遍历所有图标
                        for (int i = 0; i < count; i++)
                        {
                            try
                            {
                                // 获取图标位置
                                SendMessage(hDesktop, LVM_GETITEMPOSITION, i, pv);
                                ReadProcessMemoryPoint(hProcess, pv, out Point position, Marshal.SizeOf<Point>(), out _);

                                // 获取图标名称
                                string name = GetIconName(hDesktop, hProcess, i);

                                icons.Add(new IconInfo { Name = name, Position = position });
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"处理图标 {i} 时出错：{ex.Message}");
                            }
                        }
                    }
                    finally
                    {
                        VirtualFreeEx(hProcess, pv, Marshal.SizeOf<Point>(), 0);
                    }
                }
                finally
                {
                    CloseHandle(hProcess);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取桌面图标时出错：{ex.Message}");
                throw;
            }

            return icons;
        }

        /// <summary>
        /// 设置桌面图标布局
        /// </summary>
        private void SetDesktopIcons(List<IconInfo> icons)
        {
            if (icons == null || icons.Count == 0)
                return;

            // 获取桌面ListView句柄
            IntPtr hDesktop = FindWindow("progman", null);
            hDesktop = FindWindowEx(hDesktop, IntPtr.Zero, "shelldll_defview", null);
            hDesktop = FindWindowEx(hDesktop, IntPtr.Zero, "syslistview32", null);

            // 获取explorer.exe进程句柄
            GetWindowThreadProcessId(hDesktop, out int processId);
            IntPtr hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, processId);

            try
            {
                // 分配内存用于存储位置信息
                IntPtr pv = VirtualAllocEx(hProcess, IntPtr.Zero, Marshal.SizeOf<Point>(), MEM_COMMIT, PAGE_READWRITE);

                // 获取图标总数
                int count = SendMessage(hDesktop, LVM_GETITEMCOUNT, 0, 0);

                // 遍历所有图标并设置位置
                for (int i = 0; i < count; i++)
                {
                    string name = GetIconName(hDesktop, hProcess, i);
                    var icon = icons.Find(x => x.Name == name);
                    if (icon != null)
                    {
                        SendMessage(hDesktop, LVM_SETITEMPOSITION, i, icon.Position.X | (icon.Position.Y << 16));
                    }
                }

                VirtualFreeEx(hProcess, pv, Marshal.SizeOf<Point>(), 0);
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// 获取指定索引的图标名称
        /// </summary>
        private string GetIconName(IntPtr hDesktop, IntPtr hProcess, int index)
        {
            // 分配内存用于存储LVITEM结构
            IntPtr lvItemPtr = VirtualAllocEx(hProcess, IntPtr.Zero, Marshal.SizeOf<LVITEM>(), MEM_COMMIT, PAGE_READWRITE);
            if (lvItemPtr == IntPtr.Zero)
            {
                throw new Exception($"无法分配LVITEM内存，错误代码：{Marshal.GetLastWin32Error()}");
            }

            IntPtr textBufferPtr = VirtualAllocEx(hProcess, IntPtr.Zero, MAX_PATH * 2, MEM_COMMIT, PAGE_READWRITE);
            if (textBufferPtr == IntPtr.Zero)
            {
                VirtualFreeEx(hProcess, lvItemPtr, Marshal.SizeOf<LVITEM>(), 0);
                throw new Exception($"无法分配文本缓冲区内存，错误代码：{Marshal.GetLastWin32Error()}");
            }

            try
            {
                // 创建并初始化LVITEM结构
                var lvItem = new LVITEM
                {
                    mask = LVIF_TEXT,
                    iItem = index,
                    iSubItem = 0,
                    cchTextMax = MAX_PATH,
                    pszText = textBufferPtr
                };

                // 将LVITEM结构写入目标进程
                IntPtr localLvItemPtr = Marshal.AllocHGlobal(Marshal.SizeOf<LVITEM>());
                try
                {
                    Marshal.StructureToPtr(lvItem, localLvItemPtr, false);
                    if (!WriteProcessMemory(hProcess, lvItemPtr, localLvItemPtr, Marshal.SizeOf<LVITEM>(), out int bytesWritten))
                    {
                        throw new Exception($"写入LVITEM结构失败，错误代码：{Marshal.GetLastWin32Error()}");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(localLvItemPtr);
                }

                // 发送消息获取文本
                SendMessage(hDesktop, LVM_GETITEMTEXT, index, lvItemPtr);

                // 读取文本内容
                byte[] buffer = new byte[MAX_PATH * 2];
                IntPtr localBufferPtr = Marshal.AllocHGlobal(buffer.Length);
                try
                {
                    if (!ReadProcessMemory(hProcess, textBufferPtr, localBufferPtr, buffer.Length, out int bytesRead))
                    {
                        throw new Exception($"读取文本失败，错误代码：{Marshal.GetLastWin32Error()}");
                    }
                    Marshal.Copy(localBufferPtr, buffer, 0, buffer.Length);
                }
                finally
                {
                    Marshal.FreeHGlobal(localBufferPtr);
                }

                return Encoding.Unicode.GetString(buffer).TrimEnd('\0');
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取图标名称时出错：{ex.Message}");
                throw;
            }
            finally
            {
                VirtualFreeEx(hProcess, lvItemPtr, Marshal.SizeOf<LVITEM>(), 0);
                VirtualFreeEx(hProcess, textBufferPtr, MAX_PATH * 2, 0);
            }
        }

        #endregion
    }
}
