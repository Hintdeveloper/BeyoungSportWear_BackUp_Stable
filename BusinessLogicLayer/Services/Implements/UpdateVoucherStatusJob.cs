using DataAccessLayer.Application;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Implements
{
    public class UpdateVoucherStatusJob : IJob
    {
        private readonly ApplicationDBContext _dbContext;

        public UpdateVoucherStatusJob(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var currentDate = DateTime.Now;
            var vouchers = await _dbContext.Voucher.ToListAsync();

            foreach (var voucher in vouchers)
            {
                if (currentDate < voucher.StartDate)
                {
                    voucher.IsActive = StatusVoucher.HasntStartedYet;
                }
                else if (currentDate >= voucher.StartDate && currentDate <= voucher.EndDate)
                {
                    voucher.IsActive = StatusVoucher.IsBeginning;
                }
                else if (currentDate > voucher.EndDate)
                {
                    voucher.IsActive = StatusVoucher.Finished;
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
