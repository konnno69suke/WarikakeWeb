using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WarikakeWeb.Data;

namespace WarikakeWeb.Models
{
    public class TDateSubscribe
    {
        WarikakeWebContext _context;

        public TDateSubscribe()
        {
            m1 = true;
            m2 = true;
            m3 = true;
            m4 = true;
            m5 = true;
            m6 = true;
            m7 = true;
            m8 = true;
            m9 = true;
            m10 = true;
            m11 = true;
            m12 = true;
        }

        public TDateSubscribe(WarikakeWebContext context)
        {
            _context = context;
        }

        public int Id { get; set; }
        public int? status { get; set; }
        public int SubscribeId { get; set; }
        public int UserId { get; set; }
        [Display(Name = "1月")]
        public Boolean m1 { get; set; }
        [Display(Name = "2月")]
        public Boolean m2 { get; set; }
        [Display(Name = "3月")]
        public Boolean m3 { get; set; }
        [Display(Name = "4月")]
        public Boolean m4 { get; set; }
        [Display(Name = "5月")]
        public Boolean m5 { get; set; }
        [Display(Name = "6月")]
        public Boolean m6 { get; set; }
        [Display(Name = "7月")]
        public Boolean m7 { get; set; }
        [Display(Name = "8月")]
        public Boolean m8 { get; set; }
        [Display(Name = "9月")]
        public Boolean m9 { get; set; }
        [Display(Name = "10月")]
        public Boolean m10 { get; set; }
        [Display(Name = "11月")]
        public Boolean m11 { get; set; }
        [Display(Name = "12月")]
        public Boolean m12 { get; set; }
        [Display(Name = "第1")]
        public Boolean r1 { get; set; }
        [Display(Name = "第2")]
        public Boolean r2 { get; set; }
        [Display(Name = "第3")]
        public Boolean r3 { get; set; }
        [Display(Name = "第4")]
        public Boolean r4 { get; set; }
        [Display(Name = "第5")]
        public Boolean r5 { get; set; }
        [Display(Name = "日")]
        public Boolean w1 { get; set; }
        [Display(Name = "月")]
        public Boolean w2 { get; set; }
        [Display(Name = "火")]
        public Boolean w3 { get; set; }
        [Display(Name = "水")]
        public Boolean w4 { get; set; }
        [Display(Name = "木")]
        public Boolean w5 { get; set; }
        [Display(Name = "金")]
        public Boolean w6 { get; set; }
        [Display(Name = "土")]
        public Boolean w7 { get; set; }
        [Display(Name = "1")]
        public Boolean d1 { get; set; }
        [Display(Name = "2")]
        public Boolean d2 { get; set; }
        [Display(Name = "3")]
        public Boolean d3 { get; set; }
        [Display(Name = "4")]
        public Boolean d4 { get; set; }
        [Display(Name = "5")]
        public Boolean d5 { get; set; }
        [Display(Name = "6")]
        public Boolean d6 { get; set; }
        [Display(Name = "7")]
        public Boolean d7 { get; set; }
        [Display(Name = "8")]
        public Boolean d8 { get; set; }
        [Display(Name = "9")]
        public Boolean d9 { get; set; }
        [Display(Name = "10")]
        public Boolean d10 { get; set; }
        [Display(Name = "11")]
        public Boolean d11 { get; set; }
        [Display(Name = "12")]
        public Boolean d12 { get; set; }
        [Display(Name = "13")]
        public Boolean d13 { get; set; }
        [Display(Name = "14")]
        public Boolean d14 { get; set; }
        [Display(Name = "15")]
        public Boolean d15 { get; set; }
        [Display(Name = "16")]
        public Boolean d16 { get; set; }
        [Display(Name = "17")]
        public Boolean d17 { get; set; }
        [Display(Name = "18")]
        public Boolean d18 { get; set; }
        [Display(Name = "19")]
        public Boolean d19 { get; set; }
        [Display(Name = "20")]
        public Boolean d20 { get; set; }
        [Display(Name = "21")]
        public Boolean d21 { get; set; }
        [Display(Name = "22")]
        public Boolean d22 { get; set; }
        [Display(Name = "23")]
        public Boolean d23 { get; set; }
        [Display(Name = "24")]
        public Boolean d24 { get; set; }
        [Display(Name = "25")]
        public Boolean d25 { get; set; }
        [Display(Name = "26")]
        public Boolean d26 { get; set; }
        [Display(Name = "27")]
        public Boolean d27 { get; set; }
        [Display(Name = "28")]
        public Boolean d28 { get; set; }
        [Display(Name = "29")]
        public Boolean d29 { get; set; }
        [Display(Name = "30")]
        public Boolean d30 { get; set; }
        [Display(Name = "31")]
        public Boolean d31 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }

