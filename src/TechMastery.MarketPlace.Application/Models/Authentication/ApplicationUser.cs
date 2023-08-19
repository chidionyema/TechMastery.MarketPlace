using System;
namespace TechMastery.MarketPlace.Application.Models.Authentication
{
	public class ApplicationUser
	{
		public ApplicationUser()
		{
		}

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}

