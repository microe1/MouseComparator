using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using OxyPlot;

namespace MouseTester
{
    using OxyPlot.Series;

    public partial class Form1 : Form
    {
        private RawMouse mouse = new RawMouse();
        private MouseLog mlog = new MouseLog();
        enum state { idle, collect };
        private state test_state = state.idle;
        private int lastHandle = MouseLog.INVALID_HANDLE;

        public Form1()
        {
            InitializeComponent();
            try
            {
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2); // Use only the second core 
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime; // Set highest process priority
                Thread.CurrentThread.Priority = ThreadPriority.Highest; // Set highest thread priority
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            this.mouse.RegisterRawInputMouse(Handle);
            this.mouse.mevent += new RawMouse.MouseEventHandler(this.logMouseEvent);
            this.textBox1.Text = "Click on the Mouse1 button with the first mouse\r\n\r\n" + 
                                 "Click on the Mouse2 button with the second mouse\r\n\r\n" +
                                 "Collect, Stop, and then Plot mouse data\r\n\r\n" +
                                 "Normalization is an optional compensation\r\n" + 
                                 "for cpi and rotation differences\r\n" + 
                                 "based on start and end point";
            this.toolStripStatusLabel1.Text = "";
            this.textBoxNormR.Text = this.mlog.NormR.ToString("F4");
            this.textBoxNormT.Text = this.mlog.NormT.ToString("F4");
            this.textBoxDesc1.Text = mlog.Desc1;
            this.textBoxDesc2.Text = mlog.Desc2;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                buttonCollect.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2)
            {
                buttonStop.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                buttonPlot.PerformClick();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            this.mouse.ProcessRawInput(m);
            base.WndProc(ref m);
        }

        public void logMouseEvent(object RawMouse, MouseEvent mevent)
        {
            //Debug.WriteLine(mevent.handle + ", " + mevent.ts + ", " + mevent.lastx + ", " + mevent.lasty + ", " + mevent.buttons);
            if (this.test_state == state.idle)
            {
                this.lastHandle = mevent.handle;
            }
            else if (this.test_state == state.collect)
            {
                this.mlog.Add(mevent);
            }
        }

        private void buttonMouse1_Click(object sender, EventArgs e)
        {
            mlog.Handle1 = this.lastHandle;
            textBoxDesc1.Text = this.lastHandle.ToString();
        }

        private void buttonMouse2_Click(object sender, EventArgs e)
        {
            mlog.Handle2 = this.lastHandle;
            textBoxDesc2.Text = this.lastHandle.ToString();
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            if (mlog.Valid_handles() == false)
            {
                return;
            }

            mlog.Normalize();
            this.textBoxNormR.Text = mlog.NormR.ToString("F4");
            this.textBoxNormT.Text = mlog.NormT.ToString("F4");
        }
        
        private void buttonCollect_Click(object sender, EventArgs e)
        {
            if (this.test_state == state.idle)
            {
                this.mlog.Clear();
                this.mouse.StopWatchReset();
                this.test_state = state.collect;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (this.test_state == state.collect)
            {
                if (this.mlog.Events.Count > 0)
                {
                    double ts_min = this.mlog.Events[0].ts;
                    foreach (MouseEvent me in this.mlog.Events)
                    {
                        me.ts -= ts_min;
                    }
                }
                this.test_state = state.idle;
            }
        }

        private void buttonPlot_Click(object sender, EventArgs e)
        {
            // TODO check handles
            if (this.mlog.Events.Count > 0)
            {
                this.mlog.Desc1 = textBoxDesc1.Text;
                this.mlog.Desc2 = textBoxDesc2.Text;
                MousePlot mousePlot = new MousePlot(mlog);
                mousePlot.Show();
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "CSV Files (*.csv)|*.csv|All Files(*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.mlog.Load(openFileDialog1.FileName);
            }
            this.textBoxDesc1.Text = mlog.Desc1;
            this.textBoxDesc2.Text = mlog.Desc2;
            this.textBoxNormR.Text = mlog.NormR.ToString("F4");
            this.textBoxNormT.Text = mlog.NormT.ToString("F4");
            this.textBox1.Text = "Events: " + mlog.Events.Count.ToString();
            if (this.mlog.Events.Count > 0)
            {
                MousePlot mousePlot = new MousePlot(mlog);
                mousePlot.Show();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV Files (*.csv)|*.csv|All Files(*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.mlog.Save(saveFileDialog1.FileName);
            }
        }

        private void textBoxNormR_Validated(object sender, EventArgs e)
        {
            this.mlog.NormR = Convert.ToDouble(this.textBoxNormR.Text);
        }

        private void textBoxNormT_Validated(object sender, EventArgs e)
        {
            this.mlog.NormT = Convert.ToDouble(this.textBoxNormT.Text);
        }

    }
}
