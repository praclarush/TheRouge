using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRouge {
    public class Program {
        public static void Main(string[] args) {



            GenerateDungeon();
        }

        private static void GenerateDungeon() {
            int windowX = 90;
            int windowY = 90;

            Console.SetWindowSize(windowX, windowY);

            Action<char> writer = Console.Write;
            Action<string> logger = WriteToLog;

            DungeonGenerator generator = new DungeonGenerator(windowX, windowY, 75, writer, logger);
            if (generator.GenerateDungeon(windowX, windowY, 100)) {
                generator.PrintDungeon();
            }

            Console.ReadKey();
        }

        private static void WriteToLog(string msg) {
            Debug.WriteLine(msg);
        }
    }
}