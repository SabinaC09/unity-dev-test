using GameCode.CameraRig;
using GameCode.Elevator;
using GameCode.Finance;
using GameCode.Mineshaft;
using GameCode.Tutorial;
using GameCode.UI;
using GameCode.Warehouse;
using UniRx;
using UnityEngine;

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

        private void Start()
        {
            var disposable = new CompositeDisposable().AddTo(this);

            var tutorialModel = new TutorialModel();
            var financeModel = new FinanceModel();
            
            new CameraController(_cameraView, tutorialModel);

            

            //Mineshaft
            var mineshaftPool = new MineshaftPool();
            var mineshaftFactory = new MineshaftFactory(mineshaftPool, financeModel, _gameConfig, disposable);
            var mineshaftModel = new MineshaftModel(1, 1, _gameConfig, financeModel, disposable);
            new MineshaftController(_mineshaftView, mineshaftModel, mineshaftFactory, _gameConfig, disposable);
            mineshaftPool.RegisterMineshaft(1, mineshaftModel, _mineshaftView);

            //Elevator
            var elevatorModel = new ElevatorModel(1, _gameConfig, financeModel, disposable);
            new ElevatorController(_elevatorView, elevatorModel, mineshaftPool, _gameConfig, disposable);
            
            //Warehouse
            var warehouseModel = new WarehouseModel(1, _gameConfig, financeModel, disposable);
            new WarehouseController(_warehouseView, warehouseModel, elevatorModel, _gameConfig, disposable);

            //Hud
            new HudController(_hudView, financeModel, tutorialModel, mineshaftPool, warehouseModel, elevatorModel, disposable);
        }
    }
}