using System;
using System.Threading.Tasks;
using Npgsql;

namespace PulseFlow.Simulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine(" 🚀 PulseFlow 현장 기계 시뮬레이터 (Direct DB Insert) ");
            Console.WriteLine("==================================================\n");

            string connectionString = "Host=localhost;Database=PulseFlowDB;Username=postgres;Password=0000";

            string[] machines = { "Machine_A", "Machine_B", "Machine_C" };
            Random random = new Random();

            Console.WriteLine("PostgreSQL로 데이터 전송을 시작합니다...\n");

            while (true)
            {
                try
                {

                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        await conn.OpenAsync();

                        foreach (var machine in machines)
                        {
                            double temp = Math.Round(20.0 + (random.NextDouble() * 60.0), 2);
                            double press = Math.Round(1.0 + (random.NextDouble() * 4.0), 2);
                            DateTime now = DateTime.UtcNow;


                            string sql = @"
                                INSERT INTO ""SensorLogs"" (""MachineName"", ""Temperature"", ""Pressure"", ""LoggedAt"") 
                                VALUES (@m, @t, @p, @l)";

                            using (var cmd = new NpgsqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("m", machine);
                                cmd.Parameters.AddWithValue("t", temp);
                                cmd.Parameters.AddWithValue("p", press);
                                cmd.Parameters.AddWithValue("l", now);


                                await cmd.ExecuteNonQueryAsync();
                            }


                            Console.ForegroundColor = GetColorForMachine(machine);
                            Console.WriteLine($"[{now.ToLocalTime():HH:mm:ss}] {machine} 전송 완료 | 온도: {temp:F2}℃ | 압력: {press:F2}atm");
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[에러 발생] DB 연결 또는 저장 실패: {ex.Message}");
                }

                Console.ResetColor();
                await Task.Delay(1000); 
            }
        }

        static ConsoleColor GetColorForMachine(string machineName)
        {
            return machineName switch
            {
                "Machine_A" => ConsoleColor.Cyan,
                "Machine_B" => ConsoleColor.Green,
                "Machine_C" => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };
        }
    }
}