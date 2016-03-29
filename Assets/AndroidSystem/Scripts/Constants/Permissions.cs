using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLManipulation
{
    public static class Permissions
    {
        public const string ACCESS_CHECKIN_PROPERTIES = "android.permission.ACCESS_CHECKIN_PROPERTIES";
        public const string ACCESS_COARSE_LOCATION = "android.permission.ACCESS_COARSE_LOCATION";
        public const string ACCESS_FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";
        public const string ACCESS_LOCATION_EXTRA_COMMANDS = "android.permission.ACCESS_LOCATION_EXTRA_COMMANDS";
        public const string ACCESS_MOCK_LOCATION = "android.permission.ACCESS_MOCK_LOCATION";
        public const string ACCESS_NETWORK_STATE = "android.permission.ACCESS_NETWORK_STATE";
        public const string ACCESS_SURFACE_FLINGER = "android.permission.ACCESS_SURFACE_FLINGER";
        public const string ACCESS_WIFI_STATE = "android.permission.ACCESS_WIFI_STATE";
        public const string ACCOUNT_MANAGER = "android.permission.ACCOUNT_MANAGER";
        public const string ADD_VOICEMAIL = "com.android.voicemail.permission.ADD_VOICEMAIL";
        public const string AUTHENTICATE_ACCOUNTS = "android.permission.AUTHENTICATE_ACCOUNTS";
        public const string BATTERY_STATS = "android.permission.BATTERY_STATS";
        public const string BIND_ACCESSIBILITY_SERVICE = "android.permission.BIND_ACCESSIBILITY_SERVICE";
        public const string BIND_APPWIDGET = "android.permission.BIND_APPWIDGET";
        public const string BIND_DEVICE_ADMIN = "android.permission.BIND_DEVICE_ADMIN";
        public const string BIND_DREAM_SERVICE = "android.permission.BIND_DREAM_SERVICE";
        public const string BIND_INPUT_METHOD = "android.permission.BIND_INPUT_METHOD";
        public const string BIND_NFC_SERVICE = "android.permission.BIND_NFC_SERVICE";
        public const string BIND_NOTIFICATION_LISTENER_SERVICE = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE";
        public const string BIND_PRINT_SERVICE = "android.permission.BIND_PRINT_SERVICE";
        public const string BIND_REMOTEVIEWS = "android.permission.BIND_REMOTEVIEWS";
        public const string BIND_TEXT_SERVICE = "android.permission.BIND_TEXT_SERVICE";
        public const string BIND_TV_INPUT = "android.permission.BIND_TV_INPUT";
        public const string BIND_VOICE_INTERACTION = "android.permission.BIND_VOICE_INTERACTION";
        public const string BIND_VPN_SERVICE = "android.permission.BIND_VPN_SERVICE";
        public const string BIND_WALLPAPER = "android.permission.BIND_WALLPAPER";
        public const string BLUETOOTH = "android.permission.BLUETOOTH";
        public const string BLUETOOTH_ADMIN = "android.permission.BLUETOOTH_ADMIN";
        public const string BLUETOOTH_PRIVILEGED = "android.permission.BLUETOOTH_PRIVILEGED";
        public const string BODY_SENSORS = "android.permission.BODY_SENSORS";
        public const string BRICK = "android.permission.BRICK";
        public const string BROADCAST_PACKAGE_REMOVED = "android.permission.BROADCAST_PACKAGE_REMOVED";
        public const string BROADCAST_SMS = "android.permission.BROADCAST_SMS";
        public const string BROADCAST_STICKY = "android.permission.BROADCAST_STICKY";
        public const string BROADCAST_WAP_PUSH = "android.permission.BROADCAST_WAP_PUSH";
        public const string CALL_PHONE = "android.permission.CALL_PHONE";
        public const string CALL_PRIVILEGED = "android.permission.CALL_PRIVILEGED";
        public const string CAMERA = "android.permission.CAMERA";
        public const string CAPTURE_AUDIO_OUTPUT = "android.permission.CAPTURE_AUDIO_OUTPUT";
        public const string CAPTURE_SECURE_VIDEO_OUTPUT = "android.permission.CAPTURE_SECURE_VIDEO_OUTPUT";
        public const string CAPTURE_VIDEO_OUTPUT = "android.permission.CAPTURE_VIDEO_OUTPUT";
        public const string CHANGE_COMPONENT_ENABLED_STATE = "android.permission.CHANGE_COMPONENT_ENABLED_STATE";
        public const string CHANGE_CONFIGURATION = "android.permission.CHANGE_CONFIGURATION";
        public const string CHANGE_NETWORK_STATE = "android.permission.CHANGE_NETWORK_STATE";
        public const string CHANGE_WIFI_MULTICAST_STATE = "android.permission.CHANGE_WIFI_MULTICAST_STATE";
        public const string CHANGE_WIFI_STATE = "android.permission.CHANGE_WIFI_STATE";
        public const string CLEAR_APP_CACHE = "android.permission.CLEAR_APP_CACHE";
        public const string CLEAR_APP_USER_DATA = "android.permission.CLEAR_APP_USER_DATA";
        public const string CONTROL_LOCATION_UPDATES = "android.permission.CONTROL_LOCATION_UPDATES";
        public const string DELETE_CACHE_FILES = "android.permission.DELETE_CACHE_FILES";
        public const string DELETE_PACKAGES = "android.permission.DELETE_PACKAGES";
        public const string DEVICE_POWER = "android.permission.DEVICE_POWER";
        public const string DIAGNOSTIC = "android.permission.DIAGNOSTIC";
        public const string DISABLE_KEYGUARD = "android.permission.DISABLE_KEYGUARD";
        public const string DUMP = "android.permission.DUMP";
        public const string EXPAND_STATUS_BAR = "android.permission.EXPAND_STATUS_BAR";
        public const string FACTORY_TEST = "android.permission.FACTORY_TEST";
        public const string FLASHLIGHT = "android.permission.FLASHLIGHT";
        public const string FORCE_BACK = "android.permission.FORCE_BACK";
        public const string GET_ACCOUNTS = "android.permission.GET_ACCOUNTS";
        public const string GET_PACKAGE_SIZE = "android.permission.GET_PACKAGE_SIZE";
        //@Deprecated
        public const string GET_TASKS = "android.permission.GET_TASKS";
        public const string GET_TOP_ACTIVITY_INFO = "android.permission.GET_TOP_ACTIVITY_INFO";
        public const string GLOBAL_SEARCH = "android.permission.GLOBAL_SEARCH";
        public const string HARDWARE_TEST = "android.permission.HARDWARE_TEST";
        public const string INJECT_EVENTS = "android.permission.INJECT_EVENTS";
        public const string INSTALL_LOCATION_PROVIDER = "android.permission.INSTALL_LOCATION_PROVIDER";
        public const string INSTALL_PACKAGES = "android.permission.INSTALL_PACKAGES";
        public const string INSTALL_SHORTCUT = "com.android.launcher.permission.INSTALL_SHORTCUT";
        public const string INTERNAL_SYSTEM_WINDOW = "android.permission.INTERNAL_SYSTEM_WINDOW";
        public const string INTERNET = "android.permission.INTERNET";
        public const string KILL_BACKGROUND_PROCESSES = "android.permission.KILL_BACKGROUND_PROCESSES";
        public const string LOCATION_HARDWARE = "android.permission.LOCATION_HARDWARE";
        public const string MANAGE_ACCOUNTS = "android.permission.MANAGE_ACCOUNTS";
        public const string MANAGE_APP_TOKENS = "android.permission.MANAGE_APP_TOKENS";
        public const string MANAGE_DOCUMENTS = "android.permission.MANAGE_DOCUMENTS";
        public const string MASTER_CLEAR = "android.permission.MASTER_CLEAR";
        public const string MEDIA_CONTENT_CONTROL = "android.permission.MEDIA_CONTENT_CONTROL";
        public const string MODIFY_AUDIO_SETTINGS = "android.permission.MODIFY_AUDIO_SETTINGS";
        public const string MODIFY_PHONE_STATE = "android.permission.MODIFY_PHONE_STATE";
        public const string MOUNT_FORMAT_FILESYSTEMS = "android.permission.MOUNT_FORMAT_FILESYSTEMS";
        public const string MOUNT_UNMOUNT_FILESYSTEMS = "android.permission.MOUNT_UNMOUNT_FILESYSTEMS";
        public const string NFC = "android.permission.NFC";
        //@Deprecated
        public const string PERSISTENT_ACTIVITY = "android.permission.PERSISTENT_ACTIVITY";
        public const string PROCESS_OUTGOING_CALLS = "android.permission.PROCESS_OUTGOING_CALLS";
        public const string READ_CALENDAR = "android.permission.READ_CALENDAR";
        public const string READ_CALL_LOG = "android.permission.READ_CALL_LOG";
        public const string READ_CONTACTS = "android.permission.READ_CONTACTS";
        public const string READ_EXTERNAL_STORAGE = "android.permission.READ_EXTERNAL_STORAGE";
        public const string READ_FRAME_BUFFER = "android.permission.READ_FRAME_BUFFER";
        public const string READ_HISTORY_BOOKMARKS = "com.android.browser.permission.READ_HISTORY_BOOKMARKS";
        //@Deprecated
        public const string READ_INPUT_STATE = "android.permission.READ_INPUT_STATE";
        public const string READ_LOGS = "android.permission.READ_LOGS";
        public const string READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";
        public const string READ_PROFILE = "android.permission.READ_PROFILE";
        public const string READ_SMS = "android.permission.READ_SMS";
        //@Deprecated
        public const string READ_SOCIAL_STREAM = "android.permission.READ_SOCIAL_STREAM";
        public const string READ_SYNC_SETTINGS = "android.permission.READ_SYNC_SETTINGS";
        public const string READ_SYNC_STATS = "android.permission.READ_SYNC_STATS";
        public const string READ_USER_DICTIONARY = "android.permission.READ_USER_DICTIONARY";
        public const string READ_VOICEMAIL = "com.android.voicemail.permission.READ_VOICEMAIL";
        public const string REBOOT = "android.permission.REBOOT";
        public const string RECEIVE_BOOT_COMPLETED = "android.permission.RECEIVE_BOOT_COMPLETED";
        public const string RECEIVE_MMS = "android.permission.RECEIVE_MMS";
        public const string RECEIVE_SMS = "android.permission.RECEIVE_SMS";
        public const string RECEIVE_WAP_PUSH = "android.permission.RECEIVE_WAP_PUSH";
        public const string RECORD_AUDIO = "android.permission.RECORD_AUDIO";
        public const string REORDER_TASKS = "android.permission.REORDER_TASKS";
        //@Deprecated
        public const string RESTART_PACKAGES = "android.permission.RESTART_PACKAGES";
        public const string SEND_RESPOND_VIA_MESSAGE = "android.permission.SEND_RESPOND_VIA_MESSAGE";
        public const string SEND_SMS = "android.permission.SEND_SMS";
        public const string SET_ACTIVITY_WATCHER = "android.permission.SET_ACTIVITY_WATCHER";
        public const string SET_ALARM = "com.android.alarm.permission.SET_ALARM";
        public const string SET_ALWAYS_FINISH = "android.permission.SET_ALWAYS_FINISH";
        public const string SET_ANIMATION_SCALE = "android.permission.SET_ANIMATION_SCALE";
        public const string SET_DEBUG_APP = "android.permission.SET_DEBUG_APP";
        public const string SET_ORIENTATION = "android.permission.SET_ORIENTATION";
        public const string SET_POINTER_SPEED = "android.permission.SET_POINTER_SPEED";
        //@Deprecated
        public const string SET_PREFERRED_APPLICATIONS = "android.permission.SET_PREFERRED_APPLICATIONS";
        public const string SET_PROCESS_LIMIT = "android.permission.SET_PROCESS_LIMIT";
        public const string SET_TIME = "android.permission.SET_TIME";
        public const string SET_TIME_ZONE = "android.permission.SET_TIME_ZONE";
        public const string SET_WALLPAPER = "android.permission.SET_WALLPAPER";
        public const string SET_WALLPAPER_HINTS = "android.permission.SET_WALLPAPER_HINTS";
        public const string SIGNAL_PERSISTENT_PROCESSES = "android.permission.SIGNAL_PERSISTENT_PROCESSES";
        public const string STATUS_BAR = "android.permission.STATUS_BAR";
        public const string SUBSCRIBED_FEEDS_READ = "android.permission.SUBSCRIBED_FEEDS_READ";
        public const string SUBSCRIBED_FEEDS_WRITE = "android.permission.SUBSCRIBED_FEEDS_WRITE";
        public const string SYSTEM_ALERT_WINDOW = "android.permission.SYSTEM_ALERT_WINDOW";
        public const string TRANSMIT_IR = "android.permission.TRANSMIT_IR";
        public const string UNINSTALL_SHORTCUT = "com.android.launcher.permission.UNINSTALL_SHORTCUT";
        public const string UPDATE_DEVICE_STATS = "android.permission.UPDATE_DEVICE_STATS";
        public const string USE_CREDENTIALS = "android.permission.USE_CREDENTIALS";
        public const string USE_SIP = "android.permission.USE_SIP";
        public const string VIBRATE = "android.permission.VIBRATE";
        public const string WAKE_LOCK = "android.permission.WAKE_LOCK";
        public const string WRITE_APN_SETTINGS = "android.permission.WRITE_APN_SETTINGS";
        public const string WRITE_CALENDAR = "android.permission.WRITE_CALENDAR";
        public const string WRITE_CALL_LOG = "android.permission.WRITE_CALL_LOG";
        public const string WRITE_CONTACTS = "android.permission.WRITE_CONTACTS";
        public const string WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
        public const string WRITE_GSERVICES = "android.permission.WRITE_GSERVICES";
        public const string WRITE_HISTORY_BOOKMARKS = "com.android.browser.permission.WRITE_HISTORY_BOOKMARKS";
        public const string WRITE_PROFILE = "android.permission.WRITE_PROFILE";
        public const string WRITE_SECURE_SETTINGS = "android.permission.WRITE_SECURE_SETTINGS";
        public const string WRITE_SETTINGS = "android.permission.WRITE_SETTINGS";
        public const string WRITE_SMS = "android.permission.WRITE_SMS";
        //@Deprecated
        public const string WRITE_SOCIAL_STREAM = "android.permission.WRITE_SOCIAL_STREAM";
        public const string WRITE_SYNC_SETTINGS = "android.permission.WRITE_SYNC_SETTINGS";
        public const string WRITE_USER_DICTIONARY = "android.permission.WRITE_USER_DICTIONARY";
        public const string WRITE_VOICEMAIL = "com.android.voicemail.permission.WRITE_VOICEMAIL";

        public const string BILLING = "com.android.vending.BILLING";
        public const string INSTALL_REFERRER = "com.android.vending.INSTALL_REFERRER";

        public static readonly Dictionary<string, string> permissions = new Dictionary<string, string>()
        {
            { "ACCESS_CHECKIN_PROPERTIES", ACCESS_CHECKIN_PROPERTIES },
            { "ACCESS_COARSE_LOCATION", ACCESS_COARSE_LOCATION },
            { "ACCESS_FINE_LOCATION", ACCESS_FINE_LOCATION },
            { "ACCESS_LOCATION_EXTRA_COMMANDS", ACCESS_LOCATION_EXTRA_COMMANDS },
            { "ACCESS_MOCK_LOCATION", ACCESS_MOCK_LOCATION },
            { "ACCESS_NETWORK_STATE", ACCESS_NETWORK_STATE },
            { "ACCESS_SURFACE_FLINGER", ACCESS_SURFACE_FLINGER },
            { "ACCESS_WIFI_STATE", ACCESS_WIFI_STATE },
            { "ACCOUNT_MANAGER", ACCOUNT_MANAGER },
            { "ADD_VOICEMAIL", ADD_VOICEMAIL },
            { "AUTHENTICATE_ACCOUNTS", AUTHENTICATE_ACCOUNTS },
            { "BATTERY_STATS", BATTERY_STATS },
            { "BIND_ACCESSIBILITY_SERVICE", BIND_ACCESSIBILITY_SERVICE },
            { "BIND_APPWIDGET", BIND_APPWIDGET },
            { "BIND_DEVICE_ADMIN", BIND_DEVICE_ADMIN },
            { "BIND_DREAM_SERVICE", BIND_DREAM_SERVICE },
            { "BIND_INPUT_METHOD", BIND_INPUT_METHOD },
            { "BIND_NFC_SERVICE", BIND_NFC_SERVICE },
            { "BIND_NOTIFICATION_LISTENER_SERVICE", BIND_NOTIFICATION_LISTENER_SERVICE },
            { "BIND_PRINT_SERVICE", BIND_PRINT_SERVICE },
            { "BIND_REMOTEVIEWS", BIND_REMOTEVIEWS },
            { "BIND_TEXT_SERVICE", BIND_TEXT_SERVICE },
            { "BIND_TV_INPUT", BIND_TV_INPUT },
            { "BIND_VOICE_INTERACTION", BIND_VOICE_INTERACTION },
            { "BIND_VPN_SERVICE", BIND_VPN_SERVICE },
            { "BIND_WALLPAPER", BIND_WALLPAPER },
            { "BLUETOOTH", BLUETOOTH },
            { "BLUETOOTH_ADMIN", BLUETOOTH_ADMIN },
            { "BLUETOOTH_PRIVILEGED", BLUETOOTH_PRIVILEGED },
            { "BODY_SENSORS", BODY_SENSORS },
            { "BRICK", BRICK },
            { "BROADCAST_PACKAGE_REMOVED", BROADCAST_PACKAGE_REMOVED },
            { "BROADCAST_SMS", BROADCAST_SMS },
            { "BROADCAST_STICKY", BROADCAST_STICKY },
            { "BROADCAST_WAP_PUSH", BROADCAST_WAP_PUSH },
            { "CALL_PHONE", CALL_PHONE },
            { "CALL_PRIVILEGED", CALL_PRIVILEGED },
            { "CAMERA", CAMERA },
            { "CAPTURE_AUDIO_OUTPUT", CAPTURE_AUDIO_OUTPUT },
            { "CAPTURE_SECURE_VIDEO_OUTPUT", CAPTURE_SECURE_VIDEO_OUTPUT },
            { "CAPTURE_VIDEO_OUTPUT", CAPTURE_VIDEO_OUTPUT },
            { "CHANGE_COMPONENT_ENABLED_STATE", CHANGE_COMPONENT_ENABLED_STATE },
            { "CHANGE_CONFIGURATION", CHANGE_CONFIGURATION },
            { "CHANGE_NETWORK_STATE", CHANGE_NETWORK_STATE },
            { "CHANGE_WIFI_MULTICAST_STATE", CHANGE_WIFI_MULTICAST_STATE },
            { "CHANGE_WIFI_STATE", CHANGE_WIFI_STATE },
            { "CLEAR_APP_CACHE", CLEAR_APP_CACHE },
            { "CLEAR_APP_USER_DATA", CLEAR_APP_USER_DATA },
            { "CONTROL_LOCATION_UPDATES", CONTROL_LOCATION_UPDATES },
            { "DELETE_CACHE_FILES", DELETE_CACHE_FILES },
            { "DELETE_PACKAGES", DELETE_PACKAGES },
            { "DEVICE_POWER", DEVICE_POWER },
            { "DIAGNOSTIC", DIAGNOSTIC },
            { "DISABLE_KEYGUARD", DISABLE_KEYGUARD },
            { "DUMP", DUMP },
            { "EXPAND_STATUS_BAR", EXPAND_STATUS_BAR },
            { "FACTORY_TEST", FACTORY_TEST },
            { "FLASHLIGHT", FLASHLIGHT },
            { "FORCE_BACK", FORCE_BACK },
            { "GET_ACCOUNTS", GET_ACCOUNTS },
            { "GET_PACKAGE_SIZE", GET_PACKAGE_SIZE },
            //@Deprecated
            { "GET_TASKS", GET_TASKS },
            { "GET_TOP_ACTIVITY_INFO", GET_TOP_ACTIVITY_INFO },
            { "GLOBAL_SEARCH", GLOBAL_SEARCH },
            { "HARDWARE_TEST", HARDWARE_TEST },
            { "INJECT_EVENTS", INJECT_EVENTS },
            { "INSTALL_LOCATION_PROVIDER", INSTALL_LOCATION_PROVIDER },
            { "INSTALL_PACKAGES", INSTALL_PACKAGES },
            { "INSTALL_SHORTCUT", INSTALL_SHORTCUT },
            { "INTERNAL_SYSTEM_WINDOW", INTERNAL_SYSTEM_WINDOW },
            { "INTERNET", INTERNET },
            { "KILL_BACKGROUND_PROCESSES", KILL_BACKGROUND_PROCESSES },
            { "LOCATION_HARDWARE", LOCATION_HARDWARE },
            { "MANAGE_ACCOUNTS", MANAGE_ACCOUNTS },
            { "MANAGE_APP_TOKENS", MANAGE_APP_TOKENS },
            { "MANAGE_DOCUMENTS", MANAGE_DOCUMENTS },
            { "MASTER_CLEAR", MASTER_CLEAR },
            { "MEDIA_CONTENT_CONTROL", MEDIA_CONTENT_CONTROL },
            { "MODIFY_AUDIO_SETTINGS", MODIFY_AUDIO_SETTINGS },
            { "MODIFY_PHONE_STATE", MODIFY_PHONE_STATE },
            { "MOUNT_FORMAT_FILESYSTEMS", MOUNT_FORMAT_FILESYSTEMS },
            { "MOUNT_UNMOUNT_FILESYSTEMS", MOUNT_UNMOUNT_FILESYSTEMS },
            { "NFC", NFC },
            //@Deprecated
            { "PERSISTENT_ACTIVITY", PERSISTENT_ACTIVITY },
            { "PROCESS_OUTGOING_CALLS", PROCESS_OUTGOING_CALLS },
            { "READ_CALENDAR", READ_CALENDAR },
            { "READ_CALL_LOG", READ_CALL_LOG },
            { "READ_CONTACTS", READ_CONTACTS },
            { "READ_EXTERNAL_STORAGE", READ_EXTERNAL_STORAGE },
            { "READ_FRAME_BUFFER", READ_FRAME_BUFFER },
            { "READ_HISTORY_BOOKMARKS", READ_HISTORY_BOOKMARKS },
            //@Deprecated
            { "READ_INPUT_STATE", READ_INPUT_STATE },
            { "READ_LOGS", READ_LOGS },
            { "READ_PHONE_STATE", READ_PHONE_STATE },
            { "READ_PROFILE", READ_PROFILE },
            { "READ_SMS", READ_SMS },
            //@Deprecated
            { "READ_SOCIAL_STREAM", READ_SOCIAL_STREAM },
            { "READ_SYNC_SETTINGS", READ_SYNC_SETTINGS },
            { "READ_SYNC_STATS", READ_SYNC_STATS },
            { "READ_USER_DICTIONARY", READ_USER_DICTIONARY },
            { "READ_VOICEMAIL", READ_VOICEMAIL },
            { "REBOOT", REBOOT },
            { "RECEIVE_BOOT_COMPLETED", RECEIVE_BOOT_COMPLETED },
            { "RECEIVE_MMS", RECEIVE_MMS },
            { "RECEIVE_SMS", RECEIVE_SMS },
            { "RECEIVE_WAP_PUSH", RECEIVE_WAP_PUSH },
            { "RECORD_AUDIO", RECORD_AUDIO },
            { "REORDER_TASKS", REORDER_TASKS },
            //@Deprecated
            { "RESTART_PACKAGES", RESTART_PACKAGES },
            { "SEND_RESPOND_VIA_MESSAGE", SEND_RESPOND_VIA_MESSAGE },
            { "SEND_SMS", SEND_SMS },
            { "SET_ACTIVITY_WATCHER", SET_ACTIVITY_WATCHER },
            { "SET_ALARM", SET_ALARM },
            { "SET_ALWAYS_FINISH", SET_ALWAYS_FINISH },
            { "SET_ANIMATION_SCALE", SET_ANIMATION_SCALE },
            { "SET_DEBUG_APP", SET_DEBUG_APP },
            { "SET_ORIENTATION", SET_ORIENTATION },
            { "SET_POINTER_SPEED", SET_POINTER_SPEED },
            //@Deprecated
            { "SET_PREFERRED_APPLICATIONS", SET_PREFERRED_APPLICATIONS },
            { "SET_PROCESS_LIMIT", SET_PROCESS_LIMIT },
            { "SET_TIME", SET_TIME },
            { "SET_TIME_ZONE", SET_TIME_ZONE },
            { "SET_WALLPAPER", SET_WALLPAPER },
            { "SET_WALLPAPER_HINTS", SET_WALLPAPER_HINTS },
            { "SIGNAL_PERSISTENT_PROCESSES", SIGNAL_PERSISTENT_PROCESSES },
            { "STATUS_BAR", STATUS_BAR },
            { "SUBSCRIBED_FEEDS_READ", SUBSCRIBED_FEEDS_READ },
            { "SUBSCRIBED_FEEDS_WRITE", SUBSCRIBED_FEEDS_WRITE },
            { "SYSTEM_ALERT_WINDOW", SYSTEM_ALERT_WINDOW },
            { "TRANSMIT_IR", TRANSMIT_IR },
            { "UNINSTALL_SHORTCUT", UNINSTALL_SHORTCUT },
            { "UPDATE_DEVICE_STATS", UPDATE_DEVICE_STATS },
            { "USE_CREDENTIALS", USE_CREDENTIALS },
            { "USE_SIP", USE_SIP },
            { "VIBRATE", VIBRATE },
            { "WAKE_LOCK", WAKE_LOCK },
            { "WRITE_APN_SETTINGS", WRITE_APN_SETTINGS },
            { "WRITE_CALENDAR", WRITE_CALENDAR },
            { "WRITE_CALL_LOG", WRITE_CALL_LOG },
            { "WRITE_CONTACTS", WRITE_CONTACTS },
            { "WRITE_EXTERNAL_STORAGE", WRITE_EXTERNAL_STORAGE },
            { "WRITE_GSERVICES", WRITE_GSERVICES },
            { "WRITE_HISTORY_BOOKMARKS", WRITE_HISTORY_BOOKMARKS },
            { "WRITE_PROFILE", WRITE_PROFILE },
            { "WRITE_SECURE_SETTINGS", WRITE_SECURE_SETTINGS },
            { "WRITE_SETTINGS", WRITE_SETTINGS },
            { "WRITE_SMS", WRITE_SMS },
            //@Deprecated
            { "WRITE_SOCIAL_STREAM", WRITE_SOCIAL_STREAM },
            { "WRITE_SYNC_SETTINGS", WRITE_SYNC_SETTINGS },
            { "WRITE_USER_DICTIONARY", WRITE_USER_DICTIONARY },
            { "WRITE_VOICEMAIL", WRITE_VOICEMAIL },

            { "BILLING", BILLING },
            { "INSTALL_REFERRER", INSTALL_REFERRER}
        };
    }
}
