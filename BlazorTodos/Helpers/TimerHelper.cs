using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorTodos.Helpers
{
    public class TimerHelper
    {

        public Action OnTick { get; set; }

        private Timer timer1;

        private bool IsRunning = false;

        public void Start(int DueTime, int Period)
        {
            if (IsRunning)
            {
                Stop();
            }
            else
            {
                IsRunning = true;

                if (DueTime < 1)
                {
                    DueTime = 1; //It can be 0 but in case of 0 blazor does not update
                }

                if (Period < 1)
                {
                    Period = 1;
                }

                timer1 = new Timer(Timer1Callback, null, DueTime, Period);

            }

        }


        private void Timer1Callback(object o)
        {
            OnTick?.Invoke();

        }


        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                if (timer1 != null)
                {
                    timer1.Dispose();
                }
            }

        }

    }
}
