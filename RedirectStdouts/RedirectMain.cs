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
            Environment.Exit(1);
        }

        static StreamReader streamStdOut = null;
        static StreamReader streamStdErr = null;
        static Process redirProgram = null;
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            redirProgram = new Process();

            ProcessStartInfo processStartInfo = redirProgram.StartInfo;
            processStartInfo.CreateNoWindow = false;
            processStartInfo.FileName = args[0];
            if (args.Length > 1) processStartInfo.Arguments = args[1];
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
            redirProgram.Close();
        }
        static void read1() {
            while (!streamStdOut.EndOfStream)
                Console.WriteLine(streamStdOut.ReadLine());
        }
        static void read2(){
            while (!streamStdErr.EndOfStream)
                Console.WriteLine(streamStdErr.ReadLine());
        }
    }
}
