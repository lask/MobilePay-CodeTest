using System;

namespace LogTest
{
	/// <summary>
	/// Class for getting the current date.
	/// </summary>
	public class DateProvider : IDateProvider
	{
		public DateProvider () {}

		/// <summary>
		/// Gets a DateTime object that represents now..
		/// </summary>
		/// <value>DateTime that represents now.</value>
		public DateTime Now {
			get {
				return DateTime.Now;
			}
		}
	}
}

