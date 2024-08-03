using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using System.Runtime.InteropServices;
using System.Reflection;

namespace ThingsLibrary.Metrics
{
    /// <summary>
    /// Machine Metrics
    /// </summary>
    public static class MachineMetrics
    {
        /// <summary>
        /// Name of the machine
        /// </summary>
        /// <returns></returns>
        public static string MachineName() => Environment.MachineName;

        /// <summary>
        /// Number of CPUs
        /// </summary>
        /// <returns></returns>
        public static short CpuCount() => Convert.ToInt16(Environment.ProcessorCount);

        #region -- OS ---

        /// <summary>
        /// Operating System Version
        /// </summary>
        /// <returns></returns>
        public static string OsVersion() => Environment.OSVersion.ToString();
                
        /// <summary>
        /// Mac operating system
        /// </summary>
        /// <returns></returns>
        public static bool IsMac() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        
        /// <summary>
        /// Linux operating system
        /// </summary>
        /// <returns></returns>        
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        /// Windows operating system
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Unix based operating system
        /// </summary>
        /// <returns></returns>
        public static bool IsUnix() => (IsLinux() || IsMac() || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD));

        #endregion

        #region --- Networking ---

        /// <summary>
        /// Is Network interface available
        /// </summary>
        /// <returns></returns>
        public static bool IsNetworkAvailable() => NetworkInterface.GetIsNetworkAvailable();

        /// <summary>
        /// All IP Addresses
        /// </summary>
        /// <returns></returns>
        public static List<string> LocalIPAddresses()
        {
            var list = new List<string>();

            if (!IsNetworkAvailable()) { return list; }

            var addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (var ip in addresses)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {                    
                    list.Add(ip.ToString());
                }
            }

            return list;
        }

        /// <summary>
        /// First IP Address
        /// </summary>
        /// <returns></returns>
        public static string LocalIPAddress() => LocalIPAddresses()?.FirstOrDefault() ?? string.Empty;
        
        public static List<string> MacAddresses()
        {
            var list = new List<string>();

            if (!IsNetworkAvailable()) { return list; }

            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    list.Add(nic.GetPhysicalAddress().ToString());
                }
            }

            return list;
        }

        /// <summary>
        /// First MAC Address
        /// </summary>
        /// <returns></returns>
        public static string MacAddress() => MacAddresses().FirstOrDefault() ?? string.Empty;

        /// <summary>
        /// Checks google.com/generate_204 to see if it is reachable
        /// </summary>
        /// <returns></returns>
        public static bool IsInternetAvailable()
        {
            if (!IsNetworkAvailable()) { return false; }

            try
            {
                var client = new HttpClient();

                var response = client.Send(new HttpRequestMessage(HttpMethod.Get, "https://google.com/generate_204"));

                return (response.StatusCode == HttpStatusCode.NoContent);   //aka: 204 No Content
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region --- Memory ---
        
        /// <summary>
        /// Amount of process memory used
        /// </summary>
        public static double ProcessUsedMemoryKB => ((double)Environment.WorkingSet / 1000d);   //bytes to KB

        #endregion

        #region --- Hard Drive ---

        public static long? GetAvailableHardDriveSpaceKB(string? folderPath = null)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                folderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
            }
            else if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException($"Folder path does not exist: '{folderPath}'");
            }

            var rootPath = Path.GetPathRoot(folderPath);

            try
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && string.Compare(drive.Name, rootPath, true) == 0)
                    {
                        return drive.TotalFreeSpace / 1000; // bytes to KB
                    }
                }

                // should never get here
                return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: GetAvailableHardDriveSpaceKB():");
                Console.WriteLine(ex.ToString());

                return null;
            }            
        }

        #endregion

        #region --- WIFI DETAILS ---

        /// <summary>
        /// Get the Wifi details for the machine the code is running on.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Dictionary<string, string> GetWifiDetails()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetWifiDetailsWindows();
            }
            else
            {
                throw new NotImplementedException("Only Windows operating system is supported at this time.");
            }
        }

        /// <summary>
        /// Get the wifi details based on windows 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Throws exception if not Windows operating system</exception>
        public static Dictionary<string, string> GetWifiDetailsWindows()
        {
            #region --- Examples ---

            // ================================================================================
            // EXAMPLE OUTPUT 1 (Disconnected):
            // ================================================================================

            //            
            //There is 1 interface on the system:
            //
            //    Name                   : Wi-Fi
            //    Description            : Intel(R) Wi-Fi 6E AX211 160MHz
            //    GUID                   : 4eb0f11f-9647-45cc-8565-8112cb6250c0
            //    Physical address       : 68:7a:64:7a:58:aa
            //    State                  : disconnected
            //    Radio status           : Hardware On
            //                             Software On
            //
            //    Hosted network status  : Not available
            //

            // ================================================================================
            // EXAMPLE OUTPUT 2 (Connected)
            // ================================================================================

            //            
            //There is 1 interface on the system:
            //
            //    Name                   : Wi-Fi
            //    Description            : Intel(R) Wi-Fi 6E AX211 160MHz
            //    GUID                   : 4eb0f11f-9647-45cc-8565-8112cb6250c0
            //    Physical address       : 68:7a:64:7a:58:aa
            //    State                  : connected
            //    SSID                   : Test-Guest
            //    BSSID                  : ee:55:88:75:16:cb
            //    Network type           : Infrastructure
            //    Radio type             : 802.11ax
            //    Authentication         : Open
            //    Cipher                 : None
            //    Connection mode        : Profile
            //    Channel                : 173
            //    Receive rate (Mbps)    : 413
            //    Transmit rate (Mbps)   : 413
            //    Signal                 : 78%
            //    Profile                : Test-Guest
            //
            //    Hosted network status  : Not available
            //

            // ================================================================================
            // EXAMPLE OUTPUT 3 (NO WIFI)
            // ================================================================================

            //The Wireless AutoConfig Service(wlansvc) is not running.

            #endregion

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotImplementedException("Only Windows operating system is supported at this time.");
            }

            var details = new Dictionary<string, string>();

            try
            {
                using var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "netsh.exe",
                        Arguments = "wlan show interfaces",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                // parse the output
                var output = process.StandardOutput.ReadToEnd();
                var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines.Where(x => x.Contains(":")))
                {
                    var pair = line.Split(":", 2, StringSplitOptions.TrimEntries);
                    if (string.IsNullOrWhiteSpace(pair[0]) || string.IsNullOrWhiteSpace(pair[1])) { continue; }

                    details[pair[0].Trim()] = pair[1].Trim();
                }
            }
            catch
            {
                //nothing we can do if it fails.  Might not even be on wifi or on a dev box.. return empty collection (don't return a partially populated or otherwise
                return [];
            }

            return details;

        }

        #endregion
    }
}