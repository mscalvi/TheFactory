using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IShipService
    {
        /// <summary>
        /// Atualiza o progresso de viagem de todos os navios.
        /// Chamado a cada tick pelo ShipTickSink.
        /// </summary>
        void TickShips(GameModel game, double dtSeconds);

        /// <summary>
        /// Verifica se este navio pode iniciar uma viagem nesta rota,
        /// sem efeitos colaterais.
        /// </summary>
        bool CanStartVoyage(ShipModel ship, RouteModel route);

        /// <summary>
        /// Inicia uma viagem para a rota indicada.
        /// Decide automaticamente se é viagem de descoberta ou rota normal
        /// com base no estado da rota.
        /// Retorna true se a viagem foi iniciada com sucesso.
        /// </summary>
        bool StartVoyage(string shipId, string routeId);
    }

    /// <summary>
    /// Sink conectado ao TickService para atualizar viagens dos navios.
    /// </summary>
    public sealed class ShipTickSink : ITickSink, IDisposable
    {
        private readonly ITickService _ticks;
        private readonly IShipService _ships;

        public ShipTickSink(ITickService ticks, IShipService ships)
        {
            _ticks = ticks;
            _ships = ships;
            _ticks.Subscribe(this);
        }

        public void OnTick(GameModel game, double dtSeconds)
        {
            _ships.TickShips(game, dtSeconds);
        }

        public void Dispose() => _ticks.Unsubscribe(this);
    }

    /// <summary>
    /// Lida com viagens de navios, descoberta de rotas e rotas normais.
    /// </summary>
    public sealed class ShipService : IShipService
    {
        private readonly ICurrentGameService _game;
        private readonly IUiLogService _log;

        public ShipService(ICurrentGameService game, IUiLogService log)
        {
            _game = game;
            _log = log;
        }

        #region Tick

        public void TickShips(GameModel game, double dtSeconds)
        {
            if (game is null || dtSeconds <= 0) return;
            if (game.Ships is null || game.Ships.Count == 0) return;
            if (game.Routes is null || game.Routes.Count == 0) return;

            foreach (var ship in game.Ships.Values)
            {
                if (ship is null) continue;

                if (ship.ShipState != UnlockHelper.ShipState.InDiscovery &&
                    ship.ShipState != UnlockHelper.ShipState.InRoute)
                    continue;

                if (string.IsNullOrWhiteSpace(ship.InRouteId))
                    continue;

                if (!game.Routes.TryGetValue(ship.InRouteId, out var route) || route is null)
                    continue;

                var travelSeconds = ComputeTravelDurationSeconds(ship, route);
                if (travelSeconds <= 0)
                    travelSeconds = 1; // fallback defensivo

                ship.TravelProgress += dtSeconds / travelSeconds;

                if (ship.TravelProgress >= 1.0)
                {
                    ship.TravelProgress = 1.0;
                    OnVoyageCompleted(game, ship, route);
                }

                if (ship.TravelProgress < 0) ship.TravelProgress = 0;
                if (ship.TravelProgress > 1) ship.TravelProgress = 1;
            }
        }

        private static double ComputeTravelDurationSeconds(ShipModel ship, RouteModel route)
        {
            // Fórmula simples: tempo = distância / velocidade
            // Ajuste à vontade depois.
            var dist = Math.Max(1, route.Distance);
            var speed = Math.Max(1, ship.Speed);

            return dist / (double)speed;
        }

        private void OnVoyageCompleted(GameModel game, ShipModel ship, RouteModel route)
        {
            var wasDiscovery = ship.ShipState == UnlockHelper.ShipState.InDiscovery;

            ship.TravelProgress = 0;
            ship.InRouteId = null;
            ship.ShipState = UnlockHelper.ShipState.InStage;

            if (wasDiscovery)
            {
                // Descoberta de rota concluída
                if (route.RouteState == UnlockHelper.RouteState.Available ||
                    route.RouteState == UnlockHelper.RouteState.Discovering)
                {
                    route.RouteState = UnlockHelper.RouteState.Known;
                }

                _log.Lore($"O navio {ship.Name} descobriu a rota {route.Name}.");
            }
            else
            {
                // Rota normal concluída (melhorar relações / influências no futuro)
                _log.Info($"O navio {ship.Name} concluiu a rota {route.Name}.");
            }

            // Aqui é um ótimo ponto para:
            // - aplicar influência na ilha destino (route.PointB)
            // - disparar triggers de unlock para upgrades de Stage (us02, etc.)
        }

        #endregion

        #region Start Voyage

        public bool CanStartVoyage(ShipModel ship, RouteModel route)
        {
            if (ship is null || route is null) return false;

            // Navio precisa estar desbloqueado e parado
            if (ship.State != UnlockHelper.State.Unlocked)
                return false;

            if (ship.ShipState == UnlockHelper.ShipState.InDiscovery ||
                ship.ShipState == UnlockHelper.ShipState.InRoute)
                return false;

            // Rota não pode estar bloqueada
            if (route.RouteState == UnlockHelper.RouteState.Blocked)
                return false;

            // Porto de origem precisa bater com a rota
            if (!string.Equals(ship.InStageId, route.PointA, StringComparison.Ordinal))
                return false;

            return true;
        }

        public bool StartVoyage(string shipId, string routeId)
        {
            if (string.IsNullOrWhiteSpace(shipId) || string.IsNullOrWhiteSpace(routeId))
                return false;

            bool started = false;

            _game.Mutate(g =>
            {
                if (g.Ships is null || g.Routes is null) return;

                if (!g.Ships.TryGetValue(shipId, out var ship) || ship is null)
                    return;

                if (!g.Routes.TryGetValue(routeId, out var route) || route is null)
                    return;

                if (!CanStartVoyage(ship, route))
                    return;

                bool isDiscovery = route.RouteState != UnlockHelper.RouteState.Known;

                ship.InRouteId = route.Id;
                ship.TravelProgress = 0;
                ship.ShipState = isDiscovery
                    ? UnlockHelper.ShipState.InDiscovery
                    : UnlockHelper.ShipState.InRoute;

                if (isDiscovery && route.RouteState == UnlockHelper.RouteState.Available)
                {
                    route.RouteState = UnlockHelper.RouteState.Discovering;
                }

                // Logs simples
                if (isDiscovery)
                {
                    _log.Info($"O navio {ship.Name} partiu para descobrir a rota {route.Name}.");
                }
                else
                {
                    _log.Info($"O navio {ship.Name} partiu em rota para {route.Name}.");
                }

                started = true;

            }, save: true);

            return started;
        }

        #endregion
    }
}
