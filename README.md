# TheBelowPart - 投掷游戏模拟器

一个使用C#和Avalonia UI开发的投掷游戏模拟器，通过UDP组播实现实时数据传输，使用FlatBuffers进行高效的数据序列化。

## 项目概述

本项目模拟了一个简单的投掷游戏系统，包含以下主要组件：

- **showdetails**: 使用Avalonia UI框架开发的可视化客户端，用于显示游戏状态和对象位置
- **UdpSender**: 模拟游戏服务器，生成并发送游戏状态数据

项目使用UDP组播技术实现数据传输，确保多个客户端可以同时接收游戏状态更新。通过FlatBuffers实现高效的数据序列化和反序列化，适合实时游戏场景。

## 项目结构

```
├── showdetails/            # 客户端应用
│   ├── Models/            # 数据模型
│   │   ├── GameState.fbs  # FlatBuffers模式定义
│   │   └── StatusLight.cs # 状态指示灯模型
│   ├── ViewModels/        # MVVM视图模型
│   ├── Views/             # UI视图
│   └── Converters/        # 数据转换器
├── showdetails.Desktop/   # 桌面应用入口
└── UdpSender/             # 数据发送模拟器
```

## 技术栈

- **C# (.NET)**: 主要开发语言
- **Avalonia UI**: 跨平台UI框架
- **FlatBuffers**: 高效的跨平台序列化库
- **UDP组播**: 网络通信协议
- **MVVM架构**: 应用程序设计模式

## 功能特点

- 实时显示游戏对象（球和石头）的位置和状态
- 可视化距离标尺和打击范围
- 状态指示灯系统，显示不同的系统状态
- 模拟物体运动和碰撞检测
- 高效的数据传输和处理

## 使用方法

### 环境要求

- .NET 8.0 或更高版本
- 支持Avalonia UI的操作系统（Windows、macOS、Linux）

### 运行步骤

1. 克隆仓库到本地
   ```
   git clone https://github.com/yourusername/thebelowpart.git
   cd thebelowpart
   ```

2. 构建项目
   ```
   dotnet build
   ```

3. 先启动客户端应用
   ```
   cd showdetails.Desktop
   dotnet run
   ```

4. 在另一个终端中启动数据发送器
   ```
   cd UdpSender
   dotnet run
   ```

5. 观察客户端界面上的实时数据更新

## 已知问题

### FlatBuffers版本兼容性问题

项目中使用的FlatBuffers版本可能存在兼容性问题。showdetails项目使用的是Google.FlatBuffers 22.9.24版本，而UdpSender项目使用的是24.3.25版本。这可能导致以下问题：

- 自动生成的代码中引用了不存在的`Google.FlatBuffers.Verifier`类
- 调用了不存在的`FlatBufferConstants.FLATBUFFERS_25_2_10()`方法

**解决方案**：

1. 统一FlatBuffers版本：确保所有项目使用相同版本的FlatBuffers库
2. 修改生成的代码：注释掉不兼容的引用和方法调用，将验证方法改为直接返回`true`

## 贡献

欢迎贡献代码、报告问题或提出改进建议。请遵循以下步骤：

1. Fork本仓库
2. 创建您的特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交您的更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 开启一个Pull Request

## 许可证

本项目采用MIT许可证 - 详情请参见[LICENSE](LICENSE)文件
