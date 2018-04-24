using System;

namespace LogTest
{
	public interface IDateProvider
	{
		/// <summary>
		/// Gets a DateTime object that represents now..
		/// </summary>
		/// <value>DateTime that represents now.</value>
		DateTime Now { get; }
	}
}

