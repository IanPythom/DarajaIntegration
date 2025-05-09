using Microsoft.AspNetCore.Identity;

namespace DarajaAPI.Models.Dto
{
    public class MailRequestDto
    {
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UniversityName { get; set; }
        public string IntakeDate { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string UniversityRegistrationNumber { get; set; }
        public string RoomType { get; set; }
        public DateTime OnboardingDate { get; set; }
        public DateTime JoiningDate { get; set; } = DateTime.Now;
        public int NextOfKinPhoneNumber { get; set; }
        public string NextOfKinRelation { get; set; }
        public DepositStatus Deposit { get; set; }
    }
    public enum DepositStatus
    {
        ReadyToSendDeposit,
        UnableToSendDepositNow
    }
}
