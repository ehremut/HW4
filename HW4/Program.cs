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
        static CancellationTokenSource cancel = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.WriteLine("1 - First Request");
            Console.WriteLine("2 - Second Request");
            Console.WriteLine();
            try
            {
                Task.Run(() => RunQueue(), cancel.Token);
                Task.Run(() => ListenForInput(), cancel.Token);
                cancel.Token.WaitHandle.WaitOne();
                cancel.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation Canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ListenForInput()
        {
            while (true)
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
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            }
        }

        static void RunQueue()
        { 
            string connectionString = "Server=localhost,1433;Database=Lab3;User=sa;Password=P@55w0rd;";
            
            while (true)
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