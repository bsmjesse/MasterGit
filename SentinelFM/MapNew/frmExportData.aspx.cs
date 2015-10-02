using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Script.Serialization;
using ClosedXML.Excel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;

namespace SentinelFM
{
    public partial class MapNew_frmExportData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string exportdata = Server.UrlDecode(Request["exportdata"]);
            //exportdata = exportdata.Replace("\\\"", "\\\"\\\"");
            //exportdata = "{\"Header\":[\"UnitID\",\"Description\",\"Status\",\"Speed\",\"Date/Time\",\"Address\",\"Armed\",\"History\"],\"Data\":[[\"991199\",\"Ravi_Test_Box_991199\",\"NA\",\"24\",\"04/07/2013 04:57 pm\",\"Raymond \\\"\\\"Landmark\\\"\\\"04\",\"false\",\"History\"],[\"997799\",\"Ravi_SFM7000-997799\",\"PowerSave\",\"0\",\"04/07/2013 04:56 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"37432\",\"Costa SFM3000 Laval\",\"NA\",\"0\",\"04/07/2013 04:52 pm\",\"454 Boulevard Armand-Frappier, Laval-Des-Rapides, QC, H7V 4B4\",\"false\",\"History\"],[\"939393\",\"939393-QingQing\",\"NA*\",\"0\",\"04/07/2013 04:47 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"9281\",\"Devon Test\",\"NA*\",\"0\",\"04/07/2013 03:55 pm\",\"Hwy-403, Mississauga, ON, L5L\",\"false\",\"History\"],[\"30610\",\"Box 30610-Devin\",\"NA\",\"0\",\"04/07/2013 11:55 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"10702\",\"Joanna\",\"NA*\",\"0\",\"04/07/2013 10:02 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"16267\",\"Brendan\",\"NA\",\"0\",\"04/07/2013 09:42 am\",\"Brendan\",\"false\",\"History\"],[\"949494\",\"3000BT 949494\",\"PowerSave\",\"0\",\"04/07/2013 09:11 am\",\"BSM Head Office\",\"false\",\"History\"],[\"999877\",\"Phil's Toyota\",\"NA\",\"0\",\"04/07/2013 09:07 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"10269\",\"Randy-2000\",\"NA\",\"0\",\"04/07/2013 09:02 am\",\"Raymond Landmark04\",\"true\",\"History\"],[\"999999\",\"Box     999999\",\"NA\",\"0\",\"03/07/2013 05:12 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"15013\",\"Ben's test unit\",\"NA*\",\"0\",\"03/07/2013 08:27 am\",\"1458 Boulevard De L'Avenir, Laval-Des-Rapides, QC, H7N\",\"false\",\"History\"],[\"995599\",\"Ravi_995599_QA_Garmin_Test_Box\",\"PowerSave\",\"0\",\"02/07/2013 04:38 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"9268\",\"peter\",\"NA\",\"0\",\"02/07/2013 02:38 pm\",\"20 Hawkridge Trl, Brampton, ON, L6P 2T4\",\"false\",\"History\"],[\"24765\",\"Chris P - HOS\",\"NA*\",\"0\",\"02/07/2013 11:17 am\",\"85 Elm St, Toronto, ON, M5G 0A8\",\"false\",\"History\"],[\"200200\",\"Ravi_3000_Test_Unit\",\"NA*\",\"0\",\"26/06/2013 04:31 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"36836\",\"Trent - HOS\",\"NA*\",\"0\",\"26/06/2013 10:32 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"38351\",\"Box 38351\",\"NA*\",\"0\",\"24/06/2013 10:40 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"101197\",\"1516644\",\"NA\",\"0\",\"21/06/2013 10:56 pm\",\"470 Main St W [Hwy-2/Hwy-8], Hamilton, ON, L8P\",\"false\",\"History\"],[\"101198\",\"8509743\",\"NA\",\"0\",\"21/06/2013 04:50 pm\",\"235 Rue P\u00e9pin, Sherbrooke, QC, J1L\",\"false\",\"History\"],[\"101199\",\"2518903\",\"NA\",\"0\",\"21/06/2013 04:33 pm\",\"666 Hardy Ave, Ottawa, ON, K1K 2B1\",\"false\",\"History\"],[\"101200\",\"0513016\",\"NA\",\"0\",\"21/06/2013 04:16 pm\",\"99 Bradstock Rd, North York, ON, M9M\",\"false\",\"History\"],[\"37079\",\"Box 37079\",\"PowerSave\",\"0\",\"21/06/2013 02:57 pm\",\"2614 Ellwood Dr SW, Edmonton, AB, T6X 0A9\",\"false\",\"History\"],[\"101196\",\"1516564\",\"NA\",\"0\",\"21/06/2013 11:45 am\",\"test149\",\"false\",\"History\"],[\"37431\",\"Costa SFM7000 Laval\",\"NA\",\"0\",\"20/06/2013 04:58 pm\",\"464 Boulevard Armand-Frappier, Laval-Des-Rapides, QC, H7V 4B4\",\"false\",\"History\"],[\"37872\",\"Box 37872\",\"NA*\",\"0\",\"19/06/2013 06:42 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"24759\",\"Brendan S-HOS\",\"NA*\",\"0\",\"18/06/2013 08:51 am\",\"Brendan\",\"false\",\"History\"],[\"9994\",\"Ravi_Test BOX Sasktel_9994\",\"NA\",\"0\",\"14/06/2013 01:38 pm\",\"Raymond Landmark04\",\"true\",\"History\"],[\"21027\",\"testcolor3\",\"NA*\",\"0\",\"12/06/2013 04:00 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"24758\",\"Jason_HOS\",\"PowerSave\",\"0\",\"09/06/2013 07:50 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"30774\",\"Box 30774\",\"PowerSave\",\"0\",\"06/06/2013 06:55 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"36451\",\"3000 Sirf4\",\"NA*\",\"0\",\"02/06/2013 11:13 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"500999\",\"Calamp 500999\",\"PowerSave\",\"0\",\"30/05/2013 10:31 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"999555\",\"Ravi_Trailer_Test_Unit\",\"NA\",\"13\",\"29/05/2013 02:31 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"9277\",\"9277_KORE_Ravi\",\"NA*\",\"0\",\"29/05/2013 11:29 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"120831\",\"CC1\",\"NA\",\"0\",\"29/05/2013 08:41 am\",\"454 Boulevard Armand-Frappier, Laval-Des-Rapides, QC, H7V 4B4\",\"false\",\"History\"],[\"35498\",\"Box 35498\",\"NA\",\"0\",\"24/05/2013 05:13 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"9280\",\"Devin Test Box\",\"NA\",\"0\",\"21/05/2013 12:36 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"35499\",\"Box  35499\",\"NA\",\"0\",\"21/05/2013 10:58 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"10703\",\"Box 10703 Test\",\"NA\",\"0\",\"15/05/2013 10:22 am\",\"198 Centennial Rd, Orangeville, ON, L9W 5K2\",\"false\",\"History\"],[\"21153\",\"Gabriela's Test Box IRIDIUM\",\"NA*\",\"0\",\"14/05/2013 04:11 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"201201\",\"Ravi_3000_Garmin_Test_Unit\",\"PowerSave\",\"0\",\"23/04/2013 02:45 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"999919\",\"Box 999919\",\"NA\",\"0\",\"16/04/2013 10:43 am\",\"71 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"24762\",\"Eric G - HOS\",\"NA\",\"0\",\"04/04/2013 09:20 am\",\"BSM Head Office\",\"false\",\"History\"],[\"993399\",\"Devin's Test box 993399\",\"NA\",\"0\",\"28/03/2013 12:12 pm\",\"BSM Head Office\",\"false\",\"History\"],[\"929292\",\"Ravi_929292_Gen6_TestBox\",\"NA\",\"0\",\"25/03/2013 05:49 pm\",\"BSM Head Office\",\"false\",\"History\"],[\"34040\",\"Box 34040\",\"PowerSave\",\"0\",\"17/03/2013 08:32 pm\",\"48 Summerhill Ave, Toronto, ON, M4T 1A8\",\"false\",\"History\"],[\"32165\",\"Box 32165\",\"NA*\",\"0\",\"11/03/2013 03:14 pm\",\"73 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"21994\",\"Chris's Jeep\",\"NA\",\"0\",\"26/02/2013 11:03 pm\",\"3641 Avenue Orient, Brossard, QC, J4Y 2K2\",\"false\",\"History\"],[\"9278\",\"Francisco_Test_Unit_3\",\"NA*\",\"55\",\"15/02/2013 10:51 am\",\"73 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"24761\",\"Wayne R - HOS\",\"NA*\",\"0\",\"08/02/2013 04:48 pm\",\"130 Ave Se, Calgary, AB, T2Z\",\"false\",\"History\"],[\"555670\",\"A1007-892-XS-00506835VTI655C\",\"NA\",\"0\",\"05/02/2013 04:57 pm\",\"71 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"991234\",\"QA_OBDIIBUS_TEST_991234\",\"NA\",\"0\",\"25/01/2013 11:34 am\",\"71 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"992345\",\"Box 992345\",\"PowerSave\",\"0\",\"23/01/2013 09:55 pm\",\"165 International Blvd, Etobicoke, ON, M9W 6L9\",\"false\",\"History\"],[\"969696\",\"Phil_SFM3000_Test_969696\",\"NA\",\"5\",\"16/01/2013 10:45 am\",\"200 Galaxy Blvd, Etobicoke, ON, M9W 5R8\",\"false\",\"History\"],[\"994499\",\"Ravi_Test Box 994499\",\"NA\",\"0\",\"09/01/2013 09:40 pm\",\"63 Mcintosh Ave, Hamilton, ON, L9B 1J3\",\"false\",\"History\"],[\"999927\",\"Box 999927\",\"NA\",\"0\",\"09/01/2013 05:26 pm\",\"71 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"999921\",\"Box  999921\",\"NA\",\"0\",\"22/11/2012 02:50 pm\",\"BSM\",\"false\",\"History\"],[\"10489\",\"Box 10489\",\"NA*\",\"0\",\"20/11/2012 01:10 pm\",\"BSM\",\"false\",\"History\"],[\"32166\",\"Box 32166\",\"NA*\",\"0\",\"16/11/2012 04:24 pm\",\"BSM\",\"false\",\"History\"],[\"32169\",\"Box 32169\",\"NA*\",\"0\",\"16/11/2012 03:25 pm\",\"BSM\",\"false\",\"History\"],[\"32164\",\"Box 32164\",\"NA*\",\"0\",\"14/11/2012 05:15 pm\",\"BSM\",\"false\",\"History\"],[\"32168\",\"Box 32168\",\"NA*\",\"0\",\"14/11/2012 03:50 pm\",\"BSM\",\"false\",\"History\"],[\"10707\",\"Francisco_Test_Unit_1\",\"NA\",\"0\",\"09/11/2012 04:35 pm\",\"BSM\",\"false\",\"History\"],[\"10000\",\"Francisco_Test_Unit_2\",\"NA\",\"0\",\"09/11/2012 04:25 pm\",\"BSM\",\"false\",\"History\"],[\"992299\",\"Ravi_Gen-6 Test Box\",\"NA*\",\"0\",\"02/11/2012 04:59 pm\",\"BSM\",\"false\",\"History\"],[\"789870\",\"Francisco_Gen6_870\",\"NA\",\"0\",\"02/11/2012 03:15 pm\",\"BSM\",\"false\",\"History\"],[\"600052\",\"Box 600052\",\"Parked\",\"0\",\"19/10/2012 06:40 pm\",\"(unnamed street), Moosomin No 121, SK, S0G\",\"false\",\"History\"],[\"606060\",\"AIRLINK_TEST_RAVI\",\"Moving\",\"1\",\"17/10/2012 05:04 pm\",\"(unnamed street), Wallace, MB, R0M\",\"false\",\"History\"],[\"21995\",\"Kristen's Car\",\"NA*\",\"0\",\"27/09/2012 02:22 pm\",\"101 Elizabeth St, Floral Park, NY, 11001\",\"false\",\"History\"],[\"24760\",\"Driss - HOS\",\"NA\",\"0\",\"21/09/2012 01:26 pm\",\"2624 Royal Windsor Dr, Clarkson, ON, L5J 1K7\",\"false\",\"History\"],[\"12422\",\"Ravi_Garmin_Test_Box_12422\",\"NA\",\"0\",\"29/08/2012 01:59 pm\",\"BSM\",\"false\",\"History\"],[\"999957\",\"Alban's Rogers G24\",\"NA*\",\"0\",\"23/08/2012 12:46 pm\",\"1310 Islington Ave, Etobicoke, ON, M9A 5C4\",\"false\",\"History\"],[\"10753\",\"QA_J1939_Test_Vehicle\",\"NA\",\"0\",\"23/08/2012 11:57 am\",\"BSM\",\"false\",\"History\"],[\"16035\",\"Tony\",\"NA\",\"0\",\"04/08/2012 01:01 am\",\"262 Mcroberts Ave, Toronto, ON, M6E 4P6\",\"false\",\"History\"],[\"979797\",\"Box 979797\",\"NA*\",\"0\",\"31/07/2012 01:09 pm\",\"BSM\",\"false\",\"History\"],[\"28106\",\"Box 28106\",\"NA\",\"0\",\"06/07/2012 05:20 pm\",\"BSM\",\"false\",\"History\"],[\"999954\",\"Francisco_Test_Unit_4\",\"NA*\",\"0\",\"29/06/2012 04:57 pm\",\"BSM\",\"false\",\"History\"],[\"909090\",\"Box 909\",\"NA\",\"0\",\"22/06/2012 04:04 pm\",\"BSM\",\"false\",\"History\"],[\"9997\",\"9997_TELUS_Ravi\",\"NA\",\"0\",\"21/06/2012 11:40 am\",\"BSM\",\"false\",\"History\"],[\"570001\",\"Box 570001_CHRIS\",\"NA\",\"0\",\"13/06/2012 03:02 pm\",\"BSM\",\"false\",\"History\"],[\"996699\",\"Box 996699\",\"NA*\",\"0\",\"07/06/2012 06:49 pm\",\"BSM\",\"false\",\"History\"],[\"60102\",\"raytest_vehicle02\",\"NA\",\"0\",\"04/06/2012 05:00 pm\",\"BSM\",\"true\",\"History\"],[\"10754\",\"Francisco HSPA Test Box\",\"NA*\",\"2\",\"07/05/2012 03:01 pm\",\"BSM\",\"false\",\"History\"],[\"999079\",\"Arie LOST-Box\",\"NA\",\"13\",\"25/04/2012 09:31 am\",\"69 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"10708\",\"ALBAN_KORE_SFM7000\",\"NA\",\"0\",\"10/04/2012 03:12 pm\",\"BSM\",\"false\",\"History\"],[\"12424\",\"Chameleon 232\",\"NA\",\"0\",\"04/04/2012 09:52 pm\",\"265 Rue De L'\u00c9cole, Les Coteaux, QC, J7X \",\"false\",\"History\"],[\"12423\",\"Chameleon 233\",\"NA\",\"0\",\"04/04/2012 07:54 pm\",\"151 Rue Gauthier, St-Polycarpe, QC, J0P \",\"false\",\"History\"],[\"9279\",\"9279_Bell_Ravi\",\"NA*\",\"0\",\"23/03/2012 12:59 pm\",\"Galaxy Blvd, Etobicoke, ON, M9W \",\"false\",\"History\"],[\"12321\",\"Box 12321\",\"NA\",\"0\",\"20/03/2012 03:47 pm\",\"5965 Hwy-7 [Hwy-7/Rr-7], Woodbridge, ON, L4L \",\"false\",\"History\"],[\"999950\",\"Mathieu car\",\"NA*\",\"0\",\"19/03/2012 12:57 pm\",\"172 Galaxy Blvd, Etobicoke, ON, M9W \",\"false\",\"History\"],[\"24763\",\"Eric M - HOS\",\"NA*\",\"0\",\"15/03/2012 01:19 pm\",\"71 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"999956\",\"HOS_ROGERS_TEST_BOX\",\"NA\",\"0\",\"13/03/2012 10:05 am\",\"190 Galaxy Blvd, Etobicoke, ON, M9W \",\"false\",\"History\"],[\"98989\",\"Box 98989\",\"NA\",\"0\",\"19/01/2012 09:21 am\",\"3134 Golden Orchard Dr, Mississauga, ON, L4X 2W2\",\"false\",\"History\"],[\"999951\",\"Mathieu\",\"NA\",\"0\",\"20/12/2011 03:03 pm\",\"test syed3\",\"false\",\"History\"],[\"10752\",\"Box 10752\",\"NA*\",\"0\",\"08/12/2011 03:53 pm\",\"BSM Wireless\",\"false\",\"History\"],[\"10709\",\"Bell Mobility Radio 1\",\"NA\",\"0\",\"08/12/2011 03:01 pm\",\"BSM Wireless\",\"true\",\"History\"],[\"999952\",\"1234567\",\"NA\",\"0\",\"07/12/2011 10:58 am\",\"BSM Wireless\",\"false\",\"History\"],[\"24075\",\"BOX 24075\",\"NA\",\"0\",\"01/12/2011 06:51 pm\",\"177 Wedgewood Dr W, Montgomery, TX, 77356\",\"false\",\"History\"],[\"999953\",\"Albans Mobile\",\"NA*\",\"0\",\"20/10/2011 10:26 am\",\"BSM Wireless\",\"false\",\"History\"],[\"700541\",\"Box 700541\",\"NA\",\"0\",\"31/08/2011 03:13 pm\",\"73 International Blvd, Etobicoke, ON, M9W 6K4\",\"false\",\"History\"],[\"5581\",\"Box_5581\",\"NA\",\"34\",\"24/01/2011 04:21 pm\",\"9282 The Gore Rd [Rr-8], Brampton, ON, L6P 0A9\",\"false\",\"History\"],[\"13017\",\"customIcontest\",\"NA*\",\"0\",\"09/04/2010 11:27 am\",\"BSM Wireless Inc.\",\"false\",\"History\"],[\"10755\",\"Devin 123\",\"NA\",\"0\",\"06/08/2009 02:50 pm\",\"5951 Rr-7 , Vaughan, ON, L4L\",\"false\",\"History\"],[\"10178\",\"jacky test vehicle1\",\"NA\",\"0\",\"09/07/2009 12:44 pm\",\"48 William Gasper Ct , Markham, ON, L6B\",\"false\",\"History\"],[\"500926\",\"Box 500926\",\"Null\",\"0\",\"01/01/1900 12:00 am\",\"Not activated\",\"false\",\"History\"],[\"800543\",\"Box 800543\",\"Null\",\"0\",\"01/01/1900 12:00 am\",\"Not activated\",\"false\",\"History\"],[\"919191\",\"Box 919191\",\"Null\",\"0\",\"01/01/1900 12:00 am\",\"Not activated\",\"false\",\"History\"],[\"10001\",\"test12\",\"Null\",\"0\",\"01/01/1900 12:00 am\",\"Not activated\",\"false\",\"History\"],[\"7000781\",\"test130212\",\"Null\",\"0\",\"01/01/1900 12:00 am\",\"Not activated\",\"false\",\"History\"]]}";
            //exportdata = "{\"Header\":[\"ID\",\"Date\",\"Inspection\",\"Off Duty\",\"Sleeper\",\"Driving\",\"On Duty\"],\"Data\":[[\"120183\",\"06/26/2013\",\"\",\"\",\"\",\"\",\"\"],[\"120303\",\"06/27/2013\",\"\",\"\",\"\",\"\",\"\"],[\"120444\",\"06/28/2013\",\"\",\"\",\"\",\"\",\"\"],[\"120518\",\"06/29/2013\",\"\",\"\",\"\",\"\",\"\"],[\"120769\",\"07/01/2013\",\"\",\"\",\"\",\"\",\"\"],[\"120770\",\"06/30/2013\",\"\",\"\",\"\",\"\",\"\"],[\"120946\",\"07/03/2013\",\"\",\"\",\"\",\"\",\"\"],[\"121065\",\"07/04/2013\",\"\",\"\",\"\",\"\",\"\"],[\"121155\",\"07/05/2013\",\"\",\"\",\"\",\"\",\"\"],[\"121289\",\"07/06/2013\",\"\",\"\",\"\",\"\",\"\"],[\"121480\",\"07/08/2013\",\"Pre-Trip 05:00[br]Post-Trip 16:00[br]Post-Trip 16:00[br]\",\"\",\"\",\"\",\"\"],[\"121481\",\"07/07/2013\",\"\",\"\",\"\",\"\",\"\"],[\"121633\",\"07/09/2013\",\"Post-Trip 16:00[br]Post-Trip 16:00[br]\",\"\",\"\",\"\",\"\"]]}";
            //exportdata = "{\"Header\":[\"UnitID\",\"Description\",\"Status\",\"Speed\",\"Date/Time\",\"Address\",\"Armed\",\"History\"],\"Data\":[[\"991199\",\"Ravi_Test_Box_991199\",\"NA\",\"24\",\"04/07/2013 04:57 pm\",\"Raymond \\\"\\\"Landmark\\\"\\\"04\",\"false\",\"History\"],[\"37432\",\"Costa SFM3000 Laval\",\"NA\",\"0\",\"04/07/2013 04:52 pm\",\"454 Boulevard Armand-Frappier, Laval-Des-Rapides, QC, H7V 4B4\",\"false\",\"History\"],[\"939393\",\"939393-QingQing\",\"NA*\",\"0\",\"04/07/2013 04:47 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"9281\",\"Devon Test\",\"NA*\",\"0\",\"04/07/2013 03:55 pm\",\"Hwy-403, Mississauga, ON, L5L\",\"false\",\"History\"],[\"30610\",\"Box 30610-Devin\",\"NA\",\"0\",\"04/07/2013 11:55 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"10702\",\"Joanna\",\"NA*\",\"0\",\"04/07/2013 10:02 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"16267\",\"Brendan\",\"NA\",\"0\",\"04/07/2013 09:42 am\",\"Brendan\",\"false\",\"History\"],[\"949494\",\"3000BT 949494\",\"PowerSave\",\"0\",\"04/07/2013 09:11 am\",\"BSM Head Office\",\"false\",\"History\"],[\"999877\",\"Phil's Toyota\",\"NA\",\"0\",\"04/07/2013 09:07 am\",\"Raymond Landmark04\",\"false\",\"History\"],[\"10269\",\"Randy-2000\",\"NA\",\"0\",\"04/07/2013 09:02 am\",\"Raymond Landmark04\",\"true\",\"History\"],[\"999999\",\"Box     999999\",\"NA\",\"0\",\"03/07/2013 05:12 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"15013\",\"Ben's test unit\",\"NA*\",\"0\",\"03/07/2013 08:27 am\",\"1458 Boulevard De L'Avenir, Laval-Des-Rapides, QC, H7N\",\"false\",\"History\"],[\"995599\",\"Ravi_995599_QA_Garmin_Test_Box\",\"PowerSave\",\"0\",\"02/07/2013 04:38 pm\",\"Raymond Landmark04\",\"false\",\"History\"],[\"9268\",\"peter\",\"NA\",\"0\",\"02/07/2013 02:38 pm\",\"20 Hawkridge Trl, Brampton, ON, L6P 2T4\",\"false\",\"History\"],[\"24765\",\"Chris P - HOS\",\"NA*\",\"0\",\"02/07/2013 11:17 am\",\"85 Elm St, Toronto, ON, M5G 0A8\",\"false\",\"History\"]]}";
            //exportdata = "{\"Header\":[\"UnitID\"],\"Data\":[[\"120183\"]]}";

