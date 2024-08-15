using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Brand;
using BusinessLogicLayer.Viewmodels.Manufacturer;
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
    public class ManufacturerService : IManufacturerService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public ManufacturerService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<List<ManufacturerVM>> GetAllActiveAsync()
        {
            var Obj = await _dbcontext.Manufacturer.Where(b => b.Status == 1).ToListAsync();
            return _mapper.Map<List<ManufacturerVM>>(Obj);
        }
        public async Task<bool> CreateAsync(ManufacturerCreateVM request)
        {
            if (request != null)
            {
                var Obj = new Manufacturer()
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
                await _dbcontext.Manufacturer.AddRangeAsync(Obj);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false;
        }

      

        public async Task<List<ManufacturerVM>> GetAllAsync()
        {
            var Obj = await _dbcontext.Manufacturer.ToListAsync();
            return _mapper.Map<List<ManufacturerVM>>(Obj);
        }

        public async Task<ManufacturerVM> GetByIDAsync(Guid ID)
        {
            var Obj = await _dbcontext.Manufacturer.FindAsync(ID);
            return _mapper.Map<ManufacturerVM>(Obj);
        }

        public async Task<bool> SetStatus(Guid ID)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbcontext.Manufacturer.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null && obj.Status == 1)
                    {
                        obj.Status = 0;

                        _dbcontext.Manufacturer.Attach(obj);
                        await _dbcontext.SaveChangesAsync();


                        transaction.Commit();
                        return true;
                    }
                    else if(obj != null && obj.Status == 0)
                    {
                        obj.Status = 1;

                        _dbcontext.Manufacturer.Attach(obj);
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

        public async Task<bool> UpdateAsync(Guid ID, ManufacturerUpdateVM request)
        {
            var Obj = await _dbcontext.Manufacturer.FindAsync(ID);
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
            _dbcontext.Manufacturer.Update(Obj);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
    }
}
