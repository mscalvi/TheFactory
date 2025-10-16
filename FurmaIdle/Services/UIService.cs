namespace FurmaIdle.Services
{
    public interface IUiService
    {
        void CreateScreen();

        Task LoadStage(string stageId);
    }

    public sealed class UiService : IUiService
    {
        private readonly ILogger<UiService> _log;
        private readonly ICurrentGameService _game;

        public UiService(ILogger<UiService> log, ICurrentGameService game)
        {
            _log = log;
            _game = game;
        }

        public void CreateScreen()
        {

        }

        public async Task LoadStage(string stageId)
        {
            await _game.Mutate(g =>
            {
                if (string.IsNullOrWhiteSpace(stageId) || g is null)
                    return;

                if (!g.Stages.TryGetValue(stageId, out var stage))
                    throw new KeyNotFoundException($"Stage '{stageId}' não existe no jogo atual.");

                var before = g.SelectedStageId;
                g.SelectedStageId = stageId;

                _log.LogInformation("[UI] LoadStage: {Before} -> {After}", before, stageId);
            });
        }
    }
}
