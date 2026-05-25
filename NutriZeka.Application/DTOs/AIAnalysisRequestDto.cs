using System;
using System.ComponentModel.DataAnnotations;

namespace NutriZeka.Application.DTOs
{
    public class AIAnalysisRequestDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public Guid ScanHistoryId { get; set; }

        [Required]
        public int QuestionType { get; set; } // 1: Safe?, 2: Ingredients?, 3: Alternatives?
    }
}