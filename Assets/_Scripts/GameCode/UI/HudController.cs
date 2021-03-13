using GameCode.Elevator;
using GameCode.Finance;
using GameCode.Mineshaft;
using GameCode.Tutorial;
using GameCode.Warehouse;
using UniRx;
using UnityEngine;

namespace GameCode.UI
{
    public class HudController
    {
        private readonly HudView _view;

        public HudController(HudView view, FinanceModel financeModel, ITutorialModel tutorialModel, MineshaftPool mineshaftPool,
            WarehouseModel warehouseModel, ElevatorModel elevatorModel, CompositeDisposable disposable)
        {
            _view = view;

            _view.StatisticsButton.onClick.AddListener(onClick);

            var moneyRow = GameObject.Instantiate(_view.StatisticsRowPrefab, _view.StatisticsPanel.transform).GetComponent<StatisticsRow>();
            moneyRow.key.text = "Money";

            financeModel.Money
                .Subscribe(money => { view.CashAmount = money; moneyRow.value.text = money.ToString("F0"); })
                .AddTo(disposable);
            
            tutorialModel.ShouldShowTooltip
                .Subscribe(UpdateTooltipVisibility)
                .AddTo(disposable);

            var mineshaftsRow = GameObject.Instantiate(_view.StatisticsRowPrefab, _view.StatisticsPanel.transform).GetComponent<StatisticsRow>();
            mineshaftsRow.key.text = "Mineshafts #";

            mineshaftPool.MineshaftCount
                .Subscribe(count => mineshaftsRow.value.text = count + "")
                .AddTo(disposable);

            var warehouseLevelRow = GameObject.Instantiate(_view.StatisticsRowPrefab, _view.StatisticsPanel.transform).GetComponent<StatisticsRow>();
            warehouseLevelRow.key.text = "Warehouse lvl";

            warehouseModel.WarehouseLevel
                .Subscribe(level => warehouseLevelRow.value.text = level + "")
                .AddTo(disposable);

            var elevatorLevelRow = GameObject.Instantiate(_view.StatisticsRowPrefab, _view.StatisticsPanel.transform).GetComponent<StatisticsRow>();
            elevatorLevelRow.key.text = "Elevator lvl";

            elevatorModel.ElevatorLevel
                .Subscribe(level => elevatorLevelRow.value.text = level + "")
                .AddTo(disposable);

            var headerRow = GameObject.Instantiate(_view.StatisticsRowPrefab, _view.StatisticsPanel.transform).GetComponent<StatisticsRow>();
            headerRow.key.text = "Mineshaft";
            headerRow.value.text = "Capacity";

            var firstMineshaftRow = GameObject.Instantiate(_view.StatisticsRowPrefab, _view.StatisticsPanel.transform).GetComponent<StatisticsRow>();
            firstMineshaftRow.key.text = mineshaftPool.MineshaftModels[0].MineshaftNumber + " -level:" +mineshaftPool.MineshaftModels[0].Level;
            firstMineshaftRow.value.text = mineshaftPool.MineshaftModels[0].CarryingCapacity.Value.ToString("F0");
            mineshaftPool.MineshaftModels[0].CarryingCapacity
                .Subscribe(capacity => firstMineshaftRow.value.text = capacity.ToString("F0"))
                .AddTo(disposable);

            mineshaftPool.MineshaftModels[0].Level
                .Subscribe(level => firstMineshaftRow.key.text = mineshaftPool.MineshaftModels[0].MineshaftNumber + " -level:" + level)
                .AddTo(disposable);

            mineshaftPool.MineshaftModels
                .ObserveAdd()
                .Subscribe(ev => {
                    var mineshaftRow = GameObject.Instantiate(_view.StatisticsRowPrefab, _view.StatisticsPanel.transform).GetComponent<StatisticsRow>();
                    mineshaftRow.key.text = ev.Value.MineshaftNumber + " -level:" + ev.Value.Level;
                    mineshaftRow.value.text = ev.Value.CarryingCapacity.Value.ToString("F0");

                   ev.Value.CarryingCapacity
                    .Subscribe(capacity => mineshaftRow.value.text = capacity.ToString("F0"))
                    .AddTo(disposable);

                    ev.Value.Level
                    .Subscribe(level => mineshaftRow.key.text = ev.Value.MineshaftNumber + " -level:" + level)
                    .AddTo(disposable);
                })
                .AddTo(disposable);


        }

        private void onClick()
        {
            _view.StatisticsPanel.SetActive(!_view.StatisticsPanel.activeSelf);
        }

        private void UpdateTooltipVisibility(bool shouldShowTooltip)
        {
            _view.TooltipVisible = shouldShowTooltip;
        }
    }
}