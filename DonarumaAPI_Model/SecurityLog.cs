using System;
using System.ComponentModel.DataAnnotations;

namespace DonarumaAPI_Model
{
    public class SecurityLog
    {
        [Key]
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public string EventType { get; set; } = string.Empty; 
        public string IpAddress { get; set; } = string.Empty;
    }
}