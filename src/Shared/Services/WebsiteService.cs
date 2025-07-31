using Application.Dtos;
using Application.Enums;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility.AWS;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class WebsiteService : IWebsiteService
    {
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public WebsiteService(IWebsiteRepository websiteRepository, IMapper mapper, IConfiguration configuration)
        {
            _websiteRepository = websiteRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task DeleteByIdAsync(int key)
        {
            var website = await this._websiteRepository.FirstOrDefaultAsync(w => w.Id == key);
            if (website != new Website())
            {
                await _websiteRepository.DeleteAsync(website);
            }
        }

        public async Task<List<WebsiteDto>> GetAllAsync()
        {
            var websites = await this._websiteRepository.GetAllAsync();
            var websitesDto = _mapper.Map<List<WebsiteDto>>(websites);

            websitesDto.ForEach(website =>
            {
                website.LogoSrc = !string.IsNullOrEmpty(website.Logo)
                    ? S3Path.GetS3Url(_configuration, website.Logo, BucketType.SourceDefault)
                    : "";
            });

            return websitesDto;
        }

        public async Task<WebsiteDto> GetByIdAsync(int key)
        {
            var website = await _websiteRepository.FirstOrDefaultAsync(x => x.Id == key);

            return new WebsiteDto
            {
                Id = website.Id
            };
        }

        public Task<WebsiteDto> InsertAsync(WebsiteDto entity)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(WebsiteDto entity)
        {
            var website = await _websiteRepository.FirstOrDefaultAsync(x => x.Id == entity.Id);
            if (website == null)
            {
                await _websiteRepository.AddAsync(_mapper.Map<Website>(entity));
                return;
            }

            website.Address = entity.Address;
            website.Copyright = entity.Copyright;
            website.Email = entity.Email;
            website.Facebook = entity.Facebook;
            website.Fax = entity.Fax;
            website.Location = entity.Location;
            website.Logo = entity.Logo;
            website.Name = entity.Name;
            website.PhoneNumber = entity.PhoneNumber;
            website.Youtube = entity.Youtube;

            await _websiteRepository.UpdateAsync(website);
        }
    }
}