using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using gherkin;

namespace TechTalk.SpecFlow.Parser
{
    public class GherkinDialectServices
    {
        private readonly CultureInfo defaultLanguage;

        public CultureInfo DefaultLanguage
        {
            get { return defaultLanguage; }
        }

        public GherkinDialectServices(CultureInfo defaultLanguage)
        {
            this.defaultLanguage = defaultLanguage;
        }

        static private readonly Regex languageRe = new Regex(@"^\s*#\s*language:\s*(?<lang>[\w-]+)\s*\n");
        static private readonly Regex languageLineRe = new Regex(@"^\s*#\s*language:\s*(?<lang>[\w-]+)\s*$");
        internal string GetLanguageNameFromFileContent(string fileContent)
        {
            string langName = defaultLanguage.Name;
            var langMatch = languageRe.Match(fileContent);
            if (langMatch.Success)
                langName = langMatch.Groups["lang"].Value;

            return langName;
        }

        public GherkinDialectAdapter GetDefaultDialect()
        {
            return GetGherkinDialect(defaultLanguage.Name);
        }

        public GherkinDialectAdapter GetGherkinDialect(string langName)
        {
            return new GherkinDialectAdapter(langName);
        }

        public GherkinDialectAdapter GetGherkinDialect(Feature feature)
        {
            string langName =  feature.Language ?? defaultLanguage.Name;
            return GetGherkinDialect(langName);
        }

        public GherkinDialectAdapter GetGherkinDialectFromFileContent(string fileContent)
        {
            var langName = GetLanguageNameFromFileContent(fileContent);
            return GetGherkinDialect(langName);
        }

        public GherkinDialectAdapter GetGherkinDialect(Func<int, string> lineProvider)
        {
            var langName = GetLanguageName(lineProvider);
            return GetGherkinDialect(langName);
        }

        internal string GetLanguageName(Func<int, string> lineProvider)
        {
            string langName = defaultLanguage.Name;
            int lineNo = 0;
            string line;
            while ((line = lineProvider(lineNo++)) != null)
            {
                var langMatch = languageLineRe.Match(line);
                if (langMatch.Success)
                {
                    langName = langMatch.Groups["lang"].Value;
                    break;
                }

                if (line.Trim().Length != 0)
                    break;
            }

            return langName;
        }

        static public bool IsLanguageLine(string line)
        {
            return languageLineRe.Match(line).Success;
        }
    }
}