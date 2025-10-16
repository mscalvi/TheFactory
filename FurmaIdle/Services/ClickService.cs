using Microsoft.Extensions.Logging;

namespace FurmaIdle.Services
{
    public interface IClickService
    {
        Task ClickAsync(int count = 1);
    }
    public sealed class ClickService : IClickService
    {
        private readonly ILogger<ClickService> _log;
        private readonly ICurrentGameService _game;

        public ClickService(ILogger<ClickService> log, ICurrentGameService game)
        {
            _log = log;
            _game = game;
        }

        public async Task ClickAsync(int count = 1)
        {
            await _game.Mutate(g =>
            {

            }, save: false);
        }
    }
}
