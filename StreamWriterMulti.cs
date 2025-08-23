using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_screenshot_ai
{
    internal class StreamWriterMulti : TextWriter
    {
        private readonly TextWriter console;
        private readonly TextWriter file;
        public StreamWriterMulti(TextWriter console , TextWriter file)
        {
            this.console = console;
            this.file = file;
            file = new StreamWriter(new FileStream("logs.txt", FileMode.Append, FileAccess.Write, FileShare.Read));
        }

        public override void WriteLine(string value)
        {
            Log(value);
        }

        public override void WriteLine(object value)
        {
            Log(value?.ToString() ?? "null");
        }

        public override void WriteLine(int value) => Log(value.ToString());
        public override void WriteLine(long value) => Log(value.ToString());
        public override void WriteLine(double value) => Log(value.ToString());
        public override void WriteLine(float value) => Log(value.ToString());

        private void Log(string value)
        {
            string formatted = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] | {value}";
            console.WriteLine(value);
            file.WriteLine(formatted);
        }

        public override Encoding Encoding =>Console.OutputEncoding;
    }
}
