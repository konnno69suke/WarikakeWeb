using Microsoft.Identity.Client;
using WarikakeWeb.Data;
using WarikakeWeb.Models;

namespace WarikakeWeb.Logic
{
    public class ModelLogic
    {
        WarikakeWebContext _context;
        public ModelLogic(WarikakeWebContext context) {
            _context = context;   
        }

        public TCost GetCurrentCost(int UserId, DateTime currTime, String pg)
        {
            TCost currCost = _context.TCost.Where(c => c.UpdateUser.Equals(UserId.ToString()) && c.UpdatedDate.Equals(currTime) && c.UpdatePg.Equals(pg)).OrderByDescending(a => a.CostId).FirstOrDefault();

            return currCost;
        }

        public TCostSubscribe GetCurrentSubscribe(int UserId, DateTime currTime, String pg)
        {
            TCostSubscribe currSubscribe = _context.TCostSubscribe.Where(c => c.UpdateUser.Equals(UserId.ToString()) && c.UpdatedDate.Equals(currTime) && c.UpdatePg.Equals(pg)).OrderByDescending(a => a.SubscribeId).FirstOrDefault();

            return currSubscribe;
        }
    }
}
