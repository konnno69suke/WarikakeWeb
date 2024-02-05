using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WarikakeWeb.Data;

namespace WarikakeWeb.Entities
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
        public bool m1 { get; set; }
        [Display(Name = "2月")]
        public bool m2 { get; set; }
        [Display(Name = "3月")]
        public bool m3 { get; set; }
        [Display(Name = "4月")]
        public bool m4 { get; set; }
        [Display(Name = "5月")]
        public bool m5 { get; set; }
        [Display(Name = "6月")]
        public bool m6 { get; set; }
        [Display(Name = "7月")]
        public bool m7 { get; set; }
        [Display(Name = "8月")]
        public bool m8 { get; set; }
        [Display(Name = "9月")]
        public bool m9 { get; set; }
        [Display(Name = "10月")]
        public bool m10 { get; set; }
        [Display(Name = "11月")]
        public bool m11 { get; set; }
        [Display(Name = "12月")]
        public bool m12 { get; set; }
        [Display(Name = "第1")]
        public bool r1 { get; set; }
        [Display(Name = "第2")]
        public bool r2 { get; set; }
        [Display(Name = "第3")]
        public bool r3 { get; set; }
        [Display(Name = "第4")]
        public bool r4 { get; set; }
        [Display(Name = "第5")]
        public bool r5 { get; set; }
        [Display(Name = "日")]
        public bool w1 { get; set; }
        [Display(Name = "月")]
        public bool w2 { get; set; }
        [Display(Name = "火")]
        public bool w3 { get; set; }
        [Display(Name = "水")]
        public bool w4 { get; set; }
        [Display(Name = "木")]
        public bool w5 { get; set; }
        [Display(Name = "金")]
        public bool w6 { get; set; }
        [Display(Name = "土")]
        public bool w7 { get; set; }
        [Display(Name = "1")]
        public bool d1 { get; set; }
        [Display(Name = "2")]
        public bool d2 { get; set; }
        [Display(Name = "3")]
        public bool d3 { get; set; }
        [Display(Name = "4")]
        public bool d4 { get; set; }
        [Display(Name = "5")]
        public bool d5 { get; set; }
        [Display(Name = "6")]
        public bool d6 { get; set; }
        [Display(Name = "7")]
        public bool d7 { get; set; }
        [Display(Name = "8")]
        public bool d8 { get; set; }
        [Display(Name = "9")]
        public bool d9 { get; set; }
        [Display(Name = "10")]
        public bool d10 { get; set; }
        [Display(Name = "11")]
        public bool d11 { get; set; }
        [Display(Name = "12")]
        public bool d12 { get; set; }
        [Display(Name = "13")]
        public bool d13 { get; set; }
        [Display(Name = "14")]
        public bool d14 { get; set; }
        [Display(Name = "15")]
        public bool d15 { get; set; }
        [Display(Name = "16")]
        public bool d16 { get; set; }
        [Display(Name = "17")]
        public bool d17 { get; set; }
        [Display(Name = "18")]
        public bool d18 { get; set; }
        [Display(Name = "19")]
        public bool d19 { get; set; }
        [Display(Name = "20")]
        public bool d20 { get; set; }
        [Display(Name = "21")]
        public bool d21 { get; set; }
        [Display(Name = "22")]
        public bool d22 { get; set; }
        [Display(Name = "23")]
        public bool d23 { get; set; }
        [Display(Name = "24")]
        public bool d24 { get; set; }
        [Display(Name = "25")]
        public bool d25 { get; set; }
        [Display(Name = "26")]
        public bool d26 { get; set; }
        [Display(Name = "27")]
        public bool d27 { get; set; }
        [Display(Name = "28")]
        public bool d28 { get; set; }
        [Display(Name = "29")]
        public bool d29 { get; set; }
        [Display(Name = "30")]
        public bool d30 { get; set; }
        [Display(Name = "31")]
        public bool d31 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateUser { get; set; }
        public string? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? UpdatePg { get; set; }

        public List<int> getMonthList()
        {
            List<int> monthList = new List<int>();
            if (m1) monthList.Add(1);
            if (m2) monthList.Add(2);
            if (m3) monthList.Add(3);
            if (m4) monthList.Add(4);
            if (m5) monthList.Add(5);
            if (m6) monthList.Add(6);
            if (m7) monthList.Add(7);
            if (m8) monthList.Add(8);
            if (m9) monthList.Add(9);
            if (m10) monthList.Add(10);
            if (m11) monthList.Add(11);
            if (m12) monthList.Add(12);
            return monthList;
        }
        public bool isWeekly()
        {
            if (r1 || r2 || r3 || r4 || r5)
            {
                if (w1 || w2 || w3 || w4 || w5 || w6 || w7)
                {
                    return true;
                }
            }
            return false;
        }
        public bool isDayly()
        {
            if (d1 || d2 || d3 || d4 || d5 || d6 || d7 || d8 || d9 || d10
                || d11 || d12 || d13 || d14 || d15 || d16 || d17 || d18 || d19 || d20
                || d21 || d22 || d23 || d24 || d25 || d26 || d27 || d28 || d29 || d30 || d31)
            {
                return true;
            }
            return false;
        }
        public List<int> getWeekOfMonthList()
        {
            List<int> weekOfMonthList = new List<int>();
            if (r1) weekOfMonthList.Add(1);
            if (r2) weekOfMonthList.Add(2);
            if (r3) weekOfMonthList.Add(3);
            if (r4) weekOfMonthList.Add(4);
            if (r5) weekOfMonthList.Add(5);
            return weekOfMonthList;
        }
        public List<int> getDayOfWeekList()
        {
            List<int> weekOfWeekList = new List<int>();
            if (w1) weekOfWeekList.Add(0);
            if (w2) weekOfWeekList.Add(1);
            if (w3) weekOfWeekList.Add(2);
            if (w4) weekOfWeekList.Add(3);
            if (w5) weekOfWeekList.Add(4);
            if (w6) weekOfWeekList.Add(5);
            if (w7) weekOfWeekList.Add(6);
            return weekOfWeekList;
        }
        public List<int> getdayList()
        {
            List<int> dayList = new List<int>();
            if (d1) dayList.Add(1);
            if (d2) dayList.Add(2);
            if (d3) dayList.Add(3);
            if (d4) dayList.Add(4);
            if (d5) dayList.Add(5);
            if (d6) dayList.Add(6);
            if (d7) dayList.Add(7);
            if (d8) dayList.Add(8);
            if (d9) dayList.Add(9);
            if (d10) dayList.Add(10);
            if (d11) dayList.Add(11);
            if (d12) dayList.Add(12);
            if (d13) dayList.Add(13);
            if (d14) dayList.Add(14);
            if (d15) dayList.Add(15);
            if (d16) dayList.Add(16);
            if (d17) dayList.Add(17);
            if (d18) dayList.Add(18);
            if (d19) dayList.Add(19);
            if (d20) dayList.Add(20);
            if (d21) dayList.Add(21);
            if (d22) dayList.Add(22);
            if (d23) dayList.Add(23);
            if (d24) dayList.Add(24);
            if (d25) dayList.Add(25);
            if (d26) dayList.Add(26);
            if (d27) dayList.Add(27);
            if (d28) dayList.Add(28);
            if (d29) dayList.Add(29);
            if (d30) dayList.Add(30);
            if (d31) dayList.Add(31);
            return dayList;
        }

        public TDateSubscribe GetDateSubscribe(int SubscribeId)
        {
            TDateSubscribe mwd = _context.TDateSubscribe.Where(d => d.SubscribeId == SubscribeId).FirstOrDefault();
            return mwd;
        }
    }
}
