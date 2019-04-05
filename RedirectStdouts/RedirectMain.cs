using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            string[] argsForProgram = args;
            if (args.Length > 1) processStartInfo.Arguments = arrayToString(removeFirstArg(args));
            processStartInfo.ErrorDialog = true;
            processStartInfo.FileName = args[0];
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            redirProgram.Start();
            Task rd1 = new Task(read1, ct);
            Task rd2 = new Task(read2, ct);
            rd1.Start();
            rd2.Start();
            redirProgram.WaitForExit();
            exited = true;
            cts.Cancel();
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
            for (int i = 1; i > remove.Length - 1; i++) {
                new_arg[i] = remove[i];
            }
            return new_arg;
        }
        static string arrayToString(string[] arr) {
            string ret = "";
            for (int i = 0; i < arr.Length - 1; i++) {
                ret += arr[i];
            }
            return ret;
        }
    }
}
