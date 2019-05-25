using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace LionFire.Services
{
    public class RunFile
    {

        #region (Public) Properties

        /// <summary>
        /// Discovery is typically used by remote hosts to query for running servers/services.
        /// Typically, in the case of Valor, clients will connect to a central server and know their
        /// services that way.
        /// 
        /// Examples:
        ///  + Valor.Server
        ///  + Valor.Server
        ///  - Valor.Client
        ///  - Valor.IM.Client
        ///  + Valor.VoiceServer
        ///  + Valor.VoiceServer
        /// </summary>
        public string DiscoveryClass 
        {
            get { return discoveryClass; }
            set
            {
                if (value.Contains(RunFile.Separator)) throw new ArgumentException("Class name cannot contain " + RunFile.Separator);

                discoveryClass = value;
            }
        } private string discoveryClass = null;

        /// <summary>
        /// Examples: Valor.Server`123`1400.run
        /// </summary>
        public string DiscoveryId { get; set; }

        public int ProcessId { get; set; }

        public int Port { get; set; }

        public static char Separator = '`';
        public static string Extension = "run";
        public static string DotExtension { get { return "." + Extension; } }
        
        public string FileName
        {
            get
            {
                return (DiscoveryClass ?? "") + Separator + DiscoveryId + Separator + ProcessId + Separator + Port + "." + Extension;
            }
        }

        public string FilePath
        {
            get
            {
                return Path.Combine(RunFileManager.RunFileDirectory, FileName);
            }
        }

        #endregion


        #region Construction

        public RunFile()
        {
            this.ProcessId = Process.GetCurrentProcess().Id;
        }

        public static RunFile FromPath(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            string[] fields = fileName.Split(Separator);

            if (fields == null || fields.Length != 4) return null;

            string discoveryClass = fields[0];
            string discoveryId = fields[1];
            string processIdString = fields[2];
            string port = fields[3];
            int processId;
            try
            {
                processId = Convert.ToInt32(processIdString);
            }
            catch { return null; }

            return new RunFile
            {
                DiscoveryClass = discoveryClass,
                DiscoveryId = discoveryId,
                ProcessId = processId,
                Port = Convert.ToInt32(port),
            };
        }

        #endregion

        /// <summary>
        /// Examples:
        /// 
        /// LionServer.ServerID.PID.run
        ///  - TheatreServer1
        ///    - Port
        ///  - TheatreServer1
        ///    - Port
        /// LionServer.ServerID.PID.run
        ///  - 
        /// LionChat.ClientID.PID.run
        ///  - User account
        /// ValorClient.ClientID.PID.run
        ///  - User account
        ///  - Theatre
        /// ValorClient.ClientID.PID.run
        ///  - User account
        ///  
        /// </summary>

        #region (Public) Methods
        
        public void DeleteIfInvalid()
        {
            //Process process = null;
            try
            {
                if (!Process.GetProcesses().Where(p => p.Id == this.ProcessId).Any())
                {
                    File.Delete(this.FilePath);
                }
            }
            catch { } // SILENTFAIL
            //if (process == null)
            //{
            //    File.Delete(this.FilePath);
            //}
        }

        public bool SaveContents = false;

        public bool IsPersisted
        {
            get { return File.Exists(FilePath); }
            set
            {
                if(value == IsPersisted) return;
                if(value) Save(); else Delete();
            }
        }

        public void Save()
        {
            if(SaveContents) throw new NotImplementedException();

            if (ProcessId == default(int)) throw new ArgumentOutOfRangeException();
            if (DiscoveryClass == default(string)) throw new ArgumentOutOfRangeException();


            if (!File.Exists(FilePath))
            {
                using (File.Create(FilePath)) { }
            }
        }

        public void Delete()
        {
            File.Delete(this.FilePath);
        }

        #endregion

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            RunFile other = obj as RunFile;
            if (other == null) return false;

            return other.FileName == this.FileName;
        }

        public override int GetHashCode()
        {
            return this.FileName.GetHashCode();
        }

        #endregion

    }
}
