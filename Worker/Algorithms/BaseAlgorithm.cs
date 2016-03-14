using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Plagiarism.Worker.Algorithms
{
    public abstract class BaseAlgorithm : IAlgorithm
    {
        public BaseAlgorithm(int id, string name, bool enabled)
        {
            Id = id;
            Name = name;
            Enabled = enabled;
        }
        public int Id;
        public string Name;
        public bool Enabled;

        public abstract KeyValuePair<double, string> CompareSrc(string source1, string source2);
    }
}
