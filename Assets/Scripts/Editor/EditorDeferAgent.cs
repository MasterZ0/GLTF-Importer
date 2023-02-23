using GLTFast;
using System.Threading.Tasks;

namespace GLTFImporter.Editor
{
    public class EditorDeferAgent : IDeferAgent
    {
        public Task BreakPoint()
        {
            return Task.CompletedTask;
        }

        public Task BreakPoint(float duration)
        {
            return Task.CompletedTask;
        }

        public bool ShouldDefer()
        {
            return false;
        }

        public bool ShouldDefer(float duration)
        {
            return false;
        }
    }
}