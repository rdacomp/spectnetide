using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Run Z80 tests command
    /// </summary>
    [CommandId(0x0815)]
    public class RunAllZ80TestsCommand : Z80TestCommandBase
    {
        /// <summary>
        /// Override this property to allow project item selection
        /// </summary>
        public override bool AllowProjectItem => true;

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override Task ExecuteAsync()
        {
            return Task.FromResult(0);
        }
    }
}