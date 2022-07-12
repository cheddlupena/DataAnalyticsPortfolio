using BrotherConnection.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;
using System.ComponentModel;

/*
 this function is for.....
 */

namespace BrotherConnection
{
    using MTConnect;
    class Program
    {
        static string STOP;
        static string STATUS;
        static string PROGRAMNUM;
        static string PANELMODE;
        static string MAINTNOTICE; //for message
        static string MAINTNOTICECURVALUE; //for current value
        static string MAINTNOTICETARVALUE; // for target value
        static string MAINTNOTICEFUNC; // function
        static string MAINTNOTICENTC; // notice
        static string MAINTNOTICEUNIT; // unit
        static string MAINTNOTICESTATUS; //for Status
        static string MAINTNOTICESTATUS1;
        static string FEEDRATE;
        static string ALARM;
        static string SPINDLESPEED;
        static string SPINDLETOOL;
        static string DOORINTERLOCK;
        static string FEEDOVERRIDE;
        static string SPINDLEOVERRIDE;
        static string OP;

        /*
         * this is for...
         */
        public static void Main(string[] args)
        {
            Adapter adapter = new Adapter();
            Event mAvail = new Event("avail");
            Event eStop = new Event("eStop");                               //PANEL
            Event mStatus = new Event("status");                            //PRD3
            Event ProgNoErrNo = new Event("ProgramNum/ErrorNum");

            Event panelMode = new Event("pMode");                           //PANEL
            Message mesNotice = new Message("message");                     //MAINTENANCE NOTICE
            Message mAlarm = new Message("alarm");                          //ALARM

            Sample Feedrate = new Sample("Feedrate");                       //PDSP
            Sample SpindleSpeed = new Sample("sSpeed");                     //PDSP
            Event SpindleTool = new Event("sToolNum");                      //PDSP
            Event DoorInterlock = new Event("opMode");                      //PDSP

            Event fOverride = new Event("fOverride");                       //PDSP
            Event sOverride = new Event("sOverride");                       //PDSP

            Event OPLOG = new Event("OPLOG");

            Event unit = new Event("MAINTC_UNIT");
            Event function = new Event("MAINTC_FUNCTION");
            Event curval = new Event("MAINTC_CURRENT_VAL");
            Event tarval = new Event("MAINTC_TARGET_VAL");
            Event ntc = new Event("MAINTC_NOTICE");
            Event stat = new Event("MAINTC_STATUS");



            Event WorkPieceCount = new Event("WrkpcCntrCount");             //WRKPNTR
            Event WorkPieceCurrent = new Event("WrkpcCntrCurrent");         //WRKPNTR
            //Event WorkPieceEnd = new Event("WrkpcCntrEnd");               //WRKPNTR
            //Event WorkPieceWarning = new Event("WrkpcCntrWarning");       //WRKPNTR

            var prodData3Map = JsonConvert.DeserializeObject<DataMap>(File.ReadAllText("ProductionData3.json"));
            var prodData2Map = JsonConvert.DeserializeObject<DataMap>(File.ReadAllText("ProductionData2.json"));
            var posData = JsonConvert.DeserializeObject<DataMap>(File.ReadAllText("PositionData.json"));
            var alarm = JsonConvert.DeserializeObject<DataMap>(File.ReadAllText("Alarm.json"));
            var panel = JsonConvert.DeserializeObject<DataMap>(File.ReadAllText("Panel.json"));
            var mNotice = JsonConvert.DeserializeObject<DataMap>(File.ReadAllText("MaintenanceNotice.json"));
            var wrkpcntr = JsonConvert.DeserializeObject<DataMap>(File.ReadAllText("WorkpieceCount.json"));
            var oplog = JsonConvert.DeserializeObject<DataMap>(File.ReadAllText("Oplog.json"));

            adapter.AddDataItem(mAvail);
            mAvail.Value = "AVAILABLE";
            adapter.AddDataItem(eStop);
            adapter.AddDataItem(mStatus);
            adapter.AddDataItem(ProgNoErrNo);
            adapter.AddDataItem(panelMode);
            adapter.AddDataItem(mesNotice);
            adapter.AddDataItem(mAlarm);
            adapter.AddDataItem(Feedrate);
            adapter.AddDataItem(SpindleSpeed);
            adapter.AddDataItem(SpindleTool);
            adapter.AddDataItem(DoorInterlock);
            adapter.AddDataItem(fOverride);
            adapter.AddDataItem(sOverride);
            adapter.AddDataItem(OPLOG);
            adapter.AddDataItem(WorkPieceCount);
            adapter.AddDataItem(WorkPieceCurrent);
            //adapter.AddDataItem(WorkPieceEnd);
            //adapter.AddDataItem(WorkPieceWarning);
            adapter.AddDataItem(unit);
            adapter.AddDataItem(function);
            adapter.AddDataItem(curval);
            adapter.AddDataItem(tarval);
            adapter.AddDataItem(ntc);
            adapter.AddDataItem(stat);

            adapter.Begin();
            adapter.Port = Convert.ToInt32("7878");
            adapter.Start();
            adapter.SendChanged();
            /*
            if (true)
            {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\MICROSOFT\\Windows\\CurrentVersion\\Run", true);
                reg.SetValue("BrotherConnection", System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.WriteLine( System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) );
                Console.WriteLine("AutoStartConfirmed");
            }

            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString() + "\\Agent\\runAgent.bat";
                    myProcess.StartInfo.CreateNoWindow = false;
                    myProcess.Start();
                }
                using (Process myProcess1 = new Process())
                {
                    myProcess1.StartInfo.UseShellExecute = false;
                    myProcess1.StartInfo.FileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Azure\\MTConnectAgent.exe";
                    myProcess1.StartInfo.CreateNoWindow = false;
                    myProcess1.Start();
                }
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.StartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString() + "\\BrotherConnection.exe";
                    myProcess.StartInfo.CreateNoWindow = false;
                    myProcess.Start();
                }
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            */



            var filesToLoad = new List<String>
              {
                prodData3Map.FileName,
                //prodData2Map.FileName,                    //CycleTime
                posData.FileName,                           //SpindleTool - SpindleSpeed - Feedrate - Spindle Override - Feed Override
                //alarm.FileName,
                mNotice.FileName,                           //Notifications
                //panel.FileName,                             //Emergency Stop
                wrkpcntr.FileName,                          //Workpiece Counter
                oplog.FileName                              //Panel Inputs
             };

            while (true)
            {
                foreach (var file in filesToLoad)
                {

                    if (file.ToString() == posData.FileName)
                    {
                        var DecodedResults = new Dictionary<String, String>();
                        Console.Clear();
                        var req = new Request();
                        req.Command = "LOD";
                        req.Arguments = posData.FileName;
                        var rawData = req.Send().Split(new String[] { "\r\n" }, StringSplitOptions.None);
                        Console.Write(req.Send());
                        foreach (var line in posData.Lines)
                        {
                            var rawLine = rawData[line.Number].Split(',');
                            if (rawLine[0] == line.Symbol)
                            {
                                rawLine = rawLine.Skip(1).ToArray();
                                for (int i = 1; i < line.Items.Count; i++)
                                {
                                    if (line.Items[i].Type == "Number")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }
                                    else
                                    {
                                        DecodedResults[line.Items[i].Name] = line.Items[i].EnumValues.First(v => v.Index == Convert.ToInt32(rawLine[i])).Value;
                                    }
                                }
                            }
                        }
                        foreach (var decode in DecodedResults)
                        {
                            Console.WriteLine($"{decode.Key}: {decode.Value}");

                            if (decode.Key == "Spindle Speed")
                            {
                                
                                Console.WriteLine(decode.Key + " = " + SPINDLESPEED + "-----------" + decode.Value.ToString());
                                if (SPINDLESPEED != decode.Value.ToString())
                                {
                                    SpindleSpeed.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                SPINDLESPEED = decode.Value.ToString();
                                Console.WriteLine(decode.Key + " = " + SPINDLESPEED + "-----------" + decode.Value.ToString());
                            }

                            if (decode.Key == "Spindle tool No.")
                            {
                                Console.WriteLine(decode.Key + " = " + SPINDLETOOL + "-----------" + decode.Value.ToString());
                                if (SPINDLETOOL != decode.Value.ToString())
                                {
                                    SpindleTool.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                SPINDLETOOL = decode.Value.ToString();
                                Console.WriteLine(decode.Key + " = " + SPINDLETOOL + "-----------" + decode.Value.ToString());
                            }

                            if (decode.Key == "Spindle Override")
                            {
                                Console.WriteLine(decode.Key + " = " + SPINDLEOVERRIDE + "-----------" + decode.Value.ToString());
                                if (SPINDLEOVERRIDE != decode.Value.ToString())
                                {
                                    sOverride.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                SPINDLEOVERRIDE = decode.Value.ToString();
                                Console.WriteLine(decode.Key + " = " + SPINDLEOVERRIDE + "-----------" + decode.Value.ToString());
                            }

                            if (decode.Key == "Feedrate")
                            {
                                Console.WriteLine(decode.Key + " = " + FEEDRATE + "-----------" + decode.Value.ToString());
                                if (FEEDRATE != decode.Value.ToString())
                                {
                                    Feedrate.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                FEEDRATE = decode.Value.ToString();
                                Console.WriteLine(decode.Key + " = " + FEEDRATE + "-----------" + decode.Value.ToString());
                            }

                            if (decode.Key == "Feedrate override")
                            {
                                Console.WriteLine(decode.Key + " = " + FEEDOVERRIDE + "-----------" + decode.Value.ToString());
                                if (FEEDOVERRIDE != decode.Value.ToString())
                                {
                                    fOverride.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                FEEDOVERRIDE = decode.Value.ToString();
                                Console.WriteLine(decode.Key + " = " + FEEDOVERRIDE + "-----------" + decode.Value.ToString());
                            }

                            if (decode.Key == "Door interlock")
                            {
                                if (DOORINTERLOCK != decode.Value.ToString())
                                {
                                    if (decode.Value == "0")
                                    {
                                        DoorInterlock.Value = "OPEN";
                                        adapter.SendChanged();
                                    }
                                    if (decode.Value == "1")
                                    {
                                        DoorInterlock.Value = "UNLATCHED";
                                        adapter.SendChanged();
                                    }
                                    if (decode.Value == "2")
                                    {
                                        DoorInterlock.Value = "CLOSED";
                                        adapter.SendChanged();
                                    }
                                }
                                DOORINTERLOCK = decode.Value.ToString();
                            }

                            if (decode.Key == "")
                            {
                                decode.Value.ToString();
                            }
                            else if (decode.Value == "")
                            {
                                Console.Write("No Decoded results.");
                            }
                        }
                    }

                    if (file.ToString() == prodData3Map.FileName)
                    {
                        var DecodedResults = new Dictionary<String, String>();
                        var req = new Request();
                        req.Command = "LOD";
                        req.Arguments = prodData3Map.FileName;
                        var rawData = req.Send().Split(new String[] { "\r\n" }, StringSplitOptions.None);
                        Console.Write(req.Send());
                        foreach (var line in prodData3Map.Lines)
                        {
                            var rawLine = rawData[line.Number].Split(',');
                            if (rawLine[0] == line.Symbol)
                            {
                                rawLine = rawLine.Skip(1).ToArray();
                                for (int i = 1; i < line.Items.Count; i++)
                                {
                                    if (line.Items[i].Type == "Number")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }
                                    if (line.Items[i].Type == "Character")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }
                                    if (line.Items[i].Type == "enum")
                                    {
                                        DecodedResults[line.Items[i].Name] = line.Items[i].EnumValues.First(v => v.Index == Convert.ToInt32(rawLine[i])).Value;
                                    }
                                }
                            }
                        }
                        /*
                         var filesToLoad0 = new List<String>
                         {/*
                             "POSNI#",
                             "POSSI#",
                             "TOLSI#",
                             "MCRNI#",
                             "MCRSI#",
                             "EXIO#",
                             "ATCTL",
                             "GCOMT",
                             "CSTPL1",
                             "CSTTP1",
                             "SYSC99",
                             "SYSC98",
                             "SYSC97",
                             "SYSC96",
                             "SYSC95",
                             "SYSC94",
                             "SYSC89",
                             "PRD1",
                             "PRDC2",
                             "PRD3",
                             "MAINTC",
                             "WKCNTR",
                             "MSRRSC",
                             "PAINT", 
                             "WVPRM",  
                             "PLCDAT",
                             "PLCMON",
                             "SHTCUT",
                             "IO",
                             "MEM",
                             "PANEL",
                             "PDSP",
                             "VER",
                             "LOG",
                             "LOGBK",
                             "ALARM",
                             "OPLOG",
                             "PRTCTC" 
                         };
                        foreach (var file0 in filesToLoad0)
                        {
                            if (file0.Contains("#"))
                            {
                                for (int i = 0; i <= 10 ; i++)
                                {
                                    var toLoad = file0.Replace('#', i.ToString().Last()); // 0 is 10, not 0
                                    req.Arguments = toLoad;
                                    File.WriteAllText(toLoad + ".txt", req.Send());
                                    Console.WriteLine($"Loaded {toLoad}");
                                }
                            }
                            else
                            {
                                req.Arguments = file0;
                                File.WriteAllText(file0 + ".RAW", req.Send());
                                Console.WriteLine($"Loaded {file0}");
                            }
                        }
                        req.Arguments = "PANEL";
                        Console.Write(req.Send());
                        req.Arguments = "MEM";
                        Console.Write(req.Send());
                        req.Arguments = "ALARM";
                        Console.Write(req.Send());
                        req.Arguments = "TOLNI1";
                        Console.Write(req.Send());
                        req.Arguments = "MCRNI1";
                        Console.Write(req.Send());
                        */

                        foreach (var decode in DecodedResults)
                        {                                                   //PRD3
                            if (decode.Key == "Current (Status)")
                            {
                                Console.WriteLine("*****************************" + decode.Key + "        " + decode.Value + "*****" + STATUS + "    " + mStatus.Value.ToString());
                                if (STATUS != decode.Value.ToString())
                                {
                                    if (decode.Value == "Power OFF")
                                    {
                                        mStatus.Value = "Unavailable";
                                        adapter.SendChanged();
                                    }

                                    if (decode.Value == "Standby mode")
                                    {
                                        mStatus.Value = "Setup";
                                        adapter.SendChanged();
                                    }

                                    if (decode.Value == "Operating")
                                    {
                                        mStatus.Value = "Running";
                                        adapter.SendChanged();
                                    }

                                    if (decode.Value == "Stopped")
                                    {
                                        mStatus.Value = "Stop";
                                        adapter.SendChanged();
                                    }

                                    if (decode.Value == "Error")
                                    {
                                        mStatus.Value = "Error";
                                        adapter.SendChanged();
                                    }

                                }

                                Console.WriteLine(STATUS);
                                STATUS = decode.Value.ToString();
                                Console.WriteLine(STATUS);
                            }


                            if (decode.Key == "Current status (Program No. / Error No.)")
                            {
                                Console.WriteLine("*****************************" + decode.Key + "        " + decode.Value + "*****" + STATUS + "    " + mStatus.Value.ToString());
                                if (PROGRAMNUM != decode.Value.ToString())
                                {
                                    ProgNoErrNo.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                Console.WriteLine(PROGRAMNUM);
                                PROGRAMNUM = decode.Value.ToString();
                                Console.WriteLine(PROGRAMNUM);
                            }
                        

                            Console.WriteLine($"{decode.Key}: {decode.Value}"+decode.Value.ToString());
                            if (decode.Key == "")
                            {
                                decode.Value.ToString();
                            }

                            else if (decode.Value == "")
                            {
                                Console.Write("No Decoded results.");
                            }
                        }
                    }
                    
                    if (file.ToString() == panel.FileName)
                    {
                        var DecodedResults = new Dictionary<String, String>();
                        Console.Clear();    
                        var req = new Request();
                        req.Command = "LOD";
                        req.Arguments = panel.FileName;
                        var rawData = req.Send().Split(new String[] { "\r\n" }, StringSplitOptions.None);
                        Console.Write(req.Send());
                        foreach (var line in panel.Lines)
                        {
                            var rawLine = rawData[line.Number].Split(',');
                            if (rawLine[0] == line.Symbol)
                            {
                                rawLine = rawLine.Skip(1).ToArray();
                                for (int i = 1; i < line.Items.Count; i++)
                                {
                                    if (line.Items[i].Type == "Number")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }

                                    if (line.Items[i].Type == "Character")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }

                                    if (line.Items[i].Type == "enum")
                                    {
                                      DecodedResults[line.Items[i].Name] = line.Items[i].EnumValues.First(v => v.Index == Convert.ToInt32(rawLine[i])).Value;
                                    }
                                }
                            }
                        }
                        foreach (var decode in DecodedResults)
                        {
                            Console.WriteLine($"{decode.Key}: {decode.Value}");
                            if (decode.Key == "Emergency Stop")
                            {
                                if (STOP != decode.Value)
                                {
                                    if (decode.Value == "ON")
                                    {
                                        eStop.Value = "TRIGGERED";
                                        adapter.SendChanged();
                                    }

                                    else
                                    {
                                        eStop.Value = "ARMED";
                                        adapter.SendChanged();
                                    }
                                }
                                STOP = decode.Value.ToString();
                            }

                            if (decode.Key == "")
                            {
                                decode.Value.ToString();
                            }

                            else if (decode.Value == "")
                            {
                                Console.Write("No Decoded results.");
                            }
                        }
                    }
                    
                    if (file.ToString() == alarm.FileName)
                    {
                        var DecodedResults = new Dictionary<String, String>();
                        Console.Clear();
                        var req = new Request();
                        req.Command = "LOD";
                        req.Arguments = alarm.FileName;
                        var rawData = req.Send().Split(new String[] { "\r\n" }, StringSplitOptions.None);
                        Console.Write(req.Send());
                        foreach (var line in alarm.Lines)
                        {
                            var rawLine = rawData[line.Number].Split(',');
                            if (rawLine[0] == line.Symbol)
                            {
                                rawLine = rawLine.Skip(1).ToArray();
                                for (int i = 1; i < line.Items.Count; i++)
                                {
                                    if (line.Items[i].Type == "Number")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }

                                    if (line.Items[i].Type == "Character")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }

                                    if (line.Items[i].Type == "enum")
                                    {
                                        DecodedResults[line.Items[i].Name] = line.Items[i].EnumValues.First(v => v.Index == Convert.ToInt32(rawLine[i])).Value;
                                    }
                                }
                            }
                        }
                        foreach (var decode in DecodedResults)
                        {
                            Console.WriteLine($"{decode.Key}: {decode.Value}");

                            if (decode.Key == "Alarm/Operator message No.1")
                            {
                                if (ALARM != decode.Value)
                                {
                                    mAlarm.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                ALARM = decode.Value.ToString();
                            }

                            if (decode.Key == "")
                            {
                                decode.Value.ToString();
                            }

                            else if (decode.Value == "")
                            {
                                Console.Write("No Decoded results.");
                            }
                        }
                    }
                    
                    if (file.ToString() == mNotice.FileName)
                    {
                        var DecodedResults = new Dictionary<String, String>();
                        Console.Clear();
                        var req = new Request();
                        req.Command = "LOD";
                        req.Arguments = mNotice.FileName;
                        var rawData = req.Send().Split(new String[] { "\r\n" }, StringSplitOptions.None);
                        Console.Write(req.Send());
                        foreach (var line in mNotice.Lines)
                        {
                            var rawLine = rawData[line.Number].Split(',');
                            if (rawLine[0] == line.Symbol)
                            {
                                rawLine = rawLine.Skip(1).ToArray();
                                for (int i = 1; i < line.Items.Count; i++)
                                {
                                    if (line.Items[i].Type == "Number")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }

                                    if (line.Items[i].Type == "Character")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }

                                    if (line.Items[i].Type == "enum")
                                    {
                                        DecodedResults[line.Items[i].Name] = line.Items[i].EnumValues.First(v => v.Index == Convert.ToInt32(rawLine[i])).Value;
                                    }
                                }
                            }
                        }
                        foreach (var decode in DecodedResults)
                        {
                            Console.WriteLine($"{decode.Key}: {decode.Value}");
                            if (decode.Key == "No.1 (Message)")
                            {
                                if (MAINTNOTICE != decode.Value)
                                {
                                    mesNotice.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                MAINTNOTICE = decode.Value.ToString();
                            }

                            if (decode.Key == "No.1 (Type)")
                            {
                                if (MAINTNOTICEUNIT != decode.Value)
                                {
                                    unit.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                MAINTNOTICEUNIT = decode.Value.ToString();
                            }

                            if (decode.Key == "No.1 (Function)")
                            {
                                if (MAINTNOTICEFUNC != decode.Value)
                                {
                                    function.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                MAINTNOTICEFUNC = decode.Value.ToString();
                            }

                            if (decode.Key == "No.1 (Current value)")
                            {
                                if (MAINTNOTICECURVALUE != decode.Value)
                                {
                                    curval.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                MAINTNOTICECURVALUE = decode.Value.ToString();
                            }

                            if (decode.Key == "No.1 (End value)")
                            {
                                if (MAINTNOTICETARVALUE != decode.Value)
                                {
                                    tarval.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                MAINTNOTICETARVALUE = decode.Value.ToString();
                            }

                            if (decode.Key == "No.1 (Message notification)")
                            {
                                if (MAINTNOTICENTC != decode.Value)
                                {
                                    ntc.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                MAINTNOTICENTC = decode.Value.ToString();
                            }

                            
                                int currentvalue = int.Parse(MAINTNOTICECURVALUE);
                                int targetvalue = int.Parse(MAINTNOTICETARVALUE);

                                if (currentvalue > targetvalue)
                                {
                                    stat.Value = "Tgt Val Over";
                                    MAINTNOTICESTATUS1 = "Tgt Val Over";
                                    adapter.SendChanged();

                                }

                                if (currentvalue < targetvalue)

                                {
                                    stat.Value = "Tgt Val Under";
                                    MAINTNOTICESTATUS1 = "Tgt Val Under";
                                    adapter.SendChanged();

                                } if (currentvalue ==  targetvalue)

                                {
                                    stat.Value = "Tgt Val Equal";
                                    MAINTNOTICESTATUS1 = "Tgt Val Under";
                                    adapter.SendChanged();
                                }

                            if (decode.Key == "")
                            {
                                decode.Value.ToString();
                            }

                            else if (decode.Value == "")
                            {
                                Console.Write("No Decoded results.");
                            }
                        }
                    }
                    if (file.ToString() == oplog.FileName)
                    {
                        var DecodedResults = new Dictionary<String, String>();
                        Console.Clear();
                        var req = new Request();
                        req.Command = "LOD";
                        req.Arguments = oplog.FileName;
                        var rawData = req.Send().Split(new String[] { "\r\n" }, StringSplitOptions.None);
                        Console.Write(req.Send());
                        foreach (var line in oplog.Lines)
                        {
                            var rawLine = rawData[line.Number].Split(',');
                            if (rawLine[0] == line.Symbol)
                            {
                                rawLine = rawLine.Skip(1).ToArray();
                                for (int i = 1; i < line.Items.Count; i++)
                                {
                                    if (line.Items[i].Type == "Number")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }

                                    if (line.Items[i].Type == "Character")
                                    {
                                        DecodedResults[line.Items[i].Name] = rawLine[i].Trim();
                                    }

                                    if (line.Items[i].Type == "enum")
                                    {
                                        DecodedResults[line.Items[i].Name] = line.Items[i].EnumValues.First(v => v.Index == Convert.ToInt32(rawLine[i])).Value;
                                    }
                                }
                            }
                        }
                        foreach (var decode in DecodedResults)
                        {
                            Console.WriteLine($"{decode.Key}: {decode.Value}");
                            if (decode.Key == "Key log 1 (Key type)")
                            {
                                if (OP != decode.Value)
                                {
                                    OPLOG.Value = decode.Value;
                                    adapter.SendChanged();
                                }
                                OP = decode.Value.ToString();
                            }

                            if (decode.Key == "")
                            {
                                decode.Value.ToString();
                            }

                            else if (decode.Value == "")
                            {
                                Console.Write("No Decoded results.");
                            }
                        }
                    }
                    //throw new NotImplementedException();
                    Thread.Sleep(10);
                }
            }
        }
    }
}




