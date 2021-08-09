using System.Collections.Generic;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Wox.Plugin.MSToDo
{
    public static class Config
    {
        // App settings
        public static readonly string[] Scopes = {"user.read", "Tasks.ReadWrite"};
        public const string ClientId = "6ff23c6e-84f7-4356-bfee-4d6dddf6d755"; 
        public static readonly string MSGraphURL = "https://graph.microsoft.com/v1.0/";
        private const string Tenant = "common";
        private const string Authority = "https://login.microsoftonline.com/" + Tenant;

        // Cache settings
        public const string CacheFileName = "myapp_msal_cache.txt";
        public readonly static string CacheDir = MsalCacheHelper.UserRootDirectory;
        public const string KeyChainServiceName = "myapp_msal_service";
        public const string KeyChainAccountName = "myapp_msal_account";
        public const string LinuxKeyRingSchema = "com.contoso.devtools.tokencache";
        public const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
        public const string LinuxKeyRingLabel = "MSAL token cache for all Contoso dev tool apps.";
        public static readonly KeyValuePair<string, string> LinuxKeyRingAttr1 = new KeyValuePair<string, string>("Version", "1");
        public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new KeyValuePair<string, string>("ProductGroup", "MyApps");
        
    }
}