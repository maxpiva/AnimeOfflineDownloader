using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ADBaseLibrary.Helpers
{
    public class ShellParser
    {
        public delegate void LineEvent(string line);

        public delegate void FinishEvent();

        public event LineEvent OnLine;
        public event LineEvent OnError;
        public event FinishEvent OnFinish;

        private Process process;


        public void DoLine(string line)
        {
            if (OnLine != null)
                OnLine(line);
        }

        public void DoError(string line)
        {
            if (OnError != null)
                OnError(line);
        }
        public void DoFinish()
        {
            if (OnFinish != null)
                OnFinish();
        }

        private bool _finished = false;
        public async Task Start(string path, string arguments, CancellationToken token)
        {
            await Task.Run(() =>
            {
                _finished = false;
                process = new Process();
                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.EnableRaisingEvents = true;
                process.ErrorDataReceived += (a, e) =>
                {
                    if ((e == null) || (e.Data == null))
                        return;
                    string str = e.Data;
                    if (str.Length > 0)
                        DoError(str);
                };
                process.OutputDataReceived += (a, e) =>
                {
                    if ((e == null) || (e.Data == null))
                        return;
                    string str = e.Data; //.Replace("\r\n", string.Empty);
                    if (str.Length > 0)
                        DoLine(str);

                };
                process.Exited += (a, b) =>
                {
                    _finished = true;
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                bool ret = false;
                do
                {
                    Thread.Sleep(100);
                    if (token.IsCancellationRequested)
                    {
                        process.Kill();
                        token.ThrowIfCancellationRequested();
                    }
                } while (!_finished);
                DoFinish();
            },token);
        }

    }
}
