using System;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using Yousful.Calendar;

namespace Yousful.Calendar
{
	[Register ("EventCreatePage")]
	public class EventCreatePage : UIViewController
	{
		DialogViewController discussion;
		RootElement root;
		UIView discussionHost;
		UITextView entry;
		UIImageView chatBar;
		UIImageView headerBar;
		UIButton sendButton;

		EntryElement nameEntry;
		EntryElement descEntry;
		EntryElement dateEntry;

		const float messageFontSize = 16;
		const float maxContentHeight = 84;
		const int entryHeight = 40;
		float previousContentHeight;

		NSObject showObserver, hideObserver;
		RootViewController parentController;
		BL.Event currentEvent;
		int EventId;

		public EventCreatePage (RootViewController p, RootElement root, int eventId)
		{
			this.parentController = p;
			this.root = root;
			this.EventId = eventId;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//NavigationController.NavigationBar.Translucent = false;
			var bounds = View.Bounds;

			var backgroundColor =  new UIColor (0.859f, 0.866f, 0.929f, 1);

			View.BackgroundColor = backgroundColor;
			//
			// Add the bubble chat interface
			//
			discussionHost = new UIView (new RectangleF (bounds.X, bounds.Y+20, bounds.Width, bounds.Height-20)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				AutosizesSubviews = true,
				UserInteractionEnabled = true
			};
			View.AddSubview (discussionHost);

			discussion = new DialogViewController (UITableViewStyle.Plain, root, true);	
			discussionHost.AddSubview (discussion.View);
			discussion.View.BackgroundColor = backgroundColor;


			nameEntry = new EntryElement ("", "Name the Event?", "");
			//descEntry = new EntryElement ("Description", "Describe it!", "");
			//dateEntry = new EntryElement ("Date", "When is it?", "");
			discussion.Root[0].Add (nameEntry);
			//discussion.Root [0].Add (dateEntry);
			//discussion.Root[0].Add (descEntry);
			// 
			// Add styled entry
			//
			/*headerBar = new UIImageView (new RectangleF (0, 0, bounds.Width, entryHeight)) {
				ClearsContextBeforeDrawing = false,
				AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth,
				Image = UIImage.FromFile ("Assets/ChatBar.png").StretchableImage (18, 20),
				UserInteractionEnabled = true
			};
			View.AddSubview (headerBar);*/

			chatBar = new UIImageView (new RectangleF (0, bounds.Height-entryHeight, bounds.Width, entryHeight)) {
				ClearsContextBeforeDrawing = false,
				AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth,
				Image = UIImage.FromFile ("Assets/GreyBar.png").StretchableImage (18, 20),
				UserInteractionEnabled = true
			};
			View.AddSubview (chatBar);

			/*
			entry = new UITextView (new RectangleF (10, 9, 234, 22)) {
				ContentSize = new SizeF (234, 22),
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				ScrollEnabled = true,
				ScrollIndicatorInsets = new UIEdgeInsets (5, 0, 4, -2),
				ClearsContextBeforeDrawing = false,
				Font = UIFont.SystemFontOfSize (messageFontSize),
				DataDetectorTypes = UIDataDetectorType.All,
				BackgroundColor = UIColor.Clear,
			};

			// Fix a scrolling glitch
			entry.ShouldChangeText = (textView, range, text) => {
				entry.ContentInset = new UIEdgeInsets (0, 0, 3, 0);
				return true;
			};
			previousContentHeight = entry.ContentSize.Height;
			chatBar.AddSubview (entry);
*/
			// 
			// The send button
			//
			sendButton = UIButton.FromType (UIButtonType.Custom);
			sendButton.ClearsContextBeforeDrawing = false;
			sendButton.Frame = new RectangleF (chatBar.Frame.Width/2, 8, 74, 26);
			sendButton.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;

			var sendBackground = UIImage.FromFile ("Assets/green.png");
			sendButton.SetBackgroundImage (sendBackground, UIControlState.Normal);
			sendButton.SetBackgroundImage (sendBackground, UIControlState.Disabled);
			sendButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize (16);
			sendButton.TitleLabel.Frame = new RectangleF (new PointF (0,0), sendBackground.Size );
			sendButton.TitleLabel.ShadowOffset = new SizeF (0, -1);
			sendButton.SetTitle ("Save", UIControlState.Normal);
			sendButton.SetTitleShadowColor (new UIColor (0.325f, 0.463f, 0.675f, 1), UIControlState.Normal);
			sendButton.AddTarget (CreateCalenderEvent , UIControlEvent.TouchUpInside);
			//DisableSend ();
			chatBar.AddSubview (sendButton);


			//
			// Listen to keyboard notifications to animate
			//
			showObserver = UIKeyboard.Notifications.ObserveWillShow (PlaceKeyboard);
			hideObserver = UIKeyboard.Notifications.ObserveWillHide (PlaceKeyboard);

			ScrollToBottom (false);
			// Track changes in the entry to resize the view accordingly
			//entry.Changed += HandleEntryChanged;
		}

