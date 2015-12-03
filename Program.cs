using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRouge {
    public class Program {
        public static void Main(string[] args) {
            Action<char> writer = Console.Write;
            Action<string> logger = WriteToLog;

            DungeonGenerator generator = new DungeonGenerator(80, 50, 90, writer, logger);
            if (generator.GenerateDungeon(80,50,100)) {
                generator.PrintDungeon();
            }

            Console.ReadKey();
        }

        private static void WriteToLog(string msg) {
            Debug.WriteLine(msg);
        }
    }
}
