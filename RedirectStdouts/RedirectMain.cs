using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RedirectStdouts
{
    class RedirectMain
    {
        public RedirectMain() {
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            redirProgram.Kill();
            redirProgram.Close();
            exited = true;
            Environment.Exit(1);
        }

        static StreamReader streamStdOut = null;
        static StreamReader streamStdErr = null;
        static Process redirProgram = null;
        static bool exited = false;
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            redirProgram = new Process();

            ProcessStartInfo processStartInfo = redirProgram.StartInfo;
            processStartInfo.CreateNoWindow = false;
            processStartInfo.FileName = args[0];
            if (args.Length >= 1) processStartInfo.Arguments = args[1];
            processStartInfo.ErrorDialog = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;

            redirProgram.Start();

            streamStdErr = redirProgram.StandardError;
            streamStdOut = redirProgram.StandardOutput;
            Task rd1 = new Task(read1, ct);
            Task rd2 = new Task(read2, ct);
            rd1.Start();
            rd2.Start();

            redirProgram.WaitForExit();
            exited = true;

            cts.Cancel();
            cts.Dispose();
            rd1.Dispose();
            rd2.Dispose();

            redirProgram.Close();
        }
        static void read1() {
            string line = null;
            while (!streamStdOut.EndOfStream && !exited)
            {
                line = streamStdOut.ReadLine();
                Console.WriteLine(line);
            }
        }
        static void read2()
        {
            string line = null;
            while (!streamStdErr.EndOfStream && !exited)
            {
                line = streamStdErr.ReadLine();
                Console.WriteLine(line);
            }
        }
    }
}
