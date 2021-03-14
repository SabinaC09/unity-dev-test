using GameCode.Init;
using GameCode.Worker;
using UniRx;

namespace GameCode.Mineshaft
{
    public class MineshaftController
    {
        private readonly MineshaftView _view;
        private readonly MineshaftModel _model;
        private readonly IMineshaftFactory _mineshaftFactory;
        private readonly GameStateModel _gameStateModel;

        private MineshaftController _nextController;
        public MineshaftController NextController => _nextController;

        public MineshaftController(MineshaftView view, MineshaftModel model, IMineshaftFactory mineshaftFactory, 
            GameConfig gameConfig, GameStateModel gameStateModel, CompositeDisposable disposable)
        {
            _view = view;
            _model = model;
            _mineshaftFactory = mineshaftFactory;
            _gameStateModel = gameStateModel;

            var workerModel = new WorkerModel(model, gameConfig.MineshaftWorkerConfig, disposable);
            new MineshaftWorkerController(view, model, workerModel, disposable);

            var canUpgrade = model.CanUpgrade.ToReactiveCommand();
            canUpgrade.BindTo(view.AreaUiCanvasView.UpgradeButton).AddTo(disposable);
            canUpgrade.Subscribe(_ => Upgrade())
                .AddTo(disposable);

            model.StashAmount
                .Subscribe(amount => view.StashAmount = amount.ToString("F0"))
                .AddTo(disposable);
            workerModel.CarryingCapacity
                .Subscribe(capacity => { view.AreaUiCanvasView.CarryingCapacity = capacity.ToString("F0"); _model.CarryingCapacity.Value=capacity; })
                .AddTo(disposable);

            model.UpgradePrice
                .Subscribe(upgradePrice => view.AreaUiCanvasView.UpgradeCost = upgradePrice.ToString("F0"))
                .AddTo(disposable);

            view.NextShaftView.Cost = model.NextShaftPrice.ToString("F0");
            var canBuyNextShaft = model.CanBuyNextShaft.ToReactiveCommand();
            canBuyNextShaft.BindTo(view.NextShaftView.Button).AddTo(disposable);
            canBuyNextShaft.Subscribe(_ => BuyNextShaft())
                .AddTo(disposable);

            
        }

        public void Upgrade(bool free = false)
        {
            _model.Upgrade(free);

            if(!free)
            {
                _gameStateModel.MineshaftLevels[_model.MineshaftNumber - 1]++;
            }
           
        }

        public void BuyNextShaft(bool free = false)
        {
            if(free == false)
            {
                _model.BuyNextShaft();
                _gameStateModel.MineshaftLevels.Add(1);
            }
            
            _view.NextShaftView.Visible = false;
            _nextController = _mineshaftFactory.CreateMineshaft(_model.MineshaftNumber + 1, 1, _view.NextShaftView.NextShaftPosition);
        }
    }
}