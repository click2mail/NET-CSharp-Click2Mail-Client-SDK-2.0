using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testSDKCSharp
{
    internal class startApp
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Please select your test");
            Console.WriteLine("1. USE STAGE");
            Console.WriteLine("2. USE LIVE**WARNING**");
            int optionMode = Convert.ToInt32(Console.ReadLine());
            c2mAPI.Environment.Mode m = c2mAPI.Environment.Mode.StageMode;
            if (optionMode == 2)
            {
                 m = c2mAPI.Environment.Mode.LiveMode;
            }
            Console.WriteLine("Enter username ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter Password (This is only for testing, and this is not secure as keystrokes will be visible in console)");
            string password = Console.ReadLine();
            Console.WriteLine("1. Batch (Creating multiple jobs to send at 1x)");
            Console.WriteLine("2. Rest");
            int option1 = Convert.ToInt32(Console.ReadLine());
            if (option1 == 1)
            {

                Console.WriteLine("Please select your test");
                Console.WriteLine("1. Run Batch Merge");
                Console.WriteLine("2. Get Batch Info");
                Console.WriteLine("3. Get Batch Cost");
                Console.WriteLine("4. Run Batch Simple, splitting single file to different addresses");
                int option = Convert.ToInt32(Console.ReadLine());
                if (option == 1)
                {
                    batch_merge.runBatchSimpleSendMultipleFiles(username,password, m);
                }
                else if (option == 2)
                {
                    Console.WriteLine("Enter BatchId:");
                    int batchNo = Convert.ToInt32(Console.ReadLine());
                    batch_info.runBatchInfo(username,password, m,batchNo);
                }
                else if (option == 3)
                {

                    batch_cost.runBatchCost(username, password, m);
                }
                else if (option == 4)
                {

                    batch_simpleSend.runBatchSimpleSend(username, password, m);
                }
                
                
            }
            else if(option1 ==2)
            {

                rest_SimpleSend.runSimpleSend(username, password, m);
                //Console.WriteLine("Please select your test");
                //Console.WriteLine("1. Create a simple Job(RAW ADDRESS XML)");
                //Console.WriteLine("2. Get Batch Info");
                //Console.WriteLine("3. Get Batch Cost");
                // Console.WriteLine("4. Run Batch Simple, splitting single file to different addresses");
            }

            
        }
    }
}
