using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace Nipendo.Evaluation
{
    /// <summary>
    /// The RatingEngine reads the policy application details from a file and produces a numeric 
    /// rating value based on the details.
    /// </summary>
    public class RatingEngine
    {
        public decimal Rating { get; set; }
        public void Rate()
        {
            // log start rating
            Console.WriteLine("Starting rate.");

            Console.WriteLine("Loading policy.");

            // load policy - open file policy.json
            string policyJson = File.ReadAllText("policy.json");

            var policy = JsonConvert.DeserializeObject<Policy>(policyJson,
                new StringEnumConverter());

            switch (policy.Type)
            {
                case PolicyType.Health:
                    Console.WriteLine("Rating HEALTH policy...");
                    Console.WriteLine("Validating policy.");
                    if (String.IsNullOrEmpty(policy.Gender))
                    {
                        Console.WriteLine("Health policy must specify Gender");
                        return;
                    }
                    if (policy.Gender == "Male")
                    {
                        if (policy.Deductible < 500)
                        {
                            Rating = 1000m;
                        }
                        Rating = 900m;
                    }
                    else
                    {
                        if (policy.Deductible < 800)
                        {
                            Rating = 1100m;
                        }
                        Rating = 1000m;
                    }
                    

                    break;

                case PolicyType.Travel:
                    Console.WriteLine("Rating TRAVEL policy...");
                    Console.WriteLine("Validating policy.");
                    if (policy.Days <=0)
                    {
                        Console.WriteLine("Travel policy must specify Days.");
                        return;
                    }
                    if (policy.Days >180)
                    {
                        Console.WriteLine("Travel policy cannot be more then 180 Days.");
                        return;
                    }
                    if (String.IsNullOrEmpty(policy.Country))
                    {
                        Console.WriteLine("Travel policy must specify country.");
                        return;
                    }
                    Rating = policy.Days * 2.5m;
                    if (policy.Country == "Italy")
                    {
                        Rating *= 3;
                    }
                    break;

                case PolicyType.Life:
                    Console.WriteLine("Rating LIFE policy...");
                    Console.WriteLine("Validating policy.");
                    if (policy.DateOfBirth == DateTime.MinValue)
                    {
                        Console.WriteLine("Life policy must include Date of Birth.");
                        return;
                    }
                    if (policy.DateOfBirth < DateTime.Today.AddYears(-100))
                    {
                        Console.WriteLine("Max eligible age for coverage is 100 years.");
                        return;
                    }
                    if (policy.Amount == 0)
                    {
                        Console.WriteLine("Life policy must include an Amount.");
                        return;
                    }
                    
                    int age = DateTime.Today.Year - policy.DateOfBirth.Year;
                    if (policy.DateOfBirth.Month == DateTime.Today.Month &&
                        DateTime.Today.Day < policy.DateOfBirth.Day ||
                        DateTime.Today.Month < policy.DateOfBirth.Month)
                    {
                        age--;
                    }
                    decimal baseRate = policy.Amount * age / 200;
                    if (policy.IsSmoker)
                    {
                        Rating = baseRate * 2;
                        break;
                    }
                    Rating = baseRate;
                    break;

                default:
                    Console.WriteLine("Unknown policy type");
                    break;
            }

            Console.WriteLine("Rating completed.");
        }
    }
}
