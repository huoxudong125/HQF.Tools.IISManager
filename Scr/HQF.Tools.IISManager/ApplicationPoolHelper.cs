using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQF.Tools.IISManager
{
    class ApplicationPoolHelper
    {
        private const string MetabasePath = "IIS://localhost/W3SVC/AppPools";

        /// <summary>
        /// Returns a list of all the Application Pools configured
        /// </summary>
        /// <returns></returns>
        public static List<ApplicationPool> GetApplicationPools()
        {
            DirectoryEntry root = GetDirectoryEntry(MetabasePath);
            if (root == null)
                return null;

            List<ApplicationPool> pools = new List<ApplicationPool>();

            foreach (DirectoryEntry entry in root.Children)
            {
                pools.Add(new ApplicationPool(entry));
            }
            return pools;
        }

        /// <summary>
        /// Create a new Application Pool and return an instance of the entry
        /// </summary>
        /// <param name="appPoolName"></param>
        /// <returns></returns>
        public static ApplicationPool CreateApplicationPool(string appPoolName)
        {
            DirectoryEntry root = GetDirectoryEntry(MetabasePath);
            if (root == null)
                return null;

            var appPool = root.Invoke("Create", "IIsApplicationPool", appPoolName) as DirectoryEntry;
            appPool.CommitChanges();
            return new ApplicationPool(appPool);
        }


        /// <summary>
        /// Returns an instance of an Application Pool
        /// </summary>
        /// <param name="appPoolName"></param>
        /// <returns></returns>
        public static ApplicationPool GetApplicationPool(string appPoolName)
        {
            DirectoryEntry root = GetDirectoryEntry(MetabasePath + "/" + appPoolName);
            return new ApplicationPool(root);
        }

        /// <summary>
        /// Retrieves an Adsi Node by its path. Abstracted for error handling
        /// </summary>
        /// <param name="path">the ADSI path to retrieve: IIS://localhost/w3svc/root</param>
        /// <returns>node or null</returns>
        private static DirectoryEntry GetDirectoryEntry(string path)
        {
            DirectoryEntry root;
            try
            {
                root = new DirectoryEntry(path);
            }
            catch
            {
                return null;
            }
            return root;
        }
    }
}
