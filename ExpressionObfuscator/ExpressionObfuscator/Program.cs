using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ExpressionObfuscator {
    class Program {

        [STAThread]
        static void Main(string[] args) {
            var obfuscator = new Obfuscator();

            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            var code = File.ReadAllText(fd.FileName);

            var obfuscatedCode = obfuscator.Obfuscate(code);

            Console.Write(obfuscatedCode);

            Console.ReadKey();
        }
    }
}
