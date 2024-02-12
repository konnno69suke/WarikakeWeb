using System.ComponentModel.DataAnnotations;

namespace WarikakeWeb.ViewModel
{
    public class RequestDisp
    {
        public int RequestId { get; set; }
        public int Status { get; set; }
        [Display(Name = "状態")]
        public string StatusName { get; set; }
        public int ReqType { get; set; }
        [Display(Name ="申請種別")]
        public string TypeName { get; set; }
        public int GroupId { get; set; }
        public int ReqUserId { get; set; }
        [Display(Name = "申請者")]
        public string ReqUserName { get; set; }
        public int AgreeUserId { get; set; }
        public string TempKey { get; set; }
        [Display(Name = "期日")]
        public DateTime LimitDate { get; set; }
    }
}
