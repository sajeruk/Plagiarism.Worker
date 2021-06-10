using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Plagiarism.Worker
{
    // public is set for XML serializer

    [Serializable]
    public sealed class Configuration
    {
        public RunningMode Mode;
        public AggregatorType AggType;
        public bool Debug;

        public string DllDirectory;

        // mode-specific configs
        public SelfTestingMode.CustomConfiguration SelfTestingModeConfig;
        public ApiMode.CustomConfiguration ApiModeConfiguration;

        [XmlIgnore]
        public const string DefaultConfigFileName = "config.xml";

        [XmlArray("Algorithms")]
        [XmlArrayItem("DllAlgorithm", typeof(Algorithms.DllAlgorithm))]
        [XmlArrayItem("HttpAlgorithm", typeof(Algorithms.HttpAlgorithm))]
        public Algorithms.BaseAlgorithm[] Algorithms;
        
        public static void Serialize(string workingDirectory, Configuration conf)
        {
            var path = Path.Combine(workingDirectory, DefaultConfigFileName);
            XmlSerializer xs = new XmlSerializer(conf.GetType());
            
            StreamWriter writer = File.CreateText(path);
            xs.Serialize(writer, conf);

            writer.Flush();
            writer.Close();            
        }

        public static Configuration DefaultConfig()
        {
            Configuration config = new Configuration();
            config.Mode = RunningMode.SelfTesting;
            config.AggType = AggregatorType.Max;
            config.Debug = true;
            config.DllDirectory = "lib";
            config.ApiModeConfiguration = new ApiMode.CustomConfiguration();
            config.ApiModeConfiguration.Endpoint = "localhost:8000";
            config.ApiModeConfiguration.RequestTimeout = 30;
            config.Algorithms = new Algorithms.BaseAlgorithm[] {
                new Algorithms.DllAlgorithm(1, "algo2", true, "AntiChitDLL2.dll"),
                new Algorithms.DllAlgorithm(2, "algo3", true, "AntiChitDLL3.dll"),
                new Algorithms.HttpAlgorithm(3, "newalgo", true, "127.0.0.1:7861")
            };
            return config;
        }

        public static Configuration Deserialize(string workingDirectory)
        {
            var path = Path.Combine(workingDirectory, DefaultConfigFileName);
            XmlSerializer configSerializer = new XmlSerializer(typeof(Configuration));
            using (var input = new StreamReader(path))
            {
                Configuration config = (Configuration)configSerializer.Deserialize(input);
                config.Validate(workingDirectory);
                return config;
            }
        }

        private void Validate(string workingDirectory)
        {
            NormalizePath(workingDirectory, ref DllDirectory);
        }

        private static void NormalizePath(string home, ref string path)
        {
            path = Path.IsPathRooted(path) ? path : Path.Combine(home, path);
            path = Path.GetFullPath(path);
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(path);
            }
        }
    }
}
