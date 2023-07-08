using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalk.API.Data;
using NZWalk.API.Models.Domain;
using NZWalk.API.Models.DTO;

namespace NZWalk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDBContext dbContext;

        public RegionsController(NZWalksDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        //GET ALL REGIONS
        //http:localhost/api/regions
        [HttpGet]
        public IActionResult GetAll()
        {
            //get data from db to domain
            var regionsDomain = dbContext.Regions.ToList();

            //map domain models to DTO
            var regionsDto = new List<RegionDTO>();
            foreach (var regionDomain in regionsDomain)
            {
                regionsDto.Add(new RegionDTO()
                {
                    Id = regionDomain.Id,
                    Code = regionDomain.Code,
                    Name = regionDomain.Name,
                    RegionImageUrl = regionDomain.RegionImageUrl,
                });
            }

            //return DTOs
            return Ok(regionsDto);
        }

        //GET SINGLE REGION BY ID
        //http:localhost/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id);
            //using Find() only takes primary key as parameter

            //get region domain from db
            var regionDomain = dbContext.Regions.FirstOrDefault(r => r.Id == id);
            if (regionDomain == null)
            {
                return NotFound();
            }
            else
            {
                //map/convert region domain to region DTO
                var regionDto = new RegionDTO
                {
                    Id = regionDomain.Id,
                    Code = regionDomain.Code,
                    Name = regionDomain.Name,
                    RegionImageUrl = regionDomain.RegionImageUrl,
                };
                //return DTO back to client
                return Ok(regionDto);
            }
        }

        //POST

        [HttpPost]
        public IActionResult Create([FromBody] AddRegionRequestDTO addRegionRequestDto)
        {
            //convert DTO to domain model
            var regionDomainModel = new Region
            {
                Code =addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };

            //use domain model to create region
            dbContext.Regions.Add(regionDomainModel);
            dbContext.SaveChanges();

            //map domain model back to DTO
            var regionDto = new RegionDTO
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return CreatedAtAction(nameof(GetById), new {id=regionDomainModel.Id }, regionDto);
        }

        //UPDATEorPUT

        [HttpPut]
        [Route("{id:Guid}")]
        public IActionResult Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDTO updateRegionRequestDto)
        {
            var regionDomainModel=dbContext.Regions.FirstOrDefault(x => x.Id == id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            
            //convert DTO to domain model
            regionDomainModel.Code=updateRegionRequestDto.Code;
            regionDomainModel.Name=updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl=updateRegionRequestDto.RegionImageUrl;

            dbContext.SaveChanges();

            //convert domain to DTO
            var regionDto = new RegionDTO
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return Ok(regionDto);
        }

        //DELETE
        [HttpDelete]
        [Route("{id:Guid}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            var regionDomainModel=dbContext.Regions.FirstOrDefault(x => x.Id == id);

            if(regionDomainModel == null)
            {
                return NotFound();
            }

            //delete region
            dbContext.Regions.Remove(regionDomainModel);
            dbContext.SaveChanges();

            //return deleted region back
            //map domain to DTO
            //optional step in case of delete
            var regionDto = new RegionDTO
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return Ok(regionDto);
        }
    }
}
