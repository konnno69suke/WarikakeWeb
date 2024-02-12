namespace WarikakeWeb.Entities
{
    public class TRequest
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int Status { get; set; }
        public int ReqType { get; set; }
        public int GroupId { get; set; }
        public int ReqUserId { get; set; }
        public int AgreeUserId { get; set; }
        public string TempKey { get; set; }
        public DateTime LimitDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String? CreateUser { get; set; }
        public String? CreatePg { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String? UpdateUser { get; set; }
        public String? UpdatePg { get; set; }

        public enum ReqStatusEnum
        {
            申請 = 1,
            承認 = 2,
            却下 = 3,
            使用済 = 8,
            削除 = 9
        }

        public enum ReqTypeEnum
        {
            グループ申請 = 1,
            パスワード申請 = 2,
            一括精算 = 3
        }
    }
}
