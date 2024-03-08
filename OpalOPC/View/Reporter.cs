using Model;

namespace View
{
    public interface IReporter
    {
        public void WriteReportToStream(Report report, Stream outputStream);
    }
}
