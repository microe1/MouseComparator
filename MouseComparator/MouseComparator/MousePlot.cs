﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OxyPlot;

namespace MouseTester
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using OxyPlot.WindowsForms;
    using OxyPlot.Annotations;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    
    public partial class MousePlot : Form
    {
        private MouseLog mlog;
        private int last_start;
        private int last_end;
        double x_min;
        double x_max;
        double y_min;
        double y_max;
        private string xlabel = "";
        private string ylabel = "";

        public MousePlot(MouseLog Mlog)
        {
            InitializeComponent();
            this.mlog = Mlog;
            this.last_start = 0;
            this.last_end = mlog.Events.Count - 1;
            initialize_plot();

            this.comboBoxPlotType.SelectedIndex = 0;
            this.comboBoxPlotType.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);

            this.numericUpDownStart.Minimum = 0;
            this.numericUpDownStart.Maximum = mlog.Events.Count - 1;
            this.numericUpDownStart.Value = last_start;
            this.numericUpDownStart.ValueChanged += new System.EventHandler(this.numericUpDownStart_ValueChanged);

            this.numericUpDownEnd.Minimum = 0;
            this.numericUpDownEnd.Maximum = mlog.Events.Count - 1;
            this.numericUpDownEnd.Value = last_end;
            this.numericUpDownEnd.ValueChanged += new System.EventHandler(this.numericUpDownEnd_ValueChanged);

            this.checkBoxStem.Checked = false;
            this.checkBoxStem.CheckedChanged += new System.EventHandler(this.checkBoxStem_CheckedChanged);

            this.checkBoxLines.Checked = false;
            this.checkBoxLines.CheckedChanged += new System.EventHandler(this.checkBoxLines_CheckedChanged);

            refresh_plot();
        }

        private void initialize_plot()
        {
            var pm = new PlotModel(mlog.Desc1.ToString() + " (Blue) vs. " + mlog.Desc2.ToString() + " (Red)")
            {
                PlotType = PlotType.Cartesian,
                Background = OxyColors.White,
                Subtitle = mlog.NormR.ToString("F4") + " " + mlog.NormT.ToString("F4")
            };
            plot1.Model = pm;
        }

        private void refresh_plot()
        {
            PlotModel pm = plot1.Model;
            pm.Series.Clear();
            pm.Axes.Clear();

            var scatterSeries1 = new ScatterSeries
            {
                BinSize = 8,
                MarkerFill = OxyColors.Blue,
                MarkerSize = 2.0,
                MarkerStroke = OxyColors.Blue,
                MarkerStrokeThickness = 1.0,
                MarkerType = MarkerType.Circle
            };

            var scatterSeries2 = new ScatterSeries
            {
                BinSize = 8,
                MarkerFill = OxyColors.Red,
                MarkerSize = 2.0,
                MarkerStroke = OxyColors.Red,
                MarkerStrokeThickness = 1.0,
                MarkerType = MarkerType.Circle
            };

            var lineSeries1 = new LineSeries
            {
                Color = OxyColors.Blue,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 1.0,
                Smooth = true
            };

            var lineSeries2 = new LineSeries
            {
                Color = OxyColors.Red,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 1.0,
                Smooth = true
            };            

            if (checkBoxLines.Checked)
            {
                lineSeries1.Smooth = false;
                lineSeries2.Smooth = false;
            }

            var stemSeries1 = new StemSeries
            {
                Color = OxyColors.Blue,
                StrokeThickness = 1.0,
            };

            var stemSeries2 = new StemSeries
            {
                Color = OxyColors.Red,
                StrokeThickness = 1.0
            };

            if (comboBoxPlotType.Text.Contains("xCount"))
            {
                plot_xcounts_vs_time(scatterSeries1, stemSeries1, lineSeries1, scatterSeries2, stemSeries2, lineSeries2);
                pm.Series.Add(scatterSeries1);
                pm.Series.Add(scatterSeries2);
                if (!checkBoxLines.Checked)
                {
                    plot_fit(scatterSeries1, lineSeries1);
                    plot_fit(scatterSeries2, lineSeries2);
                }
                pm.Series.Add(lineSeries1);
                pm.Series.Add(lineSeries2);
                if (checkBoxStem.Checked)
                {
                    pm.Series.Add(stemSeries1);
                    pm.Series.Add(stemSeries2);
                }
            }
            else if (comboBoxPlotType.Text.Contains("yCount"))            
            {
                plot_ycounts_vs_time(scatterSeries1, stemSeries1, lineSeries1, scatterSeries2, stemSeries2, lineSeries2);
                pm.Series.Add(scatterSeries1);
                pm.Series.Add(scatterSeries2);
                if (!checkBoxLines.Checked)
                {
                    plot_fit(scatterSeries1, lineSeries1);
                    plot_fit(scatterSeries2, lineSeries2);
                }
                pm.Series.Add(lineSeries1);
                pm.Series.Add(lineSeries2);
                if (checkBoxStem.Checked)
                {
                    pm.Series.Add(stemSeries1);
                    pm.Series.Add(stemSeries2);
                }

            }
            else if (comboBoxPlotType.Text.Contains("xSum"))
            {
                plot_xsum_vs_time(scatterSeries1, stemSeries1, lineSeries1, scatterSeries2, stemSeries2, lineSeries2);
                pm.Series.Add(scatterSeries1);
                pm.Series.Add(scatterSeries2);
                if (!checkBoxLines.Checked)
                {
                    plot_fit(scatterSeries1, lineSeries1);
                    plot_fit(scatterSeries2, lineSeries2);
                }
                pm.Series.Add(lineSeries1);
                pm.Series.Add(lineSeries2);
                if (checkBoxStem.Checked)
                {
                    pm.Series.Add(stemSeries1);
                    pm.Series.Add(stemSeries2);
                }
            }
            else if (comboBoxPlotType.Text.Contains("ySum"))
            {
                plot_ysum_vs_time(scatterSeries1, stemSeries1, lineSeries1, scatterSeries2, stemSeries2, lineSeries2);
                pm.Series.Add(scatterSeries1);
                pm.Series.Add(scatterSeries2);
                if (!checkBoxLines.Checked)
                {
                    plot_fit(scatterSeries1, lineSeries1);
                    plot_fit(scatterSeries2, lineSeries2);
                }
                pm.Series.Add(lineSeries1);
                pm.Series.Add(lineSeries2);
                if (checkBoxStem.Checked)
                {
                    pm.Series.Add(stemSeries1);
                    pm.Series.Add(stemSeries2);
                }
            }
            else if (comboBoxPlotType.Text.Contains("X vs. Y"))
            {
                plot_x_vs_y(scatterSeries1, lineSeries1, scatterSeries2, lineSeries2);
                pm.Series.Add(scatterSeries1);
                pm.Series.Add(scatterSeries2);
                if (checkBoxLines.Checked)
                {
                    pm.Series.Add(lineSeries1);
                    pm.Series.Add(lineSeries2);
                }
            }

            var linearAxis1 = new LinearAxis();
            linearAxis1.AbsoluteMinimum = x_min - (x_max - x_min + 1) / 20.0;
            linearAxis1.AbsoluteMaximum = x_max + (x_max - x_min + 1) / 20.0;
            linearAxis1.MajorGridlineColor = OxyColor.FromArgb(40, 0, 0, 139);
            linearAxis1.MajorGridlineStyle = LineStyle.Solid;
            linearAxis1.MinorGridlineColor = OxyColor.FromArgb(20, 0, 0, 139);
            linearAxis1.MinorGridlineStyle = LineStyle.Solid;
            linearAxis1.Position = AxisPosition.Bottom;
            linearAxis1.Title = xlabel;
            pm.Axes.Add(linearAxis1);

            var linearAxis2 = new LinearAxis();
            linearAxis2.AbsoluteMinimum = y_min - (y_max - y_min + 1) / 20.0;
            linearAxis2.AbsoluteMaximum = y_max + (y_max - y_min + 1) / 20.0;
            linearAxis2.MajorGridlineColor = OxyColor.FromArgb(40, 0, 0, 139);
            linearAxis2.MajorGridlineStyle = LineStyle.Solid;
            linearAxis2.MinorGridlineColor = OxyColor.FromArgb(20, 0, 0, 139);
            linearAxis2.MinorGridlineStyle = LineStyle.Solid;
            linearAxis2.Title = ylabel;
            pm.Axes.Add(linearAxis2);

            plot1.RefreshPlot(true);
        }

        private void reset_minmax()
        {
            x_min = double.MaxValue;
            x_max = double.MinValue;
            y_min = double.MaxValue;
            y_max = double.MinValue;
        }

        private void update_minmax(double x, double y)
        {
            if (x < x_min)
            {
                x_min = x;
            }
            if (x > x_max)
            {
                x_max = x;
            }
            if (y < y_min)
            {
                y_min = y;
            }
            if (y > y_max)
            {
                y_max = y;
            }
        }

        private void plot_xcounts_vs_time(ScatterSeries scatterSeries1, StemSeries stemSeries1, LineSeries lineSeries1,
                                          ScatterSeries scatterSeries2, StemSeries stemSeries2, LineSeries lineSeries2)
        {
            xlabel = "Time (ms)";
            ylabel = "xCounts";
            reset_minmax();
            for (int i = last_start; i <= last_end; i++)
            {
                double x = mlog.Events[i].ts;
                if (mlog.Events[i].handle == mlog.Handle1)
                {
                    double y = mlog.Events[i].lastx;
                    update_minmax(x, y);
                    scatterSeries1.Points.Add(new ScatterPoint(x, y));
                    lineSeries1.Points.Add(new DataPoint(x, y));
                    stemSeries1.Points.Add(new DataPoint(x, y));
                }
                else if (mlog.Events[i].handle == mlog.Handle2)
                {
                    double y = (mlog.Events[i].lastx * Math.Cos(mlog.NormT) - mlog.Events[i].lasty * Math.Sin(mlog.NormT)) * mlog.NormR;
                    update_minmax(x, y);
                    scatterSeries2.Points.Add(new ScatterPoint(x, y));
                    lineSeries2.Points.Add(new DataPoint(x, y));
                    stemSeries2.Points.Add(new DataPoint(x, y));
                }
            }
        }

        private void plot_ycounts_vs_time(ScatterSeries scatterSeries1, StemSeries stemSeries1, LineSeries lineSeries1,
                                          ScatterSeries scatterSeries2, StemSeries stemSeries2, LineSeries lineSeries2)
        {
            xlabel = "Time (ms)";
            ylabel = "xCounts";
            reset_minmax();
            for (int i = last_start; i <= last_end; i++)
            {
                double x = mlog.Events[i].ts;
                if (mlog.Events[i].handle == mlog.Handle1)
                {
                    double y = mlog.Events[i].lasty;
                    update_minmax(x, y);
                    scatterSeries1.Points.Add(new ScatterPoint(x, y));
                    lineSeries1.Points.Add(new DataPoint(x, y));
                    stemSeries1.Points.Add(new DataPoint(x, y));
                }
                else if (mlog.Events[i].handle == mlog.Handle2)
                {
                    double y = (mlog.Events[i].lastx * Math.Sin(mlog.NormT) + mlog.Events[i].lasty * Math.Cos(mlog.NormT)) * mlog.NormR;
                    update_minmax(x, y);
                    scatterSeries2.Points.Add(new ScatterPoint(x, y));
                    lineSeries2.Points.Add(new DataPoint(x, y));
                    stemSeries2.Points.Add(new DataPoint(x, y));
                }
            }
        }

        private void plot_xsum_vs_time(ScatterSeries scatterSeries1, StemSeries stemSeries1, LineSeries lineSeries1,
                                       ScatterSeries scatterSeries2, StemSeries stemSeries2, LineSeries lineSeries2)
        {
            xlabel = "Time (ms)";
            ylabel = "xSum";
            reset_minmax();
            double y1 = 0.0;
            double y2 = 0.0;
            for (int i = last_start; i <= last_end; i++)
            {
                double x = mlog.Events[i].ts;
                if (mlog.Events[i].handle == mlog.Handle1)
                {
                    y1 += mlog.Events[i].lastx;
                    update_minmax(x, y1);
                    scatterSeries1.Points.Add(new ScatterPoint(x, y1));
                    lineSeries1.Points.Add(new DataPoint(x, y1));
                    stemSeries1.Points.Add(new DataPoint(x, y1));
                }
                else if (mlog.Events[i].handle == mlog.Handle2)
                {
                    y2 += (mlog.Events[i].lastx * Math.Cos(mlog.NormT) - mlog.Events[i].lasty * Math.Sin(mlog.NormT)) * mlog.NormR;
                    update_minmax(x, y2);
                    scatterSeries2.Points.Add(new ScatterPoint(x, y2));
                    lineSeries2.Points.Add(new DataPoint(x, y2));
                    stemSeries2.Points.Add(new DataPoint(x, y2));
                }
            }
        }

        private void plot_ysum_vs_time(ScatterSeries scatterSeries1, StemSeries stemSeries1, LineSeries lineSeries1,
                                       ScatterSeries scatterSeries2, StemSeries stemSeries2, LineSeries lineSeries2)
        {
            xlabel = "Time (ms)";
            ylabel = "ySum";
            reset_minmax();
            double y1 = 0.0;
            double y2 = 0.0;
            for (int i = last_start; i <= last_end; i++)
            {
                double x = mlog.Events[i].ts;
                if (mlog.Events[i].handle == mlog.Handle1)
                {
                    y1 += mlog.Events[i].lasty;
                    update_minmax(x, y1);
                    scatterSeries1.Points.Add(new ScatterPoint(x, y1));
                    lineSeries1.Points.Add(new DataPoint(x, y1));
                    stemSeries1.Points.Add(new DataPoint(x, y1));
                }
                else if (mlog.Events[i].handle == mlog.Handle2)
                {
                    y2 += (mlog.Events[i].lastx * Math.Sin(mlog.NormT) + mlog.Events[i].lasty * Math.Cos(mlog.NormT)) * mlog.NormR;
                    update_minmax(x, y2);
                    scatterSeries2.Points.Add(new ScatterPoint(x, y2));
                    lineSeries2.Points.Add(new DataPoint(x, y2));
                    stemSeries2.Points.Add(new DataPoint(x, y2));
                }
            }
        }

        private void plot_x_vs_y(ScatterSeries scatterSeries1, LineSeries lineSeries1, 
                                 ScatterSeries scatterSeries2, LineSeries lineSeries2)
        {
            xlabel = "xCounts";
            ylabel = "yCounts";
            reset_minmax();
            double x1 = 0.0;
            double y1 = 0.0;
            double x2 = 0.0;
            double y2 = 0.0;
            for (int i = last_start; i <= last_end; i++)
            {
                if (mlog.Events[i].handle == mlog.Handle1)
                {
                    x1 += mlog.Events[i].lastx;
                    y1 += mlog.Events[i].lasty;
                    update_minmax(x1, x1);
                    update_minmax(y1, y1);
                    scatterSeries1.Points.Add(new ScatterPoint(x1, y1));
                    lineSeries1.Points.Add(new DataPoint(x1, y1));
                }
                else if (mlog.Events[i].handle == mlog.Handle2)
                {
                    x2 += (mlog.Events[i].lastx * Math.Cos(mlog.NormT) - mlog.Events[i].lasty * Math.Sin(mlog.NormT)) * mlog.NormR;
                    y2 += (mlog.Events[i].lastx * Math.Sin(mlog.NormT) + mlog.Events[i].lasty * Math.Cos(mlog.NormT)) * mlog.NormR;
                    update_minmax(x2, x2);
                    update_minmax(y2, y2);
                    scatterSeries2.Points.Add(new ScatterPoint(x2, y2));
                    lineSeries2.Points.Add(new DataPoint(x2, y2));
                }
            }
        }

