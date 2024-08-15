using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Colors;
using BusinessLogicLayer.Viewmodels.Material;
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
    public class MaterialService : IMaterialService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public MaterialService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<bool> CreateAsync(MaterialCreateVM request)
        {
            if (request != null)
            {
                var Obj = new Material()
                {
                    ID = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Status = 1,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now,

                };
                await _dbcontext.Material.AddRangeAsync(Obj);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<MaterialVM>> GetAllActiveAsync()
        {
            var Obj = await _dbcontext.Material.Where(b => b.Status == 1).ToListAsync();
            return _mapper.Map<List<MaterialVM>>(Obj);
        }

        public async Task<List<MaterialVM>> GetAllAsync()
        {
            var Obj = await _dbcontext.Material.ToListAsync();
            return _mapper.Map<List<MaterialVM>>(Obj);
        }

        public async Task<MaterialVM> GetByIDAsync(Guid ID)
        {
            var Obj = await _dbcontext.Material.FindAsync(ID);
            return _mapper.Map<MaterialVM>(Obj);
        }

        public async Task<bool> SetStatus(Guid ID)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbcontext.Material.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null && obj.Status == 1)
                    {
                        obj.Status = 0;

                        _dbcontext.Material.Attach(obj);
                        await _dbcontext.SaveChangesAsync();


                        transaction.Commit();
                        return true;
                    }
                    else if (obj != null && obj.Status == 0)
                    {
                        obj.Status = 1;

                        _dbcontext.Material.Attach(obj);
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

        public async Task<bool> UpdateAsync(Guid ID, MaterialUpdateVM request)
        {
            var Obj = await _dbcontext.Material.FindAsync(ID);
            if (Obj == null)
            {
                return false;
            }

            Obj.Name = request.Name;
            Obj.Description = request.Description;
            Obj.Status = request.Status;
            Obj.ModifiedBy = request.ModifiedBy;
            Obj.ModifiedDate = DateTime.Now;

            _dbcontext.Material.Update(Obj);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
    }
}
