using System;
namespace TechMastery.UsermanagementService.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class ClientRegistrationDto
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public string ClientSecret { get; set; } = string.Empty;

        [Required]
        [Url]
        public string RedirectUri { get; set; } = string.Empty;
    }

}

