//
// Bubble.cs: Provides both a UITableViewCell that can be used with UITableViews
// as well as a ChatBubble which is a MonoTouch.Dialog Element that can be used
// inside a DialogViewController for quick UIs.
//
// Author:
//   Miguel de Icaza
//
using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using Yousful;
using System.Drawing;

namespace Yousful.Calendar
{
	public class BubbleCell : UITableViewCell {
		public static NSString KeyLeft = new NSString ("BubbleElementLeft");
		public static NSString KeyRight = new NSString ("BubbleElementRight");
		public static UIImage bleft, bright, left, right; 
		public static UIFont titleFont = UIFont.SystemFontOfSize (22);
		public static UIFont font = UIFont.SystemFontOfSize (14);
		UIView view;
		UIView imageView;
		UILabel title,description,dateLabel,coordLabel;
		//bool isLeft;
		BL.Event eventData; 
		string EventId;
		SizeF size;

		static BubbleCell ()
		{
			bright = UIImage.FromFile ("Assets/green.png");
			//bleft = UIImage.FromFile ("Assets/grey.png");

			// buggy, see https://bugzilla.xamarin.com/show_bug.cgi?id=6177
			//left = bleft.CreateResizableImage (new UIEdgeInsets (10, 16, 18, 26));
			//right = bright.CreateResizableImage (new UIEdgeInsets (11, 11, 17, 18));
			//left = bleft.StretchableImage (26, 16);
			right = bright.StretchableImage (11, 11);
		}

		public BubbleCell (BL.Event data) : base (UITableViewCellStyle.Default,data.ID.ToString())
		{
			var rect = new RectangleF (0, 0, 1, 1);
			this.eventData = data;
			this.EventId = this.eventData.ID.ToString();
			view = new UIView (rect);
			imageView = new UIImageView (right);
			view.AddSubview (imageView);
			//title
			var textRect = new RectangleF (0, 0, 1, 1);

			title = new UILabel (textRect) {
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
				Font = titleFont,
				BackgroundColor = UIColor.Clear
			};
			imageView.AddSubview (title);

			//description
			description = new UILabel (textRect) {
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
				Font = font,
				BackgroundColor = UIColor.Clear
			};
			imageView.AddSubview (description); 

			//time
			dateLabel = new UILabel (textRect) {
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
				Font = font,
				BackgroundColor = UIColor.Clear
			};
			imageView.AddSubview(dateLabel);

			//coords
			coordLabel =new UILabel (textRect){
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
				Font = font,
				BackgroundColor = UIColor.Clear
			};
			imageView.AddSubview(coordLabel);


			ContentView.Add (view);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			var frame = ContentView.Frame;
			//var size = GetSizeForText (this, label.Text) + BubblePadding;
			size = new SizeF (200, 200);
			SizeF imageSize = new SizeF (200, 200);
			imageView.Frame = new RectangleF (new PointF ( frame.Width-imageSize.Width-10, 0), imageSize);
			view.SetNeedsDisplay ();
			frame = imageView.Frame;
			//label.Frame = new RectangleF (new PointF (frame.X + ( 8), frame.Y + 6), size-BubblePadding);

			//title
			var titleSize = GetSizeForText (this,titleFont, title.Text);
			//title.Frame = new RectangleF (new PointF (frame.X+(frame.Width/2)-(titleSize.Width/2),frame.Y), titleSize );
			title.Frame = new RectangleF (new PointF (10,0), titleSize );
			//title.BackgroundColor = UIColor.Blue;
			var descriptionSize = GetSizeForText (this,font, description.Text);
			//description.Frame = new RectangleF(new PointF(frame.X+(frame.Width/2)-(titleSize.Width/2),frame.Y - descriptionSize.Height-titleSize.Height ), titleSize );
			description.Frame = new RectangleF (new PointF (10,frame.Y + titleSize.Height), descriptionSize );

			var dateSize = GetSizeForText (this,font, dateLabel.Text);
			dateLabel.Frame = new RectangleF(new PointF(10,frame.Y + titleSize.Height+descriptionSize.Height), dateSize );

			var coordSize = GetSizeForText (this,font, coordLabel.Text);
			coordLabel.Frame = new RectangleF(new PointF(10,frame.Y + titleSize.Height+descriptionSize.Height+dateSize.Height), coordSize );
		}

		static internal SizeF BubblePadding = new SizeF (22, 16);

		static internal SizeF GetSizeForText (UIView tv,UIFont fontToSize, string text)
		{
			return tv.StringSize (text, fontToSize, new SizeF (tv.Bounds.Width*.7f-32, 99999));
		}

		public void Update ()
		{
			title.Text = this.eventData.Name;
			description.Text = this.eventData.Notes;
			dateLabel.Text = this.eventData.Date;
			coordLabel.Text = this.eventData.Lat.ToString () + "," + this.eventData.Long.ToString ();
			SetNeedsLayout ();
		}
	}

	public class CalendarEventEntry : Element, IElementSizing {
		//bool isLeft;
		string EventId;
		BL.Event myEvent;

		public CalendarEventEntry (BL.Event eventData) : base (eventData.ID.ToString())
		{
			this.EventId = eventData.ID.ToString();
			this.myEvent = eventData;
		}


		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (this.EventId) as BubbleCell;
			if (cell == null)
				cell = new BubbleCell (this.myEvent);
			cell.Update ();
			return cell;
		}

		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			//SizeF test = BubbleCell.GetSizeForText (tableView, Caption);
			//return size.Height;// + BubbleCell.BubblePadding.Height;
			return 225;
		}
	}
}
