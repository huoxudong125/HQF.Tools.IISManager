using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace HQF.Tools.IISManager
{
    public class Manager
    {
        private List<ApplicationPool> _ApplicationPools;

        public List<IisWebApplication> GetWebApplications(int siteId, bool showSubApplications)
        {
            _ApplicationPools = ApplicationPoolHelper.GetApplicationPools();

            var webApps = new List<IisWebApplication>();
            var MetabaseRootPath = @"IIS://localhost/W3SVC/" + siteId + "/ROOT";
            DirectoryEntry appEntry = null;
            try
            {
                appEntry = new DirectoryEntry(MetabaseRootPath);
                foreach (DirectoryEntry s in appEntry.Children)
                {
                    CheckAndAddWebApplication(s, webApps, showSubApplications);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (appEntry != null)
                    appEntry.Close();
            }
            return webApps;
        }

        public List<WebSite> GetWebSites()
        {
            var metabasePath = "IIS://localhost/W3SVC";
            DirectoryEntry root = null;
            var webSites = new List<WebSite>();
            try
            {
                root = new DirectoryEntry(metabasePath);
                var hasAppPools = HasApplicationPools();
                foreach (DirectoryEntry s in root.Children)
                {
                    int siteId;
                    if (s.SchemaClassName == "IIsWebServer" && int.TryParse(s.Name, out siteId))
                    {
                        var webSite = new WebSite();
                        webSite.Id = siteId;
                        webSite.Name = s.Properties["ServerComment"].Value.ToString();
                        webSite.Description = s.Properties["ServerComment"].Value.ToString();
                        webSite.FolderPath = GetFolderPath(s);
                        webSite.ServerState = GetServerState(s.Properties["ServerState"].Value.ToString());
                        if (hasAppPools)
                        {
                            webSite.ApplicationPool = s.Properties["AppPoolId"].Value.ToString();
                        }
                        webSites.Add(webSite);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (root != null)
                    root.Close();
            }
            return webSites;
        }

        public bool HasApplicationPools()
        {
            var metabaseAppPoolsPath = "IIS://localhost/W3SVC/AppPools";
            var appPoolsEntry = new DirectoryEntry(metabaseAppPoolsPath);
            return IsValidMetabasePath(appPoolsEntry);
        }

        public bool IsValidMetabasePath(DirectoryEntry entry)
        {
            try
            {
                if (entry != null && !string.IsNullOrEmpty(entry.SchemaClassName))
                    return true;
            }
            catch
            {
            }
            return false;
        }

        private void CheckAndAddWebApplication(DirectoryEntry entry, List<IisWebApplication> webApps,
            bool addSubApplications)
        {
            var reserved = true;
            if (IsWebApplicationEntry(entry))
            {
                reserved = IsReservedWebApplication(entry.Name);
                if (!reserved)
                    webApps.Add(GetWebApplication(entry));
            }
            if (entry.Children != null && addSubApplications && !reserved)
            {
                foreach (DirectoryEntry s in entry.Children)
                {
                    CheckAndAddWebApplication(s, webApps, addSubApplications);
                }
            }
        }

        private string GetApplicationFolderPath(DirectoryEntry entry)
        {
            var keyType = entry.Properties["KeyType"].Value.ToString();
            if (keyType == "IIsWebDirectory")
            {
                var path = entry.Path;
                var matchString = "/ROOT/";
                var index = path.IndexOf(matchString);
                return "/" + path.Substring(index + matchString.Length);
            }
            if (keyType == "IIsWebVirtualDir")
            {
                return entry.Properties["Path"].Value.ToString();
            }
            return "";
        }

        private string GetApplicationName(DirectoryEntry entry)
        {
            var appRoot = entry.Properties["AppRoot"].Value.ToString();
            var matchString = "/ROOT/";
            var index = appRoot.ToUpper().IndexOf(matchString);
            if (index > -1)
                return appRoot.Substring(index + matchString.Length);

            return entry.Name;
        }

        private string GetAspNetVersion(DirectoryEntry entry)
        {
            var appPool = entry.Properties["AppPoolId"].Value.ToString();
            var metabaseAppPoolPath = "IIS://localhost/W3SVC/AppPools/" + appPool;
            var appPoolEntry = new DirectoryEntry(metabaseAppPoolPath);
            return appPoolEntry.Properties["ManagedRuntimeVersion"].Value.ToString();
        }
        private string GetFolderPath(DirectoryEntry server)
        {
            foreach (DirectoryEntry s in server.Children)
                if (s.Name.ToUpper() == "ROOT")
                    return s.Properties["Path"].Value.ToString();
            return "";
        }

        private ServerState GetServerState(string serverStatePropertyValue)
        {
            switch (serverStatePropertyValue)
            {
                case "1":
                    return ServerState.Starting;
                case "2":
                    return ServerState.Started;
                case "3":
                    return ServerState.Stopping;
                case "4":
                    return ServerState.Stopped;
                case "5":
                    return ServerState.Pausing;
                case "6":
                    return ServerState.Paused;
                case "7":
                    return ServerState.Continuing;
            }
            return ServerState.Unknown;
        }

        private IisWebApplication GetWebApplication(DirectoryEntry entry)
        {
            var webApp = new IisWebApplication();
            webApp.Name = GetApplicationName(entry);
            webApp.AspNetFrameworkVersion = GetAspNetVersion(entry);
            webApp.FolderPath = GetApplicationFolderPath(entry);
            if (_ApplicationPools != null)
            {
                var appPoolName = entry.Properties["AppPoolId"].Value.ToString();
                webApp.ApplicationPool = ApplicationPoolHelper.GetApplicationPool(appPoolName);
            }
            return webApp;
        }

        private bool IsReservedWebApplication(string appName)
        {
            if (appName.StartsWith("_vti_") || appName == "_private" || appName == "bin" || appName == "Printers"
                || appName == "aspnet_client")
                return true;
            return false;
        }

        private bool IsWebApplicationEntry(DirectoryEntry entry)
        {
            var keyType = entry.Properties["KeyType"].Value.ToString();
            if (keyType == "IIsWebDirectory" ||
                (keyType == "IIsWebVirtualDir" && entry.Properties["Path"].Value != null))
            {
                var appNames = GetApplicationName(entry).Split("/".ToCharArray());
                if (appNames[appNames.Length - 1] == entry.Name)
                    return true;
            }
            return false;
        }
    }
}