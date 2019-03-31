using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Scheduling
{
    public class Priority
    {
        private string processName;
        private int brustTime;
        private int arrivalTime;
        private int priority;
        public int ival;

        public string ProcessName { get => processName; set => processName = value; }
        public int BrustTime { get => brustTime; set => brustTime = value; }
        public int ArrivalTime { get => arrivalTime; set => arrivalTime = value; }
        public int PriorityValue { get => priority; set => priority = value; }

        public Priority(string processName, int brustTime, int arrivalTime, int priority)
        {
            this.processName = processName;
            this.brustTime = brustTime;
            this.arrivalTime = arrivalTime;
            this.priority = priority;
        }
    }
}
