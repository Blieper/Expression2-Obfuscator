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

            Console.WindowWidth = 180;
            Console.WindowHeight = 60;

            var obfuscator = new Obfuscator();
            string code = string.Empty;

            Console.WriteLine("Expression 2 Obfuscator --- Written by KrypteK");
            Console.WriteLine("");

            using (var fileDialog = new OpenFileDialog()) {
                fileDialog.ShowDialog();

                if (!string.IsNullOrEmpty(fileDialog.FileName)) {
                    code = File.ReadAllText(fileDialog.FileName);
                } else {
                    return;
                }
            }

            var obfuscatedCode = obfuscator.Obfuscate(code);

            Console.Write(obfuscatedCode);
            Console.WriteLine("\n\nInsert file name, press enter to select folder to save in.");
            Console.Write(">");

            string fileName = Console.ReadLine();

            if (fileName.EndsWith(".txt")) {
                fileName = fileName.Substring(0, fileName.LastIndexOf(".txt"));
            }

            SaveFileDialog sf = new SaveFileDialog {
                FileName = fileName
            };

            if (sf.ShowDialog() == DialogResult.OK) {
                string savePath = Path.GetDirectoryName(sf.FileName) + "\\" + fileName + ".txt";

                File.WriteAllText(savePath, obfuscatedCode);

                Console.WriteLine("\nSuccesfully saved to: \n" + savePath);
            }

            Console.WriteLine("Press any key to quit.");
            Console.Read();
        }
    }
}
