using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Drawing;
using ZedGraph;
using System.Web;
using System.Configuration; 
/* ---------------------------------------------------------------
 * Set the right destination path in the "save the work" section *
 * --------------------------------------------------------------*/

namespace HOSgraph
{
    /// <summary>
    /// HosDutyState for Hours of Service Calculations
    /// </summary>       
    [Flags]
    public enum HosDutyState
    {
        Unknown = 0x00,
        OffDuty = 0x01,
        Sleep = 0x02,
        Driving = 0x04,
        OnDuty = 0x08,
    }

    public class HoSGraph
    {
        private int[] v1;
        private double[] v2;
        private double[] totals = new double[4];

        /// <summary>
        /// Routine to generate the HOS graph in English. May be overloaded for other data interfaces
        /// </summary>
        /// <param name="dt">The DataTable containing data to be plotted</param>
        /// <param name="xname">Column name in <b>dt</b> with values for x axis</param>
        /// <param name="yname">Column name in <b>dt</b> with values for y axis</param>
        /// <param name="imex">The image extension</param>
        /// <returns>bitmap filename, or an error message starting with "Error"</returns>
        public String drawHOS(DataTable dt, String xname, String yname, String imex)
        {
            return drawHOS(dt, xname, yname, imex, "en");
        }

        /// <summary>
        /// Routine to generate the HOS graph. May be overloaded for other data interfaces
        /// </summary>
        /// <param name="dt">The DataTable containing data to be plotted</param>
        /// <param name="xname">Column name in <b>dt</b> with values for x axis</param>
        /// <param name="yname">Column name in <b>dt</b> with values for y axis</param>
        /// <param name="imex">The image extension</param>
        /// <param name="lang">Localization parameter</param>
        /// <returns>bitmap filename, or an error message starting with "Error"</returns>
        public String drawHOS(DataTable dt, String xname, String yname, String imex, String lang)
        {
            if (!dt.Columns.Contains(xname))
                return String.Format("Error: missing {0} in DataTable", xname);
            if (!dt.Columns.Contains(yname))
                return String.Format("Error: missing {0} in DataTable", yname);
            int c = dt.Rows.Count;
            if (c < 2)
                return "Error: Nothing to plot";

            DataTableReader dtr = dt.CreateDataReader();
            this.v1 = new int[c];
            this.v2 = new double[c];
            DateTime x0 = new DateTime(2008, 1, 1, 0, 0, 0);
            DateTime x = new DateTime();
            int y;
            short i = 0;
            while (dtr.Read())
            {
                try
                {
                    x = (DateTime)dtr[xname];
                }
                catch (Exception e)
                {
                    String s = (String)dtr[xname];
                    x = Convert.ToDateTime(s);
                    if (s.Length > 19)
                    {
                        String[] split;
                        if (s.IndexOf('Z') > -1)
                            split = s.Substring(20).Split(':');
                        else
                            split = s.Substring(19).Split(':');
                        x = x.AddHours(Convert.ToDouble(split[0]));
                        x = x.AddMinutes(Convert.ToDouble(split[1])); // thanks MichaelK
                    }
                }
                try
                {
                    y = (int)dtr[yname];
                }
                catch (Exception e)
                {
                    y = Convert.ToInt32((String)dtr[yname]);
                }
                this.v2[i] = (x.TimeOfDay - x0.TimeOfDay).TotalHours;
                x0 = x;
                switch (y)
                {
                    case 1: //HosDutyState.OffDuty:
                        this.v1[i] = 4;
                        break;
                    case 2: //HosDutyState.Sleep:
                        this.v1[i] = 3;
                        break;
                    case 4: //HosDutyState.Driving:
                        this.v1[i] = 2;
                        break;
                    case 8: //HosDutyState.OnDuty:
                        this.v1[i] = 1;
                        break;
                    default: //Unknown:
                        this.v1[i] = 0;
                        continue;
                }
                ++i;
            }
            return draw(imex, lang);
        }

        /// <summary>
        /// Routine to generate the HOS graph in English. May be overloaded for other data interfaces
        /// </summary>
        /// <param name="x">Moments in time when the status changes</param>
        /// <param name="y">The new status</param>
        /// <param name="imex">The image extension</param>
        /// <returns>bitmap filename, or an error message starting with "Error"</returns>
        public String drawHOS(DateTime[] x, HosDutyState[] y, String imex)
        {
            return drawHOS(x, y, imex, "en");
        }

