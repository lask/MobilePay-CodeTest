using System;
using System.IO;
using System.Threading;

namespace LogTest
{
	public class Tests
	{
		/// <summary>
		/// Tests the day change.
		/// </summary>
		/// <returns><c>true</c>, if day changed after crossing midnight, <c>false</c> otherwise.</returns>
		public bool TestDayChange ()
		{
			if (File.Exists ("./LogTest/20000101.log"))
				File.Delete ("./LogTest/20000101.log");

			if(File.Exists("./LogTest/20000102.log")) 
				File.Delete("./LogTest/20000102.log");

			TestDateProvider dt = new TestDateProvider (new DateTime (2000, 1, 1,23,59,59));
			AsyncLog al = new AsyncLog (dt);
			al.Write ("This should go in the file 20000101.log");
			dt.Now = new DateTime (2000, 1, 2, 0, 0, 1);
			al.Write ("This should go in the file 20000102.log");
			al.StopWithFlush ();
			return (File.Exists ("./LogTest/20000101.log") && File.Exists ("./LogTest/20000102.log"));

		}

		public bool TestCallToWrite ()
		{
			if (File.Exists ("./LogTest/20000101.log"))
				File.Delete ("./LogTest/20000101.log");

			TestDateProvider dt = new TestDateProvider (new DateTime (2000, 1, 1,23,59,59));
			AsyncLog al = new AsyncLog (dt);
			al.Write ("This should go in the file 20000101.log");
			al.StopWithFlush ();
			return File.Exists (@"./LogTest/20000101.log") && 
				File.ReadAllLines(@"./LogTest/20000101.log")[1].Substring(24,40) == ("This should go in the file 20000101.log.");		
		}

		public bool TestStopWithFlush ()
		{
			if (File.Exists ("./LogTest/20000103.log"))
				File.Delete ("./LogTest/20000103.log");

			TestDateProvider dt = new TestDateProvider (new DateTime (2000, 1, 3,23,59,59));
			AsyncLog al = new AsyncLog (dt);
			for (int i = 0; i < 100; i++) {
				al.Write ("This should go in the file 20000101.log ("+i+")");

			}
			al.StopWithFlush ();
			return File.Exists (@"./LogTest/20000103.log") && 
				File.ReadAllLines(@"./LogTest/20000103.log").Length == 101;		
		}

		public bool TestStopWithoutFlush ()
		{
			if (File.Exists ("./LogTest/20000103.log"))
				File.Delete ("./LogTest/20000103.log");

			TestDateProvider dt = new TestDateProvider (new DateTime (2000, 1, 3,23,59,59));
			AsyncLog al = new AsyncLog (dt);
			for (int i = 0; i < 10000; i++) {
				al.Write ("This should go in the file 20000101.log ("+i+")");

			}
			al.StopWithoutFlush ();
			return File.Exists (@"./LogTest/20000103.log") && 
				File.ReadAllLines(@"./LogTest/20000103.log").Length < 10001;		
		}

		static void Main(string[] args) {
			Tests tests = new Tests ();
			if (tests.TestDayChange()) {
				Console.Write ("PASSED: TestDayChange!");
			} else {
				Console.Write("FAILED: TestDayChange!");
			}
		}
	}
}

