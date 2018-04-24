using System;

namespace LogTest
{
	/// <summary>
	/// Test stub. This class allows the date to be set to allow for testing.
	/// </summary>
	public class TestDateProvider : IDateProvider
	{
		private DateTime _dateTime;

		/// <summary>
		/// Initializes a new instance of the <see cref="LogTest.TestDateProvider"/> class.
		/// </summary>
		/// <param name="dt">The initial DateTime object returned by this DateProvider.</param>
		public TestDateProvider (DateTime dt)
		{
			_dateTime = dt;
		}

		/// <summary>
		/// Gets or sets the DateTime returned by this DateProvider.
		/// </summary>
		/// <value>Now according to this test stub.</value>
		public DateTime Now {
			get {
				return _dateTime;
			}
			set {
				_dateTime = value;
			}
		}

	}
}

