using Model;

namespace View
{
    public interface ISarifReporter : IReporter
    {
    }
    public class SarifReporter : ISarifReporter
    {
        public void WriteReportToStream(Report report, Stream outputStream)
        {
            return; // TODO
        }
    }
}
