using GameCode.Finance;
using GameCode.Init;
using UniRx;
using UnityEngine;

namespace GameCode.Mineshaft
{
    public class MineshaftFactory : IMineshaftFactory
    {
        private readonly MineshaftPool _pool;
        private readonly FinanceModel _financeModel;
        private readonly GameConfig _config;
        private readonly CompositeDisposable _disposable;
        private readonly GameStateModel _gameStateModel;

        public MineshaftFactory(MineshaftPool pool, FinanceModel financeModel, GameConfig config, GameStateModel gameStateModel, CompositeDisposable disposable)
        {
            _pool = pool;
            _financeModel = financeModel;
            _config = config;
            _disposable = disposable;
            _gameStateModel = gameStateModel;
        }

        public MineshaftController CreateMineshaft(int mineshaftNumber, int mineshaftLevel, Vector2 position)
        {
            var view = Object.Instantiate(_config.MineshaftConfig.MineshaftPrefab, position, Quaternion.identity);
            var mineshaftModel = new MineshaftModel(mineshaftNumber, mineshaftLevel, _config, _financeModel, _disposable);
            var mineshaftController = new MineshaftController(view, mineshaftModel, this, _config, _gameStateModel, _disposable);
            _pool.RegisterMineshaft(mineshaftNumber, mineshaftModel, view);
            return mineshaftController;
        }
    }
}