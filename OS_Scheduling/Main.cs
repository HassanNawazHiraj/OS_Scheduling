using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OS_Scheduling
{
    public partial class Main : Form
    {
        private List<Arrival> arrivalList = new List<Arrival>();
        private List<Priority> priorityList = new List<Priority>();
        private BindingSource source = new BindingSource();
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //preloaded data
            //typeCombo.SelectedIndex = 2;



            //arrivalList.Add(new Arrival("p1", 5, 0));
            //arrivalList.Add(new Arrival("p2", 6, 2));
            //arrivalList.Add(new Arrival("p3", 1, 10));

            //arrivalList.Add(new Arrival("p4", 7, 1));
            //arrivalList.Add(new Arrival("p5", 3, 1));
            //source.DataSource = arrivalList;
            //inputGrid.DataSource = source;
            //source.ResetBindings(false);





            //priorityList.Add(new Priority("p1", 5, 0,5));
            //priorityList.Add(new Priority("p2", 6, 2,6));
            //priorityList.Add(new Priority("p3", 1, 10,1));

            //priorityList.Add(new Priority("p4", 7, 1,7));
            //priorityList.Add(new Priority("p5", 3, 1,3));

            
            //source.ResetBindings(false);
            //source.DataSource = priorityList;
            //inputGrid.DataSource = source;
        }

        private void typeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeCombo.SelectedIndex == 0 || typeCombo.SelectedIndex == 1)
            {
                arrivalList.Clear();
                source.ResetBindings(false);
                source.DataSource = arrivalList;
                inputGrid.DataSource = source;

            }

            if (typeCombo.SelectedIndex == 2)
            {
                priorityList.Clear();
                source.ResetBindings(false);
                source.DataSource = priorityList;
                inputGrid.DataSource = source;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (typeCombo.SelectedIndex == 0 || typeCombo.SelectedIndex == 1)
            {
                arrivalList.Add(new Arrival("", 0, 0));
                source.DataSource = arrivalList;
                source.ResetBindings(false);

            }

            if (typeCombo.SelectedIndex == 2)
            {
                priorityList.Add(new Priority("", 0, 0, 0));
                source.DataSource = priorityList;
                source.ResetBindings(false);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            priorityList.Clear();
            arrivalList.Clear();
            button1_Click(null, null);
            source.ResetBindings(false);
        }

        private class ProcessBlock
        {
            private string processName;
            private int startRange;
            private int endRange;
            private int brustTime;
            private int waitingTime;

            public string ProcessName { get => processName; set => processName = value; }
            public int StartRange { get => startRange; set => startRange = value; }
            public int EndRange { get => endRange; set => endRange = value; }
            public int BrustTime { get => brustTime; set => brustTime = value; }
            public int WaitingTime { get => waitingTime; set => waitingTime = value; }

            public ProcessBlock(string processName, int startRange, int endRange, int brustTime, int waitingTime)
            {
                this.processName = processName;
                this.startRange = startRange;
                this.endRange = endRange;
                this.brustTime = brustTime;
                this.waitingTime = waitingTime;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //pre run checks
            foreach (Arrival a in arrivalList)
            {
                if (a.BrustTime == 0)
                {
                    MessageBox.Show("Brust time can't be 0");
                    return;
                }
            }
            foreach (Priority p in priorityList)
            {
                if (p.BrustTime == 0)
                {
                    MessageBox.Show("Brust time can't be 0");
                    return;
                }
            }
            //prerun checks end

            arrivalList = arrivalList.OrderBy(a => a.ArivalTime).ToList();
            priorityList = priorityList.OrderBy(p => p.PriorityValue).ToList();
            List<ProcessBlock> result = new List<ProcessBlock>();
            if (typeCombo.SelectedIndex == 0)
            {
                //first come first serve
                int sr = 0; int er = 0; string p = "idle";
                int i = 0; int cp = 0;
                while (cp < arrivalList.Count)
                {
                    Arrival currentProcess = arrivalList[cp];

                    if (p.Equals("idle"))
                    {
                        //no waiting for process. Just switch if required
                        if (currentProcess.ArivalTime == i)
                        {
                            //if process have arrived
                            //change state

                            if (!(sr == 0 && er == 0))
                            {
                                //log the idle here as well
                                result.Add(new ProcessBlock("idle", sr, er, er - sr, 0));
                            }

                            p = currentProcess.ProcessName;
                            currentProcess.ival = i;
                            sr = i;
                            er = i;


                        }

                    }
                    else
                    {
                        //an process is already in processor
                        //check if its done
                        int expectedEndRange = currentProcess.ival + currentProcess.BrustTime;
                        if (i == expectedEndRange)
                        {
                            //process has ended


                            result.Add(new ProcessBlock(currentProcess.ProcessName, sr,
                                er, currentProcess.BrustTime, currentProcess.ival - currentProcess.ArivalTime));

                            cp++;
                            if (cp == arrivalList.Count) break;
                            currentProcess = arrivalList[cp];
                            if (currentProcess.ArivalTime <= i)
                            {
                                p = currentProcess.ProcessName;
                                currentProcess.ival = i;
                                sr = i;
                                er = i;
                            }
                            else
                            {
                                p = "idle";
                                sr = i;
                                er = i;
                            }


                        }
                    }


                    er++;
                    i++;
                }

                resultGrid.DataSource = result;
                //calculate average waiting time
                float totalWaitTime = 0;
                foreach (ProcessBlock r in result)
                {
                    if (!r.ProcessName.Equals("idle"))
                    {
                        totalWaitTime += r.WaitingTime;
                    }

                }
                averageWaitTimeLabel.Text = (totalWaitTime / result.Count) + " ms";
            }

            if (typeCombo.SelectedIndex == 1)
            {
                //order based on arival time and then brust time
                arrivalList = arrivalList.OrderBy(a => a.ArivalTime).ThenBy(a => a.BrustTime).ToList();
                //shortest job first
                int sr = 0; int er = 0; string p = "idle";
                int i = 0; int cp = 0;
                while (cp < arrivalList.Count)
                {
                    Arrival currentProcess = arrivalList[cp];

                    if (p.Equals("idle"))
                    {
                        //no waiting for process. Just switch if required
                        if (currentProcess.ArivalTime == i)
                        {
                            //if process have arrived
                            //change state

                            if (!(sr == 0 && er == 0))
                            {
                                //log the idle here as well
                                result.Add(new ProcessBlock("idle", sr, er, er - sr, 0));
                            }




                            p = currentProcess.ProcessName;
                            currentProcess.ival = i;
                            sr = i;
                            er = i;


                        }

                    }
                    else
                    {
                        //an process is already in processor
                        //check if its done
                        int expectedEndRange = currentProcess.ival + currentProcess.BrustTime;
                        if (i == expectedEndRange)
                        {
                            //process has ended

                            cp++;
                            result.Add(new ProcessBlock(currentProcess.ProcessName, sr,
                                er, currentProcess.BrustTime, currentProcess.ival - currentProcess.ArivalTime));

                            if (cp == arrivalList.Count) break;

                            //resort again according to latest tick
                            int sIndex = cp; int sVal = arrivalList[cp].BrustTime;
                            for (int j = cp; j < arrivalList.Count; j++)
                            {
                                if (arrivalList[j].ArivalTime <= i)
                                {
                                    if (arrivalList[j].BrustTime < sVal)
                                    {

                                        sVal = arrivalList[j].BrustTime;
                                        sIndex = j;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (sIndex != cp)
                            {
                                Swap<Arrival>(arrivalList, cp, sIndex);
                            }





                            currentProcess = arrivalList[cp];
                            if (currentProcess.ArivalTime <= i)
                            {
                                p = currentProcess.ProcessName;
                                currentProcess.ival = i;
                                sr = i;
                                er = i;
                            }
                            else
                            {
                                p = "idle";
                                sr = i;
                                er = i;
                            }


                        }
                    }


                    er++;
                    i++;
                }

                resultGrid.DataSource = result;
                //calculate average waiting time
                float totalWaitTime = 0;
                foreach (ProcessBlock r in result)
                {
                    if (!r.ProcessName.Equals("idle"))
                    {
                        totalWaitTime += r.WaitingTime;
                    }

                }
                averageWaitTimeLabel.Text = (totalWaitTime / result.Count) + " ms";

            }

            if (typeCombo.SelectedIndex == 2)
            {
                //priority list

                //order based on arival time and then priority 
                priorityList = priorityList.OrderBy(pr => pr.ArrivalTime).ThenBy(pr => pr.PriorityValue).ToList();
                //shortest job first
                int sr = 0; int er = 0; string p = "idle";
                int i = 0; int cp = 0;
                while (cp < priorityList.Count)
                {
                    Priority currentProcess = priorityList[cp];

                    if (p.Equals("idle"))
                    {
                        //no waiting for process. Just switch if required
                        if (currentProcess.ArrivalTime == i)
                        {
                            //if process have arrived
                            //change state

                            if (!(sr == 0 && er == 0))
                            {
                                //log the idle here as well
                                result.Add(new ProcessBlock("idle", sr, er, er - sr, 0));
                            }




                            p = currentProcess.ProcessName;
                            currentProcess.ival = i;
                            sr = i;
                            er = i;


                        }

                    }
                    else
                    {
                        //an process is already in processor
                        //check if its done
                        int expectedEndRange = currentProcess.ival + currentProcess.BrustTime;
                        if (i == expectedEndRange)
                        {
                            //process has ended

                            cp++;
                            result.Add(new ProcessBlock(currentProcess.ProcessName, sr,
                                er, currentProcess.BrustTime, currentProcess.ival - currentProcess.ArrivalTime));

                            if (cp == priorityList.Count) break;

                            //resort again according to latest tick
                            int sIndex = cp; int sVal = priorityList[cp].PriorityValue;
                            for (int j = cp; j < priorityList.Count; j++)
                            {
                                if (priorityList[j].ArrivalTime <= i)
                                {
                                    if (priorityList[j].PriorityValue < sVal)
                                    {

                                        sVal = priorityList[j].PriorityValue;
                                        sIndex = j;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (sIndex != cp)
                            {
                                Swap<Priority>(priorityList, cp, sIndex);
                            }





                            currentProcess = priorityList[cp];
                            if (currentProcess.ArrivalTime <= i)
                            {
                                p = currentProcess.ProcessName;
                                currentProcess.ival = i;
                                sr = i;
                                er = i;
                            }
                            else
                            {
                                p = "idle";
                                sr = i;
                                er = i;
                            }


                        }
                    }


                    er++;
                    i++;
                }

                resultGrid.DataSource = result;
                //calculate average waiting time
                float totalWaitTime = 0;
                foreach (ProcessBlock r in result)
                {
                    if (!r.ProcessName.Equals("idle"))
                    {
                        totalWaitTime += r.WaitingTime;
                    }

                }
                averageWaitTimeLabel.Text = (totalWaitTime / result.Count) + " ms";

            }
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by : Hassan Nawaz\nByteremix.com");
        }
    }
}
