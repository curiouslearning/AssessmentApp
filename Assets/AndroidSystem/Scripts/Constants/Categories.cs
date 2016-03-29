using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLManipulation
{
    public static class Categories
    {
        public const string ALTERNATIVE = "android.intent.category.ALTERNATIVE";
        public const string APP_BROWSER = "android.intent.category.APP_BROWSER";
        public const string APP_CALCULATOR = "android.intent.category.APP_CALCULATOR";
        public const string APP_CALENDAR = "android.intent.category.APP_CALENDAR";
        public const string APP_CONTACTS = "android.intent.category.APP_CONTACTS";
        public const string APP_EMAIL = "android.intent.category.APP_EMAIL";
        public const string APP_GALLERY = "android.intent.category.APP_GALLERY";
        public const string APP_MAPS = "android.intent.category.APP_MAPS";
        public const string APP_MARKET = "android.intent.category.APP_MARKET";
        public const string APP_MESSAGING = "android.intent.category.APP_MESSAGING";
        public const string APP_MUSIC = "android.intent.category.APP_MUSIC";
        public const string BROWSABLE = "android.intent.category.BROWSABLE";
        public const string CAR_DOCK = "android.intent.category.CAR_DOCK";
        public const string CAR_MODE = "android.intent.category.CAR_MODE";
        public const string DEFAULT = "android.intent.category.DEFAULT";
        public const string DESK_DOCK = "android.intent.category.DESK_DOCK";
        public const string DEVELOPMENT_PREFERENCE = "android.intent.category.DEVELOPMENT_PREFERENCE";
        public const string EMBED = "android.intent.category.EMBED";
        public const string FRAMEWORK_INSTRUMENTATION_TEST = "android.intent.category.FRAMEWORK_INSTRUMENTATION_TEST";
        public const string HE_DESK_DOCK = "android.intent.category.HE_DESK_DOCK";
        public const string HOME = "android.intent.category.HOME";
        public const string INFO = "android.intent.category.INFO";
        public const string LAUNCHER = "android.intent.category.LAUNCHER";
        public const string LEANBACK_LAUNCHER = "android.intent.category.LEANBACK_LAUNCHER";
        public const string LE_DESK_DOCK = "android.intent.category.LE_DESK_DOCK";
        public const string MONKEY = "android.intent.category.MONKEY";
        public const string OPENABLE = "android.intent.category.OPENABLE";
        public const string PREFERENCE = "android.intent.category.PREFERENCE";
        public const string SAMPLE_CODE = "android.intent.category.SAMPLE_CODE";
        public const string SELECTED_ALTERNATIVE = "android.intent.category.SELECTED_ALTERNATIVE";
        public const string TAB = "android.intent.category.TAB";
        public const string TEST = "android.intent.category.TEST";
        public const string UNIT_TEST = "android.intent.category.UNIT_TEST";

        public static readonly Dictionary<string, string> categories = new Dictionary<string, string>()
        {
            { "ALTERNATIVE", ALTERNATIVE },
            { "APP_BROWSER", APP_BROWSER },
            { "APP_CALCULATOR", APP_CALCULATOR },
            { "APP_CALENDAR", APP_CALENDAR },
            { "APP_CONTACTS", APP_CONTACTS },
            { "APP_EMAIL", APP_EMAIL },
            { "APP_GALLERY", APP_GALLERY },
            { "APP_MAPS", APP_MAPS },
            { "APP_MARKET", APP_MARKET },
            { "APP_MESSAGING", APP_MESSAGING },
            { "APP_MUSIC", APP_MUSIC },
            { "BROWSABLE", BROWSABLE },
            { "CAR_DOCK", CAR_DOCK },
            { "CAR_MODE", CAR_MODE },
            { "DEFAULT", DEFAULT },
            { "DESK_DOCK", DESK_DOCK },
            { "DEVELOPMENT_PREFERENCE", DEVELOPMENT_PREFERENCE },
            { "EMBED", EMBED },
            { "FRAMEWORK_INSTRUMENTATION_TEST", FRAMEWORK_INSTRUMENTATION_TEST },
            { "HE_DESK_DOCK", HE_DESK_DOCK },
            { "HOME", HOME },
            { "INFO", INFO },
            { "LAUNCHER", LAUNCHER },
            { "LEANBACK_LAUNCHER", LEANBACK_LAUNCHER },
            { "LE_DESK_DOCK", LE_DESK_DOCK },
            { "MONKEY", MONKEY },
            { "OPENABLE", OPENABLE },
            { "PREFERENCE", PREFERENCE },
            { "SAMPLE_CODE", SAMPLE_CODE },
            { "SELECTED_ALTERNATIVE", SELECTED_ALTERNATIVE },
            { "TAB", TAB },
            { "TEST", TEST },
            { "UNIT_TEST", UNIT_TEST }
        };

    }
}
