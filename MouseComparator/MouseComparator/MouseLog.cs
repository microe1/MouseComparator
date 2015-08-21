using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MouseTester
{
    public class MouseLog
    {
        public const int INVALID_HANDLE = -1;
        private string desc1 = "Mouse 1";
        private string desc2 = "Mouse 2";
        private double normR = 1.0000;
        private double normT = 0.0000;
        private int handle1 = INVALID_HANDLE;
        private int handle2 = INVALID_HANDLE;
        private List<MouseEvent> events = new List<MouseEvent>(1000000);
        
        public double NormR
        { 
            get 
            { 
                return this.normR; 
            } 
            set
            { 
                this.normR = value; 
            }
        }

        public double NormT
        {
            get
            {
                return this.normT;
            }
            set
            {
                this.normT = value;
            }
        }

        public int Handle1
        {
            get
            {
                return this.handle1;
            }
            set
            {
                this.handle1 = value;
            }
        }

        public int Handle2
        {
            get
            {
                return this.handle2;
            }
            set
            {
                this.handle2 = value;
            }
        }

        public string Desc1
        {
            get
            {
                return this.desc1;
            }
            set
            {
                this.desc1 = value;
            }
        }

        public string Desc2
        {
            get
            {
                return this.desc2;
            }
            set
            {
                this.desc2 = value;
            }
        }

        public List<MouseEvent> Events
        {
            get
            {
                return this.events;
            }
        }

        public void Add(MouseEvent e)
        {
            this.events.Add(e);
        }

        public void Clear()
        {
            this.events.Clear();
        }

        public void Load(string fname)
        {
            this.Clear();

            try
            {
                using (StreamReader sr = File.OpenText(fname))
                {
                    this.desc1 = sr.ReadLine();
                    this.handle1 = int.Parse(sr.ReadLine());
                    this.desc2 = sr.ReadLine();
                    this.handle2 = int.Parse(sr.ReadLine());
                    this.normR = double.Parse(sr.ReadLine());
                    this.normT = double.Parse(sr.ReadLine());
                    string headerline = sr.ReadLine();
                    while (sr.Peek() > -1)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split(',');
                        this.Add(new MouseEvent(int.Parse(values[0]), 0, int.Parse(values[1]), int.Parse(values[2]), double.Parse(values[3])));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void Save(string fname)
        {
            try
            {
                using (StreamWriter sw = File.CreateText(fname))
                {
                    sw.WriteLine(this.desc1);
                    sw.WriteLine(this.handle1.ToString());
                    sw.WriteLine(this.desc2);
                    sw.WriteLine(this.handle2.ToString());
                    sw.WriteLine(this.normR.ToString());
                    sw.WriteLine(this.normT.ToString());
                    sw.WriteLine("handle,xCount,yCount,Time (ms)");
                    foreach (MouseEvent e in this.events)
                    {
                        sw.WriteLine(e.handle.ToString() + "," + e.lastx.ToString() + "," + e.lasty.ToString() + "," + e.ts.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public Boolean Valid_handles()
        {
            if (this.handle1 == INVALID_HANDLE || this.handle2 == INVALID_HANDLE)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Normalize()
        {
            double x1 = 0.0;
            double y1 = 0.0;
            double x2 = 0.0;
            double y2 = 0.0;
            int count1 = 0;
            int count2 = 0;
            foreach (MouseEvent me in this.events)
            {
                if (me.handle == this.handle1)
                {
                    x1 += (double)me.lastx;
                    y1 += (double)me.lasty;
                    count1++;
                }
                if (me.handle == this.handle2)
                {
                    x2 += (double)me.lastx;
                    y2 += (double)me.lasty;
                    count2++;
                }
            }

            if (count1 == 0)
            {
                return;
            }
            if (count2 == 0)
            {
                return;
            }

            double r1 = Math.Sqrt(x1 * x1 + y1 * y1);
            double r2 = Math.Sqrt(x2 * x2 + y2 * y2);
            double t1 = Math.Atan2(y1, x1);
            double t2 = Math.Atan2(y2, x2);
            this.normR = r1 / r2;
            this.normT = t1 - t2;
        }
    }
}
