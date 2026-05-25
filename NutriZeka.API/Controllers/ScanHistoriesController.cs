using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NutriZeka.Application.DTOs;
using NutriZeka.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NutriZeka.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanHistoriesController : ControllerBase
    {
        private readonly IScanHistoryService _scanHistoryService;
        private readonly IMapper _mapper;

        public ScanHistoriesController(IScanHistoryService scanHistoryService, IMapper mapper)
        {
            _scanHistoryService = scanHistoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScanHistoryDto>>> GetAllHistory()
        {
            // Hatırlatma: Az önce Interface ve Service'e eklediğimiz metodu çağırıyoruz
            var dtos = await _scanHistoryService.GetAllHistoryAsync();

            // WebUI tarafındaki JToken kutu açma mantığına uyum sağlamak için "data" zarfına koyuyoruz
            return Ok(new { data = dtos });
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ScanHistoryDto>>> GetUserHistory(Guid userId)
        {
            var dtos = await _scanHistoryService.GetUserHistoryAsync(userId);
            return Ok(dtos);
        }

        [HttpGet("favorites/{userId}")]
        public async Task<ActionResult<IEnumerable<ScanHistoryDto>>> GetFavorites(Guid userId)
        {
            var dtos = await _scanHistoryService.GetFavoritesAsync(userId);
            return Ok(dtos);
        }

        [HttpGet("saved/{userId}")]
        public async Task<ActionResult<IEnumerable<ScanHistoryDto>>> GetSavedProducts(Guid userId)
        {
            var dtos = await _scanHistoryService.GetSavedAsync(userId);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<ScanHistoryDto>> AddOrUpdateHistory([FromBody] ScanHistoryCreateDto dto)
        {
            var result = await _scanHistoryService.AddOrUpdateHistoryAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] ScanHistoryUpdateDto dto)
        {
            await _scanHistoryService.UpdateStatusAsync(id, dto);
            return Ok();
        }

        [HttpDelete("clear-all/{userId}")]
        public async Task<IActionResult> ClearAllHistory(Guid userId)
        {
            await _scanHistoryService.ClearAllHistoryAsync(userId);
            return Ok(new { message = "Tüm geçmişiniz başarıyla temizlendi." });
        }
    }
}