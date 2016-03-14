using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Plagiarism.Worker.Util;
using System.Xml.Serialization;

namespace Plagiarism.Worker.Algorithms
{
    [Serializable]
    public class DllAlgorithm : BaseAlgorithm
    {
        [XmlIgnore]
        private LibraryLoader Loader;

        public string DllName;

        public DllAlgorithm() : base(-1, "", false)
        {
            DllName = "";
        }

        public DllAlgorithm(int id, string name, bool enabled, string dllName) : base(id, name, enabled)
        {
            DllName = dllName;
            Loader = new LibraryLoader(dllName);
        }

        public override KeyValuePair<double, string> CompareSrc(string source1, string source2)
        {
            var res = Loader.Call(source1, source2);
            int idx = res.IndexOf(' ');
            
            return new KeyValuePair<double, string>(res.Substring(0, idx).ParseAsDouble(), res.Substring(idx + 1).Trim());
        }
    }
}
