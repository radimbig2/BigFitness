using BigFitness.Data;
using BigFitness.Models;
using BigFitness.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BigFitness.Services;

public class ProfileService
{
    private readonly AppDbContext _db;

    public ProfileService(AppDbContext db) => _db = db;

    public async Task<UserProfile> GetProfile()
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync();
        if (profile is null)
        {
            profile = new UserProfile(
                name: "User",
                height: 175,
                goalWeight: null,
                age: 0,
                gender: Gender.Male,
                activityLevel: ActivityLevel.Sedentary,
                calorieGoalMode: CalorieGoalMode.Manual,
                dailyCalorieGoal: 2000,
                dailyProteinGoal: 150,
                dailyFatGoal: 70,
                dailyCarbGoal: 250);
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
