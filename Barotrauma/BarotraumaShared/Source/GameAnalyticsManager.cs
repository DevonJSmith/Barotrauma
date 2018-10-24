﻿using GameAnalyticsSDK.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace Barotrauma
{
    public static class GameAnalyticsManager
    {
        private static HashSet<string> sentEventIdentifiers = new HashSet<string>();

        public static void Init()
        {
#if DEBUG
            GameAnalytics.SetEnabledInfoLog(true);
#endif
            
            string exePath = Assembly.GetEntryAssembly().Location;
            string exeName = null;
            Md5Hash exeHash = null;
            exeName = Path.GetFileNameWithoutExtension(exePath).Replace(":", "");
            var md5 = MD5.Create();
            try
            {
                using (var stream = File.OpenRead(exePath))
                {
                    exeHash = new Md5Hash(stream);
                }
            }
            catch (Exception e)
            {
                DebugConsole.ThrowError("Error while calculating MD5 hash for the executable \"" + exePath + "\"", e);
            }
            
            GameAnalytics.ConfigureBuild(GameMain.Version.ToString()
                + (string.IsNullOrEmpty(exeName) ? "Unknown" : exeName) + ":"
                + ((exeHash?.ShortHash == null) ? "Unknown" : exeHash.ShortHash));
            
            GameAnalytics.AddDesignEvent("Executable:"
                + (string.IsNullOrEmpty(exeName) ? "Unknown" : exeName) + ":"
                + ((exeHash?.ShortHash == null) ? "Unknown" : exeHash.ShortHash));

            GameAnalytics.ConfigureAvailableCustomDimensions01("singleplayer", "multiplayer", "editor");
            GameAnalytics.Initialize("a3a073c20982de7c15d21e840e149122", "9010ad9a671233b8d9610d76cec8c897d9ff3ba7");
            
            string contentPackageName = GameMain.Config?.SelectedContentPackage?.Name;
            if (!string.IsNullOrEmpty(contentPackageName))
            {
                GameAnalytics.AddDesignEvent("ContentPackage:" +
                    contentPackageName.Replace(":", "").Substring(0, Math.Min(32, contentPackageName.Length)));
            }
        }

        /// <summary>
        /// Adds an error event to GameAnalytics if an event with the same identifier has not been added yet.
        /// </summary>
        public static void AddErrorEventOnce(string identifier, EGAErrorSeverity errorSeverity, string message)
        {
            if (!GameSettings.SendUserStatistics) return;
            if (sentEventIdentifiers.Contains(identifier)) return;

            GameAnalytics.AddErrorEvent(errorSeverity, message);
            sentEventIdentifiers.Add(identifier);
        }

        public static void AddDesignEvent(string eventID)
        {
            if (!GameSettings.SendUserStatistics) return;
            GameAnalytics.AddDesignEvent(eventID);
        }

        public static void AddDesignEvent(string eventID, double value)
        {
            if (!GameSettings.SendUserStatistics) return;
            GameAnalytics.AddDesignEvent(eventID, value);
        }

        public static void AddProgressionEvent(EGAProgressionStatus progressionStatus, string progression01)
        {
            if (!GameSettings.SendUserStatistics) return;
            GameAnalytics.AddProgressionEvent(progressionStatus, progression01);
        }

        public static void AddProgressionEvent(EGAProgressionStatus progressionStatus, string progression01, string progression02)
        {
            if (!GameSettings.SendUserStatistics) return;
            GameAnalytics.AddProgressionEvent(progressionStatus, progression01, progression02);
        }

        public static void SetCustomDimension01(string dimension)
        {
            if (!GameSettings.SendUserStatistics) return;
            GameAnalytics.SetCustomDimension01(dimension);
        }
    }
}
