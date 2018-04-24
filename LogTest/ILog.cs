using System.Linq;
using System.Threading.Tasks;

namespace LogTest
{
    public interface ILog
    {
        /// <summary>
        /// Stops logging. Outstanding writes will not be logged.
        /// </summary>
        void StopWithoutFlush();

        /// <summary>
        /// Stops logging. Outstanding writes will be written to the log.
        /// </summary>
        void StopWithFlush();

        /// <summary>
        /// Writes a message to the Log.
        /// </summary>
        /// <param name="text">The text to be written to the log</param>
        void Write(string text);


    }
}
