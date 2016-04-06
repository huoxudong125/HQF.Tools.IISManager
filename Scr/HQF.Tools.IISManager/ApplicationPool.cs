using System;
using System.DirectoryServices;

namespace HQF.Tools.IISManager
{
    public class ApplicationPool
    {
        private DirectoryEntry _directoryEntry;

        public ApplicationPool(DirectoryEntry directoryEntry)
        {
            _directoryEntry = directoryEntry;
            Name = directoryEntry.Name;
            IdentityUserName = (string)directoryEntry.InvokeGet("WAMUserName");
            IdentityPassWord = (string)directoryEntry.InvokeGet("WAMUserPass");
        }

        public string Name { get; set; }
        public string IdentityUserName { get; set; }
        public string IdentityPassWord { get; set; }

        /// <summary>
        /// Change the identity for the given app pool
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="passWord">PassWord</param>
        public void ChangeIdentity(string userName, string passWord)
        {
            _directoryEntry.InvokeSet("WAMUserName", new Object[] { userName });
            _directoryEntry.InvokeSet("WAMUserPass", new Object[] { passWord });

            /*Commit changes*/
            _directoryEntry.CommitChanges();
        }
    }
}