#if true
// Window based smoothing
        private void plot_fit(ScatterSeries scatterSeries1, LineSeries lineSeries1)
        {
            double sum = 0.0;

            lineSeries1.Points.Clear();

            for (int i = 0; ((i < 8) && (i < scatterSeries1.Points.Count)); i++)
            {
                sum = sum + scatterSeries1.Points[i].Y;
            }

            for (int i = 3; i < scatterSeries1.Points.Count - 5; i++)
            {
                double x = (scatterSeries1.Points[i].X + scatterSeries1.Points[i + 1].X) / 2.0;
                double y = sum;
                lineSeries1.Points.Add(new DataPoint(x, y / 8.0));
                sum = sum - scatterSeries1.Points[i - 3].Y;
                sum = sum + scatterSeries1.Points[i + 5].Y;
            }
        }
#else
// Time based smoothing
        private void plot_fit(ScatterSeries scatterSeries1, LineSeries lineSeries1)
        {
            double hz = 125;
            double ms = 1000.0 / hz;
            lineSeries1.Points.Clear();

            int ind = 0;
            for (double x = scatterSeries1.Points[0].X; x <= scatterSeries1.Points[scatterSeries1.Points.Count - 1].X; x += ms)
            {
                double sum = 0.0;
                while (scatterSeries1.Points[ind].X <= x)
                {
                    sum += scatterSeries1.Points[ind++].Y;
                }
                lineSeries1.Points.Add(new DataPoint(x - (ms / 2.0), sum / ms));
            }
        }
