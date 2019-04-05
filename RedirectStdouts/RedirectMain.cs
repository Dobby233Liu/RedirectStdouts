using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RedirectStdouts
{
    class RedirectMain
    {
        static StreamReader streamStdOut = null;
        static StreamReader streamStdErr = null;
        static bool exited = false;
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            Process redirProgram = new Process();
            ProcessStartInfo processStartInfo = redirProgram.StartInfo;
            processStartInfo.CreateNoWindow = false;
            processStartInfo.FileName = args[0];
            if (args.Length > 1) processStartInfo.Arguments = arrayToString(removeFirstArg(args));
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
            redirProgram.Dispose();
        }
        static void read1() {
            string line = null;
            while (!streamStdOut.EndOfStream && !exited)
            {
                line += streamStdOut.ReadLine() + Environment.NewLine;
                Console.WriteLine(line);
            }
        }
        static void read2()
        {
            string line = null;
            while (!streamStdErr.EndOfStream && !exited)
            {
                line += streamStdErr.ReadLine() + Environment.NewLine;
                Console.WriteLine(line);
            }
        }
        static string[] removeFirstArg(string[] remove) {
            string[] new_arg = new string[] { };
            for (int i = 0; i > remove.Length - 1; i++) {
                if (i == 0) continue;
                new_arg[i] = remove[i];
            }
            return new_arg;
        }
        static string arrayToString(string[] arr) {
            string ret = "";
            for (int i = 0; i < arr.Length; i++) {
                ret += arr[i];
            }
            return ret;
        }
    }
}
