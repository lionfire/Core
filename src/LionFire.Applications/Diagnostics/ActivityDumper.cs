#define ShowTicksInConsole
using System;
using Timer = System.Timers.Timer;

namespace LionFire.Diagnostics
{
    public class ActivityDumper
    {
        private int ticksInLastInterval = 0;

        private Timer timer;

        #region Enabled

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value)
                {
                    return;
                }

                enabled = value;
                if (enabled)
                {
                    timer = new Timer(60000);
                    timer.Elapsed += ShowTicksInConsole_Timer_Elapsed;
                    timer.Enabled = true;
                }
                else
                {
                    timer.Elapsed -= ShowTicksInConsole_Timer_Elapsed;
                    timer.Enabled = false;
                    timer = null;
                }
            }
        }
        private bool enabled;

        #endregion

        #region Verbose

        private void ShowTicksInConsole_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ticksInLastInterval > 0)
            {
                Console.WriteLine();
            }

            for (int i = 0; i < ticksInLastInterval; i++)
            {
                Console.Write((char)8);
            }

            Console.WriteLine($"{DateTime.Now} {ticksInLastInterval} ticks");
            ticksInLastInterval = 0;
        }

        public void OnActivity(string v)
        {
            Console.Write(v);
            ticksInLastInterval++;
        }

        #endregion

    }
}
