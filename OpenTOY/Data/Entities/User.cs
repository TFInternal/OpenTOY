using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OpenTOY.Data.Entities;

[PrimaryKey(nameof(Id), nameof(ServiceId))]
public class UserEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public MembershipType MembershipType { get; set; }
}

[PrimaryKey(nameof(ServiceId), nameof(Email))]
public class EmailAccountEntity
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    [ForeignKey("Id,ServiceId")]
    public UserEntity? User { get; set; }
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Salt { get; set; } = string.Empty;
}

[PrimaryKey(nameof(ServiceId), nameof(DeviceId))]
public class GuestAccountEntity
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    [ForeignKey("Id,ServiceId")]
    public UserEntity? User { get; set; }
    // On Android, this is https://developer.android.com/reference/android/provider/Settings.Secure#ANDROID_ID
    // I don't know what TOY uses on iOS so the length may need to be adjusted
    [MaxLength(16)]
    public string DeviceId { get; set; } = string.Empty;
}

public enum MembershipType
{
    Email = 4,
    Guest = 9999
}