using GameCode.Finance;
using GameCode.Mineshaft;
using GameCode.Tutorial;
using UniRx;
using UnityEngine;

namespace GameCode.UI
{
    public class HudController
    {
        private readonly HudView _view;

        public HudController(HudView view, FinanceModel financeModel, ITutorialModel tutorialModel, MineshaftPool mineshaftPool,
            CompositeDisposable disposable)
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