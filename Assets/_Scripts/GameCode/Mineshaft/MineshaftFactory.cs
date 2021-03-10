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

        public MineshaftFactory(MineshaftPool pool, FinanceModel financeModel, GameConfig config, CompositeDisposable disposable)
        {
            _pool = pool;
            _financeModel = financeModel;
            _config = config;
            _disposable = disposable;
        }

        public void CreateMineshaft(int mineshaftNumber, int mineshaftLevel, Vector2 position)
        {
            var view = Object.Instantiate(_config.MineshaftConfig.MineshaftPrefab, position, Quaternion.identity);
            var mineshaftModel = new MineshaftModel(mineshaftNumber, mineshaftLevel, _config, _financeModel, _disposable);
            new MineshaftController(view, mineshaftModel, this, _config, _disposable);
            _pool.RegisterMineshaft(mineshaftNumber, mineshaftModel, view);
        }
    }
}