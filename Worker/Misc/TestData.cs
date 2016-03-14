using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiarism.Worker.Misc
{
    class TestData
    {
        public static readonly string Source1 = @"
                #include <stdio.h>

                int main() {
                    long long a, b;
                    freopen(""input.txt"", ""r"", stdin);
                    freopen(""output.txt"", ""w"", stdout);

                    scanf(""%I64d %I64d"", &a, &b);
                    printf(""%I64d\n"", a + b);

                    return 0;
                }
            ";
        public static readonly string Source2 = @"
                #include <stdio.h>

                int main() {
                    int cmd = 0;
                    long long c, d;
                    freopen(""aaa.txt"", ""w"", stdout);
                    freopen(""aaa.txt"", ""r"", stdin);                    

                    printf(""%I64d\n"", c + d);
                    scanf(""%I64d %I64d"", &c, &d);
                    
                    return 0;
                }
            ";
    }
}
