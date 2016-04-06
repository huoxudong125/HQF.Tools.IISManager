using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQF.Tools.IISManager
{
    public class IisWebApplication
    {
        private string _Name = "";
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _AspNetFrameworkVersion = "";
        public string AspNetFrameworkVersion
        {
            get { return _AspNetFrameworkVersion; }
            set { _AspNetFrameworkVersion = value; }
        }

        private string _Description = "";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _FolderPath;
        public string FolderPath
        {
            get { return _FolderPath; }
            set { _FolderPath = value; }
        }

        public ApplicationPool ApplicationPool { get; set; }
    }
}
