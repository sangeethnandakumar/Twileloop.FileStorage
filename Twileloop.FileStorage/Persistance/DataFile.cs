using System;
using System.Collections.Generic;
using System.Reflection;

namespace Twileloop.FileStorage.Persistance
{
    public class DataFile
    {
        public string Program { get; set; }
        public string EncodedData { get; set; }
        public bool IsEncrypted { get; set; }
        public string EncryptionAlgorithm { get; set; }
        public string EncryptionProvider { get; set; }
        public long DataSize { get; set; }
        public Dictionary<string, string> FileMeta { get; set; }
        public string UsedAssembly { get; }
        public Version UsedVersion { get; }
        public string FileFormat { get; }

        public DataFile()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            AssemblyFileVersionAttribute fileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            UsedAssembly = assemblyName.Name;
            UsedVersion = new Version(fileVersionAttribute.Version);
            FileFormat = "PDR.v1";
        }
    }
}