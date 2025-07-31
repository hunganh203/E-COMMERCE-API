using Application.Dtos;
using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using Ecommerce.Service;

namespace Shared.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _galleryRepository;

        public GalleryService(IGalleryRepository galleryRepository)
        {
            _galleryRepository = galleryRepository;
        }

        public async Task DeleteById(int key, string? userSession = null)
        {
            var gallery = await _galleryRepository.FirstOrDefaultAsync(g => g.Id == key);

            if (gallery != new Gallery())
            {
                await _galleryRepository.DeleteAsync(gallery);
            }
        }

        public async Task<List<GalleryDto>> GetAll()
        {
            return (await _galleryRepository.GetAllAsync()).Select(x => new GalleryDto()
            {
                Id = x.Id,
                Image = x.Image,
                Type = x.Type
            }).ToList();
        }

        public async Task<GalleryDto> GetById(int key)
        {
            return (await _galleryRepository.GetAllAsync())
                .Where(x => x.Id == key)
                .Select(x => new GalleryDto()
                {
                    Id = x.Id,
                    Image = x.Image,
                    Type = x.Type
                }).FirstOrDefault() ?? new GalleryDto();
        }

        public async Task<GalleryDto> Insert(GalleryDto entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Image))
            {
                if (entity.Image.Contains("data:image/png;base64,"))
                {
                    var path = "";
                    var imgName = Guid.NewGuid().ToString("N") + ".png";
                    var bytes = Convert.FromBase64String(entity.Image.Replace("data:image/png;base64,", ""));
                    await using (var imageFile = new FileStream(path + "/" + imgName, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                    entity.Image = imgName;
                }
            }

            var gallery = new Gallery()
            {
                Image = entity.Image,
                Type = entity.Type
            };

            await _galleryRepository.AddAsync(gallery);

            return entity;
        }

        public async Task Update(int key, GalleryDto entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Image))
            {
                if (entity.Image.Contains("data:image/png;base64,"))
                {
                    var path = "";
                    var imgName = Guid.NewGuid().ToString("N") + ".png";
                    var bytes = Convert.FromBase64String(entity.Image.Replace("data:image/png;base64,", ""));
                    await using (var imageFile = new FileStream(path + "/" + imgName, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                    entity.Image = imgName;
                }
            }

            var gallery = await _galleryRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (gallery != new Gallery())
            {
                await this._galleryRepository.UpdateAsync(gallery);
            }
        }
    }
}