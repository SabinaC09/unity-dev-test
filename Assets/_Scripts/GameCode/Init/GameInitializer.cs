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
using UnityEngine.SceneManagement;

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


            if (_gameState.MineshaftLevels.Count == 0)
            {
                _gameState.MineshaftLevels.Add(1);
            }

            //Mineshaft
            var mineshaftPool = new MineshaftPool();
            var mineshaftFactory = new MineshaftFactory(mineshaftPool, financeModel, _gameConfig, _gameState, disposable);
            var mineshaftModel = new MineshaftModel(1, _gameState.MineshaftLevels[0], _gameConfig, financeModel, disposable);
            var mineshaftController = new MineshaftController(_mineshaftView, mineshaftModel, mineshaftFactory, _gameConfig,_gameState, disposable);
            mineshaftPool.RegisterMineshaft(1, mineshaftModel, _mineshaftView);

           

            for(int i = 1; i < _gameState.MineshaftLevels.Count; i++)
            {
                mineshaftController.BuyNextShaft(true);
                mineshaftController = mineshaftController.NextController;
                for(int j=0; j < _gameState.MineshaftLevels[i] - 1; j++)
                {
                    mineshaftController.Upgrade(true);
                }
            }

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

            _hudView.RestartButton.OnClickAsObservable().Subscribe(_ => restartClick());
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

        public void restartClick()
        {
            PlayerPrefs.DeleteAll();
            _gameState = new GameStateModel();
            SceneManager.LoadScene(0);
        }
    }
}