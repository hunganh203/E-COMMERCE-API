using Application.Dtos;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using AutoMapper;

namespace Shared.Services
{
    public class AttributeService : IAttributeService
    {
        private readonly IAttributeRepository _attributeRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IMapper _mapper;

        public AttributeService(IAttributeRepository attributeRepository, IProductAttributeRepository productAttributeRepository, IMapper mapper)
        {
            _attributeRepository = attributeRepository;
            _productAttributeRepository = productAttributeRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Delete Attribute
        /// </summary>
        /// <param name="key"></param>
        public async Task DeleteById(int key)
        {
            var productAttribute = await _productAttributeRepository.FirstOrDefaultAsync(x => x.AttributeId == key);
            if (productAttribute != null)
                throw new ArgumentException("HAS_USED");

            var attribute = await _attributeRepository.FirstOrDefaultAsync(x => x.Id == key);

            if (attribute == null)
                throw new ArgumentException("ATTRIBUTE_DOES_NOT_EXIST");
            await _attributeRepository.DeleteAsync(attribute);
        }

        /// <summary>
        /// Get attribute by key
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        public async Task<List<AttributeDto>> Get(string? keySearch)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            var attributes = await Task.Run(() => _attributeRepository.Query(x => keySearch == null || x.Name.Contains(keySearch)));

            return _mapper.Map<List<AttributeDto>>(attributes);
        }

        /// <summary>
        /// Get all attributes
        /// </summary>
        /// <returns></returns>
        public async Task<List<AttributeDto>> GetAll()
        {
            var attributes = await _attributeRepository.GetAllAsync();
            return _mapper.Map<List<AttributeDto>>(attributes);
        }

        /// <summary>
        ///  Get attribute by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<AttributeDto> GetById(int key)
        {
            var attribute = await _attributeRepository.FirstOrDefaultAsync(x => x.Id == key);
            return _mapper.Map<AttributeDto>(attribute);
        }

        /// <summary>
        /// Add attribute
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AttributeDto> Insert(AttributeDto entity)
        {
            var attribute = new Domain.Entities.Attribute
            {
                Name = entity.Name
            };
            await _attributeRepository.AddAsync(attribute);

            return entity;
        }

        /// <summary>
        /// Update Attribute
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        public async Task Update(int key, AttributeDto entity)
        {
            var attribute = await _attributeRepository.FirstOrDefaultAsync(x => x.Id == key);

            if (attribute == null)
                throw new ArgumentNullException();

            attribute.Name = entity.Name;
            await _attributeRepository.UpdateAsync(attribute);
        }
    }
}