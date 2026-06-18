using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var machineStates = new ConcurrentDictionary<string, bool>();
machineStates.TryAdd("Machine_A", true);
machineStates.TryAdd("Machine_B", true);
machineStates.TryAdd("Machine_C", true);


app.MapPost("/api/control/{machineName}/{command}", (string machineName, string command) =>
{
    if (!machineStates.ContainsKey(machineName))
        return Results.NotFound("기계를 찾을 수 없습니다.");

    if (command.ToLower() == "stop")
    {
        machineStates[machineName] = false;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[명령 수신] {machineName} 긴급 정지!\n");
    }
    else if (command.ToLower() == "start")
    {
        machineStates[machineName] = true;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"\n[명령 수신] {machineName} 재가동!\n");
    }

    Console.ResetColor();
    return Results.Ok();
});


_ = Task.Run(async () =>
{
    string connString = "Host=localhost;Database=PulseFlowDB;Username=postgres;Password=0000";
    Random random = new Random();

    while (true)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync();

            foreach (var machine in machineStates.Keys)
            {
                if (machineStates[machine])
                {
                    double temp = Math.Round(20.0 + (random.NextDouble() * 60.0), 2);
                    double press = Math.Round(1.0 + (random.NextDouble() * 4.0), 2);

                    string sql = @"INSERT INTO ""SensorLogs"" (""MachineName"", ""Temperature"", ""Pressure"", ""LoggedAt"") VALUES (@m, @t, @p, @l)";
                    using var cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("m", machine);
                    cmd.Parameters.AddWithValue("t", temp);
                    cmd.Parameters.AddWithValue("p", press);
                    cmd.Parameters.AddWithValue("l", DateTime.UtcNow);
                    await cmd.ExecuteNonQueryAsync();

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {machine} 정상 가동 중...");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DB 통신 에러: {ex.Message}");
        }
        await Task.Delay(1000);
    }
});




app.Lifetime.ApplicationStarted.Register(() =>
{
    var server = app.Services.GetService(typeof(IServer)) as IServer;
    var addresses = server?.Features.Get<IServerAddressesFeature>()?.Addresses;

    Console.Clear(); // 화면 한번 정리
    Console.WriteLine("==================================================");
    if (addresses != null)
    {
        foreach (var address in addresses)
        {

            Console.WriteLine($"  PulseFlow 시뮬레이터 & API 서버 작동 중");
            Console.WriteLine($" 실제 수신 포트: {address}");
        }
    }
    Console.WriteLine("==================================================\n");
});


app.Run("http://localhost:5000");