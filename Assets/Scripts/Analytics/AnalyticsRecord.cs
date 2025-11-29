using System;
using UnityEngine;

namespace Analytics
{
    [Serializable]
    public class AnalyticsRecord
    {
        public string profile;
        public string evt;
        public string detail;
        public int amount;
        public bool flag;
        public float t;
        public long utcMs;

        public AnalyticsRecord(string profile, string evt, string detail = "", int amount = 0, bool flag = false)
        {
            this.profile = profile;
            this.evt = evt;
            this.detail = detail;
            this.amount = amount;
            this.flag = flag;
            t = Time.time;
            utcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}