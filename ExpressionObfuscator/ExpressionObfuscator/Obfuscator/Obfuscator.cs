using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ExpressionObfuscator {
    class Obfuscator {
        private readonly Dictionary<string, string> _variableNames = new Dictionary<string, string>();
        private readonly List<string> _io = new List<string>();
        private readonly Random _random = new Random();
        private readonly string[] _operators = new string[] {
            ",",
            "*",
            "+",
            "-",
            "/",
            "%",
            "^",
            "<",
            ">",
            "?",
            ":",
            "(",
            ")",
            "&",
            "|",
            "=",
            "{",
            "}"
        };

        private void GetIO(string code) {
            var dirRegex = new Regex(@"@(.*)\n?");
            var nameRegex = new Regex(@"(?<!\w)([A-Z]([A-Za-z0-9_])*)(?!\w)");

            foreach (var m in dirRegex.Matches(code)) {
                var directive = m.ToString();

                if (directive.StartsWith("@inputs") || directive.StartsWith("@outputs")) {
                    foreach (var name in nameRegex.Matches(directive)) {
                        _io.Add(name.ToString());
                    }
                }
            }
        }

        private string DoDirectives(string code) {
            var directives = "";
            var dirRegex = new Regex(@"@(.*)\n?");

            foreach (var m in dirRegex.Matches(code)) {
                var directive = m.ToString();

                if (directive.StartsWith("@persist")) {
                    foreach (var name in _variableNames) {
                        if (directive.Contains(name.Value)) {
                            directive = Regex.Replace(directive, @"(?<!\w)" + name.Value + @"(?!\w)", name.Key);
                        }
                    }
                }

                directives += directive;
            }

            return directives;
        }

        private string GenerateNameFor(string name) {
            if (_io.Contains(name)) return _io.Find(x => x == name);

            const string startChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string chars = "abcdefghijklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";

            string generated = "" + startChars[_random.Next(startChars.Length)];

            generated += new string(Enumerable.Repeat(chars, _random.Next(32))
              .Select(s => s[_random.Next(s.Length)]).ToArray());

            if (_variableNames.ContainsKey(generated)) {
                return GenerateNameFor(name);
            }
            else {
                _variableNames[generated] = name;
                return generated;
            }
        }

        private string DoVarNames(string code) {
            var newCode = code;
            var regex = new Regex(@"[A-Z]([A-Za-z0-9_])*\s*=");
            var matches = regex.Matches(code);
            var nameList = new List<string>();

            foreach (var m in matches) {
                var name = Regex.Replace(m.ToString(), @"\s*=", "");

                if (!nameList.Contains(name)) {
                    Console.WriteLine(name);

                    nameList.Add(name);
                }
            }

            Console.WriteLine("\n\n");

            foreach (var name in nameList) {
                var newName = GenerateNameFor(name);

                newCode = Regex.Replace(newCode, @"(?<!["".*])(?<!\w)" + name + @"(?!\w)(?![.*""])", newName);
            }

            return newCode;
        }

        private string RemoveComments(string code) {
            var newCode = code;
            newCode = Regex.Replace(newCode, @"\#\[([^\[\]].|\n)*\]\#", "");
            newCode = Regex.Replace(newCode, @"#(.*)\n?", "");

            return newCode;
        }

        private string DoNumbers(string code) {
            var newCode = code;
            var regex = new Regex(@"(?<![\w.])\d+(?![.])(?!\w)");
            var matches = regex.Matches(code);
            var numList = new List<string>();

            foreach (var m in matches) {
                var num = m.ToString();

                if (numList.Find(x => x == num) == null) {
                    numList.Add(num);
                }
            }

            foreach (var num in numList) {
                newCode = new Regex(@"(?<![\w.])" + num + @"(?![.])(?!\w)").Replace(newCode, "0x" + int.Parse(num).ToString("X"));
            }

            return newCode;
        }

        private string RemoveWhiteSpace(string code) {
            var newCode = code;

            newCode = Regex.Replace(newCode, @"(?<!["".*]) +(?![.*""])", " ");

            foreach (var op in _operators) {
                string space = (op == ")" || op == "}") ? " " : "";

                newCode = Regex.Replace(newCode, @"(?<!["".*])(\s*\" + op + @"\s+)(?![.*""])", op + space);
            }

            newCode = Regex.Replace(newCode, @"(?<!["".*])(\t|(\n|\r|\r\n))*(?![.*""])", "");

            return newCode;
        }



        public string Obfuscate(string code) {
            var directoryString = string.Empty;
            var dirRegex = new Regex(@"@(.*)\n?");
            foreach (var m in dirRegex.Matches(code)) directoryString += m.ToString();

            string restCode = code.Substring(directoryString.Length);

            GetIO(code);

            restCode = DoNumbers(restCode);
            restCode = DoVarNames(restCode);
            restCode = RemoveComments(restCode);
            restCode = RemoveWhiteSpace(restCode);

            Console.WriteLine(string.Join(",", _variableNames));

            directoryString = DoDirectives(code);

            return directoryString + restCode;
        }
    }
}
