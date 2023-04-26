namespace View
{
    public interface IReporter
    {
        public void printPdfReport(string filename);
    }

    public class Reporter : IReporter
    {
    }
}