		public override void ViewDidUnload ()
		{
			showObserver.Dispose ();
			hideObserver.Dispose ();
			discussion = null;
			discussionHost = null;
			root = null;
			entry = null;
			chatBar = null;

			base.ViewDidUnload ();
		}

		void EnableSend ()
		{
			sendButton.Enabled = true;
			sendButton.TitleLabel.Alpha = 1;
		}

		void DisableSend ()
		{
			sendButton.Enabled = false;
			sendButton.TitleLabel.Alpha = 0.5f;
		}

		void CreateCalenderEvent(object sender, EventArgs args){
			//Console.WriteLine ("Name2: " + nameEntry.Value);
			//nameEntry.FetchValue ();
			//descEntry.FetchValue ();
			//dateEntry.FetchValue ();

			/*Console.WriteLine ("Name2: " + nameEntry.Value);
			BL.Event currentEvent = new BL.Event();
			currentEvent.Name = nameEntry.Value;
			currentEvent.Date = dateEntry.Value;
			currentEvent.Notes = descEntry.Value;
			BL.Managers.EventManager.SaveEvent(currentEvent);

			this.GoToCalendarPage ();*/
			//this.NavigationController.PopToRootViewController (true);
			this.parentController.ShowMapPage (Yousful.Map.MapPage.Mode.CREATE_NEW);
		}

		void GoToCalendarPage(){
			//this.discussion.RemoveFromParentViewController
			//this.RemoveFromParentViewController ();
			//this.discussion.PresentViewController (this.discussion.dialo, true, null);
			//this.discussion.DismissViewController (true, null);
			this.NavigationController.PopToRootViewController (true);
		}

		// Just show messages alternating
		void SendMessage (object sender, EventArgs args)
		{		
			//discussion.Root [0].Add (new CalendarEventEntry ("TEST"));
			entry.Text = "";
			ScrollToBottom (true);
		}

		//
		// Resizes the UITextView based on the current text
		//
		void HandleEntryChanged (object sender, EventArgs e)
		{
			var contentHeight = entry.ContentSize.Height - messageFontSize + 2;
			if (entry.HasText){
				if (contentHeight != previousContentHeight){
					if (contentHeight <= maxContentHeight){
						SetChatBarHeight (contentHeight + 18);
						if (previousContentHeight > maxContentHeight)
							entry.ScrollEnabled = false;
						entry.ContentOffset = new PointF (0, 6);
					} else if (previousContentHeight <= maxContentHeight){
						entry.ScrollEnabled = true;
						entry.ContentOffset = new PointF (0, contentHeight-68);
						if (previousContentHeight < maxContentHeight){
							ExpandChatBarHeight ();
						}
					}
				}
			} else {
				if (previousContentHeight > 22){
					ResetChatBarHeight ();
					if (previousContentHeight > maxContentHeight)
						entry.ScrollEnabled = false;
				}
				AdjustEntry ();
			}
			if (entry.Text != "") 
				EnableSend ();
			else
				DisableSend ();

			previousContentHeight = contentHeight;

		}

		// Resizes the chat bar to the specified height
		void SetChatBarHeight (float height)
		{
			var chatFrame = discussion.View.Frame;
			chatFrame.Height = View.Frame.Height-height;
			discussion.View.Frame = chatFrame;

			UIView.BeginAnimations ("");
			UIView.SetAnimationDuration (.3);
			discussion.View.Frame = chatFrame;
			chatBar.Frame = new RectangleF (chatBar.Frame.X, chatFrame.Height, chatFrame.Width, height);
			UIView.CommitAnimations ();
		}

		// Sets the default height
		void ResetChatBarHeight ()
		{
			SetChatBarHeight (entryHeight);
		}

		// Sets the maximum height
		void ExpandChatBarHeight ()
		{
			SetChatBarHeight (94);
		}

		// Adjusts the UITextView after an update
		void AdjustEntry ()
		{
			// This fixes a rendering glitch
			//entry.ContentOffset = new PointF (0, 6);
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		// 
		// Custom layout: when our discussionHost changes, so does the discussion's view
		//
		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			discussion.View.Frame = discussionHost.Frame;
		}

		// 
		// When the keyboard appears, animate the new position for the entry
		// and scroll the chat to the bottom
		//
		void PlaceKeyboard (object sender, UIKeyboardEventArgs args)
		{
			UIView.BeginAnimations (""); {
				UIView.SetAnimationCurve (args.AnimationCurve);
				UIView.SetAnimationDuration (args.AnimationDuration);
				var viewFrame = View.Frame;
				var endRelative = View.ConvertRectFromView (args.FrameEnd, null);
				viewFrame.Height = endRelative.Y;
				View.Frame = viewFrame;
			} UIView.CommitAnimations ();

			ScrollToBottom (true);
			AdjustEntry ();
		}

		void ScrollToBottom (bool animated)
		{
			int row = discussion.Root [0].Elements.Count-1;
			if (row == -1)
				return;
			discussion.TableView.ScrollToRow (NSIndexPath.FromRowSection (row, 0), UITableViewScrollPosition.Bottom, true);
		}

		public override bool AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers {
			get {
				return true;
			}
		}
	}
}