#endif

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            refresh_plot();
        }

        private void numericUpDownStart_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownStart.Value >= numericUpDownEnd.Value)
            {
                numericUpDownStart.Value = last_start;
            }
            else
            {
                last_start = (int)numericUpDownStart.Value;
                refresh_plot();
            }
        }

        private void numericUpDownEnd_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownEnd.Value <= numericUpDownStart.Value)
            {
                numericUpDownEnd.Value = last_end;
            }
            else
            {
                last_end = (int)numericUpDownEnd.Value;
                refresh_plot();
            }
        }

        private void checkBoxStem_CheckedChanged(object sender, EventArgs e)
        {
            refresh_plot();
        }

        private void checkBoxLines_CheckedChanged(object sender, EventArgs e)
        {
            refresh_plot();
        }

        private void buttonSavePNG_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG Files (*.png)|*.png";
            saveFileDialog1.FilterIndex = 1;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MousePlot.Export(this.plot1.Model, saveFileDialog1.FileName, 800, 600);
            }         
        }

        public static void Export(PlotModel model, string fileName, int width, int height, Brush background = null)
        {
            using (var bm = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bm))
                {
                    if (background != null)
                    {
                        g.FillRectangle(background, 0, 0, width, height);
                    }

                    var rc = new GraphicsRenderContext { RendersToScreen = false };
                    rc.SetGraphicsTarget(g);
                    model.Update();
                    model.Render(rc, width, height);
                    bm.Save(fileName, ImageFormat.Png);
                }
            }
        }
    }
}
