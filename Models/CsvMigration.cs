using Microsoft.EntityFrameworkCore;
using System.Composition;
using System.Text;
using System.Text.RegularExpressions;
using WarikakeWeb.Data;

namespace WarikakeWeb.Models
{
    public class CsvMigration
    {
        private readonly WarikakeWebContext _context;

        public CsvMigration()
        {

        }

        public CsvMigration(WarikakeWebContext context)
        {
            _context = context;
        }


        public int Id { get; set; }
        public int status { get; set; }
        public String inputDate { get; set; }
        public String buyDate { get; set; }
        public String kindName { get; set; }
        public String buyAmount { get; set; }
        public String pf1 { get; set; }
        public String pf2 { get; set; }
        public String pf3 { get; set; }
        public String pa1 { get; set; }
        public String pa2 { get; set; }
        public String pa3 { get; set; }
        public String pr1 { get; set; }
        public String pr2 { get; set; }
        public String pr3 { get; set; }
        public String buyStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }

        // CSVエクスポート用情報を取得
        public List<CsvMigration> GetExportData(int GroupId)
        {
            Serilog.Log.Information($"SQL param:{GroupId}");
            List<CsvMigration> csvMigrations = _context.CsvMigration.FromSql($@"
                with paydata as (select costid,
                                max(case when payid = 0 then payamount end) as pay1,
                                max(case when payid = 1 then payamount end) as pay2,
                                max(case when payid = 2 then payamount end) as pay3
                                from tpay
                                group by costid),
                    repaydata as (select costid,
                                max(case when repayid = 0 then repayamount end) as repay1,
                                max(case when repayid = 1 then repayamount end) as repay2,
                                max(case when repayid = 2 then repayamount end) as repay3
                                from trepay
                                group by costid)
                select tc.id, tc.status, 
                       format(tc.createddate, N'yyyy年MM月dd日HH時mm分ss秒') inputDate, format(tc.costdate, 'yyyyMMdd') buyDate, tc.genrename kindName, convert(varchar, tc.costamount) buyAmount, 
                       convert(varchar, isnull(tp.pay1,0)) pf1, convert(varchar, isnull(tp.pay2,0)) pf2, convert(varchar, isnull(tp.pay3,0)) pf3,  
                       convert(varchar, isnull(tr.repay1, 0)) pa1, convert(varchar, isnull(tr.repay2, 0)) pa2, convert(varchar, isnull(tr.repay3, 0)) pa3, 
                       '0' pr1, '0' pr2, '0' pr3, convert(varchar, tc.status) buyStatus, 
                       tc.createddate, tc.createuser, tc.createpg, tc.updateddate, tc.updateuser, tc.updatepg
                from tcost tc
                inner join paydata tp on tc.costid = tp.costid
                inner join repaydata tr on tc.costid = tr.costid
                where tc.groupid = {GroupId}
                order by tc.costdate, tc.costid
                ").ToList();

            return csvMigrations;
        }

        // DBから取得したデータをcsvフォーマットの文字列に変換
        public string GetExportString(List<CsvMigration> csvMigrations, Boolean hasQuote)
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendLine("#入力日時,購入日,品目,金額,DB立替,RM立替,FN立替,DB分担,RM分担,FN分担,DB端数,RM端数,FN端数,精算状況");
            if (hasQuote)
            {
                csvMigrations.ForEach(a => sb.AppendLine(CreateCsvBodyWithQuote(a)));
            }
            else
            {
                csvMigrations.ForEach(a => sb.AppendLine(CreateCsvBodyNoQuote(a)));
            }
            return sb.ToString();

        }
        private static string CreateCsvBodyWithQuote(CsvMigration a)
        {
            WarikakeDisp wari = new WarikakeDisp();
            var sb = new StringBuilder();
            sb.Append(string.Format($@"""{a.inputDate}"","));
            sb.Append(string.Format($@"""{a.buyDate}"","));
            sb.Append(string.Format($@"""{a.kindName}"","));
            sb.Append(string.Format($@"""{a.buyAmount}"","));
            sb.Append(string.Format($@"""{a.pf1}"","));
            sb.Append(string.Format($@"""{a.pf2}"","));
            sb.Append(string.Format($@"""{a.pf3}"","));
            sb.Append(string.Format($@"""{a.pa1}"","));
            sb.Append(string.Format($@"""{a.pa2}"","));
            sb.Append(string.Format($@"""{a.pa3}"","));
            sb.Append(string.Format($@"""{a.pr1}"","));
            sb.Append(string.Format($@"""{a.pr2}"","));
            sb.Append(string.Format($@"""{a.pr3}"","));
            sb.Append(string.Format($@"""{(statusEnum)int.Parse(a.buyStatus)}"""));
            return sb.ToString();
        }

        private static string CreateCsvBodyNoQuote(CsvMigration a)
        {
            WarikakeDisp wari = new WarikakeDisp();
            var sb = new StringBuilder();
            sb.Append(string.Format($@"{a.inputDate},"));
            sb.Append(string.Format($@"{a.buyDate},"));
            sb.Append(string.Format($@"{a.kindName},"));
            sb.Append(string.Format($@"{a.buyAmount},"));
            sb.Append(string.Format($@"{a.pf1},"));
            sb.Append(string.Format($@"{a.pf2},"));
            sb.Append(string.Format($@"{a.pf3},"));
            sb.Append(string.Format($@"{a.pa1},"));
            sb.Append(string.Format($@"{a.pa2},"));
            sb.Append(string.Format($@"{a.pa3},"));
            sb.Append(string.Format($@"{a.pr1},"));
            sb.Append(string.Format($@"{a.pr2},"));
            sb.Append(string.Format($@"{a.pr3},"));
            sb.Append(string.Format($@"{(statusEnum)int.Parse(a.buyStatus)}"));
            return sb.ToString();
        }
    }
}
