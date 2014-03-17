using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769
using Microsoft.Live;

namespace LiveBoard.View
{
	// http://msdn.microsoft.com/en-us/library/live/hh968445.aspx#settings 참고.

	public sealed partial class AccountSettingsFlyout : SettingsFlyout
	{
		public AccountSettingsFlyout()
		{
			this.InitializeComponent();

			SetNameField(false);
			//Task.Run(async () =>
			//{
			//	await SetNameField(false);
			//});
		}

		private async void SignInButton_OnClick(object sender, RoutedEventArgs e)
		{
			await SetNameField(true);
		}

		private async void SignOutButton_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				// Initialize access to the Live Connect SDK.
				LiveAuthClient LCAuth = new LiveAuthClient();
				LiveLoginResult LCLoginResult = await LCAuth.InitializeAsync();
				// Sign the user out, if he or she is connected;
				//  if not connected, skip this and just update the UI
				if (LCLoginResult.Status == LiveConnectSessionStatus.Connected)
				{
					LCAuth.Logout();
				}

				// At this point, the user should be disconnected and signed out, so
				//  update the UI.
				var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
				this.UserNameTextBlock.Text = loader.GetString("MicrosoftAccount/Text");

				// Show sign-in button.
				SignInButton.Visibility = Visibility.Visible;
				SignOutButton.Visibility = Visibility.Collapsed;
			}
			catch (LiveConnectException x)
			{
				// Handle exception.
			}
		}

		private async Task SetNameField(Boolean login)
		{
			// If login == false, just update the name field. 
			await App.updateUserName(this.UserNameTextBlock, login);

			// Test to see if the user can sign out.
			Boolean userCanSignOut = true;

			var LCAuth = new LiveAuthClient();
			LiveLoginResult LCLoginResult = await LCAuth.InitializeAsync();

			if (LCLoginResult.Status == LiveConnectSessionStatus.Connected)
			{
				userCanSignOut = LCAuth.CanLogout;
			}

			var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

			if (String.IsNullOrEmpty(UserNameTextBlock.Text) 
				|| UserNameTextBlock.Text.Equals(loader.GetString("MicrosoftAccount/Text")))
			{
				// Show sign-in button.
				SignInButton.Visibility = Visibility.Visible;
				SignOutButton.Visibility = Visibility.Collapsed;
			}
			else
			{
				// Show sign-out button if they can sign out.
				SignOutButton.Visibility = userCanSignOut ? Visibility.Visible : Visibility.Collapsed;
				SignInButton.Visibility = Visibility.Collapsed;
			}
		}
	}
}
