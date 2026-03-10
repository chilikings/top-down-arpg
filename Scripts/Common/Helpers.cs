using DOUKH.Common.Numbers;
using DOUKH.Common.Strings;
using DOUKH.Common.Enums;
using UnityEngine;

namespace DOUKH.Common.Helpers
{
    public static class Helper
    {
        public static void SetDefault<T>(ref T fieldValue, T defaultValue) { if (fieldValue.ToString() == "0") fieldValue = defaultValue; }

        public static T GetDefault<T>(T fieldValue, T defaultValue) => fieldValue.ToString() == "0" ? defaultValue : fieldValue;

        public static bool AreDifferent(float value1, float value2, float error = Number.DefaultError) => Diff(value1, value2) > error;

        public static float Diff(float value1, float value2) => Mathf.Abs(value1 - value2);

        public static float GetDeltaTime(DeltaTimeType deltaTimeType) => deltaTimeType switch
        {
            DeltaTimeType.Normal => Time.deltaTime,
            DeltaTimeType.Fixed => Time.fixedDeltaTime,
            DeltaTimeType.Unscaled => Time.unscaledDeltaTime,
            DeltaTimeType.Smooth => Time.smoothDeltaTime
        };

        public static float CalcValueFactor(float value, float min, float max) => (value - min) / (max - min);

        public static bool ArePenetrated(Collider c1, Collider c2) => Physics.ComputePenetration(c1, c1.transform.position, c1.transform.rotation, c2, c2.transform.position, c2.transform.rotation, out _, out _);

        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask) => layerMask == (layerMask | (1 << obj.layer)); //((1 << obj.layer) & layerMask) != 0

        public static bool IsWisp(Transform character) => character?.gameObject.layer == Number.WispLayer;

        public static bool IsGuise(Transform character) => character?.gameObject.layer == Number.GuiseLayer;

        public static bool IsEnemy(Transform character) => character?.gameObject.layer == Number.EnemyLayer;

        public static bool IsInsideLimits(float value, Vector2 limits) => value > limits.x && value < limits.y;

        public static void Log<T>(T value, string text = null, bool isSeparated = false) => Debug.Log(string.Concat(text ?? nameof(value), " = ", value, isSeparated ? String.SeparateLine : null));

    }
}