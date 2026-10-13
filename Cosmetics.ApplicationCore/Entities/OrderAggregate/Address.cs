namespace Cosmetics.ApplicationCore.Entities.OrderAggregate;

/// <summary>
/// Địa chỉ giao hàng - Value Object.
/// </summary>
public class Address
{
    public string ReceiverName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string Ward { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;

    private Address() { }

    public Address(string receiverName, string phoneNumber, string street, string ward, string district, string city)
    {
        ReceiverName = receiverName;
        PhoneNumber = phoneNumber;
        Street = street;
        Ward = ward;
        District = district;
        City = city;
    }

    public string FullAddress => $"{Street}, {Ward}, {District}, {City}";
}
