using System.Collections.Generic;
using UniRx;

namespace GameCode.Mineshaft
{
    public class MineshaftPool
    {
        private readonly Dictionary<int, MineshaftView> _views;
        private readonly Dictionary<int, MineshaftModel> _models;

        private IReactiveProperty<int> _mineshaftCount;
        public IReactiveProperty<int> MineshaftCount => _mineshaftCount;

        public MineshaftPool()
        {
            _views = new Dictionary<int, MineshaftView>();
            _models = new Dictionary<int, MineshaftModel>();
            _mineshaftCount = new ReactiveProperty<int>(0);
        }

        public void RegisterMineshaft(int mineshaftNumber, MineshaftModel model, MineshaftView view)
        {
            _views.Add(mineshaftNumber, view);
            _models.Add(mineshaftNumber, model);
            _mineshaftCount.Value++;
        }

        public int GetCount()
        {
            return _models.Count;
        }

        public MineshaftModel GetModel(int mineshaftNumber)
        {
            return _models[mineshaftNumber];
        }

        public MineshaftView GetView(int mineshaftNumber)
        {
            return _views[mineshaftNumber];
        }
}
}