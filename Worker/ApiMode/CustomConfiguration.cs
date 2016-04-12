using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Plagiarism.Worker.ApiMode
{
    [XmlType("ApiMode.CustomConfiguration")]
    public class CustomConfiguration
    {
        public string Endpoint;
        // Request timeout in seconds
        public int RequestTimeout;
        public string Token;
    }
}
