public record CustomerSaleDto(
    CustomerId customerId,
    CustomerMood customerMood,
    MerchandiseColor colorWanted,
    MerchandiseType merchandiseTypeWanted
);