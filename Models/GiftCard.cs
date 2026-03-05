using System.ComponentModel.DataAnnotations;

namespace MedicalRecordsManager.Models
{
    public class GiftCard
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        [Required]
        public decimal OriginalAmount { get; set; }

        public decimal RemainingBalance { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; }
    }
}