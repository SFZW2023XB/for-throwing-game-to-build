using System;
using System.Net.Sockets;
using System.Text;
using Google.FlatBuffers;
using showdetails.Models;
using System.Threading;

class Program
{
    // 可以修改这些配置
    private static string MULTICAST_ADDRESS = "239.0.0.1";  // 组播地址
    private static int PORT = 12345;                      // 目标端口
    private static int INTERVAL_MS = 1000;                // 发送间隔（毫秒）

    // 足球运动模拟参数
    private static double ballX = 200;                    // 初始X坐标
    private static double ballY = 0;                      // 初始Y坐标
    private static double ballZ = 0;                      // 初始Z坐标
    private static double ballSpeedX = 10;                // X方向速度
    private static double ballSpeedY = 5;                 // Y方向速度
    private static double ballSpeedZ = 2;                 // Z方向速度
    private static Random random = new Random();          // 随机数生成器

    // 石头模拟参数
    private static double stoneX = 0;                    // 初始X坐标
    private static double stoneY = 0;                    // 初始Y坐标
    private static double stoneZ = 0;                    // 初始Z坐标
    private static int stoneCount = 5;                   // 石头数量
    
    // 状态指示灯参数
    private static int statusValue = 0;                  // 状态值（0-6）

    static void Main(string[] args)
    {
        Console.WriteLine("UDP组播发送程序启动");
        Console.WriteLine($"组播地址: {MULTICAST_ADDRESS}:{PORT}");
        Console.WriteLine($"发送间隔: {INTERVAL_MS}毫秒");
        Console.WriteLine("按Ctrl+C停止发送\n");

        try
        {
            using (var udpClient = new UdpClient())
            {
                // 设置TTL（生存时间）
                udpClient.Ttl = 1;
                
                int messageCount = 0;

                // 在循环外定义变量
                bool isBallInRange = false;
                double targetX = 100;
                double targetY = 50;
                double targetZ = 25;
                bool isTargetInRange = true;
                double minRange = 10.0;
                double maxRange = 100.0;

                while (true)
                {
                    try
                    {
                        // 更新足球位置
                        UpdateBallPosition();

                        // 创建FlatBuffer
                        var builder = new FlatBufferBuilder(1024);

                        var ballPosition = Vec3.CreateVec3(builder, ballX, ballY, ballZ);
                        // 随机更新 isBallInRange
                        isBallInRange = random.NextDouble() > 0.5;
                        var ball = Ball.CreateBall(builder, ballPosition, isBallInRange);

                        var stonePosition = Vec3.CreateVec3(builder, stoneX, stoneY, stoneZ);
                        var stone = Stone.CreateStone(builder, stonePosition);

                        var targetPosition = Vec3.CreateVec3(builder, targetX, targetY, targetZ);

                        // 创建GameState
                        GameState.StartGameState(builder);
                        GameState.AddItemCount(builder, statusValue); // 使用statusValue作为状态指示灯的值
                        GameState.AddTargetPosition(builder, targetPosition);
                        GameState.AddIsTargetInRange(builder, isTargetInRange);
                        GameState.AddMinRange(builder, minRange);
                        GameState.AddMaxRange(builder, maxRange);
                        GameState.AddBall(builder, ball);
                        GameState.AddStone(builder, stone);
                        GameState.AddStoneCount(builder, stoneCount); // 使用stoneCount作为石头数量
                        var gameStateOffset = GameState.EndGameState(builder);

                        builder.Finish(gameStateOffset.Value);

                        byte[] bytes = builder.SizedByteArray();

                        // 发送数据到组播地址
                        udpClient.Send(bytes, bytes.Length, MULTICAST_ADDRESS, PORT);

                        // 打印发送的数据
                        messageCount++;
                        Console.WriteLine($"[{messageCount}] 已发送 GameState 数据，字节长度: {bytes.Length}，球位置: ({ballX:F1}, {ballY:F1}, {ballZ:F1}), 石头位置: ({stoneX:F1}, {stoneY:F1}, {stoneZ:F1}), 石头数量: {stoneCount}, 状态值: {statusValue}");

                        // 更新状态值，循环0-6
                        statusValue = (statusValue + 1) % 7;
                        
                        // 随机更新石头数量（1-10之间）
                        if (random.NextDouble() < 0.3) // 30%的概率改变石头数量
                        {
                            stoneCount = random.Next(1, 11);
                        }
                        
                        // 等待指定时间
                        Thread.Sleep(INTERVAL_MS);
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"\n发送失败 (SocketException): {ex.Message}");
                        Console.WriteLine($"错误代码: {ex.ErrorCode}");
                        Console.WriteLine("请检查接收端是否正在运行，以及端口是否正确\n");
                        Thread.Sleep(1000); // 等待一秒后继续尝试
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n发送失败: {ex.GetType().Name} - {ex.Message}");
                        Console.WriteLine("程序将在3秒后退出\n");
                        Thread.Sleep(3000);
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n程序初始化失败: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }
    }
    
    // 更新足球位置的方法
    private static void UpdateBallPosition()
    {
        // 更新位置
        ballX += ballSpeedX;
        ballY += ballSpeedY;
        ballZ += ballSpeedZ;
        
        // 边界检查和反弹 (X坐标在0-400之间)
        if (ballX < 0 || ballX > 400)
        {
            ballSpeedX = -ballSpeedX; // 反向
            ballX = Math.Clamp(ballX, 0, 400); // 确保在范围内
        }
        
        // Y和Z坐标随机变化，模拟自然运动
        if (random.NextDouble() < 0.1) // 10%的概率改变方向
        {
            ballSpeedY = random.NextDouble() * 10 - 5; // -5到5之间的随机值
            ballSpeedZ = random.NextDouble() * 6 - 3;   // -3到3之间的随机值
        }
        
        // 限制Y和Z的范围
        if (Math.Abs(ballY) > 50) ballSpeedY = -ballSpeedY;
        if (Math.Abs(ballZ) > 30) ballSpeedZ = -ballSpeedZ;
    
        // 更新石头位置
        UpdateStonePosition();
    }
    
    // 更新石头位置的方法
    private static void UpdateStonePosition()
    {
        // 石头位置随机变化
        if (random.NextDouble() < 0.3) // 30%的概率改变位置
        {
            stoneX = random.NextDouble() * 400; // 0-400之间的随机值
            stoneY = random.NextDouble() * 50 - 25; // -25到25之间的随机值
            stoneZ = random.NextDouble() * 40 - 20; // -20到20之间的随机值
        }
    }
}