using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c2mAPI;
namespace testSDKCSharp
{
    class batch_cost
    {
        static public void runBatchCost(String un, String pw, c2mAPI.Environment.Mode mode)
        {
            Console.WriteLine("Starting Batch Test");
            Console.WriteLine(DateTime.Now.ToString());
            Batchc2mAPI r = new Batchc2mAPI(new Authentication(un, pw, mode),"TEST");
            r.BatchId = 3144642;
            //r.costCheck += R_costCheck;
            r.CostCheckComplete += R_costCheckComplete;
            r.loadBatchObjectAllAsync(true);

        }

        private static void R_costCheck(int batchId, int jobId, float cost, float costTax, float costTotal, int remaining, string batchRefId = null, string jobRefId = null)
        {

            Console.WriteLine("JOBID:" + jobId.ToString() + " that is in BatchId: " + batchId.ToString() + " has a total cost of $" + cost.ToString() + " and a totalTax of $" + costTax.ToString() + " for a grand total of $" + (cost + costTax).ToString() + " " + remaining.ToString() + " items remaining to get cost on.");

        }

        private static void R_costCheckComplete(int batchId, float totalCost, float totalTax, float grandTotal, string refId = null)
        {
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine("Completed check: BatchId: " + batchId.ToString() + " has a total cost of $" + totalCost.ToString() + " and a totalTax of $" + totalTax.ToString() + " for a grand total of $" + grandTotal.ToString());
        }



    }
}

