using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Plagiarism.Worker.Util
{
    public static class Extensions
    {
        public static double ParseAsDouble(this string str)
        {
            return Double.Parse(str, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}
