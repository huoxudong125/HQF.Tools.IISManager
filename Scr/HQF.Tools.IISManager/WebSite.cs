namespace HQF.Tools.IISManager
{
    public class WebSite
    {
        private int _Id;
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _ApplicationPool;
        public string ApplicationPool
        {
            get { return _ApplicationPool; }
            set { _ApplicationPool = value; }
        }

        private string _FolderPath;
        public string FolderPath
        {
            get { return _FolderPath; }
            set { _FolderPath = value; }
        }

        private ServerState _ServerState;
        public ServerState ServerState
        {
            get { return _ServerState; }
            set { _ServerState = value; }
        }
    }
}