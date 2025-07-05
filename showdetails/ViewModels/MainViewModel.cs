using System;
using System.Net.Sockets;
using System.Net;
using FlatBuffers;
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
    private GameState _gameState; // 添加 GameState 字段

    public ICommand TestStatus { get; }

    public GameState GameState // 添加 GameState 属性
    {
        get => _gameState;
        private set => this.RaiseAndSetIfChanged(ref _gameState, value);
    }

    public MainViewModel()
    {
        _gameState = new GameState // 初始化 GameState
        {
            ItemCount = 0,
            MinRange = 50,
            MaxRange = 150,
            TargetPositionX = 200,
            TargetPositionY = 0,
            TargetPositionZ = 0,
            Ball = new Ball
            {
                PositionX = 200,
                PositionY = 0,
                PositionZ = 0
            },
            Stone = new Stone
            {
                PositionX = 0,
                PositionY = 0,
                PositionZ = 0
            },
            StoneCount = 5
        };

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
                    var byteBuffer = new ByteBuffer(result.Buffer);
                    var gameState = GameState.GetRootAsGameState(byteBuffer);

                    GameState.ItemCount = gameState.ItemCount;
                    GameState.TargetPositionX = gameState.TargetPosition.Value.X;
                    GameState.TargetPositionY = gameState.TargetPosition.Value.Y;
                    GameState.TargetPositionZ = gameState.TargetPosition.Value.Z;
                    GameState.MinRange = gameState.MinRange;
                    GameState.MaxRange = gameState.MaxRange;

                    var ball = new Ball
                    {
                        PositionX = gameState.Ball.Value.Position.Value.X,
                        PositionY = gameState.Ball.Value.Position.Value.Y,
                        PositionZ = gameState.Ball.Value.Position.Value.Z,
                        IsInRange = gameState.Ball.Value.IsInRange
                    };
                    GameState.Ball = ball;

                    var stone = new Stone
                    {
                        PositionX = gameState.Stone.Value.Position.Value.X,
                        PositionY = gameState.Stone.Value.Position.Value.Y,
                        PositionZ = gameState.Stone.Value.Position.Value.Z
                    };
                    GameState.Stone = stone;

                    GameState.StoneCount = gameState.StoneCount;
                    }
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
