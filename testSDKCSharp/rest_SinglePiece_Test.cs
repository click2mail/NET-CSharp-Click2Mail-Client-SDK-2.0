using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c2mAPI;


namespace testSDKCSharp
{
    class rest_SinglePiece
    {
        static public void runSimpleSend(String un, String pw, c2mAPI.Environment.Mode mode)
        {
            Console.WriteLine("Starting REST Single Piece Test");

            Jobs r = new Jobs(new Authentication(un, pw, mode));
            r.StatusChanged += r_statusChanged;
            r.JobStatusCheck += r_jobStatusCheck;
            r.AddressList.Clear(); //Not needed just good habit to clear before you add any addresses

            String addressXML = "<address>"
                                    +"<name>Test Smith</name>"
                                    +"<organization>ABC Inc</organization>"
                                    +"<address1>One Church Street</address1>"
                                    +"<address2>Suite# 401</address2>"
                                    +"<address3></address3>"
                                    +"<city>Rockville</city>"
                                    +"<state>MD</state>"
                                    +"<postalCode>20850</postalCode>"
                                    +"<country>US</country>"
                                +"</address>";

            r.createJobSinglePiece("testSinglePieceTemplate", @"/Users/mlavannis/Documents/PDFs/test_letter.pdf", addressXML);
            Console.ReadLine();
            Console.WriteLine("Completed REST Single Piece TEST");
        }

        static void r_jobStatusCheck(string id, string status, string description)
        {
            Console.WriteLine("jobId is:" + id);
            Console.WriteLine("job Status is:" + status);
            Console.WriteLine("job Description is:" + description);
        }

        static void r_statusChanged(string reason)
        {
                    Console.WriteLine(reason);
        }
    }
}
