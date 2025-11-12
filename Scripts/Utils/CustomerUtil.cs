public class CustomerUtil {
    public static Texture2dId GetCustomerTexture(CustomerId customerId, CustomerMood customerMood) {
        return customerId switch {
            CustomerId.RichMale => customerMood switch {
                CustomerMood.Happy => Texture2dId.RichManHappy,
                CustomerMood.Neutral => Texture2dId.RichManNeutral,
                _ => Texture2dId.RichManAngry
            },
            CustomerId.RichFemale => customerMood switch {
                CustomerMood.Happy => Texture2dId.RichWomanHappy,
                CustomerMood.Neutral => Texture2dId.RichWomanNeutral,
                _ => Texture2dId.RichWomanAngry
            },
            CustomerId.RegularMale => customerMood switch {
                CustomerMood.Happy => Texture2dId.AverageManHappy,
                CustomerMood.Neutral => Texture2dId.AverageManNeutral,
                _ => Texture2dId.AverageManAngry
            },
            CustomerId.RegularFemale => customerMood switch {
                CustomerMood.Happy => Texture2dId.AverageWomanHappy,
                CustomerMood.Neutral => Texture2dId.AverageWomanNeutral,
                _ => Texture2dId.AverageWomanAngry
            },
            CustomerId.PoorMale => customerMood switch {
                CustomerMood.Happy => Texture2dId.PoorManHappy,
                CustomerMood.Neutral => Texture2dId.PoorManNeutral,
                _ => Texture2dId.PoorManAngry
            },
            CustomerId.PoorFemale => customerMood switch {
                CustomerMood.Happy => Texture2dId.PoorWomanHappy,
                CustomerMood.Neutral => Texture2dId.PoorWomanNeutral,
                _ => Texture2dId.PoorWomanAngry
            },
            _ => Texture2dId.CustomerPlaceholder
        };
    }
}