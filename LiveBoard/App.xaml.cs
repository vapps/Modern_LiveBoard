using System.Threading.Tasks;
using Windows.System;
using Windows.UI.ApplicationSettings;
using LiveBoard.Common;

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226
using LiveBoard.View;
using Microsoft.Live;

namespace LiveBoard
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		/// <summary>
		/// Initializes the singleton Application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();
			this.Suspending += OnSuspending;
		}

		public static LiveConnectSession Session { get; set; }

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used when the application is launched to open a specific file, to display
		/// search results, and so forth.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override async void OnLaunched(LaunchActivatedEventArgs args)
		{
			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active

			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();
				//Associate the frame with a SuspensionManager key                                
				SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

				if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					// Restore the saved session state only when appropriate
					try
					{
						await SuspensionManager.RestoreAsync();
					}
					catch (SuspensionManagerException)
					{
						//Something went wrong restoring state.
						//Assume there is no state and continue
					}
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
			}
			if (rootFrame.Content == null)
			{
				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter
				if (!rootFrame.Navigate(typeof(StartPage)))
				{
					throw new Exception("Failed to create initial page");
				}
			}
			// Ensure the current window is active
			Window.Current.Activate();
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private async void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			await SuspensionManager.SaveAsync();
			deferral.Complete();
		}

		protected override void OnWindowCreated(WindowCreatedEventArgs args)
		{
			SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
		}

		/// <summary>
		// Handle file activations.
		/// </summary>
		protected override async void OnFileActivated(FileActivatedEventArgs args)
		{
			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active

			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();
				// Associate the frame with a SuspensionManager key
				SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

				if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					// Restore the saved session state only when appropriate
					try
					{
						await SuspensionManager.RestoreAsync();
					}
					catch (SuspensionManagerException)
					{
						//Something went wrong restoring state.
						//Assume there is no state and continue
					}
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null)
			{
				if (!rootFrame.Navigate(typeof(StartPage)))
				{
					throw new Exception("Failed to create initial page");
				}
			}

			var p = rootFrame.Content as StartPage;
			if (p != null)
			{
				p.FileEvent = args;
				p.ProtocolEvent = null;
				await p.NavigateToFilePage();
			}
			// Ensure the current window is active
			Window.Current.Activate();
		}
		private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
		{
			// 언어 리소스 로더.
			var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
			// 설정 참바.

			//args.Request.ApplicationCommands.Add(new SettingsCommand("account", "Account", (handler) =>
			//{
			//	var accountFlyout = new AccountSettingsFlyout();
			//	accountFlyout.Show();
			//}));
			args.Request.ApplicationCommands.Add(new SettingsCommand(
				"PrivacySettingsCommand", "Privacy Policy", handler =>
				{
					var customSettingFlyout = new PrivacyPolicySettingsFlyout();
					customSettingFlyout.Show();
				}));

			args.Request.ApplicationCommands.Add(new SettingsCommand(
				"InquirySettingsCommand", loader.GetString("InquiryEmail"), async handler =>
				{
					await Launcher.LaunchUriAsync(new Uri("mailto:youngjae@bapul.net"));
				}));
		}

		public static async Task updateUserName(TextBlock userName, Boolean signIn)
		{
			try
			{
				// Open Live Connect SDK client.
				LiveAuthClient LCAuth = new LiveAuthClient();
				LiveLoginResult LCLoginResult = await LCAuth.InitializeAsync();
				try
				{
					LiveLoginResult loginResult = null;
					if (signIn)
					{
						// Sign in to the user's Microsoft account with the required scope.
						//  
						//  This call will display the Microsoft account sign-in screen if 
						//   the user is not already signed in to their Microsoft account 
						//   through Windows 8.
						// 
						//  This call will also display the consent dialog, if the user has 
						//   has not already given consent to this app to access the data 
						//   described by the scope.
						// 
						//  Change the parameter of LoginAsync to include the scopes 
						//   required by your app.
						loginResult = await LCAuth.LoginAsync(new string[] { "wl.basic" });
					}
					else
					{
						// If we don't want the user to sign in, continue with the current 
						//  sign-in state.
						loginResult = LCLoginResult;
					}
					if (loginResult.Status == LiveConnectSessionStatus.Connected)
					{
						// Create a client session to get the profile data.
						LiveConnectClient connect = new LiveConnectClient(LCAuth.Session);

						// Get the profile info of the user.
						LiveOperationResult operationResult = await connect.GetAsync("me");
						dynamic result = operationResult.Result;
						if (result != null)
						{
							// Update the text of the object passed in to the method. 
							userName.Text = string.Join(" ", "Hello", result.name, "!");
						}
						else
						{
							// Handle the case where the user name was not returned. 
						}
					}
					else
					{
						// The user hasn't signed in so display this text 
						//  in place of his or her name.
						userName.Text = "You're not signed in.";
					}
				}
				catch (LiveAuthException exception)
				{
					// Handle the exception. 
				}
			}
			catch (LiveAuthException exception)
			{
				// Handle the exception. 
			}
			catch (LiveConnectException exception)
			{
				// Handle the exception. 
			}
		}

	}
}
