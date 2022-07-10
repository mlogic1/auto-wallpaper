using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger
{
	internal class FileLogger : ILogger, IDisposable
	{
		private string logFileName;
		private FileStream logFileStream;
		private StreamWriter logStreamWriter;
		private const string LOG_MESSAGE_FORMAT = "[{0}][{1}] {2}";    // time, severity, message
		private readonly Dictionary<LogSeverity, string> SEVERITY_NAMES = new Dictionary<LogSeverity, string>
		{
			{ LogSeverity.Info, "Info" },
			{ LogSeverity.Warning, "Warning" },
			{ LogSeverity.Error, "Error" },
			{ LogSeverity.Debug, "Debug" }
		};

		enum LogSeverity
		{
			Info,
			Warning,
			Error,
			Debug
		};

		internal FileLogger(string file)
		{
			logFileName = file;
			try
			{
				logFileStream = new FileStream(logFileName, FileMode.OpenOrCreate | FileMode.Append);
				logStreamWriter = new StreamWriter(logFileStream);
				logStreamWriter.AutoFlush = true;

				if (logFileStream == null)
				{
					throw new Exception("Unable to open log file: " + logFileName);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public void Debug(string message)
		{
			WriteToFile(message, LogSeverity.Debug);
		}

		public void Dispose()
		{
			logStreamWriter.Close();
			logFileStream.Close();
		}

		public void Error(string message)
		{
			WriteToFile(message, LogSeverity.Error);
		}

		public void Info(string message)
		{
			WriteToFile(message, LogSeverity.Info);
		}

		public void Warn(string message)
		{
			WriteToFile(message, LogSeverity.Warning);
		}
		private void WriteToFile(string message, LogSeverity severity)
		{
			message = String.Format(LOG_MESSAGE_FORMAT, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), SEVERITY_NAMES[severity], message);
			logStreamWriter.WriteLine(message);
		}
	}
}
