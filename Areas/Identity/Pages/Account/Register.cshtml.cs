using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MusicQuiz.Core.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MusicQuiz.Application.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using MusicQuiz.Core.Migrations;

namespace MusicQuiz.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UserData> _signInManager;
        private readonly UserManager<UserData> _userManager;
        private readonly IUserStore<UserData> _userStore;
        private readonly IUserEmailStore<UserData> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager">User manager service</param>
        /// <param name="userStore">User store service</param>
        /// <param name="signInManager">Sign-in manager service</param>
        /// <param name="logger">Logger</param>
        /// <param name="context">Database context</param>
        /// <param name="emailSender">Email sender service</param>
        public RegisterModel(
            UserManager<UserData> userManager,
            IUserStore<UserData> userStore,
            SignInManager<UserData> signInManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Input model
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            StudentID = string.Empty,
            Email = string.Empty,
            Password = string.Empty,
            ConfirmPassword = string.Empty,
            AcademicYear = string.Empty
        };

        /// <summary>
        /// Return URL
        /// </summary>
        public string ReturnUrl { get; set; } = string.Empty;

        /// <summary>
        /// External logins
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; } = [];

        /// <summary>
        /// Academic year options
        /// </summary>
        public List<string> AcademicYearOptions { get; set; } = [];

        /// <summary>
        /// Input model
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Forename
            /// </summary>
            [Required(ErrorMessage = "First name is required")]
            [Display(Name = "First Name")]
            [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
            public required string FirstName { get; set; }

            /// <summary>
            /// Surname
            /// </summary>
            [Required(ErrorMessage = "Last name is required")]
            [Display(Name = "Last Name")]
            [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
            public required string LastName { get; set; }

            /// <summary>
            /// Student ID
            /// </summary>
            [Required(ErrorMessage = "Student ID is required")]
            [Display(Name = "Student ID")]
            [StringLength(20, ErrorMessage = "Student ID cannot exceed 20 characters")]
            public required string StudentID { get; set; }

            /// <summary>
            /// Academic year
            /// </summary>
            [Required(ErrorMessage = "Please select an academic year")]
            [Display(Name = "Academic Year")]
            public string? AcademicYear { get; set; }

            /// <summary>
            /// Email
            /// </summary>
            [Required(ErrorMessage = "Email address is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public required string Email { get; set; }

            /// <summary>
            /// Password
            /// </summary>
            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public required string Password { get; set; }

            /// <summary>
            /// Confirm password
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
            public required string ConfirmPassword { get; set; }
        }

        /// <summary>
        /// Getting list of academic years, this year, the previous year and next
        /// This is more of a just-in-case rather than necesscary
        /// The users of the app will tpically be for the modue in that year.
        /// They will need to change this in the user section if they use this application
        /// for more than an academic year as this will be used for leaderboards and assessments
        /// </summary>
        /// <returns>List of academic year options</returns>
        public List<string> GetAcademicYearOptions()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            // Adjust the logic to consider the current academic year as 24/25 for Sept - Aug
            var currentAcademicYear = (currentMonth >= 9 && currentMonth <= 12)
                ? currentYear
                : (currentMonth >= 1 && currentMonth <= 8) ? currentYear - 1 : currentYear;

            // Generate the correct academic year options
            var options = new List<string>
            {
                // Previous academic year
                $"{(currentAcademicYear - 1) % 100}/{currentAcademicYear % 100}",

                // Current academic year
                $"{currentAcademicYear % 100}/{(currentAcademicYear + 1) % 100}",

                // Next academic year
                $"{(currentAcademicYear + 1) % 100}/{(currentAcademicYear + 2) % 100}"
            };

            return options;
        }

        /// <summary>
        /// On get async
        /// </summary>
        /// <param name="returnUrl">Return URL after registration</param>
        /// <returns>Task</returns>
        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = [.. (await _signInManager.GetExternalAuthenticationSchemesAsync())];

            // Populate the Academic Year options
            AcademicYearOptions = GetAcademicYearOptions();
        }

        /// <summary>
        /// On post async
        /// </summary>
        /// <param name="returnUrl">Return URL after registration</param>
        /// <returns>Task with IActionResult</returns>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = [.. (await _signInManager.GetExternalAuthenticationSchemesAsync())];
            AcademicYearOptions = GetAcademicYearOptions();

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the email is already in use
                    var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "This email is already taken. If it's your account, please use the 'Forgot your password?' link.");
                        return Page();
                    }

                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                    // Set the Forename, Surname, StudentID, and AcademicYear
                    user.FirstName = Input.FirstName;
                    user.LastName = Input.LastName;
                    user.StudentID = Input.StudentID;
                    user.AcademicYear = Input.AcademicYear ?? "Unknown";

                    // Increment the UserID
                    var lastUserID = await _context.LastAssignedUserID.FirstOrDefaultAsync();
                    if (lastUserID == null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to retrieve the last assigned user ID.");
                        return Page();
                    }

                    // Set the new UserID and update the LastAssignedUserID
                    user.IntID = lastUserID.LastUserID + 1;
                    lastUserID.LastUserID = user.IntID;

                    // Set LastLoggedIn field to DateTime.Now
                    user.LastLoggedIn = DateTime.Now;

                    // Save the updated LastAssignedUserID
                    _context.LastAssignedUserID.Update(lastUserID);
                    await _context.SaveChangesAsync();

                    // Create the user with password
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {Email} created a new account with password.", Input.Email);

                        // Assign the "User" role to the new user
                        var roleResult = await _userManager.AddToRoleAsync(user, "User");
                        if (!roleResult.Succeeded)
                        {
                            _logger.LogWarning("Failed to assign User role to user {Email}.", Input.Email);
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return Page();
                        }

                        // Generate the email confirmation token AFTER the user is created
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId, code, returnUrl },
                            protocol: Request.Scheme);

                        try
                        {
                            await _emailSender.SendEmailAsync(
                                Input.Email,
                                "Confirm your Music Quiz account",
                                $"<h2>Welcome to Music Quiz!</h2>" +
                                $"<p>Thank you for registering. Please confirm your account by " +
                                $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</p>" +
                                $"<p>If you did not create this account, you can ignore this email.</p>"
                            );

                            _logger.LogInformation("Verification email sent to {Email}", Input.Email);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send confirmation email to {Email}", Input.Email);
                            // Continue with registration even if email fails
                        }

                        // Update the user in the database
                        await _context.SaveChangesAsync();

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during user registration");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred during registration. Please try again.");
                }
            }

            // If we got here, something failed. Repopulate the AcademicYearOptions again just in case.
            AcademicYearOptions = GetAcademicYearOptions();

            return Page();
        }

        /// <summary>
        /// Create a new instance of UserData
        /// </summary>
        /// <returns>New UserData instance</returns>
        /// <exception cref="InvalidOperationException">Thrown if UserData cannot be created</exception>
        private UserData CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UserData>() ?? throw new InvalidOperationException($"Can't create an instance of '{nameof(UserData)}'.");
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserData)}'. Ensure that '{nameof(UserData)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        /// <summary>
        /// Get the email store
        /// </summary>
        /// <returns>Email store instance</returns>
        /// <exception cref="NotSupportedException">Thrown if email store is not supported</exception>
        /// <exception cref="InvalidOperationException">Thrown if user store cannot be cast to email store</exception>
        private IUserEmailStore<UserData> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return _userStore as IUserEmailStore<UserData> ?? throw new InvalidOperationException("User store does not support email.");
        }
    }
}

