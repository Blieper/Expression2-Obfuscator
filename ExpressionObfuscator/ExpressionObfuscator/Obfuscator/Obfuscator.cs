using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ExpressionObfuscator {
    class Obfuscator {
        Dictionary<string, string> VariableNames = new Dictionary<string, string>();
        Random random = new Random();

        private string GenerateNameFor (string name) {
            const string startChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string chars = "abcdefghijklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";

            string generated = "" + startChars[random.Next(startChars.Length)];

            generated += new string(Enumerable.Repeat(chars, random.Next(10))
              .Select(s => s[random.Next(s.Length)]).ToArray());

            if (VariableNames.ContainsKey(generated)) {
                return GenerateNameFor(name);
            } else {
                VariableNames[generated] = name;
                return generated;
            }
        }

        private string DoVarNames (string code) {
            var newCode = code;
            var regex = new Regex(@"[A-Z]([A-Za-z0-9_])*\s*=");
            var matches = regex.Matches(code);
            var nameList = new List<string>();

            foreach (var m in matches) {
                var name = Regex.Replace(m.ToString(), @"\s*=","");

                if (nameList.Find(x => x == name) == null) {
                    nameList.Add(name);
                }
            }

            foreach (var name in nameList) {
                var n = GenerateNameFor(name);
                newCode = Regex.Replace(newCode, @"(?<!\w)" + name + @"(?!\w)", GenerateNameFor(name));
            }

            return newCode;
        }

        private string RemoveComments (string code) {
            var newCode = code;
            newCode = Regex.Replace(newCode, @"#\[(.|\n)*\]#", "");
            newCode = Regex.Replace(newCode, @"#(.*)\n?", "");

            return newCode;
        }

        private string DoNumbers (string code) {
            var newCode = code;
            var regex = new Regex(@"(?<![\w.-])\d+(?![.])(?!\w)");
            var matches = regex.Matches(code);
            var numList = new List<string>();

            foreach (var m in matches) {
                var num = m.ToString();

                if (numList.Find(x => x == num) == null) {
                    numList.Add(num);
                }         
            }

            foreach (var num in numList) {
                newCode = new Regex(@"(?<![\w.-])" + num + @"(?![.])(?!\w)").Replace(newCode, "0x" + int.Parse(num).ToString("X"));
            }

            return newCode;
        }

        public string Obfuscate(string code) {
            var namedir = "";
            var dirRegex = new Regex(@"@name(.*)\n?");
            var dirmatches = dirRegex.Matches(code);

            foreach (var m in dirmatches) {
                namedir += m.ToString();
            }

            string restCode = code.Substring(namedir.Length);

            restCode = DoNumbers(restCode);
            restCode = DoVarNames(restCode);
            restCode = RemoveComments(restCode);

            restCode = Regex.Replace(restCode, @"\s*\}", "}");
            restCode = Regex.Replace(restCode, @"\s*\)", ")");

            restCode = Regex.Replace(restCode, @"\{\s*", "{");
            restCode = Regex.Replace(restCode, @"\(\s*", "(");

            var directives = "";
            dirRegex = new Regex(@"@(.*)\n?");
            dirmatches = dirRegex.Matches(restCode);

            foreach (var m in dirmatches) {
                directives += m.ToString();
            }

            string withoutDirs = restCode.Substring(directives.Length);

            withoutDirs = Regex.Replace(withoutDirs, @"\s+", " ");
            withoutDirs = Regex.Replace(withoutDirs, @"\{", "{\n");
            withoutDirs = Regex.Replace(withoutDirs, @"\(", "{\n");

            withoutDirs = withoutDirs.Trim();

            return namedir + (directives + withoutDirs);
        }
    }
}
