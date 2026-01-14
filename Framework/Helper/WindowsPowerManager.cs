using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Framework.Helper
{
    public static class WindowsPowerManager
    {
        // https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setthreadexecutionstate
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [Flags]
        private enum EXECUTION_STATE : uint
        {
            ES_CONTINUOUS = 0x80000000,
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040
        }

        /// <summary>
        /// Prüft, ob die Power-Management-Funktion verfügbar ist
        /// </summary>
        public static bool IsAvailable()
        {
            try {
                EXECUTION_STATE result = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                if(result != 0)
                    result = SetThreadExecutionState(result);
                return result != 0;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Verhindert, dass der PC in den Standby geht (Display kann ausgehen)
        /// </summary>
        /// <exception cref="Win32Exception">Wird geworfen, wenn der API-Aufruf fehlschlägt</exception>
        public static void PreventSleep()
        {
            EXECUTION_STATE result = SetThreadExecutionState(
                EXECUTION_STATE.ES_CONTINUOUS 
                | EXECUTION_STATE.ES_SYSTEM_REQUIRED
            );

            if(result == 0) {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "SetThreadExecutionState konnte Standby-Verhinderung nicht aktivieren");
            }
        }

        /// <summary>
        /// Verhindert Standby UND hält das Display aktiv
        /// </summary>
        /// <exception cref="Win32Exception">Wird geworfen, wenn der API-Aufruf fehlschlägt</exception>
        public static void PreventSleepAndDisplayOff()
        {
            EXECUTION_STATE result = SetThreadExecutionState(
                EXECUTION_STATE.ES_CONTINUOUS 
                | EXECUTION_STATE.ES_SYSTEM_REQUIRED 
                | EXECUTION_STATE.ES_DISPLAY_REQUIRED
            );

            if(result == 0) {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "SetThreadExecutionState konnte Standby- und Display-Verhinderung nicht aktivieren");
            }
        }

        /// <summary>
        /// Stellt die normalen Energieeinstellungen wieder her
        /// </summary>
        /// <exception cref="Win32Exception">Wird geworfen, wenn der API-Aufruf fehlschlägt</exception>
        public static void AllowSleep()
        {
            EXECUTION_STATE result = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);

            if(result == 0) {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "SetThreadExecutionState konnte Energieeinstellungen nicht wiederherstellen");
            }
        }
    }
}
