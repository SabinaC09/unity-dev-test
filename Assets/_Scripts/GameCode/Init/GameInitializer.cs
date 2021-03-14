using GameCode.CameraRig;
using GameCode.Elevator;
using GameCode.Finance;
using GameCode.Mineshaft;
using GameCode.Tutorial;
using GameCode.UI;
using GameCode.Warehouse;
using UniRx;
using UnityEngine;
using Newtonsoft.Json;

namespace GameCode.Init
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private HudView _hudView;
        [SerializeField] private CameraView _cameraView;

        [SerializeField] private MineshaftView _mineshaftView;
        [SerializeField] private ElevatorView _elevatorView;
        [SerializeField] private WarehouseView _warehouseView;
        
        GameStateModel _gameState;

        private void Start()
        {
            LoadGameState();
            var disposable = new CompositeDisposable().AddTo(this);

            var tutorialModel = new TutorialModel();
            var financeModel = new FinanceModel(_gameState);
            
            new CameraController(_cameraView, tutorialModel);

            

            //Mineshaft
            var mineshaftPool = new MineshaftPool();
            var mineshaftFactory = new MineshaftFactory(mineshaftPool, financeModel, _gameConfig, disposable);
            var mineshaftModel = new MineshaftModel(1, 1, _gameConfig, financeModel, disposable);
            new MineshaftController(_mineshaftView, mineshaftModel, mineshaftFactory, _gameConfig, disposable);
            mineshaftPool.RegisterMineshaft(1, mineshaftModel, _mineshaftView);

            //Elevator
            var elevatorModel = new ElevatorModel(_gameState.ElevatorLevel, _gameConfig, financeModel, disposable);
            new ElevatorController(_elevatorView, elevatorModel, mineshaftPool, _gameConfig, disposable);
            
            //Warehouse
            var warehouseModel = new WarehouseModel(_gameState.WarehouseLevel, _gameConfig, financeModel, disposable);
            new WarehouseController(_warehouseView, warehouseModel, elevatorModel, _gameConfig, disposable);

            //Hud
            new HudController(_hudView, financeModel, tutorialModel, mineshaftPool, warehouseModel, elevatorModel, disposable);

            financeModel.Money
              .Subscribe(money => _gameState.Money = money)
              .AddTo(disposable);

            warehouseModel.Level
              .Subscribe(level =>  _gameState.WarehouseLevel = level)
              .AddTo(disposable);

            elevatorModel.Level
              .Subscribe(level =>  _gameState.ElevatorLevel = level)
              .AddTo(disposable);


        }

        private void OnDestroy()
        {
            SaveGameState();
        }

        void LoadGameState()
        {
            string stateString= PlayerPrefs.GetString("state");

            _gameState = JsonConvert.DeserializeObject<GameStateModel>(stateString) ?? new GameStateModel();
        }

        void SaveGameState()
        {
            string stateString = JsonConvert.SerializeObject(_gameState);
            PlayerPrefs.SetString("state", stateString);
        }
    }
}