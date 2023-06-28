using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicAlbumStore.Areas.Identity.Data;
using MusicAlbumStore.Data;

namespace MusicAlbumStore.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MusicAlbumStoreUser>>();
            IdentityResult roleResult;
            IdentityResult userRoleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            //Add user Role
            var userRoleCheck = await RoleManager.RoleExistsAsync("User");
            if (!userRoleCheck) { userRoleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }

            MusicAlbumStoreUser user = await UserManager.FindByEmailAsync("admin@musicalbumstore.com");
            if (user == null)
            {
                var User = new MusicAlbumStoreUser();
                User.Email = "admin@musicalbumstore.com";
                User.UserName = "admin@musicalbumstore.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MusicAlbumStoreContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<MusicAlbumStoreContext>>()))
            {

                //context.Artist.RemoveRange(context.Artist);
                //context.SaveChanges();

                CreateUserRoles(serviceProvider).Wait();
            }
        }
    }
}