        /// <summary>
        /// Routine to generate the HOS graph. May be overloaded for other data interfaces
        /// </summary>
        /// <param name="x">Moments in time when the status changes</param>
        /// <param name="y">The new status</param>
        /// <param name="imex">The image extension</param>
        /// <param name="lang">Localization parameter</param>
        /// <returns>bitmap filename, or an error message starting with "Error"</returns>
        public String drawHOS(DateTime[] x, HosDutyState[] y, String imex, String lang)
        {
            if (x.GetLength(0) != y.GetLength(0))
                return "Error: Non-matching x and y sizes";
            if (x.GetLength(0) < 2)
                return "Error: Nothing to plot";

            this.v1 = new int[y.GetLength(0)];
            this.v2 = new double[x.GetLength(0)];

            // prepare graphic elements: points to plot, totals to show
            for (int i = 0; i < x.GetLength(0); ++i)
            {
                if (i > 0)
                    this.v2[i] = (x[i].TimeOfDay - x[i - 1].TimeOfDay).TotalHours;
                else
                    this.v2[i] = x[i].TimeOfDay.TotalHours;
                switch (y[i])
                {
                    case HosDutyState.OffDuty:
                        this.v1[i] = 4;
                        break;
                    case HosDutyState.Sleep:
                        this.v1[i] = 3;
                        break;
                    case HosDutyState.Driving:
                        this.v1[i] = 2;
                        break;
                    case HosDutyState.OnDuty:
                        this.v1[i] = 1;
                        break;
                    case HosDutyState.Unknown:
                        this.v1[i] = 0;
                        continue;
                }
            }
            return draw(imex, lang);
        }

