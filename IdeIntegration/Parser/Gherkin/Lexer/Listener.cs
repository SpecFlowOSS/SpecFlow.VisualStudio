using System;
using System.Collections.Generic;

namespace gherkin.lexer
{
    public interface Listener
    {
        void docString(string str1, string str2, int i);

        void feature(string str1, string str2, string str3, int i);

        void background(string str1, string str2, string str3, int i);

        void scenario(string str1, string str2, string str3, int i);

        void scenarioOutline(string str1, string str2, string str3, int i);

        void examples(string str1, string str2, string str3, int i);

        void step(string str1, string str2, int i);

        void comment(string str, int i);

        void tag(string str, int i);

        void row(List<string> l, int i);

        void eof();
    }
}