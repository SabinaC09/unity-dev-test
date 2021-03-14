using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace GameCode.UI
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _cashAmount;
        [SerializeField] private GameObject _tooltip;
        [SerializeField] private Button _statisticsButton;
        [SerializeField] private GameObject _statisticsPanel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _switchMineButton;
        [SerializeField] private Button _speedBoostButton;

        [Header("Prefabs")]
        [SerializeField] private GameObject _statisticsRowPrefab;

        private List<StatisticsRow> _statisticsRows=new List<StatisticsRow>();


        public double CashAmount
        {
            set => _cashAmount.SetText(value.ToString("F0"));
        }

        public bool TooltipVisible
        {
            set => _tooltip.gameObject.SetActive(value);
        }

        public Button StatisticsButton => _statisticsButton;

        public GameObject StatisticsPanel => _statisticsPanel;

        public GameObject StatisticsRowPrefab => _statisticsRowPrefab;

        public List<StatisticsRow> StatisticsRows => _statisticsRows;

        public Button RestartButton => _restartButton;

        public Button SwitchMineButton => _switchMineButton;

        public Button SpeedBoostButton => _speedBoostButton;
    }
}