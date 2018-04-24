namespace LogTest
{
    using System;
    using System.Collections.Generic;
	using System.Collections.Concurrent;
    using System.IO;
    using System.Text;
    using System.Threading;
	/// <summary>
	/// This class is an asynchronous logger. If the logger is closed with a flush, then it will write all registered lines to the log.
	/// If the logger is used by one thread, then it will register all calls to write.
	/// If the logger is used by multiple threads, then it does not guarantee all writes to be written. However, the request to write will not block or wait.
	/// </summary>
    public class AsyncLog : ILog
    {
		private String _directory = @"./LogTest/";

        private Thread _runThread;
        private ConcurrentQueue<LogLine> _lines = new ConcurrentQueue<LogLine>();

		private IDateProvider _dateTime;

        private StreamWriter _writer; 

        private bool _exit = false;
		private bool _quitWithFlush = false;

		private DateTime _tomorrowMidnightDate;

		private bool _logRunning = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="LogTest.AsyncLog"/> class.
		/// </summary>
		public AsyncLog() : this(new DateProvider())
        {

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="LogTest.AsyncLog"/> class. Should primarily be used for testing. 
		/// Use the constructor with no arguments for ready to use logger.
		/// </summary>
		/// <param name="dt">The DateTime provider.</param>
		public AsyncLog (IDateProvider dt) {
			_dateTime = dt;
			if (!Directory.Exists (_directory)) {
				Directory.CreateDirectory (_directory);
			}
				
			CreateNewLog (_dateTime.Now);

			_runThread = new Thread(MainLoop);
			_runThread.Start();

			while (!_logRunning) {
			}
		}
			
		/// <summary>
		/// Returns a DateTime object that represents tomorrow just after midnight.
		/// </summary>
		/// <returns>The start of tomorrow.</returns>
		private DateTime GetStartOfTomorrow() {
			DateTime now = _dateTime.Now;
			return (new DateTime (now.Year,now.Month,now.Day)).AddDays(1);
		}

		/// <summary>
		/// Writes the string <paramref name="line"/> to the log file. If the writer does not exist, then the method just returns.
		/// </summary>
		/// <param name="line">The line to be written to the log.</param>
		private void WriteLine(String line) {
			if (_writer == null) {
				return;
			}
			try {
			_writer.WriteLine (line);
			} catch (IOException e) {
				Console.Error.WriteLine ("AsyncLog failed to write line: \"" + line + "\" with exception: " + e);
				_writer = null;
			} catch (Exception e) {
				Console.Error.WriteLine ("AsyncLog failed to write line: \"" + line + "\" with exception: " + e);
			}
		}

		/// <summary>
		/// Opens a new writer for a log file with the file name dictated by <paramref name="time"/>.
		/// </summary>
		/// <param name="time">The time for the log file.</param>
		private void UpdateWriter(DateTime time) {
			bool overwrite = File.Exists (_directory + time.ToString("yyyyMMdd") + ".log");

			try {
				if(_writer != null) {
					_writer.Close();
				}
				_writer = File.AppendText(_directory + time.ToString("yyyyMMdd") + ".log");

				if (!overwrite) {
					WriteLine ("Timestamp".PadRight (25, ' ') + "\t" + "Data".PadRight (15, ' ') + "\t");
				}

				_writer.AutoFlush = true;
			} catch (UnauthorizedAccessException e) {
				Console.Error.WriteLine("AsyncLog does not have authority to access file: " + e);
				_writer = null;
			}  catch (Exception e) {
				Console.Error.WriteLine ("AsyncLog unable to set new writer correctly: "+e);
				_writer = null;
			} 
		}

		/// <summary>
		/// Creates a new log file with where <paramref name="time"/> is used to generate the name.
		/// Updates the internal variable that keeps track of when tomorrow is.
		/// </summary>
		/// <param name="time">The time used for the new log name.</param>
		private void CreateNewLog (DateTime time) {
			_tomorrowMidnightDate = GetStartOfTomorrow ();

			UpdateWriter (time);
		}


		/// <summary>
		/// Logger main loop. This loop takes lines from the queue and writes them to the log. 
		/// If midnight is crossed, then a new log file i opened.
		/// The loop runs until one of the two logger exit methods are called.
		/// </summary>
        private void MainLoop()
        {
			lock (_writer) {
				_logRunning = true;
				while (!_exit) {
					LogLine nextEntry;

					if (!_lines.TryDequeue (out nextEntry)) {
						// Queue is empty
						if (_quitWithFlush) {
							// We are done flushing. Exit loop.
							_exit = true;
						} else {
							// Yield for main process
							Thread.Yield ();
						}
						continue;
					}

					if (_tomorrowMidnightDate <= nextEntry.Timestamp) {
						CreateNewLog (nextEntry.Timestamp);
					}

					WriteLine (nextEntry.Timestamp.ToString ("yyyy-MM-dd HH:mm:ss:fff") + "\t" + nextEntry.LineText () + "\t");
				
					if (_quitWithFlush == true && _lines.IsEmpty) {
						_exit = true;
					}
					Thread.Yield ();
				}
			}
        }

		/// <summary>
		/// Stops logging. Outstanding writes will not be logged.
		/// </summary>
        public void StopWithoutFlush()
        {
            _exit = true;
			lock (_writer) {
				if (_writer != null) {
					_writer.Close ();
				}
			}
        }

		/// <summary>
		/// Stops logging. Outstanding writes will be written to the log.
		/// </summary>
        public void StopWithFlush()
        {
            _quitWithFlush = true;
			lock (_writer) {
				if (_writer != null) {
					_writer.Close ();
				}
			}
        }

		/// <summary>
		/// Writes a message to the Log.
		/// </summary>
		/// <param name="text">The text to be written to the log</param>
        public void Write(string text)
        {
			if (!_quitWithFlush) {
				_lines.Enqueue (new LogLine () { Text = text, Timestamp = _dateTime.Now });
			}
        }
    }
}