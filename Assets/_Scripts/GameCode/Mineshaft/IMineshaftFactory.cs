using UnityEngine;

namespace GameCode.Mineshaft
{
    public interface IMineshaftFactory
    {
        MineshaftController CreateMineshaft(int mineshaftNumber, int mineshaftLevel, Vector2 position);
    }
}