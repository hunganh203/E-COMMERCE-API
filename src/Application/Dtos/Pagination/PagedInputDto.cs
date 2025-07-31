using System;
using System.ComponentModel.DataAnnotations;
using Application.Constants;

namespace Application.DTOs.Pagination
{
    public class PagedInputDto
    {
        [Range(1, AppConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public PagedInputDto()
        {
            MaxResultCount = AppConsts.DefaultPageSize;
        }
    }
}