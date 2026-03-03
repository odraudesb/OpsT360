using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class ReeferMonitoringSupply
    {
        public string UNIT { get; set; }
        public string TIME_OF_RECORDING { get; set; }
        public string SUPPLY_TMP { get; set; }
        public string HUMIDITY_PCT { get; set; }
        public string TMP_SET_PT { get; set; }
        public string VENT_SETTING { get; set; }
        public string O2_PCT { get; set; }
        public string REC_FAULT_CODE { get; set; }

        public ReeferMonitoringSupply(string _UNIT, string _TIME_OF_RECORDING,  string _HUMIDITY_PCT, string _SUPPLY_TMP, string _TMP_SET_PT, string _VENT_SETTING, string _O2_PCT, string _REC_FAULT_CODE)
        {
            this.UNIT = _UNIT.Trim();
            this.TIME_OF_RECORDING = _TIME_OF_RECORDING.Trim();
            this.SUPPLY_TMP = _SUPPLY_TMP.Trim();
            this.HUMIDITY_PCT = _HUMIDITY_PCT.Trim();
            this.TMP_SET_PT = _TMP_SET_PT.Trim();
            this.VENT_SETTING = _VENT_SETTING.Trim();
            this.O2_PCT = _O2_PCT.Trim();
            this.REC_FAULT_CODE = _REC_FAULT_CODE.Trim();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<argo:reefer-monitor xmlns:argo=\"http://www.navis.com/argo\"><update-reefers><reefer-monitoring>");

            sb.AppendFormat("<unit-identity id=\"{0}\"/>", UNIT);
            sb.AppendFormat("<reefer-recording-histories update-mode=\"MERGE\">");
            sb.AppendFormat("<reefer-recording-history time-of-recording=\"{0}\" humidity-pct=\"{1}\" supply-tmp=\"{2}\" tmp-set-pt=\"{3}\" vent-setting=\"{4}\" o2-pct=\"{5}\" rec-fault-code=\"{6}\" />",
                TIME_OF_RECORDING, HUMIDITY_PCT, SUPPLY_TMP, TMP_SET_PT, VENT_SETTING, O2_PCT, REC_FAULT_CODE);
            sb.Append("</reefer-recording-histories></reefer-monitoring></update-reefers></argo:reefer-monitor>");
            return sb.ToString();
        }
    }
}
