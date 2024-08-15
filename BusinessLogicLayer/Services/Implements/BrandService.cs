using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Brand;
using BusinessLogicLayer.Viewmodels.Sizes;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Implements
{
    public class BrandService : IBrandService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public BrandService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<bool> CreateAsync(BrandCreateVM request)
        {
            if (request != null)
            {
                var Obj = new Brand()
                {
                    ID = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Address = request.Address,
                    Phone = request.Phone,
                    Gmail = request.Gmail,
                    Website = request.Website,
                    Status = 1,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now,

                };
                await _dbcontext.Brand.AddRangeAsync(Obj);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<BrandVM>> GetAllActiveAsync()
        {
            var Obj = await _dbcontext.Brand.Where(b => b.Status == 1).ToListAsync();
            return _mapper.Map<List<BrandVM>>(Obj);
        }

        public async Task<List<BrandVM>> GetAllAsync()
        {
            var Obj = await _dbcontext.Brand.ToListAsync();
            return _mapper.Map<List<BrandVM>>(Obj);
        }

        public async Task<BrandVM> GetByIDAsync(Guid ID)
        {
            var Obj = await _dbcontext.Brand.FindAsync(ID);
            return _mapper.Map<BrandVM>(Obj);

        }

        public async Task<bool> SetStatus(Guid ID)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbcontext.Brand.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null && obj.Status == 1)
                    {
                        obj.Status = 0;

                        _dbcontext.Brand.Attach(obj);
                        await _dbcontext.SaveChangesAsync();


                        transaction.Commit();
                        return true;
                    }
                    else if(obj != null && obj.Status == 0)
                    {
                        obj.Status = 1;

                        _dbcontext.Brand.Attach(obj);
                        await _dbcontext.SaveChangesAsync();


                        transaction.Commit();
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> UpdateAsync(Guid ID, BrandUpdateVM request)
        {
            var Obj = await _dbcontext.Brand.FindAsync(ID);
            if (Obj == null)
            {
                return false;
            }

            Obj.Name = request.Name;
            Obj.Description = request.Description;
            Obj.Status = request.Status;
            Obj.Address = request.Address;
            Obj.Phone = request.Phone;
            Obj.Gmail = request.Gmail;
            Obj.Website = request.Website;
            Obj.ModifiedBy = request.ModifiedBy;
            Obj.ModifiedDate = DateTime.Now;
            _dbcontext.Brand.Update(Obj);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
    }
}
