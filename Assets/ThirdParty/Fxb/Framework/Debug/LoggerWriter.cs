
namespace Framework
{
    using System;
    using System.IO;

    using UDebug = UnityEngine.Debug;

    public class LoggerWriter : ILogOutput {
        private Action<string> writeFileAction;

        private FileStream fs;

        private StreamWriter sw;

        private readonly static object lockerObj = new object();

        public LoggerWriter()
        {
            
        }

        public void WriteLog(string msg)
        {
            if (writeFileAction == null)
            {
                return;
            }

            writeFileAction.BeginInvoke(msg, null, null);
        }

        public void Release()
        {
            lock (lockerObj)
            {
                if (sw != null)
                {
                    sw.Close();

                    sw.Dispose();

                    sw = null;
                }
                if (fs != null)
                {
                    fs.Close();

                    fs.Dispose();

                    fs = null;
                }

                writeFileAction = null;
            }
        }

        private void WriteFile(string msg)
        {
            lock (lockerObj)
                try
                {
                    sw.WriteLine(msg);

                    sw.Flush();
                }
                catch (Exception ex)
                {
                    Release();

                    UDebug.LogError(ex.Message);
                }
        }

        public void Init(string logPath, string fileName)
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            string filePath = string.Concat(logPath, string.Concat(fileName,"_", DateTime.Today.ToString("yyyyMMdd"),".txt"));

            try
            {
                    string[] fileList = Directory.GetFiles(logPath, "*.txt", SearchOption.AllDirectories);
                    
                    if (null != fileList && 0 != fileList.Length)
                    {
                        foreach (var f in fileList)
                        {
                            if (File.Exists(f) && f.IndexOf(fileName) != -1 && false == filePath.Equals(f))   
                            {
                                File.Delete(f);
                            }
                        }
                    }

                writeFileAction = WriteFile;

                fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

                sw = new StreamWriter(fs);
            }
            catch (Exception ex)
            {
                Release();

                UDebug.LogError(ex.Message);
            }
        }
    }
}

