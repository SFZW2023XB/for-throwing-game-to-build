using System;
using System.Net.Sockets;
using System.Net;
using Google.FlatBuffers;
using showdetails.Models;
using ReactiveUI;
using System.Reactive;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using Avalonia.Threading;
using System.Collections.ObjectModel;

namespace showdetails.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ObservableCollection<StatusLight> _statusLights;
    private UdpClient? udpClient;
    private const string MULTICAST_ADDRESS = "239.0.0.1"; // 组播地址
    private const int PORT = 12345;
    private showdetails.Models.GameState _gameState; // 添加 GameState 字段

    public ICommand TestStatus { get; }

    public showdetails.Models.GameState GameState // 添加 GameState 属性
    {
        get => _gameState;
        private set => this.RaiseAndSetIfChanged(ref _gameState, value);
    }


    public MainViewModel()
    {
        // 初始化一个默认的游戏状态，让“点迹”在启动时可见
        var builder = new FlatBufferBuilder(1);
        var position = Vec3.CreateVec3(builder, 50, 0, 0); // 默认位置 X=50
        var ball = Ball.CreateBall(builder, position);

        GameState.StartGameState(builder);
        GameState.AddBall(builder, ball);
        var gameStateOffset = GameState.EndGameState(builder);
        builder.Finish(gameStateOffset.Value);

        var buffer = builder.SizedByteArray();
        _gameState = GameState.GetRootAsGameState(new ByteBuffer(buffer));
        _statusLights = new ObservableCollection<StatusLight>
        {
            new StatusLight { Text = "正常", Description = "系统运行正常" },
            new StatusLight { Text = "异常", Description = "系统出现异常" },
            new StatusLight { Text = "警告", Description = "系统警告" },
            new StatusLight { Text = "运行", Description = "系统正在运行" },
            new StatusLight { Text = "停止", Description = "系统已停止" },
            new StatusLight { Text = "错误", Description = "系统错误" }
        };

        Debug.WriteLine("MainViewModel 已初始化");
        StartUdpListener();
        TestStatus = ReactiveCommand.Create<string>(status =>
        {
            Debug.WriteLine($"测试按钮被点击，状态值：{status}");
            if (int.TryParse(status, out int value))
            {
                UpdateStatus(value);
            }
        });
    }

    private async void StartUdpListener()
    {
        while (true)
        {
            try
            {
                if (udpClient != null)
                {
                    udpClient.Dispose();
                    udpClient = null;
                }

                udpClient = new UdpClient();
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

                // 加入组播组
                udpClient.JoinMulticastGroup(IPAddress.Parse(MULTICAST_ADDRESS));
                Debug.WriteLine($"UDP监听器已启动，端口：{PORT}，组播地址：{MULTICAST_ADDRESS}");
                
                while (true)
                {
                    var result = await udpClient.ReceiveAsync();
                    var buffer = result.Buffer;
                    var byteBuffer = new ByteBuffer(buffer);
                    //var veryfy = GameState.VerifyGameState(new Google.FlatBuffers.ByteBuffer(data));
                    //if (!veryfy)
                    //{
                    //    return;
                    //}
                    var gameState = showdetails.Models.GameState.GetRootAsGameState(byteBuffer);

                    Dispatcher.UIThread.Post(() =>
                    {
                        GameState = gameState;
                        // 从接收到的GameState中获取状态值并更新状态指示灯
                        UpdateStatus(gameState.ItemCount);
                    });
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"UDP监听错误: {ex.Message}");
                if (udpClient != null)
                {
                    try
                    {
                        udpClient.DropMulticastGroup(IPAddress.Parse(MULTICAST_ADDRESS));
                    }
                    catch { }
                }
                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"未预期的错误: {ex.Message}");
                await Task.Delay(5000);
            }
        }
    }

    private void UpdateStatus(int status)
    {
        Debug.WriteLine($"开始更新状态：{status}");
        if (status < 0 || status > 6)
        {
            Debug.WriteLine($"状态值 {status} 超出范围（0-6），忽略此次更新");
            return;
        }
        
        Dispatcher.UIThread.Post(() =>
        {
            for (int i = 0; i < _statusLights.Count; i++)
            {
                _statusLights[i].IsActive = i < status;
            }
            Debug.WriteLine($"状态更新完成，触发属性变更通知");
            this.RaisePropertyChanged(nameof(StatusLights));
        });
    }

    public ObservableCollection<StatusLight> StatusLights
    {
        get
        {
            Debug.WriteLine($"获取 StatusLights 属性");
            return _statusLights;
        }
    }
}