        /// <summary>
        /// The actual graphic renderer
        /// </summary>
        /// <param name="v1">Values for y axis</param>
        /// <param name="v2">Values for x axis</param>
        /// <param name="totals">Totals</param>
        /// <param name="imex">Image extension</param>
        /// <param name="lang">Localization parameter</param>
        /// <returns>File name with the plot</returns>
        private String draw(String imex, String lang)
        {
            String[] msg;
            if (lang == "fr")
                msg = new String[6] { "Mid-\nnuit", "Midi", "4:En service", "3:Conduire", "2:Se reposer", "1:Hors service" };
            else if (lang == "es")
                msg = new String[6] { "Media-\nnoche", "Medio\nd\u00eda", "4:De servicio", "3:Conducci\u00f3n", "2:Durmiente", "1:Fuera de servicio" };
            else // defaults to English
                msg = new String[6] { "MID-\nNight", "Noon", "4:On Duty", "3:Driving", "2:Sleeper", "1:Off Duty" };
            double xx = 0;
            int i = 0;
            // Build the GraphPane
            GraphPane myPane = new GraphPane((RectangleF)new RectangleF(0, 0, 1000, 375), "", "", "");
            myPane.Legend.IsVisible = false;
            myPane.Border.Color = Color.White;
            myPane.Chart.Border.Color = Color.Silver;

            // Prepare axis and grids
            myPane.XAxis.Scale.MajorStep = 1;
            myPane.XAxis.Scale.MinorStep = .25;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 24;
            myPane.XAxis.Scale.Align = AlignP.Inside;
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.DashOff = 0;
            myPane.XAxis.MajorGrid.Color = Color.Silver;
            myPane.XAxis.MajorTic.Color = Color.Silver;
            myPane.XAxis.MajorTic.IsOpposite = false;
            myPane.XAxis.MinorGrid.Color = Color.Silver;
            myPane.XAxis.MinorTic.IsOutside = false;
            myPane.XAxis.MinorTic.IsInside = false;
            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MinorTic.Color = Color.Silver;

            String[] x2lbls = {msg[0],"1","2","3","4","5","6","7","8","9","10","11",
                               msg[1],"1","2","3","4","5","6","7","8","9","10","11","12","Total"};
            myPane.X2Axis.IsVisible = true;
            myPane.X2Axis.Type = AxisType.Text;
            myPane.X2Axis.Scale.TextLabels = x2lbls;
            myPane.X2Axis.Scale.Min = 1;
            myPane.X2Axis.Scale.Max = 25;
            myPane.X2Axis.Scale.MajorStep = 1;
            myPane.X2Axis.Scale.IsPreventLabelOverlap = false;
            myPane.X2Axis.Scale.Align = AlignP.Inside;
            myPane.X2Axis.MajorTic.Color = Color.Silver;

            String[] ylbls = { msg[2], msg[3], msg[4], msg[5] };
            myPane.YAxis.Type = AxisType.Text;
            myPane.YAxis.Scale.TextLabels = ylbls;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.DashOff = 0;
            myPane.YAxis.MajorGrid.Color = Color.Silver;
            myPane.YAxis.MajorTic.IsBetweenLabels = true;
            myPane.YAxis.MajorTic.IsOutside = false;
            myPane.YAxis.MajorTic.IsInside = false;
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MajorTic.Color = Color.Silver;

            // prepare data for curve - in this point, to be able to sort before Y2
            PointPairList pl = new PointPairList();
            for (i = 0; i < this.v2.GetLength(0); i++)
            {
                xx += this.v2[i];
                pl.Add(xx, this.v1[i]);
            }
            pl.Sort();

            String[] y2lbls = new String[4];
            for (i = 1; i < this.v1.GetLength(0); ++i)
                this.totals[(int)pl[i - 1].Y - 1] += pl[i].X - pl[i - 1].X;
            for (i = 0; i < 3; ++i)
                y2lbls[i] = String.Format("\n\n\n{0}", System.Math.Round(this.totals[i], 2));

            
                y2lbls[3] = String.Format("Total\n\n\n{0}", System.Math.Round(this.totals[3], 2));

            myPane.Y2Axis.IsVisible = true;
            myPane.Y2Axis.Type = AxisType.Text;
            myPane.Y2Axis.Scale.TextLabels = y2lbls;
            myPane.Y2Axis.Scale.Align = AlignP.Inside;
            myPane.Y2Axis.MajorTic.IsOutside = false;
            myPane.Y2Axis.MajorTic.IsInside = false;
            myPane.Y2Axis.MajorTic.IsOpposite = false;
            myPane.Y2Axis.MajorTic.Color = Color.Silver;

            // Add the curve
            LineItem curve = myPane.AddCurve("", pl, Color.Black, SymbolType.None);
            curve.Line.StepType = StepType.ForwardStep;
            curve.IsOverrideOrdinal = true;

            // custom gridlines
            sgLine[][] smallGrid = new sgLine[8][];
            for (i = 0; i < 4; i++)
            {
                smallGrid[i] = new sgLine[24];
                for (int j = 0; j < 24; ++j)
                    smallGrid[i][j] = new sgLine();
            }
            for (i = 4; i < 8; i++)
            {
                smallGrid[i] = new sgLine[48];
                for (int j = 0; j < 48; ++j)
                    smallGrid[i][j] = new sgLine();
            }
            for (xx = .25, i = 0; xx < 24; xx += .5, i++)
            {
                smallGrid[4][i].draw(xx, 1.5, xx, 1.75, myPane);
                smallGrid[5][i].draw(xx, 3.25, xx, 3.5, myPane);
                smallGrid[6][i].draw(xx, .5, xx, .75, myPane);
                smallGrid[7][i].draw(xx, 4.25, xx, 4.5, myPane);
            }
            for (xx = .5, i = 0; xx < 24; xx++, i++)
            {
                smallGrid[0][i].draw(xx, 0, xx, 1, myPane);
                smallGrid[1][i].draw(xx, 1.5, xx, 2, myPane);
                smallGrid[2][i].draw(xx, 3, xx, 3.5, myPane);
                smallGrid[3][i].draw(xx, 4, xx, 4.5, myPane);
            }

            // save the work
            myPane.AxisChange();
            string strPath = HttpContext.Current.Server.MapPath("~/TempReports/");
            string strURL = ""; //ConfigurationSettings.AppSettings["MapExternalPath"] + "Maps/";
            String result = String.Format("HOS{0}.{1}",  Guid.NewGuid().ToString().Substring(9), imex);
            if (imex == "png")
                myPane.GetImage().Save(strPath+result, System.Drawing.Imaging.ImageFormat.Png);
            else if (imex == "gif")
                myPane.GetImage().Save(strPath+result, System.Drawing.Imaging.ImageFormat.Gif);
            else if (imex == "jpg")
                myPane.GetImage().Save(strPath+result, System.Drawing.Imaging.ImageFormat.Jpeg);

            strURL = "../TempReports/"; 
            return strURL+result;
        }
    }

    class sgLine
    {
        double[] x;
        double[] y;
        LineItem li;

        public void draw(double x1, double y1, double x2, double y2, GraphPane myPane)
        {
            this.x = new double[2];
            this.x[0] = x1;
            this.x[1] = x2;
            this.y = new double[2];
            this.y[0] = y1;
            this.y[1] = y2;
            this.li = myPane.AddCurve("", this.x, this.y, Color.Silver, SymbolType.None);
            this.li.IsOverrideOrdinal = true;
        }
    }
}