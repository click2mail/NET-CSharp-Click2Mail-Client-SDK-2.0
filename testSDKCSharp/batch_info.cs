using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c2mAPI;
namespace testSDKCSharp
{
    class batch_info
    {
        static public void runBatchInfo(String un,String pw, c2mAPI.Environment.Mode mode, int batchId)
        {
            Console.WriteLine("Starting Batch Test" );
            Console.WriteLine(DateTime.Now.ToString());
            here:
                Batchc2mAPI r = new Batchc2mAPI(new Authentication(un, pw, mode),null, new System.IO.DirectoryInfo(@"c:\intel\batchRef\"));
            r.BatchId = batchId;
            //r.JobDetailsAvailable += R_JobDetailsAvailable;
            r.loadBatchObjectAll(false);
            if(r.JobList.Count ==0)
            {

                System.Threading.Thread.Sleep(10000);
                goto here;
            }
           // r.ActivateJobCheck();
            
            foreach (Jobs j in r.JobList)
            {
                j.cancelJob();
                //Console.WriteLine("REFID:" + j.RefId);
                //Console.WriteLine("JobID:" + j.JobId);
                //Console.WriteLine("JobID:" + j.MergeFile);
            }

        }

        private static void R_JobDetailsAvailable(Batchc2mAPI b)
        {
            string myRef = b.RefId != null ? b.RefId : "";
            foreach (Jobs j in b.JobList)
            {
                System.Console.WriteLine("THIS BATCH HAS JOBS NOW: " + j.JobId.ToString() + " and this jobs ref was " + j.RefId + " and this batchref is :" + myRef);
            }
        }


    }
}

