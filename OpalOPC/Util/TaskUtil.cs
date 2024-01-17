
namespace Util
{
    public interface ITaskUtil
    {
        public void CheckForCancellation();
        public CancellationToken? token { set; }
    }

    public class TaskUtil : ITaskUtil
    {
        public CancellationToken? token { private get; set; } = null;

        public void CheckForCancellation()
        {
            if (token.HasValue)
            {
                token.Value.ThrowIfCancellationRequested();
            }
        }
    }
}
