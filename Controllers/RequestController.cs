using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarikakeWeb.Data;
using WarikakeWeb.Models;
using WarikakeWeb.ViewModel;
using static WarikakeWeb.Entities.TRequest;

namespace WarikakeWeb.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly WarikakeWebContext _context;

        public RequestController(WarikakeWebContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            if (UserId == null || GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // DB検索
            RequestModel model = new RequestModel(_context);
            List<RequestDisp> requestList = model.searchGroupRequest((int)GroupId, (int)UserId);


            return View(requestList);
        }

        public ActionResult Agreement(int Id)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            if (UserId == null || GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // DB検索
            RequestModel model = new RequestModel(_context);
            RequestDisp requestDisp = model.GetRequestDisp(Id, (int) GroupId, (int)UserId);

            return View(requestDisp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Agreement(RequestDisp input)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            if (UserId == null || GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            // 存在チェック
            // セキュリティを考慮し入力内容はrequestid以外は信用せず、グループ、ユーザーが適切かを再判定する
            RequestModel model = new RequestModel(_context);
            RequestDisp requestDisp = model.GetRequestDisp(input.RequestId, (int)GroupId, (int)UserId);
            if (requestDisp == null)
            {
                return NotFound();
            }

            try
            {
                // DB更新
                switch (requestDisp.ReqType)
                {
                    case (int)ReqTypeEnum.グループ申請:
                        // グループにメンバーを追加
                        model.AddGroupMember(requestDisp, (int)GroupId, (int)UserId);
                        break;
                    case (int)ReqTypeEnum.パスワード申請:
                        // 特に何もしない
                        break;
                    default: break;
                }

                // ステータス更新
                model.ChangeRequestStatus(input.RequestId, (int)UserId, (int)ReqStatusEnum.承認);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(input);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deny(RequestDisp input)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? GroupId = HttpContext.Session.GetInt32("GroupId");
            if (UserId == null || GroupId == null)
            {
                // セッション切れ
                return RedirectToAction("Login");
            }
            Serilog.Log.Information($"GroupId:{GroupId}, UserId:{UserId}");

            try
            {
                // DB更新
                RequestModel model = new RequestModel(_context);
                model.ChangeRequestStatus(input.RequestId, (int)UserId, (int)ReqStatusEnum.却下);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
