using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Plagiarism.Worker.Algorithms
{
    public interface IAlgorithm
    {
        KeyValuePair<double, string> CompareSrc(Source source1, Source source2);
    }
}