            string filename = "download";
            if (Request["filename"] != null && Request["filename"].ToString().Trim() != string.Empty)
                filename = Request["filename"].ToString().Trim();

            string formatter = "csv";
            if (Request["formatter"] != null && Request["formatter"].ToString().Trim() != string.Empty)
                formatter = Request["formatter"].ToString().Trim();

            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = int.MaxValue;
            
            exportDataStruture foo = js.Deserialize<exportDataStruture>(exportdata);
            SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];

            if (formatter == "csv")
            {

                StringBuilder sresult = new StringBuilder();
                sresult.Append("sep=,");
                sresult.Append(Environment.NewLine);
                //sresult.Append("DriverID,First Name, Last Name, License, Vehicle Description");
                string header = string.Empty;
                foreach (string s in foo.Header)
                {
                    header += "\"" + s + "\",";
                }
                header = header.Substring(0, header.Length - 1);
                sresult.Append(header);
                sresult.Append(Environment.NewLine);

                foreach (string[] sa in foo.Data)
                {
                    string data = string.Empty;
                    foreach (string s in sa)
                    {
                        data += "\"" + s.Replace("[br]", Environment.NewLine).Replace("\"", "\"\"") + "\",";
                    }
                    data = data.Substring(0, data.Length - 1);
                    sresult.Append(data);
                    sresult.Append(Environment.NewLine);
                }


                Response.Clear();
                Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.csv", filename));
                Response.Charset = Encoding.GetEncoding("iso-8859-1").BodyName;
                Response.ContentType = "application/csv";
                Response.ContentEncoding = Encoding.GetEncoding("iso-8859-1");

                Response.Write(sresult.ToString());
                Response.Flush();
                Response.End();
            }
            else if (formatter == "excel2003")
            {
                HSSFWorkbook wb = new HSSFWorkbook();
                ISheet ws = wb.CreateSheet("Sheet1");
                ICellStyle cellstyle1 = wb.CreateCellStyle();
                ICellStyle cellstyle2 = wb.CreateCellStyle();
                ICellStyle cellstyle3 = wb.CreateCellStyle();
                ICellStyle cellstyle4 = wb.CreateCellStyle();
                ICellStyle cellstyle5 = wb.CreateCellStyle();
                cellstyle1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                cellstyle2.FillPattern = FillPatternType.SOLID_FOREGROUND;
                cellstyle3.FillPattern = FillPatternType.SOLID_FOREGROUND;
                cellstyle4.FillPattern = FillPatternType.SOLID_FOREGROUND;
                cellstyle5.FillPattern = FillPatternType.SOLID_FOREGROUND;
                HSSFPalette palette = wb.GetCustomPalette();
                palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index, (byte)123, (byte)178, (byte)115);
                palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.YELLOW.index, (byte)239, (byte)215, (byte)0);
                palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index, (byte)255, (byte)166, (byte)74);
                palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.ROSE.index, (byte)222, (byte)121, (byte)115);
                palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.INDIGO.index, (byte)99, (byte)125, (byte)165);                
                cellstyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index;
                cellstyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index; 
                cellstyle3.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index;
                cellstyle4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.ROSE.index;
                cellstyle5.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.INDIGO.index;
                IRow row = ws.CreateRow(0);
                for (int i = 0; i < foo.Header.Length; i++)
                {
                    row.CreateCell(i).SetCellValue(foo.Header[i]);
                }

                for (int i = 0; i < foo.Data.Length; i++)
                {
                    string data = string.Empty;
                    IRow rowData = ws.CreateRow(i + 1);
                    for (int j = 0; j < foo.Data[i].Length; j++)
                    {
                        rowData.CreateCell(j).SetCellValue(foo.Data[i][j].Replace("[br]", Environment.NewLine));
                        //if (foo.Header[j] == "Date/Time" || foo.Header[j] == "Data/Hora" || foo.Header[j] == "Fecha/Hora" || foo.Header[j] == "Date / Heure" || foo.Header[j] == "Date/Temps")
                        //{                            
                        //    DateTime currentDate = DateTime.Now.ToUniversalTime();
                        //    DateTime recordDate;

                        //    if (!(sn.User.UnitOfMes < 0.7))
                        //    {
                        //        //recordDate = DateTime.ParseExact(foo.Data[i][j], "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime(); 
                        //        try
                        //        {
                        //            recordDate = DateTime.ParseExact(foo.Data[i][j], "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                        //        }
                        //        catch
                        //        {
                        //            recordDate = DateTime.ParseExact(foo.Data[i][j], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //recordDate = DateTime.ParseExact(foo.Data[i][j], "MM/dd/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                        //        try
                        //        {
                        //            recordDate = DateTime.ParseExact(foo.Data[i][j], "MM/dd/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                        //        }
                        //        catch
                        //        {
                        //            recordDate = DateTime.ParseExact(foo.Data[i][j], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                        //        }
                        //    }
                        //    TimeSpan diffDate = currentDate.Subtract(recordDate);

                        //    if (diffDate .TotalHours < 24)
                        //    {
                        //        cellstyle1.WrapText = true;
                        //        cellstyle1.VerticalAlignment = VerticalAlignment.TOP;
                        //        rowData.Cells[j].CellStyle = cellstyle1;
                        //    }
                        //    else if (diffDate.TotalHours < 48)
                        //    {
                        //        cellstyle2.WrapText = true;
                        //        cellstyle2.VerticalAlignment = VerticalAlignment.TOP;
                        //        rowData.Cells[j].CellStyle = cellstyle2;
                        //    }
                        //    else if (diffDate.TotalHours < 72)
                        //    {
                        //        cellstyle3.WrapText = true;
                        //        cellstyle3.VerticalAlignment = VerticalAlignment.TOP;
                        //        rowData.Cells[j].CellStyle = cellstyle3;
                        //    }
                        //    else if (diffDate.TotalHours < 168)
                        //    {
                        //        cellstyle4.WrapText = true;
                        //        cellstyle4.VerticalAlignment = VerticalAlignment.TOP;
                        //        rowData.Cells[j].CellStyle = cellstyle4;
                        //    }
                        //    else if (diffDate.TotalHours > 168)
                        //    {
                        //        cellstyle5.WrapText = true;
                        //        cellstyle5.VerticalAlignment = VerticalAlignment.TOP;
                        //        rowData.Cells[j].CellStyle = cellstyle5;
                        //    }         

                        //}                
                        
                    }
                }

                for (int i = 0; i < foo.Data[0].Length; i++)
                {
                    try
                    {
                        ws.AutoSizeColumn(i);
                    }
                    catch { }
                }
                

                //for (int i = foo.Data.Length; i < 100; i++)
                //    ws.Cells[i, 0] = new Cell("");

                HttpContext.Current.Response.Clear();

                Response.AddHeader("Content-Type", "application/Excel");
                Response.ContentType = "application/vnd.xls";
                HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", filename));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.Write(memoryStream);
                    memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    memoryStream.Close();
                }

                HttpContext.Current.Response.End();

            }
            else if (formatter == "excel2007")
            {
                try
                {
                    var wb = new XLWorkbook();
                    var ws = wb.Worksheets.Add("Sheet1");
                    
                    for (int i = 0; i < foo.Header.Length; i++)
                    {
                        ws.Cell(1, i + 1).Value = foo.Header[i];
                    }

                    for(int i=0; i<foo.Data.Length;i++)
                    {
                        string data = string.Empty;
                        for (int j = 0; j < foo.Data[i].Length; j++)
                        {
                            ws.Cell(i + 2, j + 1).DataType = XLCellValues.Text;
                            ws.Cell(i + 2, j + 1).Value = "'" + foo.Data[i][j].Replace("[br]", Environment.NewLine);
                            //if (foo.Header[j] == "Date/Time" || foo.Header[j] == "Data/Hora" || foo.Header[j] == "Fecha/Hora" || foo.Header[j] == "Date / Heure" || foo.Header[j] == "Date/Temps")
                            //{
                            //    DateTime currentDate = DateTime.Now.ToUniversalTime();
                            //    DateTime recordDate;

                            //    if (!(sn.User.UnitOfMes < 0.7))
                            //    {
                            //        //recordDate = DateTime.ParseExact(foo.Data[i][j], "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                            //        try
                            //        {
                            //            recordDate = DateTime.ParseExact(foo.Data[i][j], "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                            //        }
                            //        catch
                            //        {
                            //            recordDate = DateTime.ParseExact(foo.Data[i][j], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //recordDate = DateTime.ParseExact(foo.Data[i][j], "MM/dd/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                            //        try
                            //        {
                            //            recordDate = DateTime.ParseExact(foo.Data[i][j], "MM/dd/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                            //        }
                            //        catch
                            //        {
                            //            recordDate = DateTime.ParseExact(foo.Data[i][j], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                            //        }
                            //    }
                            //    TimeSpan diffDate = currentDate.Subtract(recordDate);

                            //    if (diffDate.TotalHours < 24)
                            //    {
                            //        ws.Cell(i + 2, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#7BB273");
                            //    }
                            //    else if (diffDate.TotalHours < 48)
                            //    {
                            //        ws.Cell(i + 2, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#EFD700");
                            //    }
                            //    else if (diffDate.TotalHours < 72)
                            //    {
                            //        ws.Cell(i + 2, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFA64A");
                            //    }
                            //    else if (diffDate.TotalHours < 168)
                            //    {
                            //        ws.Cell(i + 2, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#DE7973");
                            //    }
                            //    else if (diffDate.TotalHours > 168)
                            //    {
                            //        ws.Cell(i + 2, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#637DA5");
                            //    }

                            //}
                        }
                    }

                    ws.Rows().Style.Alignment.SetWrapText();
                    ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    ws.Columns().AdjustToContents();    

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", filename));

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wb.SaveAs(memoryStream);
                        memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                        memoryStream.Close();
                    }

                    HttpContext.Current.Response.End();
                }
                catch
                {
                }

            }
            
        }
    }

    public class exportDataStruture
    {
        public string[] Header { get; set; }
        public string[][] Data { get; set; }
    }
    
}
