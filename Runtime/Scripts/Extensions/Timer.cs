using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
#if ID_LOGGER
using instance.id.Logging;
using Logger = instance.id.Logging.Logger;
#endif

#if ID_INFOPANEL
using instance.id.InfoPanel.Runtime;
#endif

namespace instance.id.EATK
{
    // --| Performance Measurement ------------------------
    // --| ------------------------------------------------
    internal class Timer
    {
        internal enum DisplayType { Panel, Console, All }
        private static Type logType;

        public Dictionary<(string, string), Stopwatch> sw;
        private string defaultCategory;
        private bool customCategory;

        public Timer(string category = "Performance", Type loggingType = default, bool useCustomCategory = false)
        {
            defaultCategory = category;
            logType = loggingType;
            customCategory = useCustomCategory;
            sw = new Dictionary<(string, string), Stopwatch>();
        }
        
        public void PrintTimeOf(string name, string category = "Performance")
        {
            if (customCategory)
                category = defaultCategory;

            var key = (name, category);
            if (sw.ContainsKey(key))
                Log($"[{category}][{name}]: {sw[key].Elapsed}");
        }

        public string GetTimeOf(string name, string category = "Performance")
        {
            if (customCategory)
                category = defaultCategory;

            var key = (name, category);
            return sw.ContainsKey(key) 
                ? $"[{category}][{name}]: {sw[key].Elapsed}" 
                : $"[{category}][{name}]: No Timer Found.";
        }

        public void Toggle(string name, string category = "Performance")
        {
            if (customCategory)
                category = defaultCategory;
            
            var key = (name, category);
            if (sw.ContainsKey(key) && sw[key].IsRunning)
            {
                sw[key].Stop();
                return;
            }

            sw.Add(key, new Stopwatch());
            sw[key].Start();
        }

        public void Start(string name, string category = "Performance")
        {
            if (customCategory)
                category = defaultCategory;
            
            var key = (name, category);
            if (!sw.ContainsKey(key))
                sw.Add(key, new Stopwatch());

            sw[key].Start();
        }

        public void Stop(string name, string category = "Performance")
        {
            if (customCategory)
                category = defaultCategory;
            
            var key = (name, category);
            if (sw.ContainsKey(key))
                sw[key].Stop();
        }

        public void StopAll()
        {
            foreach (var item in sw)
                item.Value.Stop();
        }

        public void Reset(string name, string category = "Performance")
        {
            var key = (name, category);
            if (sw.ContainsKey(key))
                sw[key].Reset();
        }

        public void Print(string name, string category = "Performance")
        {
            if (customCategory)
                category = defaultCategory;
            
            var key = (name, category);
            Log($"[{category}][{name}]: {sw[key].Elapsed}");
        }

        public void PrintAll(DisplayType type = DisplayType.Panel)
        { // @formatter:off
            switch (type)
            {
                case DisplayType.Panel:   PrintToPanel(); break;
                case DisplayType.Console: PrintToConsole(); break;
                case DisplayType.All:     PrintToPanel(); PrintToConsole(); break;
            } // @formatter:on
        }

        private void PrintToConsole()
        {
            foreach (var item in sw)
            {
                Log($"[{item.Key.Item2}][{item.Key.Item1}]: {item.Value.Elapsed}");
            }
        }

        private void PrintToPanel()
        {
#if ID_INFOPANEL && UNITY_EDITOR
            if (infoPanel.i.CheckForPanel())
            {
                sw.forEach(x =>
                {
                    if (x.Key.Item1 == null || x.Key.Item1 == "TotalTime") return;
                    if (x.Value.IsRunning) x.Value.Stop();
                    infoPanel.i.Log(
                        x.Key.Item2,
                        x.Key.Item1,
                        $@"{x.Value.Elapsed:hh\:mm\:ss\.fffff}"
                    );
                });

                var key = ("TotalTime", defaultCategory);
                if (sw.ContainsKey(key))
                    infoPanel.i.Log(
                        defaultCategory,
                        "TotalTime:",
                        $@"{sw[key].Elapsed:hh\:mm\:ss\.fffff}"
                    );
            }

#endif
#if !ID_INFOPANEL && UNITY_EDITOR
            PrintToConsole();
#endif
        }

        // --| Logging Methods --------------------------------------
#if ID_LOGGER
        [Conditional("ID_LOGGER")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden] [HideInCallstack] [StackTraceHidden]
        private static void Log(string message, Level level = default)
            => Logger.Log(level, message, Channel.Performance, logType.Name);
#else
        [Conditional("UNITY_EDITOR")]
        private static void Log(string message)
            => UnityEngine.Debug.Log(message);
#endif
        
    }
}
