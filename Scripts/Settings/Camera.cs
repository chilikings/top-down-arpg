using DOUKH.CM3.Settings;
using DOUKH.OBF.Settings;
using UnityEngine;

namespace DOUKH.Settings.Camera
{
    public static class CameraSettings
    {
        private static SettingsCM3 _cm3;
        private static SettingsOBF _obf;

        public static SettingsCM3 CM3 => _cm3 ??= Resources.Load<SettingsCM3>("Settings/CM3");
        public static SettingsOBF OBF => _obf ??= Resources.Load<SettingsOBF>("Settings/OBF");
    }
}