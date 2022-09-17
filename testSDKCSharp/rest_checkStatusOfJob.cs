using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c2mAPI;
namespace testSDKCSharp
{
    class rest_checkStatusOfJob
    {
        static void runCheckStatus()
        {
            Jobs r = new Jobs(new Authentication("username", "password", c2mAPI.Environment.Mode.StageMode));
            r.JobId = 1234;
            r.JobStatusCheck += r_jobStatusCheck;
            Console.Write(r.checkJobStatus());
            Console.ReadLine();
        }

        static void r_jobStatusCheck(string id, string status, string description)
        {
              
            Console.WriteLine("jobId is:" + id);
            Console.WriteLine("job Status is:" + status);
           Console.WriteLine("job Description is:" + description);
        
        }
    }

}
