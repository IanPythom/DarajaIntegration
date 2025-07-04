﻿using Microsoft.AspNetCore.Identity;

namespace DarajaAPI.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string ?FirstName { get; set; }
        public string ?MiddleName { get; set; }
        public string ?LastName { get; set; }
        public string ?UniversityName { get; set; }
        public string ?IntakeDate { get; set; }
        public string ?PhoneNumber { get; set; }
        public string ?UniversityRegistrationNumber { get; set; }
        public string ?RoomType { get; set; }
        public DateTime ?OnboardingDate { get; set; }
        public DateTime ?JoiningDate { get; set; } = DateTime.Now;
        public int ?NextOfKinPhoneNumber { get; set; }
        public string ?NextOfKinRelation { get; set; }
        public DepositStatus ?Deposit { get; set; }
        public string AccountNumber { get; set; } // e.g., "2173219-USER-123" for subscription tracking
        public DateTime SubscriptionExpiry { get; set; } // Time the subscription expires
        public int Tokens { get; set; } // Number of tokens the user has for scanning/purchasing
    }

    public enum DepositStatus
    {
        ReadyToSendDeposit,
        UnableToSendDepositNow
    }
}
