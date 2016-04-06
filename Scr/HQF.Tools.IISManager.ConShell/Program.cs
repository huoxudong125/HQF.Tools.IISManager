using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQF.Tools.IISManager.ConShell
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager=new Manager();
            var sites=manager.GetWebSites();

            foreach (var webSite in sites)
            {
                Console.WriteLine("{2}=====Website [{0}],state[{1}]",webSite.Name,webSite.ServerState,Environment.NewLine);

                var applicationPools = manager.GetWebApplications(webSite.Id,true);
                foreach (var applicationPool in applicationPools)
                {
                    Console.WriteLine("======Application pool [{0}],[{1}]",applicationPool.Name,applicationPool.AspNetFrameworkVersion);
                }
            }

          


            Console.Read();

        }
    }
}
