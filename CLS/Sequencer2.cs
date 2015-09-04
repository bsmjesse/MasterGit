using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using VLF.CLS.Def;

namespace VLF.CLS
{
    /// <summary>
    /// Function of this class is to ensure unique origindatetime stamp according to arrival order from box...
    /// </summary>
    [Serializable]
    public class Sequencer2
    {
        public const short MILLISECOND_STEP = 13;

        Dictionary<long, SequenceData> _BoxList;
        bool _Persist;
        string _DCLName;
        string _PersistantStore = @"C:\SentinelFM\bin\Services\";

        public Sequencer2() : this("UNKNOWN", false) { }


        public Sequencer2(string dclName, bool persist)
        {
            _DCLName = dclName;
            _Persist = persist;
            _BoxList = new Dictionary<long, SequenceData>(15000); //TODO: this needs to be changed to be dynamically loaded/modified
            LoadFromStore();
        }

        public DateTime SetSequenceData(CMFIn cmfIn)
        {
            try
            {
                SequenceData sd = new SequenceData(cmfIn);
                SequenceData last = sd;

                if (!_BoxList.ContainsKey(cmfIn.boxID))
                    _BoxList.Add(cmfIn.boxID, sd);
                else
                {
                    last = _BoxList[cmfIn.boxID];
                    if (last.Timestamp == sd.Timestamp)
                        sd.Shift(last.Sequence);
                    else
                        sd.Sequence = 1;
                    _BoxList[cmfIn.boxID] = sd;
                }
                Trace.WriteLine(string.Format("Sequencer.SetSequenceData: BoxID:[{0}] LAST:[{1}][{2}] CURRENT:[{3}][{4}]", cmfIn.boxID, last.UniqueTimestamp.ToString("MM/dd/yyyy HH:mm:ss.fff"), last.Sequence, sd.UniqueTimestamp.ToString("MM/dd/yyyy HH:mm:ss.fff"), sd.Sequence));
                return sd.UniqueTimestamp;
            }
            catch (Exception exc) { Trace.WriteLine(string.Format("Sequencer.SetSequenceData: ERROR -> {0}", exc.Message)); }
            finally { if (_Persist) new Thread(new ThreadStart(SaveToStore)).Start(); }
            return cmfIn.originatedDateTime;
        }

        void LoadFromStore()
        {
            try
            {
                _PersistantStore = _PersistantStore + _DCLName + ".dat";
                using (FileStream fs = new FileStream(_PersistantStore, FileMode.Open))
                {
                    _BoxList = (Dictionary<long, SequenceData>)new BinaryFormatter().Deserialize(fs);
                }
            }
            catch (FileNotFoundException) { Trace.WriteLine(string.Format("Sequencer.SaveToStore: No data to load for DCL:[{0}] with filename:[{1}]", _DCLName, _PersistantStore)); }
            catch (Exception exc) { Trace.WriteLine(string.Format("Sequencer.SaveToStore: ERROR -> {0}", exc.Message)); }
            finally { }
        }

        void SaveToStore()
        {
            try
            {
                using (FileStream fs = new FileStream(_PersistantStore, FileMode.Create))
                {
                    new BinaryFormatter().Serialize(fs, _BoxList);
                    fs.Flush();
                }
            }
            catch (Exception exc) { Trace.WriteLine(string.Format("Sequencer.SaveToStore: ERROR -> {0}", exc.Message)); }
            finally { }
        }
    }

    [Serializable]
    public class SequenceData
    {
        public DateTime Timestamp;
        public DateTime UniqueTimestamp;
        public short Sequence;

        public SequenceData() : this(DateTime.Parse("01/01/1970"), DateTime.Parse("01/01/1970"), 1) { }

        public SequenceData(DateTime timestamp, DateTime uniqueTimestamp, short sequence)
        {
            Timestamp = timestamp;
            UniqueTimestamp = uniqueTimestamp;
            Sequence = sequence;
        }

        public SequenceData(CMFIn cmfIn)
        {
            UniqueTimestamp = Timestamp = cmfIn.originatedDateTime;
            Sequence = 1;
        }

        public void Shift(short lastSequence)
        {
            if (lastSequence == 0) lastSequence++;
            UniqueTimestamp = UniqueTimestamp.AddMilliseconds(Sequencer2.MILLISECOND_STEP * lastSequence);
            Sequence = lastSequence;
            Sequence++;
        }


    }
}
