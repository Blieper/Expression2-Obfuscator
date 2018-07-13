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
            var regex = new Regex(@"[A-Z]([A-Za-z0-9_])*\s=");
            var matches = regex.Matches(code);
            var nameList = new List<string>();
            var newCode = code;

            foreach (var m in matches) {
                var name = m.ToString();
                name = Regex.Replace(name,@"\s=","");

                if (nameList.Find(x => x == name) == null) {
                    nameList.Add(name);
                }
            }

            foreach (var name in nameList) {
                var n = GenerateNameFor(name);
                newCode = newCode.Replace(name,GenerateNameFor(name));
            }

            return newCode;
        }

        public string Obfuscate(string code) {
            code = DoVarNames(code);

            return code;
        }
    }
}
