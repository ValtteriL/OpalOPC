
namespace Util
{
    public class TaskUtil
    {
        public static void CheckForCancellation(CancellationToken? token)
        {
            if (token.HasValue)
            {
                token.Value.ThrowIfCancellationRequested();
            }
        }
    }
}
