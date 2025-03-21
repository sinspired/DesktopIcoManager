﻿# DesktopICOManager 桌面图标布局管理器

桌面图标布局管理器是一个用于备份和恢复Windows桌面图标位置的工具。它可以帮助用户在重装系统或其他情况下快速恢复桌面图标布局，避免图标位置混乱。

## 功能特性

- 保存当前桌面图标布局
- 恢复已保存的桌面图标布局
- 重命名已保存的布局
- 删除已保存的布局
- 设置开机自启

## 安装

1. 克隆或下载此项目的代码。
2. 使用Visual Studio 2022打开解决方案文件。
3. 生成解决方案。

## 使用方法

1. 启动应用程序。
2. 点击“保存桌面布局”按钮保存当前桌面图标位置。
3. 在布局列表中选择一个布局，然后点击“恢复选中布局”按钮恢复桌面图标布局。
4. 可以通过“重命名布局”按钮重命名已保存的布局。
5. 可以通过“删除”按钮删除不需要的布局。
6. 点击“开机启动”按钮设置或取消应用程序的开机自启。

## 项目结构

- `DesktopIcoManager.cs`：包含桌面图标布局管理的核心逻辑，包括保存和恢复图标布局。
- `MainForm.cs`：主窗体代码，处理用户交互和界面更新。
- `MainForm.Designer.cs`：主窗体的设计器代码，定义了界面元素。
- `MainForm.resx`：主窗体的资源文件，包含界面资源。

## 依赖项

- .NET 8
- Windows API（通过P/Invoke调用）

## 贡献

欢迎提交问题和贡献代码。请确保在提交之前阅读并遵循项目的贡献指南。

## 许可证

此项目使用MIT许可证。有关详细信息，请参阅LICENSE文件。
