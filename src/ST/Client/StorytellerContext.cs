using System.Threading.Tasks;
using StoryTeller.Messages;
using StoryTeller.Remotes;

namespace ST.Client
{
    public class StorytellerContext
    {
        private readonly RemoteController _controller;
        private readonly OpenInput _input;
        private Task<SystemRecycled> _startup;

        public StorytellerContext(RemoteController controller, OpenInput input)
        {
            _controller = controller;
            _input = input;
        }

        public void Start()
        {
            _startup = _controller.Start(EngineMode.Interactive).ContinueWith(t =>
            {
                t.Result.WriteSystemUsage();

                return t.Result;
            });
        }

        public QueueState QueueState()
        {
            return _controller.QueueState();
        }

        public void AddRemoteListener(object listener)
        {
            _controller.Messaging.AddListener(listener);
        }

        public void WaitForResults()
        {
            _startup.Wait();
        }

        public string SpecPath
        {
            get { return _input.SpecPath; }
        }


        public Task<SystemRecycled> Startup
        {
            get { return _startup; }
        }

        public SystemRecycled LatestSystemRecycled
        {
            get { return _controller.LatestSystemRecycled ?? Startup.Result; }
        }
    }
}