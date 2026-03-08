using BigFitnes.Data;
using BigFitnes.Models;
using Microsoft.EntityFrameworkCore;

namespace BigFitnes.Services;

public class ProfileService
{
    private readonly AppDbContext _db;

    public ProfileService(AppDbContext db) => _db = db;

    public async Task<UserProfile> GetProfile()
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync();
        if (profile is null)
        {
            profile = new UserProfile
            {
                Name = "User",
                Height = 175,
                DailyCalorieGoal = 2000,
                DailyProteinGoal = 150,
                DailyFatGoal = 70,
                DailyCarbGoal = 250,
                Age = 0,
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.Sedentary,
                CalorieGoalMode = CalorieGoalMode.Manual
            };
            _db.UserProfiles.Add(profile);
            await _db.SaveChangesAsync();
        }
        return profile;
    }

    public async Task UpdateProfile(UserProfile profile)
    {
        _db.UserProfiles.Update(profile);
        await _db.SaveChangesAsync();
    }
}