        public List<int> getMonthList()
        {
            List<int> monthList = new List<int>();
            if (this.m1) monthList.Add(1);
            if (this.m2) monthList.Add(2);
            if (this.m3) monthList.Add(3);
            if (this.m4) monthList.Add(4);
            if (this.m5) monthList.Add(5);
            if (this.m6) monthList.Add(6);
            if (this.m7) monthList.Add(7);
            if (this.m8) monthList.Add(8);
            if (this.m9) monthList.Add(9);
            if (this.m10) monthList.Add(10);
            if (this.m11) monthList.Add(11);
            if (this.m12) monthList.Add(12);
            return monthList;
        }
        public Boolean isWeekly()
        {
            if (this.r1 || this.r2 || this.r3 || this.r4 || this.r5) { 
                if(this.w1 || this.w2 || this.w3 || this.w4 || this.w5 || this.w6 || this.w7)
                {
                    return true;
                }
            }
            return false;
        }
        public Boolean isDayly()
        {
            if(this.d1 || this.d2 || this.d3 || this.d4 || this.d5 || this.d6 || this.d7 || this.d8 || this.d9 || this.d10
                || this.d11 || this.d12 || this.d13 || this.d14 || this.d15 || this.d16 || this.d17 || this.d18 || this.d19 || this.d20
                || this.d21 || this.d22 || this.d23 || this.d24 || this.d25 || this.d26 || this.d27 || this.d28 || this.d29 || this.d30 || this.d31)
            {
                return true;
            }
            return false;
        }
        public List<int>getWeekOfMonthList()
        {
            List<int> weekOfMonthList = new List<int>();
            if (this.r1) weekOfMonthList.Add(1);
            if (this.r2) weekOfMonthList.Add(2);
            if (this.r3) weekOfMonthList.Add(3);
            if (this.r4) weekOfMonthList.Add(4);
            if (this.r5) weekOfMonthList.Add(5);
            return weekOfMonthList;
        }
        public List<int> getDayOfWeekList()
        {
            List<int> weekOfWeekList = new List<int>();
            if (this.w1) weekOfWeekList.Add(0);
            if (this.w2) weekOfWeekList.Add(1);
            if (this.w3) weekOfWeekList.Add(2);
            if (this.w4) weekOfWeekList.Add(3);
            if (this.w5) weekOfWeekList.Add(4);
            if (this.w6) weekOfWeekList.Add(5);
            if (this.w7) weekOfWeekList.Add(6);
            return weekOfWeekList;
        }
        public List<int> getdayList()
        {
            List<int> dayList = new List<int>();
            if (this.d1) dayList.Add(1);
            if (this.d2) dayList.Add(2);
            if (this.d3) dayList.Add(3);
            if (this.d4) dayList.Add(4);
            if (this.d5) dayList.Add(5);
            if (this.d6) dayList.Add(6);
            if (this.d7) dayList.Add(7);
            if (this.d8) dayList.Add(8);
            if (this.d9) dayList.Add(9);
            if (this.d10) dayList.Add(10);
            if (this.d11) dayList.Add(11);
            if (this.d12) dayList.Add(12);
            if (this.d13) dayList.Add(13);
            if (this.d14) dayList.Add(14);
            if (this.d15) dayList.Add(15);
            if (this.d16) dayList.Add(16);
            if (this.d17) dayList.Add(17);
            if (this.d18) dayList.Add(18);
            if (this.d19) dayList.Add(19);
            if (this.d20) dayList.Add(20);
            if (this.d21) dayList.Add(21);
            if (this.d22) dayList.Add(22);
            if (this.d23) dayList.Add(23);
            if (this.d24) dayList.Add(24);
            if (this.d25) dayList.Add(25);
            if (this.d26) dayList.Add(26);
            if (this.d27) dayList.Add(27);
            if (this.d28) dayList.Add(28);
            if (this.d29) dayList.Add(29);
            if (this.d30) dayList.Add(30);
            if (this.d31) dayList.Add(31);
            return dayList;
        }

        public TDateSubscribe GetDateSubscribe(int SubscribeId)
        {
            TDateSubscribe mwd = _context.TDateSubscribe.Where(d => d.SubscribeId == SubscribeId).FirstOrDefault();
            return mwd;
        }
    }
}
