using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace HW4
{
    class Program
    {
        static Queue<string> queue = new Queue<string>();
        private static bool isRunning = true;
        
        
        static void Main(string[] args)
        {
            Console.WriteLine("1 - First Request");
            Console.WriteLine("2 - Second Request");
            Console.WriteLine();
            while (isRunning)
            {
                RunQueueAsync();
                RunListenAsync();
            }
        }

        static async Task RunQueueAsync()
        {
            await Task.Run(() => RunQueue());
        }

        static async void RunListenAsync()
        {
            await Task.Run(() => ListenForInput());
        }


        static async Task ListenForInput()
        {
            while (isRunning)
            {
                string userInput = Console.ReadLine();
                
                switch (userInput)
                {
                    case "1":
                        Console.WriteLine("Case 1");
                        queue.Enqueue(userInput);
                        break;
                    case "2":
                        Console.WriteLine("Case 2");
                        queue.Enqueue(userInput);
                        break;
                    case "exit":
                        Console.WriteLine("Exit");
                        isRunning = false;
                        await RunQueueAsync();
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            }
        }

        static async Task RunQueue()
        { 
            string connectionString = "Server=localhost,1433;Database=Lab3;User=sa;Password=P@55w0rd;";
            
            while (isRunning)
            {
                Thread.Sleep(2000);
                string res = "";
                if (queue.Count > 0)
                {
                    queue.TryDequeue(out res);
                    Console.WriteLine($"dequeue {res}");
                }

                if (res != "")
                {
                    Boolean canConnection = false;
                    var request = "";
                    if (res == "1")
                    {
                        canConnection = true;
                        request = String.Format($"select * from ProductSeller where ProductId = 1234");
                    }

                    if (res == "2")
                    {
                        canConnection = true;
                        request = String.Format($"select * from Product where endPrice < 1000 and endPrice > 2033");
                    }

                    if (canConnection)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(request, connection))
                            {
                                connection.Open();
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                                    }
                                }
                            }
                            connection.Close();
                        }
                    }
                }
            }
        }
    }
}