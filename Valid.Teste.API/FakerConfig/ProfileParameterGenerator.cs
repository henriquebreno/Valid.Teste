using Faker;
using Faker.Extensions;
using System;
using System.Collections.Generic;

namespace Valid.Teste.API.FakerConfig
{
    public class ProfileParameterGenerator
    {
        private static readonly List<string> Keys = new List<string>
        {
            "UserName",
            "CanEdit",
            "CanDelete",          
            "Arn",
            "CreateDate",
            "PasswordLastUsed",
            "Tags",         
            "Groups",        
            "MFADevice",
            "Status"
        };

        public Dictionary<string, object> GenerateProfile()
        {
            var profile = new Dictionary<string, object>();

            foreach (var key in Keys)
            {
                object value = GenerateRandomValueForKey(key);
                profile.Add(key, value);
            }

            return profile;
        }

        private object GenerateRandomValueForKey(string key)
        {
            return key switch
            {
                "UserName" => Internet.UserName(),
                "CanEdit" => Faker.Boolean.Random(),
                "CanDelete" => Faker.Boolean.Random(),
                "Arn" => $"arn:aws:iam::123456789012:user/{Internet.UserName()}",
                "CreateDate" => GenerateRandomDate(),
                "PasswordLastUsed" => GenerateRandomDate(),
                "Tags" =>  (new[] { "Development", "Production", "Staging" }).Random(),
                "Groups" => (new []{ "Admins", "Developers" }).Random(),
                "MFADevice" => "arn:aws:iam::123456789012:mfa/user_mfa",
                "Status" => (new[] { "Active", "Inactive", "Suspended" }).Random(),
                _ => null
            };
        }

        private DateTime GenerateRandomDate()
        {
            var start = new DateTime(2000, 1, 1); 
            var end = DateTime.Now; 
            var range = end - start; 
            var randomValue = new Random().Next(0, (int)range.TotalDays); 
            return start.AddDays(randomValue); 
        }
    }
}
