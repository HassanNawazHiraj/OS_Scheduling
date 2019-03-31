using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Scheduling
{
    public class Arrival
    {
        private string processName;
        private int brustTime;
        private int arrivalTime;
        public int ival;

        public string ProcessName { get => processName; set => processName = value; }
        public int BrustTime { get => brustTime; set => brustTime = value; }
        public int ArivalTime { get => arrivalTime; set => arrivalTime = value; }

        public Arrival(string processName, int brustTime, int arivalTime)
        {
            ProcessName = processName;
            BrustTime = brustTime;
            ArivalTime = arivalTime;
        }


    }
}
