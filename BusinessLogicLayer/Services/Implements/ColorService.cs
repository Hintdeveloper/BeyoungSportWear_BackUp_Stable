using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Colors;
using BusinessLogicLayer.Viewmodels.Sizes;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Implements
{
    public class ColorService : IColorService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public ColorService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<bool> CreateAsync(ColorCreateVM request)
        {
            if (request != null)
            {
                var Obj = new Colors()
                {
                    ID = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Status = 1,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now,

                };
                await _dbcontext.Colors.AddRangeAsync(Obj);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ColorVM>> GetAllActiveAsync()
        {
            var Obj = await _dbcontext.Colors.Where(b => b.Status == 1).ToListAsync();
            return _mapper.Map<List<ColorVM>>(Obj);
        }

        public async Task<List<ColorVM>> GetAllAsync()
        {
            var Obj = await _dbcontext.Colors.ToListAsync();
            return _mapper.Map<List<ColorVM>>(Obj);
        }

        public async Task<ColorVM> GetByIDAsync(Guid ID)
        {
            var Obj = await _dbcontext.Colors.FindAsync(ID);
            return _mapper.Map<ColorVM>(Obj);
        }

        public async Task<bool> SetStatus(Guid ID)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbcontext.Colors.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null && obj.Status == 1)
                    {
                        obj.Status = 0;

                        _dbcontext.Colors.Attach(obj);
                        await _dbcontext.SaveChangesAsync();


                        transaction.Commit();
                        return true;
                    }
                    else if(obj != null && obj.Status == 0)
                    {
                        obj.Status = 1;

                        _dbcontext.Colors.Attach(obj);
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

        public async Task<bool> UpdateAsync(Guid ID, ColorUpdateVM request)
        {
            var Obj = await _dbcontext.Colors.FindAsync(ID);
            if (Obj == null)
            {
                return false;
            }

            Obj.Name = request.Name;
            Obj.Description = request.Description;
            Obj.Status = request.Status;
            Obj.ModifiedBy = request.ModifiedBy;
            Obj.ModifiedDate = DateTime.Now;

            _dbcontext.Colors.Update(Obj);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
    }
}
